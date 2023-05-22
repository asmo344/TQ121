#include "stdafx.h"
#include "usbemu.h"
#include "fpsapi.h"

#include <winioctl.h>
#include <initguid.h>
#include <setupapi.h>
#include <iostream>
#include <string>
#include <algorithm>

char *g_VendorDevicePath_EGIS = "usbstor#disk&ven_egist&prod_fpset505";     // global vendor device
//char *g_VendorDevicePath = "USBSTOR\Disk&Ven_EGIST&Prod_FPSET505";
char *g_VendorDevicePath_YINS = "usbstor#disk&ven_yins&prod_fpset505";
char* g_VendorDevicePath_TY7805 = "usbstor#disk&ven_tyrafos&prod";

#define MAX_DEVICE		16
#define INTERFACE_DETAIL_SIZE	(1024)

DEFINE_GUID(GUID_CLASS_USB_DEVICE, 0xA5DCBF10L, 0x6530, 0x11D2, 0x90, 0x1F, 0x00, 0xC0, 0x4F, 0xB9, 0x51, 0xED);
DEFINE_GUID(GUID_DEVINTERFACE_DISK, 0x53F56307, 0xB6BF, 0x11D0, 0x94, 0xF2, 0x00, 0xA0, 0xC9, 0x1E, 0xFB, 0x8B);
static const GUID OSR_DEVICE_INTERFACE =
{ 0x53F56307, 0xB6BF, 0x11D0, { 0x94, 0xF2, 0x00, 0xA0, 0xC9, 0x1E, 0xFB, 0x8B } };

int USBDevice = -1;

BOOL NameCampare(char* name1, char* name2);

BOOL DevicePathMapToMountpoint(CHAR* vendorDevicePath, CHAR* mountPoint)
{
	BOOL status = false;
	int sz = 65536;
	CHAR* physical;
	CHAR* logical;

	physical = (char*)malloc(sz);
	logical = (char*)malloc(sz);
	QueryDosDevice(NULL, physical, sz);

	//CHAR physical[65536];
	//CHAR logical[65536];
	//QueryDosDevice(NULL, physical, sizeof(physical));

	std::string temp;
	std::string device = vendorDevicePath;
	if (device.empty())
	{
		status = false;
		goto OUTPUT;
	}
	std::cout << "device: " << device << std::endl;

	if (physical != NULL && logical != NULL)
	{
		for (char* pos = physical; *pos; pos += strlen(pos) + 1)		
		{
			QueryDosDevice(pos, logical,sz);
			//std::cout << pos << " : \t" << logical << std::endl;
			std::string x = pos;
			transform(x.begin(), x.end(), x.begin(), tolower);
			if (x.find("storage#volume#") != std::string::npos && x.find(device) != std::string::npos)
			{
				temp = logical;
				status = true;
				break;
			}
			else
			{
				status = false;
			}
		}

		if (!status)
		{
			goto OUTPUT;
		}

		for (char* pos = physical; *pos; pos += strlen(pos) + 1)
		{
			QueryDosDevice(pos, logical, sz);
			//std::cout << pos << " : \t" << logical << std::endl;
			std::string x = pos;
			std::string y = logical;
			if (x.size() < 3 && y.find(temp) != std::string::npos)
			{
				temp = pos;
				status = true;
				break;
			}
			else
			{
				status = false;
			}
		}

		if (!status)
		{
			goto OUTPUT;
		}
	}
	else
	{
		status = false;
		goto OUTPUT;
	}

	if (status)
	{
		std::string dst = "\\\\.\\" + temp;
		std::cout << "dst: " << dst << std::endl;
		//std::cout << "len: " << dst.size() << std::endl;
		strcpy(mountPoint, dst.c_str());
		status = true;
		goto OUTPUT;
	}
	else
	{
		status = false;
		goto OUTPUT;
	}

OUTPUT:
	free(physical);
	free(logical);
	return status;
}

//
// FUNCTION NAME.
//      GetDeviceHandle
//
// FUNCTIONAL DESCRIPTION.
//      Get VID/PID device path, search all USB Device
//
// ENTRY PARAMETERS.
//      guidDeviceInterface
//
// EXIT PARAMETERS.
//      Function Return          ¡V EFI status code.
//      hDeviceHandle            -  Output,Ptr to Device Handle
//

BOOL GetDeviceHandle(GUID guidDeviceInterface, PHANDLE hDeviceHandle, int* otg_fw_ver, VendorDeviceHandle *VendorDeviceHandle, BYTE *DeviceTotal)
{
	if (guidDeviceInterface == GUID_NULL) {
		return FALSE;
	}

	BOOL bResult = TRUE;
	HDEVINFO hDeviceInfo;
	SP_DEVINFO_DATA DeviceInfoData;
	int compcount = 0;                // record the found device number

	SP_DEVICE_INTERFACE_DATA deviceInterfaceData;
	PSP_DEVICE_INTERFACE_DETAIL_DATA pInterfaceDetailData = NULL;

	ULONG requiredLength = 0;

	LPTSTR lpDevicePath = NULL;

	DWORD index = 0;

	CString strMsg;
	LPVOID lpMsgBuf;

	CHAR* vendorDevice;
	CHAR* mountpoint;
	int _devicecount = 0;
	//
	// Get information about all the installed devices for the specified
	// device interface class.
	//

	hDeviceInfo = SetupDiGetClassDevs(
		&guidDeviceInterface,
		NULL,
		NULL,
		DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

	if (hDeviceInfo == INVALID_HANDLE_VALUE) {
		goto done;
	}

	//
	//Enumerate all the device interfaces in the device information set.
	//
	DeviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
	for (index = 0; SetupDiEnumDeviceInfo(hDeviceInfo, index, &DeviceInfoData); index++)
	{
		//
		//Reset for this iteration
		//
		if (lpDevicePath)
		{
			LocalFree(lpDevicePath);
		}

		if (pInterfaceDetailData)
		{
			LocalFree(pInterfaceDetailData);
		}

		deviceInterfaceData.cbSize = sizeof(SP_INTERFACE_DEVICE_DATA);

		//
		//Get information about the device interface.
		//
		bResult = SetupDiEnumDeviceInterfaces(
			hDeviceInfo,
			&DeviceInfoData,
			&guidDeviceInterface,
			0,
			&deviceInterfaceData);

		//
		// Check if last item
		//
		if (GetLastError() == ERROR_NO_MORE_ITEMS)
		{
			break;
		}

		//Check for some other error
		if (!bResult)
		{
			goto done;
		}

		//
		//Interface data is returned in SP_DEVICE_INTERFACE_DETAIL_DATA
		//which we need to allocate, so we have to call this function twice.
		//First to get the size so that we know how much to allocate
		//Second, the actual call with the allocated buffer
		//
		bResult = SetupDiGetDeviceInterfaceDetail(
			hDeviceInfo,
			&deviceInterfaceData,
			NULL,
			0,
			&requiredLength,
			NULL);

		//
		//Check for some other error
		//
		if (!bResult) {
			if ((ERROR_INSUFFICIENT_BUFFER == GetLastError()) && (requiredLength > 0)) {
				//
				//we got the size, allocate buffer
				//
				pInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)LocalAlloc(LPTR, requiredLength);

				if (!pInterfaceDetailData) {
					goto done;
				}
			}
			else {
				goto done;
			}
		}

		//
		//get the interface detailed data
		//
		pInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

		//Now call it with the correct size and allocated buffer
		bResult = SetupDiGetDeviceInterfaceDetail(
			hDeviceInfo,
			&deviceInterfaceData,
			pInterfaceDetailData,
			requiredLength,
			NULL,
			&DeviceInfoData);

		//            
		//Check for some other error
		//
		if (!bResult) {
			printf("Error SetupDiGetDeviceInterfaceDetail: %d.\n", GetLastError());
			goto done;
		}

		if (NameCampare(pInterfaceDetailData->DevicePath, g_VendorDevicePath_EGIS))
		{
			compcount = 1;
			*otg_fw_ver = 1;
		}
		else if (NameCampare(pInterfaceDetailData->DevicePath, g_VendorDevicePath_TY7805))
		{
			compcount = 7805;
			USBDevice = EMU_GET_DEVICE_SUCCESS_TY7805;
			vendorDevice = g_VendorDevicePath_TY7805;
		}
		else if (NameCampare(pInterfaceDetailData->DevicePath, g_VendorDevicePath_YINS))
		{
			compcount = 1;
			*otg_fw_ver = 2;
		}
		else
		{
			continue;
		}

		VendorDeviceHandle[_devicecount].TargetId = index;
		_devicecount++;
	}
	//SetupDiDestroyDeviceInfoList(hDeviceInfo);

	for (index = 0; index < _devicecount; index++)
	{
		int idx = VendorDeviceHandle[index].TargetId;
		bool rst = SetupDiEnumDeviceInfo(hDeviceInfo, idx, &DeviceInfoData);
		if (!rst)
			std::cout << "SetupDiEnumDeviceInfo False " << std::endl;
		//
		//Reset for this iteration
		//
		if (lpDevicePath)
		{
			LocalFree(lpDevicePath);
		}

		if (pInterfaceDetailData)
		{
			LocalFree(pInterfaceDetailData);
		}

		deviceInterfaceData.cbSize = sizeof(SP_INTERFACE_DEVICE_DATA);

		//
		//Get information about the device interface.
		//
		bResult = SetupDiEnumDeviceInterfaces(
			hDeviceInfo,
			&DeviceInfoData,
			&guidDeviceInterface,
			0,
			&deviceInterfaceData);

		//
		// Check if last item
		//
		if (GetLastError() == ERROR_NO_MORE_ITEMS)
		{
			break;
		}

		//Check for some other error
		if (!bResult)
		{
			goto done;
		}

		//
		//Interface data is returned in SP_DEVICE_INTERFACE_DETAIL_DATA
		//which we need to allocate, so we have to call this function twice.
		//First to get the size so that we know how much to allocate
		//Second, the actual call with the allocated buffer
		//
		bResult = SetupDiGetDeviceInterfaceDetail(
			hDeviceInfo,
			&deviceInterfaceData,
			NULL,
			0,
			&requiredLength,
			NULL);

		//
		//Check for some other error
		//
		if (!bResult) {
			if ((ERROR_INSUFFICIENT_BUFFER == GetLastError()) && (requiredLength > 0)) {
				//
				//we got the size, allocate buffer
				//
				pInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)LocalAlloc(LPTR, requiredLength);

				if (!pInterfaceDetailData) {
					goto done;
				}
			}
			else {
				goto done;
			}
		}

		//
		//get the interface detailed data
		//
		pInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

		//Now call it with the correct size and allocated buffer
		bResult = SetupDiGetDeviceInterfaceDetail(
			hDeviceInfo,
			&deviceInterfaceData,
			pInterfaceDetailData,
			requiredLength,
			NULL,
			&DeviceInfoData);

		//            
		//Check for some other error
		//
		if (!bResult) {
			printf("Error SetupDiGetDeviceInterfaceDetail: %d.\n", GetLastError());
			goto done;
		}

		if (NameCampare(pInterfaceDetailData->DevicePath, g_VendorDevicePath_EGIS))
		{
			compcount = 1;
			*otg_fw_ver = 1;
		}
		else if (NameCampare(pInterfaceDetailData->DevicePath, g_VendorDevicePath_TY7805))
		{
			compcount = 7805;
			USBDevice = EMU_GET_DEVICE_SUCCESS_TY7805;
			vendorDevice = g_VendorDevicePath_TY7805;
		}
		else if (NameCampare(pInterfaceDetailData->DevicePath, g_VendorDevicePath_YINS))
		{
			compcount = 1;
			*otg_fw_ver = 2;
		}
		else
		{
			continue;
		}

		//    
		//copy device path
		//            
		size_t nLength = strlen(pInterfaceDetailData->DevicePath) + 1;
		lpDevicePath = (TCHAR *)LocalAlloc(LPTR, nLength * sizeof(TCHAR));
		//strcpy(lpDevicePath, pInterfaceDetailData->DevicePath);
		strcpy_s(lpDevicePath, nLength * sizeof(TCHAR), pInterfaceDetailData->DevicePath);
		lpDevicePath[nLength - 1] = 0;

		//
		//Open the device
		//
		mountpoint = (char*)malloc(6);
		if (DevicePathMapToMountpoint(vendorDevice, mountpoint))
		{
			std::cout << "Volume: " << mountpoint << std::endl;
			lpDevicePath = mountpoint;
		}
		std::cout << "lpDevicePath: " << lpDevicePath << std::endl;

		*hDeviceHandle = CreateFile(
			lpDevicePath,
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL,
			OPEN_EXISTING,
			0,
			NULL);

		if (*hDeviceHandle == INVALID_HANDLE_VALUE) {
			goto done;
		}

		VendorDeviceHandle[index].fileHandle = *hDeviceHandle;
		VendorDeviceHandle[index].OS_VERSION = Win2K_XP;
	}

	*DeviceTotal = _devicecount;

done:
	bResult = SetupDiDestroyDeviceInfoList(hDeviceInfo);

	if (compcount == 0) {
		bResult = FALSE;
	}
	return bResult;
}

//
// FUNCTION NAME.
//      OpenDevice
//
// FUNCTIONAL DESCRIPTION.
//      Get Device Handle from OpenDevice() 
//
// ENTRY PARAMETERS.
//      pszDevicePath - Input,Ptr to Device Path 
//
// EXIT PARAMETERS.
//      Function Return          ¡V EFI status code.
//

HANDLE OpenDevice(
	LPCTSTR pszDevicePath
	)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	HANDLE hDevice;

	hDevice = CreateFile(pszDevicePath,
		GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ | FILE_SHARE_WRITE,
		NULL,
		OPEN_EXISTING,
		0,
		NULL);

	return hDevice;
}

//
// FUNCTION NAME.
//      USBStorageEnumerate
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (int) - 
//

int USBStorageEnumerate(
	VendorDeviceHandle *VendorDeviceHandle,
	DeviceInfoDesciptor *DeviceInfo,
	BYTE *DeviceTotal
	)
{
	BYTE Buffer512[512];
	HANDLE hDevice;
	BOOL Status;
	int  otg_fw_ver;

	//
	// Search device by OSR_DEVICE_INTERFACE and Get the device handle
	//
	Status = GetDeviceHandle(OSR_DEVICE_INTERFACE, &hDevice, &otg_fw_ver, VendorDeviceHandle, DeviceTotal);
	if (Status == FALSE){
		return EMU_FALSE_NO_DEVICE_HANDLE;
	}

	//VendorDeviceHandle->fileHandle = hDevice;
	//VendorDeviceHandle->OS_VERSION = Win2K_XP;

	ZeroMemory(Buffer512, sizeof(Buffer512));

	if (otg_fw_ver == 1) {
		return EMU_GET_DEVICE_SUCCESS_EGIS;
	}
	else if (USBDevice != -1)
	{
		return USBDevice;
	}
	else {
		return EMU_GET_DEVICE_SUCCESS_YINS;
	}
}

BOOL NameCampare(char* name1, char* name2)
{
	BOOL ret = true;
	for (UINT i = 0; i < 20; i++)
	{
		if (name1[i + 4] != name2[i]) {
			ret = false;
			break;
		}
	}
	return ret;
}
