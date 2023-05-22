#include "stdafx.h"
#include "ModuleDef.h"   
#include "fps_stru.h"
#include "fpsapi.h"
#include "usbemu.h"

#include "cis_cmd.h"
#include "ModuleDef.h"
#include "OpticalSensor.h"
#include <iostream>
#include <math.h>

extern BYTE otgversion;
extern BYTE _devicecount;

double GetADCclk();
int WriteRegister(DWORD Address, BYTE Data);
int ReadRegister(DWORD Address, BYTE* Data);
int CaptureFrame(int* imagePtr);
double CalcGain(double gain);
void SetExposureTime(double exposureTime);

double mclkfreqsel[] = {11.5, 13.1, 14.5, 16.0, 17.3, 18.7, 20.0, 21.3, 22.6, 23.8,
25.0, 26.3, 27.4, 28.6, 29.7, 30.9, 31.9, 33.0, 34.0, 35.1,
36.2, 37.2, 38.3, 39.3, 40.3, 41.3, 42.3, 43.3, 44.3, 45.2,
46.2, 47.1, 48.0, 49.0, 49.9, 50.8, 51.7, 52.6, 53.5, 54.4,
55.3, 56.1, 57.0, 57.9, 58.7, 59.6, 60.4, 61.2, 62.0, 62.8,
63.6, 64.4, 65.2, 66.0, 66.8, 67.6, 68.4, 69.2, 69.9, 70.7,
71.5, 72.2, 73.0, 73.8};

void ClrReadFullImageData(BYTE* imageBuf)
{
	BYTE total_page_num = 5, page_size = 16;
	BYTE* imagedata;
	BYTE* data;

	imagedata = (BYTE *)malloc(5 * page_size * 1024);
	data = (BYTE *)malloc(page_size * 1024);
	memset(imagedata, 0, 5 * page_size * 1024);
	memset(data, 0, page_size * 1024);

	for (int i = 0; i < total_page_num; i++)
	{
		ReadFullImageData(&_devicecount, i, page_size, data);
		memcpy(imagedata + i * page_size * 1024, data, page_size * 1024);
	}

	BYTE temp1, temp2;
	for (int i = 0; i < 300 * 258; i += 2)
	{
		temp1 = imagedata[i];
		temp2 = imagedata[i + 1];
		imagedata[i] = temp2;
		imagedata[i + 1] = temp1;
	}

	memcpy(imageBuf, imagedata, 300 * 258);
}

void CombineImage(int width, int height, BYTE* src_image, int* dst_image)
{
	int Total_pixel = height * width;
	int pixelvalue, intdata1, intdata2;
	unsigned char filter = 0b00000000;
	size_t ptr = 0;

	BYTE* mData;
	mData = (BYTE *)malloc(300 * 240);

	memcpy(mData, src_image + 300 * 18, 300 * 240);

	for (int i = 0; i < Total_pixel / 4; i++) {
		intdata2 = (int)mData[ptr + 4];
		for (int j = 0; j < 4; j++) {
			intdata1 = (int)mData[ptr + j];
			filter = 3 * pow(4, 3 - j);
			pixelvalue = ((intdata1 << 2) | ((intdata2 & filter) >> ((3 - j) * 2)));
			dst_image[i * 4 + j] = pixelvalue;
		}

		ptr += 5;
	}
}

int storeimage(int* data, int size, char* file) {
	FILE *stream;
	errno_t err;
	char   buf[MAX_PATH];
	BYTE* mEightBitData;
	mEightBitData = (BYTE *)malloc(size);

	for (int i = 0; i < size; i++)
	{
		mEightBitData[i] = data[i] >> 2;
	}

	err = fopen_s(&stream, file, "wb+");
	if (err != 0)
	{
		sprintf_s(buf, "The file 'TestPattern.raw' was not opened\n");
		return -1;
	}

	fwrite(mEightBitData, sizeof(BYTE), size, stream);

	if (stream)
	{
		err = fclose(stream);
	}
	free(mEightBitData);
	return 0;
}

void get16seghistogram(int* image, int imageLength, unsigned int* histogram)
{
	memset(histogram, 0, sizeof(unsigned int) * 16);

	int seglength = 1024 / 16;
	for (int i = 0; i < imageLength; i++)
	{
		histogram[(image[i] / seglength)]++;
	}

	/*for (int sz = 0; sz < 16; sz++)
	{
		std::cout << "hisrange[" << sz << "] = " << histogram[sz] << std::endl;
	}*/
}

void sensorInit()
{
	BYTE data;
	SetExposureTime(16.66667);
	WriteRegister(0x18, 0);
	WriteRegister(0x19, 0);
	WriteRegister(0x1A, 0);
	WriteRegister(0x1B, 0);
	WriteRegister(0x1E, 0);
	WriteRegister(0x1F, 0);
	WriteRegister(0x09, 0x1F);
}

void AdjustAEOffset(int offset_add, int offset_minus)
{
	BYTE REG_0x18, REG_0x19, REG_0x1A, REG_0x1B, REG_0x1E, REG_0x1F;
	offset_minus = offset_minus / 2;
	if ((offset_add != 0) && (offset_minus != 0))
	{
		// either offset_add or offset_minus should be 0
		return;
	}

	REG_0x18 = offset_add & 0x00FF;
	REG_0x19 = (REG_0x19 & 0b11111100) + ((offset_add >> 8) & 0b11);
	REG_0x1A = REG_0x18;
	REG_0x1B = REG_0x19;
	REG_0x1E = offset_minus & 0x00FF;
	REG_0x1F = (REG_0x1F & 0b11111100) + ((offset_minus >> 8) & 0b11);

	//std::cout << "REG_0x1E = " << (int)REG_0x1E << std::endl;
	//std::cout << "REG_0x1F = " << (int)REG_0x1F << std::endl;
	WriteRegister(0x18, REG_0x18);
	WriteRegister(0x19, REG_0x19);
	WriteRegister(0x1A, REG_0x1A);
	WriteRegister(0x1B, REG_0x1B);
	WriteRegister(0x1E, REG_0x1E);
	WriteRegister(0x1F, REG_0x1F);
	WriteRegister(0x09, 0x1F);
}

double AdjustAEValue_new(int* data, int dataLength, int threshold)
{
	int SENSOR_WIDTH = 240;
	int SENSOR_HEIGHT = 240;
	double Total_gain = 1;
	int hismin = 0;
	int hismax = 0;
	int hisrange = 0;
	BYTE reg0x93 = 0, reg0x94 = 0;
	ReadRegister(0x93, &reg0x93);
	ReadRegister(0x94, &reg0x94);
	unsigned int initexpo = (reg0x94 << 8) + reg0x93;
	unsigned int Total_exposure = 0;

	const int histogramLength = 16;
	unsigned int histogram[histogramLength];

	for (int sz = 0; sz < histogramLength; sz++)
	{
		histogram[sz] = 0;
	}

	get16seghistogram(data, dataLength, histogram);

	for (int i = 0; i < histogramLength; i++)
	{
		if (histogram[i] > threshold)
		{
			hismin = i;
			break;
		}
	}

	for (int i = histogramLength - 1; i >= 0; i--)
	{
		if (histogram[i] > threshold)
		{
			hismax = i;
			break;
		}
	}

	hisrange = hismax - hismin + 1;
	//std::cout << "hisrange = " << (int)hisrange << std::endl;
	double doublemax = (double)hismax;

	Total_exposure = (16 * initexpo) / (doublemax + 1);
	if (Total_exposure > 4 * initexpo)
	{
		Total_exposure = 4 * initexpo;
	}

	//std::cout << "Total_exposure = " << Total_exposure << std::endl;
	reg0x94 = (Total_exposure & 0xFF00) >> 0x8;
	reg0x93 = (Total_exposure & 0x00FF);
	//std::cout << "hismax = " << (int)hismax << std::endl;
	//std::cout << "hismin = " << (int)hismin << std::endl;
	//std::cout << "initexpo = " << initexpo << std::endl;
	//std::cout << "Total_exposure = " << Total_exposure << std::endl;

	double Total_gain2 = (double)(16 * initexpo) / (double)(Total_exposure * hisrange);

	Total_gain = CalcGain(Total_gain2);

	int newmin = (int)(64 * hismin * (Total_exposure / initexpo));

	WriteRegister(0x94, reg0x94);
	WriteRegister(0x93, reg0x93);
	WriteRegister(0x09, 0x1F);

	AdjustAEOffset(0, newmin);
	//std::cout << "newmin = " << (int)newmin << std::endl;
	return Total_gain;
}

int GetImageAE(int* imagePtr)
{
	BYTE* imageBuf = (BYTE *)malloc(300 * 258);
	int SENSOR_WIDTH = 240, SENSOR_HEIGHT = 240;
	//int* imagePtr = (int *)malloc(SENSOR_WIDTH * SENSOR_HEIGHT * sizeof(int));
	char buf[MAX_PATH];
	double gain;

	sensorInit();

	CaptureFrame(imagePtr);
	storeimage(imagePtr, SENSOR_WIDTH * SENSOR_HEIGHT, "ImageAeBefore.raw");

	gain = AdjustAEValue_new(imagePtr, SENSOR_WIDTH * SENSOR_HEIGHT, 500);
	//std::cout << "Arder AE : gaine = " << gain << std::endl;

	CaptureFrame(imagePtr);
	const int histogramLength = 16;
	unsigned int histogram[histogramLength];
	get16seghistogram(imagePtr, SENSOR_WIDTH * SENSOR_HEIGHT, histogram);
	//storeimage(imagePtr, SENSOR_WIDTH * SENSOR_HEIGHT, "ImageAeAfter1.raw");

	for (int sz = 0; sz < SENSOR_WIDTH * SENSOR_HEIGHT; sz++)
	{

		imagePtr[sz] = (int)((double)imagePtr[sz] * gain);
		if (imagePtr[sz] > 1023)
			imagePtr[sz] = 1023;

	}
	storeimage(imagePtr, SENSOR_WIDTH * SENSOR_HEIGHT, "ImageAeAfter.raw");

	return 0;
}

double GetExposureTime()
{
	BYTE mLineLengthL, mLineLengthH;
	BYTE mIntgL, mIntgH;
	int mLineLength, mIntg;
	double mADCclk;

	ReadRegister(0x91, &mLineLengthL);
	ReadRegister(0x92, &mLineLengthH);
	mLineLength = (mLineLengthH << 8) | mLineLengthL;
	
	ReadRegister(0x93, &mIntgL);
	ReadRegister(0x94, &mIntgH);
	mIntg = (mIntgH << 8) | mIntgL;

	mADCclk = GetADCclk();

	return (1 / mADCclk) * 1000 * mLineLength * mIntg;
}

double GetADCclk()
{
	BYTE REG_0xFB, REG_0x50, REG_0x51, REG_0x0A;
	//byte data1, data2, data3;
	BYTE mclkdiv, mclkfreq;
	double mclk = 0, ADCclk;

	ReadRegister(0xFB, &REG_0xFB);
	if (REG_0xFB == 24) // MCLK_EN = enable
	{
		mclk = 24000000;
	}
	else
	{
		ReadRegister(0x50, &REG_0x50);
		mclkfreq = (byte)(REG_0x50 & 0x3F);
		mclk = mclkfreqsel[mclkfreq] * 1000000;
	}

	ReadRegister(0x51, &REG_0x51);
	mclkdiv = (byte)(REG_0x51 & 0x3);

	mclk = mclk / pow(2, mclkdiv);

	ReadRegister(0x0A, &REG_0x0A);
	if ((REG_0x0A & 0b00100000) == 0)
	{
		ADCclk = mclk;
	}
	else
	{
		if ((REG_0x0A & 0b00010000) == 0)
		{
			return -1;
		}
		else
		{
			ADCclk = (double)(mclk / (2 * ((REG_0x0A & 0b00001111) + 1)));
		}
	}
	return ADCclk;
}

void SetExposureTime(double exposureTime)
{
	BYTE mLineLengthL, mLineLengthH;
	BYTE mIntgL, mIntgH;
	int mLineLength, mIntg;
	double mADCclk;

	ReadRegister(0x91, &mLineLengthL);
	ReadRegister(0x92, &mLineLengthH);
	mLineLength = (mLineLengthH << 8) | mLineLengthL;

	mADCclk = GetADCclk();
	double mIntgDebug = exposureTime * mADCclk / (1000 * mLineLength);

	mIntg = (BYTE)(exposureTime * mADCclk / (1000 * mLineLength));
	mIntgH = (mIntg & 0xFF00) >> 0x8;
	mIntgL = (mIntg & 0x00FF);

	WriteRegister(0x93, mIntgL);
	WriteRegister(0x94, mIntgH);
}

int WriteRegister(DWORD Address, BYTE Data)
{
	int ret;
	if (otgversion == 0x01)
	{
		ret = WriteOneCISRegister(&_devicecount, Address, Data);
	}
	else if (otgversion == 0x00 || otgversion == 0x02 || otgversion == 123) //ECFA version read
	{
		ret = WriteECFAOneCISRegister(&_devicecount, Address, Data);
	}
	return ret;
}

int ReadRegister(DWORD Address, BYTE* Data)
{
	int ret;
	if (otgversion == 0x01)
	{
		ret = ReadOneCISRegister(&_devicecount, Address, Data);
	}
	else if (otgversion == 0x00 || otgversion == 0x02 || otgversion == 123) //ECFA version read
	{
		ret = ReadECFAOneCISRegister(&_devicecount, Address, Data);
	}
	return ret;
}

int CaptureFrame(int* imagePtr)
{
	BYTE* imageBuf = (BYTE *)malloc(300 * 258);
	BYTE data;
	int delay_time = 2000; // 2s
	int SENSOR_WIDTH = 240, SENSOR_HEIGHT = 240;
	
	StartCaptureImage(&_devicecount, 300, 258, 10);

	if (otgversion == 0x00 || otgversion == 0x02 || otgversion == 123)
	{
		while (delay_time >= 0)
		{
			CheckDataReady(&_devicecount, &data);
			if (data == 123)
				break;
			delay_time -= 20;
			Sleep(20);
		}
		if (delay_time < 0)
		{
			std::cout << "Delay time out" << std::endl;

			return false;
		}
	}
	else
	{
		Sleep(50);
	}

	ClrReadFullImageData(imageBuf);
	CombineImage(SENSOR_WIDTH, SENSOR_HEIGHT, imageBuf, imagePtr);
	free(imageBuf);
}

double CalcGain(double Total_gain2)
{
	double gain;
	if (Total_gain2 < (16.00 / 15.00))
	{
		gain = 1;
	}
	else if (Total_gain2 < (16.00 / 14.00))
	{
		gain = (16.00 / 15.00);
	}
	else if (Total_gain2 < (16.0 / 13.0))
	{
		gain = (16.0 / 14.0);
	}
	else if (Total_gain2 < (16.0 / 12.0))
	{
		gain = (16.0 / 13.0);
	}
	else if (Total_gain2 < (16.0 / 11.0))
	{
		gain = (16.0 / 12.0);
	}
	else if (Total_gain2 < (16.0 / 10.0))
	{
		gain = (16.0 / 11.0);
	}
	else if (Total_gain2 < (16.0 / 9.0))
	{
		gain = (16.0 / 10.0);
	}
	else if (Total_gain2 < (16.0 / 8.0))
	{
		gain = (16.0 / 9.0);
	}
	else if (Total_gain2 < (16.0 / 7.0))
	{
		gain = (16.0 / 8.0);
	}
	else if (Total_gain2 < (16.0 / 6.0))
	{
		gain = (16.0 / 7.0);
	}
	else if (Total_gain2 < (16.0 / 5.0))
	{
		gain = (16.0 / 6.0);
	}
	else if (Total_gain2 < (16.0 / 4.0))
	{
		gain = (16.0 / 5.0);
	}
	else if (Total_gain2 < (16.0 / 3.0))
	{
		gain = (16.0 / 4.0);
	}
	else if (Total_gain2 < (16.0 / 2.0))
	{
		gain = (16.0 / 3.0);
	}
	else if (Total_gain2 < (16.0 / 1.0))
	{
		gain = (16.0 / 2.0);
	}
	else
	{
		gain = 16;
	}

	return gain;
}