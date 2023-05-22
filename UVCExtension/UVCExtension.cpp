// UVCExtension.cpp : 定義 DLL 應用程式的匯出函式。
//

#include "stdafx.h"
#include <mfapi.h>
#include <mfplay.h>
#include <mfreadwrite.h>
#include <vector>
#include <string>
#include <ks.h>
#include <ksproxy.h>
#include <vidcap.h>
#include "UVCExtension.h"
#include <stdio.h>
#include <iostream>

using namespace UVCEXTENSION;

// GUID of the extension unit, {ACB6890C-A3B3-4060-8B9A-DF34EEF39A2E}
static const GUID xuGuidAN75779 =
{ 0xacb6890c, 0xa3b3, 0x4060,{ 0x8b, 0x9a, 0xdf, 0x34, 0xee, 0xf3, 0x9a, 0x2e } };

//Media foundation and DSHOW specific structures, class and variables
IMFMediaSource *pVideoSource = NULL;
IMFAttributes *pVideoConfig = NULL;
IMFActivate **ppVideoDevices = NULL;
IMFSourceReader *pVideoReader = NULL;

DWORD _xuID;
IKsControl *_ks_control = NULL;
//Other variables
UINT32 noOfVideoDevices = 0;
WCHAR *szFriendlyName = NULL;
UINT32 DeviceId = 0;

HRESULT CreateVideoDeviceSource(IMFMediaSource **ppSource)
{
	*ppSource = NULL;

	IMFMediaSource *pSource = NULL;

	size_t returnValue;
	CHAR videoDevName[20][MAX_PATH];
	// Create an attribute store to specify the enumeration parameters.
	HRESULT hr = MFCreateAttributes(&pVideoConfig, 1);
	if (FAILED(hr))
	{
		goto done;
	}

	// Source type: video capture devices
	hr = pVideoConfig->SetGUID(
		MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
		MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
	);
	if (FAILED(hr))
	{
		goto done;
	}

	// Enumerate devices.
	UINT32 count;
	hr = MFEnumDeviceSources(pVideoConfig, &ppVideoDevices, &count);
	if (FAILED(hr))
	{
		goto done;
	}

	if (count == 0)
	{
		hr = E_FAIL;
		goto done;
	}

	if (DeviceId == 0xFFFFFFFF)
	{
		for (UINT32 i = 0; i < count; i++)
		{
			//Get the device names
			UINT32 cchName;

			HRESULT hr = ppVideoDevices[i]->GetAllocatedString(
				MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME,
				&szFriendlyName, &cchName);
			if (FAILED(hr))
			{
				goto done;
			}

			wcstombs_s(&returnValue, videoDevName[i], MAX_PATH, szFriendlyName, MAX_PATH);
			//printf("%d: %s\n", i, videoDevName[i]);

			//Store the App note firmware (Whose name is *FX3*) device index  
			if (!(strcmp(videoDevName[i], "FX3")) || !(strcmp(videoDevName[i], "CX3-UVC")))
				DeviceId = i;
		}
	}

	// Create the media source object.
	hr = ppVideoDevices[DeviceId]->ActivateObject(IID_PPV_ARGS(&pSource));
	if (FAILED(hr))
	{
		goto done;
	}
	else
	{
		char buffer[MAX_PATH];
		FILE *file;
		int idx = 0;
		errno_t err = fopen_s(&file, "Connected.txt", "w");

		memset(buffer, '\0', sizeof(buffer));

		sprintf_s(buffer, "%s\n", videoDevName[DeviceId]);
		for (int i = 0; i < sizeof(buffer); i++)
		{
			if (buffer[i] == '\n')
			{
				idx = i + 1;
				break;
			}
		}
		fwrite(buffer, 1, idx, file);
		_fcloseall();
	}

	*ppSource = pSource;
	(*ppSource)->AddRef();

done:
	SafeRelease(&pVideoConfig);

	for (DWORD i = 0; i < count; i++)
	{
		SafeRelease(&ppVideoDevices[i]);
	}
	CoTaskMemFree(ppVideoDevices);
	SafeRelease(&pSource);
	return hr;
}

HRESULT FindExtensionUnit()
{
	HRESULT hr = S_OK;
	GUID pNodeType;
	IUnknown *unKnown;
	IKsControl *ks_control = _ks_control;
	IKsTopologyInfo * pKsTopologyInfo = NULL;
	KSP_NODE kspNode;

	hr = pVideoSource->QueryInterface(__uuidof(IKsTopologyInfo), (void **)&pKsTopologyInfo);
	CHECK_HR_RESULT(hr, "IMFMediaSource::QueryInterface(IKsTopologyInfo)");

	// Retrieve the number of nodes in the filter
	DWORD numNodes = 0;
	if ((hr = pKsTopologyInfo->get_NumNodes(&numNodes)) != S_OK || numNodes == 0)
	{
		//logger(LOG_ERROR) << "UVCXU: Could not get number of nodes in the given IKsTopologyInfo for video input device '" << usb->id() << "'" << std::endl;
		return hr;
	}

	// Find the extension unit node that corresponds to the given GUID
	ULONG bytesReturned = 0;

	for (unsigned int i = 0; i < numNodes; i++)
	{
		hr = pKsTopologyInfo->get_NodeType(i, &pNodeType);
		CHECK_HR_RESULT(hr, "IKsTopologyInfo->get_NodeType(...)");

		//if (IsEqualGUID(pNodeType, xuGuidAN75779))
		{
			if ((hr = pKsTopologyInfo->CreateNodeInstance(i, IID_IUnknown, (LPVOID *)&unKnown)) == S_OK)
			{
				kspNode.Property.Set = xuGuidAN75779;
				kspNode.Property.Id = 0;
				kspNode.Property.Flags = KSPROPERTY_TYPE_SETSUPPORT | KSPROPERTY_TYPE_TOPOLOGY;
				kspNode.NodeId = i;
				kspNode.Reserved = 0;

				hr = unKnown->QueryInterface(__uuidof(IKsControl), (void **)&ks_control);

				if ((hr = ((IKsControl *)ks_control)->KsProperty((PKSPROPERTY)&kspNode, sizeof(kspNode), NULL, 0, &bytesReturned)) != S_OK)
				{
					SafeRelease(&ks_control);
					continue;
				}

				//SafeRelease(&ks_control);
				_ks_control = ks_control;
				_xuID = i;
				break;
			}
		}
	}
	return hr;
done:
	SafeRelease(&ks_control);
	return hr;
}

UVC::UVC()
{
	HRESULT hr = S_OK;
	DeviceId = 0xFFFFFFFF;

	hr = CreateVideoDeviceSource(&pVideoSource);
	if (FAILED(hr))
	{
		std::cout << "CreateVideoDeviceSource Fail" << std::endl;
		return;
	}
	hr = FindExtensionUnit();
	if (FAILED(hr))
	{
		std::cout << "FindExtensionUnit Fail" << std::endl;
		DeviceId = 0xFFFFFFFF;
	}
}

void UVC::EnumDev()
{
	FILE *file;
	errno_t err;
	char buffer[MAX_PATH];
	CHAR videoDevName[20][MAX_PATH];
	IMFMediaSource *pSource = NULL;
	size_t returnValue;

	// Create an attribute store to specify the enumeration parameters.
	HRESULT hr = MFCreateAttributes(&pVideoConfig, 1);
	if (FAILED(hr))
	{
		std::cout << "MFCreateAttributes Fail" << std::endl;
		goto done;
	}

	// Source type: video capture devices
	hr = pVideoConfig->SetGUID(
		MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
		MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
	);
	if (FAILED(hr))
	{
		std::cout << "pVideoConfig->SetGUID Fail" << std::endl;
		goto done;
	}

	// Enumerate devices.
	UINT32 count;
	hr = MFEnumDeviceSources(pVideoConfig, &ppVideoDevices, &count);
	if (FAILED(hr))
	{
		std::cout << "MFEnumDeviceSources Fail" << std::endl;
		goto done;
	}

	if (count == 0)
	{
		std::cout << "E_FAIL" << std::endl;
		hr = E_FAIL;
		goto done;
	}

	memset(buffer, '\0', sizeof(buffer));
	err = fopen_s(&file, "Device.txt", "w");
	for (UINT32 i = 0; i < count; i++)
	{
		//Get the device names
		GetVideoDeviceFriendlyNames(i);
		wcstombs_s(&returnValue, videoDevName[i], MAX_PATH, szFriendlyName, MAX_PATH);

		std::cout << videoDevName[i] << std::endl;
		//Store the App note firmware (Whose name is *FX3*) device index 
		int idx = 0;
		if (!(strcmp(videoDevName[i], "FX3")) || !(strcmp(videoDevName[i], "CX3-UVC")))
		{
			sprintf_s(buffer, "%d,%s\n", i, videoDevName[i]);
			for (int i = 0; i < sizeof(buffer); i++)
			{
				if (buffer[i] == '\n')
				{
					idx = i + 1;
					break;
				}
			}
			fwrite(buffer, 1, idx, file);
		}
	}
	_fcloseall();

done:
	SafeRelease(&pVideoConfig);

	for (DWORD i = 0; i < count; i++)
	{
		SafeRelease(&ppVideoDevices[i]);
	}
	CoTaskMemFree(ppVideoDevices);
	SafeRelease(&pSource);
}

bool UVC::ConnectDev(UINT32 devId)
{
	HRESULT hr = S_OK;

	DeviceId = devId;

	hr = CreateVideoDeviceSource(&pVideoSource);
	if (FAILED(hr))
	{
		std::cout << "CreateVideoDeviceSource Fail" << std::endl;
		return false;
	}
	hr = FindExtensionUnit();
	if (FAILED(hr))
	{
		std::cout << "FindExtensionUnit Fail" << std::endl;
		return false;
	}
	return true;
}

DWORD UVC::isInitialized()
{
	return _xuID;
}

bool UVC::RegRead(uint8_t deviceid, uint8_t *address, uint8_t *value)
{
	if (!isInitialized())
		return false;

	BYTE data[6] = { XU_RD_2A1D, deviceid, address[0], address[1], 0, 0};
	bool res = mXuSetCur(CY_FX_UVC_XU_REG_RW, data);
	if (!res)
		return res;

	res = mXuGetCur(CY_FX_UVC_XU_REG_RW, data);
	if (res)
	{
		value[0] = data[0];
	}
	return res;
}

bool UVC::RegWrite(uint8_t deviceid, uint8_t *address, uint8_t *value)
{
	if (!isInitialized())
		return false;

	BYTE data[6] = { XU_WR_2A1D, deviceid, address[0], address[1], value[0], value[1] };

	return mXuSetCur(CY_FX_UVC_XU_REG_RW, data);
}

bool UVC::RegBurstRead(uint8_t type, uint8_t deviceID, uint16_t address, uint8_t* value, uint8_t burstLength)
{
	if (!isInitialized())
		return false;

	BYTE addrL, addrH;
	BYTE valueL, valueH;
	addrL = address & 0xFF;
	addrH = (address & 0xFF00) >> 8;
	BYTE data[] = { type, deviceID, addrL, addrH, 0, 0 , burstLength };

	bool res = mXuSetCur(CY_FX_UVC_XU_FIRMWARE_VERSION_CONTROL, data);
	if (!res)
		return res;

	res = mXuGetCur(CY_FX_UVC_XU_FIRMWARE_VERSION_CONTROL, value);
	return res;
}

bool UVC::RegWrite(uint8_t deviceid, uint16_t address, uint8_t value)
{
	if (!isInitialized())
		return false;

	BYTE addrL, addrH;
	addrL = address & 0xFF;
	addrH = (address & 0xFF00) >> 8;
	BYTE data[6] = { XU_RD_2A1D, deviceid, addrL, addrH,value ,0 };

	return mXuSetCur(CY_FX_UVC_XU_REG_RW, data);
}

uint8_t UVC::RegRead(uint8_t deviceid, uint16_t address)
{
	if (!isInitialized())
		return false;

	BYTE addrL, addrH;
	addrL = address & 0xFF;
	addrH = (address & 0xFF00) >> 8;
	BYTE data[6] = { XU_RD_2A1D, deviceid, addrL, addrH, 0, 0 };
	bool res = mXuSetCur(CY_FX_UVC_XU_REG_RW, data);
	if (!res)
		return res;

	res = mXuGetCur(CY_FX_UVC_XU_REG_RW, data);
	if (res)
	{
		return data[0];
	}
}

uint16_t UVC::RegRead(uint8_t type, uint8_t deviceid, uint16_t address)
{
	if (!isInitialized())
		return false;

	BYTE addrL, addrH;
	BYTE valueL, valueH;
	addrL = address & 0xFF;
	addrH = (address & 0xFF00) >> 8;
	BYTE data[6] = { type, deviceid, addrL, addrH, 0, 0 };
	uint16_t value;

	bool res = mXuSetCur(CY_FX_UVC_XU_REG_RW, data);
	if (!res)
		return res;

	res = mXuGetCur(CY_FX_UVC_XU_REG_RW, data);

	value = (data[1] << 8) + data[0];
	return value;
}

bool UVC::RegWrite(uint8_t type, uint8_t deviceid, uint16_t address, uint16_t value)
{
	if (!isInitialized())
		return false;

	BYTE addrL, addrH;
	BYTE valueL, valueH;
	addrL = address & 0xFF;
	addrH = (address & 0xFF00) >> 8;
	valueL = value & 0xFF;
	valueH = (value & 0xFF00) >> 8;

	BYTE data[6] = { type, deviceid, addrL, addrH, valueL, valueH };

	return mXuSetCur(CY_FX_UVC_XU_REG_RW, data);
}

bool UVC::FwVerGet(uint8_t* value)
{
	if (!isInitialized())
		return false;

	return mXuGetCur(CY_FX_UVC_XU_FIRMWARE_VERSION_CONTROL, value);
}

bool UVC::WriteFpagImage(byte image[])
{
	if (!isInitialized())
		return false;

	return mXuSetCur(CY_FX_UVC_XU_FIRMWARE_VERSION_CONTROL, image);
}

bool UVC::TransferData(byte buf[], int len)
{
	if (!isInitialized())
		return false;
	
	return mXuSetCur(CY_FX_UVC_XU_REG_RW, buf);
}

bool UVC::ReceiveData(byte* buf, int len)
{
	if (!isInitialized())
		return false;

	return mXuGetCur(CY_FX_UVC_XU_REG_RW, buf);
}

bool UVC::mXuSetCur(uint8_t id, Byte *data)
{
	KSP_NODE ExtensionProp;
	ULONG bytesReturned = 0;
	IKsControl *ks_control = _ks_control;

	ExtensionProp.Property.Set = xuGuidAN75779;
	ExtensionProp.Property.Id = id;
	ExtensionProp.Property.Flags = KSPROPERTY_TYPE_SET | KSPROPERTY_TYPE_TOPOLOGY;
	ExtensionProp.NodeId = _xuID;

	if (ks_control->KsProperty((PKSPROPERTY)&ExtensionProp, sizeof(ExtensionProp), (PVOID)data, 512, &bytesReturned) != S_OK)
	{
		return false;
	}

	return true;
}

bool UVC::mXuGetCur(uint8_t id, Byte *data)
{
	KSP_NODE ExtensionProp;
	ULONG bytesReturned = 0;
	IKsControl *ks_control = _ks_control;

	ExtensionProp.Property.Set = xuGuidAN75779;
	ExtensionProp.Property.Id = id;
	ExtensionProp.Property.Flags = KSPROPERTY_TYPE_GET | KSPROPERTY_TYPE_TOPOLOGY;
	ExtensionProp.NodeId = _xuID;

	if (ks_control->KsProperty((PKSPROPERTY)&ExtensionProp, sizeof(ExtensionProp), (PVOID)data, 512, &bytesReturned) != S_OK)
	{
		return false;
	}

	return true;
}

UINT32 UVC::GetDeviceId()
{
	return DeviceId;
}

//Function to get UVC video devices
HRESULT GetVideoDevices()
{
	CoInitializeEx(NULL, COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);
	MFStartup(MF_VERSION);

	// Create an attribute store to specify the enumeration parameters.
	HRESULT hr = MFCreateAttributes(&pVideoConfig, 1);
	CHECK_HR_RESULT(hr, "Create attribute store");

	// Source type: video capture devices
	hr = pVideoConfig->SetGUID(
		MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE,
		MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID
	);
	CHECK_HR_RESULT(hr, "Video capture device SetGUID");

	// Enumerate devices.
	hr = MFEnumDeviceSources(pVideoConfig, &ppVideoDevices, &noOfVideoDevices);
	CHECK_HR_RESULT(hr, "Device enumeration");

done:
	return hr;
}

//Function to get device friendly name
HRESULT GetVideoDeviceFriendlyNames(int deviceIndex)
{
	// Get the the device friendly name.
	UINT32 cchName;

	HRESULT hr = ppVideoDevices[deviceIndex]->GetAllocatedString(
		MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME,
		&szFriendlyName, &cchName);
	CHECK_HR_RESULT(hr, "Get video device friendly name");

done:
	return hr;
}

//Function to initialize video device
HRESULT InitVideoDevice(int deviceIndex)
{
	HRESULT hr = ppVideoDevices[deviceIndex]->ActivateObject(IID_PPV_ARGS(&pVideoSource));
	CHECK_HR_RESULT(hr, "Activating video device");

	// Create a source reader.
	hr = MFCreateSourceReaderFromMediaSource(pVideoSource, pVideoConfig, &pVideoReader);
	CHECK_HR_RESULT(hr, "Creating video source reader");

done:
	return hr;
}

//Function to set/get parameters of UVC extension unit
HRESULT SetGetExtensionUnit(GUID xuGuid, DWORD dwExtensionNode, ULONG xuPropertyId, ULONG flags, void *data, int len, ULONG *readCount)
{
	GUID pNodeType;
	IUnknown *unKnown;
	IKsControl * ks_control = NULL;
	IKsTopologyInfo * pKsTopologyInfo = NULL;
	KSP_NODE kspNode;

	HRESULT hr = pVideoSource->QueryInterface(__uuidof(IKsTopologyInfo), (void **)&pKsTopologyInfo);
	CHECK_HR_RESULT(hr, "IMFMediaSource::QueryInterface(IKsTopologyInfo)");

	hr = pKsTopologyInfo->get_NodeType(dwExtensionNode, &pNodeType);
	CHECK_HR_RESULT(hr, "IKsTopologyInfo->get_NodeType(...)");

	hr = pKsTopologyInfo->CreateNodeInstance(dwExtensionNode, IID_IUnknown, (LPVOID *)&unKnown);
	CHECK_HR_RESULT(hr, "ks_topology_info->CreateNodeInstance(...)");

	hr = unKnown->QueryInterface(__uuidof(IKsControl), (void **)&ks_control);
	CHECK_HR_RESULT(hr, "ks_topology_info->QueryInterface(...)");

	kspNode.Property.Set = xuGuid;              // XU GUID
	kspNode.NodeId = (ULONG)dwExtensionNode;   // XU Node ID
	kspNode.Property.Id = xuPropertyId;         // XU control ID
	kspNode.Property.Flags = flags;             // Set/Get request

	hr = ks_control->KsProperty((PKSPROPERTY)&kspNode, sizeof(kspNode), (PVOID)data, len, readCount);
	CHECK_HR_RESULT(hr, "ks_control->KsProperty(...)");

done:
	SafeRelease(&ks_control);
	return hr;
}

int TestUVC()
{
	HRESULT hr = S_OK;
	CHAR enteredStr[MAX_PATH], videoDevName[20][MAX_PATH];
	UINT32 selectedVal = 0xFFFFFFFF;
	ULONG flags, readCount;
	size_t returnValue;
	BYTE an75779FwVer[5] = { 2, 2, 12, 20, 18 }; //Write some random value

	//Get all video devices
	GetVideoDevices();

	printf("Video Devices connected:\n");
	for (UINT32 i = 0; i < noOfVideoDevices; i++)
	{
		//Get the device names
		GetVideoDeviceFriendlyNames(i);
		wcstombs_s(&returnValue, videoDevName[i], MAX_PATH, szFriendlyName, MAX_PATH);
		printf("%d: %s\n", i, videoDevName[i]);

		//Store the App note firmware (Whose name is *FX3*) device index  
		if (!(strcmp(videoDevName[i], "FX3")))
			selectedVal = i;
	}

	//Specific to UVC extension unit in AN75779 appnote firmware
	if (selectedVal != 0xFFFFFFFF)
	{
		printf("\nFound \"FX3\" device\n");

		//Initialize the selected device
		InitVideoDevice(selectedVal);

		/*printf("\nSelect\n1. To Set Firmware Version \n2. To Get Firmware version\nAnd press Enter\n");
		fgets(enteredStr, MAX_PATH, stdin);
		fflush(stdin);
		selectedVal = atoi(enteredStr);

		if (selectedVal == 1)
			flags = KSPROPERTY_TYPE_SET | KSPROPERTY_TYPE_TOPOLOGY;
		else
			flags = KSPROPERTY_TYPE_GET | KSPROPERTY_TYPE_TOPOLOGY;*/

		flags = KSPROPERTY_TYPE_GET | KSPROPERTY_TYPE_TOPOLOGY;
		printf("\nTrying to invoke UVC extension unit...\n");

		if (!SetGetExtensionUnit(xuGuidAN75779, 2, 1, flags, (void*)an75779FwVer, 5, &readCount))
		{
			printf("Found UVC extension unit\n");

			if (flags == (KSPROPERTY_TYPE_SET | KSPROPERTY_TYPE_TOPOLOGY))
			{
				printf("\nAN75779 Firmware Version Set to %d.%d (Major.Minor), Build Date: %d/%d/%d (MM/DD/YY)\n\n", an75779FwVer[0], an75779FwVer[1],
					an75779FwVer[2], an75779FwVer[3], an75779FwVer[4]);
			}
			else
			{
				printf("\nCurrent AN75779 Firmware Version is %d.%d (Major.Minor), Build Date: %d/%d/%d (MM/DD/YY)\n\n", an75779FwVer[0], an75779FwVer[1],
					an75779FwVer[2], an75779FwVer[3], an75779FwVer[4]);
			}
		}

		/*flags = KSPROPERTY_TYPE_GET | KSPROPERTY_TYPE_TOPOLOGY;
		if (!SetGetExtensionUnit(xuGuidAN75779, 2, 1, flags, (void*)an75779FwVer, 5, &readCount))
		{
			printf("Found UVC extension unit\n");

			if (flags == (KSPROPERTY_TYPE_SET | KSPROPERTY_TYPE_TOPOLOGY))
			{
				printf("\nAN75779 Firmware Version Set to %d.%d (Major.Minor), Build Date: %d/%d/%d (MM/DD/YY)\n\n", an75779FwVer[0], an75779FwVer[1],
					an75779FwVer[2], an75779FwVer[3], an75779FwVer[4]);
			}
			else
			{
				printf("\nCurrent AN75779 Firmware Version is %d.%d (Major.Minor), Build Date: %d/%d/%d (MM/DD/YY)\n\n", an75779FwVer[0], an75779FwVer[1],
					an75779FwVer[2], an75779FwVer[3], an75779FwVer[4]);
			}
		}*/
	}
	else
	{
		printf("\nDid not find \"FX3\" device (AN75779 firmware)\n\n");
	}

	//Release all the video device resources
	for (UINT32 i = 0; i < noOfVideoDevices; i++)
	{
		SafeRelease(&ppVideoDevices[i]);
	}
	CoTaskMemFree(ppVideoDevices);
	SafeRelease(&pVideoConfig);
	SafeRelease(&pVideoSource);
	CoTaskMemFree(szFriendlyName);

	printf("\nExiting App in 5 sec...");
	Sleep(5000);
	
	return an75779FwVer[0];
}
