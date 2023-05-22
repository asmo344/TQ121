// JIG_BOX.h

#pragma once

#ifndef JIG_BOX_DLL_H
#define JIG_BOX_DLL_H

using namespace System;

namespace JIG_BOX {
	public ref class JIG_BOX_ET727_CTRL
	{
	public:
		static void bLEDBackLightOn(BOOL bOn, unsigned char led_gain, unsigned char *pLightness);
		static BOOL JIG_BOX_ET727_INIT(int Com);
		static BOOL JIG_BOX_ET727_DEINIT(void);
		static BOOL JIG_BOX_ET727_GET_STATUS(void);
		static BOOL JIG_BOX_ET727_Check_STANDBY();
		static BOOL JIG_BOX_ET727_Star_Exposure();
		static BOOL JIG_BOX_ET727_Stop_Exposure();
		static BOOL JIB_BOX_ET727_SetINTG(unsigned __int16 INTG);
		static BOOL JIB_BOX_ET727_SetLineLength(unsigned __int16 LineLength);
		static BOOL JIG_BOX_Read_ET727_Register(unsigned char addr, unsigned char *pValue);
		//static BOOL JIG_BOX_Read_ET727_Register(unsigned char addr, unsigned char len, unsigned char *pValue);
		static BOOL JIG_BOX_Write_ET727_Register(unsigned char addr, unsigned char value);
		//static BOOL JIG_BOX_Write_ET727_Register(unsigned char addr, unsigned char len, unsigned char *pValue);
		static BOOL JIG_BOX_Read_ET727_Register_B(unsigned char *pAddr, unsigned char *pValue, unsigned char len);
		static BOOL JIG_BOX_Write_ET727_Register_B(unsigned char *pAddr, unsigned char *pValue, unsigned char len);
		static BOOL JIG_BOX_Set_SPI_Mode(unsigned char mode, unsigned char spi_clk);
		static BOOL JIG_BOX_Get_SPI_Mode(unsigned char *pValue);
		static BOOL JIG_BOX_ET727_Enter_StandBy(void);
		static BOOL JIG_BOX_ET727_Wakeup(void);
		static BOOL JIG_BOX_ET727_Get_Power_Status(unsigned char *pwr_status);
		static BOOL JIG_BOX_ET727_Get_Image(int len, unsigned char *pData);
		static BOOL JIG_BOX_ET727_Get_Image(unsigned char *pData, int size);
		static BOOL JIG_BOX_ET727_Get_Full_Image_by_Four(int len, unsigned char *pData, bool strip_header, int pixelidx, unsigned int *pValue, unsigned int *pValue2);
		static BOOL JIG_BOX_Read_ET727_Efuse(unsigned char addr, unsigned char len, unsigned char *pValue);
		static BOOL JIG_BOX_Write_ET727_Efuse(unsigned char addr, unsigned char value);
		static BOOL JIG_BOX_Write_ET727_Efuse(unsigned char addr, unsigned char len, unsigned char *pValue);
		static BOOL JIG_BOX_Read_ET727_Efuse_B(unsigned char *pAddr, unsigned char *pValue, unsigned char len);
		static BOOL JIG_BOX_ET727_Set_GPIO(unsigned char pin, unsigned char mode, unsigned char data);
		static BOOL JIG_BOX_ET727_SetMCLK(unsigned char clock_value);
		static BOOL Reorder_Image(unsigned char* src_image);
		static BOOL binaryToGray(unsigned char* src_image);
		static BOOL grayToBinary(unsigned char* src_image);
		static BOOL Combine_Image(unsigned char* src_image, unsigned char* dst_image);
		static BOOL Combine_image_ET728(int width, int height, unsigned char* src_image, int* dst_image);
		static BOOL Eight_Exposure_Image(unsigned char* src_image, UInt16* dst_image);
		static unsigned char JIG_BOX_ET727_Detect_Bad_Frame(unsigned char *pData);
		static const int width = 240;
		static const int height = 258;
		static const int CONTINUOUS_COUNT_MAX = 20;
		static const int IMAGE_HEADER_SIZE = 12;
	private:
		static BOOL bCheckResponse();
		static BOOL bCheckFrameReady();
	};

	private ref class JIG_BOX_CTRL
	{
	public:
		static int iCom = -1;
		static BOOL bOpenSensor(void);
		static BOOL bCloseSensor(void);
		static BOOL bWriteRegisterSingleByte(unsigned char addr, unsigned char value, unsigned char I2CAddr);
		static BOOL bReadComPort(unsigned char *pReadBuf, int iReadBufLen, int timeout);
		static BOOL bWritComPort(unsigned char *pWriteBuf, int iWriteBufLen);
		static BOOL bSetGPIOMode(unsigned char Pin, unsigned char Mode);
		static BOOL bSetGPIOData(unsigned char Pin, unsigned char Data);
	};
}

#endif
