//
// FILENAME.
//      fpsapi.h - 
//
//      $PATH:
//
// FUNCTIONAL DESCRIPTION.
//      
//
//
// MODIFICATION HISTORY.
//		2017.7.7   remodify it for optical sensor
//
// NOTICE.
//

#ifndef _FPSAPI_H
#define _FPSAPI_H

/*
 * STATUS RETURN 
 */
#define FUNCTION_SUCCESS                        1
#define DATA_IS_NOT_READY                       2
#define NO_USB_DEVICE                           3   
#define ERROR_FAIL_GET_SENSOR_DEVICE            0xFFF10000;
#define ERROR_FAIL_GET_ONE_IMAGE                0xFFF10001;
#define ERROR_FAIL_SET_CIS_REG_BYTE             0xFFF20000;

#define ERROR_NOT_IMPLANTATION                  0xE0001000;
#include <windows.h>

/*
 * Open FPS Device
 */
int OpenFPSSensorDevice(
	BYTE   *DeviceCount				// Total Device ,Output
	);


int WriteOneCISRegister(
	BYTE   *DeviceCount,			// Device Count
	DWORD   Address,				// register address 
	BYTE    Data					// register data 		
);

int WriteTY7868Register(
	BYTE   *DeviceCount,			// Device Count
	BYTE    Page,					// Page
	DWORD   Address,				// register address 
	BYTE    Data					// register data 		
);

int WriteECFAOneCISRegister(
	BYTE   *DeviceCount,			// Device Count
	DWORD   Address,				// register address 
	BYTE    Data					// register data 		
);

int ReadOneCISRegister(
	BYTE   *DeviceCount,			// Total Device ,Output
	DWORD   Address,				// register address
	BYTE   *Data					// register data 		
);

int ReadTY7868Register(
	BYTE   *DeviceCount,			// Total Device ,Output
	BYTE    Page,					// Page
	DWORD   Address,				// register address
	BYTE   *Data					// register data 	
);

int ReadECFAOneCISRegister(
	BYTE   *DeviceCount,			// Total Device ,Output
	DWORD   Address,				// register address
	BYTE   *Data					// register data 		
);

int ReadLedRegister(
	BYTE   *DeviceCount,			// Total Device ,Output
	BYTE   Address,				    // register address
	BYTE   *Data					// register data 	
);

int WriteLedRegister(
	BYTE   *DeviceCount,			// Total Device ,Output
	BYTE   Address,				    // register address
	BYTE   Data					// register data 	
);

/*
 * Read Full Image Data 
 */
int ReadFullImageData(
	BYTE   *DeviceCount,             // Total Device ,Output
	BYTE   page_num,
	BYTE   page_size,
	BYTE   *pImageData               // image data
	);

/*
 * start capture image data 
 */
int StartCaptureImage(
	BYTE  *DeviceCount,             // Total Device ,Output
	int image_width,
	int image_height,
	BYTE bit_num
	);

int StartTestPattern(
	BYTE  *DeviceCount,             // Total Device ,Output
	int image_width,
	int image_height,
	BYTE bit_num
);

int CheckDataReady(
	BYTE   *DeviceCount,			// Total Device ,Output
	BYTE   *Data					// register data 		
);

int ProcessStandby(
	BYTE   *DeviceCount,           // Total Device, Output
	DWORD	wakeorsleep
);

int ProcessRst(
	BYTE   *DeviceCount           // Total Device, Output
);

int GetImageAE(
	int* Data
);

/* T7805JA */
int UsbTransmit(
	BYTE* DeviceCount,
	BYTE* DataBuffer,
	USHORT BufferLength,
	BYTE SCSI_CMD
);

int UsbTransmitReceive(
	BYTE* DeviceCount,
	BYTE* DataBuffer,
	USHORT BufferLength,
	BYTE SCSI_CMD
);

int GetFwVer(
	BYTE* DeviceCount,			// Total Device ,Output
	BYTE* Data					// register data 		
);

int SpiMode(
	BYTE* DeviceCount,			// Total Device ,Output
	BYTE* Mode,
	BYTE* Clk
);

int GpioSetup(
	BYTE* DeviceCount,           // Total Device, Output
	BYTE  Cmd,                   // select gpio cmd
	BOOL  State                  // 0: Low, 1: High
);
#endif