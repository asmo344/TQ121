// JIG_BOX.cpp : 
//

#include "stdafx.h"

#include "JIG_BOX.h"
#include "ET727_reg.h"
#include "stdio.h"
#include "Com_helper.h"
#include "math.h"

using namespace JIG_BOX;

HANDLE m_hCom = 0;
#define COMMAND_LENGTH            8
#define CE_ARDUINO_PIN            40
#define WAKE_ARDUINO_PIN          28
#define GPIO0_ARDUINO_PIN         29

#define READ_TIMEOUT              8000
#define BIT_LDO_ON                0x0
#define BIT_RESET_N               0x2
#define BIT_PWR_KEY               0x4
#define BIT_INT                   0x8

int com_ports[MAX_COM_PORTS];

BOOL JIG_BOX_CTRL::bOpenSensor()
{
	WCHAR wsrc[MAX_PATH];
	char port_name[MAX_PATH];
	if (JIG_BOX_CTRL::iCom == -1) {
		OutputDebugString(L"Com is NULL\n");
		return FALSE;
	}

	sprintf_s(port_name, MAX_PATH, "\\\\.\\COM%d", JIG_BOX_CTRL::iCom);
	swprintf(wsrc, MAX_PATH, L"%S", port_name);

	if (m_hCom != 0)
		CloseHandle(m_hCom);

	m_hCom = CreateFile(wsrc, GENERIC_READ | GENERIC_WRITE, 0, NULL,
		OPEN_EXISTING, 0, NULL);


	if (m_hCom == INVALID_HANDLE_VALUE) {
		return FALSE;
	}

	PurgeComm(m_hCom,
		PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);

	// Set COM Port Queue Size
	SetupComm(m_hCom, 1024, 1024);

	COMMTIMEOUTS TimeOuts;

	//set read timeout
	TimeOuts.ReadIntervalTimeout = MAXDWORD;
	TimeOuts.ReadTotalTimeoutMultiplier = 0;
	TimeOuts.ReadTotalTimeoutConstant = 0;

	//set write timeout
	TimeOuts.WriteTotalTimeoutMultiplier = 100;
	TimeOuts.WriteTotalTimeoutConstant = 500;
	SetCommTimeouts(m_hCom, &TimeOuts);

	DCB dcb;
	GetCommState(m_hCom, &dcb);
	dcb.BaudRate = 115200;
	dcb.ByteSize = 8;
	dcb.Parity = NOPARITY;
	dcb.StopBits = ONESTOPBIT;
	dcb.fDtrControl = 1;

	SetCommState(m_hCom, &dcb);

	return TRUE;
}

BOOL JIG_BOX_CTRL::bCloseSensor()
{
	if (m_hCom != INVALID_HANDLE_VALUE) {
		return CloseHandle(m_hCom);
	}
	return FALSE;
}

void JIG_BOX_ET727_CTRL::bLEDBackLightOn(BOOL bOn, unsigned char led_gain, unsigned char *pLightness) {
	JIG_BOX_CTRL::bWriteRegisterSingleByte(0xA5, 0x5A, 0x6B);

	if (bOn) {
		JIG_BOX_CTRL::bWriteRegisterSingleByte(0x1C, led_gain, 0x69);

		JIG_BOX_CTRL::bWriteRegisterSingleByte(0x00, 0x08, 0x68);
		JIG_BOX_CTRL::bWriteRegisterSingleByte(0x01, 0x00, 0x69);

		if (pLightness) {
			int         i;
			for (i = 0; i < 16; i++) {
				JIG_BOX_CTRL::bWriteRegisterSingleByte(0x02 + i, *pLightness, 0x69);
			}
		}
		else {
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x02, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x03, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x04, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x05, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x06, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x07, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x08, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x09, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x0A, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x0B, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x0C, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x0D, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x0E, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x0F, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x10, 0xA0, 0x69);
			JIG_BOX_CTRL::bWriteRegisterSingleByte(0x11, 0xA0, 0x69);
		}

		JIG_BOX_CTRL::bWriteRegisterSingleByte(0x14, 0xFF, 0x69);
		JIG_BOX_CTRL::bWriteRegisterSingleByte(0x15, 0xFF, 0x69);
		JIG_BOX_CTRL::bWriteRegisterSingleByte(0x16, 0xFF, 0x69);
		JIG_BOX_CTRL::bWriteRegisterSingleByte(0x17, 0xFF, 0x69);

		// Wait some time for LED Stable
		Sleep(1000);
	}
}


BOOL JIG_BOX_CTRL::bWriteRegisterSingleByte(unsigned char addr, unsigned char value, unsigned char I2CAddr) {
	BOOL    r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = 0x58; //Write LED data

	// Parameter
	command[5] = I2CAddr;

	command[6] = addr;

	command[7] = value;

	// Write Command
	r_value = bWritComPort(command, 8);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = bReadComPort(buf, 7, 8000);
	if (!r_value) {
		return FALSE;
	}

	if ((buf[0] != 'S') || (buf[1] != 'I') || (buf[2] != 'G') || (buf[3] != 'E')) {
		return FALSE;
	}

	if (buf[6] != 0x01) {
		return FALSE;
	}

	return TRUE;
}


BOOL JIG_BOX_CTRL::bWritComPort(unsigned char *pWriteBuf, int iWriteBufLen)
{
	if (m_hCom == INVALID_HANDLE_VALUE) {
		if (!JIG_BOX_CTRL::bOpenSensor())
			return FALSE;
	}

	COMSTAT ComStat;
	DWORD dwErrorFlags;
	DWORD dwDataWritten;
	BOOL bWriteStat;

	ClearCommError(m_hCom, &dwErrorFlags, &ComStat);

	bWriteStat = WriteFile(m_hCom, pWriteBuf, iWriteBufLen, &dwDataWritten,
		NULL);
	if (!bWriteStat) {
		return FALSE;
	}

	return TRUE;
}

BOOL JIG_BOX_CTRL::bReadComPort(unsigned char *pReadBuf, int iReadBufLen, int timeout)
{
	if (m_hCom == INVALID_HANDLE_VALUE) {
		if (!bOpenSensor())
			return FALSE;
	}

	SYSTEMTIME st_begin;
	SYSTEMTIME st_end;

	GetSystemTime(&st_begin);

	BOOL bTimeOut = FALSE;

	BYTE read_data[67800];
	DWORD read_count;

	BYTE DataQueue[67800];
	int DataQueueLen;

	BOOL r_value;

	memset(DataQueue, 0x00, sizeof(DataQueue));
	DataQueueLen = 0;

	while (!bTimeOut) {
		memset(read_data, 0x00, sizeof(read_data));
		read_count = 67800;

		r_value = ReadFile(m_hCom, read_data, read_count, &read_count, NULL);
		if ((r_value) && (read_count > 0)) {
			char buf[MAX_PATH];
			sprintf_s(buf, MAX_PATH, "%d data received.\n", read_count);
			//OutputDebugString(buf);


			memcpy(DataQueue + DataQueueLen, read_data, read_count);
			DataQueueLen += read_count;

			// Reset Timer
			GetSystemTime(&st_begin);

			if (DataQueueLen >= iReadBufLen) {
				break;
			}
		}

		GetSystemTime(&st_end);

		int     mille_time_passed = (st_end.wSecond * 1000 +
			st_end.wMilliseconds) -
			(st_begin.wSecond * 1000 +
				st_begin.wMilliseconds);

		if (mille_time_passed > timeout) {
			bTimeOut = TRUE;
		}
	}

	PurgeComm(m_hCom,
		PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);

	if (bTimeOut) {
		if (DataQueueLen) {
			OutputDebugString(L"????\n");
		}
		return FALSE;
	}

	memcpy(pReadBuf, DataQueue, iReadBufLen);

	return TRUE;
}

BOOL JIG_BOX_CTRL::bSetGPIOMode(unsigned char Pin, unsigned char Mode)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = 0x20; // Set Pin Mode

	// Parameter
	command[5] = Pin;

	// Value
	command[6] = Mode; //0:input 1:output

	// Write Command
	r_value = bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = bReadComPort(buf, 7, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	if ((buf[0] != 'S') ||
		(buf[1] != 'I') ||
		(buf[2] != 'G') ||
		(buf[3] != 'E'))
	{
		return FALSE;
	}

	if (buf[6] != 0x01) {
		return FALSE;
	}
	return TRUE;
}

BOOL JIG_BOX_CTRL::bSetGPIOData(unsigned char Pin, unsigned char Data)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = 0x21; // Set Pin Data

	// Parameter
	command[5] = Pin;

	// Value
	command[6] = Data;

	// Write Command
	r_value = bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = bReadComPort(buf, 7, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	if ((buf[0] != 'S') ||
		(buf[1] != 'I') ||
		(buf[2] != 'G') ||
		(buf[3] != 'E'))
	{
		return FALSE;
	}

	if (buf[6] != 0x01)
	{
		return FALSE;
	}
	return TRUE;
}

/* ============================================================ */
// ET727 sensor

#define ET727_SENSOR_STANDBY              0x87
#define ET727_SENSOR_WAKEUP               0x88
#define ET727_SENSOR_CHECK_PST            0x89
#define ET727_SENSOR_REG_READ             0x8A
#define ET727_SENSOR_REG_WRITE            0X8B
#define ET727_SENSOR_GET_IMAGE            0X8C
#define ET727_SENSOR_EFUSE_READ           0X8D
#define ET727_SENSOR_EFUSE_WRITE          0X8E
#define ET727_SENSOR_MCLK_SELECT          0X8F
#define ET727_SENSOR_SET_SPI_MODE         0X07
#define ET727_SENSOR_GET_SPI_MODE         0X08
#define ET727_SENSOR_REG_READ_B           0X90
#define ET727_SENSOR_REG_WRITE_B          0X91
#define ET727_SENSOR_EFUSE_READ_B         0x92
#define RESOLUTION                        JIG_BOX_ET727_CTRL::width * JIG_BOX_ET727_CTRL::height
#define CHECK_TIMEOUT                     5 // 5s

//*** for ET727A IC issue, need to reorder raw data ***//
BOOL JIG_BOX_ET727_CTRL::binaryToGray(unsigned char* src_image) {
	int ET727_height = JIG_BOX_ET727_CTRL::height;
	int ET727_height_2 = JIG_BOX_ET727_CTRL::height / 2;
	int ET727_width = JIG_BOX_ET727_CTRL::width;
	int ET727_width_2 = JIG_BOX_ET727_CTRL::width / 2;

	size_t ptr = 0;

	for (int frame = 0; frame < 4; frame++)
	{
		ptr += 12;
		for (int width = 0; width < ET727_width_2; width++) {
			for (int height = 0; height < ET727_height_2; height++) {

				src_image[ptr] = src_image[ptr] ^ (src_image[ptr] >> 1);

				ptr++;
			}
		}
	}
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::grayToBinary(unsigned char* src_image) {
	int ET727_height = JIG_BOX_ET727_CTRL::height;
	int ET727_height_2 = JIG_BOX_ET727_CTRL::height / 2;
	int ET727_width = JIG_BOX_ET727_CTRL::width;
	int ET727_width_2 = JIG_BOX_ET727_CTRL::width / 2;

	size_t ptr = 0;

	for (int frame = 0; frame < 4; frame++)
	{
		ptr += 12;
		for (int width = 0; width < ET727_width_2; width++) {
			for (int height = 0; height < ET727_height_2; height++) {

                             unsigned char mask = src_image[ptr];
                             while (mask != 0)
                             {
                                 mask = mask >> 1;
                                 src_image[ptr] = src_image[ptr] ^ mask;
                             }

				ptr++;
			}
		}
	}
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::Reorder_Image(unsigned char* src_image) {
	int ET727_height = JIG_BOX_ET727_CTRL::height;
	int ET727_height_2 = JIG_BOX_ET727_CTRL::height / 2;
	int ET727_height_8 = JIG_BOX_ET727_CTRL::height / 8;
	int ET727_width = JIG_BOX_ET727_CTRL::width;
	int ET727_width_2 = JIG_BOX_ET727_CTRL::width / 2;
	unsigned char tmp_image[(JIG_BOX_ET727_CTRL::height)*(JIG_BOX_ET727_CTRL::width)+48];

	binaryToGray(src_image);

	memcpy(tmp_image, src_image, (JIG_BOX_ET727_CTRL::height)*(JIG_BOX_ET727_CTRL::width)+48);

	size_t ptr = 0;

	for (int frame = 0; frame < 4; frame++)
	{
		ptr += 12;
		for (int width = 0; width < ET727_width_2; width++) {
			for (int height = 0; height < ET727_height_2; height++) {
				if (height < ET727_height_8) {
				}
				else if (height < ET727_height_8 * 2) {
					if (height == ET727_height_8) {
					}
					else {
						src_image[ptr] = (tmp_image[ptr] & 0xF0) | (tmp_image[ptr-1] & 0x0F);
					}
				}
				else if (height < ET727_height_8 * 3) {
					if (height == (ET727_height_8 * 2)) {
						src_image[ptr] = (tmp_image[ptr] & 0xF0) | (src_image[ptr - ET727_height_8*2] & 0x0F);   // new
						//src_image[ptr] = (tmp_image[ptr] & 0xF0) |(tmp_image[ptr+ET727_height_8] & 0x0F);   // old
					}
					else {
						src_image[ptr] = (tmp_image[ptr] & 0xF0) |(tmp_image[ptr+ET727_height_8-1] & 0x0F);
					}
				}
				else {
					if (height < (ET727_height_8*4 -1)) {
						src_image[ptr] = (tmp_image[ptr] & 0xF0) |(tmp_image[ptr-ET727_height_8+1] & 0x0F);
						if (height == ET727_height_8 * 3) {
							src_image[ptr - ET727_height_8*2] = (src_image[ptr - ET727_height_8*2] & 0xF0) | (src_image[ptr] & 0x0F);  // new
						}
					}
					else {
						src_image[ptr] = (tmp_image[ptr] & 0xF0) | (src_image[ptr - ET727_height_8*2] & 0x0F);   // new
					}
				}

				ptr++;
			}
		}
	}

	grayToBinary(src_image);

	return TRUE;
}
//*** for ET727A IC issue, need to reorder raw data ***//

BOOL JIG_BOX_ET727_CTRL::Combine_Image(unsigned char* src_image, unsigned char* dst_image) {
	int ET727_height = JIG_BOX_ET727_CTRL::height;
	int ET727_height_2 = JIG_BOX_ET727_CTRL::height / 2;
	int ET727_height_8 = JIG_BOX_ET727_CTRL::height / 8;
	int ET727_width = JIG_BOX_ET727_CTRL::width;
	int ET727_width_2 = JIG_BOX_ET727_CTRL::width / 2;

	size_t ptr = 0;

	for (int frame = 0; frame < 4; frame++)
	{
		int y, index;
		ptr += 12;
		for (int width = 0; width < ET727_width_2; width++) {
			for (int height = 0; height < ET727_height_2; height++) {
				if (height < ET727_height_8) {
					y = 8 * height;
				}
				else if (height < ET727_height_8 * 2) {
					y = 8 * (height - ET727_height_8) + 4;
				}
				else if (height < ET727_height_8 * 3) {
					y = 8 * (height - ET727_height_8 * 2) + 2;
					/*** workaround for output mux design ***/
					if ((frame == 0) || (frame == 2)) {
						y = y + 1;
					}
					else {
						y = y - 1;
					}
					/*** workaround for output mux design ***/
				}
				else {
					y = 8 * (height - ET727_height_8 * 3) + 6;
					/*** workaround for output mux design ***/
					if ((frame == 0) || (frame == 2)) {
						y = y + 1;
					}
					else {
						y = y - 1;
					}
					/*** workaround for output mux design ***/
				}

				if (frame == 0)
					index = (2 * width) + ET727_width * y;
				else if (frame == 1)
					index = (2 * width) + ET727_width * y + ET727_width;
				else if (frame == 2)
					index = (2 * width) + ET727_width * y + 1;
				else
					index = (2 * width) + ET727_width * y + 1 + ET727_width;

				dst_image[index] = src_image[ptr++];
			}
		}
	}
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::Combine_image_ET728(int width, int height, unsigned char* src_image, int* dst_image) 
{
	int Total_pixel = height * width;
	int pixelvalue, intdata1, intdata2;
	unsigned char filter = 0b00000000;
	size_t ptr = 0;


	for (int i = 0; i < Total_pixel / 4; i++) {
		intdata2 = (int)src_image[ptr + 4];
		for (int j = 0; j < 4; j++) {			
			intdata1 = (int)src_image[ptr + j];
			filter = 3 * pow(4, 3-j);
			//pixelvalue = (((intdata2 & filter) << 8) | intdata1);
			pixelvalue = ((intdata1 << 2) | ((intdata2 & filter) >> ((3-j)*2)));
			dst_image[i * 4 + j] = pixelvalue;
		}

		ptr += 5;
	}
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::Eight_Exposure_Image(unsigned char* src_image, UInt16* eight_exp_image) {
	int ET727_height_2 = JIG_BOX_ET727_CTRL::height / 2;
	int ET727_height_4 = JIG_BOX_ET727_CTRL::height / 4;
	int ET727_height_8 = JIG_BOX_ET727_CTRL::height / 8;
	int ET727_width    = JIG_BOX_ET727_CTRL::width;
	int ET727_width_2  = JIG_BOX_ET727_CTRL::width / 2;
	int ET727_width_8  = JIG_BOX_ET727_CTRL::width / 8;
	int ET727_width_16 = JIG_BOX_ET727_CTRL::width / 16;

	size_t ptr = 0;

	for (int frame = 0; frame < 4; frame++)
	{
		int x, y, index;
		ptr += 12;
		/*
		for (int height = 0; height < ET727_height_2; height++) {
			for (int width = 0; width < ET727_width_2; width++) {
				if ((width % ET727_width_8) < ET727_width_16)
					x = ((width/ET727_width_8)*ET727_width_8) + (width%ET727_width_16)*2;
				else
					x = ((width/ET727_width_8)*ET727_width_8) + (width%ET727_width_16)*2 + 1;

				if ((height % 2) == 0)
					y = (height/2)*ET727_width_2;
				else
					y = ((height-1)/2)*ET727_width_2 + ET727_width_2*ET727_height_4;

				eight_exp_image[x+y] += src_image[ptr++];
			}
		}
		*/
		for (int width = 0; width < ET727_width_2; width++) {
			for (int height = 0; height < ET727_height_2; height++) {
				if (height < ET727_height_8) {
					// 00 -> 02 -> 04 -> ... -> 44
					y = 2 * height;
				}
				else if (height < ET727_height_8 * 2) {
					// 01 -> 03 -> 05 -> ... -> 45
					y = 2 * (height - ET727_height_8) + 1;
				}
				else if (height < ET727_height_8 * 3) {
					// 46 -> 48 -> 50 -> ... -> 90
					y = 2 * (height - ET727_height_8 * 2) + ET727_height_4;
				}
				else {
					// 47 -> 49 -> 51 -> ... -> 91
					y = 2 * (height - ET727_height_8 * 3) + ET727_height_4 + 1;
				}

				x = (width % 4) * ET727_width_8 + (width / 4);
				index = x + ET727_width_2 * y;

				eight_exp_image[index] += src_image[ptr++];
			}
		}
	}
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_INIT(int Com)
{
	BOOL r_value;
	//char buf[MAX_PATH];
	int n_device;

	if(Com > -1) {
		JIG_BOX_CTRL::iCom = Com;
	}
	else {
		n_device = iFindArduinoCDC(com_ports);
		//sprintf(buf, "%d com port(s) found.\r\n", n_device);
		//m_OutputBox.ReplaceSel(buf);

		if (n_device) {
			JIG_BOX_CTRL::iCom = com_ports[0];
		} else {
			JIG_BOX_CTRL::iCom = -1;
		}
	}

	if (JIG_BOX_CTRL::bOpenSensor()) {
		//sprintf(buf, "Devie is OPEN.\r\n");
		//m_OutputBox.ReplaceSel(buf);

		// set CE pin mode
		r_value = JIG_BOX_CTRL::bSetGPIOMode(CE_ARDUINO_PIN, 1);
		if (!r_value) {
			//sprintf(buf, "Set Mode fail\r\n");
			//m_OutputBox.ReplaceSel(buf);
			goto init_fail;
		}

		// reset module
		r_value = JIG_BOX_CTRL::bSetGPIOData(CE_ARDUINO_PIN, 0);
		if (!r_value) {
			//sprintf(buf, "Set Mode fail\r\n");
			//m_OutputBox.ReplaceSel(buf);
			goto init_fail;
		}

		Sleep(100);

		r_value = JIG_BOX_CTRL::bSetGPIOData(CE_ARDUINO_PIN, 1);
		if (!r_value) {
			//sprintf(buf, "Set Mode fail\r\n");
			//m_OutputBox.ReplaceSel(buf);
			goto init_fail;
		}
	}
	else
	{
		//sprintf(buf, "Devie is CLOSE.\r\n");
		//m_OutputBox.ReplaceSel(buf);
		goto init_fail;
	}

	return TRUE;

init_fail:
	JIG_BOX_CTRL::iCom = -1;
	return FALSE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_DEINIT(void)
{
	return JIG_BOX_CTRL::bCloseSensor();
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_GET_STATUS(void)
{
	return JIG_BOX_CTRL::iCom;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(unsigned char addr, unsigned char *pValue)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_REG_READ;

	// Parameter
	command[5] = addr;  // Register Address

	// Value
	command[6] = 0; // NONE

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = JIG_BOX_CTRL::bReadComPort(buf, 7, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	if (buf[0] != 'S') {
		return FALSE;
	}
	if (buf[1] != 'I') {
		return FALSE;
	}
	if (buf[2] != 'G') {
		return FALSE;
	}
	if (buf[3] != 'E') {
		return FALSE;
	}

	// buf[4]: param
	*pValue = buf[5];

	if (buf[6] != 0x01) {
		return FALSE;
	}

	return TRUE;
}

/*
BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(unsigned char addr, unsigned char len, unsigned char *pValue)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_REG_READ;

	// Parameter
	command[5] = addr;  // Register Address

	// Value
	command[6] = len; // Length

	if (len > JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX)
		len = JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Value
	r_value = JIG_BOX_CTRL::bReadComPort(pValue, len, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	//return bCheckResponse();
	return TRUE;
}
*/

/*
BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(unsigned char addr, unsigned char Value)
{
	unsigned char pValue[1] = { Value };
	return JIG_BOX_Write_ET727_Register(addr, 1, pValue);
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(unsigned char addr, unsigned char len, unsigned char *pValue)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_REG_WRITE;

	// Parameter
	command[5] = addr;  // Register Address

    // Parameter
	command[6] = len; // Length

	if (len > JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX)
		len = JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX;

    // Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Write Data
	r_value = JIG_BOX_CTRL::bWritComPort(pValue, len);
	if (!r_value) {
		return FALSE;
	}

	return bCheckResponse();
	//return TRUE;
}
*/

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(unsigned char addr, unsigned char value)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_REG_WRITE;

	// Parameter
	command[5] = addr;  // Register Address

	// Parameter
	command[6] = value; // data

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = JIG_BOX_CTRL::bReadComPort(buf, 7, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	if (buf[0] != 'S') {
		return FALSE;
	}
	if (buf[1] != 'I') {
		return FALSE;
	}
	if (buf[2] != 'G') {
		return FALSE;
	}
	if (buf[3] != 'E') {
		return FALSE;
	}

	// buf[4]: param
	// buf[5]: value
	if (buf[6] != 0x01) {
		return FALSE;
	}

	//sprintf(temp, "ET510A_REG_WRITE(%02X, %02X)\n", addr, value);
	//OutputDebugString(temp);

	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register_B(unsigned char *pAddr, unsigned char *pValue, unsigned char len)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

    // Operation code
	command[4] = ET727_SENSOR_REG_READ_B;

	// Parameter
	command[5] = len;  // Length

    // Parameter
	command[6] = 0;

	if (len > JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX)
		len = JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX;

    // Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Write Address
	r_value = JIG_BOX_CTRL::bWritComPort(pAddr, len);
	if (!r_value) {
		return FALSE;
	}

	// Read Data
	r_value = JIG_BOX_CTRL::bReadComPort(pValue, len, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	//return bCheckResponse();
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register_B(unsigned char *pAddr, unsigned char *pValue, unsigned char len)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

    // Operation code
	command[4] = ET727_SENSOR_REG_WRITE_B;

	// Parameter
	command[5] = len;  // Length

    // Parameter
	command[6] = 0;

	if (len > JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX)
		len = JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Write Address
	r_value = JIG_BOX_CTRL::bWritComPort(pAddr, len);
	if (!r_value) {
		return FALSE;
	}

	// Write Value
	r_value = JIG_BOX_CTRL::bWritComPort(pValue, len);
	if (!r_value) {
		return FALSE;
	}

	return bCheckResponse();
	//return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Set_SPI_Mode(unsigned char mode, unsigned char spi_clk)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_SET_SPI_MODE;

	// Parameter
	command[5] = mode;  // Register Address

	// Parameter
	command[6] = spi_clk; // data

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = JIG_BOX_CTRL::bReadComPort(buf, 7, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	if (buf[0] != 'S') {
		return FALSE;
	}
	if (buf[1] != 'I') {
		return FALSE;
	}
	if (buf[2] != 'G') {
		return FALSE;
	}
	if (buf[3] != 'E') {
		return FALSE;
	}

	// buf[4]: param
	// buf[5]: value
	if (buf[6] != 0x01) {
		return FALSE;
	}

	//sprintf(temp, "ET510A_REG_WRITE(%02X, %02X)\n", addr, value);
	//OutputDebugString(temp);

	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Get_SPI_Mode(unsigned char *pValue)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_GET_SPI_MODE;

	// Parameter
	command[5] = 0;  // Register Address

	// Value
	command[6] = 0; // NONE

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = JIG_BOX_CTRL::bReadComPort(buf, 7, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	if (buf[0] != 'S') {
		return FALSE;
	}
	if (buf[1] != 'I') {
		return FALSE;
	}
	if (buf[2] != 'G') {
		return FALSE;
	}
	if (buf[3] != 'E') {
		return FALSE;
	}

	// buf[4]: param
	*pValue = buf[5];

	if (buf[6] != 0x01) {
		return FALSE;
	}

	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Enter_StandBy(void)
{
	BOOL    r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_STANDBY;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, 8);
	if (!r_value) {
		return FALSE;
	}

	return bCheckResponse();
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Wakeup(void)
{
	BOOL    r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_WAKEUP;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, 8);
	if (!r_value) {
		return FALSE;
	}

	return bCheckResponse();
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Get_Power_Status(unsigned char *pwr_status)
{
	BOOL    r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_CHECK_PST;

	// Parameter
	command[5] = 0x00; // Don't care

	// Value
	command[6] = 0x00; // Don't care

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Response
	r_value = JIG_BOX_CTRL::bReadComPort(buf, 7, READ_TIMEOUT);

	if (!r_value) {
		return FALSE;
	}

	if (buf[0] != 'S') {
		return FALSE;
	}
	if (buf[1] != 'I') {
		return FALSE;
	}
	if (buf[2] != 'G') {
		return FALSE;
	}
	if (buf[3] != 'E') {
		return FALSE;
	}

	// buf[4]: param                
	*pwr_status = buf[5];

	if (buf[6] != 0x01) {
		return FALSE;
	}

	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Get_Image(int len, unsigned char *pData)
{
	BOOL r_value;
	unsigned char data = 0;
	unsigned char command[512];
	memset(command, 0x00, sizeof(command));
	int transferlength;
	int time_out = 0;
	transferlength = len;

	do
	{
		//JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, 1, &data);
		JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, &data);
		// check frame status
		if (((data >> FRM_RDY_BIT) & 1) == 0)
		{
			Sleep(2);
			time_out += 1;
		}
		else
			break;
	} while (time_out < 80);

	if (time_out == 80)
		return FALSE;

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_GET_IMAGE;

	// len high byte
	command[5] = (transferlength & 0xFF0000) >> 16;
	command[6] = (transferlength & 0xFF00) >> 8;
	command[7] = (transferlength & 0xFF);

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Image
	r_value = JIG_BOX_CTRL::bReadComPort(pData, transferlength, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Get_Image(unsigned char *pData, int size)
{
	BOOL r_value;
	unsigned char data = 0;
	unsigned char command[512];
	memset(command, 0x00, sizeof(command));
	int frame_pixel = JIG_BOX_ET727_CTRL::width * JIG_BOX_ET727_CTRL::height;
	int frame_pixel_4 = frame_pixel / 4;
	int transferlength = frame_pixel + 48;
	int time_out = 0;
	if (size < frame_pixel)
		return FALSE;

	unsigned char* image_raw = new unsigned char[transferlength];

	do
	{
		//JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, 1, &data);
		JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, &data);
		// check frame status
		if (((data >> FRM_RDY_BIT) & 1) == 0)
		{
			Sleep(2);
			time_out += 1;
		}
		else
			break;
	} while (time_out < 10);

	if (time_out == 10)
		goto fail;

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_GET_IMAGE;

	// len high byte
	command[5] = (transferlength & 0xFF0000) >> 16;
	command[6] = (transferlength & 0xFF00) >> 8;
	command[7] = (transferlength & 0xFF);

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		goto fail;
	}

	// Read Image
	r_value = JIG_BOX_CTRL::bReadComPort(image_raw, transferlength, READ_TIMEOUT);
	if (!r_value) {
		goto fail;
	}

	for (int i = 0; i < 4; i++)
	{
		memcpy(pData + frame_pixel_4 * i, image_raw + frame_pixel_4 * i + 12 * (i + 1), frame_pixel_4);
	}

	delete[] image_raw;
	return TRUE;

fail:
	delete[] image_raw;
	return FALSE;
}

/*
len : frame size + headers
*pData : buffer for image frame (headers included according to strip_header)
strip_header : strip header or not
*/
BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Get_Full_Image_by_Four(int len, unsigned char *pData, bool strip_header, int pixelidx, unsigned int *pValue, unsigned int *pValue2)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char *pSubFrame = NULL;
	int transferlength;
	int badframe_retry = 3;
	unsigned char badframe_detected;
	/***  WORKAROUND for bad frame  ***/
	int dummy_len = 0, dummytail_len = 0, good_found = 0;
	unsigned char dummy_chk = 0;
	/***  WORKAROUND for bad frame  ***/

	transferlength = len / 4;
	pSubFrame = new unsigned char[transferlength+20];

	do
	{
		/***  WORKAROUND for bad frame  ***/
		memset(pSubFrame, 0x00, transferlength);
		dummy_len = 0;
		dummytail_len = 0;
		good_found = 0;
		/***  WORKAROUND for bad frame  ***/
		badframe_detected = 0;
		badframe_retry--;

		/***  ENABLE_ONE_FRAME_MODE  ***/
		int i = pixelidx;
		/***  ENABLE_ONE_FRAME_MODE  ***/
		//for (int i = 0; i < 4; i++)
		{
			if (i == 0)
			{
				JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(MODE_SEL_REG, 0x44);
				dummy_chk = 0x03;   /***  ENABLE_ONE_FRAME_MODE  ***/
			}
			else if (i == 1)
			{
				JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(MODE_SEL_REG, 0x4C);
				dummy_chk = 0x0B;   /***  ENABLE_ONE_FRAME_MODE  ***/
			}
			else if (i == 2)
			{
				JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(MODE_SEL_REG, 0x54);
				dummy_chk = 0x13;  /***  ENABLE_ONE_FRAME_MODE  ***/
			}
			else if (i == 3)
			{
				JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(MODE_SEL_REG, 0x5C);
				dummy_chk = 0x1B;  /***  ENABLE_ONE_FRAME_MODE  ***/
			}

			JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Star_Exposure();

			if (!bCheckFrameReady())
				goto FAIL;

			memset(command, 0x00, sizeof(command));

			// Signature
			command[0] = 0x45; // 'E'
			command[1] = 0x47; // 'G'
			command[2] = 0x49; // 'I'
			command[3] = 0x53; // 'S'

			// Operation code
			command[4] = ET727_SENSOR_GET_IMAGE;

			// len high byte
			command[5] = (transferlength+20 & 0xFF0000) >> 16;
			command[6] = (transferlength+20 & 0xFF00) >> 8;
			command[7] = (transferlength+20 & 0xFF);

			// Write Command
			r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
			if (!r_value) {
				goto FAIL;
			}

			// Read Image
			r_value = JIG_BOX_CTRL::bReadComPort(pSubFrame, transferlength+20, READ_TIMEOUT);
			if (!r_value) {
				goto FAIL;
			}

			/***  WORKAROUND for bad frame  ***/
			for (int i = 0; i < IMAGE_HEADER_SIZE; i ++)
			{
				if (pSubFrame[i] == 0xFF)
				{
  					dummy_len ++;
				}
				else if (pSubFrame[i] == dummy_chk)
				{
  					good_found = 1;
  					break;
				}
			}

			// Check bad frame
			if ((good_found == 0) || ((good_found == 1) && (dummy_len < IMAGE_HEADER_SIZE-1)))
			{
				badframe_detected |= (0x1 << i);
				*pValue = *pValue + 1;

				if((good_found == 1) && (dummy_len < IMAGE_HEADER_SIZE-1))
				{
					for (int i = 0; i < (IMAGE_HEADER_SIZE-1); i ++)
					{
						if (pSubFrame[transferlength-1-i] == 0xFF)
							dummytail_len ++;
					}

					if (dummytail_len != ((IMAGE_HEADER_SIZE-1)-dummy_len))
						*pValue2 = *pValue2 + 1;
				}
			}
			/* check frame size is correct or not, only for IC debug */
			for (int i = 0; i < (transferlength - IMAGE_HEADER_SIZE); i ++)
			{
				if (pSubFrame[dummy_len + 1 + i] != 0x00)
				{
					*pValue2 = *pValue2 + 1000;
					break;
				}
			}

			/* check frame size is correct or not, only for IC debug */
			/***  WORKAROUND for bad frame  ***/
			// Check bad frame
			//if (!(pSubFrame[IMAGE_HEADER_SIZE - 1] & 0x1))
			//{
			//	badframe_detected |= (0x1 << i);
			//}

			// Combine Image
			if (strip_header)
			{
				memcpy(pData + i * (transferlength - IMAGE_HEADER_SIZE), pSubFrame + IMAGE_HEADER_SIZE,
					transferlength - IMAGE_HEADER_SIZE);
			}
			else
			{
				memcpy(pData + i * transferlength, pSubFrame, transferlength);
			}
		} // for (int i = 0; i < 4; i++)

	} while ( (badframe_detected != 0) && (badframe_retry >= 0) );

	/***  ENABLE_ONE_FRAME_MODE  ***/
	/***  WORKAROUND for bad frame  ***/
	if ((badframe_detected != 0) && (badframe_retry < 0))
	{
		if (good_found == 1)
		{
			for (int i = 0; i < 4; i++)
			{
				// Combine Image
				if (strip_header)
				{
					memcpy(pData + i * (transferlength - IMAGE_HEADER_SIZE), pSubFrame + dummy_len + 1,
						transferlength - IMAGE_HEADER_SIZE);
				}
				else
				{
					memset(pData + i * transferlength, 0xFF, transferlength);
               			memcpy(pData + i * transferlength + (IMAGE_HEADER_SIZE - (dummy_len + 1)), pSubFrame, transferlength - (IMAGE_HEADER_SIZE - (dummy_len + 1)));
				}
			}
			badframe_detected = 0;
			badframe_retry = 0;
		}
		else
			goto FAIL;
	}
	else
	{
		for (int i = 0; i < 4; i++)
		{
			// Combine Image
			if (strip_header)
			{
				memcpy(pData + i * (transferlength - IMAGE_HEADER_SIZE), pSubFrame + IMAGE_HEADER_SIZE,
               			transferlength - IMAGE_HEADER_SIZE);
			}
			else
			{
				memcpy(pData + i * transferlength, pSubFrame, transferlength);
			}
		}
	}
	/***  WORKAROUND for bad frame  ***/
	/***  ENABLE_ONE_FRAME_MODE  ***/

	if (pSubFrame)
		delete[] pSubFrame;

	return (badframe_detected == 0);

FAIL:
	if (pSubFrame)
		delete[] pSubFrame;
	return FALSE;
}

unsigned char JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Detect_Bad_Frame(unsigned char *pData)
{
	unsigned char header[4] = { 0 };
	unsigned char ret = 0;
	int total_pixel = JIG_BOX_ET727_CTRL::width * JIG_BOX_ET727_CTRL::height;

	for (int i = 0; i < 4; i++)
	{
		header[i] = pData[(IMAGE_HEADER_SIZE + total_pixel / 4) * i + (IMAGE_HEADER_SIZE - 1)];
	}

	if (!(header[0] & 0x1)) ret |= 0x1;
	if (!(header[1] & 0x1)) ret |= 0x2;
	if (!(header[2] & 0x1)) ret |= 0x4;
	if (!(header[3] & 0x1)) ret |= 0x8;
		
	return ret;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Efuse(unsigned char addr, unsigned char len, unsigned char *pValue)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_EFUSE_READ;

	// Parameter
	command[5] = addr;  // Register Address

	// Value
	command[6] = len; // Length

	if (len > JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX)
		len = JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Read Value
	r_value = JIG_BOX_CTRL::bReadComPort(pValue, len, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	//return bCheckResponse();
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Efuse(unsigned char addr, unsigned char value)
{
	unsigned char pValue[1] = { value };
	return JIG_BOX_Write_ET727_Efuse(addr, 1, pValue);
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Efuse(unsigned char addr, unsigned char len, unsigned char *pValue)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];
	unsigned char data1 = 0, data2 = 0, data3 = 0;

	JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(REG_IF_CLK_00, &data1);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(REG_IF_CLK_00, 0x0A);   // Switch MCLK to 24MHz
	JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(REG_IF_CLK_01, &data2);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(REG_IF_CLK_01, 0x00);   // Switch MCLK to 24MHz
	JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(REG_DIGI_CLK_SET, &data3);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(REG_DIGI_CLK_SET, 0x00);   // Switch MCLK to 24MHz

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_EFUSE_WRITE;

	// Parameter
	command[5] = addr;  // Register Address

	// Parameter
	command[6] = len; // Length

	if (len > JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX)
		len = JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Write Data
	r_value = JIG_BOX_CTRL::bWritComPort(pValue, len);
	if (!r_value) {
		return FALSE;
	}

	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(REG_IF_CLK_00, data1);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(REG_IF_CLK_01, data2);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(REG_DIGI_CLK_SET, data3);

	return bCheckResponse();
	//return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Efuse_B(unsigned char *pAddr, unsigned char *pValue, unsigned char len)
{
	BOOL r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_EFUSE_READ_B;

	// Parameter
	command[5] = len;  // Length

	// Parameter
	command[6] = 0;

	if (len > JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX)
		len = JIG_BOX_ET727_CTRL::CONTINUOUS_COUNT_MAX;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, COMMAND_LENGTH);
	if (!r_value) {
		return FALSE;
	}

	// Write Address
	r_value = JIG_BOX_CTRL::bWritComPort(pAddr, len);
	if (!r_value) {
		return FALSE;
	}

	// Read Data
	r_value = JIG_BOX_CTRL::bReadComPort(pValue, len, READ_TIMEOUT);
	if (!r_value) {
		return FALSE;
	}

	//return bCheckResponse();
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Check_STANDBY()
{
	unsigned char data = 0;
	int time_out = 0;

	do
	{
		Sleep(2);

		//JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, 1, &data);
		JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, &data);
		// check chip status
		if (((data >> CHIP_RDY_BIT) & 1) == 1)
			return TRUE;
		else
			time_out += 1;
	} while (time_out < CHECK_TIMEOUT*10);

	return FALSE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Star_Exposure()
{
	unsigned char data = 0;
	//JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, 1, &data);
	JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, &data);
	data |= (1 << EXPO_STS_BIT);
	return JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(CIS_STATUS_REG, data);
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Stop_Exposure()
{
	unsigned char data = 0;
	//JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, 1, &data);
	JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, &data);
	data &= ~(1 << EXPO_STS_BIT);
	return JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(CIS_STATUS_REG, data);
}

BOOL JIG_BOX_ET727_CTRL::JIB_BOX_ET727_SetINTG(unsigned __int16 INTG)
{
	unsigned char data[2];
	data[0] = (INTG & 0xFF00) >> 8;
	data[1] = (INTG & 0xFF);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(EXPO_H_REG, data[0]);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(EXPO_L_REG, data[1]);
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIB_BOX_ET727_SetLineLength(unsigned __int16 LineLength)
{
	unsigned char data[2];
	data[0] = (LineLength & 0xFF00) >> 8;
	data[1] = (LineLength & 0xFF);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(LINELENGTH_H_REG, data[0]);
	JIG_BOX_ET727_CTRL::JIG_BOX_Write_ET727_Register(LINELENGTH_L_REG, data[1]);
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_Set_GPIO(unsigned char pin, unsigned char mode, unsigned char data)
{
	/* mode: 0->input, 1->output */

	BOOL ret = JIG_BOX_CTRL::bSetGPIOMode(pin, mode);
	if (!ret)
		return ret;
	return JIG_BOX_CTRL::bSetGPIOData(pin, data);
}

BOOL JIG_BOX_ET727_CTRL::JIG_BOX_ET727_SetMCLK(unsigned char clock_value) 
{
	BOOL    r_value;
	unsigned char command[512];
	unsigned char buf[512];

	memset(command, 0x00, sizeof(command));
	memset(buf, 0x00, sizeof(buf));

	// Signature
	command[0] = 0x45; // 'E'
	command[1] = 0x47; // 'G'
	command[2] = 0x49; // 'I'
	command[3] = 0x53; // 'S'

	// Operation code
	command[4] = ET727_SENSOR_MCLK_SELECT; 

	command[5] = clock_value;

	// Write Command
	r_value = JIG_BOX_CTRL::bWritComPort(command, 8);
	if (!r_value) {
		return FALSE;
	}

	return bCheckResponse();
}

BOOL JIG_BOX_ET727_CTRL::bCheckResponse()
{
	BOOL r_value;
	unsigned char buf[512];

	r_value = JIG_BOX_CTRL::bReadComPort(buf, 7, READ_TIMEOUT);

	if (!r_value)       return FALSE;
	if (buf[0] != 'S')  return FALSE;
	if (buf[1] != 'I')  return FALSE;
	if (buf[2] != 'G')  return FALSE;
	if (buf[3] != 'E')  return FALSE;
	if (buf[6] != 0x01) return FALSE;
	
	return TRUE;
}

BOOL JIG_BOX_ET727_CTRL::bCheckFrameReady()
{
	unsigned char data = 0;
	int time_out = 0;

	do
	{
		JIG_BOX_ET727_CTRL::JIG_BOX_Read_ET727_Register(CIS_STATUS_REG, &data);
		// check frame status
		if (((data >> FRM_RDY_BIT) & 1) == 0)
		{
			Sleep(2);
			time_out += 1;
		}
		else
			break;
	} while (time_out < 800);

	return (time_out < 800);
}
