#include "stdafx.h"
#include "Converter.h"

using namespace PG_ISP;

/*
 * REFERENCE:
 * HIGH-QUALITY LINEAR INTERPOLATION FOR DEMOSAICING OF BAYER-PATTERNED COLOR IMAGES
 * https://www.researchgate.net/publication/4087683_High-quality_linear_interpolation_for_demosaicing_of_Bayer-patterned_color_images
 */

bool IsOutOfEdge(const int start_addr, const int array_len, const int* ptr)
{
	const int size = sizeof(ptr);
	const int end_addr = start_addr + ((array_len - 1) * size);
	const int addr = (int)ptr;

	if (addr < start_addr) return true;
	else if (addr > end_addr) return true;
	else return false;
}

// |   1   |   2   |   3    |   4   |   5   |
// | :---: | :---: | :----: | :---: | :---: |
// |       |       |  R/B1  |       |       |
// |       | B/R1  |        | B/R2  |       |
// | R/B2  |       | center |       | R/B3  |
// |       | B/R3  |        | B/R4  |       |
// |       |       |  R/B4  |       |       |
int CatC(const int* center, const int width, const int start_addr, const int array_len)
{
	const int tmp = 0;

	int value = (center[0] * 6);

	int r3 = !IsOutOfEdge(start_addr, array_len, (center + 2)) ? (center + 2)[0] : tmp;
	int r4 = !IsOutOfEdge(start_addr, array_len, (center + (width << 1))) ? (center + (width << 1))[0] : tmp;
	int r1 = !IsOutOfEdge(start_addr, array_len, (center - (width << 1))) ? (center - (width << 1))[0] : tmp;
	int r2 = !IsOutOfEdge(start_addr, array_len, (center - 2)) ? (center - 2)[0] : tmp;

	int b4 = !IsOutOfEdge(start_addr, array_len, (center + (width + 1))) ? (center + (width + 1))[0] : tmp;
	int b2 = !IsOutOfEdge(start_addr, array_len, (center - width + 1)) ? (center - width + 1)[0] : tmp;
	int b3 = !IsOutOfEdge(start_addr, array_len, (center + (width - 1))) ? (center + (width - 1))[0] : tmp;
	int b1 = !IsOutOfEdge(start_addr, array_len, (center - width - 1)) ? (center - width - 1)[0] : tmp;

	value = value + ((b1 + b2 + b3 + b4) << 1) - (3 * (r1 + r2 + r3 + r4) >> 1);
	value = value >> 3;

	return value;
}
// |   1   |   2   |   3    |   4   |   5   |
// | :---: | :---: | :----: | :---: | :---: |
// |       |       |   c1   |       |       |
// |       |       |   g1   |       |       |
// |  c2   |  g2   | center |  g3   |  c3   |
// |       |       |   g4   |       |       |
// |       |       |   c4   |       |       |
int GatC(const int* center, const int width, const int start_addr, const int array_len)
{
	const int tmp = 0;

	int value = center[0] * 4;

	int g3 = !IsOutOfEdge(start_addr, array_len, (center + 1)) ? (center + 1)[0] : tmp;
	int g4 = !IsOutOfEdge(start_addr, array_len, (center + width)) ? (center + width)[0] : tmp;
	int g2 = !IsOutOfEdge(start_addr, array_len, (center - 1)) ? (center - 1)[0] : tmp;
	int g1 = !IsOutOfEdge(start_addr, array_len, (center - width)) ? (center - width)[0] : tmp;

	int c3 = !IsOutOfEdge(start_addr, array_len, (center + 2)) ? (center + 2)[0] : tmp;
	int c4 = !IsOutOfEdge(start_addr, array_len, (center + (width << 1))) ? (center + (width << 1))[0] : tmp;
	int c2 = !IsOutOfEdge(start_addr, array_len, (center - 2)) ? (center - 2)[0] : tmp;
	int c1 = !IsOutOfEdge(start_addr, array_len, (center - (width << 1))) ? (center - (width << 1))[0] : tmp;

	value = value + ((g1 + g2 + g3 + g4) << 1) - (c1 + c2 + c3 + c4);
	value = value >> 3;

	return value;
}
// |   1   |   2   |   3    |   4   |   5   |
// | :---: | :---: | :----: | :---: | :---: |
// |       |       |  Gr1   |       |       |
// |       |  Gb1  |        |  Gb2  |       |
// |  Gb3  |  C1   | center |  C2   |  Gb4  |
// |       |  Gb5  |        |  Gb6  |       |
// |       |       |  Gr2   |       |       |
int CatGinCrow(const int* center, const int width, const int start_addr, const int array_len)
{
	const int tmp = 0;

	int value = (center[0] * 5);
	int gb4 = !IsOutOfEdge(start_addr, array_len, (center + 2)) ? (center + 2)[0] : tmp;
	int gb6 = !IsOutOfEdge(start_addr, array_len, (center + (width + 1))) ? (center + (width + 1))[0] : tmp;
	int gb5 = !IsOutOfEdge(start_addr, array_len, (center + (width - 1))) ? (center + (width - 1))[0] : tmp;
	int gb3 = !IsOutOfEdge(start_addr, array_len, (center - 2)) ? (center - 2)[0] : tmp;
	int gb2 = !IsOutOfEdge(start_addr, array_len, (center - width + 1)) ? (center - width + 1)[0] : tmp;
	int gb1 = !IsOutOfEdge(start_addr, array_len, (center - width - 1)) ? (center - width - 1)[0] : tmp;

	int c2 = !IsOutOfEdge(start_addr, array_len, (center + 1)) ? (center + 1)[0] : tmp;
	int c1 = !IsOutOfEdge(start_addr, array_len, (center - 1)) ? (center - 1)[0] : tmp;

	int gr2 = !IsOutOfEdge(start_addr, array_len, (center + (width << 1))) ? (center + (width << 1))[0] : tmp;
	int gr1 = !IsOutOfEdge(start_addr, array_len, (center - (width << 1))) ? (center - (width << 1))[0] : tmp;

	value = value - (gb1 + gb2 + gb3 + gb4 + gb5 + gb6) + ((gr1 + gr2) >> 1) + ((c1 + c2) << 2);
	value = value >> 3;

	return value;
}
// |   1   |   2   |   3    |   4   |   5   |
// | :---: | :---: | :----: | :---: | :---: |
// |       |       |  Gb3   |       |       |
// |       |  Gb1  |   C1   |  Gb2  |       |
// |  Gr1  |       | center |       |  Gr2  |
// |       |  Gb5  |   C2   |  Gb6  |       |
// |       |       |  Gb4   |       |       |
int CatGinCcol(const int* center, const int width, const int start_addr, const int array_len)
{
	const int tmp = 0;

	int value = center[0] * 5;

	int gr2 = !IsOutOfEdge(start_addr, array_len, (center + 2)) ? (center + 2)[0] : tmp;
	int gr1 = !IsOutOfEdge(start_addr, array_len, (center - 2)) ? (center - 2)[0] : tmp;

	int c2 = !IsOutOfEdge(start_addr, array_len, (center + width)) ? (center + width)[0] : tmp;
	int c1 = !IsOutOfEdge(start_addr, array_len, (center - width)) ? (center - width)[0] : tmp;

	int gb6 = !IsOutOfEdge(start_addr, array_len, (center + (width + 1))) ? (center + (width + 1))[0] : tmp;
	int gb4 = !IsOutOfEdge(start_addr, array_len, (center + (width << 1))) ? (center + (width << 1))[0] : tmp;
	int gb5 = !IsOutOfEdge(start_addr, array_len, (center + (width - 1))) ? (center + (width - 1))[0] : tmp;
	int gb2 = !IsOutOfEdge(start_addr, array_len, (center - width + 1)) ? (center - width + 1)[0] : tmp;
	int gb1 = !IsOutOfEdge(start_addr, array_len, (center - width - 1)) ? (center - width - 1)[0] : tmp;
	int gb3 = !IsOutOfEdge(start_addr, array_len, (center - (width << 1))) ? (center - (width << 1))[0] : tmp;

	value = value - (gb1 + gb2 + gb3 + gb4 + gb5 + gb6) + ((gr1 + gr2) >> 1) + ((c1 + c2) << 2);
	value = value >> 3;

	return value;
}

unsigned char TEN_BITS_TABLE[1024] = {
	0,1,1,2,3,3,4,4,5,5,6,6,7,7,8,8,9,9,10,10,10,11,11,12,12,13,13,13,14,14,15,15,16,16,16,17,17,17,18,18,19,19,19,20,20,21,21,21,22,22,22,23,23,23,24,24,25,25,25,26,26,26,27,27,27,28,28,28,29,29,29,30,30,30,31,31,31,32,32,32,33,33,33,34,34,34,35,35,35,36,36,36,37,37,37,38,38,38,39,39,39,40,40,40,41,41,41,42,42,42,42,43,43,43,44,44,44,45,45,45,46,46,46,46,47,47,47,48,48,48,49,49,49,50,50,50,50,51,51,51,52,52,52,52,53,53,53,54,54,54,55,55,55,55,56,56,56,57,57,57,57,58,58,58,59,59,59,60,60,60,60,61,61,61,62,62,62,62,63,63,63,63,64,64,64,65,65,65,65,66,66,66,67,67,67,67,68,68,68,69,69,69,69,70,70,70,70,71,71,71,72,72,72,72,73,73,73,73,74,74,74,75,75,75,75,76,76,76,76,77,77,77,78,78,78,78,79,79,79,79,80,80,80,81,81,81,81,82,82,82,82,83,83,83,83,84,84,84,84,85,85,85,86,86,86,86,87,87,87,87,88,88,88,88,89,89,89,89,90,90,90,90,91,91,91,92,92,92,92,93,93,93,93,94,94,94,94,95,95,95,95,96,96,96,96,97,97,97,97,98,98,98,98,99,99,99,99,100,100,100,100,101,101,101,101,102,102,102,102,103,103,103,103,104,104,104,104,105,105,105,105,106,106,106,106,107,107,107,107,108,108,108,108,109,109,109,109,110,110,110,110,111,111,111,111,112,112,112,112,113,113,113,113,114,114,114,114,115,115,115,115,116,116,116,116,117,117,117,117,118,118,118,118,118,119,119,119,119,120,120,120,120,121,121,121,121,122,122,122,122,123,123,123,123,124,124,124,124,125,125,125,125,125,126,126,126,126,127,127,127,127,128,128,128,128,129,129,129,129,130,130,130,130,130,131,131,131,131,132,132,132,132,133,133,133,133,134,134,134,134,134,135,135,135,135,136,136,136,136,137,137,137,137,138,138,138,138,138,139,139,139,139,140,140,140,140,141,141,141,141,141,142,142,142,142,143,143,143,143,144,144,144,144,144,145,145,145,145,146,146,146,146,147,147,147,147,147,148,148,148,148,149,149,149,149,150,150,150,150,150,151,151,151,151,152,152,152,152,152,153,153,153,153,154,154,154,154,155,155,155,155,155,156,156,156,156,157,157,157,157,157,158,158,158,158,159,159,159,159,159,160,160,160,160,161,161,161,161,162,162,162,162,162,163,163,163,163,164,164,164,164,164,165,165,165,165,166,166,166,166,166,167,167,167,167,168,168,168,168,168,169,169,169,169,170,170,170,170,170,171,171,171,171,172,172,172,172,172,173,173,173,173,174,174,174,174,174,175,175,175,175,175,176,176,176,176,177,177,177,177,177,178,178,178,178,179,179,179,179,179,180,180,180,180,181,181,181,181,181,182,182,182,182,182,183,183,183,183,184,184,184,184,184,185,185,185,185,186,186,186,186,186,187,187,187,187,187,188,188,188,188,189,189,189,189,189,190,190,190,190,190,191,191,191,191,192,192,192,192,192,193,193,193,193,193,194,194,194,194,195,195,195,195,195,196,196,196,196,196,197,197,197,197,198,198,198,198,198,199,199,199,199,199,200,200,200,200,201,201,201,201,201,202,202,202,202,202,203,203,203,203,204,204,204,204,204,205,205,205,205,205,206,206,206,206,206,207,207,207,207,208,208,208,208,208,209,209,209,209,209,210,210,210,210,210,211,211,211,211,212,212,212,212,212,213,213,213,213,213,214,214,214,214,214,215,215,215,215,215,216,216,216,216,217,217,217,217,217,218,218,218,218,218,219,219,219,219,219,220,220,220,220,220,221,221,221,221,222,222,222,222,222,223,223,223,223,223,224,224,224,224,224,225,225,225,225,225,226,226,226,226,226,227,227,227,227,228,228,228,228,228,229,229,229,229,229,230,230,230,230,230,231,231,231,231,231,232,232,232,232,232,233,233,233,233,233,234,234,234,234,234,235,235,235,235,236,236,236,236,236,237,237,237,237,237,238,238,238,238,238,239,239,239,239,239,240,240,240,240,240,241,241,241,241,241,242,242,242,242,242,243,243,243,243,243,244,244,244,244,244,245,245,245,245,245,246,246,246,246,246,247,247,247,247,247,248,248,248,248,248,249,249,249,249,249,250,250,250,250,250,251,251,251,251,251,252,252,252,252,252,253,253,253,253,253,254,254,254,254,254,255,255,255,255 };

int ValueLimit(int value, const unsigned char pixel_bit)
{
	switch (pixel_bit)
	{
	case 8:
		if (value < 0) return 0;
		else if (value > 255) return 255;
		else return value;
	case 10:
		if (value < 0) value = 0;
		else if (value > 1023) value = 1023;
		return value;
	default:
		return value;
	}
}

unsigned char Converter::Int2Byte(int value, const unsigned char pixel_bit)
{
	switch (pixel_bit)
	{
	case 8:
		return ValueLimit(value, pixel_bit);
	case 10:
		value = ValueLimit(value, pixel_bit);
		return TEN_BITS_TABLE[value];
	default:
		return value;
	}
}

void Converter::BayerToBgr(const int* bayer_data, const int img_w, const int img_h, int* bgr_data, const BayerFilter filter, const unsigned char pixel_bit)
{
	const int bayerStep = img_w;
	const int rgbStep = img_w * 3;

	bool redRow = filter == PG_ISP::Converter::BayerFilter::RGGB ||
		filter == PG_ISP::Converter::BayerFilter::GRBG;

	bool greenStart = filter == PG_ISP::Converter::BayerFilter::GRBG ||
		filter == PG_ISP::Converter::BayerFilter::GBRG;

	const int startAddr = (int)bayer_data;
	const int length = img_w * img_h;

	unsigned long i, j;
	unsigned long start = 0;
	unsigned long index = 0;
	int r, g, b, tmp1, tmp2;
	for (j = 0; j < img_h; ++j)
	{
		start = 0;
		index = 0;
		if (greenStart)
		{
			/* green position */
			g = bayer_data[0];
			tmp1 = CatGinCrow(bayer_data, img_w, startAddr, length);
			tmp2 = CatGinCcol(bayer_data, img_w, startAddr, length);
			r = redRow ? tmp1 : tmp2;
			b = redRow ? tmp2 : tmp1;
			/*bgr_data[0] = Int2Byte(b, pixel_bit);
			bgr_data[1] = Int2Byte(g, pixel_bit);
			bgr_data[2] = Int2Byte(r, pixel_bit);*/
			bgr_data[0] = ValueLimit(b, pixel_bit);
			bgr_data[1] = ValueLimit(g, pixel_bit);
			bgr_data[2] = ValueLimit(r, pixel_bit);
			bayer_data++;
			bgr_data += 3;
			start++;
		}

		for (i = start; i < img_w; ++i)
		{
			if ((index & 0b1) == 0)
			{
				if (redRow)
				{
					r = bayer_data[0];
					g = GatC(bayer_data, img_w, startAddr, length);
					b = CatC(bayer_data, img_w, startAddr, length);
				}
				else
				{
					r = CatC(bayer_data, img_w, startAddr, length);
					g = GatC(bayer_data, img_w, startAddr, length);
					b = bayer_data[0];
				}
			}
			else
			{
				/* green position */
				g = bayer_data[0];
				tmp1 = CatGinCrow(bayer_data, img_w, startAddr, length);
				tmp2 = CatGinCcol(bayer_data, img_w, startAddr, length);
				r = redRow ? tmp1 : tmp2;
				b = redRow ? tmp2 : tmp1;
			}
			/*bgr_data[0] = Int2Byte(b, pixel_bit);
			bgr_data[1] = Int2Byte(g, pixel_bit);
			bgr_data[2] = Int2Byte(r, pixel_bit);*/
			bgr_data[0] = ValueLimit(b, pixel_bit);
			bgr_data[1] = ValueLimit(g, pixel_bit);
			bgr_data[2] = ValueLimit(r, pixel_bit);
			bayer_data++;
			bgr_data += 3;
			index++;
		}

		redRow = !redRow;
		greenStart = !greenStart;
	}
}
