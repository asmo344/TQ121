//
// FILENAME.
//      FPSAPI - FPS Sensor Moudle API
//
//      $PATH:
//
// FUNCTIONAL DESCRIPTION.
//      Interface of FPS Sensor Module Application 
//
//
// MODIFICATION HISTORY.
//
// NOTICE.
//

#include "stdafx.h"
#include "ModuleDef.h"   
#include "fps_stru.h"
#include "fpsapi.h"
#include "usbemu.h"

#include "cis_cmd.h"
#include "ModuleDef.h"

#include <iostream>
#include "OpticalSensor.h"

VendorDeviceHandle   MAIN_FPDevice[30];
DeviceInfoDesciptor  FPS_DeviceInfor;
DeviceInfo FPSDeviceInfo;

BYTE otgversion;
BYTE _devicecount;

//
// FUNCTION NAME.
//      OpenFPSSensorDevice
//
// FUNCTIONAL DESCRIPTION.
//      Open FPS sensor device
//
// ENTRY PARAMETERS.
//      DeviceCount - in/out parameter
//
// EXIT PARAMETERS.
//      Function Return          ¡V FPS API status code.
//

int OpenFPSSensorDevice(
	BYTE   *DeviceCount              // Total Device ,Output
	)
{
	int iStatus = 0;

#if DEVICE_IS_USB_STORAGE == 1
	iStatus = USBStorageEnumerate(MAIN_FPDevice, &FPS_DeviceInfor, &_devicecount);
#endif
	*DeviceCount = _devicecount;
	if (iStatus == 1)
	{
		std::cout << "Get the EGIS USB device " << std::endl;
		otgversion = 1;
	}
	else if (iStatus == 2)
	{
		//sprintf_s(buf, "Get the YINS USB device\r\n");
		BYTE mData;
		ReadECFAOneCISRegister(&_devicecount, 0xFA, &mData); // Get FW version
		//std::cout << "mFWversion =  " << (int)mData << std::endl;
		if (mData == 0)
			otgversion = 0x01; // Old 446FW
		else if (mData < 101)
			otgversion = 0x02; // ET728 446FW
		else
			otgversion = 0x00; // FW9600 446FW
	}

	return iStatus;
}


//
// FUNCTION NAME.
//      WriteOneCISRegister
//
// FUNCTIONAL DESCRIPTION.
//      write one fps register into device 
//
// ENTRY PARAMETERS.
//      DeviceCount
//      Data
//
// EXIT PARAMETERS.
//      Function Return          ¡V FPS API status code.
//

int WriteOneCISRegister(
	BYTE   *DeviceCount,             // Device Count
	DWORD   Address,                 // register address 
	BYTE   Data                      // register data 		
	)
{

	WORD length;
	BYTE* buffer;

	//
	// Setting Initial Data
	//
	length  = MAX_SCSI_DATA_PACKET_SIZE;
	buffer = new BYTE[length];
	memset(buffer, 0, length);

	buffer[0] = 0x1;

	buffer[1] = (Address & 0xff00) >> 0x8;
	buffer[2] = (Address & 0x00ff);
	buffer[3] = Data;

	CIS_FPSSetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, buffer);
	return FUNCTION_SUCCESS;
}

int WriteTY7868Register(
	BYTE   *DeviceCount,             // Device Count
	BYTE    Page,                    // Page
	DWORD   Address,                 // register address 
	BYTE   Data                      // register data 		
)
{
	WORD length;
	BYTE* buffer;

	//
	// Setting Initial Data
	//
	length = MAX_SCSI_DATA_PACKET_SIZE;
	buffer = new BYTE[length];
	memset(buffer, 0, length);

	buffer[0] = 0x1;
	buffer[1] = Page;
	buffer[2] = (Address & 0xff00) >> 0x8;
	buffer[3] = (Address & 0x00ff);
	buffer[4] = Data;

	CIS_FPSSetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, buffer);
	delete[](buffer);
	return FUNCTION_SUCCESS;
}

int WriteECFAOneCISRegister(
	BYTE   *DeviceCount,             // Device Count
	DWORD   Address,                 // register address 
	BYTE   Data                      // register data 		
)
{

	WORD length;
	BYTE* buffer;

	//
	// Setting Initial Data
	//
	length = MAX_SCSI_DATA_PACKET_SIZE;
	buffer = new BYTE[length];
	memset(buffer, 0, length);

	buffer[0] = 0x1;

	/*buffer[1] = (Address & 0xff00) >> 0x8;
	buffer[2] = (Address & 0x00ff);
	buffer[3] = Data;*/
	buffer[1] = 0xC0;
	buffer[2] = (Address & 0xff00) >> 0x8;
	buffer[3] = (Address & 0x00ff);
	buffer[4] = Data;

	CIS_FPSSetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, buffer);
	return FUNCTION_SUCCESS;
}


//
// FUNCTION NAME.
//      ReadOneCISRegister
//
// FUNCTIONAL DESCRIPTION.
//      read one CMOS sensor register from device 
//
// ENTRY PARAMETERS.
//      DeviceCount
//      Data
//
// EXIT PARAMETERS.
//      Function Return          ¡V FPS API status code.
//

int ReadOneCISRegister(
	BYTE   *DeviceCount,             // Total Device ,Output
	DWORD   Address,                 // register address
	BYTE   *Data                     // register data 		
	)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	ret_data[0] = (Address & 0xff00) >> 0x8;
	ret_data[1] = Address & 0x00ff;
	FPS_STATUS iStatus;

	iStatus = CIS_FPSGetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	Data[0] = ret_data[0];
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

int ReadTY7868Register(
	BYTE   *DeviceCount,             // Total Device ,Output
	BYTE    Page,                    // Page
	DWORD   Address,                 // register address
	BYTE   *Data                     // register data 		
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	ret_data[0] = Page;
	ret_data[1] = (Address & 0xff00) >> 0x8;
	ret_data[2] = Address & 0x00ff;
	FPS_STATUS iStatus;

	iStatus = CIS_FPSGetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	Data[0] = ret_data[0];
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

int ReadECFAOneCISRegister(
	BYTE   *DeviceCount,             // Total Device ,Output
	DWORD   Address,                 // register address
	BYTE   *Data                     // register data 		
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	/*ret_data[0] = (Address & 0xff00) >> 0x8;
	ret_data[1] = Address & 0x00ff;*/
	ret_data[0] = 0xC0;
	ret_data[1] = (Address & 0xff00) >> 0x8;
	ret_data[2] = Address & 0x00ff;
	FPS_STATUS iStatus;

	iStatus = CIS_FPSGetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	Data[0] = ret_data[0];
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

int ReadLedRegister(
	BYTE   *DeviceCount,             // Total Device ,Output
	BYTE   Address,                 // register address
	BYTE   *Data                     // register data 		
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	ret_data[0] = 0xD0;
	ret_data[1] = Address;
	FPS_STATUS iStatus;

	iStatus = CIS_FPSGetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	Data[0] = ret_data[0];
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

int WriteLedRegister(
	BYTE   *DeviceCount,             // Device Count
	BYTE   Address,                  // register address 
	BYTE   Data                      // register data 		
)
{

	WORD length;
	BYTE* buffer;

	//
	// Setting Initial Data
	//
	length = MAX_SCSI_DATA_PACKET_SIZE;
	buffer = new BYTE[length];
	memset(buffer, 0, length);

	buffer[0] = 0x1;
	if (Address == 0xA5 && Data == 0x5A)
	{
		buffer[1] = 0xD6;
	}
	else
	{
		buffer[1] = 0xD0;
	}
	buffer[2] = Address;
	buffer[3] = Data;
	CIS_FPSSetRegisters(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, buffer);
	return FUNCTION_SUCCESS;
}

//
// FUNCTION NAME.
//      ProcessStandby
//
// FUNCTIONAL DESCRIPTION.
//      wake up chip or enter sleep mode 
//
// ENTRY PARAMETERS.
//      DeviceCount
//      Data
//
// EXIT PARAMETERS.
//      Function Return          ¡V FPS API status code.
//

int ProcessStandby(
	BYTE   *DeviceCount,             // Total Device ,Output
	DWORD   Address                 // register address 		
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	ret_data[0] = (Address & 0xff00) >> 0x8;
	ret_data[1] = Address & 0x00ff;
	FPS_STATUS iStatus;

	iStatus = CIS_FPSProcessStandby(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}


//
// FUNCTION NAME.
//      CheckDataReady
//
// FUNCTIONAL DESCRIPTION.
//      read OTG fwversion from device 
//
// ENTRY PARAMETERS.
//      DeviceCount
//      Data
//
// EXIT PARAMETERS.
//      Function Return          ¡V FPS API status code.
//

int CheckDataReady(
	BYTE   *DeviceCount,             // Total Device ,Output
	BYTE   *Data                     // data 		
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	FPS_STATUS iStatus;

	iStatus = CIS_FPSGetOTGver(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	Data[0] = ret_data[0];
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

//
// FUNCTION NAME.
//      ReadFullImageData
//
// FUNCTIONAL DESCRIPTION.
//      Read a full image data 
//
// ENTRY PARAMETERS.
//      DeviceCount
//      pImageData
//
// EXIT PARAMETERS.
//      Function Return          ¡V FPS API status code.
//

int ReadFullImageData(
	BYTE   *DeviceCount,             // Total Device ,Output
	BYTE   page_num,
	BYTE   page_size,
	BYTE   *pImageData               // image data
	)
{
	FPS_STATUS iStatus;

	iStatus = FPSGetFramesB(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, page_num, page_size, pImageData);

	return iStatus;
}

int StartCaptureImage(
	BYTE  *DeviceCount,             // Total Device ,Output
	int image_width,
	int image_height,
	BYTE bit_num
	)
{
	FPS_STATUS iStatus;

	iStatus = FPS_Start_Capture(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, image_width, image_height, bit_num);
	return iStatus;
}

int StartTestPattern(
	BYTE  *DeviceCount,             // Total Device ,Output
	int image_width,
	int image_height,
	BYTE bit_num
)
{
	FPS_STATUS iStatus;

	iStatus = FPS_Start_TestPattern(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, image_width, image_height, bit_num);
	return iStatus;
}

//
// FUNCTION NAME.
//      ProcessRST
//
// FUNCTIONAL DESCRIPTION.
//      RESET SENSOR 
//
// ENTRY PARAMETERS.
//      DeviceCount
//      Data
//
// EXIT PARAMETERS.
//      Function Return          ¡V FPS API status code.
//

int ProcessRst(
	BYTE   *DeviceCount             // Total Device ,Output	
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	FPS_STATUS iStatus;

	iStatus = CIS_FPSProcessRst(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

/* T7805JA */
int UsbTransmit(
	BYTE* DeviceCount,
	BYTE* DataBuffer,
	USHORT BufferLength,
	BYTE SCSI_CMD
)
{
	FPS_STATUS iStatus;
	iStatus = UsbTransmit(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, DataBuffer, BufferLength, SCSI_CMD);
	return iStatus;
}

int UsbTransmitReceive(
	BYTE* DeviceCount,
	BYTE* DataBuffer,
	USHORT BufferLength,
	BYTE SCSI_CMD
	)
{
	FPS_STATUS iStatus;
	BYTE ret_data[512];
	USHORT length = min(sizeof(ret_data), BufferLength);
	memcpy(ret_data, DataBuffer, length);
	iStatus = UsbTransmitReceive(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, ret_data, SCSI_CMD);
	memcpy(DataBuffer, ret_data, length);
	return iStatus;
}

int GetFwVer(
	BYTE* DeviceCount,			// Total Device ,Output
	BYTE* Data					// register data 		
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	FPS_STATUS iStatus;

	iStatus = GetFwVer(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	Data[0] = ret_data[0];
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

int SpiMode(
	BYTE* DeviceCount,			// Total Device ,Output
	BYTE* Mode,
	BYTE* Clk
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	ret_data[0] = *Mode & 0xff;
	ret_data[1] = *Clk & 0xff;
	FPS_STATUS iStatus;

	iStatus = spiMode(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	*Mode = ret_data[0];
	*Clk = ret_data[1];
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}

int GpioSetup(
	BYTE* DeviceCount,           // Total Device, Output
	BYTE  Cmd,                   // select gpio cmd
	BOOL  State                  // 0: Low, 1: High	 
)
{
	int length;
	length = 512;
	BYTE ret_data[512];

	ret_data[0] = Cmd & 0xff;
	ret_data[1] = State & 0xff;

	FPS_STATUS iStatus;

	iStatus = CIS_FPSProcessRst(&MAIN_FPDevice[*DeviceCount], &FPSDeviceInfo, CONTINU_RW, length, ret_data);
	if (iStatus != OK)
	{
		return ERROR_FAIL_SET_CIS_REG_BYTE;
	}
	return FUNCTION_SUCCESS;
}