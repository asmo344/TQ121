#include "stdafx.h"

#include "ClrOpticalSensor.h"
#include "fpsapi.h"

using namespace ClrOpticalSensor;

int OpticalSensor::ClrOpenFPSSensorDevice(BYTE *DeviceCount)
{
     return OpenFPSSensorDevice(DeviceCount);
}

int OpticalSensor::ClrWriteOneCISRegister(BYTE *DeviceCount, DWORD Address, BYTE Data)
{
	return WriteOneCISRegister(DeviceCount, Address, Data);
}

int OpticalSensor::ClrWriteTY7868Register(BYTE *DeviceCount, BYTE Page, DWORD Address, BYTE Data)
{
	return WriteTY7868Register(DeviceCount, Page, Address, Data);
}

int OpticalSensor::ClrWriteECFAOneCISRegister(BYTE *DeviceCount, DWORD Address, BYTE Data)
{
	return WriteECFAOneCISRegister(DeviceCount, Address, Data);
}

int OpticalSensor::ClrReadOneCISRegister(BYTE *DeviceCount, DWORD Address, BYTE *Data)
{
	return ReadOneCISRegister(DeviceCount, Address, Data);
}

int OpticalSensor::ClrReadTY7868Register(BYTE *DeviceCount, BYTE Page, DWORD Address, BYTE *Data)
{
	return ReadTY7868Register(DeviceCount, Page, Address, Data);
}

int OpticalSensor::ClrReadECFAOneCISRegister(BYTE *DeviceCount, DWORD Address, BYTE *Data)
{
	return ReadECFAOneCISRegister(DeviceCount, Address, Data);
}

int OpticalSensor::ClrReadFullImageData(BYTE *DeviceCount, BYTE page_num, BYTE page_size, BYTE *pImageData)
{
	return ReadFullImageData(DeviceCount, page_num, page_size, pImageData);
}

int OpticalSensor::ClrStartCaptureImage(BYTE *DeviceCount, int image_width, int image_height,
	                                 BYTE bit_num)
{
	return StartCaptureImage(DeviceCount, image_width, image_height, bit_num);
}

int OpticalSensor::ClrStartTestPattern(BYTE *DeviceCount, int image_width, int image_height,
	BYTE bit_num)
{
	return StartTestPattern(DeviceCount, image_width, image_height, bit_num);
}

int OpticalSensor::ClrCheckDataReady(BYTE *DeviceCount, BYTE *Data)
{
	return CheckDataReady(DeviceCount, Data);
}

int OpticalSensor::ClrProcessStandby(BYTE *DeviceCount, BYTE wakeorsleep)
{
	return ProcessStandby(DeviceCount, wakeorsleep);
}

int OpticalSensor::ClrReadLedI2C(BYTE *DeviceCount, BYTE Address, BYTE *Data)
{
	return ReadLedRegister(DeviceCount, Address, Data);
}

int OpticalSensor::ClrWriteLedI2C(BYTE *DeviceCount, BYTE Address, BYTE Data)
{
	return WriteLedRegister(DeviceCount, Address, Data);
}

int OpticalSensor::ClrProcessRst(BYTE *DeviceCount)
{
	return ProcessRst(DeviceCount);
}

int OpticalSensor::ClrGetImageAE(BYTE *DeviceCount, int* Data)
{
	return GetImageAE(Data);
}

/* T7805JA */
int OpticalSensor::ClrUsbTransmit(BYTE* DeviceCount, BYTE* DataBuffer, USHORT BufferLength, BYTE SCSI_CMD)
{
	return UsbTransmit(DeviceCount, DataBuffer, BufferLength, SCSI_CMD);
}

int OpticalSensor::ClrUsbTransmitReceive(BYTE* DeviceCount, BYTE* DataBuffer, USHORT BufferLength, BYTE SCSI_CMD)
{
	return UsbTransmitReceive(DeviceCount, DataBuffer, BufferLength, SCSI_CMD);
}

int OpticalSensor::ClrGetFwVer(BYTE* DeviceCount, BYTE* Data)
{
	return GetFwVer(DeviceCount, Data);
}

int OpticalSensor::ClrSpiMode(BYTE* DeviceCount, BYTE* spiMode, BYTE* clk)
{
	return SpiMode(DeviceCount, spiMode, clk);
}

int OpticalSensor::ClrGpioSetup(BYTE* DeviceCount, BYTE Cmd, bool State)
{
	return GpioSetup(DeviceCount, Cmd, State);
}