#pragma once
namespace PG_ISP
{
	public ref class Basic
	{
	public:
		static void Raw10ToTenBit(unsigned char* src, int width, int height, int* dst);		
		static void TenBitToRaw10(int* src, int width, int height, unsigned char* dst);
		static void MipiRaw10ToTenBit(unsigned char* src, int width, int height, int* dst);
		static void TenBitToMipiRaw10(int* src, int width, int height, unsigned char* dst);
		static void ZoomingArgb(
			unsigned char* src, long src_width, long src_height, long src_stride,
			unsigned char* dst, long dst_width, long dst_height, long dst_stride);
	private:

	};
}