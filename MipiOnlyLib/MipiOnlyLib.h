// JIG_BOX.h

#pragma once

#ifndef MIPI_ONLY_DLL_H
#define MIPI_ONLY_DLL_H
#include <iostream>
#include <algorithm>
#include <fstream>
#include <string>
#include "../MipiOnlyLib/DTCCM2_SDK/dtccm2.h"
#include <comdef.h>
#pragma comment(lib, "../MipiOnlyLib/DTCCM2_SDK/dtccm2.lib")
using namespace System;
using namespace Runtime::InteropServices;

namespace MIPI_ONLY
{
public
	ref class MIPI_ONLY_CTRL
	{
	public:
		MIPI_ONLY_CTRL();
		static array<String ^> ^ Enumerate();
		static bool LoadConfig(wchar_t *filename);
		static bool InitialDevice(String ^ DevName, int DevNum);
		static int GrabSize();
		static byte *GrabImage();
		static byte *GrabRawData();
		static byte *GrabImageProcess(byte * pixels);
		static bool Close();
		static void ReadReg(UCHAR SlaveID, USHORT Address, USHORT* pData);
		static void WriteReg(UCHAR SlaveID, USHORT Address, USHORT Data);
		static void SetPinResetState(bool level);
		static void SetPinPowerDownState(bool level);
		static void GetPinState(bool *pwdn, bool *rst);
		static void SetAutoWhiteBalanceState(bool enable);
		static void GetAutoWhiteBalanceValue(bool *enable, float * gain_red, float * gain_green, float * gain_blue);
		static void SetDigitalGainValue(float gain_red, float gain_green, float gain_blue);
		static void SetDeadPixelsCleanState(bool enable, int HotCpth, int DeadCpth);
		static void SetGammaState(bool enable, int gamma);
		static void SetContrastState(bool enable, int contrast);
		static void SetSaturationState(bool enable, int saturation);
		static void SetSharpnessState(bool enable, int sharpness);
		static void SetNoiseReductionState(bool enable, int noise);
		static void SetGpioDirection(int pin, bool dir);
		static void SetGpioLevel(int pin, bool level);
		static void GetGpioLevel(int pin, bool *level);
		static void SetVoltage(int avdd, int dovdd, int dvdd);
		static void GetVoltage(int *avdd, int *dovdd, int *dvdd);

	private:
		static int m_DevID;
		static bool m_IsTV;
		static USHORT m_PreviewWidth;
		static USHORT m_PreviewHeight;
		static ULONG m_BufferSize;
		static int m_BufferMode;
		static ULONG m_UpLimitSize;
	};
}

#endif
