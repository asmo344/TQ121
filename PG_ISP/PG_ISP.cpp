#include "stdafx.h"
#include "PG_ISP.h"
#include "math.h"
#include "Matrix_M_N.h"
#include "algorithm_basic.h"
#include <algorithm>
#include <vector>

using namespace PG_ISP;

uint16_t ISP::DownSampling(uint16_t nImageW, uint16_t nImageH, uint16_t nDstWidth, uint16_t nDstHeight, uint8_t* pImage)
{
	uint16_t RESULT_ARRAY_SIZE = algorithm_basic::InitialResultArraySize(nImageW, nImageH);
	uint16_t nRatioX = (uint16_t)nImageW / nDstWidth;
	uint16_t nRatioY = (uint16_t)nImageH / nDstHeight;
	uint8_t* aResult = new uint8_t[RESULT_ARRAY_SIZE];
	uint16_t nIndex = 0;
	uint32_t nSingle = 0;
	uint16_t nPerc = 0;

	for (uint16_t i = 0; (i) < nImageH; i = i + nRatioX)
	{
		for (uint16_t j = 0; (j) < nImageW; j = j + nRatioY)
		{
			nSingle = uint8_t(pImage[(i + (nRatioX / 2)) * nImageW + (j + (nRatioY / 2))]);

			aResult[nIndex] = nSingle;
			nIndex++;
		}
	}

	//第一步:
	algorithm_basic::Step1PixelOfCluster(nDstWidth, nDstHeight, aResult);//BB 1 ~ 5

	//第二步:
	//printf("nDstWidth=%d,nDstWidth=%d\n", nDstWidth, nDstWidth);	 
	algorithm_basic::Step2PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB1Map, t_BlockCluster.aBB1Map, t_BlockCluster.aBB2Map, Class2); //GB 2
	algorithm_basic::Step2PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB1Map, t_BlockCluster.aBB2Map, t_BlockCluster.aBB3Map, Class4); //GB 4

	//第三步:
	algorithm_basic::Step3PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB3Map, t_BlockCluster.aBB3Map, t_BlockCluster.aBB4Map, Class3); //GB 3

	//第四步:
	algorithm_basic::Step4PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB4Map, t_BlockCluster.aBB4Map, t_BlockCluster.aBB5Map, Class1); //GB 1
	algorithm_basic::Step4PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB4Map, t_BlockCluster.aBB5Map, t_BlockCluster.aBB6Map, Class5); //GB 5
	algorithm_basic::Step4PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB4Map, t_BlockCluster.aBB6Map, t_BlockCluster.aBB7Map, Class4); //GB 4
	algorithm_basic::Step4PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB4Map, t_BlockCluster.aBB7Map, t_BlockCluster.aBB8Map, Class2); //GB 2

	//第五步:
	nPerc = algorithm_basic::Step5PixelOfCluster(nDstWidth, nDstHeight, t_BlockCluster.aBB8Map);

	delete aResult;
	return nPerc;
}

BOOL ISP::DeadPixelCorrection(
	unsigned char* src_image,
	unsigned int width,
	unsigned int height,
	unsigned int threshold_floor,
	unsigned int threshold_ceiling,
	unsigned char* dst_image,
	unsigned char* dead_pixel_image)
{
	unsigned int x, y;

	if (!src_image || !dst_image || !dead_pixel_image ||
		(width == 0) || (height == 0) || (threshold_ceiling > 255) || (threshold_floor > 255))
		return FALSE;

	// Collection all dead pixels according to the threshold
	for (y = 0; y < height; y++)
	{
		for (x = 0; x < width; x++)
		{
			int index = x + y * width;
			dead_pixel_image[index] =
				(src_image[index] < threshold_floor || src_image[index] > threshold_ceiling) ? 1 : 0;
		}
	}

	// Correct all dead pixels
	for (y = 0; y < height; y++)
	{
		for (x = 0; x < width; x++)
		{
			int index = x + y * width;
			int num_of_samples = 0;
			int value = 0;

			if (dead_pixel_image[index] == 1)  // dead pixel
			{
				int left = ((int)x - 1 > -1) ? index - 1 : -1;
				int top = ((int)y - 1 > -1) ? index - width : -1;
				int right = ((x + 1) < width) ? index + 1 : -1;
				int bottom = ((y + 1) < height) ? index + width : -1;

				// Check four corners first
				if (x == 0 && y == 0 && dead_pixel_image[right] == 0 && dead_pixel_image[bottom] == 0)
				{
					value += src_image[right] + src_image[bottom];
					num_of_samples += 2;
				}
				else if (x == width - 1 && y == 0 && dead_pixel_image[left] == 0 && dead_pixel_image[bottom] == 0)
				{
					value += src_image[left] + src_image[bottom];
					num_of_samples += 2;
				}
				else if (x == width - 1 && y == height - 1 && dead_pixel_image[top] == 0 && dead_pixel_image[left] == 0)
				{
					value += src_image[top] + src_image[left];
					num_of_samples += 2;
				}
				else if (x == 0 && y == height - 1 && dead_pixel_image[top] == 0 && dead_pixel_image[right] == 0)
				{
					value += src_image[top] + src_image[right];
					num_of_samples += 2;
				}
				else  // others
				{
					if (left > -1 && dead_pixel_image[left] == 0 &&
						right > -1 && dead_pixel_image[right] == 0)
					{
						value += src_image[left] + src_image[right];
						num_of_samples += 2;
					}

					if (top > -1 && dead_pixel_image[top] == 0 &&
						bottom > -1 && dead_pixel_image[bottom] == 0)
					{
						value += src_image[top] + src_image[bottom];
						num_of_samples += 2;
					}
				}
			}

			// if (num_of_samples == 0) means no good neighbors for correction. set pixel value the same as source.
			dst_image[index] = (num_of_samples > 0) ? (value / num_of_samples) : src_image[index];
		}
	}

	return TRUE;
}

bool ISP::fpn_collect(unsigned char* frame, int frame_size)
{
	double mean = 0;
	double* fpn_table;
	fpn_table = new double[frame_size];
	memset(fpn_table, 0, frame_size);
	// Cal the mean = sum(total_pixel) / total_pixel
	for (int index = 0; index < frame_size; index++)
	{
		mean += frame[index];
	}
	mean /= frame_size;

	for (int index = 0; index < frame_size; index++)
	{
		fpn_table[index] = frame[index] - mean;
	}

	delete[] fpn_table;
	return true;
}

bool ISP::fpn_collect(unsigned char* src, int frame_size, int num_frame, double* dst)
{
	double mean = 0;
	//double* fpn_table;
	//fpn_table = new double[frame_size];
	memset(dst, 0, frame_size);
	for (int index = 0; index < frame_size; index++)
	{
		for (unsigned short f = 0; f < num_frame; f++)
		{
			dst[index] = dst[index] + src[index + f * frame_size];
			mean += src[index + f * frame_size];
		}
	}
	mean /= frame_size;

	for (int index = 0; index < frame_size; index++)
	{
		dst[index] = (dst[index] - mean) / num_frame;
	}
	//delete[] fpn_table;
	return true;
}

ISPERR ISP::FpnCorrection(unsigned char* frame_in, char* frame_out, double* table, unsigned depth, unsigned int width, unsigned int height)
{
#if 0
	if (frame_in == NULL || frame_out == NULL || table == NULL)
		return ISPERR::INVALID_ARGUMENT;

	if (depth < 1 || width < 1 || height < 1)
		return ISPERR::INVALID_ARGUMENT;

	int data = 0;
	for (unsigned int i = 0; i < height; i++)
	{
		for (unsigned int j = 0; j < width; j++)
		{
			data = ((double)frame_in[j + i * width] - (table[j + i * width] * (double)depth)) + 0.5;

			if (data < 0) frame_out[j + i * width] = 0;
			else if (data > 255) frame_out[j + i * width] = 255;
			else frame_out[j + i * width] = (char)data;
		}
	}
#endif
	return ISPERR::UNKNOWN;
}

ISPERR ISP::FpnCorrection(unsigned char* frame_in, unsigned int frame_width, unsigned int frame_height,
	double* table, unsigned int table_width, unsigned int table_height, unsigned depth, unsigned header_size)
{
	if (frame_in == NULL || table == NULL)
		return ISPERR::INVALID_ARGUMENT;

	if (depth < 1 || frame_width < 1 || frame_height < 1)
		return ISPERR::INVALID_ARGUMENT;

	double* tbl = NULL;
	unsigned total_pixel = frame_width * frame_height;
	bool expand = false;

	if (table_width < frame_width && table_height < frame_height)
	{
		tbl = new double[total_pixel];

		ExpandTable(tbl, table, table_width, table_height, frame_width, frame_height);
		expand = true;
	}
	else if (table_width == frame_width && table_height == frame_height)
	{
		tbl = table;
	}

	unsigned char* p = frame_in;

	int j = 0, data = 0;
	for (int i = 0; i < 4; i++)
	{
		p += header_size;

		for (unsigned int k = 0; k < total_pixel / 4; k++, j++)
		{
			data = (int)(((double)p[j] - (tbl[j] * (double)depth)) + 0.5);

			if (data < 0) p[j] = 0;
			else if (data > 255) p[j] = 255;
			else p[j] = (char)data;
		}
	}

	if (expand)
		delete[] tbl;

	return ISPERR::SUCCESS;
}

void ISP::ExpandTable(double* tbl, double* table, unsigned int table_width, unsigned int table_height, unsigned int frame_width, unsigned int frame_height)
{
	unsigned int block_width = frame_width / table_width;
	unsigned int block_height = frame_height / table_height;

	for (int h = 0; (unsigned int)h < table_height; h++)
	{
		for (int w = 0; (unsigned int)w < table_width; w++)
		{
			double v = table[h * table_width + w];

			int by = h * block_height;
			int bx = w * block_width;

			double* p = &tbl[by * frame_width + bx];

			for (int y = 0; (unsigned int)y < block_height; y++)
			{
				for (int x = 0; (unsigned int)x < block_width; x++)
				{
					*(p++) = v;
				}
				p = p - block_width + frame_width;
			}
		}
	}
}

void Blur(int* src, int* dst, unsigned int width, unsigned int height, unsigned int blurLevel, double* kernel)
{
	int ksize = blurLevel / 2;
	double ksum = 0, sum = 0;
	int kcount = 0;

	for (int i = 0; i < (int)(blurLevel * blurLevel); i++)
	{
		ksum += kernel[i];
	}

	for (int y = 0; y < (int)height; y++)
	{
		for (int x = 0; x < (int)width; x++)
		{
			sum = 0;
			kcount = 0;
			for (int top = y - ksize; top <= y + ksize; top++)
			{
				for (int left = x - ksize; left <= x + ksize; left++)
				{
					int srcx = left < 0 ? Math::Abs(left + 1) : left >= (int)width ? (width - (left - width + 1)) : left;
					int srcy = top < 0 ? Math::Abs(top + 1) : top >= (int)height ? (height - (top - height + 1)) : top;
					sum += src[srcx + srcy * width] * kernel[kcount++];
				}
			}
			dst[x + y * width] = sum / ksum;
		}
	}
}

void ISP::HomogeneousBlur(int* src, int* dst, unsigned int width, unsigned int height, unsigned int blurLevel)
{
	double* kernel = new double[blurLevel * blurLevel];

	for (int i = 0; i < (int)(blurLevel * blurLevel); i++)
	{
		kernel[i] = 1;
	}

	Blur(src, dst, width, height, blurLevel, kernel);
}

void ISP::GaussianBlur(int* src, int* dst, unsigned int width, unsigned int height, unsigned int blurLevel, double standardDeviation)
{
	int ksize = blurLevel / 2;
	double base = 1 / (2 * Math::PI * standardDeviation * standardDeviation);
	double* kernel = new double[blurLevel * blurLevel];
	double kernelbase = 0;
	int kcount = 0;

	for (int x = -ksize; x <= ksize; x++)
	{
		for (int y = -ksize; y <= ksize; y++)
		{
			kernel[kcount] = base * Math::Pow(Math::E, (-1) * (x * x + y * y) / (2 * standardDeviation * standardDeviation));
			kcount++;
		}
	}

	kernelbase = kernel[0];
	kcount = 0;
	for (int x = -ksize; x <= ksize; x++)
	{
		for (int y = -ksize; y <= ksize; y++)
		{
			kernel[kcount] /= kernelbase;
			kcount++;
		}
	}

	Blur(src, dst, width, height, blurLevel, kernel);
}

void ISP::MedianBlur(int* src, int* dst, unsigned int width, unsigned int height, unsigned int blurLevel)
{
	int ksize = blurLevel / 2;
	int kcount = 0;
	int knum = blurLevel * blurLevel;
	double* kernel = new double[knum];

	for (int y = 0; y < (int)height; y++)
	{
		for (int x = 0; x < (int)width; x++)
		{
			// collect all neighbor elements
			kcount = 0;
			for (int top = y - ksize; top <= y + ksize; top++)
			{
				for (int left = x - ksize; left <= x + ksize; left++)
				{
					int srcx = left < 0 ? Math::Abs(left + 1) : left >= (int)width ? (width - (left - width + 1)) : left;
					int srcy = top < 0 ? Math::Abs(top + 1) : top >= (int)height ? (height - (top - height + 1)) : top;
					kernel[kcount++] = src[srcx + srcy * width];
				}
			}

			// sort elements
			for (int i = 0; i < knum - 1; i++)
			{
				for (int j = 0; j < knum - 1 - i; j++)
				{
					if (kernel[j] > kernel[j + 1])
					{
						double temp = kernel[j];
						kernel[j] = kernel[j + 1];
						kernel[j + 1] = temp;
					}
				}
			}

			// pick the middle one
			dst[x + y * width] = kernel[knum / 2];
		}
	}
}

class Matrix_3_3 {
public:
	int size = 9;
	unsigned char element[9];
	Matrix_3_3(unsigned char element0, unsigned char element1, unsigned char element2,
		unsigned char element3, unsigned char element4, unsigned char element5,
		unsigned char element6, unsigned char element7, unsigned char element8)
	{
		element[0] = element0;
		element[1] = element1;
		element[2] = element2;
		element[3] = element3;
		element[4] = element4;
		element[5] = element5;
		element[6] = element6;
		element[7] = element7;
		element[8] = element8;
	}

	Matrix_3_3(unsigned char* ele)
	{
		for (int size = 0; size < 9; size++)
			element[size] = ele[size];
	}

	Matrix_3_3(unsigned char* src_image, int width, int x, int y)
	{
		element[0] = src_image[(y - 1) * width + (x - 1)];
		element[1] = src_image[(y - 1) * width + x];
		element[2] = src_image[(y - 1) * width + (x + 1)];
		element[3] = src_image[y * width + (x - 1)];
		element[4] = src_image[y * width + x];
		element[5] = src_image[y * width + (x + 1)];
		element[6] = src_image[(y + 1) * width + (x - 1)];
		element[7] = src_image[(y + 1) * width + x];
		element[8] = src_image[(y + 1) * width + (x + 1)];
	}

	unsigned char get(int h, int w)
	{
		return element[(h - 1) * 3 + (w - 1)];
	}

	void get(int h, int w, unsigned char value)
	{
		element[(h - 1) * 3 + (w - 1)] = value;
	}
};

class Matrix_3_3_int {
public:
	int size = 9;
	int element[9];
	Matrix_3_3_int(int element0, int element1, int element2,
		int element3, int element4, int element5,
		int element6, int element7, int element8)
	{
		element[0] = element0;
		element[1] = element1;
		element[2] = element2;
		element[3] = element3;
		element[4] = element4;
		element[5] = element5;
		element[6] = element6;
		element[7] = element7;
		element[8] = element8;
	}

	Matrix_3_3_int(int* ele)
	{
		for (int size = 0; size < 9; size++)
			element[size] = ele[size];
	}

	Matrix_3_3_int(int* src_image, int width, int x, int y)
	{
		element[0] = src_image[(y - 1) * width + (x - 1)];
		element[1] = src_image[(y - 1) * width + x];
		element[2] = src_image[(y - 1) * width + (x + 1)];
		element[3] = src_image[y * width + (x - 1)];
		element[4] = src_image[y * width + x];
		element[5] = src_image[y * width + (x + 1)];
		element[6] = src_image[(y + 1) * width + (x - 1)];
		element[7] = src_image[(y + 1) * width + x];
		element[8] = src_image[(y + 1) * width + (x + 1)];
	}
};

class Matrix_5_5 {
public:
	int size = 25;
	unsigned char element[25];
	Matrix_5_5(unsigned char element0, unsigned char element1, unsigned char element2, unsigned char element3, unsigned char element4,
		unsigned char element5, unsigned char element6, unsigned char element7, unsigned char element8, unsigned char element9,
		unsigned char element10, unsigned char element11, unsigned char element12, unsigned char element13, unsigned char element14,
		unsigned char element15, unsigned char element16, unsigned char element17, unsigned char element18, unsigned char element19,
		unsigned char element20, unsigned char element21, unsigned char element22, unsigned char element23, unsigned char element24)
	{
		element[0] = element0;
		element[1] = element1;
		element[2] = element2;
		element[3] = element3;
		element[4] = element4;
		element[5] = element5;
		element[6] = element6;
		element[7] = element7;
		element[8] = element8;
		element[9] = element9;
		element[10] = element10;
		element[11] = element11;
		element[12] = element12;
		element[13] = element13;
		element[14] = element14;
		element[15] = element15;
		element[16] = element16;
		element[17] = element17;
		element[18] = element18;
		element[19] = element19;
		element[20] = element20;
		element[21] = element21;
		element[22] = element22;
		element[23] = element23;
		element[24] = element24;
	}

	Matrix_5_5(unsigned char* ele)
	{
		for (int size = 0; size < 25; size++)
			element[size] = ele[size];
	}

	Matrix_5_5(unsigned char* src_image, int width, int x, int y)
	{
		element[0] = src_image[(y - 2) * width + (x - 2)];
		element[1] = src_image[(y - 2) * width + (x - 1)];
		element[2] = src_image[(y - 2) * width + x];
		element[3] = src_image[(y - 2) * width + (x + 1)];
		element[4] = src_image[(y - 2) * width + (x + 2)];
		element[5] = src_image[(y - 1) * width + (x - 2)];
		element[6] = src_image[(y - 1) * width + (x - 1)];
		element[7] = src_image[(y - 1) * width + x];
		element[8] = src_image[(y - 1) * width + (x + 1)];
		element[9] = src_image[(y - 1) * width + (x + 2)];
		element[10] = src_image[y * width + (x - 2)];
		element[11] = src_image[y * width + (x - 1)];
		element[12] = src_image[y * width + x];
		element[13] = src_image[y * width + (x + 1)];
		element[14] = src_image[y * width + (x + 2)];
		element[15] = src_image[(y + 1) * width + (x - 2)];
		element[16] = src_image[(y + 1) * width + (x - 1)];
		element[17] = src_image[(y + 1) * width + x];
		element[18] = src_image[(y + 1) * width + (x + 1)];
		element[19] = src_image[(y + 1) * width + (x + 2)];
		element[20] = src_image[(y + 2) * width + (x - 2)];
		element[21] = src_image[(y + 2) * width + (x - 1)];
		element[22] = src_image[(y + 2) * width + x];
		element[23] = src_image[(y + 2) * width + (x + 1)];
		element[24] = src_image[(y + 2) * width + (x + 2)];
	}
};

class Matrix_5_5_int {
public:
	int size = 25;
	int element[25];
	Matrix_5_5_int(int element0, int element1, int element2, int element3, int element4,
		int element5, int element6, int element7, int element8, int element9,
		int element10, int element11, int element12, int element13, int element14,
		int element15, int element16, int element17, int element18, int element19,
		int element20, int element21, int element22, int element23, int element24)
	{
		element[0] = element0;
		element[1] = element1;
		element[2] = element2;
		element[3] = element3;
		element[4] = element4;
		element[5] = element5;
		element[6] = element6;
		element[7] = element7;
		element[8] = element8;
		element[9] = element9;
		element[10] = element10;
		element[11] = element11;
		element[12] = element12;
		element[13] = element13;
		element[14] = element14;
		element[15] = element15;
		element[16] = element16;
		element[17] = element17;
		element[18] = element18;
		element[19] = element19;
		element[20] = element20;
		element[21] = element21;
		element[22] = element22;
		element[23] = element23;
		element[24] = element24;
	}

	Matrix_5_5_int(int* ele)
	{
		for (int size = 0; size < 25; size++)
			element[size] = ele[size];
	}

	Matrix_5_5_int(int* src_image, int width, int x, int y)
	{
		element[0] = src_image[(y - 2) * width + (x - 2)];
		element[1] = src_image[(y - 2) * width + (x - 1)];
		element[2] = src_image[(y - 2) * width + x];
		element[3] = src_image[(y - 2) * width + (x + 1)];
		element[4] = src_image[(y - 2) * width + (x + 2)];
		element[5] = src_image[(y - 1) * width + (x - 2)];
		element[6] = src_image[(y - 1) * width + (x - 1)];
		element[7] = src_image[(y - 1) * width + x];
		element[8] = src_image[(y - 1) * width + (x + 1)];
		element[9] = src_image[(y - 1) * width + (x + 2)];
		element[10] = src_image[y * width + (x - 2)];
		element[11] = src_image[y * width + (x - 1)];
		element[12] = src_image[y * width + x];
		element[13] = src_image[y * width + (x + 1)];
		element[14] = src_image[y * width + (x + 2)];
		element[15] = src_image[(y + 1) * width + (x - 2)];
		element[16] = src_image[(y + 1) * width + (x - 1)];
		element[17] = src_image[(y + 1) * width + x];
		element[18] = src_image[(y + 1) * width + (x + 1)];
		element[19] = src_image[(y + 1) * width + (x + 2)];
		element[20] = src_image[(y + 2) * width + (x - 2)];
		element[21] = src_image[(y + 2) * width + (x - 1)];
		element[22] = src_image[(y + 2) * width + x];
		element[23] = src_image[(y + 2) * width + (x + 1)];
		element[24] = src_image[(y + 2) * width + (x + 2)];
	}
};

class Matrix_7_7 {
public:
	int size = 49;
	unsigned char element[49];
	Matrix_7_7(unsigned char element0, unsigned char element1, unsigned char element2, unsigned char element3, unsigned char element4, unsigned char element5, unsigned char element6,
		unsigned char element7, unsigned char element8, unsigned char element9, unsigned char element10, unsigned char element11, unsigned char element12, unsigned char element13,
		unsigned char element14, unsigned char element15, unsigned char element16, unsigned char element17, unsigned char element18, unsigned char element19, unsigned char element20,
		unsigned char element21, unsigned char element22, unsigned char element23, unsigned char element24, unsigned char element25, unsigned char element26, unsigned char element27,
		unsigned char element28, unsigned char element29, unsigned char element30, unsigned char element31, unsigned char element32, unsigned char element33, unsigned char element34,
		unsigned char element35, unsigned char element36, unsigned char element37, unsigned char element38, unsigned char element39, unsigned char element40, unsigned char element41,
		unsigned char element42, unsigned char element43, unsigned char element44, unsigned char element45, unsigned char element46, unsigned char element47, unsigned char element48)
	{
		element[0] = element0;
		element[1] = element1;
		element[2] = element2;
		element[3] = element3;
		element[4] = element4;
		element[5] = element5;
		element[6] = element6;
		element[7] = element7;
		element[8] = element8;
		element[9] = element9;
		element[10] = element10;
		element[11] = element11;
		element[12] = element12;
		element[13] = element13;
		element[14] = element14;
		element[15] = element15;
		element[16] = element16;
		element[17] = element17;
		element[18] = element18;
		element[19] = element19;
		element[20] = element20;
		element[21] = element21;
		element[22] = element22;
		element[23] = element23;
		element[24] = element24;
		element[25] = element25;
		element[26] = element26;
		element[27] = element27;
		element[28] = element28;
		element[29] = element29;
		element[30] = element30;
		element[31] = element31;
		element[32] = element32;
		element[33] = element33;
		element[34] = element34;
		element[35] = element35;
		element[36] = element36;
		element[37] = element37;
		element[38] = element38;
		element[39] = element39;
		element[40] = element40;
		element[41] = element41;
		element[42] = element42;
		element[43] = element43;
		element[44] = element44;
		element[45] = element45;
		element[46] = element46;
		element[47] = element47;
		element[48] = element48;
	}

	Matrix_7_7(unsigned char* ele)
	{
		for (int size = 0; size < 49; size++)
			element[size] = ele[size];
	}

	Matrix_7_7(unsigned char* src_image, int width, int x, int y)
	{
		element[0] = src_image[(y - 3) * width + (x - 3)];
		element[1] = src_image[(y - 3) * width + (x - 2)];
		element[2] = src_image[(y - 3) * width + (x - 1)];
		element[3] = src_image[(y - 3) * width + x];
		element[4] = src_image[(y - 3) * width + (x + 1)];
		element[5] = src_image[(y - 3) * width + (x + 2)];
		element[6] = src_image[(y - 3) * width + (x + 3)];
		element[7] = src_image[(y - 2) * width + (x - 3)];
		element[8] = src_image[(y - 2) * width + (x - 2)];
		element[9] = src_image[(y - 2) * width + (x - 1)];
		element[10] = src_image[(y - 2) * width + x];
		element[11] = src_image[(y - 2) * width + (x + 1)];
		element[12] = src_image[(y - 2) * width + (x + 2)];
		element[13] = src_image[(y - 2) * width + (x + 3)];
		element[14] = src_image[(y - 1) * width + (x - 3)];
		element[15] = src_image[(y - 1) * width + (x - 2)];
		element[16] = src_image[(y - 1) * width + (x - 1)];
		element[17] = src_image[(y - 1) * width + x];
		element[18] = src_image[(y - 1) * width + (x + 1)];
		element[19] = src_image[(y - 1) * width + (x + 2)];
		element[20] = src_image[(y - 1) * width + (x + 3)];
		element[21] = src_image[y * width + (x - 3)];
		element[22] = src_image[y * width + (x - 2)];
		element[23] = src_image[y * width + (x - 1)];
		element[24] = src_image[y * width + x];
		element[25] = src_image[y * width + (x + 1)];
		element[26] = src_image[y * width + (x + 2)];
		element[27] = src_image[y * width + (x + 3)];
		element[28] = src_image[(y + 1) * width + (x - 3)];
		element[29] = src_image[(y + 1) * width + (x - 2)];
		element[30] = src_image[(y + 1) * width + (x - 1)];
		element[31] = src_image[(y + 1) * width + x];
		element[32] = src_image[(y + 1) * width + (x + 1)];
		element[33] = src_image[(y + 1) * width + (x + 2)];
		element[34] = src_image[(y + 1) * width + (x + 3)];
		element[35] = src_image[(y + 2) * width + (x - 3)];
		element[36] = src_image[(y + 2) * width + (x - 2)];
		element[37] = src_image[(y + 2) * width + (x - 1)];
		element[38] = src_image[(y + 2) * width + x];
		element[39] = src_image[(y + 2) * width + (x + 1)];
		element[40] = src_image[(y + 2) * width + (x + 2)];
		element[41] = src_image[(y + 2) * width + (x + 3)];
		element[42] = src_image[(y + 3) * width + (x - 3)];
		element[43] = src_image[(y + 3) * width + (x - 2)];
		element[44] = src_image[(y + 3) * width + (x - 1)];
		element[45] = src_image[(y + 3) * width + x];
		element[46] = src_image[(y + 3) * width + (x + 1)];
		element[47] = src_image[(y + 3) * width + (x + 2)];
		element[48] = src_image[(y + 3) * width + (x + 3)];
	}
};

class Matrix_7_7_int {
public:
	int size = 49;
	int element[49];
	Matrix_7_7_int(int element0, int element1, int element2, int element3, int element4, int element5, int element6,
		int element7, int element8, int element9, int element10, int element11, int element12, int element13,
		int element14, int element15, int element16, int element17, int element18, int element19, int element20,
		int element21, int element22, int element23, int element24, int element25, int element26, int element27,
		int element28, int element29, int element30, int element31, int element32, int element33, int element34,
		int element35, int element36, int element37, int element38, int element39, int element40, int element41,
		int element42, int element43, int element44, int element45, int element46, int element47, int element48)
	{
		element[0] = element0;
		element[1] = element1;
		element[2] = element2;
		element[3] = element3;
		element[4] = element4;
		element[5] = element5;
		element[6] = element6;
		element[7] = element7;
		element[8] = element8;
		element[9] = element9;
		element[10] = element10;
		element[11] = element11;
		element[12] = element12;
		element[13] = element13;
		element[14] = element14;
		element[15] = element15;
		element[16] = element16;
		element[17] = element17;
		element[18] = element18;
		element[19] = element19;
		element[20] = element20;
		element[21] = element21;
		element[22] = element22;
		element[23] = element23;
		element[24] = element24;
		element[25] = element25;
		element[26] = element26;
		element[27] = element27;
		element[28] = element28;
		element[29] = element29;
		element[30] = element30;
		element[31] = element31;
		element[32] = element32;
		element[33] = element33;
		element[34] = element34;
		element[35] = element35;
		element[36] = element36;
		element[37] = element37;
		element[38] = element38;
		element[39] = element39;
		element[40] = element40;
		element[41] = element41;
		element[42] = element42;
		element[43] = element43;
		element[44] = element44;
		element[45] = element45;
		element[46] = element46;
		element[47] = element47;
		element[48] = element48;
	}

	Matrix_7_7_int(int* ele)
	{
		for (int size = 0; size < 49; size++)
			element[size] = ele[size];
	}

	Matrix_7_7_int(int* src_image, int width, int x, int y)
	{
		element[0] = src_image[(y - 3) * width + (x - 3)];
		element[1] = src_image[(y - 3) * width + (x - 2)];
		element[2] = src_image[(y - 3) * width + (x - 1)];
		element[3] = src_image[(y - 3) * width + x];
		element[4] = src_image[(y - 3) * width + (x + 1)];
		element[5] = src_image[(y - 3) * width + (x + 2)];
		element[6] = src_image[(y - 3) * width + (x + 3)];
		element[7] = src_image[(y - 2) * width + (x - 3)];
		element[8] = src_image[(y - 2) * width + (x - 2)];
		element[9] = src_image[(y - 2) * width + (x - 1)];
		element[10] = src_image[(y - 2) * width + x];
		element[11] = src_image[(y - 2) * width + (x + 1)];
		element[12] = src_image[(y - 2) * width + (x + 2)];
		element[13] = src_image[(y - 2) * width + (x + 3)];
		element[14] = src_image[(y - 1) * width + (x - 3)];
		element[15] = src_image[(y - 1) * width + (x - 2)];
		element[16] = src_image[(y - 1) * width + (x - 1)];
		element[17] = src_image[(y - 1) * width + x];
		element[18] = src_image[(y - 1) * width + (x + 1)];
		element[19] = src_image[(y - 1) * width + (x + 2)];
		element[20] = src_image[(y - 1) * width + (x + 3)];
		element[21] = src_image[y * width + (x - 3)];
		element[22] = src_image[y * width + (x - 2)];
		element[23] = src_image[y * width + (x - 1)];
		element[24] = src_image[y * width + x];
		element[25] = src_image[y * width + (x + 1)];
		element[26] = src_image[y * width + (x + 2)];
		element[27] = src_image[y * width + (x + 3)];
		element[28] = src_image[(y + 1) * width + (x - 3)];
		element[29] = src_image[(y + 1) * width + (x - 2)];
		element[30] = src_image[(y + 1) * width + (x - 1)];
		element[31] = src_image[(y + 1) * width + x];
		element[32] = src_image[(y + 1) * width + (x + 1)];
		element[33] = src_image[(y + 1) * width + (x + 2)];
		element[34] = src_image[(y + 1) * width + (x + 3)];
		element[35] = src_image[(y + 2) * width + (x - 3)];
		element[36] = src_image[(y + 2) * width + (x - 2)];
		element[37] = src_image[(y + 2) * width + (x - 1)];
		element[38] = src_image[(y + 2) * width + x];
		element[39] = src_image[(y + 2) * width + (x + 1)];
		element[40] = src_image[(y + 2) * width + (x + 2)];
		element[41] = src_image[(y + 2) * width + (x + 3)];
		element[42] = src_image[(y + 3) * width + (x - 3)];
		element[43] = src_image[(y + 3) * width + (x - 2)];
		element[44] = src_image[(y + 3) * width + (x - 1)];
		element[45] = src_image[(y + 3) * width + x];
		element[46] = src_image[(y + 3) * width + (x + 1)];
		element[47] = src_image[(y + 3) * width + (x + 2)];
		element[48] = src_image[(y + 3) * width + (x + 3)];
	}
};

double _OfpsDpc_3_3(Matrix_3_3 matrix, double std_DN, double factor)
{
	double image_mean = 0, image_dpc;

	for (int i = 0; i < matrix.size; i++)
		image_mean += matrix.element[i];
	image_mean /= matrix.size;

	if (matrix.element[4] >= (image_mean + std_DN * factor))
		image_dpc = (image_mean + std_DN * factor);
	else if (matrix.element[4] <= (image_mean - std_DN * factor))
		image_dpc = (image_mean - std_DN * factor);
	else
		image_dpc = matrix.element[4];

	return image_dpc;
}

void ISP::OfpsDpc_3_3(unsigned char* src_image, int width, int height, unsigned char* ofpsdpc_image, double FWC, int Gain, double factor)
{
	double SN_FW = sqrt(FWC);
	double DN_e = FWC / 256 / Gain;
	double std_DN = SN_FW / DN_e;
	double DC_offset_e = FWC * (Gain - 1) / Gain;

	for (int y = 1; y < (height - 1); y++)
	{
		for (int x = 1; x < (width - 1); x++)
		{
			Matrix_3_3 filter_block = Matrix_3_3(src_image, width, x, y);
			double filter_block_dpc = _OfpsDpc_3_3(filter_block, std_DN, factor);
			if (filter_block_dpc < 0)
				filter_block_dpc = 0;
			else if (filter_block_dpc > 255)
				filter_block_dpc = 255;
			ofpsdpc_image[(y) * (width)+(x)] = (unsigned char)filter_block_dpc;
		}
	}

	/*FWC = 30000;
	SN_FW = sqrt(FWC);
	Gain = 8;
	DN_e = FWC / 256 / Gain;
	std_DN = SN_FW / DN_e;
	DC_offset_e = FWC * (Gain - 1) / Gain;
	filter_block = ones(3, 3);
	for frame_i = 2:91,
		for frame_j = 2 : 183,
			filter_block = A_1(frame_i - 1:frame_i + 1, frame_j - 1 : frame_j + 1, 1);
			filter_block_mean = mean(mean(filter_block(:, : , 1)));
			filter_block_std = std(std(filter_block(:, : , 1)));
			%DN_Edge_Pattern = ND_DN_ee_v2(filter_block, En_Edge_Factor, BL, PRNU, TG, RN, CG);
			filter_block_dpc = ofps_dpc(filter_block, std_DN);
			A_1_dpc(frame_i - 1, frame_j - 1, 1) = filter_block_dpc;
			A_1_dpc(frame_i - 1, frame_j - 1, 2) = filter_block_dpc;
			A_1_dpc(frame_i - 1, frame_j - 1, 3) = filter_block_dpc;
		end
	end*/
}

double _OfpsDpc_3_3(Matrix_3_3_int matrix, double std_DN, double factor)
{
	double image_mean = 0, image_dpc;

	for (int i = 0; i < matrix.size; i++)
		image_mean += matrix.element[i];
	image_mean /= matrix.size;

	if (matrix.element[4] >= (image_mean + std_DN * factor))
		image_dpc = (image_mean + std_DN * factor);
	else if (matrix.element[4] <= (image_mean - std_DN * factor))
		image_dpc = (image_mean - std_DN * factor);
	else
		image_dpc = matrix.element[4];

	return image_dpc;
}

void ISP::OfpsDpc_3_3(int* src_image, int width, int height, int* ofpsdpc_image, double FWC, int Gain, double factor)
{
	double SN_FW = sqrt(FWC);
	double DN_e = FWC / 256 / Gain;
	double std_DN = SN_FW / DN_e;
	double DC_offset_e = FWC * (Gain - 1) / Gain;

	for (int y = 1; y < (height - 1); y++)
	{
		for (int x = 1; x < (width - 1); x++)
		{
			Matrix_3_3_int filter_block = Matrix_3_3_int(src_image, width, x, y);
			double filter_block_dpc = _OfpsDpc_3_3(filter_block, std_DN, factor);
			ofpsdpc_image[(y) * (width)+(x)] = (int)filter_block_dpc;
		}
	}

	/*FWC = 30000;
	SN_FW = sqrt(FWC);
	Gain = 8;
	DN_e = FWC / 256 / Gain;
	std_DN = SN_FW / DN_e;
	DC_offset_e = FWC * (Gain - 1) / Gain;
	filter_block = ones(3, 3);
	for frame_i = 2:91,
	for frame_j = 2 : 183,
	filter_block = A_1(frame_i - 1:frame_i + 1, frame_j - 1 : frame_j + 1, 1);
	filter_block_mean = mean(mean(filter_block(:, : , 1)));
	filter_block_std = std(std(filter_block(:, : , 1)));
	%DN_Edge_Pattern = ND_DN_ee_v2(filter_block, En_Edge_Factor, BL, PRNU, TG, RN, CG);
	filter_block_dpc = ofps_dpc(filter_block, std_DN);
	A_1_dpc(frame_i - 1, frame_j - 1, 1) = filter_block_dpc;
	A_1_dpc(frame_i - 1, frame_j - 1, 2) = filter_block_dpc;
	A_1_dpc(frame_i - 1, frame_j - 1, 3) = filter_block_dpc;
	end
	end*/
}

double _OfpsDpc_5_5(Matrix_5_5 matrix, double std_DN, double factor)
{
	double image_mean = 0, image_dpc;

	for (int i = 0; i < matrix.size; i++)
		image_mean += matrix.element[i];
	image_mean /= matrix.size;

	if (matrix.element[12] >= (image_mean + std_DN * factor))
		image_dpc = (image_mean + std_DN * factor);
	else if (matrix.element[12] <= (image_mean - std_DN * factor))
		image_dpc = (image_mean - std_DN * factor);
	else
		image_dpc = matrix.element[12];

	return image_dpc;
}

void ISP::OfpsDpc_5_5(unsigned char* src_image, int width, int height, unsigned char* ofpsdpc_image, double FWC, int Gain, double factor)
{
	double SN_FW = sqrt(FWC);
	double DN_e = FWC / 256 / Gain;
	double std_DN = SN_FW / DN_e;
	double DC_offset_e = FWC * (Gain - 1) / Gain;

	for (int y = 2; y < (height - 2); y++)
	{
		for (int x = 2; x < (width - 2); x++)
		{
			Matrix_5_5 filter_block = Matrix_5_5(src_image, width, x, y);
			double filter_block_dpc = _OfpsDpc_5_5(filter_block, std_DN, factor);
			if (filter_block_dpc < 0)
				filter_block_dpc = 0;
			else if (filter_block_dpc > 255)
				filter_block_dpc = 255;
			//ofpsdpc_image[(y - 2)*(width - 4) + (x - 2)] = filter_block_dpc;
			ofpsdpc_image[(y) * (width)+(x)] = (unsigned char)filter_block_dpc;
		}
	}

	/*FWC=30000;
	SN_FW=sqrt(FWC);
	Gain=8;
	DN_e=FWC/256/Gain;
	std_DN=SN_FW/DN_e;
	Offset=0;
	EEF=0.5;
	STD_Factor=5;
	bkg_offset=658;
	for frame_dpc_i=3:m-2,
		for frame_dpc_j=3:n-2,
			filter_block=A_1_tmp(frame_dpc_i-2:frame_dpc_i+2,frame_dpc_j-2:frame_dpc_j+2);
			filter_block_mean=mean(mean(filter_block(:,:,1)));
			filter_block_std=std(std(filter_block(:,:,1)));
			filter_block_dpc=ofps_dpc(filter_block,std_DN);
			A_1_dpc(frame_dpc_i-2,frame_dpc_j-2,1)=filter_block_dpc;
			A_1_dpc(frame_dpc_i-2,frame_dpc_j-2,2)=filter_block_dpc;
			A_1_dpc(frame_dpc_i-2,frame_dpc_j-2,3)=filter_block_dpc;
		end
	end*/
}

double _OfpsDpc_5_5(Matrix_5_5_int matrix, double std_DN, double factor)
{
	double image_mean = 0, image_dpc;

	for (int i = 0; i < matrix.size; i++)
		image_mean += matrix.element[i];
	image_mean /= matrix.size;

	if (matrix.element[12] >= (image_mean + std_DN * factor))
		image_dpc = (image_mean + std_DN * factor);
	else if (matrix.element[12] <= (image_mean - std_DN * factor))
		image_dpc = (image_mean - std_DN * factor);
	else
		image_dpc = matrix.element[12];

	return image_dpc;
}

void ISP::OfpsDpc_5_5(int* src_image, int width, int height, int* ofpsdpc_image, double FWC, int Gain, double factor)
{
	double SN_FW = sqrt(FWC);
	double DN_e = FWC / 256 / Gain;
	double std_DN = SN_FW / DN_e;
	double DC_offset_e = FWC * (Gain - 1) / Gain;

	for (int y = 2; y < (height - 2); y++)
	{
		for (int x = 2; x < (width - 2); x++)
		{
			Matrix_5_5_int filter_block = Matrix_5_5_int(src_image, width, x, y);
			double filter_block_dpc = _OfpsDpc_5_5(filter_block, std_DN, factor);
			//ofpsdpc_image[(y - 2)*(width - 4) + (x - 2)] = filter_block_dpc;
			ofpsdpc_image[(y) * (width)+(x)] = (int)filter_block_dpc;
		}
	}

	/*FWC=30000;
	SN_FW=sqrt(FWC);
	Gain=8;
	DN_e=FWC/256/Gain;
	std_DN=SN_FW/DN_e;
	Offset=0;
	EEF=0.5;
	STD_Factor=5;
	bkg_offset=658;
	for frame_dpc_i=3:m-2,
	for frame_dpc_j=3:n-2,
	filter_block=A_1_tmp(frame_dpc_i-2:frame_dpc_i+2,frame_dpc_j-2:frame_dpc_j+2);
	filter_block_mean=mean(mean(filter_block(:,:,1)));
	filter_block_std=std(std(filter_block(:,:,1)));
	filter_block_dpc=ofps_dpc(filter_block,std_DN);
	A_1_dpc(frame_dpc_i-2,frame_dpc_j-2,1)=filter_block_dpc;
	A_1_dpc(frame_dpc_i-2,frame_dpc_j-2,2)=filter_block_dpc;
	A_1_dpc(frame_dpc_i-2,frame_dpc_j-2,3)=filter_block_dpc;
	end
	end*/
}

double _OfpsDpc_7_7(Matrix_7_7 matrix, double std_DN, double factor)
{
	double image_mean = 0, image_dpc;

	for (int i = 0; i < matrix.size; i++)
		image_mean += matrix.element[i];
	image_mean /= matrix.size;

	if (matrix.element[24] >= (image_mean + std_DN * factor))
		image_dpc = (image_mean + std_DN * factor);
	else if (matrix.element[24] <= (image_mean - std_DN * factor))
		image_dpc = (image_mean - std_DN * factor);
	else
		image_dpc = matrix.element[24];

	return image_dpc;
}

void ISP::OfpsDpc_7_7(unsigned char* src_image, int width, int height, unsigned char* ofpsdpc_image, double FWC, int Gain, double factor)
{
	double SN_FW = sqrt(FWC);
	double DN_e = FWC / 256 / Gain;
	double std_DN = SN_FW / DN_e;
	double DC_offset_e = FWC * (Gain - 1) / Gain;

	for (int y = 3; y < (height - 3); y++)
	{
		for (int x = 3; x < (width - 3); x++)
		{
			Matrix_7_7 filter_block = Matrix_7_7(src_image, width, x, y);
			double filter_block_dpc = _OfpsDpc_7_7(filter_block, std_DN, factor);
			if (filter_block_dpc < 0)
				filter_block_dpc = 0;
			else if (filter_block_dpc > 255)
				filter_block_dpc = 255;
			//ofpsdpc_image[(y - 3)*(width - 4) + (x - 3)] = filter_block_dpc;
			ofpsdpc_image[(y) * (width)+(x)] = (unsigned char)filter_block_dpc;
		}
	}
}

double _OfpsDpc_7_7(Matrix_7_7_int matrix, double std_DN, double factor)
{
	double image_mean = 0, image_dpc;

	for (int i = 0; i < matrix.size; i++)
		image_mean += matrix.element[i];
	image_mean /= matrix.size;

	if (matrix.element[24] >= (image_mean + std_DN * factor))
		image_dpc = (image_mean + std_DN * factor);
	else if (matrix.element[24] <= (image_mean - std_DN * factor))
		image_dpc = (image_mean - std_DN * factor);
	else
		image_dpc = matrix.element[24];

	return image_dpc;
}

void ISP::OfpsDpc_7_7(int* src_image, int width, int height, int* ofpsdpc_image, double FWC, int Gain, double factor)
{
	double SN_FW = sqrt(FWC);
	double DN_e = FWC / 256 / Gain;
	double std_DN = SN_FW / DN_e;
	double DC_offset_e = FWC * (Gain - 1) / Gain;

	for (int y = 3; y < (height - 3); y++)
	{
		for (int x = 3; x < (width - 3); x++)
		{
			Matrix_7_7_int filter_block = Matrix_7_7_int(src_image, width, x, y);
			double filter_block_dpc = _OfpsDpc_7_7(filter_block, std_DN, factor);
			//ofpsdpc_image[(y - 3)*(width - 4) + (x - 3)] = filter_block_dpc;
			ofpsdpc_image[(y) * (width)+(x)] = (int)filter_block_dpc;
		}
	}
}

void matrix_status(void* src, int size, double* std, double* mean, double* max, double* min)
{
	double* doublePtr = (double*)src;
	double sum = 0, std_tmp = 0, mean_tmp = 0;
	double min_tmp = doublePtr[0];
	double max_tmp = doublePtr[0];

	for (int sz = 0; sz < size; sz++)
	{
		sum += doublePtr[sz];
		if (doublePtr[sz] > max_tmp)
			max_tmp = doublePtr[sz];
		if (doublePtr[sz] < min_tmp)
			min_tmp = doublePtr[sz];
	}

	mean_tmp = sum / size;


	if (std != NULL)
	{
		for (int sz = 0; sz < size; sz++) {
			std_tmp += (doublePtr[sz] - mean_tmp) * (doublePtr[sz] - mean_tmp);
		}
		std_tmp /= size;
		std_tmp = sqrt(std_tmp);
		*std = std_tmp;
	}

	if (mean != NULL)
		*mean = mean_tmp;

	if (max != NULL)
		*max = max_tmp;

	if (min != NULL)
		*min = min_tmp;
}

void matrix_status_stretch(void* src, int size, double* std, double* mean, double* max, double* min, int factor)
{
	double* doublePtr = (double*)src;
	double sum = 0, std_tmp = 0, mean_tmp = 0;
	double min_tmp = doublePtr[0];
	double max_tmp = doublePtr[0];
	double stretch_factor = 0;

	for (int sz = 0; sz < size; sz++)
	{
		sum += doublePtr[sz];
		if (doublePtr[sz] > max_tmp)
			max_tmp = doublePtr[sz];
		if (doublePtr[sz] < min_tmp)
			min_tmp = doublePtr[sz];
	}

	mean_tmp = sum / size;


	if (std != NULL)
	{
		for (int sz = 0; sz < size; sz++) {
			std_tmp += (doublePtr[sz] - mean_tmp) * (doublePtr[sz] - mean_tmp);
		}
		std_tmp /= size;
		std_tmp = sqrt(std_tmp);
		*std = std_tmp;
	}

	/*
		stretch_factor = 255 / (6 * std_tmp);
		min_tmp = (mean_tmp - (3 * std_tmp)) - (1 / (stretch_factor * ((mean_tmp - (3 * std_tmp)) - min_tmp)));
		max_tmp = (mean_tmp + (3 * std_tmp)) + (1 / (stretch_factor * (max_tmp - (mean_tmp + (3 * std_tmp)))));
	*/
	if (factor == 2)
		stretch_factor = 0.5;
	else
		stretch_factor = (factor - 2);
	min_tmp = (mean_tmp - (stretch_factor * std_tmp));
	max_tmp = (mean_tmp + (stretch_factor * std_tmp));

	if (min_tmp < 0)
		min_tmp = 0;
	//if (max_tmp > 255)
	//	max_tmp = 255;

	if (mean != NULL)
		*mean = mean_tmp;

	if (max != NULL)
		*max = max_tmp;

	if (min != NULL)
		*min = min_tmp;
}

void _OfpsMoire(unsigned char* src_image, int image_width, int image_height, unsigned char* dst_image,
	double offset, int P_Filter_Size, int M_Filter_Size, int F_Filter_Size, int mode)
{
	int image_size = image_width * image_height;
	int Gain_Factor = 1;
	int* bkg_img = new int[image_size];
	int* img_in_double = new int[image_size];
	double* img_bg_double = new double[image_size];
	double* compensation_table = new double[image_size];
	int* Image_Out_tmp = new int[image_size];
	int* Image_Out_DN = new int[image_size];
	int* Image_Out_Stretch = new int[image_size];
	int* Image_Out = new int[image_size];
	double img_bg_double_mean, img_bg_double_max, img_bg_double_min;

	for (int size = 0; size < image_size; size++)
	{
		img_in_double[size] = src_image[size];
	}

	for (int l = 0; l < 5; l++)
	{
		ISP::HomogeneousBlur(img_in_double, bkg_img, image_width, image_height, P_Filter_Size);
	}

	for (int size = 0; size < image_size; size++)
	{
		img_in_double[size] = (double)src_image[size] + offset;
		img_bg_double[size] = bkg_img[size] + offset;
	}

	matrix_status(img_bg_double, image_size, NULL, &img_bg_double_mean, &img_bg_double_max, &img_bg_double_min);

	double compensation_mean = img_bg_double_max - (img_bg_double_max - img_bg_double_mean) * 0.25;

	for (int size = 0; size < image_size; size++)
	{
		compensation_table[size] = img_bg_double[size] / compensation_mean;
		compensation_table[size] = img_in_double[size] / compensation_table[size]; // img_in_double./compensation_table
	}

	double compensation_table_min;
	matrix_status(compensation_table, image_size, NULL, NULL, NULL, &compensation_table_min);

	for (int size = 0; size < image_size; size++)
	{
		Image_Out_tmp[size] = Gain_Factor * (compensation_table[size] - compensation_table_min);
	}

	ISP::GaussianBlur(Image_Out_tmp, Image_Out_DN, image_width, image_height, M_Filter_Size, 2);

	double std_x, mean_x, max_x, min_x;
	if (mode == 1)
		matrix_status(Image_Out_DN, image_size, &std_x, &mean_x, &max_x, &min_x);     // 2nd version demoire
	else
		matrix_status_stretch(Image_Out_DN, image_size, &std_x, &mean_x, &max_x, &min_x, mode);     // 3rd version demoire - stretch

	for (int size = 0; size < image_size; size++)
	{
		if ((Image_Out_DN[size] - min_x) < 0)
			Image_Out_DN[size] = min_x;

		Image_Out_Stretch[size] = (((Image_Out_DN[size] - min_x) / (max_x - min_x)) * 255);

		if (Image_Out_Stretch[size] < 0)
			Image_Out_Stretch[size] = 0;
		else if (Image_Out_Stretch[size] > 255)
			Image_Out_Stretch[size] = 255;
	}

	ISP::GaussianBlur(Image_Out_Stretch, Image_Out, image_width, image_height, F_Filter_Size, 2);

	for (int size = 0; size < image_size; size++)
	{
		dst_image[size] = (unsigned char)Image_Out[size];
	}
}

void ISP::OfpsMoire(unsigned char* src_image, int width, int height, unsigned char* ofpsmoire_image,
	double bkg_offset, unsigned int p_filter_size, unsigned int m_Fflter_size, unsigned int f_filter_size, int mode)
{
	_OfpsMoire(src_image, width, height, ofpsmoire_image, bkg_offset, p_filter_size, m_Fflter_size, f_filter_size, mode);
}

void _OfpsMoire(int* src_image, int image_width, int image_height, unsigned char* dst_image,
	double offset, int P_Filter_Size, int M_Filter_Size, int F_Filter_Size, int mode)
{
	int image_size = image_width * image_height;
	int Gain_Factor = 1;
	int* bkg_img = new int[image_size];
	int* img_in_double = new int[image_size];
	double* img_bg_double = new double[image_size];
	double* compensation_table = new double[image_size];
	int* Image_Out_tmp = new int[image_size];
	int* Image_Out_DN = new int[image_size];
	int* Image_Out_Stretch = new int[image_size];
	int* Image_Out = new int[image_size];
	double img_bg_double_mean, img_bg_double_max, img_bg_double_min;

	for (int size = 0; size < image_size; size++)
	{
		img_in_double[size] = src_image[size];
	}

	for (int l = 0; l < 5; l++)
	{
		ISP::HomogeneousBlur(img_in_double, bkg_img, image_width, image_height, P_Filter_Size);
	}

	for (int size = 0; size < image_size; size++)
	{
		img_in_double[size] = (double)src_image[size] + offset;
		img_bg_double[size] = bkg_img[size] + offset;
	}

	matrix_status(img_bg_double, image_size, NULL, &img_bg_double_mean, &img_bg_double_max, &img_bg_double_min);

	double compensation_mean = img_bg_double_max - (img_bg_double_max - img_bg_double_mean) * 0.25;

	for (int size = 0; size < image_size; size++)
	{
		compensation_table[size] = img_bg_double[size] / compensation_mean;
		compensation_table[size] = img_in_double[size] / compensation_table[size]; // img_in_double./compensation_table
	}

	double compensation_table_min;
	matrix_status(compensation_table, image_size, NULL, NULL, NULL, &compensation_table_min);

	for (int size = 0; size < image_size; size++)
	{
		Image_Out_tmp[size] = Gain_Factor * (compensation_table[size] - compensation_table_min);
	}

	ISP::GaussianBlur(Image_Out_tmp, Image_Out_DN, image_width, image_height, M_Filter_Size, 2);

	double std_x, mean_x, max_x, min_x;
	if (mode == 1)
		matrix_status(Image_Out_DN, image_size, &std_x, &mean_x, &max_x, &min_x);     // 2nd version demoire
	else
		matrix_status_stretch(Image_Out_DN, image_size, &std_x, &mean_x, &max_x, &min_x, mode);     // 3rd version demoire - stretch

	for (int size = 0; size < image_size; size++)
	{
		if ((Image_Out_DN[size] - min_x) < 0)
			Image_Out_DN[size] = min_x;

		Image_Out_Stretch[size] = (((Image_Out_DN[size] - min_x) / (max_x - min_x)) * 255);

		if (Image_Out_Stretch[size] < 0)
			Image_Out_Stretch[size] = 0;
		else if (Image_Out_Stretch[size] > 255)
			Image_Out_Stretch[size] = 255;
	}

	ISP::GaussianBlur(Image_Out_Stretch, Image_Out, image_width, image_height, F_Filter_Size, 2);

	for (int size = 0; size < image_size; size++)
	{
		dst_image[size] = (unsigned char)Image_Out[size];
	}
}

void ISP::OfpsMoire(int* src_image, int width, int height, unsigned char* ofpsmoire_image,
	double bkg_offset, unsigned int p_filter_size, unsigned int m_Fflter_size, unsigned int f_filter_size, int mode)
{
	_OfpsMoire(src_image, width, height, ofpsmoire_image, bkg_offset, p_filter_size, m_Fflter_size, f_filter_size, mode);
}

double _OfpsIgn(unsigned char adc_dn, int offset, int fwc, int gain, double dist_factor)
{
	double dn_e = fwc / ((gain - 1) * 256 + 256 * dist_factor - offset);
	double dc_offset_e = fwc * (gain - 1) / gain;
	return sqrt(adc_dn * dn_e + dc_offset_e) / dn_e;
}

Matrix_M_N OfpsDnMxnEe(Matrix_M_N sci, int offset, int fwc, int gain, double dist_factor)
{
	double Noise_Ratio, DN_Weight, SCF;
	double SC_Input_mean, SC_Input_std, SC_GNoise;
	Matrix_M_N sco(sci.height(), sci.width());
	SC_Input_mean = sci.mean();
	SC_Input_std = sci.std();

	SC_GNoise = _OfpsIgn(SC_Input_mean, offset, fwc, gain, dist_factor);
	if (SC_Input_std < 0.5)
		Noise_Ratio = 1;
	else
		Noise_Ratio = (SC_Input_std / SC_GNoise);

	DN_Weight = (gain + 1) - Noise_Ratio;

	if (DN_Weight > 0)
		SCF = 1 / (1 + DN_Weight);
	else
		SCF = 1;

	for (int x = 1; x <= sci.width(); x++)
	{
		for (int y = 1; y <= sci.height(); y++)
		{
			sco.set(y, x, (sci.get(y, x) - SC_Input_mean) * SCF + SC_Input_mean);
		}
	}
	return sco;
}

Matrix_M_N OfpsDnMxnScDEe(Matrix_M_N eep, int ejpd, int ejt, Matrix_M_N ef, int offset, int fwc, int gain, double dist_factor)
{
	Matrix_M_N RP(3, 3, "ONE");
	Matrix_M_N RPD(3, 3, "ONE");
	Matrix_M_N eeo(eep.height(), eep.width());

	if (ejpd == 2)
	{
		RP.set(1, 1, eep.get(1, 2));
		RP.set(1, 2, eep.get(1, 3));
		RP.set(1, 3, eep.get(2, 3));
		RP.set(2, 1, eep.get(1, 1));
		RP.set(2, 2, eep.get(2, 2));
		RP.set(2, 3, eep.get(3, 3));
		RP.set(3, 1, eep.get(2, 1));
		RP.set(3, 2, eep.get(3, 1));
		RP.set(3, 3, eep.get(3, 2));
		if (ejt == 112)
		{
			RPD.sub_matrix_set_y(1, 2, OfpsDnMxnEe(RP.sub_matrix_y(1, 2), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			RPD.sub_matrix_set_y(3, 3, OfpsDnMxnEe(RP.sub_matrix_y(3, 3), offset, fwc, gain, dist_factor) * ef.get(1, 3));
		}
		else if (ejt == 122)
		{
			RPD.sub_matrix_set_y(1, 1, OfpsDnMxnEe(RP.sub_matrix_y(1, 1), offset, fwc, gain, dist_factor) * ef.get(1, 1));
			RPD.sub_matrix_set_y(2, 3, OfpsDnMxnEe(RP.sub_matrix_y(2, 3), offset, fwc, gain, dist_factor) * ef.get(1, 2));
		}
		else if (ejt == 123)
		{
			RPD.sub_matrix_set_y(1, 1, OfpsDnMxnEe(RP.sub_matrix_y(1, 1), offset, fwc, gain, dist_factor) * ef.get(1, 1));
			RPD.sub_matrix_set_y(2, 2, OfpsDnMxnEe(RP.sub_matrix_y(2, 2), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			RPD.sub_matrix_set_y(3, 3, OfpsDnMxnEe(RP.sub_matrix_y(3, 3), offset, fwc, gain, dist_factor) * ef.get(1, 3));
		}
		else
			RPD = OfpsDnMxnEe(RP, offset, fwc, gain, dist_factor);
	}
	else if (ejpd == 3)
	{
		RP.set(3, 1, eep.get(1, 1));
		RP.set(3, 2, eep.get(1, 2));
		RP.set(3, 3, eep.get(2, 1));
		RP.set(2, 1, eep.get(1, 3));
		RP.set(2, 2, eep.get(2, 2));
		RP.set(2, 3, eep.get(3, 1));
		RP.set(1, 1, eep.get(2, 3));
		RP.set(1, 2, eep.get(3, 2));
		RP.set(1, 3, eep.get(3, 3));
		if (ejt == 112)
		{
			RPD.sub_matrix_set_y(1, 2, OfpsDnMxnEe(RP.sub_matrix_y(1, 2), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			RPD.sub_matrix_set_y(3, 3, OfpsDnMxnEe(RP.sub_matrix_y(3, 3), offset, fwc, gain, dist_factor) * ef.get(1, 3));
		}
		else if (ejt == 122)
		{
			RPD.sub_matrix_set_y(1, 1, OfpsDnMxnEe(RP.sub_matrix_y(1, 1), offset, fwc, gain, dist_factor) * ef.get(1, 1));
			RPD.sub_matrix_set_y(2, 3, OfpsDnMxnEe(RP.sub_matrix_y(2, 3), offset, fwc, gain, dist_factor) * ef.get(1, 2));
		}
		else if (ejt == 123)
		{
			RPD.sub_matrix_set_y(1, 2, OfpsDnMxnEe(RP.sub_matrix_y(1, 2), offset, fwc, gain, dist_factor) * ef.get(1, 1));
			RPD.sub_matrix_set_y(2, 2, OfpsDnMxnEe(RP.sub_matrix_y(2, 2), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			RPD.sub_matrix_set_y(3, 3, OfpsDnMxnEe(RP.sub_matrix_y(3, 3), offset, fwc, gain, dist_factor) * ef.get(1, 2));
		}
		else
			RPD = OfpsDnMxnEe(RP, offset, fwc, gain, dist_factor);
	}

	if (ejpd == 2)
	{
		eeo.set(1, 2, RPD.get(1, 1));
		eeo.set(1, 3, RPD.get(1, 2));
		eeo.set(2, 3, RPD.get(1, 3));
		eeo.set(1, 1, RPD.get(2, 1));
		eeo.set(2, 2, RPD.get(2, 2));
		eeo.set(3, 3, RPD.get(2, 3));
		eeo.set(2, 1, RPD.get(3, 1));
		eeo.set(3, 1, RPD.get(3, 2));
		eeo.set(3, 2, RPD.get(3, 3));
	}
	else if (ejpd == 3)
	{
		eeo.set(1, 1, RPD.get(3, 1));
		eeo.set(1, 2, RPD.get(3, 2));
		eeo.set(2, 1, RPD.get(3, 3));
		eeo.set(1, 3, RPD.get(2, 1));
		eeo.set(2, 2, RPD.get(2, 2));
		eeo.set(3, 1, RPD.get(2, 3));
		eeo.set(2, 3, RPD.get(1, 1));
		eeo.set(3, 2, RPD.get(1, 2));
		eeo.set(3, 3, RPD.get(1, 3));
	}

	return eeo;
}

Matrix_M_N OfpsDnMxnScLEe(Matrix_M_N eep, int ejt, int lejp, int lejps, Matrix_M_N ef, int offset, int fwc, int gain, int dist_factor)
{
	Matrix_M_N RP(1, 9, "ONE");
	Matrix_M_N RPD(1, 9, "ONE");
	Matrix_M_N eeo(eep.height(), eep.width());
	eeo = eep;

	if (lejp == 51)
	{
		if (lejps == 1)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 2));
				RPD = OfpsDnMxnEe(RP, offset, fwc, gain, dist_factor) * ef.get(1, 2);
				RPD.set(1, 9, ef.get(1, 3) * eep.get(3, 1));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 2));
				RP.set(1, 9, eep.get(3, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 2));
				RP.set(1, 9, eep.get(3, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 8, OfpsDnMxnEe(RP.sub_matrix_x(6, 8), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.set(1, 9, eep.get(3, 1) * ef.get(1, 3));
			}
			eeo.set(1, 1, RPD.get(1, 1));
			eeo.set(1, 2, RPD.get(1, 2));
			eeo.set(1, 3, RPD.get(1, 3));
			eeo.set(2, 3, RPD.get(1, 4));
			eeo.set(3, 3, RPD.get(1, 5));
			eeo.set(2, 1, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(3, 2, RPD.get(1, 8));
			eeo.set(3, 1, RPD.get(1, 9));
		}
		else if (lejps == 2)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 1));
				RP.set(1, 9, eep.get(3, 2));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 1));
				RP.set(1, 9, eep.get(3, 2));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 1));
				RP.set(1, 9, eep.get(3, 2));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(1, 1, RPD.get(1, 1));
			eeo.set(1, 2, RPD.get(1, 2));
			eeo.set(1, 3, RPD.get(1, 3));
			eeo.set(2, 3, RPD.get(1, 4));
			eeo.set(3, 3, RPD.get(1, 5));
			eeo.set(2, 1, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(3, 1, RPD.get(1, 8));
			eeo.set(3, 2, RPD.get(1, 9));
		}
		else if (lejps == 3)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(3, 2));
				RP.set(1, 8, eep.get(2, 1));
				RP.set(1, 9, eep.get(3, 1));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(3, 2));
				RP.set(1, 8, eep.get(2, 1));
				RP.set(1, 9, eep.get(3, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(1, 2));
				RP.set(1, 3, eep.get(1, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(3, 2));
				RP.set(1, 8, eep.get(2, 1));
				RP.set(1, 9, eep.get(3, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(1, 1, RPD.get(1, 1));
			eeo.set(1, 2, RPD.get(1, 2));
			eeo.set(1, 3, RPD.get(1, 3));
			eeo.set(2, 3, RPD.get(1, 4));
			eeo.set(3, 3, RPD.get(1, 5));
			eeo.set(2, 2, RPD.get(1, 6));
			eeo.set(3, 2, RPD.get(1, 7));
			eeo.set(2, 1, RPD.get(1, 8));
			eeo.set(3, 1, RPD.get(1, 9));
		}
	}
	else if (lejp == 52)
	{
		if (lejps == 1)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 2));
				RPD.sub_matrix_set_x(1, 8, OfpsDnMxnEe(RP.sub_matrix_x(1, 8), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.set(1, 9, eep.get(1, 1) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 2));
				RP.set(1, 9, eep.get(1, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 2));
				RP.set(1, 9, eep.get(1, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 8, OfpsDnMxnEe(RP.sub_matrix_x(6, 8), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.set(1, 9, eep.get(3, 1) * ef.get(1, 3));
			}
			eeo.set(3, 1, RPD.get(1, 1));
			eeo.set(3, 2, RPD.get(1, 2));
			eeo.set(3, 3, RPD.get(1, 3));
			eeo.set(2, 3, RPD.get(1, 4));
			eeo.set(1, 3, RPD.get(1, 5));
			eeo.set(2, 1, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(1, 2, RPD.get(1, 8));
			eeo.set(1, 1, RPD.get(1, 9));
		}
		else if (lejps == 2)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 1));
				RP.set(1, 9, eep.get(1, 2));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 1));
				RP.set(1, 9, eep.get(1, 2));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 1));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 1));
				RP.set(1, 9, eep.get(1, 2));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(3, 1, RPD.get(1, 1));
			eeo.set(3, 2, RPD.get(1, 2));
			eeo.set(3, 3, RPD.get(1, 3));
			eeo.set(2, 3, RPD.get(1, 4));
			eeo.set(1, 3, RPD.get(1, 5));
			eeo.set(2, 1, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(1, 1, RPD.get(1, 8));
			eeo.set(1, 2, RPD.get(1, 9));
		}
		else if (lejps == 3)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 1));
				RP.set(1, 9, eep.get(2, 1));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 1));
				RP.set(1, 9, eep.get(2, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(3, 2));
				RP.set(1, 3, eep.get(3, 3));
				RP.set(1, 4, eep.get(2, 3));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 1));
				RP.set(1, 9, eep.get(2, 1));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(3, 1, RPD.get(1, 1));
			eeo.set(3, 2, RPD.get(1, 2));
			eeo.set(3, 3, RPD.get(1, 3));
			eeo.set(2, 3, RPD.get(1, 4));
			eeo.set(1, 3, RPD.get(1, 5));
			eeo.set(1, 2, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(1, 1, RPD.get(1, 8));
			eeo.set(2, 1, RPD.get(1, 9));
		}
	}
	else if (lejp == 53)
	{
		if (lejps == 1)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 8, OfpsDnMxnEe(RP.sub_matrix_x(1, 8), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.set(1, 9, eep.get(1, 3) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(2, 3));
				RP.set(1, 9, eep.get(1, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(2, 3));
				RP.set(1, 9, eep.get(1, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 8, OfpsDnMxnEe(RP.sub_matrix_x(6, 8), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.set(1, 9, eep.get(1, 3) * ef.get(1, 3));
			}
			eeo.set(1, 1, RPD.get(1, 1));
			eeo.set(2, 1, RPD.get(1, 2));
			eeo.set(3, 1, RPD.get(1, 3));
			eeo.set(3, 2, RPD.get(1, 4));
			eeo.set(3, 3, RPD.get(1, 5));
			eeo.set(1, 2, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(2, 3, RPD.get(1, 8));
			eeo.set(1, 3, RPD.get(1, 9));
		}
		else if (lejps == 2)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(2, 3));
				RP.set(1, 8, eep.get(1, 2));
				RP.set(1, 9, eep.get(1, 3));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(2, 3));
				RP.set(1, 8, eep.get(1, 2));
				RP.set(1, 9, eep.get(1, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(2, 3));
				RP.set(1, 8, eep.get(1, 2));
				RP.set(1, 9, eep.get(1, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(1, 1, RPD.get(1, 1));
			eeo.set(2, 1, RPD.get(1, 2));
			eeo.set(3, 1, RPD.get(1, 3));
			eeo.set(3, 2, RPD.get(1, 4));
			eeo.set(3, 3, RPD.get(1, 5));
			eeo.set(2, 2, RPD.get(1, 6));
			eeo.set(2, 3, RPD.get(1, 7));
			eeo.set(1, 2, RPD.get(1, 8));
			eeo.set(1, 3, RPD.get(1, 9));
		}
		else if (lejps == 3)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 3));
				RP.set(1, 9, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 3));
				RP.set(1, 9, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(1, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(3, 1));
				RP.set(1, 4, eep.get(3, 2));
				RP.set(1, 5, eep.get(3, 3));
				RP.set(1, 6, eep.get(1, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(1, 3));
				RP.set(1, 9, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(1, 1, RPD.get(1, 1));
			eeo.set(2, 1, RPD.get(1, 2));
			eeo.set(3, 1, RPD.get(1, 3));
			eeo.set(3, 2, RPD.get(1, 4));
			eeo.set(3, 3, RPD.get(1, 5));
			eeo.set(1, 2, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(1, 3, RPD.get(1, 8));
			eeo.set(2, 3, RPD.get(1, 9));
		}
	}
	else if (lejp == 54)
	{
		if (lejps == 1)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(3, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 8, OfpsDnMxnEe(RP.sub_matrix_x(1, 8), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.set(1, 9, eep.get(3, 3) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(3, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(2, 3));
				RP.set(1, 9, eep.get(3, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(3, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(2, 3));
				RP.set(1, 9, eep.get(3, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 8, OfpsDnMxnEe(RP.sub_matrix_x(6, 8), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.set(1, 9, eep.get(3, 3) * ef.get(1, 3));
			}
			eeo.set(3, 1, RPD.get(1, 1));
			eeo.set(2, 1, RPD.get(1, 2));
			eeo.set(1, 1, RPD.get(1, 3));
			eeo.set(1, 2, RPD.get(1, 4));
			eeo.set(1, 3, RPD.get(1, 5));
			eeo.set(3, 2, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(2, 3, RPD.get(1, 8));
			eeo.set(3, 3, RPD.get(1, 9));
		}
		else if (lejps == 2)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(2, 3));
				RP.set(1, 8, eep.get(3, 2));
				RP.set(1, 9, eep.get(3, 3));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(2, 3));
				RP.set(1, 8, eep.get(3, 2));
				RP.set(1, 9, eep.get(3, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(2, 2));
				RP.set(1, 7, eep.get(2, 3));
				RP.set(1, 8, eep.get(3, 2));
				RP.set(1, 9, eep.get(3, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(3, 1, RPD.get(1, 1));
			eeo.set(2, 1, RPD.get(1, 2));
			eeo.set(1, 1, RPD.get(1, 3));
			eeo.set(1, 2, RPD.get(1, 4));
			eeo.set(1, 3, RPD.get(1, 5));
			eeo.set(2, 2, RPD.get(1, 6));
			eeo.set(2, 3, RPD.get(1, 7));
			eeo.set(3, 2, RPD.get(1, 8));
			eeo.set(3, 3, RPD.get(1, 9));
		}
		else if (lejps == 3)
		{
			if (ejt == 112)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(3, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 3));
				RP.set(1, 9, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 7, OfpsDnMxnEe(RP.sub_matrix_x(1, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			else if (ejt == 122)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(3, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 3));
				RP.set(1, 9, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 9, OfpsDnMxnEe(RP.sub_matrix_x(6, 9), offset, fwc, gain, dist_factor) * ef.get(1, 2));
			}
			else if (ejt == 123)
			{
				RP.set(1, 1, eep.get(3, 1));
				RP.set(1, 2, eep.get(2, 1));
				RP.set(1, 3, eep.get(1, 1));
				RP.set(1, 4, eep.get(1, 2));
				RP.set(1, 5, eep.get(1, 3));
				RP.set(1, 6, eep.get(3, 2));
				RP.set(1, 7, eep.get(2, 2));
				RP.set(1, 8, eep.get(3, 3));
				RP.set(1, 9, eep.get(2, 3));
				RPD.sub_matrix_set_x(1, 5, OfpsDnMxnEe(RP.sub_matrix_x(1, 5), offset, fwc, gain, dist_factor) * ef.get(1, 1));
				RPD.sub_matrix_set_x(6, 7, OfpsDnMxnEe(RP.sub_matrix_x(6, 7), offset, fwc, gain, dist_factor) * ef.get(1, 2));
				RPD.sub_matrix_set_x(8, 9, OfpsDnMxnEe(RP.sub_matrix_x(8, 9), offset, fwc, gain, dist_factor) * ef.get(1, 3));
			}
			eeo.set(3, 1, RPD.get(1, 1));
			eeo.set(2, 1, RPD.get(1, 2));
			eeo.set(1, 1, RPD.get(1, 3));
			eeo.set(1, 2, RPD.get(1, 4));
			eeo.set(1, 3, RPD.get(1, 5));
			eeo.set(3, 2, RPD.get(1, 6));
			eeo.set(2, 2, RPD.get(1, 7));
			eeo.set(3, 3, RPD.get(1, 8));
			eeo.set(2, 3, RPD.get(1, 9));
		}
	}
	return eeo;
}

Matrix_M_N OfpsEdgeThl(Matrix_M_N eep, double eef, int offset, int fwc, int gain, double dist_factor, int std_factor,
	double* ethf, int* et, double* ejpo, int* ejpsos)
{
	Matrix_M_N EF(1, 3, "ONE");
	Matrix_M_N PA(4, 3, "ONE"), PB(4, 3, "ONE"), PC(4, 3, "ONE");
	Matrix_M_N ETGF1(4, 3, "ONE"), ETGF2(4, 3, "ONE"), ETHFI(4, 3, "ONE");
	Matrix_M_N ETI(4, 3, "ONE"), EF1(4, 3, "ONE"), EF2(4, 3, "ONE"), EF3(4, 3, "ONE");
	Matrix_M_N PAN(4, 3, "ZERO"), PBN(4, 3, "ZERO"), PCN(4, 3, "ZERO");
	double EF2_C1 = 1, EF2_C2 = 1, ETHF_R_size, EJPSO_size;
	double value;
	int ETHF_RS;

	*et = 111;

	// Case-1
	value = (eep.get(1, 1) + eep.get(1, 2) + eep.get(1, 3) + eep.get(2, 3) + eep.get(3, 3)) / 5;
	for (int sz = 0; sz < PA.width(); sz++)
		PA.set(1, sz, value);
	PB.set(1, 1, (eep.get(2, 1) + eep.get(2, 2) + eep.get(3, 2)) / 3);
	PC.set(1, 1, eep.get(3, 1));
	PB.set(1, 2, (eep.get(2, 1) + eep.get(2, 2)) / 2);
	PC.set(1, 2, (eep.get(3, 1) + eep.get(3, 2)) / 2);
	PB.set(1, 3, (eep.get(2, 2) + eep.get(3, 2)) / 2);
	PC.set(1, 3, (eep.get(2, 1) + eep.get(3, 1)) / 2);

	// Case - 2
	value = (eep.get(1, 3) + eep.get(2, 3) + eep.get(3, 3) + eep.get(3, 2) + eep.get(3, 1)) / 5;
	for (int sz = 0; sz < PA.width(); sz++)
		PA.set(2, sz, value);
	PB.set(2, 1, (eep.get(1, 2) + eep.get(2, 2) + eep.get(2, 1)) / 3);
	PC.set(2, 1, eep.get(1, 1));
	PB.set(2, 2, (eep.get(2, 1) + eep.get(2, 2)) / 2);
	PC.set(2, 2, (eep.get(1, 1) + eep.get(1, 2)) / 2);
	PB.set(2, 3, (eep.get(2, 2) + eep.get(1, 2)) / 2);
	PC.set(2, 3, (eep.get(2, 1) + eep.get(1, 1)) / 2);

	// Case - 3
	value = (eep.get(1, 1) + eep.get(2, 1) + eep.get(3, 1) + eep.get(3, 2) + eep.get(3, 3)) / 5;
	for (int sz = 0; sz < PA.width(); sz++)
		PA.set(3, sz, value);
	PB.set(3, 1, (eep.get(1, 2) + eep.get(2, 2) + eep.get(2, 3)) / 3);
	PC.set(3, 1, eep.get(1, 3));
	PB.set(3, 2, (eep.get(2, 2) + eep.get(2, 3)) / 2);
	PC.set(3, 2, (eep.get(1, 2) + eep.get(1, 3)) / 2);
	PB.set(3, 3, (eep.get(1, 2) + eep.get(2, 2)) / 2);
	PC.set(3, 3, (eep.get(1, 3) + eep.get(2, 3)) / 2);

	// Case - 4
	value = (eep.get(3, 1) + eep.get(2, 1) + eep.get(1, 1) + eep.get(1, 2) + eep.get(1, 3)) / 5;
	for (int sz = 0; sz < PA.width(); sz++)
		PA.set(4, sz, value);
	PB.set(4, 1, (eep.get(2, 2) + eep.get(3, 2) + eep.get(2, 3)) / 3);
	PC.set(4, 1, eep.get(3, 3));
	PB.set(4, 2, (eep.get(2, 2) + eep.get(2, 3)) / 2);
	PC.set(4, 2, (eep.get(3, 2) + eep.get(3, 3)) / 2);
	PB.set(4, 3, (eep.get(2, 2) + eep.get(3, 2)) / 2);
	PC.set(4, 3, (eep.get(3, 3) + eep.get(2, 3)) / 2);

	for (int ni = 1; ni <= 4; ni++)
	{
		for (int nj = 1; nj <= 3; nj++)
		{
			PAN.set(ni, nj, _OfpsIgn(PA.get(ni, nj), offset, fwc, gain, dist_factor));
			PBN.set(ni, nj, _OfpsIgn(PB.get(ni, nj), offset, fwc, gain, dist_factor));
			PCN.set(ni, nj, _OfpsIgn(PC.get(ni, nj), offset, fwc, gain, dist_factor));

			value = (fabs(PA.get(ni, nj) - PB.get(ni, nj)) / fabs(std_factor * PAN.get(ni, nj) + std_factor * PBN.get(ni, nj)));
			if (value > 1)
				ETGF1.set(ni, nj, 1 + value);
			else
				ETGF1.set(ni, nj, 0);

			value = (fabs(PB.get(ni, nj) - PC.get(ni, nj)) / fabs(std_factor * PBN.get(ni, nj) + std_factor * PCN.get(ni, nj)));
			if (value > 1)
				ETGF2.set(ni, nj, 1 + value);
			else
				ETGF2.set(ni, nj, 0);


			if ((ETGF1.get(ni, nj) == 0) && (ETGF2.get(ni, nj) >= 1))
			{
				ETI.set(ni, nj, 112);
				ETHFI.set(ni, nj, ETGF2.get(ni, nj) + ETGF2.get(ni, nj));
			}
			else if ((ETGF1.get(ni, nj) >= 1) && (ETGF2.get(ni, nj) == 0))
			{
				ETI.set(ni, nj, 122);
				ETHFI.set(ni, nj, ETGF1.get(ni, nj) + ETGF1.get(ni, nj));
			}
			else
			{
				ETI.set(ni, nj, 123);
				ETHFI.set(ni, nj, ETGF1.get(ni, nj) + ETGF2.get(ni, nj));
			}
		}
	}

	Matrix_M_N matrix_temp = ETHFI.find(ETHFI.max_ele());
	ETHF_R_size = matrix_temp.height();
	EJPSO_size = matrix_temp.height();

	ETHF_RS = matrix_temp.get(1, 1);
	*ejpsos = matrix_temp.get(1, 2);

	*et = ETI.get(ETHF_RS, *ejpsos);
	*ethf = ETHFI.get(ETHF_RS, *ejpsos);
	if (PA.get(ETHF_RS, *ejpsos) > PB.get(ETHF_RS, *ejpsos))
	{
		EF2_C1 = 1 + eef * (PB.get(ETHF_RS, *ejpsos) - PA.get(ETHF_RS, *ejpsos)) / (256 * dist_factor);
		EF.set(1, 1, 1);
	}
	else
	{
		EF.set(1, 1, 1 + eef * (PA.get(ETHF_RS, *ejpsos) - PB.get(ETHF_RS, *ejpsos)) / (256 * dist_factor));
		EF2_C1 = 1;
	}

	if (PB.get(ETHF_RS, *ejpsos) > PC.get(ETHF_RS, *ejpsos))
	{
		EF.set(1, 3, 1 + eef * (PC.get(ETHF_RS, *ejpsos) - PB.get(ETHF_RS, *ejpsos)) / (256 * dist_factor));
		EF2_C2 = 1;
	}
	else
	{
		EF2_C2 = 1 + eef * (PB.get(ETHF_RS, *ejpsos) - PC.get(ETHF_RS, *ejpsos)) / (256 * dist_factor);
		EF.set(1, 3, 1);
	}

	EF.set(1, 2, min(EF2_C1, EF2_C2));

	if (ETHF_RS == 1)
		*ejpo = 41;
	else if (ETHF_RS == 2)
		*ejpo = 42;
	else if (ETHF_RS == 3)
		*ejpo = 43;
	else if (ETHF_RS == 4)
		*ejpo = 44;
	else
		*ejpo = -1;

	return EF;
}

Matrix_M_N OfpsEdgeTh(unsigned char pa, unsigned char pb, unsigned char pc, double eef, int offset,
	int fwc, int gain, double dist_factor, int std_factor, double* ETHF, int* ET)
{
	Matrix_M_N EF(1, 3, "ONE");
	double EF2_C1 = 1, EF2_C2 = 1, ETGF1, ETGF2;

	double PAN = _OfpsIgn(pa, offset, fwc, gain, dist_factor);
	double PBN = _OfpsIgn(pb, offset, fwc, gain, dist_factor);
	double PCN = _OfpsIgn(pc, offset, fwc, gain, dist_factor);

	if ((abs(pa - pb) / fabs(std_factor * PAN + std_factor * PBN)) > 1)
	{
		ETGF1 = 1 + (abs(pa - pb) / fabs(std_factor * PAN + std_factor * PBN));
		if (pa > pb)
		{
			EF2_C1 = 1 + eef * (pb - pa) / (256 * dist_factor);
			EF.set(1, 1, 1);
		}
		else
		{
			EF.set(1, 1, 1 + eef * (pa - pb) / (256 * dist_factor));
			EF2_C1 = 1;
		}
	}
	else
		ETGF1 = 0;

	if ((abs(pb - pc) / fabs(std_factor * PBN + dist_factor * PCN)) > 1)
	{
		ETGF2 = 1 + (abs(pb - pc) / fabs(std_factor * PBN + std_factor * PCN));
		if (pb > pc)
		{
			EF.set(1, 3, 1 + eef * (pc - pb) / (256 * dist_factor));
			EF2_C2 = 1;
		}
		else
		{
			EF2_C2 = 1 + eef * (pb - pc) / (256 * dist_factor);
			EF.set(1, 3, 1);
		}
	}
	else
		ETGF2 = 0;

	EF.set(1, 2, min(EF2_C1, EF2_C2));
	if ((ETGF1 == 0) && (ETGF2 >= 1))
	{
		*ET = 122;
		*ETHF = ETGF2 + ETGF2;
	}
	else if ((ETGF1 >= 1) && (ETGF2 == 0))
	{
		*ET = 112;
		*ETHF = ETGF1 + ETGF1;
	}
	else
	{
		*ET = 123;
		*ETHF = ETGF1 + ETGF2;
	}

	return EF;
}

Matrix_M_N OfpsEdgeEn(Matrix_M_N eep, double eef, int offset, int fwc, int gain, double dist_factor, int std_factor, double* ejp, double* ejps, double* ejt)
{
	double VD, HD, D1D, D2D;
	int VT, HT, D1T, D2T;
	//Matrix_M_N VEF(1, 3, "ONE"), HEF(1, 3, "ONE"), D1EF(1, 3, "ONE"), D2EF(1, 3, "ONE");
	Matrix_M_N VA(1, 3, "ONE"), HA(1, 3, "ONE"), D1A(1, 3, "ONE"), D2A(1, 3, "ONE"), LEF(1, 3, "ONE");
	Matrix_M_N EF(1, 3, "ONE");

	for (int i = 1; i <= 3; i++)
		VA.set(1, i, (eep.get(i, 1) + eep.get(i, 2) + eep.get(i, 3)) / 3);
	Matrix_M_N VEF = OfpsEdgeTh(VA.get(1, 3), VA.get(1, 2), VA.get(1, 1), eef, offset, fwc, gain, dist_factor, std_factor, &VD, &VT);

	for (int j = 1; j <= 3; j++)
		HA.set(1, j, (eep.get(1, j) + eep.get(2, j) + eep.get(3, j)) / 3);
	Matrix_M_N HEF = OfpsEdgeTh(HA.get(1, 3), HA.get(1, 2), HA.get(1, 1), eef, offset, fwc, gain, dist_factor, std_factor, &HD, &HT);

	D1A.set(1, 2, (eep.get(1, 1) + eep.get(2, 2) + eep.get(3, 3)) / 3);
	D1A.set(1, 3, (eep.get(1, 3) + eep.get(1, 2) + eep.get(2, 3)) / 3);
	D1A.set(1, 1, (eep.get(2, 1) + eep.get(3, 1) + eep.get(3, 2)) / 3);
	Matrix_M_N D1EF = OfpsEdgeTh(D1A.get(1, 3), D1A.get(1, 2), D1A.get(1, 1), eef, offset, fwc, gain, dist_factor, std_factor, &D1D, &D1T);

	D2A.set(1, 2, (eep.get(1, 3) + eep.get(2, 2) + eep.get(3, 1)) / 3);
	D2A.set(1, 3, (eep.get(1, 1) + eep.get(1, 2) + eep.get(2, 1)) / 3);
	D2A.set(1, 1, (eep.get(3, 2) + eep.get(3, 3) + eep.get(2, 3)) / 3);
	Matrix_M_N D2EF = OfpsEdgeTh(D2A.get(1, 3), D2A.get(1, 2), D2A.get(1, 1), eef, offset, fwc, gain, dist_factor, std_factor, &D2D, &D2T);

	double LD, LEJP;
	int LEJPS, LT;
	LEF = OfpsEdgeThl(eep, eef, offset, fwc, gain, dist_factor, std_factor, &LD, &LT, &LEJP, &LEJPS);
	Matrix_M_N VEC_D(1, 5);
	VEC_D.set(1, 1, VD);
	VEC_D.set(1, 2, HD);
	VEC_D.set(1, 3, D1D);
	VEC_D.set(1, 4, D2D);
	VEC_D.set(1, 5, LD);

	*ejp = -1;
	*ejt = 111;

	double max_tmp = VEC_D.max_ele();
	if (max_tmp > 1)
	{
		if (VEC_D.get(1, 1) == max_tmp)
		{
			*ejp = 0;
			*ejt = VT;
			EF = VEF;
		}
		else if (VEC_D.get(1, 2) == max_tmp)
		{
			*ejp = 1;
			*ejt = HT;
			EF = HEF;
		}
		else if (VEC_D.get(1, 3) == max_tmp)
		{
			*ejp = 2;
			*ejt = D1T;
			EF = D1EF;
		}
		else if (VEC_D.get(1, 4) == max_tmp)
		{
			*ejp = 3;
			*ejt = D2T;
			EF = D2EF;
		}
		else if (VEC_D.get(1, 5) == max_tmp)
		{
			*ejp = LEJP;
			*ejps = LEJPS;
			*ejt = LT;
			EF = LEF;
		}
	}
	else if (max_tmp <= 1)
	{
		*ejp = -1;
		*ejt = 111;
		EF = 1;
	}

	return EF;
}

Matrix_M_N _OfpsNormalization(Matrix_M_N pin, double eef, int offset, int fwc, int gain, double dist_factor, int std_factor)
{
	Matrix_M_N dept(pin.height(), pin.width());
	Matrix_M_N EF(1, 3, "ONE");
	double EJP, EJPS, EJT;

	EF = OfpsEdgeEn(pin, eef, offset, fwc, gain, dist_factor, std_factor, &EJP, &EJPS, &EJT);
	if (EJP == 0)
	{
		if (EJT == 123)
		{
			dept.sub_matrix_set_y(1, 1, OfpsDnMxnEe(pin.sub_matrix_y(1, 1), offset, fwc, gain, dist_factor) * EF.get(1, 1));
			dept.sub_matrix_set_y(2, 2, OfpsDnMxnEe(pin.sub_matrix_y(2, 2), offset, fwc, gain, dist_factor) * EF.get(1, 2));
			dept.sub_matrix_set_y(3, 3, OfpsDnMxnEe(pin.sub_matrix_y(3, 3), offset, fwc, gain, dist_factor) * EF.get(1, 3));
		}
		else if (EJT == 112)
		{
			dept.sub_matrix_set_y(1, 2, OfpsDnMxnEe(pin.sub_matrix_y(1, 2), offset, fwc, gain, dist_factor) * EF.get(1, 2));
			dept.sub_matrix_set_y(3, 3, OfpsDnMxnEe(pin.sub_matrix_y(3, 3), offset, fwc, gain, dist_factor) * EF.get(1, 3));
		}
		else if (EJT == 122)
		{
			dept.sub_matrix_set_y(1, 1, OfpsDnMxnEe(pin.sub_matrix_y(1, 1), offset, fwc, gain, dist_factor) * EF.get(1, 1));
			dept.sub_matrix_set_y(2, 3, OfpsDnMxnEe(pin.sub_matrix_y(2, 3), offset, fwc, gain, dist_factor) * EF.get(1, 2));
		}
	}
	else if (EJP == 1)
	{
		if (EJT == 123)
		{
			dept.sub_matrix_set_x(1, 1, OfpsDnMxnEe(pin.sub_matrix_x(1, 1), offset, fwc, gain, dist_factor) * EF.get(1, 1));
			dept.sub_matrix_set_x(2, 2, OfpsDnMxnEe(pin.sub_matrix_x(2, 2), offset, fwc, gain, dist_factor) * EF.get(1, 2));
			dept.sub_matrix_set_x(3, 3, OfpsDnMxnEe(pin.sub_matrix_x(3, 3), offset, fwc, gain, dist_factor) * EF.get(1, 3));
		}
		else if (EJT == 112)
		{
			dept.sub_matrix_set_x(1, 2, OfpsDnMxnEe(pin.sub_matrix_x(1, 2), offset, fwc, gain, dist_factor) * EF.get(1, 2));
			dept.sub_matrix_set_x(3, 3, OfpsDnMxnEe(pin.sub_matrix_x(3, 3), offset, fwc, gain, dist_factor) * EF.get(1, 3));
		}
		else if (EJT == 122)
		{
			dept.sub_matrix_set_x(1, 1, OfpsDnMxnEe(pin.sub_matrix_x(1, 1), offset, fwc, gain, dist_factor) * EF.get(1, 1));
			dept.sub_matrix_set_x(2, 3, OfpsDnMxnEe(pin.sub_matrix_x(2, 3), offset, fwc, gain, dist_factor) * EF.get(1, 2));
		}
	}
	else if (EJP == 2)
		dept = OfpsDnMxnScDEe(pin, 2, EJT, EF, offset, fwc, gain, dist_factor);
	else if (EJP == 3)
		dept = OfpsDnMxnScDEe(pin, 3, EJT, EF, offset, fwc, gain, dist_factor);
	else if (EJP == 41)
		dept = OfpsDnMxnScLEe(pin, EJT, 41, EJPS, EF, offset, fwc, gain, dist_factor);
	else if (EJP == 42)
		dept = OfpsDnMxnScLEe(pin, EJT, 42, EJPS, EF, offset, fwc, gain, dist_factor);
	else if (EJP == 43)
		dept = OfpsDnMxnScLEe(pin, EJT, 43, EJPS, EF, offset, fwc, gain, dist_factor);
	else if (EJP == 44)
		dept = OfpsDnMxnScLEe(pin, EJT, 44, EJPS, EF, offset, fwc, gain, dist_factor);
	else if (EJP == -1)
		dept = OfpsDnMxnEe(pin, offset, fwc, gain, dist_factor);

	return dept;
}

void ISP::OfpsNormalization(double* src_image, unsigned int width, unsigned int height, double* ofpsIgn_image, double eef, int offset, int FWC, int Gain, double dist_factor, int std_factor)
{
	for (int i = 1; i < height - 1; i++) {
		for (int j = 1; j < width - 1; j++) {
			Matrix_M_N filter_block_dn = Matrix_M_N(3, 3, "ZERO");
			for (int y = 1; y <= 3; y++) {
				for (int x = 1; x <= 3; x++) {
					filter_block_dn.set(y, x, src_image[width * (i + y - 2) + (j + x - 2)]);
				}
			}
			Matrix_M_N pattern_norm = _OfpsNormalization(filter_block_dn, eef, offset, FWC, Gain, dist_factor, std_factor);
			ofpsIgn_image[i * width + j] = pattern_norm.get(2, 2);
		}
	}
}

////////////////////Test Function////////////////////
double ISP::OfpsIgn(int i)
{
	double eef = 0.5, offset = 0, fwc = 30000, gain = 8, dist_factor = 0.3, std_factor = 5;
	unsigned char pa = 20, pb = 200, pc = 33;
	int tag = 0;
	Matrix_M_N pin(3, 3), pout(3, 3);
	double ethf = 0, ejpo = 0;
	int et = 0;
	int ejpsos = 0;
	Matrix_M_N eep(3, 3), eeo(3, 3);
	eep.set(1, 1, 11);
	eep.set(1, 2, 22);
	eep.set(1, 3, 33);
	eep.set(2, 1, 4);
	eep.set(2, 2, 5);
	eep.set(2, 3, 6);
	eep.set(3, 1, 7);
	eep.set(3, 2, 8);
	eep.set(3, 3, 9);
	Matrix_M_N ef(1, 3);

	tag = 0;
	switch (tag)
	{
	case 0:
	{
		pin.set(1, 1, 11);
		pin.set(1, 2, 22);
		pin.set(1, 3, 33);
		pin.set(2, 1, 61);
		pin.set(2, 2, 5);
		pin.set(2, 3, 4);
		pin.set(3, 1, 73);
		pin.set(3, 2, 8);
		pin.set(3, 3, 91);

		pout = _OfpsNormalization(pin, eef, offset, fwc, gain, dist_factor, std_factor);
		if (i == 1)
			return pout.get(1, 1);
		else if (i == 2)
			return pout.get(1, 2);
		else if (i == 3)
			return pout.get(1, 3);
		else if (i == 4)
			return pout.get(2, 1);
		else if (i == 5)
			return pout.get(2, 2);
		else if (i == 6)
			return pout.get(2, 3);
		else if (i == 7)
			return pout.get(3, 1);
		else if (i == 8)
			return pout.get(3, 2);
		else if (i == 9)
			return pout.get(3, 3);
		break;
	}
	case 1:
	{
		ef = OfpsEdgeTh(pa, pb, pc, eef, offset, fwc, gain, dist_factor, std_factor, &ethf, &et);
		if (i == 1)
			return ef.get(1, 1);
		else if (i == 2)
			return ef.get(1, 2);
		else if (i == 3)
			return ef.get(1, 3);
		else if (i == 4)
			return ethf;
		else if (i == 5)
			return et;
		else
			return 666;
		break;
	}
	case 2:
	{
		ef = OfpsEdgeThl(eep, eef, offset, fwc, gain, dist_factor, std_factor, &ethf, &et, &ejpo, &ejpsos);
		if (i == 1)
			return ef.get(1, 1);
		else if (i == 2)
			return ef.get(1, 2);
		else if (i == 3)
			return ef.get(1, 3);
		else if (i == 4)
			return ethf;
		else if (i == 5)
			return et;
		else if (i == 6)
			return ejpo;
		else if (i == 7)
			return ejpsos;
		else
			return 666;
	}
	}

	/*Matrix_M_N eep(3, 3), eeo(3, 3);
	Matrix_M_N ef(1, 3);
	eep.set(1, 1, 1);
	eep.set(1, 2, 2);
	eep.set(1, 3, 3);
	eep.set(2, 1, 4);
	eep.set(2, 2, 5);
	eep.set(2, 3, 6);
	eep.set(3, 1, 7);
	eep.set(3, 2, 8);
	eep.set(3, 3, 9);
	ef.set(1, 1, 1);
	ef.set(1, 2, 2);
	ef.set(1, 3, 3);
	eeo = OfpsDnMxnScDEe(eep, 2, 112, ef, 0, 30000, 8, 0.8);
	if (i == 1)
		return eeo.get(1, 1);
	else if (i == 2)
		return eeo.get(1, 2);
	else if (i == 3)
		return eeo.get(1, 3);
	else if (i == 4)
		return eeo.get(2, 1);
	else if (i == 5)
		return eeo.get(2, 2);
	else if (i == 6)
		return eeo.get(2, 3);
	else if (i == 7)
		return eeo.get(3, 1);
	else if (i == 8)
		return eeo.get(3, 2);
	else if (i == 9)
		return eeo.get(3, 3);*/

		/*Matrix_M_N SCI(1, 8, "ONE");

		SCI.set(1, 1, 2);
		SCI.set(1, 2, 3);
		SCI.set(1, 3, 5);
		SCI.set(1, 4, 7);
		SCI.set(1,5,8);
		SCI.set(1,6,1);
		SCI.set(1,7,2);
		SCI.set(1,8,2);

		Matrix_M_N SCO = OfpsDnMxnEe(SCI, 0, 30000, 8, 0.8);

		return SCO.get(1, i);*/
		//return _OfpsIgn(90, 0, 30000, 8, 0.8);
	return 0.0;
}

void ISP::MeanBlur(int* src, int* dst, unsigned int width, unsigned int height, unsigned int blurLevel)
{
	int ksize = blurLevel / 2;
	double* kernel = new double[blurLevel * blurLevel];
	double kernelbase = blurLevel * blurLevel;
	int kcount = 0;

	for (int x = -ksize; x <= ksize; x++)
	{
		for (int y = -ksize; y <= ksize; y++)
		{
			kernel[kcount] = 1;
			kcount++;
		}
	}

	kernelbase = kernel[0];
	kcount = 0;
	for (int x = -ksize; x <= ksize; x++)
	{
		for (int y = -ksize; y <= ksize; y++)
		{
			kernel[kcount] /= kernelbase;
			kcount++;
		}
	}

	Blur(src, dst, width, height, blurLevel, kernel);
}

void ISP::Normalization(uint16_t* src, uint8_t* outputImage, uint16_t width, uint16_t height)
{
	uint16_t nRangeMax = 1023;
	uint16_t nRangeMin = 0;

	uint16_t nMaxofImage = 0;
	int16_t nMinofImage = INT16_MAX;

	for (uint16_t idx = 0; idx < width * height; idx++)
	{
		nMaxofImage = max(src[idx], nMaxofImage);
		nMinofImage = min(src[idx], nMinofImage);
	}

	uint16_t nMax = nMaxofImage;
	uint16_t nMin = nMinofImage;

	float* aNor = new float[width * height];

	for (uint16_t idx = 0; idx < width * height; idx++)
	{
		aNor[idx] = (float(src[idx]) - float(nMin));
		aNor[idx] = (aNor[idx] / (float(nMax) - float(nMin))) * nRangeMax;
		outputImage[idx] = (uint8_t((uint16_t(aNor[idx]) >> 2)));
	}

	delete aNor;
}

void ISP::GaussianFilter(uint16_t* src, uint16_t* outputImage, uint16_t width, uint16_t height)
{
	uint8_t templates[25] = { 1, 4, 7, 4, 1,
						  4, 16, 26, 16, 4,
						  7, 26, 41, 26, 7,
						  4, 16, 26, 16, 4,
						  1, 4, 7, 4, 1 };

	memcpy(outputImage, src, width * height * sizeof(uint16_t));

	for (uint16_t j = 2; j < height - 2; j++)
	{
		for (uint16_t i = 2; i < width - 2; i++)
		{
			uint16_t sum = 0;
			uint16_t index = 0;
			for (uint16_t m = j - 2; m < j + 3; m++)
			{
				for (uint16_t n = i - 2; n < i + 3; n++)
				{
					sum += src[m * width + n] * templates[index++];
				}
			}
			sum /= 273;
			outputImage[j * width + i] = sum;
		}
	}
}

void ISP::BLC_Wash_Ver00_00_00(int* data_array, int width, int height)
{
	int temp1, temp2, t0, t1, t2, t3;
	int L1[4] = { 614, 1106, 638, 1128 };
	int L2[4] = { 296, 1064, 316, 1082 };
	int L3[4] = { 1166, 1246, 1188, 1268 };
	int L4[4] = { 72, 250, 100, 278 };
	int L5[4] = { 206, 484, 230, 510 };
	int L6[4] = { 884, 1080, 892, 1086 };
	int L7[4] = { 18, 894, 26, 902 };
	int L8[4] = { 564, 1060, 572, 1068 };
	int L9[4] = { 882, 958, 890, 964 };
	t0 = L1[0];
	t1 = L1[1];
	t2 = L1[2];
	t3 = L1[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L2[0];
	t1 = L2[1];
	t2 = L2[2];
	t3 = L2[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L3[0];
	t1 = L3[1];
	t2 = L3[2];
	t3 = L3[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L4[0];
	t1 = L4[1];
	t2 = L4[2];
	t3 = L4[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L5[0];
	t1 = L5[1];
	t2 = L5[2];
	t3 = L5[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L6[0];
	t1 = L6[1];
	t2 = L6[2];
	t3 = L6[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L7[0];
	t1 = L7[1];
	t2 = L7[2];
	t3 = L7[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L8[0];
	t1 = L8[1];
	t2 = L8[2];
	t3 = L8[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
	t0 = L9[0];
	t1 = L9[1];
	t2 = L9[2];
	t3 = L9[3];
	temp1 = t2 - t0;
	temp2 = t3 - t1;
	for (int i = t0 + 1; i < t2; i++) {
		for (int j = t1 + 1; j < t3; j++) {
			data_array[i * width + j] = (data_array[(t0 * width + j)] * (i - t0) / temp1 + data_array[(t2 * width + j)] * (t2 - i) / temp1) * 0.5 + (data_array[(i * width + t1)] * (j - t1) / temp2 + data_array[(i * width + t3)] * (t3 - j) / temp2) * 0.5;
		}
	}
}

void ISP::LensShadingCorrection_V00_00_00(int* data_input, int* data_output, int total_pixel, int* LSC_array, int LSC_Level)
{
	for (int i = 0; i < total_pixel; i++) {
		data_output[i] = int(((double(data_input[i]) / (double(LSC_array[i]) + 100)) * LSC_Level) + 0.5);
	}
}

void ISP::WhiteBalance(int* data_input, int* data_output, int width, int height, float ch1, float ch2, float ch3, float ch4)
{
	int x_size = height;
	int y_size = width;
	float A_gain = ch1;
	float B_gain = ch2;
	float C_gain = ch3;
	float D_gain = ch4;
	int temp;
	for (int i = 0; i < x_size; i = i + 2) {
		for (int j = 0; j < y_size; j = j + 2) {
			temp = data_input[i * y_size + j];
			temp *= A_gain;
			if (temp > 1023) temp = 1023;
			else if (temp < 0) temp = 0;
			data_output[i * y_size + j] = temp;
			temp = data_input[i * y_size + j + 1];
			temp *= B_gain;
			if (temp > 1023) temp = 1023;
			else if (temp < 0) temp = 0;
			data_output[i * y_size + j + 1] = temp;
			temp = data_input[(i + 1) * y_size + j];
			temp *= C_gain;
			if (temp > 1023) temp = 1023;
			else if (temp < 0) temp = 0;
			data_output[(i + 1) * y_size + j] = temp;
			temp = data_input[(i + 1) * y_size + j + 1];
			temp *= D_gain;
			if (temp > 1023) temp = 1023;
			else if (temp < 0) temp = 0;
			data_output[(i + 1) * y_size + j + 1] = temp;
		}
	}
	/*for (int i = 0; i < x_size; i = i + 2) {
		for (int j = 0; j < y_size; j = j + 2) {
			data_array[i * y_size + j] = data_array[i * y_size + j] * A_gain;
			if (data_array[i * y_size + j] > 1023)
				data_array[i * y_size + j] = 1023;
			if (data_array[i * y_size + j] < 0)
				data_array[i * y_size + j] = 0;
			data_array[i * y_size + j + 1] = data_array[i * y_size + j + 1] * B_gain;
			if (data_array[i * y_size + j + 1] > 1023)
				data_array[i * y_size + j + 1] = 1023;
			if (data_array[i * y_size + j + 1] < 0)
				data_array[i * y_size + j + 1] = 0;
			data_array[(i + 1) * y_size + j] = data_array[(i + 1) * y_size + j] * C_gain;
			if (data_array[(i + 1) * y_size + j] > 1023)
				data_array[(i + 1) * y_size + j] = 1023;
			if (data_array[(i + 1) * y_size + j] < 0)
				data_array[(i + 1) * y_size + j] = 0;
			data_array[(i + 1) * y_size + j + 1] = data_array[(i + 1) * y_size + j + 1] * D_gain;
			if (data_array[(i + 1) * y_size + j + 1] > 1023)
				data_array[(i + 1) * y_size + j + 1] = 1023;
			if (data_array[(i + 1) * y_size + j + 1] < 0)
				data_array[(i + 1) * y_size + j + 1] = 0;
		}
	}*/
}

typedef struct
{
	int Blue;
	int Green;
	int Red;
}ColorBGR;

void ISP::Demosaic_V00_00_00(int* data_array, int y_size, int x_size, int* BGR_output)
{
	ColorBGR* color_output = (ColorBGR*)BGR_output;
	int  i, j, temp1, temp2, temp3, temp4;
	for (i = 0; i < x_size; i = j + 2) {
		for (j = 0; j < y_size; j = j + 2) {
			temp1 = i * y_size + j;//左上
			temp2 = i * y_size + j + 1;//右上
			temp3 = (i + 1) * y_size + j;//左下
			temp4 = (i + 1) * y_size + j + 1;//右下
			color_output[temp1].Red = data_array[temp1];
			color_output[temp2].Green = data_array[temp2];
			color_output[temp3].Green = data_array[temp3];
			color_output[temp4].Blue = data_array[temp4];
		}
	}
	for (i = 1; i < (x_size - 1); i = i + 2) {
		for (j = 1; j < (y_size - 1); j = j + 2) {
			temp1 = i * y_size + j;//左上
			temp2 = i * y_size + j + 1;//右上
			temp3 = (i + 1) * y_size + j;//左下
			temp4 = (i + 1) * y_size + j + 1;//右下
			color_output[temp1].Red = (data_array[temp1 - y_size - 1] + data_array[temp1 - y_size + 1] + data_array[temp1 + y_size - 1] + data_array[temp1 + y_size + 1]) * 0.25;
			color_output[temp2].Red = (data_array[temp2 - y_size] + data_array[temp2 + y_size]) * 0.5;
			color_output[temp3].Red = (data_array[temp3 - 1] + data_array[temp3 + 1]) * 0.5;
			color_output[temp4].Red = data_array[temp4];
			color_output[temp1].Green = (data_array[temp1 - y_size] + data_array[temp1 + y_size] + data_array[temp1 - 1] + data_array[temp1 + 1]) * 0.25;
			color_output[temp2].Green = data_array[temp2] * 0.5 + ((data_array[temp2 - y_size - 1] + data_array[temp2 - y_size + 1] + data_array[temp2 + y_size - 1] + data_array[temp2 + y_size + 1]) * 0.125);
			color_output[temp3].Green = data_array[temp3] * 0.5 + ((data_array[temp3 - y_size - 1] + data_array[temp3 - y_size + 1] + data_array[temp3 + y_size - 1] + data_array[temp3 + y_size + 1]) * 0.125);
			color_output[temp4].Green = (data_array[temp4 + 1] + data_array[temp4 - 1] + data_array[temp4 - y_size] + data_array[temp4 + y_size]) * 0.2;
			color_output[temp1].Blue = data_array[temp1];
			color_output[temp2].Blue = (data_array[temp2 + 1] + data_array[temp2 - 1]) * 0.5;
			color_output[temp3].Blue = (data_array[temp3 - y_size] + data_array[temp3 + y_size]) * 0.5;
			color_output[temp4].Blue = (data_array[temp4 - y_size - 1] + data_array[temp4 - y_size + 1] + data_array[temp4 + y_size - 1] + data_array[temp4 + y_size + 1]) * 0.25;
		}
		temp1 = i * y_size;//左行上
		temp2 = temp1 + y_size;//左行下
		temp3 = temp2 - 1;//右行上
		temp4 = temp3 + y_size;//右行下
		color_output[temp1].Red = (data_array[temp2] + data_array[temp1 - y_size]) * 0.5;
		color_output[temp2].Red = data_array[temp2];
		color_output[temp3].Red = (data_array[temp3 - y_size - 1] + data_array[temp3 + y_size - 1]) * 0.5;
		color_output[temp4].Red = data_array[temp4 - 1];
		color_output[temp1].Green = data_array[temp1] * 0.5 + ((data_array[temp1 - y_size + 1] + data_array[temp2 + 1]) * 0.25);
		color_output[temp2].Green = (data_array[temp1] + data_array[temp2 + 1] + data_array[temp2 + y_size]) / 3;
		color_output[temp3].Green = (data_array[temp1 - 1] + data_array[temp4] + data_array[temp3 - 1]) / 3;
		color_output[temp4].Green = data_array[temp4] * 0.5 + ((data_array[temp3 - 1] + data_array[temp4 + y_size - 1]) * 0.25);
		color_output[temp1].Blue = data_array[temp1 + 1];
		color_output[temp2].Blue = (data_array[temp1 + 1] + data_array[temp2 + y_size + 1]) * 0.5;
		color_output[temp3].Blue = data_array[temp3];
		color_output[temp4].Blue = (data_array[temp3] + data_array[temp4 + y_size]) * 0.5;
	}
	temp1 = y_size * (x_size - 1);
	temp2 = temp1 - y_size;
	for (j = 1; j < (y_size - 1); j = j + 2) {
		color_output[j].Red = (data_array[j - 1] + data_array[j + 1]) * 0.5;
		color_output[j + 1].Red = data_array[j + 1];
		color_output[temp1 + j].Red = (data_array[temp2 + j - 1] + data_array[temp2 + j + 1]) * 0.5;
		color_output[temp1 + j + 1].Red = data_array[temp1 + j + 1];
		color_output[j].Green = data_array[j] * 0.5 + ((data_array[y_size + j - 1] + data_array[y_size + j + 1]) * 0.25);
		color_output[j + 1].Green = (data_array[j] + data_array[j + 2] + data_array[j + y_size]) / 3;
		color_output[temp1 + j].Green = (data_array[temp1 + j - 1] + data_array[temp1 + j + 1] + data_array[temp2 + j]) / 3;
		color_output[temp1 + j + 1].Green = data_array[temp1 + j + 1] * 0.5 + ((data_array[temp2 + j] + data_array[temp2 + j + 2]) * 0.25);
		color_output[j].Blue = data_array[j + y_size];
		color_output[j + 1].Blue = (data_array[j + y_size] + data_array[j + y_size + 2]) * 0.5;
		color_output[temp1 + j].Blue = data_array[temp1 + j];
		color_output[temp1 + j + 1].Blue = (data_array[temp1 + j] + data_array[temp1 + j + 2]) * 0.5;
	}
	color_output[y_size - 1].Red = data_array[y_size - 2];
	color_output[0].Red = data_array[0];
	color_output[temp1].Red = data_array[temp2];
	color_output[temp1 + y_size - 1].Red = data_array[temp1 - 2];
	color_output[y_size - 1].Green = (data_array[y_size - 1] + data_array[y_size + y_size - 2]) * 0.5;
	color_output[0].Green = (data_array[1] + data_array[y_size]) * 0.5;
	color_output[temp1].Green = (data_array[temp1] + data_array[temp2 + 1]) * 0.5;
	color_output[temp1 + y_size - 1].Green = (data_array[temp1 + y_size - 2] + data_array[temp1 - 1]) * 0.5;
	color_output[y_size - 1].Blue = data_array[y_size + y_size - 1];
	color_output[0].Blue = data_array[y_size + 1];
	color_output[temp1].Blue = data_array[temp1 + 1];
	color_output[temp1 + y_size - 1].Blue = data_array[temp1 + y_size - 1];
}

void ISP::BlueCorrection_V00_00_00(int* BGR_input, int* BGR_output, int* tableB, int total_pixel) {
	ColorBGR* color_input = (ColorBGR*)BGR_input;
	ColorBGR* color_output = (ColorBGR*)BGR_output;
	int temp1, temp2;
	for (int i = 0; i < total_pixel; i++) {
		color_output[i] = color_input[i];
		temp1 = color_output[i].Blue;
		temp2 = tableB[temp1];
		color_output[i].Blue = temp2;
	}
}

void ISP::GammaCorrection_V00_00_00(int* BGR_input, int* BGR_output, int* table, int total_pixel) {
	ColorBGR* color_input = (ColorBGR*)BGR_input;
	ColorBGR* color_output = (ColorBGR*)BGR_output;
	int temp1, temp2;
	for (int i = 0; i < total_pixel; i++) {
		color_output[i] = color_input[i];
		temp1 = color_output[i].Blue;
		temp2 = table[temp1];
		color_output[i].Blue = temp2;
		temp1 = color_output[i].Green;
		temp2 = table[temp1];
		color_output[i].Green = temp2;
		temp1 = color_output[i].Red;
		temp2 = table[temp1];
		color_output[i].Red = temp2;
	}
}

void ISP::ColorCorrectionMatrix_V00_00_00(int* BGR_input, int* BGR_output, int total_pixel, double C01, double C02, double C03, double C11, double C12, double C13, double C21, double C22, double C23) {
	ColorBGR* color_input = (ColorBGR*)BGR_input;
	ColorBGR* color_output = (ColorBGR*)BGR_output;
	int temp;
	for (int i = 0; i < total_pixel; i++)
	{
		color_output[i] = color_input[i];
		temp = color_output[i].Red * C01 + color_output[i].Green * C02 + color_output[i].Blue * C03;
		if (temp > 1023) temp = 1023;
		else if (temp < 0) temp = 0;
		color_output[i].Red = temp;
		temp = color_output[i].Red * C11 + color_output[i].Green * C12 + color_output[i].Blue * C13;
		if (temp > 1023) temp = 1023;
		else if (temp < 0) temp = 0;
		color_output[i].Green = temp;
		temp = color_output[i].Red * C21 + color_output[i].Green * C22 + color_output[i].Blue * C23;
		if (temp > 1023) temp = 1023;
		else if (temp < 0) temp = 0;
		color_output[i].Blue = temp;
	}
}