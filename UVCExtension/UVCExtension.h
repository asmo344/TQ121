#pragma once

#ifndef UVCEXTENSION_H
#define UVCEXTENSION_H

#include <mfapi.h>
#include <mfplay.h>
#include <mfreadwrite.h>
#include <vector>
#include <string>
#include <ks.h>
#include <ksproxy.h>
#include <vidcap.h>

using namespace System;

#define XU_RD_1A1D  		0x01
#define XU_RD_1A2D  		0x02
#define XU_RD_2A1D  		0x03
#define XU_RD_2A2D  		0x04
#define XU_WR_1A1D  		0x05
#define XU_WR_1A2D  		0x06
#define XU_WR_2A1D  		0x07
#define XU_WR_2A2D  		0x08

#define CY_FX_UVC_XU_FIRMWARE_VERSION_CONTROL 1
#define CY_FX_UVC_XU_REG_RW 2

//Macro to check the HR result
#define CHECK_HR_RESULT(hr, msg, ...) if (hr != S_OK) {printf("info: Function: %s, %s failed, Error code: 0x%.2x \n", __FUNCTION__, msg, hr, __VA_ARGS__); goto done; }

//Templates for the App
template <class T> void SafeRelease(T **ppT)
{
	if (*ppT)
	{
		(*ppT)->Release();
		*ppT = NULL;
	}
}

//Function to get UVC video devices
extern HRESULT GetVideoDevices();

//Function to get device friendly name
extern HRESULT GetVideoDeviceFriendlyNames(int deviceIndex);

//Function to initialize video device
extern HRESULT InitVideoDevice(int deviceIndex);

//Function to set/get parameters of UVC extension unit
extern HRESULT SetGetExtensionUnit(GUID xuGuid, DWORD dwExtensionNode, ULONG xuPropertyId, ULONG flags, void *data, int len, ULONG *readCount);

namespace UVCEXTENSION
{
	public ref class UVC
	{
	protected:
		//IKsControl *_ks_control = NULL;
		DWORD isInitialized();
		bool mXuSetCur(uint8_t id, Byte *data);
		bool mXuGetCur(uint8_t id, Byte *data);
	public:
		//int TestUVC();
		UVC();

		//virtual ~UVC();

		void EnumDev();
		bool ConnectDev(UINT32 devId);
		UINT32 GetDeviceId();
		bool RegWrite(uint8_t deviceid, uint8_t *address, uint8_t *value);
		bool RegRead(uint8_t deviceid, uint8_t *address, uint8_t *value);
		bool RegWrite(uint8_t deviceid, uint16_t address, uint8_t value);
		uint8_t RegRead(uint8_t deviceid, uint16_t address);
		uint16_t RegRead(uint8_t type, uint8_t deviceid, uint16_t address);
		bool RegWrite(uint8_t type, uint8_t deviceid, uint16_t address, uint16_t value);
		/* Function    : FwVerGet
			 Parameters  :
				  value[0] - Build Year
				  value[1] - Build month
				  value[2] - Build day
				  value[3] - Major version
				  value[4] - Minor version
		*/
		bool RegBurstRead(uint8_t type, uint8_t deviceID, uint16_t address, uint8_t* value, uint8_t burstLength);
		bool FwVerGet(uint8_t* value);
		bool WriteFpagImage(byte image[]);
		bool TransferData(byte buf[], int len);
		bool ReceiveData(byte* buf, int len);
	};
}

#endif