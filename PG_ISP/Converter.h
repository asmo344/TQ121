#pragma once
namespace PG_ISP 
{

	public ref class Converter
	{
	public:
		enum class BayerFilter { RGGB, GBRG, GRBG, BGGR, };
		static unsigned char Int2Byte(int value, const unsigned char pixel_bit);
		static void BayerToBgr(
			const int* bayer_data, const int img_w, const int img_h,
			int* bgr_data,
			const BayerFilter filter,
			const unsigned char pixel_bit);
	private:
	};
}