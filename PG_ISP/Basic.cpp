#include "stdafx.h"
#include "Basic.h"
#include <emmintrin.h>;

typedef unsigned char TUInt8;

typedef struct TARGB32
{
	TUInt8 B, G, R, A;
};

typedef struct TPicRegion
{
	TARGB32* pdata;
	long     stride;
	long     width;
	long     height;
};

#pragma managed(push,off)
/*
 * TO DO: there's some bug need to fix
 */
void zooming_sse(const TPicRegion& Src, const TPicRegion& Dst)
{
	if ((0 == Dst.width) || (0 == Dst.height)
		|| (0 == Src.width) || (0 == Src.height)) return;
	unsigned long xrIntFloat_16 = (Src.width << 16) / Dst.width + 1;
	unsigned long yrIntFloat_16 = (Src.height << 16) / Dst.height + 1;
	unsigned long dst_width = Dst.width;
	TARGB32* pDstLine = Dst.pdata;
	unsigned long srcy_16 = 0;
	unsigned long for4count = dst_width / 4 * 4;
	for (unsigned long y = 0; y < Dst.height; ++y)
	{
		TARGB32* pSrcLine = ((TARGB32*)((TUInt8*)Src.pdata + Src.stride * (srcy_16 >> 16)));
		unsigned long srcx_16 = 0;
		unsigned long x;
		for (x = 0; x < for4count; x += 4)
		{
			__m128i m0 = _mm_cvtsi32_si128(*(int*)(&pSrcLine[srcx_16 >> 16]));
			srcx_16 += xrIntFloat_16;
			m0 = _mm_unpacklo_epi32(m0, _mm_cvtsi32_si128(*(int*)(&pSrcLine[srcx_16 >> 16])));
			srcx_16 += xrIntFloat_16;
			__m128i m1 = _mm_cvtsi32_si128(*(int*)(&pSrcLine[srcx_16 >> 16]));
			srcx_16 += xrIntFloat_16;
			m1 = _mm_unpacklo_epi32(m0, _mm_cvtsi32_si128(*(int*)(&pSrcLine[srcx_16 >> 16])));
			srcx_16 += xrIntFloat_16;
			_mm_stream_si128((__m128i*) & pDstLine[x], _mm_unpacklo_epi64(m0, m1));
		}
		for (x = for4count; x < dst_width; ++x)
		{
			pDstLine[x] = pSrcLine[srcx_16 >> 16];
			srcx_16 += xrIntFloat_16;
		}
		srcy_16 += yrIntFloat_16;
		((TUInt8*&)pDstLine) += Dst.stride;
	}
}
#pragma managed(pop)

void zooming(const TPicRegion& Src, const TPicRegion& Dst)
{
	if ((0 == Dst.width) || (0 == Dst.height)
		|| (0 == Src.width) || (0 == Src.height)) return;
	unsigned long xrIntFloat_16 = (Src.width << 16) / Dst.width + 1;
	unsigned long yrIntFloat_16 = (Src.height << 16) / Dst.height + 1;
	unsigned long dst_width = Dst.width;
	TARGB32* pDstLine = Dst.pdata;
	unsigned long srcy_16 = 0;
	for (unsigned long y = 0; y < Dst.height; ++y)
	{
		TARGB32* pSrcLine = ((TARGB32*)((TUInt8*)Src.pdata + Src.stride * (srcy_16 >> 16)));
		unsigned long srcx_16 = 0;
		for (unsigned long x = 0; x < dst_width; ++x)
		{
			pDstLine[x] = pSrcLine[srcx_16 >> 16];
			srcx_16 += xrIntFloat_16;
		}
		srcy_16 += yrIntFloat_16;
		((TUInt8*&)pDstLine) += Dst.stride;
	}
}

void PG_ISP::Basic::Raw10ToTenBit(unsigned char* src, int width, int height, int* dst)
{
	const unsigned int size = width * height;
	const unsigned long for4count = size & 0xFFFFFFFC;
	unsigned long mod = 0;
	unsigned long divide = 0;
	unsigned long counter = 0;

	for (unsigned long idx = 0; idx < for4count; idx += 4)
	{
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | ((src[counter + divide + 4] >> 6) & 0b11);
		++counter;
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | ((src[counter + divide + 3] >> 4) & 0b11);
		++counter;
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | ((src[counter + divide + 2] >> 2) & 0b11);
		++counter;
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | (src[counter + divide + 1] & 0b11);
		++counter;
	}

	for (unsigned long idx = for4count; idx < size; ++idx)
	{
		mod = idx & 3;
		divide = idx >> 2;
		if (mod == 0)
		{
			dst[idx] = (src[idx + divide] << 2) | ((src[idx + divide + 4] >> 6) & 0b11);
		}
		else if (mod == 1)
		{
			dst[idx] = (src[idx + divide] << 2) | ((src[idx + divide + 3] >> 4) & 0b11);
		}
		else if (mod == 2)
		{
			dst[idx] = (src[idx + divide] << 2) | ((src[idx + divide + 2] >> 2) & 0b11);
		}
		else
		{
			dst[idx] = (src[idx + divide] << 2) | (src[idx + divide + 1] & 0b11);
		}
	}
}

void PG_ISP::Basic::TenBitToRaw10(int* src, int width, int height, unsigned char* dst)
{
	int size = width * height;
	for (unsigned long idx = 0; idx < size; ++idx)
	{
		dst[idx + (idx >> 2)] = (src[idx] >> 2) & 0xff;
		if ((idx & 3) == 0 && idx != 0)
		{
			dst[idx + (idx >> 2) - 1] = ((src[idx - 4] & 0b11) << 6) | ((src[idx - 3] & 0b11) << 4) | ((src[idx - 2] & 0b11) << 2) | ((src[idx - 1] & 0b11) << 0);
		}
	}
}

void PG_ISP::Basic::MipiRaw10ToTenBit(unsigned char* src, int width, int height, int* dst)
{
	const unsigned int size = width * height;
	const unsigned long for4count = size & 0xFFFFFFFC;
	unsigned long mod = 0;
	unsigned long divide = 0;
	unsigned long counter = 0;

	for (unsigned long idx = 0; idx < for4count; idx += 4)
	{
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | ((src[counter + divide + 4] >> 0) & 0b11);
		++counter;
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | ((src[counter + divide + 3] >> 2) & 0b11);
		++counter;
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | ((src[counter + divide + 2] >> 4) & 0b11);
		++counter;
		divide = counter >> 2;
		dst[counter] = (src[counter + divide] << 2) | ((src[counter + divide + 1] >> 6) & 0b11);
		++counter;
	}

	for (unsigned long idx = for4count; idx < size; ++idx)
	{
		mod = idx & 3;
		divide = idx >> 2;
		if (mod == 0)
		{
			dst[idx] = (src[idx + divide] << 2) | ((src[idx + divide + 4] >> 0) & 0b11);
		}
		else if (mod == 1)
		{
			dst[idx] = (src[idx + divide] << 2) | ((src[idx + divide + 3] >> 2) & 0b11);
		}
		else if (mod == 2)
		{
			dst[idx] = (src[idx + divide] << 2) | ((src[idx + divide + 2] >> 4) & 0b11);
		}
		else
		{
			dst[idx] = (src[idx + divide] << 2) | ((src[idx + divide + 1] >> 6) & 0b11);
		}
	}
}

void PG_ISP::Basic::TenBitToMipiRaw10(int* src, int width, int height, unsigned char* dst)
{
	int size = width * height;
	for (unsigned long idx = 0; idx < size; ++idx)
	{
		dst[idx + (idx >> 2)] = (src[idx] >> 2) & 0xff;
		if ((idx & 3) == 0 && idx != 0)
		{
			dst[idx + (idx >> 2) - 1] = ((src[idx - 4] & 0b11) << 0) | ((src[idx - 3] & 0b11) << 2) | ((src[idx - 2] & 0b11) << 4) | ((src[idx - 1] & 0b11) << 6);
		}
	}
}

void PG_ISP::Basic::ZoomingArgb(unsigned char* src, long src_width, long src_height, long src_stride, unsigned char* dst, long dst_width, long dst_height, long dst_stride)
{
	TPicRegion srcPic;
	TPicRegion dstPic;
	srcPic.width = src_width;
	srcPic.height = src_height;
	srcPic.stride = src_stride;
	srcPic.pdata = (TARGB32*)src;

	int size = dst_stride * dst_height;

	dstPic.width = dst_width;
	dstPic.height = dst_height;
	dstPic.stride = dst_stride;
	dstPic.pdata = (TARGB32*)dst;

#if 1
	zooming(srcPic, dstPic);
#else
	zooming_sse(srcPic, dstPic);
#endif
}
