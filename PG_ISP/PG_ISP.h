#pragma once
#include <cstdint>

#ifndef PG_ISP_H
#define PG_ISP_H

using namespace System;

namespace PG_ISP
{
	public enum ISPERR
	{
		SUCCESS = 0,
		INVALID_ARGUMENT = -1,
		UNKNOWN = -2,
		FPN_TABLE_ID_ERROR = -3,
		FPN_TABLE_ERROR = -4,
		UNSUPPORTED_GAIN = -5,
	};

	public ref class ISP
	{
	public:
		static uint16_t DownSampling(
			uint16_t nImageW, 
			uint16_t nImageH, 
			uint16_t nDstWidth,
			uint16_t nDstHeight,
			uint8_t* pImage);

		static BOOL DeadPixelCorrection (
			unsigned char* src_image,
			unsigned int width,
			unsigned int height,
			unsigned int threshold_floor,
			unsigned int threshold_ceiling,
			unsigned char* dst_image,
			unsigned char* dead_pixel_image);

		static bool fpn_collect (
			unsigned char* frame,
			int frame_size);

		static bool fpn_collect (unsigned char* frmae,
			int frame_size,
			int num,
			double* dst);

		static ISPERR FpnCorrection (
			unsigned char *frame_in,
			char *frame_out,
			double *table,
			unsigned depth,
			unsigned int width,
			unsigned int height);

		static ISPERR FpnCorrection (
			unsigned char *frame_in,
			unsigned int frame_width,
			unsigned int frame_height,
			double *table,
			unsigned int table_width,
			unsigned int table_height,
			unsigned depth,
			unsigned header_size);

		static void ExpandTable (
			double *tbl,
			double *table,
			unsigned int table_width,
			unsigned int table_height,
			unsigned int frame_width,
			unsigned int frame_height);

		static void HomogeneousBlur(
			int *src, 
			int *dst,
			unsigned int width, 
			unsigned int height, 
			unsigned int blurLevel);

		static void GaussianBlur(
			int*src,
			int*dst,
			unsigned int width, 
			unsigned int height, 
			unsigned int blurLevel,
			double standardDeviation);

		static void MedianBlur(
			int*src,
			int*dst,
			unsigned int width,
			unsigned int height,
			unsigned int blurLevel);

		static void OfpsDpc_3_3(
			unsigned char* src_image,
			int width,
			int height,
			unsigned char* ofpsdpc_image,
			double FWC,
			int gain,
			double factor);

		static void OfpsDpc_5_5(
			unsigned char* src_image,
			int width,
			int height,
			unsigned char* ofpsdpc_image,
			double FWC,
			int gain,
			double factor);

		static void OfpsDpc_7_7(
			unsigned char* src_image,
			int width,
			int height,
			unsigned char* ofpsdpc_image,
			double FWC,
			int gain,
			double factor);

		static void OfpsDpc_3_3(
			int* src_image,
			int width,
			int height,
			int* ofpsdpc_image,
			double FWC,
			int gain,
			double factor);

		static void OfpsDpc_5_5(
			int* src_image,
			int width,
			int height,
			int* ofpsdpc_image,
			double FWC,
			int gain,
			double factor);

		static void OfpsDpc_7_7(
			int* src_image,
			int width,
			int height,
			int* ofpsdpc_image,
			double FWC,
			int gain,
			double factor);

		static void OfpsMoire(
			unsigned char* src_image,
			int width,
			int height,
			unsigned char* ofpsmoire_image,
			double bkg_offset,
			unsigned int p_filter_size,
			unsigned int m_Fflter_size,
			unsigned int f_filter_size,
			int mode);

		static void OfpsMoire(
			int* src_image,
			int width,
			int height,
			unsigned char* ofpsmoire_image,
			double bkg_offset,
			unsigned int p_filter_size,
			unsigned int m_Fflter_size,
			unsigned int f_filter_size,
			int mode);

		static void OfpsNormalization(
			double* src_image,
			unsigned int width,
			unsigned int height,
			double* ofpsIgn_image,
			double eef,
			int offset,
			int FWC,
			int Gain,
			double dist_factor,
			int std_factor);

		static double OfpsIgn(int i);

		static void MeanBlur(
			int*src,
			int*dst,
			unsigned int width,
			unsigned int height,
			unsigned int blurLevel);

		static void Normalization(
			uint16_t* src, 
			uint8_t* outputImage, 
			uint16_t width,
			uint16_t height);

		static void GaussianFilter(
			uint16_t* src, 
			uint16_t* outputImage, 
			uint16_t width, 
			uint16_t height);

		static void BLC_Wash_Ver00_00_00(
			int* data_array,
			int width,
			int height);

		static void LensShadingCorrection_V00_00_00(			
			int* data_input,
			int* data_output,
			int total_pixel,
			int* LSC_array,
			int LSC_Level);

		static void WhiteBalance(
			int* data_input, int* data_output, int width, int height,
			float ch1, float ch2, float ch3, float ch4);

		static void Demosaic_V00_00_00(
			int* data_array,
			int y_size,
			int x_size,
			int* BGR_output);

		static void BlueCorrection_V00_00_00(
			int* BGR_input,
			int* BGR_output,
			int* tableB,
			int total_pixel); //Gamma Correctiontable

		static void GammaCorrection_V00_00_00(
			int* BGR_input,
			int* BGR_output,
			int* table,
			int total_pixel);

		static void ColorCorrectionMatrix_V00_00_00(
			int* BGR_input,
			int* BGR_output,
			int total_pixel,
			double C01,
			double C02,
			double C03,
			double C11,
			double C12,
			double C13,
			double C21,
			double C22,
			double C23);
	};
}

#endif