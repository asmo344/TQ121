#pragma once

using namespace System;
#include <windows.h>

namespace ClrOpticalSensor {
	public ref class OpticalSensor
	{
	public:
		static int ClrOpenFPSSensorDevice(
			BYTE *DeviceCount			// Total Device ,Output
		);

		static int ClrWriteOneCISRegister(
			BYTE *DeviceCount,			// Device Count
			DWORD Address,				// register address 
			BYTE  Data					// register data
		);

		static int ClrWriteTY7868Register(
			BYTE *DeviceCount,			// Device Count
			BYTE  page,                 // register page
			DWORD Address,				// register address 
			BYTE  Data					// register data
		);

		static int ClrWriteECFAOneCISRegister(
			BYTE *DeviceCount,			// Device Count
			DWORD Address,				// register address 
			BYTE  Data					// register data
		);

		static int ClrReadOneCISRegister(
			BYTE *DeviceCount,			// Total Device ,Output
			DWORD Address,				// register address
			BYTE *Data					// register data 
		);

		static int ClrReadTY7868Register(
			BYTE *DeviceCount,			// Total Device ,Output
			BYTE  page,                 // register page
			DWORD Address,				// register address
			BYTE *Data					// register data 
		);

		static int ClrReadECFAOneCISRegister(
			BYTE *DeviceCount,			// Total Device ,Output
			DWORD Address,				// register address
			BYTE *Data					// register data 
		);

		static int ClrReadFullImageData(
			BYTE *DeviceCount,          // Total Device ,Output
			BYTE  page_num,
			BYTE  page_size,
			BYTE *pImageData            // image data
		);

		static int ClrStartCaptureImage(
			BYTE *DeviceCount,          // Total Device ,Output
			int image_width,
			int image_height,
			BYTE bit_num
		);

		static int ClrStartTestPattern(
			BYTE *DeviceCount,          // Total Device ,Output
			int image_width,
			int image_height,
			BYTE bit_num
		);

		static int ClrCheckDataReady(
			BYTE *DeviceCount,			// Total Device ,Output
			BYTE *Data					// data 
		);

		static int ClrProcessStandby(
			BYTE *DeviceCount,			// Total Device, Output
			BYTE wakeorsleep			// wakeup or sleep selection
		);

		static int ClrProcessRst(
			BYTE *DeviceCount			// Total Device, Output
		);

		static int OpticalSensor::ClrReadLedI2C(
			BYTE *DeviceCount,          // Total Device ,Output
			BYTE Address,               // register address
			BYTE *Data                  // register data
		);

		static int OpticalSensor::ClrWriteLedI2C(
			BYTE *DeviceCount,			// Device Count
			BYTE  Address,				// register address 
			BYTE  Data					// register data
		);

		static int OpticalSensor::ClrGetImageAE(
			BYTE *DeviceCount,
			int* Data
		);

		/* T7805JA */
		static int ClrUsbTransmit(
			BYTE* DeviceCount,
			BYTE* DataBuffer,
			USHORT BufferLength,
			BYTE SCSI_CMD
		);

		static int ClrUsbTransmitReceive(
			BYTE* DeviceCount,
			BYTE* DataBuffer,
			USHORT BufferLength,
			BYTE SCSI_CMD
		);

		static int ClrGetFwVer(
			BYTE* DeviceCount,			// Total Device ,Output
			BYTE* Data					// data 
		);

		static int ClrSpiMode(
			BYTE* DeviceCount,			// Total Device ,Output
			BYTE* spiMode,
			BYTE* clk
		);

		static int ClrGpioSetup(
			BYTE* DeviceCount,			// Total Device, Output
			BYTE  Cmd,                  // select gpio cmd
			bool  State                 // false: Low, true: High
		);
	};
}
