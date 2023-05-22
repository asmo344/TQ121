// MipiOnlyLib.cpp : 定義 DLL 應用程式的匯出函式。
//

#include "stdafx.h"
#include "MipiOnlyLib.h"
#include <string>

using namespace MIPI_ONLY;

SensorTab m_SensorTab;
ULONG m_GrabSize;
byte *p_DynamicImageData = NULL;
byte *p_RawImageData = NULL;
byte *p_ProcessImageData = NULL;
FrameInfo m_FrameInfo;

MIPI_ONLY_CTRL::MIPI_ONLY_CTRL()
{
	m_DevID = -1;
	m_BufferSize = 0;
	m_BufferMode = 0;
	m_UpLimitSize = 16777216;
}

array<String ^> ^ MIPI_ONLY_CTRL::Enumerate()
{
	int i;
	int number = 0;
	int ret = 0;
	char *p_device_name[8] = {NULL};

	ret = EnumerateDevice(p_device_name, 8, &number);
	array<String ^> ^ DeviceList = gcnew array<String ^>(number);
	for (int i = 0; i < number; i++)
	{
		DeviceList[i] = gcnew String(p_device_name[i]);
	}

	return DeviceList;
}

bool MIPI_ONLY_CTRL::Close()
{
	CloseVideo(m_DevID);
	ResetSensorI2cBus(m_DevID);
	SensorEnable(m_SensorTab.pin ^ 3, 1, m_DevID);
	Sleep(50);
	SetSensorClock(0, 24 * 10, m_DevID);
	SENSOR_POWER power_source[] = {POWER_AVDD, POWER_DOVDD, POWER_DVDD, POWER_AFVCC, POWER_VPP2, POWER_OISVDD, POWER_AVDD2};
	const int power_count = sizeof(power_source) / sizeof(SENSOR_POWER);
	int power_voltage[power_count] = {0, 0, 0, 0, 0, 0, 0};
	int power_current_limit[power_count] = {200, 200, 200, 200, 200, 200, 200};
	BOOL power_state[power_count] = {true, true, true, true, true, true, true};
	CURRENT_RANGE power_current_range[power_count] = {
		CURRENT_RANGE_UA, CURRENT_RANGE_UA, CURRENT_RANGE_UA, CURRENT_RANGE_UA,
		CURRENT_RANGE_UA, CURRENT_RANGE_UA, CURRENT_RANGE_UA};
	PmuSetCurrentRange(power_source, power_current_range, power_count, m_DevID);
	Sleep(100);
	SensorEnable(m_SensorTab.pin, FALSE, m_DevID);
	PmuSetVoltage(power_source, power_voltage, power_count, m_DevID);
	Sleep(50);
	SetSoftPinPullUp(IO_NOPULL, m_DevID);
	CloseDevice(m_DevID);
	return true;
}

bool MIPI_ONLY_CTRL::MIPI_ONLY_CTRL::LoadConfig(wchar_t *filename)
{
	std::fstream file(filename);
	if (!file)
		return false;
	std::string section[] = {
		"[Sensor]",
		"[ParaList]",
		"[SleepParaList]",
		"[AF_InitParaList]",
		"[AF_AutoParaList]",
		"[AF_FarParaList]",
		"[AF_NearParaList]",
		"[Exposure_ParaList]",
		"[Gain_ParaList]",
		"[Quick_ParaList]"};
	int state = -1;
	std::string text;
	USHORT *ParaList = new USHORT[8192 * 4];
	USHORT *SleepParaList = new USHORT[2048];
	USHORT *AF_InitParaList = new USHORT[8192];
	USHORT *AF_AutoParaList = new USHORT[2048];
	USHORT *AF_FarParaList = new USHORT[2048];
	USHORT *AF_NearParaList = new USHORT[2048];
	USHORT *Exposure_ParaList = new USHORT[2048];
	USHORT *Gain_ParaList = new USHORT[2048];
	USHORT ParaListSize = 0, SleepParaListSize = 0, AF_InitParaListSize = 0, AF_AutoParaListSize = 0, AF_FarParaListSize = 0, AF_NearParaListSize = 0, Exposure_ParaListSize = 0, Gain_ParaListSize = 0;
	while (std::getline(file, text))
	{
		String ^ line = gcnew String(text.c_str());
		bool enter_section = false;
		if (line->Contains("//"))
			line = line->Remove(line->IndexOf("//"));
		if (line->Replace(" ", "") == "")
		{
			continue;
		}
		for (int i = 0; i < 10; i++)
		{
			if (line->Contains(gcnew String(section[i].c_str())))
			{
				state = i;
				if (state != 9)
					enter_section = true;
				break;
			}
		}
		if (enter_section)
			continue;
		if (state == 0)
		{
			line = line->Replace(" ", "");
			if (line->Contains("width="))
				m_SensorTab.width = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("height="))
				m_SensorTab.height = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("type="))
				m_SensorTab.type = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("port="))
				m_SensorTab.port = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("pin="))
				m_SensorTab.pin = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("SlaveID="))
				m_SensorTab.SlaveID = Convert::ToUInt16(line->Split('=')[1], 16);
			if (line->Contains("mode="))
				m_SensorTab.mode = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("FlagReg="))
				m_SensorTab.FlagReg = Convert::ToUInt16(line->Split('=')[1], 16);
			if (line->Contains("FlagMask="))
				m_SensorTab.FlagMask = Convert::ToUInt16(line->Split('=')[1], 16);
			if (line->Contains("FlagData="))
				m_SensorTab.FlagData = Convert::ToUInt16(line->Split('=')[1], 16);
			if (line->Contains("FlagReg1="))
				m_SensorTab.FlagReg1 = Convert::ToUInt16(line->Split('=')[1], 16);
			if (line->Contains("FlagMask1="))
				m_SensorTab.FlagMask1 = Convert::ToUInt16(line->Split('=')[1], 16);
			if (line->Contains("FlagData1="))
				m_SensorTab.FlagData1 = Convert::ToUInt16(line->Split('=')[1], 16);
			if (line->Contains("outformat="))
				m_SensorTab.outformat = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("mclk="))
				m_SensorTab.mclk = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("avdd="))
				m_SensorTab.avdd = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("avdd2="))
				m_SensorTab.avdd2 = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("dvdd="))
				m_SensorTab.dvdd = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("dovdd="))
				m_SensorTab.dovdd = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("AFVCC="))
				m_SensorTab.afvcc = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("VPP2="))
				m_SensorTab.vpp2 = Convert::ToUInt16(line->Split('=')[1]);
			if (line->Contains("OISVDD="))
				m_SensorTab.oisvdd = Convert::ToUInt16(line->Split('=')[1]);
			continue;
		}
		if (state == 1)
		{
			*(ParaList + ParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(ParaList + ParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			ParaListSize += 2;
			continue;
		}
		if (state == 2)
		{
			*(SleepParaList + SleepParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(SleepParaList + SleepParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			SleepParaListSize += 2;
			continue;
		}
		if (state == 3)
		{
			*(AF_InitParaList + AF_InitParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(AF_InitParaList + AF_InitParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			AF_InitParaListSize += 2;
			continue;
		}
		if (state == 4)
		{
			*(AF_AutoParaList + AF_AutoParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(AF_AutoParaList + AF_AutoParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			AF_AutoParaListSize += 2;
			continue;
		}
		if (state == 5)
		{
			*(AF_FarParaList + AF_FarParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(AF_FarParaList + AF_FarParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			AF_FarParaListSize += 2;
			continue;
		}
		if (state == 6)
		{
			*(AF_NearParaList + AF_NearParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(AF_NearParaList + AF_NearParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			AF_NearParaListSize += 2;
			continue;
		}
		if (state == 7)
		{
			*(Exposure_ParaList + Exposure_ParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(Exposure_ParaList + Exposure_ParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			Exposure_ParaListSize += 2;
			continue;
		}
		if (state == 8)
		{
			*(Gain_ParaList + Gain_ParaListSize) = Convert::ToUInt16(line->Split(',')[0], 16);
			*(Gain_ParaList + Gain_ParaListSize + 1) = Convert::ToUInt16(line->Split(',')[1], 16);
			Gain_ParaListSize += 2;
			continue;
		}
		if (ParaListSize)
		{
			m_SensorTab.ParaList = ParaList;
			m_SensorTab.ParaListSize = ParaListSize;
			if (SleepParaListSize)
			{
				m_SensorTab.SleepParaList = SleepParaList;
				m_SensorTab.SleepParaListSize = SleepParaListSize;
			}
			if (AF_InitParaListSize)
			{
				m_SensorTab.AF_InitParaList = AF_InitParaList;
				m_SensorTab.AF_InitParaListSize = AF_InitParaListSize;
			}
			if (AF_AutoParaListSize)
			{
				m_SensorTab.AF_AutoParaList = AF_AutoParaList;
				m_SensorTab.AF_AutoParaListSize = AF_AutoParaListSize;
			}
			if (AF_FarParaListSize)
			{
				m_SensorTab.AF_FarParaList = AF_FarParaList;
				m_SensorTab.AF_FarParaListSize = AF_FarParaListSize;
			}
			if (AF_NearParaListSize)
			{
				m_SensorTab.AF_NearParaList = AF_NearParaList;
				m_SensorTab.AF_NearParaListSize;
			}
			if (Exposure_ParaListSize)
			{
				m_SensorTab.Exposure_ParaList = Exposure_ParaList;
				m_SensorTab.Exposure_ParaListSize = Exposure_ParaListSize;
			}
			if (Gain_ParaListSize)
			{
				m_SensorTab.Gain_ParaList = Gain_ParaList;
				m_SensorTab.Gain_ParaListSize = Gain_ParaListSize;
			}
			file.close();
		}
	}
	if ((m_SensorTab.width == 0) || (m_SensorTab.height == 0) ||
		(m_SensorTab.ParaList == NULL) || (m_SensorTab.ParaListSize == 0))
	{
		std::cout << "Width = " << m_SensorTab.width << ", Height = " << m_SensorTab.height << std::endl;
		return false;
	}
	return true;
}

bool MIPI_ONLY_CTRL::InitialDevice(String ^ DevName, int DevNum)
{
	std::string message = "";

	float mclk = 24.0f;
	float avdd = 0.0f;
	float avdd2 = 0.0f;
	float dvdd = 0.0f;
	float dovdd = 0.0f;
	float afvcc = 0.0f;
	float vpp2 = 0.0f;
	float oisvdd = 0.0f;

	m_SensorTab.avdd = min(max(m_SensorTab.avdd, 900), 3300);
	m_SensorTab.avdd2 = min(max(m_SensorTab.avdd2, 900), 3300);
	m_SensorTab.dvdd = min(max(m_SensorTab.dvdd, 900), 2000);
	m_SensorTab.dovdd = min(max(m_SensorTab.dovdd, 1800), 3300); // 會影響到一般 GPIO 輸出的 level，所以將最低電壓設置為 1.8V 以匹配大多數裝置
	m_SensorTab.afvcc = min(max(m_SensorTab.afvcc, 900), 3300);
	m_SensorTab.vpp2 = min(max(m_SensorTab.vpp2, 900), 3300);
	m_SensorTab.oisvdd = min(max(m_SensorTab.oisvdd, 900), 3300);

	mclk = (float)m_SensorTab.mclk / 1000;
	avdd = (float)m_SensorTab.avdd / 1000;
	avdd2 = (float)m_SensorTab.avdd2 / 1000;
	dvdd = (float)m_SensorTab.dvdd / 1000;
	dovdd = (float)m_SensorTab.dovdd / 1000;
	afvcc = (float)m_SensorTab.afvcc / 1000;
	vpp2 = (float)m_SensorTab.vpp2 / 1000;
	oisvdd = (float)m_SensorTab.oisvdd / 1000;

	std::cout << "MCLK = " << mclk << std::endl;
	std::cout << "AVDD = " << avdd << std::endl;
	std::cout << "AVDD2 = " << avdd2 << std::endl;
	std::cout << "DVDD = " << dvdd << std::endl;
	std::cout << "DOVDD = " << dovdd << std::endl;
	std::cout << "AFVCC = " << afvcc << std::endl;
	std::cout << "VPP2 = " << vpp2 << std::endl;
	std::cout << "OISVDD = " << oisvdd << std::endl;

	SENSOR_POWER power_source[] = {POWER_AVDD, POWER_DOVDD, POWER_DVDD, POWER_AFVCC, POWER_VPP2, POWER_OISVDD, POWER_AVDD2};
	const int power_count = sizeof(power_source) / sizeof(SENSOR_POWER);

	int power_rise[power_count];
	CURRENT_RANGE power_current_range[power_count];
	int power_current_limit[power_count];
	int power_sample_speed[power_count];
	const int power_voltage_zero[power_count] = {0};
	int power_voltage[power_count];
	BOOL power_state[power_count];
	for (int i = 0; i < power_count; i++) // 初始化
	{
		power_rise[i] = 3000;
		power_current_range[i] = CURRENT_RANGE_MA;
		power_current_limit[i] = 200;
		power_sample_speed[i] = 50;
		power_voltage[i] = 0;
		power_state[i] = true;
	}

	char *dev_name = (char *)(void *)Marshal::StringToHGlobalAnsi(DevName);
	int dev_id;

	BYTE pin_def[40] = {0};

	USHORT tv_board_value = 0;
	FrameBufferConfig frame_buffer_config;

	if (DevName == "")
	{
		message = "No device";
		goto Finish;
	}
	if (m_DevID != DevNum)
	{
		if (DevNum >= 0)
		{
			if (IsDevConnect(m_DevID) == DT_ERROR_OK)
			{
				message = "Device connect failed";
				goto Finish;
			}
		}
	}
	if (OpenDevice(dev_name, &dev_id) != DT_ERROR_OK)
	{
		message = "Device open failed";
		goto Finish;
	}

	m_DevID = dev_id;

	SensorEnable(m_SensorTab.pin, false, m_DevID);
	SetSoftPinPullUp(IO_NOPULL, m_DevID);
	if (SetSensorClock(false, (USHORT)(mclk * 10), m_DevID) != DT_ERROR_OK)
	{
		message = "Set mclk off failed";
		goto Finish;
	}

	/*
	 * Setup power, mclk and pin
	 */

	/* power setup */
	power_voltage[0] = (int)(avdd * 1000);
	if (avdd < 1.6)
		power_current_limit[0] = 200;
	else if (avdd < 2.6)
		power_current_limit[0] = 300;
	else if (avdd < 3.4)
		power_current_limit[0] = 500;
	// power_state[0] = false;

	power_voltage[1] = (int)(dovdd * 1000);
	if (dovdd < 1.6)
		power_current_limit[1] = 200;
	else if (dovdd < 2.6)
		power_current_limit[1] = 300;
	else if (dovdd < 3.4)
		power_current_limit[1] = 500;
	// power_state[1] = false;

	power_voltage[2] = (int)(dvdd * 1000);
	if (dvdd < 2.1)
		power_current_limit[2] = 800;
	// power_state[2] = false;

	power_voltage[3] = (int)(afvcc * 1000);
	if (afvcc < 1.6)
		power_current_limit[3] = 200;
	else if (afvcc < 2.6)
		power_current_limit[3] = 300;
	else if (afvcc < 3.4)
		power_current_limit[3] = 500;
	// power_state[3] = false;

	power_voltage[4] = (int)(vpp2 * 1000);
	if (vpp2 < 1.6)
		power_current_limit[4] = 200;
	else if (vpp2 < 2.6)
		power_current_limit[4] = 300;
	else if (vpp2 < 3.4)
		power_current_limit[4] = 500;
	// power_state[4] = false;

	power_voltage[5] = (int)(oisvdd * 1000);
	if (oisvdd < 1.6)
		power_current_limit[4] = 200;
	else if (oisvdd < 2.6)
		power_current_limit[4] = 300;
	else if (oisvdd < 3.4)
		power_current_limit[4] = 600;
	// power_state[5] = false;

	power_voltage[6] = (int)(avdd2 * 1000);
	if (avdd2 < 1.6)
		power_current_limit[6] = 200;
	else if (avdd2 < 2.6)
		power_current_limit[6] = 300;
	else if (avdd2 < 3.4)
		power_current_limit[6] = 500;
		// power_state[6] = false;

#if 0
	std::cout << "/* ========== Power Log ========== */" << std::endl;
	for (int i = 0; i < power_count; i++) // log
	{
		std::cout << "Rise   [" << i << "] = " << power_rise[i] << std::endl;
		std::cout << "Range  [" << i << "] = " << power_current_range[i] << std::endl;
		std::cout << "Limit  [" << i << "] = " << power_current_limit[i] << std::endl;
		std::cout << "Speed  [" << i << "] = " << power_sample_speed[i] << std::endl;
		std::cout << "Voltage[" << i << "] = " << power_voltage[i] << std::endl;
		std::cout << "State  [" << i << "] = " << power_state[i] << std::endl;
		std::cout << std::endl;
	}
	std::cout << "/* =============================== */" << std::endl;
#endif

	if (PmuSetRise(power_source, power_rise, power_count, m_DevID) != DT_ERROR_OK)
	{
		message = "Set power rise failed";
		goto Finish;
	}
	Sleep(30);
	if (PmuSetCurrentRange(power_source, power_current_range, power_count, m_DevID) != DT_ERROR_OK)
	{
		message = "Set current range failed";
		goto Finish;
	}
	Sleep(30);
	if (PmuSetOcpCurrentLimit(power_source, power_current_limit, power_count, m_DevID) != DT_ERROR_OK)
	{
		message = "Set current limit failed";
		goto Finish;
	}
	Sleep(30);
	if (PmuSetSampleSpeed(power_source, power_sample_speed, power_count, m_DevID) != DT_ERROR_OK)
	{
		message = "Set power sample speed failed";
		goto Finish;
	}
	Sleep(30);
	if (PmuSetVoltage(power_source, (int *)power_voltage_zero, power_count, m_DevID) != DT_ERROR_OK)
	{
		message = "Set voltage zero failed";
		goto Finish;
	}
	Sleep(50);
	if (PmuSetOnOff(power_source, power_state, power_count, m_DevID) != DT_ERROR_OK)
	{
		message = "Set power on/off failed";
		goto Finish;
	}
	Sleep(50);
	if (PmuSetVoltage(power_source, power_voltage, power_count, m_DevID) != DT_ERROR_OK)
	{
		message = "Set power voltage failed";
		goto Finish;
	}
	Sleep(50);

	/* mclk setup */
	if (SetSensorClock(true, (USHORT)(mclk * 10), m_DevID) != DT_ERROR_OK)
	{
		message = "Set mclk on failed";
		goto Finish;
	}
	Sleep(50);

	/* pin setup */
	if (m_SensorTab.port == PORT_MIPI || m_SensorTab.port == PORT_HISPI)
	{
		pin_def[0] = PIN_NC;
		pin_def[1] = PIN_D0;
		pin_def[2] = PIN_D2;
		pin_def[3] = PIN_D1;
		pin_def[4] = PIN_D3;
		pin_def[5] = PIN_D4;
		pin_def[6] = PIN_D5;
		pin_def[7] = PIN_D6;
		pin_def[8] = PIN_D7;
		pin_def[9] = PIN_D8;
		pin_def[10] = PIN_D9;
		pin_def[11] = PIN_NC;
		pin_def[12] = PIN_PCLK;
		pin_def[13] = PIN_HSYNC;
		pin_def[14] = PIN_VSYNC;
		pin_def[15] = PIN_NC;
		pin_def[16] = PIN_NC;
		pin_def[17] = PIN_MCLK;
		pin_def[18] = PIN_PWDN;
		pin_def[19] = PIN_RESET;
		pin_def[20] = PIN_SCL;
		pin_def[21] = PIN_SDA;
		pin_def[22] = PIN_GPIO2;
		pin_def[23] = PIN_GPIO1;
		pin_def[24] = PIN_GPIO4;
		pin_def[25] = PIN_GPIO3;
	}
	else if (m_SensorTab.port == 0x81 || m_SensorTab.port == 0x83)
	{
		pin_def[0] = 20;
		pin_def[1] = 0;
		pin_def[2] = 2;
		pin_def[3] = 1;
		pin_def[4] = 3;
		pin_def[5] = 4;
		pin_def[6] = 5;
		pin_def[7] = 6;
		pin_def[8] = 7;
		pin_def[9] = 8;
		pin_def[10] = 9;
		pin_def[11] = 20;
		pin_def[12] = 10;
		pin_def[13] = 11;
		pin_def[14] = 12;
		pin_def[15] = 20;
		pin_def[16] = 20;
		pin_def[17] = 13;
		pin_def[18] = 15;
		pin_def[19] = 14;
		pin_def[20] = PIN_SPI_SCK; // PIN_SCL
		pin_def[21] = PIN_SPI_SDO; // PIN_SDA
		pin_def[22] = PIN_SPI_CS;
		pin_def[23] = PIN_SPI_SDI;
		pin_def[24] = PIN_GPIO3;
		pin_def[25] = PIN_GPIO4;
	}
	else // standard parallel
	{
		pin_def[0] = PIN_SDA;
		pin_def[1] = PIN_SCL;
		pin_def[2] = PIN_VSYNC;
		pin_def[3] = PIN_RESET;
		pin_def[4] = PIN_PWDN;
		pin_def[5] = PIN_HSYNC;
		pin_def[6] = PIN_MCLK;
		pin_def[7] = PIN_D0;
		pin_def[8] = PIN_D1;
		pin_def[9] = PIN_D2;
		pin_def[10] = PIN_D3;
		pin_def[11] = PIN_PCLK;
		pin_def[12] = PIN_D4;
		pin_def[13] = PIN_D5;
		pin_def[14] = PIN_D6;
		pin_def[15] = PIN_D7;
		pin_def[16] = PIN_D8;
		pin_def[17] = PIN_D9;
		pin_def[18] = PIN_NC;
		pin_def[19] = PIN_NC;
		pin_def[20] = PIN_NC;
		pin_def[21] = PIN_NC;
		pin_def[22] = PIN_NC;
		pin_def[23] = PIN_NC;
		pin_def[24] = PIN_NC;
		pin_def[25] = PIN_NC;
	}
	SetSoftPin(pin_def, m_DevID);
	EnableSoftPin(true, m_DevID);
	EnableGpio(true, m_DevID);
	SetSoftPinPullUp(IO_PULLUP, m_DevID);
	Sleep(300);

	SetMipiImageVC(0, 1, 1, m_DevID);
	Sleep(50);
	/* I2C setup */
	SetSensorI2cRate(I2C_100K, m_DevID);
	SetSensorI2cRapid(0, m_DevID);
	SetI2CInterval(0, m_DevID);
	SetSensorI2cAckWait(1000, m_DevID);
	ResetSensorI2cBus(m_DevID);

	/* Pin setup: power down, reset */
	SensorEnable(m_SensorTab.pin, true, m_DevID); // [0]: power down ; [1]: reset
	Sleep(50);
	/* Check if sensor is detected */
	if (SensorIsMe(&m_SensorTab, CHANNEL_A, 0, m_DevID) != DT_ERROR_OK)
	{
		message = "Check if sensor is detected";
		goto Finish;
	}
	/* Initial sensor */
	if (InitSensor(m_SensorTab.SlaveID, m_SensorTab.ParaList, m_SensorTab.ParaListSize, m_SensorTab.mode, m_DevID) != DT_ERROR_OK)
	{
		message = "Initial sensor failed";
		goto Finish;
	}

	m_IsTV = false;
	/* Check sensor is TV or not */
	ReadSensorReg(0xBA, 0x80, &tv_board_value, I2CMODE_MICRON, m_DevID);
	if (tv_board_value == 0x5150)
		m_IsTV = 1;

	if (m_SensorTab.type == D_YUV ||
		m_SensorTab.type == D_YUV_SPI ||
		m_SensorTab.type == D_YUV_MTK_S)
		SetYUV422Format(m_SensorTab.outformat, m_DevID);
	else
		SetRawFormat(m_SensorTab.outformat, m_DevID);

	m_PreviewWidth = m_SensorTab.width;
	m_PreviewHeight = m_SensorTab.height;

	InitRoi(0, 0, m_SensorTab.width, m_IsTV ? m_SensorTab.height >> 1 : m_SensorTab.height, 0, 0, 1, 1, m_SensorTab.type, true, m_DevID);
	SetSensorPort(m_SensorTab.port, m_SensorTab.width, m_SensorTab.height, m_DevID);

	SetMipiClkPhase(0, m_DevID);
	SetMipiEnable(true, m_DevID);
	Sleep(10);
	CalculateGrabSize(&m_GrabSize, m_DevID);
	std::cout << "Grab size = " << m_GrabSize << std::endl;
	GetFrameBufferConfig(&frame_buffer_config, m_DevID);

	m_BufferSize = frame_buffer_config.uBufferSize;
	frame_buffer_config.uMode = BUF_MODE_SKIP; // BUF_MODE_NORMAL
	frame_buffer_config.uUpLimit = m_UpLimitSize;
	SetFrameBufferConfig(&frame_buffer_config, m_DevID);

	OpenVideo(m_GrabSize, m_DevID);
	InitIsp(m_SensorTab.width, m_SensorTab.height, m_SensorTab.type, CHANNEL_A, m_DevID);
	SetGamma(100, m_DevID);					// Gamma      : default 100 is no gamma change
	SetContrast(100), m_DevID;				// Contrast   : default 100 is no contrast change
	SetSaturation(128, m_DevID);			// Saturation : default 128 is no saturation change
	SetDigitalGain(1.0, 1.0, 1.0, m_DevID); // Auto white balance digital gain value (R/G/B)
	SetMatrixEnable(TRUE, m_DevID);

Finish:
	if (message == "")
		return true;
	else
	{
		CloseDevice(m_DevID);
		std::cout << "Error: " << message << std::endl;
		return false;
	}
}

int MIPI_ONLY_CTRL::GrabSize()
{
	return m_GrabSize;
}

byte *MIPI_ONLY_CTRL::GrabImage()
{
	const int buffer_size = (m_SensorTab.width * m_SensorTab.height * 3) + 1024;
	FrameInfo frame_info;
	ULONG grab_size = 0;
	ULONG mipi_freq = 0;
	UINT err_count = 0;
	DWORD mipi_lock_state = 0;
	if ((CalibrateSensorPort(8000, m_DevID) == DT_ERROR_TIME_OUT))
	{
		std::cout << "Error: CalibrateSensorPort" << std::endl;
		return NULL;
	}
	if (p_DynamicImageData == NULL)
	{
		p_DynamicImageData = new byte[buffer_size];
		p_RawImageData = new byte[buffer_size];
	}
	int ret = GrabFrame(p_DynamicImageData, m_GrabSize, &grab_size, &frame_info, m_DevID);
#if 0
	std::cout << "Flag (GrabFrame) = " << ret << std::endl;
	GetMipiClkFrequency(&mipi_freq, 1, m_DevID);
	std::cout << "MIPI speed = " << mipi_freq << std::endl;
	GetMipiCrcErrorCount(&err_count, 1, m_DevID);
	std::cout << "MIPI CRC Err Cnt = " << err_count << std::endl;
	GetMipiEccErrorCount(&err_count, 1, m_DevID);
	std::cout << "MIPI ECC Err Cnt = " << err_count << std::endl;
	GetMipiLockState(&mipi_lock_state, 1, m_DevID);
	std::cout << "MIPI Lock State = " << mipi_lock_state << std::endl;
#endif
	m_FrameInfo = frame_info;
	memcpy(p_RawImageData, p_DynamicImageData, m_GrabSize);
	return p_DynamicImageData;
}

byte *MIPI_ONLY_CTRL::GrabRawData()
{
	return p_RawImageData;
}

byte *MIPI_ONLY_CTRL::GrabImageProcess(byte *pixels)
{
	const int buffer_size = (m_SensorTab.width * m_SensorTab.height * 3) + 1024;
	FrameInfo frame_info;
	frame_info = m_FrameInfo;
	if (p_ProcessImageData == NULL)
	{
		p_ProcessImageData = new byte[buffer_size];
	}
	ImageProcess(pixels, p_ProcessImageData, m_SensorTab.width, m_SensorTab.height, &frame_info, m_DevID);
	return p_ProcessImageData;
}

void MIPI_ONLY_CTRL::ReadReg(UCHAR SlaveID, USHORT Address, USHORT *pData)
{
	ReadSensorReg(SlaveID, Address, pData, I2CMODE_NORMAL, m_DevID);
}

void MIPI_ONLY_CTRL::WriteReg(UCHAR SlaveID, USHORT Address, USHORT pData)
{
	WriteSensorReg(SlaveID, Address, pData, I2CMODE_NORMAL, m_DevID);
}

void MIPI_ONLY_CTRL::SetPinResetState(bool level)
{
	SensorEnable(0x02, level, m_DevID); // [0]: power down ; [1]: reset
	Sleep(10);
}

void MIPI_ONLY_CTRL::SetPinPowerDownState(bool level)
{
	SensorEnable(0x01, level, m_DevID); // [0]: power down ; [1]: reset
	Sleep(10);
}

void MIPI_ONLY_CTRL::GetPinState(bool *pwdn, bool *rst)
{
	if ((m_SensorTab.pin & 0b1) == 0b1)
		*pwdn = true;
	else
		*pwdn = false;
	if ((m_SensorTab.pin & 0b10) == 0b10)
		*rst = true;
	else
		*rst = false;
}

void MIPI_ONLY_CTRL::SetAutoWhiteBalanceState(bool enable)
{
	SetAWB(enable, m_DevID);
	if (!enable)
	{
		SetDigitalGain(1.0, 1.0, 1.0, m_DevID); // Auto white balance digital gain value (R/G/B)
	}
}

void MIPI_ONLY_CTRL::GetAutoWhiteBalanceValue(bool *enable, float *gain_red, float *gain_green, float *gain_blue)
{
	GetAWB((BOOL *)(enable), m_DevID);
	GetDigitalGain(gain_red, gain_green, gain_blue, m_DevID);
}

void MIPI_ONLY_CTRL::SetDigitalGainValue(float gain_red, float gain_green, float gain_blue)
{
	SetDigitalGain(gain_red, gain_green, gain_blue, m_DevID); // Auto white balance digital gain value (R/G/B)
}

void MIPI_ONLY_CTRL::SetDeadPixelsCleanState(bool enable, int HotCpth, int DeadCpth)
{
	SetDeadPixelsClean(enable, m_DevID);
	SetHotCpth(HotCpth, m_DevID);
	SetDeadCpth(DeadCpth, m_DevID);
}

void MIPI_ONLY_CTRL::SetGammaState(bool enable, int gamma)
{
	if (enable)
	{
		SetGamma(gamma, m_DevID);
	}
	else
	{
		SetGamma(100, m_DevID);
	}
}

void MIPI_ONLY_CTRL::SetContrastState(bool enable, int contrast)
{
	if (enable)
	{
		SetContrast(contrast, m_DevID);
	}
	else
	{
		SetContrast(100, m_DevID);
	}
}

void MIPI_ONLY_CTRL::SetSaturationState(bool enable, int saturation)
{
	if (enable)
	{
		SetSaturation(saturation, m_DevID);
	}
	else
	{
		SetSaturation(128, m_DevID);
	}
}

void MIPI_ONLY_CTRL::SetSharpnessState(bool enable, int sharpness)
{
	if (enable)
	{
		SetSharpness(sharpness, m_DevID);
	}
	else
	{
		SetSharpness(0, m_DevID);
	}
}

void MIPI_ONLY_CTRL::SetNoiseReductionState(bool enable, int noise)
{
	if (enable)
	{
		SetNoiseReduction(noise, m_DevID);
	}
	else
	{
		SetNoiseReduction(0, m_DevID);
	}
}

void MIPI_ONLY_CTRL::SetGpioDirection(int pin, bool dir)
{
	SetGpioPinDir(pin, dir, m_DevID);
}

void MIPI_ONLY_CTRL::SetGpioLevel(int pin, bool level)
{
	SetGpioPinLevel(pin, level, m_DevID);
}

void MIPI_ONLY_CTRL::GetGpioLevel(int pin, bool *level)
{
	GetGpioPinLevel(pin, (BOOL *)level, m_DevID);
}

void MIPI_ONLY_CTRL::SetVoltage(int avdd, int dovdd, int dvdd)
{
	float f_avdd = (float)avdd / 1000;
	float f_dovdd = (float)dovdd / 1000;
	float f_dvdd = (float)dvdd / 1000;
	float f_afvcc = 0.0f;
	float f_vpp2 = 0.0f;
	float f_oisvdd = 0.0f;
	float f_avdd2 = 0.0f;

	SENSOR_POWER power_source[] = {POWER_AVDD, POWER_DOVDD, POWER_DVDD, POWER_AFVCC, POWER_VPP2, POWER_OISVDD, POWER_AVDD2};
	const int power_count = sizeof(power_source) / sizeof(SENSOR_POWER);
	int power_voltage[power_count] = {0, 0, 0, 0, 0, 0, 0};
	int power_current_limit[power_count] = {200, 200, 200, 200, 200, 200, 200};
	BOOL power_state[power_count] = {true, true, true, true, true, true, true};
	CURRENT_RANGE power_current_range[power_count] = {
		CURRENT_RANGE_MA, CURRENT_RANGE_MA, CURRENT_RANGE_MA, CURRENT_RANGE_MA,
		CURRENT_RANGE_MA, CURRENT_RANGE_MA, CURRENT_RANGE_MA};
	int power_sample_speed[power_count] = {150, 150, 150, 150, 150, 150, 150};

	PmuSetOnOff(power_source, power_state, power_count, m_DevID);
	Sleep(50); // wait for power on

	/* power setup */
	power_voltage[0] = (int)(f_avdd * 1000);
	if (f_avdd < 1.6)
		power_current_limit[0] = 200;
	else if (f_avdd < 2.6)
		power_current_limit[0] = 300;
	else if (f_avdd < 3.4)
		power_current_limit[0] = 500;

	power_voltage[1] = (int)(f_dovdd * 1000);
	if (f_dovdd < 1.6)
		power_current_limit[1] = 200;
	else if (f_dovdd < 2.6)
		power_current_limit[1] = 300;
	else if (f_dovdd < 3.4)
		power_current_limit[1] = 500;

	power_voltage[2] = (int)(f_dvdd * 1000);
	if (f_dvdd < 2.1)
		power_current_limit[2] = 800;

	power_voltage[3] = (int)(f_afvcc * 1000);
	if (f_afvcc < 1.6)
		power_current_limit[3] = 200;
	else if (f_afvcc < 2.6)
		power_current_limit[3] = 300;
	else if (f_afvcc < 3.4)
		power_current_limit[3] = 500;

	power_voltage[4] = (int)(f_vpp2 * 1000);
	if (f_vpp2 < 1.6)
		power_current_limit[4] = 200;
	else if (f_vpp2 < 2.6)
		power_current_limit[4] = 300;
	else if (f_vpp2 < 3.4)
		power_current_limit[4] = 500;

	power_voltage[5] = (int)(f_oisvdd * 1000);
	if (f_oisvdd < 1.6)
		power_current_limit[4] = 200;
	else if (f_oisvdd < 2.6)
		power_current_limit[4] = 300;
	else if (f_oisvdd < 3.4)
		power_current_limit[4] = 600;

	power_voltage[6] = (int)(f_avdd2 * 1000);
	if (f_avdd2 < 1.6)
		power_current_limit[6] = 200;
	else if (f_avdd2 < 2.6)
		power_current_limit[6] = 300;
	else if (f_avdd2 < 3.4)
		power_current_limit[6] = 500;

	PmuSetCurrentRange(power_source, power_current_range, power_count, m_DevID);
	PmuSetOcpCurrentLimit(power_source, power_current_limit, power_count, m_DevID);
	PmuSetVoltage(power_source, power_voltage, power_count, m_DevID);
	Sleep(2);
	PmuSetSampleSpeed(power_source, power_sample_speed, power_count, m_DevID);
	Sleep(100);
}

void MIPI_ONLY_CTRL::GetVoltage(int *avdd, int *dovdd, int *dvdd)
{
	SENSOR_POWER power_source[] = {POWER_AVDD, POWER_DOVDD, POWER_DVDD, POWER_AFVCC, POWER_VPP2, POWER_OISVDD, POWER_AVDD2};
	const int power_count = sizeof(power_source) / sizeof(SENSOR_POWER);
	int power_voltage[power_count] = {0, 0, 0, 0, 0, 0, 0};

	PmuGetVoltage(power_source, power_voltage, power_count, m_DevID);

	*avdd = power_voltage[0];
	*dovdd = power_voltage[1];
	*dvdd = power_voltage[2];

	int afvcc = power_voltage[3];
	int vpp2 = power_voltage[4];
	int oisvdd = power_voltage[5];
	int avdd2 = power_voltage[6];
	(void)afvcc;
	(void)vpp2;
	(void)oisvdd;
	(void)avdd2;
}