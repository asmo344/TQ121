#ifndef __IMAGEKIT_H__
#define __IMAGEKIT_H__

/****************************************************************************************
 *
 * imagekit系列各种硬件版本定义
 *
 ****************************************************************************************/

#define VER_HS100 0x1000
#define VER_HS200 0x1020

#define VER_HS120 0x1020
#define VER_HS128 0x1200
#define VER_HS230 0x1230

#define VER_HS280 0x1280

#define VER_HS130 0x1300
#define VER_HS300 0x1330
#define VER_HS320 0x1332

#define VER_HV810 0x1380
#define VER_HV820 0x1382
#define VER_HV910 0x1390
#define VER_HV920 0x1392

/****************************************************************************************
 *
 * SENSOR的初始化与控制相关
 *
 ****************************************************************************************/

typedef struct _SensorTab
{
	/// @brief SENSOR宽度
	USHORT width; ///< SENSOR宽度
	/// @brief SENSOR高度
	USHORT height; ///< SENSOR高度
	/// @brief SENSOR数据类型
	BYTE type; ///< SENSOR数据类型
	/// @brief SENSOR的RESET和PWDN引脚设置
	BYTE pin; ///< SENSOR的RESET和PWDN引脚设置
	/// @brief SENSOR的器件地址
	BYTE SlaveID; ///< SENSOR的器件地址
	/// @brief SENSOR的I2C模式
	BYTE mode; ///< SENSOR的I2C模式
	/// @brief SENSOR标志寄存器1.
	USHORT FlagReg; ///< SENSOR标志寄存器1.
	/// @brief SENSOR标志寄存器1的值
	USHORT FlagData; ///< SENSOR标志寄存器1的值
	/// @brief SENSOR标志寄存器1的掩码值
	USHORT FlagMask; ///< SENSOR标志寄存器1的掩码值
	/// @brief SENSOR标志寄存器2.
	USHORT FlagReg1; ///< SENSOR标志寄存器2.
	/// @brief SENSOR标志寄存器2的值
	USHORT FlagData1; ///< SENSOR标志寄存器2的值
	/// @brief SENSOR标志寄存器2的掩码值
	USHORT FlagMask1; ///< SENSOR标志寄存器2的掩码值
	/// @brief SENSOR的名称
	char name[64]; ///< SENSOR的名称

	/// @brief 初始化SENSOR数据表
	USHORT *ParaList; ///< 初始化SENSOR数据表
	/// @brief 初始化SENSOR数据表大小，单位字节
	USHORT ParaListSize; ///< 初始化SENSOR数据表大小，单位字节

	/// @brief SENSOR进入Sleep模式的参数表
	USHORT *SleepParaList; ///< SENSOR进入Sleep模式的参数表
	/// @brief SENSOR进入Sleep模式的参数表大小，单位字节
	USHORT SleepParaListSize; ///< SENSOR进入Sleep模式的参数表大小，单位字节

	/// @brief SENSOR输出数据格式，YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	BYTE outformat; ///< SENSOR输出数据格式，YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	/// @brief SENSOR的输入时钟MCLK，0:12M; 1:24M; 2:48M.
	UINT mclk; ///< SENSOR的输入时钟MCLK，0:12M; 1:24M; 2:48M.
	/// @brief SENSOR的AVDD电压值
	UINT avdd; ///< SENSOR的AVDD电压值
	/// @brief SENSOR的AVDD2电压值
	UINT avdd2; ///< SENSOR的AVDD2电压值
	/// @brief SENSOR的DVDD电压值
	UINT dvdd; ///< SENSOR的DVDD电压值
	/// @brief SENSOR的DOVDD电压值
	UINT dovdd; ///< SENSOR的DOVDD电压值
	/// @brief SENSOR的AFVCC电压值
	UINT afvcc; ///< SENSOR的AFVCC电压值
	/// @brief SENSOR的VPP2电压值
	UINT vpp2; ///< SENSOR的VPP2电压值
	/// @brief SENSOR的OISVDD电压值
	UINT oisvdd; ///< SENSOR的OISVDD电压值

	/// @brief SENSOR的数据接口类型
	BYTE port; ///< SENSOR的数据接口类型
	USHORT Ext0;
	USHORT Ext1;
	USHORT Ext2;

	/// @brief AF初始化参数表
	USHORT *AF_InitParaList; ///< AF初始化参数表
	/// @brief AF初始化参数表大小，单位字节
	USHORT AF_InitParaListSize; ///< AF初始化参数表大小，单位字节

	/// @brief AF_AUTO参数表
	USHORT *AF_AutoParaList; ///< AF_AUTO参数表
	/// @brief AF_AUTO参数表大小，单位字节
	USHORT AF_AutoParaListSize; ///< AF_AUTO参数表大小，单位字节

	/// @brief AF_FAR参数表
	USHORT *AF_FarParaList; ///< AF_FAR参数表
	/// @brief AF_FAR参数表大小，单位字节
	USHORT AF_FarParaListSize; ///< AF_FAR参数表大小，单位字节

	/// @brief AF_NEAR参数表
	USHORT *AF_NearParaList; ///< AF_NEAR参数表
	/// @brief AF_NEAR参数表大小，单位字节
	USHORT AF_NearParaListSize; ///< AF_NEAR参数表大小，单位字节

	/// @brief 曝光参数表
	USHORT *Exposure_ParaList; ///< 曝光参数表
	/// @brief 曝光参数表大小，单位字节
	USHORT Exposure_ParaListSize; ///< 曝光参数表大小，单位字节

	/// @brief 增益参数表
	USHORT *Gain_ParaList; ///< 增益参数表
	/// @brief 增益参数表大小，单位字节
	USHORT Gain_ParaListSize; ///< 增益参数表大小，单位字节

	_SensorTab()
	{
		width = 0;
		height = 0;
		type = 0;
		pin = 0;
		SlaveID = 0;
		mode = 0;
		FlagReg = 0;
		FlagData = 0;
		FlagMask = 0;
		FlagReg1 = 0;
		FlagData1 = 0;
		FlagMask1 = 0;
		memset(name, 0, sizeof(name));

		ParaList = NULL;
		ParaListSize = 0;
		SleepParaList = NULL;
		SleepParaListSize = 0;

		outformat = 0;
		mclk = 0; // 0:12M; 1:24M; 2:48M.
		avdd = 0; //
		avdd2 = 0;
		dvdd = 0;
		dovdd = 0; //
		afvcc = 0;
		vpp2 = 0;
		oisvdd = 0;

		port = 0;
		Ext0 = 0;
		Ext1 = 0;
		Ext2 = 0;

		AF_InitParaList = NULL; // AF_InitParaList
		AF_InitParaListSize = 0;

		AF_AutoParaList = NULL;
		AF_AutoParaListSize = 0;

		AF_FarParaList = NULL;
		AF_FarParaListSize = 0;

		AF_NearParaList = NULL;
		AF_NearParaListSize = 0;

		Exposure_ParaList = NULL; // 曝光
		Exposure_ParaListSize = 0;

		Gain_ParaList = NULL; // 增益
		Gain_ParaListSize = 0;
	}
} SensorTab, *pSensorTab;

// 紧随SensorTab, 增加SensorTab2定义
///////////////////////////////////////////////////////////
typedef struct _SensorTab2
{
	/// @brief SENSOR宽度
	UINT width; ///< SENSOR宽度
	/// @brief SENSOR高度
	UINT height; ///< SENSOR高度

	UINT Quick_w; ///< Quick View 宽度
	UINT Quick_h; ///< Quick View 高度

	/// @brief SENSOR数据类型
	UINT type; ///< SENSOR数据类型
	/// @brief SENSOR的RESET和PWDN引脚设置
	UINT pin; ///< SENSOR的RESET和PWDN引脚设置
	/// @brief SENSOR的器件地址
	UINT SlaveID; ///< SENSOR的器件地址
	/// @brief SENSOR的I2C模式
	UINT mode; ///< SENSOR的I2C模式
	/// @brief SENSOR标志寄存器1.
	UINT FlagReg; ///< SENSOR标志寄存器1.
	/// @brief SENSOR标志寄存器1的值
	UINT FlagData; ///< SENSOR标志寄存器1的值
	/// @brief SENSOR标志寄存器1的掩码值
	UINT FlagMask; ///< SENSOR标志寄存器1的掩码值
	/// @brief SENSOR标志寄存器2.
	UINT FlagReg1; ///< SENSOR标志寄存器2.
	/// @brief SENSOR标志寄存器2的值
	UINT FlagData1; ///< SENSOR标志寄存器2的值
	/// @brief SENSOR标志寄存器2的掩码值
	UINT FlagMask1; ///< SENSOR标志寄存器2的掩码值
	/// @brief SENSOR的名称
	char name[64]; ///< SENSOR的名称

	/// @brief 初始化SENSOR数据表
	UINT *ParaList; ///< 初始化SENSOR数据表
	/// @brief 初始化SENSOR数据表大小，单位字节
	UINT ParaListSize; ///< 初始化SENSOR数据表大小，单位字节

	/// @brief SENSOR进入Sleep模式的参数表
	UINT *SleepParaList; ///< SENSOR进入Sleep模式的参数表
	/// @brief SENSOR进入Sleep模式的参数表大小，单位字节
	UINT SleepParaListSize; ///< SENSOR进入Sleep模式的参数表大小，单位字节

	/// @brief SENSOR进入Quick View模式的参数表
	UINT *QuickParaList; ///< SENSOR进入Quick View模式的参数表
	/// @brief SENSOR进入Quick View模式的参数表大小，单位字节
	UINT QuickParaListSize; ///< SENSOR进入Quick View模式的参数表大小，单位字节

	/// @brief SENSOR输出数据格式，YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	UINT outformat; ///< SENSOR输出数据格式，YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	/// @brief SENSOR的输入时钟MCLK，0:12M; 1:24M; 2:48M.
	UINT mclk; ///< SENSOR的输入时钟MCLK，0:12M; 1:24M; 2:48M.
	/// @brief SENSOR的AVDD电压值
	UINT avdd; ///< SENSOR的AVDD电压值
	/// @brief SENSOR的DOVDD电压值
	UINT dovdd; ///< SENSOR的DOVDD电压值
	/// @brief SENSOR的DVDD电压值
	UINT dvdd; ///< SENSOR的DVDD电压值

	/// @brief SENSOR的数据接口类型
	UINT port; ///< SENSOR的数据接口类型
	UINT Ext0;
	UINT Ext1;
	UINT Ext2;

	/// @brief AF初始化参数表
	UINT *AF_InitParaList; ///< AF初始化参数表
	/// @brief AF初始化参数表大小，单位字节
	UINT AF_InitParaListSize; ///< AF初始化参数表大小，单位字节

	/// @brief AF_AUTO参数表
	UINT *AF_AutoParaList; ///< AF_AUTO参数表
	/// @brief AF_AUTO参数表大小，单位字节
	UINT AF_AutoParaListSize; ///< AF_AUTO参数表大小，单位字节

	/// @brief AF_FAR参数表
	UINT *AF_FarParaList; ///< AF_FAR参数表
	/// @brief AF_FAR参数表大小，单位字节
	UINT AF_FarParaListSize; ///< AF_FAR参数表大小，单位字节

	/// @brief AF_NEAR参数表
	UINT *AF_NearParaList; ///< AF_NEAR参数表
	/// @brief AF_NEAR参数表大小，单位字节
	UINT AF_NearParaListSize; ///< AF_NEAR参数表大小，单位字节

	/// @brief 曝光参数表
	UINT *Exposure_ParaList; ///< 曝光参数表
	/// @brief 曝光参数表大小，单位字节
	UINT Exposure_ParaListSize; ///< 曝光参数表大小，单位字节

	/// @brief 增益参数表
	UINT *Gain_ParaList; ///< 增益参数表
	/// @brief 增益参数表大小，单位字节
	UINT Gain_ParaListSize; ///< 增益参数表大小，单位字节

	_SensorTab2()
	{
		width = 0;
		height = 0;
		Quick_w = 0; // 20141031
		Quick_h = 0; // 20141031
		type = 0;
		pin = 0;
		SlaveID = 0;
		mode = 0;
		FlagReg = 0;
		FlagData = 0;
		FlagMask = 0;
		FlagReg1 = 0;
		FlagData1 = 0;
		FlagMask1 = 0;
		memset(name, 0, sizeof(name));

		ParaList = NULL;
		ParaListSize = 0;
		SleepParaList = NULL;
		SleepParaListSize = 0;

		QuickParaList = NULL;  // 20141031
		QuickParaListSize = 0; // 20141031

		outformat = 0;
		mclk = 0;  // 0:12M; 1:24M; 2:48M.
		avdd = 0;  //
		dovdd = 0; //
		dvdd = 0;

		port = 0;
		Ext0 = 0;
		Ext1 = 0;
		Ext2 = 0;

		AF_InitParaList = NULL; // AF_InitParaList
		AF_InitParaListSize = 0;

		AF_AutoParaList = NULL;
		AF_AutoParaListSize = 0;

		AF_FarParaList = NULL;
		AF_FarParaListSize = 0;

		AF_NearParaList = NULL;
		AF_NearParaListSize = 0;

		Exposure_ParaList = NULL; // 曝光
		Exposure_ParaListSize = 0;

		Gain_ParaList = NULL; // 增益
		Gain_ParaListSize = 0;
	}
} SensorTab2, *pSensorTab2;

/** @defgroup group2 ISP相关
@{

*/

/** @name SENSOR输出图像类型定义(SensorTab::type的取值定义)
@{

*/
/* SENSOR输出图像类型定义(SensorTab::type的取值定义)， 建议使用对应的IMAGE_FORMAT枚举类型 */
#define D_RAW10 0x00
#define D_RAW8 0x01
#define D_MIPI_RAW8 0x01
#define D_YUV 0x02
#define D_RAW16 0x03
#define D_MIPI_RAW16 0x03
#define D_RGB565 0x04
#define D_YUV_SPI 0x05
#define D_MIPI_RAW10 0x06 ///< 5bytes = 4 pixel...
#define D_MIPI_RAW12 0x07 ///< 3bytes = 2 pixel...
#define D_RAW12 0x07
#define D_YUV_MTK_S 0x08 // MTK output...
#define D_YUV_10 0x09
#define D_YUV_12 0x0a

#define D_MIPI_RAW14 0x0b ///< 7bytes = 4 pixel...

#define D_BGR24 0x20 ///< 排列顺序为B，G，R，各8bit
#define D_BGR32 0x21 ///< 排列顺序为B，G，R，0各8bit
#define D_P10 0x24	 ///< 一个像素占两个字节，LSB，0～1023，一般用于MIPI_RAW10转换
#define D_P12 0x25	 ///< 一个像素占两个字节，LSB，0～4095，一般用于MIPI_RAW12转换

#define D_G8 0x28
#define D_G10 0x29
#define D_GRAY8 0x2a
// #define D_RGB24             0x0b
// #define D_HISPI_SP			0x09    //aptina hispi packet sp.
/** @} */

/** @name RAW转RGB算法定义
@{

*/
/* RAW转RGB算法定义 */
#define RAW2RGB_NORMAL 0
#define RAW2RGB_SMOOTH 1
#define RAW2RGB_SHARP 2
/** @} */

/** @name YUV图像4种输出格式定义
@{

*/
/* RAW、YUV图像4种输出格式定义(SensorTab::outformat的取值定义) */
/// YUV图像4种输出格式定义。
enum OUTFORMAT_YUV
{
	OUTFORMAT_YCbYCr = 0, ///< YCbYCr输出格式
	OUTFORMAT_YCrYCb,	  ///< YCrYCb输出格式
	OUTFORMAT_CbYCrY,	  ///< CbYCrY输出格式
	OUTFORMAT_CrYCbY,	  ///< CrYCbY输出格式
};
/** @} */

/** @name RAW图像4种输出格式定义
@{

*/
/// RAW图像4种输出格式定义。
enum OUTFORMAT_RGB
{
	OUTFORMAT_RGGB = 0, ///< RGGB输出格式
	OUTFORMAT_GRBG,		///< GRBG输出格式
	OUTFORMAT_GBRG,		///< GBRG输出格式
	OUTFORMAT_BGGR,		///< BGGR输出格式
};
/** @} */

/* 本系统支持的RAW格式、YUV格式定义 */
/** @name 支持的RAW格式定义
@{

*/
/// 支持的RAW格式。
enum RAW_FORMAT
{
	RAW_RGGB = 0, ///< RAW格式按RGGB排列
	RAW_GRBG,	  ///< RAW格式按GRBG排列
	RAW_GBRG,	  ///< RAW格式按GBRG排列
	RAW_BGGR,	  ///< RAW格式按BGGR排列
};
/** @} */

/** @name 支持的YUV格式定义
@{

*/
/// 支持的YUV格式。
enum YUV_FORMAT
{
	YUV_YCBYCR = 0, ///< YUV格式按YCBYCR排列
	YUV_YCRYCB,		///< YUV格式按YCRYCB排列
	YUV_CBYCRY,		///< YUV格式按CBYCRY排列
	YUV_CRYCBY,		///< YUV格式按CRYCBY排列
};
/** @} */

/** @name 图像处理模式选择
@{

*/
/// 图像处理模式选择
enum ISP_MODE
{
	NORMAL = 0,	 /// 普通的处理模式，抓帧得到的是raw数据
	S2DFAST,	 /// S2DFAST模式，抓帧得到的是RGB数据
	S2DFAST_GPU, /// S2DFAST_GPU模式，GPU进行图像处理，抓帧得到的是RGB数据
};

/** @} */

/** @name 图像格式定义
@{

*/
enum IMAGE_FORMAT
{
	FORMAT_RAW10 = 0x00,
	FORMAT_RAW8 = 0x01,
	FORMAT_YUV = 0x02,
	FORMAT_RAW16 = 0x03,
	FORMAT_RGB565 = 0x04,
	FORMAT_YUV_SPI = 0x05,
	FORMAT_MIPI_RAW10 = 0x06, ///< 5bytes = 4 pixel...
	FORMAT_MIPI_RAW12 = 0x07, ///< 3bytes = 2 pixel...
	FORMAT_YUV_MTK_S = 0x08,  ///< MTK output...
	FORMAT_YUV_10 = 0x09,
	FORMAT_YUV_12 = 0x0a,
	FORMAT_SAMSUNG_DVS = 0x010,

	FORMAT_BGR24 = 0x20, ///< 排列顺序为B，G，R，各8bit
	FORMAT_BGR32 = 0x21, ///< 排列顺序为B，G，R，0各8bit
	FORMAT_P10 = 0x24,	 ///< 一个像素占两个字节，LSB，0～1023，一般用于MIPI_RAW10转换
	FORMAT_P12 = 0x25,	 ///< 一个像素占两个字节，LSB，0～4095，一般用于MIPI_RAW12转换
	FORMAT_G8 = 0x28,	 ///< 只取G值，8bit
	FORMAT_G10 = 0x29,	 ///< 只取G值，16bit
	FORMAT_GRAY8 = 0x2a,
};
/** @} */

/** @brief ROI结构体描述
@{

*/
typedef struct DtRoi_s
{
	UINT x; ///< ROI起始点X坐标值
	UINT y; ///< ROI起始点Y坐标值
	UINT w; ///< ROI宽度
	UINT h; ///< ROI高度
} DtRoi_t;

/** @} */

/** @brief 图像数据结构体描述
@{

*/
typedef struct DtImage_s
{
	IMAGE_FORMAT format;   ///< 图像格式
	RAW_FORMAT rawFmt;	   ///< RAW格式细节
	YUV_FORMAT yuvFmt;	   ///< YUV格式细节
	UINT width;			   ///< 图像尺寸
	UINT height;		   ///< 图像尺寸
	BYTE *data;			   ///< 图像数据
	unsigned int dataSize; ///< buffer空间大小
	UINT resv[8];		   ///< 保留32字节
} DtImage_t;
/** @} */

/** @} */ // end of group2

/** @defgroup group3 SENSOR相关


* @{

*/

/** @name SENSOR寄存器参数表中附带的控制字定义
@{

*/
/* SENSOR寄存器参数表中附带的控制字定义 */

#define DTDELAY 0xffff
#define DTMODE 0xfffe
#define DTOR 0xfffd
#define DTAND 0xfffc
#define DTPOLLT 0xfffb
#define DTPOLL1 0xfffa
#define DTPOLL0 0xfff9
#define DTI2CADDR 0xfff8
#define DTI2CREG 0xfff7
#define DTAFTYPE 0xfff6 // 20121223 added... modify the AF Device type
#define DTAFADDR 0xfff5 // 20121223 added... modify the AF Device Address..
#define DTSPIMTKCTRL 0xfff4
#define DTEND 0xfeff
#define DTMACRO_ON 0xfef0
#define DTMACRO_OFF 0xfef1
#define DTSPIMTKCTRL 0xfff4

// XHS
// XVS
// V_START
// V_END
// H_START
// H_END
/** @} */

/** @name SENSOR电源电压选择定义（dtccm使用的定义）
@{

*/
/* SENSOR电源电压选择定义 */
#define AVDD_28 0x00
#define AVDD_25 0x01
#define AVDD_18 0x02
#define AVDD_DEFAULT 0x03

#define DOVDD_28 0x00
#define DOVDD_25 0x01
#define DOVDD_18 0x02
#define DOVDD_DEFAULT 0x03

#define DVDD_18 0x00
#define DVDD_15 0x01
#define DVDD_12 0x02
#define DVDD_DEFAULT 0x03

#define AFVCC_33 0x00
#define AFVCC_28 0x01
#define AFVCC_18 0x02
#define AFVCC_DEFAULT 0x03
/** @} */

/** @name SENSOR输入时钟选择定义
@{

*/
/* SENSOR输入时钟选择定义 */
// you can use these enum type ,or use MHZ or hundred KHZ directly
enum MCLKOUT
{
	MCLK_6M = 0,
	MCLK_8M,
	MCLK_10M,
	MCLK_11M4,
	MCLK_12M,
	MCLK_12M5,
	MCLK_13M5,
	MCLK_15M,
	MCLK_18M,
	MCLK_24M,
	MCLK_25M,
	MCLK_27M,
	MCLK_30M,
	MCLK_32M,
	MCLK_36M,
	MCLK_40M,
	MCLK_45M,
	MCLK_48M,
	MCLK_50M,
	MCLK_60M,
	MCLK_DEFAULT,
};
/** @} */

/** @name 多SENSOR模组通道定义(目前只在DTLC2中存在CHANNEL_B)
@{

*/
/* 多SENSOR模组通道定义(目前只在DTLC2/UH920中存在CHANNEL_B) **/
#define CHANNEL_A 0x01	/// 只使用A通道
#define CHANNEL_B 0x02	// 只是用B通道
#define CHANNEL_AB 0x03 // AB通道同时使用
/** @} */

/** @name SensorEnable函数中，使能SENSOR时，RESET/PWDN管脚的电平状态定义
@{

*/
/* SensorEnable函数中，使能SENSOR时，RESET/PWDN管脚的电平状态定义 */
#define RESET_H 0x02
#define RESET_L 0x00
#define PWDN_H 0x01
#define PWDN_L 0x00
#define PWDN2_H 0x04
#define PWDN2_L 0x00
/** @} */

/** @name 支持的SENSOR数据接口定义
@{

*/
/// 定义支持的SENSOR数据接口类型。
typedef enum
{
	SENSOR_PORT_MIPI = 0x00,			   ///< MIPI接口
	SENSOR_PORT_PARA = 0x01,			   ///< 并行同步接口
	SENSOR_PORT_MTK_SERIAL = 0x02,		   ///< MTK公司的串行接口
	SENSOR_PORT_SPI = 0x03,				   ///< SPI接口
	SENSOR_PORT_SIM = 0x04,				   ///< 模拟图像，用于测试
	SENSOR_PORT_HISPI = 0x05,			   ///< Aptina的HISPI接口,支持packet sp格式
	SENSOR_PORT_ZX_SERIAL = 0x06,		   ///< 展讯的串行接口
	SENSOR_PORT_SAMSUNG_DVS = 0x07,		   ///< 三星DVS
	SENSOR_PORT_HISILICON_LVDS = 0x08,	   ///< 海思LVDS
	SENSOR_PORT_SMARTSENS_DDR_4BIT = 0x09, ///< 思特微DDR 4bit

	SENSOR_PORT_SLVS_EC = 0x40, ///< sony slvs-ec

	SENSOR_PORT_SONY_LVDS = 0x81,	   ///< 索尼 LVDS
	SENSOR_PORT_SMARTSENS_LVDS = 0x82, ///< 思特威 LVDS
	SENSOR_PORT_PANASONIC_LVDS = 0x85, ///< 松下 LVDS
	SENSOR_PORT_SUPERPIX_LVDS = 0x86,  ///< 思比科 LVDS
} SENSOR_PORT;
/** @} */

/** @name 早期版本使用的宏定义
@{

*/
/* 早期版本使用的宏定义 */
#define PORT_MIPI 0		///< MIPI output
#define PORT_PARALLEL 1 ///< Parallel output
#define PORT_MTK 2		///< MTK output
#define PORT_SPI 3		///< SPI output
#define PORT_TEST 4		///< TEST ouput. FPGA output the image...
#define PORT_HISPI 5	///< aptina HISPI packet sp...
#define PORT_ZX2_4 6	///< zhanxun 2bit/4bit packet sp...
#define PORT_MAX 7		///< maxium... can't support >=PORT_MAX
/** @} */

#define PORT_SONY_LVDS 0x81
#define PORT_PANASONIC 0x85
/** @name 定义柔性接口中的各种管脚功能
@{

*/
/* 定义柔性接口中的各种管脚功能 */
typedef enum
{
	PIN_D0 = 0,
	PIN_D1,
	PIN_D2,
	PIN_D3,
	PIN_D4,
	PIN_D5,
	PIN_D6,
	PIN_D7,
	PIN_D8,
	PIN_D9,
	PIN_PCLK,
	PIN_HSYNC,
	PIN_VSYNC,
	PIN_MCLK,
	PIN_RESET,
	PIN_PWDN,
	PIN_PWDN2,
	PIN_GPIO1,
	PIN_SDA,
	PIN_SCL,
	PIN_NC,
	PIN_GPIO2,
	PIN_GPIO3,
	PIN_GPIO4,
	PIN_NC1,
	PIN_NC2,
	PIN_D10,
	PIN_D11,
	PIN_SPI_SCK, ///< 柔性接口 新增SPI接口
	PIN_SPI_CS,
	PIN_SPI_SDI,
	PIN_SPI_SDO,
	PIN_SPI_SDA,
	PIN_CLK_ADJ_200K, ///< 输出0-200Khz的可调节时钟频率
	PIN_CLK_ADJ_18M,  ///< 输出0-18Mhz的可调节时钟频率
	PIN_GPIO5,
	PIN_GPIO6,
	PIN_GPIO7,
	PIN_GPIO8,
	PIN_FRAME_UPDATE,
	PIN_D12,
	PIN_D13,
	PIN_D14,
	PIN_D15,
	PIN_FRAME_REFRESH,
} PIN_FUNC;
/** @} */

/** @name 定义柔性接口管脚名称(编号)
@{

*/
/* 定义柔性接口管脚名称(编号) */
typedef enum
{
	PIN_IO1 = 0,
	PIN_IO2,
	PIN_IO3,
	PIN_IO4,
	PIN_IO5,
	PIN_IO6,
	PIN_IO7,
	PIN_IO8,
	PIN_IO9,
	PIN_IO10,
	PIN_IO11,
	PIN_IO12,
	PIN_IO13,
	PIN_IO14,
	PIN_IO15,
	PIN_IO16,
	PIN_IO17,
	PIN_IO18,
	PIN_IO19,
	PIN_IO20,
	PIN_IO21,
	PIN_IO22,
	PIN_IO23,
	PIN_IO24,
	PIN_IO25,
	PIN_IO26,
} SOFT_PIN;
/** @} */

/* MIPI ctrl 扩展结构体 */
typedef struct MipiCtrlEx_s
{
	BYTE byPhyType; ///< mipi phy set (d-phy_1.5G/d-phy_2.5G,c-phy_2.0G

	BYTE byLaneCnt; ///< lane个数设置，有1/2/4lane

	DWORD dwCtrl; ///< MIPI ctrl,使用位定义，见MIPI_CTRL_XXXX定义

	UINT uVc; ///< 设置接收的图像通道号，0/1/2/3

	BOOL bVCFilterEn; ///< 使能过滤其他的虚拟通道

	UINT uPackID; ///< 使能输出的ID号

	BOOL bPackIDEn; ///< 使能当前设置的ID号输出 */

	/* 保留，填充0 */
	BYTE resv[62];

} MipiCtrlEx_t;

/** @name MIPI控制器特性的位定义
@{

*/
/* MIPI控制器特性的位定义 */
#define MIPI_CTRL_LP_EN 1						///< 允许进入LP状态
#define MIPI_CTRL_AUTO_START (1 << 1)			///< 出现差分信号后自动启动，用于OS测试
#define MIPI_CTRL_NON_CONT (1 << 2)				///< 使用非连续时钟
#define MIPI_CTRL_FULL_CAP (1 << 3)				///< 完整数据包获取，包括包头和CRC16校验，将导致每行图像数据增加6字节
#define MIPI_CTRL_CLK_LP_CHK (1 << 4)			///< 对CLK Lane的LP状态进行检测，强制要求MIPI TX端的Clk Lane必须进入一次LP状态
#define MIPI_CTRL_CLK_LP01_CHK (1 << 6)			///< 对CLK Lane的LP-10状态进行检查，如果没有这个状态，将会一直等待
#define MIPI_CTRL_DAT_LP01_CHK (1 << 7)			///< 对DATA Lane的LP-10状态进行检查，如果没有这个状态，将会一直等待
#define MIPI_CTRL_LRTE_EN (1 << 15)				///< 开启LRTE模式
#define MIPI_CTRL_LANE_CNT_DET_MANUAL (1 << 16) ///< lane/trio个数手动设置，不使能是自动检测lane/trio个数
/** @} */

/** @} */

/** @name MIPI Phy type选择
@{

*/
/* MIPI Phy type选择 */
#define MIPI_DPHY_1_5G 0 ///< 1.5G以下不带deskew校准的DPHY选择
#define MIPI_DPHY_2_5G 1 ///< 1.5G以上带deskew校准的DPHY选择
#define MIPI_CPHY 2		 ///< CPHY选择
/** @} */

/** @name 同步并行接口特性的位定义
@{

*/
/* 同步并行接口特性的位定义 */
#define PARA_PCLK_RVS (1 << 3)	///< PCLK取反
#define PARA_VSYNC_RVS (1 << 4) ///< VSYNC取反
#define PARA_HSYNC_RVS (1 << 5) ///< HSYNC取反
#define PARA_AUTO_POL (1 << 6)	///< VSYNC,HSYNC极性自动识别
/** @} */

/** @name 同步并行接口,3bit用于选择位宽
@{

*/
/* 3bit用于选择位宽 */
#define PARA_BW_8BIT 0
#define PARA_BW_10BIT 1
#define PARA_BW_12BIT 2
#define PARA_BW_16BIT 3
/** @} */

/** @name HiSPI接口特性的位定义
@{

*/
/* HiSPI接口特性的位定义 */
// 2bit用于选择位宽
#define HISPI_WW_10BIT 0
#define HISPI_WW_12BIT 1
#define HISPI_WW_14BIT 2
#define HISPI_WW_16BIT 3
/** @} */

/** @name 模拟图像模块特性的位定义
@{

*/
/* 模拟图像模块特性的位定义 */
// 2bit用于选择模拟图像的样式
#define SIM_STYLE1 0 ///< 输出固定颜色
#define SIM_STYLE2 1 ///< 水平渐变
#define SIM_STYLE3 2 ///< 垂直渐变
#define SIM_STYLE4 3 ///< 每帧渐变
/** @} */

/** @brief SLVS-EC接口配置
@{
*/
/* 用于配置SLVS-EC接口 */
typedef struct SlvsECCtrl_s
{
	UINT uLaneNum; // lane个数

	UINT uXvsSize;

	UINT uXhsSize;

	UINT Rsv[16]; // 备用

} SlvsECCtrl_t;

/** @} */

/****************************************************************************************
 *
 * I2C总线相关
 *
 ****************************************************************************************/
/*
 * 常见的寄存器读写模式
 * I2C mode definiton
 * when read or write by I2c ,should use this definiton...
 * Normal Mode:8 bit address,8 bit data,
 * Samsung Mode:8 bit address,8 bit data,but has a stop between slave ID and addr...
 * Micron:8 bit address,16bit data...
 * Stmicro:16bit addr ,8bit data,such as eeprom and stmicro sensor...
 */
/** @name I2C模式定义
@{

*/
/// I2C模式定义。
enum I2CMODE
{
	I2CMODE_NORMAL = 0, ///< 8 bit addr,8 bit value
	I2CMODE_SAMSUNG,	///< 8 bit addr,8 bit value,Stopen
	I2CMODE_MICRON,		///< 8 bit addr,16 bit value
	I2CMODE_STMICRO,	///< 16 bit addr,8 bit value, (eeprom also)
	I2CMODE_MICRON2,	///< 16 bit addr,16 bit value
};

/// sensor i2c上拉电阻选择
enum I2CPULLUPRESISTOR
{
	PULLUP_RESISTOR_1_5K = 0,  ///< 1.5K pull up resistor
	PULLUP_RESISTOR_4_7K = 1,  ///< 4.7K pull up resistor
	PULLUP_RESISTOR_0_56K = 2, ///< CMU958/DMU956/DMU927这些机型无560欧姆上拉电阻，如设置560欧姆上拉电阻，测试盒实际会使用1.14K上拉电阻
	PULLUP_RESISTOR_CLOSED = 3,
	PULLUP_RESISTOR_1_14K = 4,
	PULLUP_RESISTOR_0_375K = 5,
	PULLUP_RESISTOR_0_407K = 6,
	PULLUP_RESISTOR_0_5K = 7,
};
/** @} */

/** @name i2c/i3c模式定义
@{
*/
typedef enum SensorBusType
{
	I2C_BUS = 0,
	I3C_SDR_BUS = 1
};

/**@} */

/**@brief I3C配置命令定义
@{

*/

typedef enum I3CCmdDef
{
	I3C_ASSIGN_DYNAMIC_ADDRESS_FROM_STATIC = 0x87,
	I3C_SET_MWL = 0x89,
	I3C_SET_MRL = 0x8A,
	I3C_GET_MWL = 0x8B,
	I3C_GET_MRL = 0x8C,
};
/** @} */

/**@brief I3C配置
@{

*/

typedef struct I3CConfig_s
{
	BYTE byStaticAddr; //< 静态地址，一般为从I2C的地址
	I3CCmdDef uCmd;	   //< I3C命令码，如动态分配地址命令，读写最大长度设置等命令，该命令一般是1个字节，这里用4个字节应该足够了
	UINT uCmdSize;	   //< 动态分配地址命令和读写长度设置命令都为1字节
	UINT uSize;		   //< 配置数据size的大小
	BYTE *pData;	   //< 配置数据
} I3CConfig_t;
/** @} */

/** @name SPI模式定义
@{

*/
/// SPI模式定义
enum SPIMODE
{
	SPIMODE_SONY_A1_D1 = 0x81, ///< 8 bit addr,8 bit value		bit0-bit7
	SPIMODE_SONY_A1_D2,		   ///< 8 bit addr,16 bit value
	SPIMODE_SONY_A2_D1,		   ///< 16 bit addr,8 bit value
	SPIMODE_SONY_A2_D2,		   ///< 16 bit addr,16 bit value

	SPIMODE_40KFPS_A1_D1 = 0x88,

	/* panasonic lsb */
	SPIMODE_PANASONIC_A1_D1 = 0x91, ///< 8 bit addr,8 bit value
	SPIMODE_PANASONIC_A1_D2,		///< 8 bit addr,16 bit value
	SPIMODE_PANASONIC_A2_D1,		///< 16 bit addr,8 bit value
	SPIMODE_PANASONIC_A2_D2,		///< 16 bit addr,16 bit value

	/* smartsens msb */
	SPIMODE_SMARTSENS_A2_D1 = 0xcb, ///< 16 bit addr,8 bit value  bit15-bit0
};
/** @} */

#define MASTER_CTRL_DATA_SHIFT (1 << 0) ///< 数据移位模式  0: MSB 先出, 1: LSB 先出
#define MASTER_CTRL_CPOL (1 << 1)		///< 时钟极性（Clock polarity） - SPI空闲时，时钟信号的电平状态[ 1: 空闲时高电平 ，0: 空闲时低电平 ]

/* 时钟相位（Clock phase） - SPI在SCLK第几个边沿采样
CPHA=0，表示第一个边沿：
CPOL=0，idle时候的是低电平，第一个边沿就是从低变到高，所以是上升沿；
对于CPOL=1，idle时候的是高电平，第一个边沿就是从高变到低，所以是下降沿；
CPHA=1，表示第二个边沿：
CPOL=0，idle时候的是低电平，第二个边沿就是从高变到低，所以是下降沿；
CPOL=1，idle时候的是高电平，第一个边沿就是从低变到高，所以是上升沿；
*/
#define MASTER_CTRL_CPHA (1 << 2)
#define MASTER_CTRL_DELAY (1 << 3) ///< 0:寄存器间隔无需延时；1：寄存器写完后需延时，延时值由data中给出

#define MASTER_CTRL_THREE_WIRE (1 << 4)	   ///< 0:是四线模式，1是三线模式
#define MASTER_CTRL_RW_ENDGE_DIFF (1 << 5) ///< 读写边沿是否一致，1是相反，0是一致
#define MASTER_CTRL_CS_HIGH (1 << 6)	   ///< 片选高有效，1是高有效，0是低有效

/* SPI配置结构体 */
typedef struct MasterSpiConfig_s
{
	double fMhz;	///< 配置SPI的时钟
	BYTE byWordLen; ///< Word length in bits. 0： 8bit ；1：16bit（暂时无效废弃），默认为0
	BYTE byCtrl;	///< 支持的位控制码：MASTER_CTRL_DATA_SHIFT/MASTER_CTRL_CPOL/ MASTER_CTRL_CPHA/MASTER_CTRL_DELAY
	BYTE Rsv[64];	///< 保留 */
} MasterSpiConfig_t;

typedef enum
{
	USB2_0 = 0,
	USB3_0 = 1,
	USB3_1 = 2,

	FIBRE = 20 /// 光纤产品
} DevLinkType;

/** @brief 设备链接描述结构体
@{
*/
/* 返回设备的链接信息 */
typedef struct DevLinkStatus_s
{
	BOOL bLinkOk;		  //< 指示当前link是否正常(已经连接设备)
	double lfLinkRate;	  //< Link 速度，单位Mbps
	DevLinkType LinkType; //< Link 类型
	ULONG uTransLineNum;  //< 当前设备传输线个数，如USB或者光纤线个数
	ULONG uPortMask;	  //< 当前设备光口MASK信息，BIT0为1，表示光口0使用，BIT1为1，表示光口1使用
	ULONG Rsv[30];
} DevLinkStatus_t;
/** @} */

/** @brief 设备信息描述结构体
@{
*/
/* 返回设备的信息 */
typedef struct DevInfo_s
{
	/* 原始帧，盒子采集到的总帧数,这些帧是sensor原始输出的 */
	UINT uOrgFrameCnt;

	/* 电脑端驱动底层GrabFrame接口获取的正确帧计数，这些帧经过进一步检查，例如宽高是否匹配，确保了正确性；
一般情况下：uOrgFrameCnt >= uFrameOkCnt + uFrameProblemCnt; */
	UINT uFrameOkCnt;

	/* 采集的问题帧计数，这些帧可能是宽高不匹配或者有crc，ecc等错误的帧 */
	UINT uFrameProblemCnt;

	/* 被提交/输出的有效帧计数,由GrabFrame接口输出给上层调用者的帧计数 */
	UINT uFrameOutCnt;

	/* 备用 */
	UINT Rsv[64];
} DevInfo_t;
/** @} */

/** @brief 获取MIPI状态信息
@{
*/
typedef struct MipiStatusInfo_s
{
	/* ecc corrected cnt */
	UINT uEccCorrectedCnt;

	/* packets per frame */
	UINT uPacketsPerFrame;

	/* lane lock state,bit0-bit3依次表示lane1-lane4的状态 */
	UINT uLaneLockState;

	/* 备用 */
	UINT Rsv[128];

} MipiStatusInfo_t;

/** @} */

/** @brief 抓帧策略设置
@{
*/
/* 用户可设置对数据量不匹配或crc等错误帧过滤 */
typedef struct FrameFilter_s
{
	/* 对有crc错误的帧过滤 */
	bool bCrcErrorFilter;

	/* 对size不匹配的帧过滤 */
	bool bSizeErrorFilter;

	/* 备用 */
	UINT Rsv[64];

} FrameFilter_t;

/** @} */

/** @} */ // end of group3

/****************************************************************************************
 *
 * 图像数据采集相关
 *
 ****************************************************************************************/

/** @defgroup group4 图像数据采集相关
@{

*/

/** @name FrameBuffer模式配置
@{

*/
///< FrameBuffer模式配置

#define BUF_MODE_NORMAL 0 ///< 一般模式，缓存效果相当于FIFO；当缓存量超过；\n
						  // 缓存上限设置时，新的帧将不会被写入到缓存；

#define BUF_MODE_SKIP 1 ///< 跳帧模式，缓存中的帧将不会出现“排队”现象；

#define BUF_MODE_NEWEST 2 ///< NEWEST模式，目前只对PCI-E接口的机型有效；\n
						  // GrabFrame将获取最新缓存到的帧；对于其他机型\n
						  // 将等效于SKIP模式
/** @} */

/** @brief Buffer信息描述结构体
@{
*/
/* 用于配置FrameBuffer */
typedef struct _FrameBufferConfig
{
	ULONG uMode;		 ///< frame buffer模式选择,见BUF_MODE_XXXX
	ULONG uBufferSize;	 ///< 设备中的帧缓存大小(字节)，设备固定，用户设置无效
	ULONG uUpLimit;		 ///< 设备缓存上限设置(字节)，缓存量超过这个上限时，新的帧将被丢弃
	ULONG uBufferFrames; ///< 驱动的帧缓存数,只对BUF_MODE_NORMAL模式有效
	BOOL bLite;			 ///< 是否使用紧凑的内存申请方式，ISP使用的内存将不预先申请
	ULONG resv[14];		 ///< 保留，填充0
} FrameBufferConfig;
/** @} */

/** @brief 帧相关信息，与帧数据对应，对其进行描述
@{
*/
/* 帧相关信息，与帧数据对应，对其进行描述 */
typedef struct sFrameInfo
{
	BYTE byChannel;		///< 图像通道标识，只有UH920/DTLC2支持
	USHORT uWidth;		///< 图像的宽度，单位字节
	USHORT uHeight;		///< 图像的高度，单位字节
	UINT uDataSize;		///< 数据量大小，单位字节
	UINT64 uiTimeStamp; ///< 帧开始时间戳值，单位us
} FrameInfo;
/** @} */
/** @brief 扩展的帧信息结构体
@{
*/
// 扩展的帧信息结构体
typedef struct sFrameInfoEx
{
	BYTE byChannel;		  ///< 图像通道标识，只有UH920/DTLC2支持
	BYTE byMipiVcChannel; ///< MIPI虚拟通道号，只对MIPI接口有效
	BYTE resvl[2];		  ///< 保留2字节，填充0
	BYTE byImgFormat;	  ///< 图像格式，D_RAW8、D_RAW10...
	USHORT uWidth;		  ///< 图像的宽度，单位字节
	USHORT uHeight;		  ///< 图像的高度，单位字节
	UINT uDataSize;		  ///< 数据量大小，单位字节
	UINT uFrameTag;		  ///< 帧标识
	double fFSTimeStamp;  ///< 帧开始的时间戳，单位us
	double fFETimeStamp;  ///< 帧结束的时间戳，单位us
	UINT uEccErrorCnt;	  ///< 每帧的ECC错误计数，只对MIPI接口有效
	UINT uCrcErrorCnt;	  ///< 每帧的CRC错误计数，只对MIPI接口有效
	UINT uFrameID;		  ///< 帧计数
	UINT resv[60];		  ///< 保留，填充0
} FrameInfoEx;
/** @} */
/** @name FrameInfoEx::uFrameTag位定义解释
@{

*/
/* FrameInfoEx::uFrameTag */
/* 采集已经开始 */
#define FRM_INFO_TAG_STARTED 1

/* 帧已经采集完成 */
#define FRM_INFO_TAG_GRAB_OK (1 << 1)

/* 帧已经处理完成 */
#define FRM_INFO_TAG_PROC_OK (1 << 2)

/* 帧已经坏掉或无效，可能采集和处理了一半，后续无需再处理，也不用提交 */
#define FRM_INFO_TAG_BAD (1 << 4)

/* 可能出现了一些错误，但是数据量完整 */
#define FRM_INFO_TAG_ERR (1 << 5)

/* Restart之后的第一帧 */
#define FRM_INFO_TAG_FIRST (1 << 6)

/* TestWindow使能标志 */
#define FRM_INFO_TAG_TW (1 << 7)
/** @} */

/** @name 预览窗口定义
@{

*/
/* 预览窗口定义 */
#define PREVIEW_ROI_B0 0x00
#define PREVIEW_ROI_B1 0x01
#define PREVIEW_ROI_B2 0x02
#define PREVIEW_ROI_B3 0x03
#define PREVIEW_ROI_B4 0x04
#define PREVIEW_ROI_GRID 0x05
#define PREVIEW_QUICK 0x06
#define PREVIEW_FULL 0x07
#define PREVIEW_NOTHING 0x08
/** @} */

typedef enum
{
	WORK_NORMAL = 0x00,
	WORK_STANDBY_CURRENT_TEST = 0x40, /// 进入待机电流测试模式
	WORK_OS_TEST = 0x60
} WorkMode;
/** @} */

/** @} */ // end of group4

/****************************************************************************************
 *
 * 电源管理相关
 *
 ****************************************************************************************/
/** @defgroup group5 电源管理单元相关

@{

*/

/* 帧相关信息，与帧数据对应，对其进行描述 */
/** @name SENSOR需要的电源类型定义
@{

*/
/* 定义SENSOR需要的电源类型 */
/// 定义SENSOR需要的电源类型。
typedef enum
{
	/* A通道，或只有一个通道时 */
	POWER_AVDD = 0,	 ///< AVDD
	POWER_DOVDD = 1, ///< DOVDD
	POWER_DVDD = 2,	 ///< DVDD
	POWER_AFVCC = 3, ///< AFVCC
	POWER_VPP = 4,	 ///< VPP

	/* B通道,(B通道电源定义，只有UH920使用) */
	POWER_AVDD_B = 5,  ///< B通道AVDD
	POWER_DOVDD_B = 6, ///< B通道DOVDD
	POWER_DVDD_B = 7,  ///< B通道DVDD
	POWER_AFVCC_B = 8, ///< B通道AFVCC
	POWER_VPP_B = 9,   ///< B通道VPP

	/* 新增加的电源通道定义 */
	POWER_OISVDD = 10,
	POWER_AVDD2 = 11,
	POWER_AUX1 = 12,
	POWER_AUX2 = 13,
	POWER_VPP2 = 14,

	/* CC16机型定义的电源 */
	POWER_SENSOR0 = 40,
	POWER_SENSOR1 = 41,
	POWER_SENSOR2 = 42,
	POWER_SENSOR3 = 43,
	POWER_VDDIO = 70
} SENSOR_POWER;

#define POWER_AUX POWER_AUX1

/** @} */

/** @name SENSOR系统电源类型定义
@{

*/
/* 定义系统电源类型 */
/// 定义系统电源类型。
typedef enum
{
	POWER_SYS_12V = 0, ///< 12V系统电源
	POWER_SYS_5V,	   ///< 5V系统电源
	POWER_SYS_3_3V	   ///< 3.3V系统电源
} SYS_POWER;
/** @} */

/** @name SENSOR电源模式定义
@{

*/
/* 定义电源模式 */
/// 定义电源模式。
typedef enum
{
	POWER_MODE_WORK = 0, ///< SENSOR电源为工作模式
	POWER_MODE_STANDBY,	 ///< SENSOR电源为待机模式
	POWER_MODE_PWDN		 ///< SENSOR电源为掉电模式
} POWER_MODE;
/** @} */

/** @name 电流测试量程定义
@{

*/
/* 定义电流测试量程 */
/// 定义电流测试量程。
typedef enum
{
	CURRENT_RANGE_MA = 0, ///< 电流测试量程为mA
	CURRENT_RANGE_UA,	  ///< 电流测试量程为uA
	CURRENT_RANGE_NA	  ///< 电流测试量程为nA
} CURRENT_RANGE;
/** @} */

/** @name 电流测试模式
@{

*/
/* 定义电流测试模式 */
/// 定义电流测试模式。
typedef enum
{
	MEASUREMENT_MODE_NORMAL = 0,		///< 普通模式，该模式下，lfTriggerPoint设置的值失效
	MEASUREMENT_MODE_TRIGGER_POINT = 1, ///< 电流采集使用触发点模式，TOF大电流采集建议使用
} CURRENT_MEASUREMENT_MODE;
/** @} */

/** @name 电流测试设置
@{
*/
typedef struct PmuCurrentMeasurement_s
{
	SENSOR_POWER PowerType;				  ///< 电源类型
	CURRENT_MEASUREMENT_MODE MeasureMode; ///< 参见CURRENT_MEASUREMENT_MODE定义
	UINT uStatisticsNum;				  ///< 设置统计个数，电流采集使用触发点模式时候有效
	double lfTriggerPoint;				  ///< 电流采集触发值	，单位，mA。精度，1mA。只有MU950(TOF)机型支持触发采样点设置，只支持PosEN=TRUE
	BOOL bPosEn;						  ///< 标识触发值正向或反向有效，为True，则大于TriggerPoint触发采集电流，为False，则小于TriggerPoint触发采集电流
	UINT Rsv[32];						  ///< 备用
} PmuCurrentMeasurement_t;
/** @} */

/** @name 滤波电容设置
@{

*/
/* 定义滤波电容模式 */
typedef enum
{
	FILTER_CAP_AUTO_SET_MODE = 0,	///< 自动配置模式
	FILTER_CAP_MANUAL_SET_MODE = 1, ///< 该模式用户可手动设置开启/关闭滤波电容
} FILTER_CAP_MODE;
/** @} */

/** @name 滤波电容配置
@{
*/
typedef struct PmuFilterCap_s
{
	SENSOR_POWER PowerType; ///< 电源类型
	FILTER_CAP_MODE Mode;	///< 模式设置
	BOOL bEn;				///< 手动设置模式才有用，true打开，false关闭
	UINT Rsv[16];			///< 备用
} PmuFilterCap_t;
/**@} */

/** @name ADC采集
@{
*/
typedef enum
{
	ADC_1 = 0,
	ADC_2 = 1,
	ADC_3 = 2,
	ADC_4 = 3,
	ADC_5 = 4,
	ADC_6 = 5
} ADC_CHANNEL;

/** @} */

/** @} */ // end of group5

/****************************************************************************************
 *
 * 按键功能定义(dtTest.exe使用的定义)
 *
 ****************************************************************************************/
/** @defgroup group6 初始化控制相关
@{

*/
/** @name 按键功能定义
@{

*/
#define KEY_ROI_B0 0x100
#define KEY_ROI_B1 0x80
#define KEY_ROI_B2 0x40
#define KEY_ROI_B3 0x10
#define KEY_ROI_B4 0x20
#define KEY_ROI_GRID 0x04
#define KEY_FULL 0x08
#define KEY_PLAY 0x01
#define KEY_CAM 0x02
#define KEY_NOTHING 0x00
/** @} */
/** @} */ // end of group6

/****************************************************************************************
 *
 * 本系统支持的AF器件型号定义
 *
 ****************************************************************************************/
/** @defgroup group7 AF相关
@{

*/
/** @name 支持的AF器件型号定义
@{

*/
#define AF_DRV_AD5820 0
#define AF_DRV_DW9710 0
#define AF_DRV_DW9714 0

#define AF_DRV_AD5823 1
#define AF_DRV_FP5512 2
#define AF_DRV_DW9718 3
#define AF_DRV_BU64241 4
#define AF_DRV_LV8498 5
#define AF_DRV_BU64291 6
#define AF_DRV_AD1457 7

#define AF_DRV_DW9761 8
#define AF_DRV_AD5816 8

#define AF_DRV_AK7345 9
#define AF_DRV_DW9800 10

#define AF_DRV_ZC533 11
#define AF_DRV_BU64295 12
#define AF_DRV_DW9719 13
// #define AF_DRV_SC9714		14
#define AF_DRV_FP5518 14

#define AF_DRV_AK7374 15

#define AF_DRV_LC898219 16

#define AF_DRV_MAX 30

/** @} */
/** @} */ // end of group7

/****************************************************************************************
 *
 * OS/LC相关
 *
 ****************************************************************************************/
/** @defgroup group8 LC/OS相关
@{

*/
/** @name OS/LC测试配置定义
@{

*/
/* OS/LC测试配置定义，在LC_OS_CommandConfig函数中使用 */

#define OS_CFG_TEST_ENA (1 << 7)  ///< OS测试使能
#define LC_CFG_TEST_ENA (1 << 6)  ///< LC测试使能
#define OS_CFG_CHANNEL_A (1 << 5) ///< 使能测试A通道OS测试(DTLC2机型支持，其他机型不生效)
#define OS_CFG_CHANNEL_B (1 << 4) ///< 使能测试B通道OS测试(DTLC2机型支持，其他机型不生效)
#define OS_CFG_HIGH (1 << 3)	  ///< 使能正端OS测试
#define OS_CFG_LOW (1 << 2)		  ///< 使能负端OS测试
#define LC_CFG_HIGH (1 << 1)	  ///< 使能正端LC测试
#define LC_CFG_LOW (1 << 0)		  ///< 使能负端LC测试

#define OS_CFG_DOUBLE (1 << 8) ///< G系列光纤产品（G42/G22）配置支持A,B或者C，D一起进行OS测试模式，双摄模组共地的时候使用，
							   ///< 并且注意:该模式下需要做同步，保证2个模组同时进入OS测试或退出OS测试

#define OS_CFG_QUICK (1 << 15) ///< 暂不支持
/** @} */
/** @name OS/LC测试结果定义，OS_Read函数返回的结果
@{

*/
/* OS/LC测试结果定义，OS_Read函数返回的结果 */
#define OS_TEST_RESULT_PASS 0		// 通过测试
#define OS_TEST_RESULT_NG 1			// 未通过测试
#define OS_TEST_RESULT_FAILED 0xfe	// 测试失败
#define OS_TEST_RESULT_INVALID 0xff // 测试无效
/** @} */

/** @} */ // end of group8

/************************************************************************
*
*外部扩展IO定义
*
/************************************************************************/
/** @defgroup group9 扩展IO
@{

*/
/** @name 外部扩展IO管脚定义
@{

*/
typedef enum
{
	GPIO0 = 0, ///< GPIO0
	GPIO1,	   ///< GPIO1
	GPIO2,	   ///< GPIO2
	GPIO3,	   ///< GPIO3
	GPIO4,	   ///< GPIO4
	GPIO5,	   ///< GPIO5
	GPIO6,	   ///< GPIO6
	GPIO7,	   ///< GPIO7
	GPIO8,	   ///< GPIO8
	GPIO9,	   ///< GPIO9
	GPIO10,	   ///< GPIO10
	GPIO11,	   ///< GPIO11
} EXT_GPIO;
/** @} */

/** @name 外部扩展IO模式定义
@{

*/
typedef enum
{
	GPIO_INPUT = 0, ///< 输入模式
	GPIO_OUTPUT,	///< 恒电平输出模式
	GPIO_OUTPUT_PP, ///< 高低电平交互输出

} EXT_GPIO_MODE;
/** @} */

/** @} */ // end of group9

/* 调试报告信息，功能模块 */
enum DtDbgRePortPart_e
{
	/* 默认或未作功能分类 */
	PART_DEFAULT = 0x0,

	/* 控制相关 */
	PART_CONTROL = 0x10,

	/* 数据流或采集相关 */
	PART_STREAM = 0x20,

	/* mipi */
	PART_MIPI = 0x30,

	/* GPIO功能相关 */
	PART_GPIO = 0x40,

	/* 图像效果相关 */
	PART_IMAGE = 0x50,

	/* power相关 */
	PART_POWER = 0x60,

	/* open/short test相关 */
	PART_OS_TEST = 0x70,

	/* 主控制器调试信息 */
	PART_MAIN_CONTROL = 0x80
};

/* 调试报告信息，级别 */
enum DtDbgRePoportLevel_e
{
	/* 信息或提示 */
	LEVEL_INFO = 0x10,

	/* 问题或故障 */
	LEVEL_PROBLEM = 0x40
};

/* 调试报告 */
typedef struct DtDbgReport_s
{
	/* 调试报告的功能分类 */
	DtDbgRePortPart_e Part;

	/* 调试报告的级别 */
	DtDbgRePoportLevel_e Level;

	/* 是否强制记录到文件，不管是否开启log文件记录 */
	bool bForce;

	/* 保留64字节 */
	UINT resv[16];

	/* 调试报告的相关文本内容，将插入到log文件当中 */
	char text[128];
} DtDbgReport_t;

// 定义的支持测试电阻的管脚
enum ResisCalculateForIoSet
{
	PO1 = 0, // 外壳PO1
	PO2 = 1, // 外壳PO2
	PO3 = 2, // 外壳PO3
	PO4 = 3	 // 外壳PO4
};

#define RESIS_TEST_COMMON_GROUND 1 << 0

/****************************************************************************************
 *
 * 一些SDK接口函数中使用到的宏定义(dtccm使用的定义)
 *
 ****************************************************************************************/
// PMU range....
#define PMU1_1 0x11
#define PMU1_2 0x10
#define PMU1_3 0x12
#define PMU2_1 0x21
#define PMU2_2 0x20
#define PMU2_3 0x22
#define PMU3_1 0x31
#define PMU3_2 0x30
#define PMU3_3 0x32
#define PMU4_1 0x41
#define PMU4_2 0x40
#define PMU4_3 0X42
#define PMU5_1 0x51
#define PMU5_2 0x50
#define PMU5_3 0X52

#define I2C_400K 1
#define I2C_100K 0

#define I_MAX_100mA 1
#define I_MAX_300mA 0

#define PMU_ON 0
#define PMU_OFF 1

#define POWER_ON 1
#define POWER_OFF 0

#define CLK_ON 1
#define CLK_OFF 0

#define IO_PULLUP 1
#define IO_NOPULL 0

#define MULTICAM_NORMAL 0x00
#define MULTICAM_PWDN_NOT 0x01
#define MULTICAM_RESET_PWDN_OVERLAP 0x02
#define MULTICAM_SPECIAL 0x03

/*
easy 电源接口相关定义
*/

typedef enum
{
	EZ_POWER_CH_ALL = 0,
	EZ_POWER_CH_DOVDD = 0,
	EZ_POWER_CH_DVDD = 1,
	EZ_POWER_CH_AVDD = 2,
	EZ_POWER_CH_VPP = 3,
	EZ_POWER_CH_AFVCC = 4,
	EZ_POWER_CH_OISVDD = 5,
	EZ_POWER_CH_AVDD2 = 6,
	EZ_POWER_CH_AUX1 = 7,
	EZ_POWER_CH_VPP2 = 8,
} esPowerCh;

#define POWER_CONFIG_ONOFF 1 << 0		 // 开关设置
#define POWER_CONFIG_VOLTAGE 1 << 1		 // 电压值设置
#define POWER_CONFIG_CURRENTLIMIT 1 << 2 // 限流值设置
#define POWER_CONFIG_SLOPE 1 << 3		 // 斜率设置
#define POWER_CONFIG_SPEED 1 << 4		 // 采样速度设置
#define POWER_CONFIG_RANGE 1 << 5		 // 量程设置

typedef struct _ezPowerSequence
{
	UINT uSequence; // 电压时序,电压上电或下电优先级设置，如0为优先级最高，1次之。如果优先级一致，则同时上电或下电
	double lfDelay; // 延时设置,ms
	UINT rsv[16];
} ezPowerSequence;

// 电源通道，每个电源通道包含开关、限流、电压、上升斜率等设置
typedef struct _ezPowerChannel
{
	BOOL bOnOff;			  // 开关
	double lfVolt;			  // 电压,mV
	double lfCurrentLimit;	  // 限流值,uA
	double lfSlope;			  // 斜率,mV/ms
	CURRENT_RANGE range;	  // 电流量程,nA,uA,mA
	UINT uSpeed;			  // 电流采样速度,ms
	ezPowerSequence sequence; // 时序控制
	UINT uConfigCode;		  // 功能配置码,通过位定义的方式指示哪些参数需要配置到设备
	UINT uStateCode;		  // 状态码
	UINT rsv[32];
} ezPowerChannel;

typedef struct _ezPowerConfig
{
	ezPowerChannel dovdd;
	ezPowerChannel dvdd;
	ezPowerChannel avdd;
	ezPowerChannel vpp;
	ezPowerChannel afvcc;
	ezPowerChannel oisvdd;
	ezPowerChannel avdd2;
	ezPowerChannel aux1;
	ezPowerChannel aux2;
	ezPowerChannel vpp2;
	ezPowerChannel rsv[16];
} ezPowerConfig;

/** @} */

#define SENSOR_I2C_RD_NO_STOP 1 << 0 // I2C读阶段不发送stop指令

/** @name SensorI2cRw结构体
@{
*/
typedef struct ezSensorI2cRw_s
{
	UINT uCtrl;		  ///< 控制码,SENSOR_I2C_xxx
	BYTE bySlaveAddr; ///< 从器件地址
	BYTE *pWrData;	  ///< 写入数据块
	UINT uWrSize;	  ///< 写入数据块的字节数
	BYTE *pRdData;	  ///< 读回数据块
	UINT uRdSize;	  ///< 读出数据块字节数
	UINT rsv[16];	  ///< 保留
} ezSensorI2cRw_t;
/** @} */

// 定义功能控制码

typedef enum
{
	EZ_POWER_ON = 0x10,	 // 电源上电控制:上电时序，电压设置，斜率设置，量程等一次配置成功
	EZ_POWER_UP = 0x11,	 // 电源下电
	EZ_POWER_GET = 0x12, // 获取电源各参数状态
} ezCtrl;

/******/
/* 指纹模组配置 */
typedef struct FpmConfig_s
{
	BYTE byMode;
	UINT uIrqCnt; ///< 设置几行为一个中断行,如宽度为320，uIrqCnt=4，则一次读取320*4个数据更新中断号
	BYTE rsv[64]; ///< 保留
} FpmConfig_t;

/****************************************************************************************
 *
 * 常用错误码
 *
 ****************************************************************************************/

#define DT_ERROR_NO_ACTION 8			   ///< 无动作
#define DT_ERROR_IGNORED 7				   ///< 操作忽略掉了，不须要任何动作
#define DT_ERROR_NEED_OTHER 6			   ///< 需要其他数据和操作
#define DT_ERROR_NEXT_STAGE 5			   ///< 还须进行下一阶段，只完成了部分动作
#define DT_ERROR_BUSY 4					   ///< 正忙(上一次操作还在进行中)，此次操作不能进行
#define DT_ERROR_WAIT 3					   ///< 需要等待(进行操作的条件不成立)，可以再次尝试
#define DT_ERROR_IN_PROCESS 2			   ///< 正在进行，已经被操作过
#define DT_ERROR_OK 1					   ///< 操作成功
#define DT_ERROR_FAILED 0				   ///< 操作失败
#define DT_ERROR_INTERNAL_ERROR -1		   ///< 内部错误
#define DT_ERROR_UNKNOW -1				   ///< 未知错误
#define DT_ERROR_NOT_SUPPORTED -2		   ///< 不支持该功能
#define DT_ERROR_NOT_INITIALIZED -3		   ///< 初始化未完成
#define DT_ERROR_PARAMETER_INVALID -4	   ///< 参数无效
#define DT_ERROR_PARAMETER_OUT_OF_BOUND -5 ///< 参数越界
#define DT_ERROR_UNENABLED -6			   ///< 未使能
#define DT_ERROR_UNCONNECTED -7			   ///< 未连接到设备
#define DT_ERROR_NOT_VALID -8			   ///< 功能无效
#define DT_ERROR_UNPLAY -9				   ///< 设备没打开
#define DT_ERROR_NOT_STARTED -10		   ///< 未启动
#define DT_ERROR_NOT_STOPPED -11		   ///< 未停止
#define DT_ERROR_NOT_READY -12			   ///< 未准备好
#define DT_ERROR_DESCR_FAULT -20		   ///< 错误的描述
#define DT_ERROR_NAME_FAULT -21			   ///< 错误的名称
#define DT_ERROR_VALUE_FAULT -22		   ///< 错误的赋值
#define DT_ERROR_LIMITED -28			   ///< 被限制
#define DT_ERROR_FUNCTION_INVALID -29	   ///< 功能无效
#define DT_ERROR_IN_AUTO -30			   ///< 在自动进行中，手动方式无效
#define DT_ERROR_DENIED -31				   ///< 操作被拒绝
#define DT_ERROR_BAD_ALIGNMENT -40		   ///< 偏移或地址未对其
#define DT_ERROR_ADDRESS_INVALID -41	   ///< 地址无效
#define DT_ERROR_SIZE_INVALID -42		   ///< 数据块大小无效
#define DT_ERROR_OVER_LOAD -43			   ///< 数据量过载
#define DT_ERROR_UNDER_LOAD -44			   ///< 数据量不够
#define DT_ERROR_BUFFER_SMALL -52		   ///< Buffer空间太小
#define DT_ERROR_CHECKED_FAILED -50		   ///< 检查、校验失败
#define DT_ERROR_UNUSABLE -51			   ///< 不可用
#define DT_ERROR_BID_INVALID -52		   ///< 业务ID无效或不匹配

/* IO，存储，设备相关  */
#define DT_ERROR_TIME_OUT -1000		   ///< 超时错误
#define DT_ERROR_IO_ERROR -1001		   ///< 硬件IO错误
#define DT_ERROR_COMM_ERROR -1002	   ///< 通讯错误
#define DT_ERROR_BUS_ERROR -1003	   ///< 总线错误
#define DT_ERROR_FORMAT_INVALID -1004  ///< 格式错误
#define DT_ERROR_CONTENT_INVALID -1005 ///< 内容无效
#define DT_ERROR_BAD_CHECKSUM -1006	   ///< 数据校验错误
#define DT_ERROR_I2C_FAULT -1010	   ///< I2C总线错误
#define DT_ERROR_I2C_ACK_TIMEOUT -1011 ///< I2C等待应答超时
#define DT_ERROR_I2C_BUS_TIMEOUT -1012 ///< I2C等待总线动作超时，例如SCL被外部器件拉为低电平
#define DT_ERROR_SPI_FAULT -1020	   ///< SPI总线错误
#define DT_ERROR_UART_FAULT -1030	   ///< UART总线错误
#define DT_ERROR_GPIO_FAULT -1040	   ///< GPIO总线错误
#define DT_ERROR_USB_FAULT -1050	   ///< USB总线错误
#define DT_ERROR_PCI_FAULT -1060	   ///< PCI总线错误
#define DT_ERROR_PHY_FAULT -1070	   ///< 物理层错误
#define DT_ERROR_LINK_FAULT -1080	   ///< 链路层错误
#define DT_ERROR_TRANS_FAULT -1090	   ///< 传输层错误

#define DT_ERROR_NO_DEVICE_FOUND -1100			   ///< 没有发现设备/// @brief 未找到逻辑设备
#define DT_ERROR_NO_LOGIC_DEVICE_FOUND -1101	   ///< 未找到逻辑设备
#define DT_ERROR_DEVICE_IS_OPENED -1102			   ///< 设备已经打开
#define DT_ERROR_DEVICE_IS_CLOSED -1103			   ///< 设备已经关闭
#define DT_ERROR_DEVICE_IS_DISCONNECTED -1104	   /// 设备已经断开连接
#define DT_ERROR_DEVICE_IS_OPENED_BY_ANOTHER -1105 /// 设备已经被其他主机打开
#define DT_ERROR_KERNEL_DRIVER_PROBLEM -1106	   /// 内核驱动出现问题
#define DT_ERROR_SOCKET_PROBLEM -1107			   /// 网络套接出现问题

#define DT_ERROR_NO_MEMORY -1200			///< 没有足够系统内存
#define DT_ERROR_MEM_FAULT -1201			///< 存储器读写出现误码或无法正常读写
#define DT_ERROR_WRITE_PROTECTED -1202		///< 写保护，不可写
#define DT_ERROR_FILE_CREATE_FAILED -1300	///< 创建文件失败
#define DT_ERROR_FILE_INVALID -1301			///< 文件格式无效
#define DT_ERROR_FILE_READ_FAILED -1302		///< 读取文件失败
#define DT_ERROR_FILE_WRITE_FAILED -1303	///< 写入文件失败
#define DT_ERROR_FILE_OPEN_FAILED -1304		///< 打开文件失败
#define DT_ERROR_FILE_CHECKSUM_FAILED -1305 ///< 读取数据较检失败

#define DT_ERROR_GRAB_FAILED -1600		  ///< 数据采集失败
#define DT_ERROR_LOST_DATA -1601		  ///< 数据丢失，不完整
#define DT_ERROR_EOF_ERROR -1602		  ///< 未接收到帧结束符
#define DT_ERROR_GRAB_IS_OPENED -1603	  ///< 数据采集功能已经打开
#define DT_ERROR_GRAB_IS_CLOSED -1604	  ///< 数据采集功能已经关闭
#define DT_ERROR_GRAB_IS_STARTED -1605	  ///< 数据采集已经启动
#define DT_ERROR_GRAB_IS_STOPPED -1606	  ///< 数据采集已经停止
#define DT_ERROR_GRAB_IS_RESTARTING -1607 ///< 数据采集正在重启
#define DT_ERROR_GRAB_IS_HOLD -1608		  ///< 数据采集已经暂停
#define DT_ERROR_FRAME_IS_OUTDATED -1609  ///< 采集的帧已经过时
#define DT_ERROR_ROI_PARAM_INVALID -1610  ///< 设置的ROI参数无效
#define DT_ERROR_ROI_NOT_SUPPORTED -1611  ///< ROI功能不支持
#define DT_ERROR_GRAB_IS_DROPPED -1613	  ///< 丢弃重启视频流后不需要的帧
#define DT_ERROR_CALIBRATE_FAILED -1614	  ///< 校准失败

/****************************************************************************************
 *
 * 用于方便程序编写，遇到错误时立即返回错误码
 *
 ****************************************************************************************/
#define CHECK_RETURN(_FUN_)         \
	{                               \
		int iChkRet = _FUN_;        \
		if (iChkRet != DT_ERROR_OK) \
			return iChkRet;         \
	}

#endif // __IMAGEKIT_H__
