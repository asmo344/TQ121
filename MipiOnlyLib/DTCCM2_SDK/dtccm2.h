/******************************************************************************
DTCCM第二版
******************************************************************************/

#ifndef _DTCCM2_H_
#define _DTCCM2_H_

// #define _DTCALL_ _stdcall
#define _DTCALL_

#include "../DTCCM2_SDK/imagekit.h"

#ifdef DTCCM2_EXPORTS

#define DTCCM_API extern "C" __declspec(dllexport)

#else

#define DTCCM_API extern "C" __declspec(dllimport)

#endif

/*! \mainpage DOTHINKEY ImageKit API Manual
 *
 * \section 产品介绍
 *
 * - USB2.0系列
 * -# HS128
 * -# HS280
 * -# HS300
 * -# HS300D
 * -# HV810
 * -# HV810D
 * -# HV910
 * -# HV910D
 * - PCIE系列
 * -# PE300
 * -# PE300D
 * -# PE810
 * -# PE810D
 * -# PE910
 * -# PE910D
 * -# PE350
 * -# PE950
 * -# MP950
 * - USB3.0系列
 * -# UT300
 * -# UO300
 * -# UV910
 * -# UM330
 * -# UM900
 * -# UH910
 * -# DTLC2
 * -# UH920
 * -# UF920
 * -# MU950
 * -# DMU956
 * -# CMU958
 * -# CMU959
 * -# DMU927
 * -# CMU970
 * -# CMU957
 * -# MU950-TOF
 * - K系列
 * -# KM25
 * -# KM27H
 * -# K25D
 * -# K15D
 * - 万兆网光纤系列
 * -# CTG970
 * -# G40
 * -# G41
 * -# G42
 * -# G22
 * -# G42L
 * -# G22L
 * -# CC16
 * -# GQ4
 * -# F24
 * -# F22
 * \section  公司网址
 * http://www.dothinkey.com
 *
 *
 *
 * \section 文档发布版本记录
 * -# 2013/8/22  生成DTCC2APIGuide；\n
 *			    DTCCM2APIGuide版本为：1.0.0.0；
 *
 * -# 2014/5/13  新增加ExtI2cWrite和ExtI2cRead函数接口用于外部扩展的I2C读写；\n
 *    		    DTCCM2APIGuide版本为：1.0.2.2；
 *
 * -# 2014/5/15  归类整理，将EEPROM这个章节整合到设备信息章节；\n
 *			    DTCCM2APIGuide版本为：1.0.3.3；
 *
 * -# 2018/10/25 注释整理;\n
 *			    DTCCM2APIGuide版本为：1.0.4.4；
 */

/* 定义默认打开的设备ID */
#define DEFAULT_DEV_ID 0

/******************************************************************************
设备信息相关
******************************************************************************/
/** @defgroup group1 设备信息相关
@{

*/

/// @brief 获取本类的版本编号
///
/// @param Version：本类的版本号
///
/// @retval DT_ERROR_OK：获取本类的版本号成功
DTCCM_API int _DTCALL_ GetLibVersion(DWORD Version[4], int iDevID = DEFAULT_DEV_ID);

/// @brief 获取dll的版本编号(不需要连接设备即可使用),有dtccm2_legacy.dll动态库时返回的是dtccm2_legacy.dll的版本号，无dtccm2_legacy.dll,返回的是dtccm2.dll版本号
///
/// @param Version：dll的版本号
///
/// @retval DT_ERROR_OK：获取dll的版本号成功
DTCCM_API int _DTCALL_ GetDllVersion(DWORD Version[4], int iDevID = DEFAULT_DEV_ID);

/* @example 获取动态库版本号
	// 不需要连接设备即可使用
	int iRet;
	char szInfoBuf[256];
	iRet = GetDriverDllVersion(szInfoBuf,256);
	if (iRet == DT_ERROR_OK)
	{
		msg(...)		// 打印到状态栏
	}
*/
/// @brief 获取所有动态库的版本号，以字符串形式给用户
///
/// @param pszInfoBuf：返回的各dll的版本号字符串，返回字符串格式：dtccm2.dll：x.x.x.x;dt_p.dtdev64.dll:x.x.x.x;dtccm2_legacy.dll:x.x.x.x;
/// @param uBufferSize：装载字符串的buffer大小
///
/// @retval DT_ERROR_OK：获取成功
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数越界，装载字符串的buffer大小不够
DTCCM_API int _DTCALL_ GetDriverDllVersion(char *pszInfoBuf, UINT uBufferSize);

/// @brief 枚举设备，获得设备名及设备个数
///
/// @param DeviceName：枚举的设备名
/// @param iDeviceNumMax：指定枚举设备的最大个数
/// @param pDeviceNum：枚举的设备个数
///
/// @retval DT_ERROR_OK：枚举操作成功
/// @retval DT_ERROR_FAILED：枚举操作失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
///
/// @note 获取的设备名称字符串需要用户程序调用GlobalFree()逐个释放。
DTCCM_API int _DTCALL_ EnumerateDevice(char *DeviceName[], int iDeviceNumMax, int *pDeviceNum);

/** @name GetFwVersion参数iChip定义
@{
*/
typedef enum
{
	FIRMWARE_BIN2 = 0,
	FIRMWARE_BIN1 = 1,
	FIRMWARE_BIN3 = 2,
	FIRMWARE_BIN4 = 3,
	FIRMWARE_BIN5 = 4,
	FIRMWARE_BIN6 = 5,
	FIRMWARE_BIN7 = 6,
	FIRMWARE_BIN8 = 7
} FirmwareBinType;
/** @} */
/// @brief 获取固件版本,iChip指定哪个芯片的固件,0是bin2版本号，1是bin1版本号
///
/// @param iChip：芯片编号，参考FirmwareBinType定义
/// @param Version：固件的版本号
///
/// @retval DT_ERROR_OK：获取指定的芯片固件版本号成功
/// @retval DT_ERROR_FAILED：获取指定的芯片固件版本号失败
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：无指定的芯片固件编号
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_TIME_OUT：获取版本号超时
DTCCM_API int _DTCALL_ GetFwVersion(int iChip, DWORD Version[4], int iDevID = DEFAULT_DEV_ID);

/// @brief 获取设备的唯一序列号
///
/// @param pSN：返回的设备唯一序列号
/// @param iBufferSize：设置要获取序列号字节的长度,最大支持32字节
/// @param pRetLen：返回实际设备序列号字节长度
///
/// @retval DT_ERROR_OK：获取设备的序列号成功
/// @retval DT_ERROR_FAILED：获取设备的序列号失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetDeviceSN(BYTE *pSN, int iBufferSize, int *pRetLen, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取设备的硬件版本
///
/// @param Version：硬件版本号
///
/// @retval DT_ERROR_OK：获取设备的硬件版本号成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetHardwareVersion(DWORD Version[4], int iDevID = DEFAULT_DEV_ID);

/// @bfief 获取设备的硬件信息，可读的字符串
///
/// @param pBuf：返回的设备硬件信息
/// @param iBufferSize：设置要获取的设备硬件信息字节长度，最大16字节
/// @param pRetLen：返回的设备序列号字节长度
///
/// @retval DT_ERROR_OK：获取设备的硬件信息成功
/// @retval DT_ERROR_FAILED：获取设备的硬件信息失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetHardwareInfo(BYTE *pBuf, int iBufferSize, int *pRetLen, int iDevID = DEFAULT_DEV_ID);

/// @brief 写用户自定义名称
///
/// @param pName：用户定义的名称
/// @param iSize：用户名称字节长度，最大32字节
///
/// @retval DT_ERROR_OK：设置用户自定义名称成功
/// @retval DT_ERROR_FAILED：设置用户自定义名称失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：iSize参数越界
DTCCM_API int _DTCALL_ WriteUserDefinedName(char *pName, int iSize, int iDevID = DEFAULT_DEV_ID);

/// @brief 写用户自定义名称
///
/// @param pName：用户定义的名称
/// @param iSize：用户名称字节长度，最大32字节
/// @param bMaster：为1设置当前设备为主机，MUD952双测试盒时注意不要将两台设备都设为主机或者从机，必须一主一从
///
/// @retval DT_ERROR_OK：设置用户自定义名称成功
/// @retval DT_ERROR_FAILED：设置用户自定义名称失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：iSize参数越界
DTCCM_API int _DTCALL_ WriteUserDefinedNameEx(char *pName, int iSize, BOOL bMaster, int iDevID = DEFAULT_DEV_ID);

/// @brief 读用户自定义名称
///
/// @param pName：返回用户定义的名称
/// @param iBufferSize：设置要获取的用户名称字节长度，最大32字节
/// @param pRetLen：返回的用户名称字节长度
///
/// @retval DT_ERROR_OK：获取用户定义名称成功
/// @retval DT_ERROR_FAILED：获取用户定义名称失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ ReadUserDefinedName(char *pName, int iBufferSize, int *pRetLen, int iDevID = DEFAULT_DEV_ID);

/// @brief 判断设备是否打开
///
/// @retval DT_ERROR_OK：设备已经连接打开
/// @retval DT_ERROR_FAILED：设备没有连接成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ IsDevConnect(int iDevID = DEFAULT_DEV_ID);

/*typedef enum
{
	WORK_NORMAL = 0,				//20190314暂未支持
	WORK_OS_TEST = 1,				//20190314暂未支持
	WORK_STANDBY_CURRENT_TEST = 2	//待机电流测试模式，会把sensor IO置为高阻，并关闭上拉
}WorkMode;*/
/// @brief 设置设备进入的测试模式
///
/// @param Mode：工作模式，枚举型定义WorkMode
///
/// @retval DT_ERROR_OK:设置成功
DTCCM_API int _DTCALL_ SetWorkMode(WorkMode Mode, int iDevID = DEFAULT_DEV_ID);

/** @name GetFwVersion参数iChip定义
@{
*/
typedef enum
{
	HS128 = 0x0010,
	HS230 = 0x0020,
	HS300 = 0x0030,
	HS300D = 0x0031,
	HV910 = 0x0092,
	HV910D = 0x0093,
	HV810 = 0x0082,
	HV810D = 0x0083,

	PE300 = 0x0130,
	PE300D = 0x0131,
	PE910 = 0x0190,
	PE910D = 0x0191,
	PE810 = 0x0180,
	PE810D = 0x0181,
	PE350 = 0x0132,
	PE950 = 0x0192,
	MP950 = 0x0193,

	UT300 = 0x0231,
	UO300 = 0x0232,
	UM330 = 0x0233,
	UV910 = 0x0292,
	UH910 = 0x0293,
	DTLC2 = 0x02A1,
	UF920 = 0x0295,
	MU950 = 0x0296,
	DMU956 = 0x0297,
	ULV330 = 0x0239,
	CMU958 = 0x0298,
	ULV913 = 0x0299,
	ULV966 = 0x029a,
	ULM928 = 0x029b,
	CMU970 = 0x029e,
	UH920 = 0x0294,
	KM25 = 0x0390,
	K25D = 0x0391,
	K20C = 0x0392,
	K15D = 0x0393,
	K27H = 0x0394,
	MU950_TOF = 0x02a8,

	CTG970 = 0x0490,
	G40 = 0x0491,
	G41 = 0x0492,
	G42 = 0x0493,
	G22 = 0x0494,
	G22F = 0x0498,
	G42L = 0x0495,
	G22L = 0x0496,
	GSL10 = 0x0499,
	GQ4 = 0x049b,
	GQ4C = 0x049e,
	F24Pro = 0x049f,
	F24 = 0x049d,
	F22 = 0x04a0,
	F22Pro = 0x04a1,
	GQ2 = 0x04a3,
	GQ2A = 0x04a4,
	GSL20 = 0x04a5,
	CP20 = 0x04a6,
	GQ4Y = 0x04a8,

	MU960 = 0x0590,
	MU960Pro = 0x0591,
	MU950A = 0x0593,
	MU360 = 0x0534
} DeviceModel;
/** @} */
/// @brief 返回设备的型号，区分不同的测试板，参考枚举型定义"DeviceType"
///
/// @retval 0x0010：HS128测试板
/// @retval 0x0020：HS230测试板
/// @retval 0x0030：HS300测试板
/// @retval 0x0031：HS300D测试板
/// @retval 0x0092：HV910测试板
/// @retval 0x0093：HV910D测试板
/// @retval 0x0082：HV810测试板
/// @retval 0x0083：HV810D测试板
///
/// @retval 0x0130：PE300测试板
/// @retval 0x0131：PE300D测试板
/// @retval 0x0190：PE910测试板
///	@retval 0x0191：PE910D测试板
/// @retval 0x0180：PE810测试板
///	@retval 0x0181：PE810D测试板
/// @retval 0x0132：PE350测试板
/// @retval 0x0192：PE950测试板
/// @retval 0x0193：MP950测试板
///
///	@retval 0x0231：UT300测试板
/// @retval 0x0232：UO300测试板
/// @retval 0x0233：UM330测试板
///	@retval 0x0292：UV910测试板
///	@retval 0x0293：UH910测试板
///	@retval 0x02A1：DTLC2测试板
/// @retval 0x0295：UF920测试板
/// @retval 0x0295：UM900测试板
/// @retval 0x0296：MU950测试板
/// @retval 0x0297：DMU956测试板
/// @retval 0x0239：ULV330测试板
/// @retval 0x0298：CMU958测试板
/// @retval 0x0299：ULV913测试板
/// @retval 0x029a：ULV966测试板
/// @retval 0x029b：ULM928测试板
/// @retval 0x029e：CMU970测试板
///	@retval 0x0294：UH920测试板
/// @retval 0x0390：KM25测试板
/// @retval 0x0391：K25D测试板
/// @retval 0x0392：K20C测试板
/// @retval 0x0393：K15D测试板
///
/// @retval 0x0490：CTG970测试板
/// @retavl 0x0491：G40测试板
/// @retval 0x0492：G41测试板
/// @retval 0x0493：G42测试板
/// @retval 0x0494：G22测试板
/// @retval 0x0499：GSL10测试板
/// @retval 0x049b: GQ4测试板
/// @retval 0x049E: GQ4C测试板
///
/// @retval 0x0590: MU960测试板

/// @note 没有写在上面的设备名称无此功能。
DTCCM_API DWORD _DTCALL_ GetKitType(int iDevID = DEFAULT_DEV_ID);

/// @brief 重启设备，此函数调用后，设备会断电，重新启动(不是所有机型都支持，\n
/// KM25/K15D/CMU970/CTG970之后的机型支持)
DTCCM_API int _DTCALL_ SystemRestart(int iDevID = DEFAULT_DEV_ID);

/*
	typedef enum
	{
	USB2_0 = 0,
	USB3_0 = 1,
	USB3_1 = 2,

	FIBRE = 20		/// 光纤产品
	}DevLinkType;

	typedef struct DevLinkStatus_s
	{
		BOOL			bLinkOk;		///< 指示当前link是否正常(已经连接设备)
		double			lfLinkRate;		///< Link 速度，单位Mbps
		DevLinkType		LinkType;		///< Link 类型，
		ULONG			Rsv[32];
	}DevLinkStatus_t;
*/

/// @brief 查询测试盒传输接口状态
DTCCM_API int _DTCALL_ GetLinkStatus(DevLinkStatus_t *pStatus, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取MIPI状态信息
DTCCM_API int _DTCALL_ GetMipiStatusInfo(MipiStatusInfo_t *pStatus, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取设备信息
///
/// @param pInfo：设备信息结构体
///
/// @retval DT_ERROR_OK：获取信息成功
/// @retval DT_ERROR_FAILED：获取信息失败
DTCCM_API int _DTCALL_ GetDeviceInfo(DevInfo_t *pInfo, int iDevID = DEFAULT_DEV_ID);
/******************************************************************************
EEPROM相关
******************************************************************************/

/// @brief 从EEPROM读一个字
///
/// @param uAddr：EEPROM的寄存器地址
/// @param pValue：向EEPROM读到字
///
/// @retval DT_ERROR_OK：读EEPROM成功
/// @retval DT_ERROR_FAILD：读EEPROM失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ ReadWordFromEEProm(USHORT uAddr, USHORT *pValue, int iDevID = DEFAULT_DEV_ID);

/** @} */ // end of group1

/******************************************************************************
ISP相关
******************************************************************************/
/** @defgroup group2 ISP相关


* @{

*/

/// @brief 获取GAMMA设置值
///
/// @param pGamma：返回的GAMMA设置值
DTCCM_API int _DTCALL_ GetGamma(int *pGamma, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置GAMMA值
///
/// @param iGamma：设置的GAMMA值
DTCCM_API int _DTCALL_ SetGamma(int iGamma, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取对比度设置值
///
/// @param pContrast：返回的对比度设置值
DTCCM_API int _DTCALL_ GetContrast(int *pContrast, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置对比度
///
/// @param iContrast：设置对比度值
DTCCM_API int _DTCALL_ SetContrast(int iContrast, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取饱和度设置值
///
/// @param pSaturation：返回的饱和度设置值
DTCCM_API int _DTCALL_ GetSaturation(int *pSaturation, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置饱和度
///
/// @param iSaturation：设置饱和度值
DTCCM_API int _DTCALL_ SetSaturation(int iSaturation, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置锐度
///
/// @param iSharpness：设置锐度值
DTCCM_API int _DTCALL_ SetSharpness(int iSharpness, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取锐度设置值
///
/// @param pSharpness：返回的锐度设置值
DTCCM_API int _DTCALL_ GetSharpness(int *pSharpness, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置噪声
///
/// @param iLevel：设置噪声值
DTCCM_API int _DTCALL_ SetNoiseReduction(int iLevel, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取设置噪声值
///
/// @param pLevel：返回设置的噪声值
DTCCM_API int _DTCALL_ GetNoiseReduction(int *pLevel, int iDevID = DEFAULT_DEV_ID);

/// @brief 开启或关闭消死点
///
/// @param bDeadPixCleanEn：为TRUE开启消死点，为FALSE关闭消死点
DTCCM_API int _DTCALL_ SetDeadPixelsClean(BOOL bDeadPixCleanEn, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取消死点开启或关闭状态
///
/// @param pbDeadPixCleanEn：返回消死点消死点开启或关闭状态
DTCCM_API int _DTCALL_ GetDeadPixelsClean(BOOL *pbDeadPixCleanEn, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置彩点阀值
///
/// @param iHotCpth：设置的彩点阀值，大于阀值的像素点判断为彩点
DTCCM_API int _DTCALL_ SetHotCpth(int iHotCpth, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取设置的彩点阀值
///
/// @param pHotCpth：返回的彩点阀值设置值
DTCCM_API int _DTCALL_ GetHotCpth(int *pHotCpth, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置暗点阀值
///
/// @param iDeadCpth：设置的暗点阀值，小于阀值的像素点判断为暗点
DTCCM_API int _DTCALL_ SetDeadCpth(int iDeadCpth, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取设置的暗点阀值
///
/// @param pHotCpth：返回的暗点阀值设置值
DTCCM_API int _DTCALL_ GetDeadCpth(int *pDeadCpth, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置raw转rgb算法
///
/// @param Algrithm：ram转rgb算法设置值,参见宏定义“RAW转RGB算法定义”
DTCCM_API int _DTCALL_ SetRaw2RgbAlgorithm(BYTE Algrithm, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取raw转rgb算法设置值
///
/// @param 返回raw转rgb算法设置值
DTCCM_API int _DTCALL_ GetRaw2RgbAlgorithm(BYTE *pAlgrithm, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置RGB数字增益
///
/// @param fRGain：R增益值
/// @param fGGain：G增益值
/// @param fBGain：B增益值
DTCCM_API int _DTCALL_ SetDigitalGain(float fRGain, float fGGain, float fBGain, int iDevID = DEFAULT_DEV_ID);

/// @brief 开启或关闭色彩矩阵
///
/// @param bEnable：为TRUE开启矩阵，为FALSE关闭矩阵
DTCCM_API int _DTCALL_ SetMatrixEnable(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取RGB数字增益
///
/// @param pRGain：返回的R数字增益值
/// @param pGGain：返回的G数字增益值
/// @param pBGain：返回的B数字增益值
DTCCM_API int _DTCALL_ GetDigitalGain(float *pRGain, float *pGGain, float *pBGain, int iDevID = DEFAULT_DEV_ID);

/// @brief 开启或关闭白平衡
///
/// @param bAWBEn：白平衡使能，为TRUE开启白平衡，为FALSE关闭白平衡
DTCCM_API int _DTCALL_ SetAWB(BOOL bAWBEn, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取白平衡开启状态
///
/// @param pAwb：返回的白平衡状态
DTCCM_API int _DTCALL_ GetAWB(BOOL *pAwb, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置颜色矩阵
///
/// @param Matrix：设置的颜色矩阵
DTCCM_API int _DTCALL_ SetMatrixData(float Matrix[3][3], int iDevID = DEFAULT_DEV_ID);

/// @brief 获取颜色矩阵
///
/// @param Matrix：返回的颜色矩阵
DTCCM_API int _DTCALL_ GetMatrixData(float Matrix[3][3], int iDevID = DEFAULT_DEV_ID);

/// @brief 开启或关闭MONO模式
///
/// @param bEnable：MONO模式使能，为TRUE开启MONO模式，为FALSE关闭MONO模式
DTCCM_API int _DTCALL_ SetMonoMode(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 开启或关闭图像水平反向
///
/// @param bEnable：图像水平反向使能，为TRUE开启水平反向，为FALSE关闭水平反向
DTCCM_API int _DTCALL_ SetHFlip(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 开启或关闭图像垂直反向
///
/// @param bEnable：图像垂直反向使能，为TRUE开启垂直反向，为FALSE关闭垂直反向
DTCCM_API int _DTCALL_ SetVFlip(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 开启或关闭“十”字线。
///
/// @param bEnable：“十”字线使能，为TRUE开启十字线，为FALSE关闭十字线
DTCCM_API int _DTCALL_ SetCrossOn(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

// RGB24 to YUV422 (YcbYcr mode)
// pBmp24 :RGB24 data pointer;
// pOut   :YUV422 pointer
// width  :Image width;
// height :Image height;

/// @brief RGB24转YUV422
///
/// @param pBmp24：RGB24数据
/// @param pOut：YUV422数据
/// @param width：图像数据宽度
/// @param height：图像数据高度
DTCCM_API int _DTCALL_ RGB24ToYUV422(BYTE *pBmp24, BYTE *pOut, USHORT uWidth, USHORT uHeight, int iDevID = DEFAULT_DEV_ID);

// RGB24 to YUV444 (Ycbcr )
// pBmp24 :RGB24 data pointer;
// pOut   :YUV422 pointer
// width  :Image width;
// height :Image height;

/// @brief RGB24转YUV444
///
/// @param pBmp24：RGB24数据
/// @param pOut：YUV422数据
/// @param width：图像数据宽度
/// @param height：图像数据高度
DTCCM_API int _DTCALL_ RGB24ToYUV444(BYTE *pBmp24, BYTE *pOut, USHORT uWidth, USHORT uHeight, int iDevID = DEFAULT_DEV_ID);

/// @brief 显示RGB图像数据，使用该接口，必须先调用InitDisplay或InitDevice初始化窗口句柄
///
/// @param pBmp24：待显示的RGB24格式的数据
/// @param pInfo：帧信息，参见结构体“FrameInfo”
///
/// @retval DT_ERROR_OK：显示RGB图像成功
/// @retval DT_ERROR_FAILD：显示RGB图像失败
DTCCM_API int _DTCALL_ DisplayRGB24(BYTE *pBmp24, FrameInfo *pInfo = NULL, int iDevID = DEFAULT_DEV_ID);

/// @brief 显示RGB图像数据
///
/// @param pBmp24：待显示的RGB24格式的数据
/// @param pInfo：帧信息，参见结构体“FrameInfoEx”
///
/// @retval DT_ERROR_OK：显示RGB图像成功
/// @retval DT_ERROR_FAILD：显示RGB图像失败
DTCCM_API int _DTCALL_ DisplayRGB24Ex(BYTE *pBmp24, FrameInfoEx *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief RAW/YUV转RGB，源数据格式由byImgFormat指定
///
/// @param pIn：源图像数据
/// @param pOut：转为RGB24的数据
/// @param uWidth：图像数据宽度
/// @param uHeight：图像数据高度
/// @param byImgFormat：源数据的格式
DTCCM_API int _DTCALL_ DataToRGB24(BYTE *pIn, BYTE *pOut, USHORT uWidth, USHORT uHeight, BYTE byImgFormat, int iDevID = DEFAULT_DEV_ID);

/// @brief RAW/YUV转RGB，源数据格式由byImgFormat指定。(双通道产品UH920使用时要指定通道)
///
/// @param pIn：源数据
/// @param pOut：转为RGB24的数据
/// @param uWidth：图像数据宽度
/// @param uHeight：图像数据高度
/// @param byImgFormat：源数据的格式
/// @param byChannel: 指定通道
DTCCM_API int _DTCALL_ DataToRGB24Ex(BYTE *pIn, BYTE *pOut, USHORT uWidth, USHORT uHeight, BYTE byImgFormat, BYTE byChannel, int iDevID = DEFAULT_DEV_ID);

/// @brief 显示RAW/YUV图像数据，源数据格式由Init函数中的byImgFormat参数指定
///
/// @param pData：待显示的图像数据
/// @param pInfo：帧信息，参见结构体“FrameInfo”
///
/// @retval DT_ERROR_OK：显示图像成功
/// @retval DT_ERROR_FAILD：显示图像失败
/// @retval DT_ERROR_NOT_INITIALIZED：没有初始化
DTCCM_API int _DTCALL_ DisplayVideo(BYTE *pData, FrameInfo *pInfo = NULL, int iDevID = DEFAULT_DEV_ID);

/// @brief 显示RAW/YUV图像数据，源数据格式由Init函数中的byImgFormat参数指定
///
/// @param pData：待显示的图像数据
/// @param pInfo：帧信息，参见结构体“FrameInfoEx”
///
/// @retval DT_ERROR_OK：显示图像成功
/// @retval DT_ERROR_FAILD：显示图像失败
/// @retval DT_ERROR_NOT_INITIALIZED：没有初始化
DTCCM_API int _DTCALL_ DisplayVideoEx(BYTE *pData, FrameInfoEx *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief 支持raw10，raw12，raw16，YUV数据转RGB，对RAW图像数据进行图像处理(MONO,WB,ColorChange,Gamma,Contrast)
///
/// @param pImage：RAW图像数据
/// @param pBmp24：经过图像处理后的数据
/// @param uWidth：图像数据宽度
/// @param uHeight：图像数据高度
/// @param pInfo：帧信息，参见结构体“FrameInfo”
///
/// @retval DT_ERROR_OK：图像处理成功
/// @retval DT_ERROR_PARAMETER_INVALID：pData无效的参数
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ImageProcess(BYTE *pImage, BYTE *pBmp24, int nWidth, int nHeight, FrameInfo *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief 支持raw10，raw12，raw16，YUV数据转RGB，对RAW图像数据进行图像处理(MONO,WB,ColorChange,Gamma,Contrast)
///
/// @param pImage：RAW图像数据
/// @param pBmp24：经过图像处理后的数据
/// @param uWidth：图像数据宽度
/// @param uHeight：图像数据高度
/// @param pInfo：帧信息，参见结构体“FrameInfoEx”
///
/// @retval DT_ERROR_OK：图像处理成功
/// @retval DT_ERROR_PARAMETER_INVALID：pData无效的参数
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ImageProcessEx(BYTE *pImage, BYTE *pBmp24, int nWidth, int nHeight, FrameInfoEx *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置RAW格式，参见枚举类型“RAW_FORMAT”
///
/// @param byRawMode：RAW格式设置
///
/// @see RAW_FORMAT
DTCCM_API int _DTCALL_ SetRawFormat(BYTE byRawMode, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置YUV格式，参见枚举类型“YUV_FORMAT”
///
/// @param byYuvMode：YUV格式设置
///
/// @see YUV_FORMAT
DTCCM_API int _DTCALL_ SetYUV422Format(BYTE byYuvMode, int iDevID = DEFAULT_DEV_ID);

/// @brief 采集图像目标格式设置，raw10，raw8，rgb24等
///
/// @param byTargetFormat:采集图像目标格式设置，目标格式是Grabframe接口最终提交给用户的图像数据格式。
DTCCM_API int _DTCALL_ SetTargetFormat(BYTE byTargetFormat, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置YUV转RGB时的一些系数
///
///	@param Y=(Y-yoff)*ycoef
///	@param r=Y+(cr-128)*bdif
///	@param g=Y-cbcoef*(cb-128)-crcoef*(cr-128)
///	@param b=Y+(cb-128)*rdif
DTCCM_API int _DTCALL_ SetYuvCoefficient(int yoff,
										 float ycoef,
										 float cbcoef,
										 float crcoef,
										 float rdif,
										 float bdif,
										 int iDevID = DEFAULT_DEV_ID);

/// @brief 初始化显示，支持2个窗口显示，如果使用2个sensor，须要使用hWndEx指定第二个窗口。InitDevice接口涵盖InitDisplay，SetSensorPort,InitIsp三个接口功能
///
/// @param hWnd：显示A通道图像的窗口句柄
/// @param uImgWidth：图像数据宽度,单位像素
/// @param uHeight：图像数据高度
/// @param byImgFormat：图像数据格式Sensor输出的原始图像格式，如：RAW/YUV
/// @param hWndEx：hWndEx：显示B通道图像的窗口句柄
DTCCM_API int _DTCALL_ InitDisplay(HWND hWnd,
								   USHORT uImgWidth,
								   USHORT uImgHeight,
								   BYTE byImgFormat,
								   BYTE byChannel = CHANNEL_A,
								   HWND hWndEx = NULL,
								   int iDevID = DEFAULT_DEV_ID);

/// 不支持
DTCCM_API int _DTCALL_ GrabDataToRaw8(BYTE *pIn, BYTE *pOut, int uWidth, int uHeight, BYTE ImgFormat, int iDevID = DEFAULT_DEV_ID);

/// 不支持
DTCCM_API int _DTCALL_ GrabDataToRaw16(BYTE *pIn, USHORT *pOut, int uWidth, int uHeight, BYTE ImgFormat, int iDevID = DEFAULT_DEV_ID);

/// @brief 对RAW图像数据进行图像处理(MONO,WB,ColorChange,Gamma,Contrast)
///
/// @param pImage：RAW图像数据：
/// @param pBmp24：经过图像处理后的数据
/// @param uWidth：图像数据宽度，单位：像素
/// @param uHeight：图像数据高度
/// @param pInfo：帧信息，参见结构体“FrameInfo”
///
/// @retval DT_ERROR_OK：图像处理成功
/// @retval DT_ERROR_PARAMETER_INVALID：pData无效的参数
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ImageProcessForRaw8(BYTE *pImage, BYTE *pBmp24, int nWidth, int nHeight, FrameInfo *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief 原始图像数据格式转换
///
/// @param srcImg：原始图像数据结构体描述，DtImage_t结构体说明参见"ImageKit.h"
/// @param desImg：目标图像数据结构体描述，DtImage_t结构体说明参见"ImageKit.h"
/// @param roi[]：需要转换的图像区域坐标和像素值设置，DtRoi_t结构体说明参见"ImageKit.h"
///
/// @retval DT_ERROR_PARAMETER_INVALID：参数无效，一般是srcImg或destImg参数设置不正确
/// @retval DT_ERROR_FUNCTION_INVALID：不支持该格式的转换
/// @retval DT_ERROR_OK：转换成功
DTCCM_API int _DTCALL_ ImageTransform(DtImage_t *srcImg, DtImage_t *destImg, DtRoi_t roi[], int roiCount, int iDevID = DEFAULT_DEV_ID);

/** @} */ // end of group2

/******************************************************************************
SENSOR相关
*******************************************************************************/
/** @defgroup group3 SENSOR相关


* @{

*/

/// @brief 总线设置,进入I3C模式必须通过设置I3C_SDR_BUS进入，如果进入了I3C模式，想用I2C模式，也需要通过该函数配置I2C_BUS
DTCCM_API int _DTCALL_ SetSensorBusType(SensorBusType type, int iDevID = DEFAULT_DEV_ID);

/*

example:
// 如下器件地址为0x18，动态分配地址为0x10
I3CConfig_t sI3cCfg;
BYTE byData[2];
byData[0]= 0x10;
sI3cCfg.byStaticAddr = 0x18;
sI3cCfg.uCmd = I3C_ASSIGN_DYNAMIC_ADDRESS_FROM_STATIC;
sI3cCfg.uCmdSize = 1;
sI3cCfg.pData = &byData[0];
sI3cCfg.uSize = 1;
iRet = SetSensorI3cConfig(&sI3cCfg, m_nDevID);
if (iRet != DT_ERROR_OK)
{
msg("SetSensorI3cConfig assign dyn addr failed!\r\n");
}

*/
/// @brief I3C命令设置
///
/// @param sI3CCmd:控制命令
DTCCM_API int _DTCALL_ SetSensorI3cConfig(I3CConfig_t *pI3CConfig, int iDevID = DEFAULT_DEV_ID);

/// @brief sensor 获取I3C配置
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
DTCCM_API int _DTCALL_ GetSensorI3cConfig(I3CConfig_t *pI3cConfig, int iDevID = DEFAULT_DEV_ID);

/// @brief sensor I3C配置
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
// DTCCM_API int _DTCALL_ SetSensorI3cConfig(I3cConfig_t *pI3cConfig, int iDevID = DEFAULT_DEV_ID);

/// @brief 复位与Sensor通讯的I2C总线
///
/// @retval DT_ERROR_OK：复位I2C操作成功
/// @retval DT_ERROR_FAILED：复位I2C操作失败
DTCCM_API int _DTCALL_ ResetSensorI2cBus(int iDevID = DEFAULT_DEV_ID);

/// @brief 设置I2C的字节间隔
///
/// @brief uInterval：字节间隔设置,单位us
DTCCM_API int _DTCALL_ SetI2CInterval(UINT uInterval, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置I2C通讯ACK超时等待时间
///
/// @param nUsWait：设置ACK超时等待时间，单位us
///
/// @retval DT_ERROR_OK：设置I2C通讯ACK超时等待时间成功
DTCCM_API int _DTCALL_ SetSensorI2cAckWait(UINT uAckWait, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置与SENSOR通讯的I2C总线速率，400Kbps或100Kbps
///
/// @param b400K：b400K=TURE，400Kbps；b400K=FALSE,100Kbps
///
/// @retval DT_ERROR_OK：设置总线速率操作成功
DTCCM_API int _DTCALL_ SetSensorI2cRate(BOOL b400K, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置与SENSOR通讯的I2C总线速率，范围10Khz-2Mhz
///
/// @param uKpbs：设置I2C总线速率，范围值为10-2000
///
/// @retval DT_ERROR_OK：设置总线速率操作成功
DTCCM_API int _DTCALL_ SetSensorI2cRateEx(UINT uKpbs, int iDevID = DEFAULT_DEV_ID);

/// @brief 使能与SENSOR通讯的I2C总线为Rapid模式
///
/// @param  bRapid=1表示，强制灌电流输出高电平;=0，I2C管脚为输入状态，借助外部上拉变成高电平
///
/// @retval DT_ERROR_OK：设置I2C总线Rapid模式成功
DTCCM_API int _DTCALL_ SetSensorI2cRapid(BOOL bRapid, int iDevID = DEFAULT_DEV_ID);

/// @brief sensor i2c外部上拉电阻选择(CMU958PLUS/DMU927P/CMU959P/KM25/CMU970/G系列支持)
///
/// @param byPullupResistorSel=PULLUP_RESISTOR_1_5K,表示1.5K；=PULLUP_RESISTOR_4_7K，表示4.7K；=PULLUP_RESISTOR_0_56K表示0.56K
/// byPullupResistorSel = PULLUP_RESISTOR_1_14K,表示1.14K；
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_PARAMETER_INVALID：byPullupResistorSel参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
DTCCM_API int _DTCALL_ SetSensorI2cPullupResistor(BYTE byPullupResistorSel, int iDevID = DEFAULT_DEV_ID);

/// @brief 忽略sensor i2c 写的过程的ACK，无ACK应答，也会继续发送下一个命令(仅UF920/UV910/PE950支持)
///
/// @param bIgnoreAck=TRUE,忽略ACK；=FALSE,表示不忽略ACK
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
/// @retval DT_ERROR_TIME_OUT：通讯超时
DTCCM_API int _DTCALL_ SetI2cIgnoreAck(BOOL bIgnoreAck, int iDevID = DEFAULT_DEV_ID);

/// @brief 不发送sensor i2c 写的过程的stop命令(仅PE系列支持)
///
/// @param bNoStop=TRUE,不发送stop；=FALSE,发送stop
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
/// @retval DT_ERROR_TIME_OUT：通讯超时
DTCCM_API int _DTCALL_ SetSensorI2cWrNoStop(BOOL bNoStop, int iDevID = DEFAULT_DEV_ID);

/// @brief 读i2c命令，I2C stop与I2C start之间的间隔设置
///
/// @param uInterval：设置间隔，单位us
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
/// @retval DT_ERROR_TIME_OUT：通讯超时
DTCCM_API int _DTCALL_ SetReadSensorI2cInterval(UINT uInterval, int iDevID = DEFAULT_DEV_ID);

/*enum I2CMODE
{
	I2CMODE_NORMAL=0,		///< 8 bit addr,8 bit value
	I2CMODE_SAMSUNG,		///< 8 bit addr,8 bit value,Stopen
	I2CMODE_MICRON,			///< 8 bit addr,16 bit value
	I2CMODE_STMICRO,		///< 16 bit addr,8 bit value, (eeprom also)
	I2CMODE_MICRON2,		///< 16 bit addr,16 bit value
};*/

/// @brief 写SENSOR寄存器,I2C通讯模式byI2cMode的设置值见I2CMODE定义
///
/// @param uAddr：从器件地址
/// @param uReg：寄存器地址
/// @param uValue：写入寄存器的值
/// @param byMode：I2C模式
///
/// @retval DT_ERROR_OK：写SENSOR寄存器操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：byMode参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
///
/// @see I2CMODE
DTCCM_API int _DTCALL_ WriteSensorReg(UCHAR uAddr, USHORT uReg, USHORT uValue, BYTE byMode, int iDevID = DEFAULT_DEV_ID);

/// @brief 读SESNOR寄存器,I2C通讯模式byI2cMode的设置值见I2CMODE定义
///
/// @param uAddr：从器件地址
/// @param uReg：寄存器地址
/// @param pValue：读到的寄存器的值
/// @param byMode：I2C模式
///
/// @retval DT_ERROR_OK：读SENSOR寄存器操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：byMode参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
///
/// @see I2CMODE
DTCCM_API int _DTCALL_ ReadSensorReg(UCHAR uAddr, USHORT uReg, USHORT *pValue, BYTE byMode, int iDevID = DEFAULT_DEV_ID);

/// @brief 写SENSOR寄存器，支持向多个寄存器写入数据
///
/// @param uAddr：从器件地址
/// @param byI2cMode：I2C模式
/// @param uRegNum：写入寄存器个数(注意：早期版本(2019/1/26之前)只支持15个)
/// @param RegAddr[]：寄存器地址数组
/// @param RegData[]：写入寄存器的数据
/// @param RegDelay[]：写完一组寄存器与下一组寄存器之间的延时,最大65ms，单位us
///
/// @retval DT_ERROR_OK：完成写SENSOR寄存器操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ WriteSensorMultiRegsWithDelay(UCHAR uAddr, BYTE byI2cMode, USHORT uRegNum, USHORT RegAddr[], USHORT RegData[], USHORT RegDelay[], int iDevID = DEFAULT_DEV_ID);

/// @brief 读SENSOR寄存器，支持向多个寄存器读出数据
///
/// @param uAddr：从器件地址
/// @param byI2cMode：I2C模式
/// @param uRegNum：读取寄存器个数(注意：早期版本(2019/1/26之前)只支持15个)
/// @param RegAddr[]：寄存器地址数组
/// @param RegData[]：读出寄存器的数据
/// @retval DT_ERROR_OK：完成写SENSOR寄存器操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ReadSensorMultiRegs(UCHAR uAddr, BYTE byI2cMode, USHORT uRegNum, USHORT RegAddr[], USHORT RegData[], int iDevID = DEFAULT_DEV_ID);

/// @brief 写SENSOR寄存器，支持向一个寄存器写入一个数据块
///
/// @param uDevAddr：从器件地址
/// @param uRegAddr：寄存器地址
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：写入寄存器的数据块
/// @param uSize：写入寄存器的数据块的字节数
///
/// @retval DT_ERROR_OK：完成写SENSOR寄存器块操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ WriteSensorI2c(UCHAR uDevAddr, USHORT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, int iDevID = DEFAULT_DEV_ID);

/// @brief 读SENSOR寄存器，支持向一个寄存器读出一个数据块
///
/// @param uDevAddr：从器件地址
/// @param uRegAddr：寄存器地址
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：读到寄存器的值
/// @param uSize：读出寄存器的数据块的字节数
/// @param bNoStop：是否发出I2C的STOP命令，一般情况下默认为FALSE，bNoStop=TRUE表示写阶段不会有I2C的停止命令，直接进入读阶段，bNoStop=FALSE有I2C的停止命令，再进入读阶段
///
/// @retval DT_ERROR_OK：完成读SENSOR寄存器块操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ReadSensorI2c(UCHAR uDevAddr, USHORT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, BOOL bNoStop = FALSE, int iDevID = DEFAULT_DEV_ID);

/// @brief 写SENSOR寄存器，支持向一个寄存器写入一个数据块,寄存器地址最大支持4个字节，WriteSensorI2c最大只支持2个字节
///
/// @param uDevAddr：从器件地址
/// @param uRegAddr：寄存器地址
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：写入寄存器的数据块
/// @param uSize：写入寄存器的数据块的字节数
///
/// @retval DT_ERROR_OK：完成写SENSOR寄存器块操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ WriteSensorI2cEx(UCHAR uDevAddr, UINT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, int iDevID = DEFAULT_DEV_ID);

/// @brief 读SENSOR寄存器，支持向一个寄存器读出一个数据块,寄存器地址最大支持4个字节，ReadSensorI2c最大只支持2个字节
///
/// @param uDevAddr：从器件地址
/// @param uRegAddr：寄存器地址
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：读到寄存器的值
/// @param uSize：读出寄存器的数据块的字节数
/// @param bNoStop：是否发出I2C的STOP命令，一般情况下默认为FALSE，bNoStop=TRUE表示写的过程不会有I2C的停止命令，bNoStop=FALSE有I2C的停止命令
///
/// @retval DT_ERROR_OK：完成读SENSOR寄存器块操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ReadSensorI2cEx(UCHAR uDevAddr, UINT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, BOOL bNoStop = FALSE, int iDevID = DEFAULT_DEV_ID);

/// @brief 读SENSOR I2C,寄存器字节个数不局限于4个
///
/// @param uDevAddr：从器件地址
/// @param pRegAddr：寄存器地址块
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：读到寄存器的值
/// @param uSize：读出寄存器的数据块的字节数
/// @param bNoStop：是否发出I2C的STOP命令，一般情况下默认为FALSE，bNoStop=TRUE表示写的过程不会有I2C的停止命令，bNoStop=FALSE有I2C的停止命令
///
/// @retval DT_ERROR_OK：完成读SENSOR寄存器块操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ReadSensorI2cMultRegAndData(UCHAR uDevAddr, BYTE *pRegAddr, USHORT uRegAddrSize, BYTE *pData, USHORT uSize, BOOL bNoStop = FALSE, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置SENSOR的限流值，100mA或300mA，老机型有的功能，建议用户使用PmuSetOcpCurrentLimit函数
///
/// @param iLimit：电流值（只能设置为100或者300）
///
/// @retval DT_ERROR_OK：设置电流值成功
/// @retval DT_ERROR_FAILED：设置电流值失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
///
/// @note 此函数功能仅HS和PE系列设置有效
DTCCM_API int _DTCALL_ SetSensorCurrentLimit(int iLimit, int iDevID = DEFAULT_DEV_ID);
/*
#define RESET_H						0x02
#define RESET_L						0x00
#define PWDN_H						0x01
#define PWDN_L						0x00
#define PWDN2_H						0x04
#define PWDN2_L						0x00
*/
/// @brief 通过Reset,PWDN管脚开启或关闭SENSOR
///
/// @param byPin：Reset，PWDN，PWDN2，位定义方式
/// @param bEnable：开启或关闭SENSOR
///
/// @retval DT_ERROR_OK：开启或关闭SENSOR操作成功
/// @retval DT_ERROR_FAILED：开启或关闭SENSOR操作失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SensorEnable(BYTE byPin, BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 检查Reset,PWDN管脚是否对SENSOR起作用(功能不能使用)
///
/// @param pInfo：SENSOR的参数信息
/// @param bRstChkEn：使能检查Reset
/// @param bPwdnChkEn：使能检查PWDN
/// @param byChannel：A/B通道选择
/// @param pErrorx：返回的Reset和PWDN的检查信息
///
/// @retval DT_ERROR_OK：操作成功
DTCCM_API int _DTCALL_ CheckRstPwdnPin(SensorTab *pInfo, BOOL bRstChkEn, BOOL bPwdnChkEn, BYTE byChannel, BYTE *pErrorx, int iDevID = DEFAULT_DEV_ID);

/// @brief 检查Reset,PWDN管脚是否对SENSOR起作用(功能不能使用)
///
/// @param pInfo：SENSOR的参数信息
/// @param bRstChkEn：使能检查Reset
/// @param bPwdnChkEn：使能检查PWDN
/// @param byChannel：A/B通道选择
/// @param pErrorx：返回的Reset和PWDN的检查信息
///
/// @retval DT_ERROR_OK：操作成功
DTCCM_API int _DTCALL_ CheckRstPwdnPin2(SensorTab2 *pInfo, BOOL bRstChkEn, BOOL bPwdnChkEn, BYTE byChannel, BYTE *pErrorx, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置SENSOR的输入时钟(6-100Mhz可设)
///
/// @param bOnOff：使能SENSOR的输入时钟，为TRUE开启输入时钟，为FALSE关闭输入时钟
/// @param uHundKhz：SENSOR的输入时钟值，单位为100Khz
///
/// @retval DT_ERROR_OK：设置SENSOR输入时钟成功
/// @retval DT_ERROR_FAILED：设置SENSOR输入时钟失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetSensorClock(BOOL bOnOff, USHORT uHundKhz, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取MIPI接口的同步时钟频率
///
/// @param pFreq：返回MIPI接口的同步时钟频率值
/// @param byChannel：A通道/B通道
///
/// @retval DT_ERROR_OK：获取MIPI接口的同步时钟频率成功
/// @retval DT_ERROR_FAILED：获取MIPI接口的同步时钟频率失败
/// @retval DT_ERROR_PARAMETER_INVALID：通道参数无效
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMipiClkFrequency(ULONG *pFreq, BYTE byChannel = CHANNEL_A, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置MIPI接口接收器时钟相位(功能不能使用)
///
/// @param byPhase：MIPI接口接收器时钟相位（可以设置的值是0-7）
///
/// @retval DT_ERROR_OK：设置MIPI接口接收器时钟相位成功
/// @retval DT_ERROR_FAILED：设置MIPI接口接收器时钟相位失败
/// @retval DT_ERROR_TIME_OUT：设置超时
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMipiClkPhase(BYTE byPhase, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取MIPI接口接收器时钟相位值(功能不能使用)
///
/// @param pPhase：返回MIPI接口接收器时钟相位值
///
/// @retval DT_ERROR_OK：获取MIPI接口接收器时钟相位值成功
/// @retval DT_ERROR_FAILED：获取MIPI接口接收器时钟相位值失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMipiClkPhase(BYTE *pPhase, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置MIPI接口控制器(KM25,K20C,K25D机型支持)
///
/// @param dwCtrl：MIPI接口控制器操作码，参见宏定义“MIPI控制器特性的位定义”
///
/// @retval DT_ERROR_OK：设置MIPI接口控制器成功
/// @retval DT_ERROR_FAILED：设置MIPI接口控制器失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMipiCtrl(DWORD dwCtrl, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取MIPI接口控制码
///
/// @param pdwCtrl：返回MIPI接口控制码
///
/// @retval DT_ERRORO_OK：获取成功
DTCCM_API int _DTCALL_ GetMipiCtrl(DWORD *pdwCtrl, int iDevID = DEFAULT_DEV_ID);

/*
DPHY:从bit0-bit9依次表示lane1_p、lane1_n、lane2_p、lane2_n、lane3_p、lane3_n、lane4_p、lane4_n、clklane_p、clklane_n
CPHY:从bit0-bit9依次表示trio3_a、trio1_b、trio1_c、trio2_c、trio1_a、trio2_a、trio3_c、nc、trio2_b、trio3_b

example:

int iRet = GetMipiLpLevel(&dwLpLevel, m_nDevID);
if (iRet != DT_ERROR_OK)
{
	msg("GetMipiLpLevel failed with error code:%d",iRet);
	return iRet;
}
lane1_p = dwLpLevel & 0x1;
lane1_n = dwLpLevel >> 2 & 0x1;
lane2_p = dwLpLevel >> 3 & 0x1;
lane2_n = dwLpLevel >> 3 & 0x1;
lane3_p = dwLpLevel >> 4 & 0x1;
lane3_n = dwLpLevel >> 5 & 0x1;
lane4_p = dwLpLevel >> 6 & 0x1;
lane4_n = dwLpLevel >> 7 & 0x1;
clklane_p = dwLpLevel >> 8 & 0x1;
clklane_n = dwLpLevel >> 9 & 0x1;
*/
/// @brief 获取MIPI LP电平值
///
/// @param pLevel：返回MIPI LP管脚的电平值
///
/// @retval DT_ERROR_OK：获取成功
/// @retval DT_ERROR_FAILED：获取失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMipiLpLevel(DWORD *pLevel, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置MIPI LP电平判定阈值
///
/// @param fThreshold：阈值电平值，单位V，约10mV可调步进值
///
///
/// @retval DT_ERROR_OK：获取成功
/// @retval DT_ERROR_FAILED：获取失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMipiLpLevelThreshold(float fThreshold, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置MIPI管脚使能信号
///
/// @param bEnable：为False时MIPI进入HS状态，为TRUE进入LP状态（该函数功能已经失效）
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMipiEnable(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/*@example
	// 接收vc channel为0的数据，其他vc channel的数据不输出
	int iRet = SetMipiImageVC(0x0,TRUE,CHANNEL_A,iDevID)
	if (iRet != DT_ERROR_OK
		return iRet;
*/

/// @brief 设置虚拟通道号
///
/// @param uVC：设置接收的图像通道号，0/1/2/3
/// @param bVCFileterEn：使能过滤其他的虚拟通道
///
/// @retval DT_ERROR_OK：设置MIPI接口控制器成功
/// @retval DT_ERROR_FAILED：设置MIPI接口控制器失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMipiImageVC(UINT uVC, BOOL bVCFilterEn, BYTE byChannel = CHANNEL_A, int iDevID = DEFAULT_DEV_ID);

/// @brief 过滤MIPI包,指定要过滤的包的ID号及过滤的包个数
///
/// @param PackID[]：设置要过滤的包的ID号
/// @param iCount：过滤的包ID的个数，DMU956最大64
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMipiPacketFilter(int PackID[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取过滤的包的ID号
///
/// @param PackID[]：返回的过滤的包的ID号
/// @param pCount：返回过滤包的个数
/// @param MaxCount：设置要获取的包ID的最大数，最大64，即PackID的数组大小
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMipiPacketFilter(int PackID[], int *pCount, int MaxCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取MIPI接口接收器CRC错误计数
///
/// @param pCount：返回MIPI接口接收器CRC错误计数值
/// @param byChannel：A/B通道选择
///
/// @retval DT_ERROR_OK：获取MIPI接口接收器CRC错误计数值成功
/// @retval DT_ERROR_FAILED：获取MIPI接口接收器CRC错误计数值失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMipiCrcErrorCount(UINT *pCount, BYTE byChannel = CHANNEL_A, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取MIPI接口接收器ECC错误计数
///
/// @param pCount：返回MIPI接口接收器ECC错误计数值
/// @param byChannel：A/B通道选择
///
/// @retval DT_ERROR_OK：获取MIPI接口接收器ECC错误计数值成功
/// @retval DT_ERROR_FAILED：获取MIPI接口接收器ECC错误计数值失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMipiEccErrorCount(UINT *pCount, BYTE byChannel = CHANNEL_A, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取MIPI接口的LOCK状态
///
/// @param pMipiLockState：获取MIPI接口的LOCK状态，从bit0-bit3依次表示LANE1、LANE2、LANE3、LANE4
///
/// @retval DT_ERROR_OK：获取MIPI接口LOCK状态成功
/// @retval DT_ERROR_FAILED：获取LOCK状态失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMipiLockState(DWORD *pMipiLockState, BYTE byChannel, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置MIPI接口控制器扩展接口。20180918 目前只支持端口和lane数配置，结构体的其他功能建议单独调用函数
///
/// @param pMipiCtrl：参见MipiCtrlEx结构体
DTCCM_API int _DTCALL_ SetMipiCtrlEx(MipiCtrlEx_t *pMipiCtrl, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取当前MIPI接口控制器的参数配置值。
///
/// @param pMipiCtrl：参见MipiCtrlEx结构体
DTCCM_API int _DTCALL_ GetMipiCtrlEx(MipiCtrlEx_t *pMipiCtrl, int iDevID = DEFAULT_DEV_ID);
/*
example:
FrameFilter_t sFramefilter;
iRet = GetErrFrameFilter(&sFramefilter,m_nDevID);
if (iRet != DT_ERROR_OK)
	return iRet;

sFramefilter.bCrcErrorFilter = false;
sFramefilter.bSizeErrorFilter = true;
iRet = SetErrFrameFilter(&sFramefilter, m_nDevID);
if (iRet != DT_ERROR_OK)
	return iRet;
*/
/// @brief 设置错误帧过滤是否使能
///
/// @param pFrameFilter：帧过滤使能信息
///
/// @retval DT_ERROR_OK：设置成功
DTCCM_API int _DTCALL_ SetErrFrameFilter(FrameFilter_t *pFrameFilter, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取错误帧过滤使能状态
///
/// @param pFrameFilter：帧过滤使能信息
///
/// @retval DT_ERROR_OK：设置成功
DTCCM_API int _DTCALL_ GetErrFrameFilter(FrameFilter_t *pFrameFilter, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置并行接口接收控制器
///
/// @param dwCtrl：并行接口控制器操作码，参见宏定义“同步并行接口特性的位定义”
///
/// @retval DT_ERROR_OK：设置并行接口控制器成功
/// @retval DT_ERROR_FAILED：设置并行接口控制器失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetParaCtrl(DWORD dwCtrl, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取并行接口接收控制器控制码
///
/// @param pdwCtrl：返回并行接口接收控制器控制码
///
/// @retval DT_ERRORO_OK：获取成功
DTCCM_API int _DTCALL_ GetParaCtrl(DWORD *pdwCtrl, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置HiSPI接口控制器
///
/// @param dwCtrl：HiSPI接口控制器操作码，参见宏定义“HiSPI接口特性的位定义”
///
/// @retval DT_ERROR_OK：设置HiSPI接口控制器成功
/// @retval DT_ERROR_FAILED：设置HiSPI接口控制器失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetHispiCtrl(DWORD dwCtrl, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置模拟图像控制器
///
/// @param dwCtrl：模拟图像控制器操作码
/// @param byRefByte：模拟图像数据定义
///
/// @retval DT_ERROR_OK：设置模拟图像控制器成功
/// @retval DT_ERROR_FAILED：设置模拟图像控制器失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetSimCtrl(DWORD dwCtrl, BYTE byRefByte, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取SLVS-EC接口控制器配置值
///
/// @param pSlvsCfg：SLVS-EC接口配置结构体
///
/// @param DT_ERROR_OK：获取成功
/// @param DT_ERROR_FAILED：获取失败
DTCCM_API int _DTCALL_ GetSlvsECCtrl(SlvsECCtrl_t *pSlvsecCfg, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置SLVS-EC接口控制器配置值
///
/// @param pSlvsCfg：SLVS-EC接口配置结构体
///
/// @param DT_ERROR_OK：设置成功
/// @param DT_ERROR_FAILED：设置失败
DTCCM_API int _DTCALL_ SetSlvsECCtrl(SlvsECCtrl_t *pSlvsecCfg, int iDevID = DEFAULT_DEV_ID);

/// @brief 初始化SENSOR
///
/// @param uDevAddr：SENSOR器件地址
/// @param pParaList：SENSOR的参数列表
/// @param uLength：pParaList的大小
/// @param byI2cMode：访问SENSOR的I2C模式，参见枚举类型I2CMODE
///
/// @retval DT_ERROR_OK：初始化SENSOR成功
/// @retval DT_ERROR_FAILED：初始化SENSOR失败
///
/// @see I2CMODE
DTCCM_API int _DTCALL_ InitSensor(UCHAR uDevAddr, USHORT *pParaList, USHORT uLength, BYTE byI2cMode, int iDevID = DEFAULT_DEV_ID);

/// @brief 初始化SENSOR
///
/// @param uDevAddr：SENSOR器件地址
/// @param pParaList：SENSOR的参数列表
/// @param uLength：pParaList的大小
/// @param byI2cMode：访问SENSOR的I2C模式，参见枚举类型I2CMODE
///
/// @retval DT_ERROR_OK：初始化SENSOR成功
/// @retval DT_ERROR_FAILED：初始化SENSOR失败
///
/// @see I2CMODE
DTCCM_API int _DTCALL_ InitSensor2(UINT uDevAddr, UINT *pParaList, UINT uLength, UINT byI2cMode, int iDevID = DEFAULT_DEV_ID);

/// @brief 检查插上的SENSOR是不是当前指定的
///
/// @param pInfo：SENSOR信息，参见SensorTab结构体
/// @param byChannel：通道选择，A/B通道，参见宏定义“多SENSOR模组通道定义”
/// @param bReset：给SENSOR复位(该参数无效)
///
/// @retval DT_ERROR_OK：找到SENSOR
/// @retval DT_ERROR_FAILED：没有找到SENSOR
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SensorIsMe(SensorTab *pInfo, BYTE byChannel, BOOL bReset, int iDevID = DEFAULT_DEV_ID);

/// @brief 检查插上的SENSOR是不是当前指定的
///
/// @param pInfo：SENSOR信息，参见SensorTab2结构体
/// @param byChannel：通道选择，A/B通道，参见宏定义“多SENSOR模组通道定义”
/// @param bReset：给SENSOR复位(该参数无效)
///
/// @retval DT_ERROR_OK：找到SENSOR
/// @retval DT_ERROR_FAILED：没有找到SENSOR
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SensorIsMe2(SensorTab2 *pInfo, BYTE byChannel, BOOL bReset, int iDevID = DEFAULT_DEV_ID);

/// @brief 通道选择(仅仅UH920支持)
///
/// @param byChannel：通道选择，A/B通道，参见宏定义“多SENSOR模组通道定义”
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILED：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetChannelSel(BYTE byChannel, int iDevID = DEFAULT_DEV_ID);

/// @brief 图像通道使能
///
/// @param byChannelEnabel：通道选择，A/B通道，参见宏定义“多SENSOR模组通道定义”
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILED：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetGrabChannelEnable(BYTE byChannelEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置柔性接口
///
/// @param PinConfig：柔性接口配置定义
///
/// @retval DT_ERROR_OK：柔性接口配置成功
/// @retval DT_ERROR_FAILED：柔性接口配置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetSoftPin(BYTE PinConfig[26], int iDevID = DEFAULT_DEV_ID);

/// @brief 设置柔性接口是否使能上拉电阻
///
/// @param bPullup：柔性接口上拉使能，bPullup=TRUE使能上拉电阻，bPullup=FALSE关闭上拉电阻
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILED：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetSoftPinPullUp(BOOL bPullup, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置是否使能柔性接口，没使能时为高阻状态
///
/// @param bEnable：柔性接口使能
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILED：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ EnableSoftPin(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 使能GPIO。和EnableSoftPin接口功能一样
///
/// @param bEnable：使能GPIO
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILED：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ EnableGpio(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取柔性接口电平，每BIT对应一个管脚，bit0对应PIN_1
///
/// @param pPinLevel：柔性接口电平
///
/// @retval DT_ERROR_OK：获取柔性接口电平成功
/// @retval DT_ERROR_FAILED：获取柔性接口电平失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetSoftPinLevel(DWORD *pPinLevel, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置单个GPIO的电平，iPin的编号值从1开始，1表示GPIO1
///
/// @param iPin：GPIO编号值，iPin的编号值从1开始，1表示GPIO1
/// @param bLevel：设置GPIO的电平
///
/// @retval DT_ERROR_OK：设置单个GPIO的电平值成功
/// @retval DT_ERROR_FAILED：设置单个GPIO的电平值失败
/// @retval DT_ERROR_PARAMETER_INVALID：GPIO编号值参数无效
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetGpioPinLevel(int iPin, BOOL bLevel, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取某个GPIO的电平，如果是输入，则是外部电平；如果是输出，则为设置电平
///
/// @param iPin：GPIO编号值，iPin的编号值从1开始，1表示GPIO1
/// @param pLevel：GPIO的电平值
///
/// @retval DT_ERROR_OK：获取GPIO的电平值成功
/// @retval DT_ERROR_FAILED：获取GPIO的电平值失败
/// @retval DT_ERROR_PARAMETER_INVALID：GPIO编号值参数无效
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetGpioPinLevel(int iPin, BOOL *pLevel, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置多个GPIO的电平，Pin中存储的编号值从1开始，1表示GPIO1
///
/// @param Pin：GPIO编号
/// @param Level：GPIO电平值
/// @param iCount：要设置的GPIO数量
///
/// @retval DT_ERROR_OK：设置多个GPIO的电平成功
/// @retval DT_ERROR_FAILED：设置多个GPIO的电平失败
/// @retval DT_ERROR_PARAMETER_INVALID：GPIO编号值参数无效
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMultiGpioPinLevel(int Pin[], BOOL Level[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置某个GPIO的IO管脚方向，bDir：TRUE为输入，FALSE为输出
///
/// @param iPin：GPIO编号
/// @param bDir：GPIO管脚方向,bDir：TRUE为输入，FALSE为输出
///
/// @retval DT_ERROR_OK：设置GPIO的IO管脚方向成功
/// @retval DT_ERROR_FAILED：设置GPIO的IO管脚方向失败
/// @retval DT_ERROR_PARAMETER_INVALID：GPIO编号值参数无效
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetGpioPinDir(int iPin, BOOL bDir, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置多个GPIO的IO管脚方向，Dir：TRUE为输入，FALSE为输出
///
/// @param Pin：GPIO编号
/// @param Dir：TRUE为输入，FALSE为输出
/// @param iCount：要设置的GPIO数量
///
/// @retval DT_ERROR_OK：设置多个GPIO的IO管脚方向成功
/// @retval DT_ERROR_FAILED：设置多个GPIO的IO管脚方向失败
/// @retval DT_ERROR_PARAMETER_INVALID：GPIO编号值参数无效
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetMultiGpioPinDir(int Pin[], BOOL Dir[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取某个GPIO的电平，如果是输入，则是外部电平；如果是输出，则为设置电平
///
/// @param Pin：GPIO管脚编号
/// @param Level：GPIO电平
/// @param iCount：要设置的GPIO数量
///
/// @retval DT_ERROR_OK：获取某个GPIO的IO管脚方向成功
/// @retval DT_ERROR_FAILED：获取某个GPIO的IO管脚方向失败
/// @retval DT_ERROR_PARAMETER_INVALID：GPIO编号值参数无效
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetMultiGpioPinLevel(int Pin[], BOOL Level[], int iCount, int iDevID = DEFAULT_DEV_ID);
/****************************************************************************************
SPI总线
*****************************************************************************************/

/// @brief uPort:SPI控制器配置
///
/// @brief pSPIConfig:SPI配置结构体，参见imagekit.h
///
/// @retval DT_ERROR_OK：SPI配置成功
/// @retval DT_ERROR_FAILD：SPI配置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ MasterSpiConfig(UCHAR uPort, MasterSpiConfig_t *pSPIConfig, int iDevID = DEFAULT_DEV_ID);

/// @brief SPI写SENSOR寄存器，支持向一个寄存器写入一个数据块（不超过255字节）,MU950更新固件后支持4000
///
/// @param uDevAddr：从器件地址,若无器件地址，则请设为0
/// @param uRegAddr：寄存器地址,寄存器地址支持最大4个byte
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：写入寄存器的数据块
/// @param uSize：写入寄存器的数据块的字节数
///
/// @retval DT_ERROR_OK：完成写SENSOR寄存器块操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ WriteSensorSPI(UCHAR uDevAddr, UINT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, int iDevID = DEFAULT_DEV_ID);

/// @brief SPI读SENSOR寄存器，支持向一个寄存器读出一个数据块（不超过255字节）,MU950更新固件后支持4000
///
/// @param uDevAddr：从器件地址,若无器件地址，则请设为0
/// @param uRegAddr：寄存器地址,寄存器地址支持最大4个byte
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：读到寄存器的值
/// @param uSize：读出寄存器的数据块的字节数（不超过255字节）
///
/// @retval DT_ERROR_OK：完成读SENSOR寄存器块操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：uSize参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ReadSensorSPI(UCHAR uDevAddr, UINT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, int iDevID = DEFAULT_DEV_ID);

/// @brief SPI控制器使能、速度、模式设置，注意：在操作SensorSpiRW之前必须使能SPI总线，如果不操作必须关闭SPI总线，以免影响SENSOR i2c的访问（仅UT300/UV910/UF920支持）
///
/// @param bEnable：为True是开启SPI总线，为False是关闭SPI总线
/// @param iRate：为True是500Kbps 为False是250Kbps
/// @param bType：三线或四线通讯选择，为False是三线SPI通讯，为True是四线SPI通讯
///
/// @retval DT_ERROR_OK：操作成功
///
/// @note 在操作SensorSpiRW之前必须使能SPI总线，如果不操作必须关闭SPI总线，以免影响SENSOR i2c的访问
DTCCM_API int _DTCALL_ SensorSpiInit(BOOL bEnable, int iRate = 0, BOOL bType = FALSE, int iDevID = DEFAULT_DEV_ID);

/// @brief SPI控制器接口，支持三线或四线通讯，支持单字节传输和多字节连续传输，支持读写（仅UT300/UV910/UF920支持）
///
/// @param bStart：为True片选信号拉低，为False片选信号不拉低
/// @param bStop：为True片选信号拉高，为False片选信号不拉高
/// @param bMsb：为True是高位先出，为False是低位先出
/// @param TxData：写入的数据BUFFER
/// @param RxData：读回的数据BUFFER
/// @param TxLen：写入数据的大小，字节数
/// @param RxLen：读取数据的大小，字节数

/// @retval DT_ERROR_OK：操作成功
/// @retval DT_ERROR_FAILD：操作失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SensorSpiRW(BOOL bStart, BOOL bStop, BOOL bMsb, UCHAR *TxData, UCHAR *RxData, UCHAR TxLen, UCHAR RxLen, int iDevID = DEFAULT_DEV_ID);

/*
@example
//
	//发送4个数据到从，读回4个字节
	UCHAR TxData[4];
	UCHAR RxData[4];
	UINT TxLen = 4;
	UINT RxLen = 4;

	TxData[0] = 0;
	TxData[1] = 1;
	TxData[2] = 2;
	TxData[3] = 3;
	int iRet = SensorSpiRWEx(TRUE,TRUE,TRUE,TxData,RxData,TxLen,RxLen,m_iDevID);
	if (iRet != DT_ERROR_OK)
		return iRet;
*/
/// @brief SPI控制器接口，四线全双工，支持单字节传输和多字节连续传输，支持读写（mu950最大支持Txlen/RxLen最大4000）
///
/// @param bStart：为True片选信号拉低，为False片选信号不拉低
/// @param bStop：为True片选信号拉高，为False片选信号不拉高
/// @param bMsb：为True是高位先出，为False是低位先出
/// @param TxData：写入的数据BUFFER
/// @param RxData：读回的数据BUFFER
/// @param TxLen：写入数据的大小，字节数
/// @param RxLen：读取数据的大小，字节数

/// @retval DT_ERROR_OK：操作成功
/// @retval DT_ERROR_FAILD：操作失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SensorSpiRWEx(BOOL bStart, BOOL bStop, BOOL bMsb, UCHAR *TxData, UCHAR *RxData, UINT uTxLen, UINT uRxLen, int iDevID = DEFAULT_DEV_ID);

/****************************************************************************************
柔性接口时钟输出
*****************************************************************************************/
/// @brief 柔性接口可调节时钟，支持柔性管脚输出0-200Khz可调节时钟（仅UF920支持）
///
/// @param uHz：输出时钟大小，单位Hz，0-200Khz可调节
/// @param bOnOff：可调节时钟输出使能，True为输出使能，False为关闭输出
///
/// @retval DT_ERROR_OK：设置时钟输出成功
/// @retval DT_ERROR_FAILD：设置时钟输出失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetSoftPinAdjClk1(UINT uHz, BOOL bOnOff, int iDevID = DEFAULT_DEV_ID);

/// @brief 柔性接口可调节时钟，支持柔性管脚输出0-18Mhz可调节时钟（仅UF920支持）
///
/// @param uHundkHz：设置输出时钟大小，单位100KHz，0-18Mhz可调节
/// @param bOnOff：可调节时钟输出使能，True为输出使能，False为关闭输出
///
/// @retval DT_ERROR_OK：设置时钟输出成功
/// @retval DT_ERROR_FAILD：设置时钟输出失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetSoftPinAdjClk2(UINT uHundkHz, BOOL bOnOff, int iDevID = DEFAULT_DEV_ID);

/** @} */ // end of group3

/******************************************************************************
图像数据采集相关
******************************************************************************/
/** @defgroup group4 图像数据采集相关


* @{

*/
/// @brief 开启图像数据采集
///
/// @param uImgBytes：图像数据大小，单位字节
///
/// @retval DT_ERROR_OK：开启图像数据采集成功
/// @retval DT_ERROR_FAILD：开启图像数据采集失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ OpenVideo(UINT uImgBytes, int iDevID = DEFAULT_DEV_ID);

/// @brief 关闭图像数据采集
///
/// @retval DT_ERROR_OK：关闭图像数据采集成功
/// @retval DT_ERROR_FAILD：关闭图像数据采集失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ CloseVideo(int iDevID = DEFAULT_DEV_ID);

/// @brief 设置采集一帧数据量
///
/// @param uGrabFrameSize：设置一帧的数据量大小，单位字节
DTCCM_API int _DTCALL_ SetGrabFrameSize(ULONG uGrabFrameSize, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置采集一帧数据量
///
/// @param uGrabFrameSize：设置一帧的数据量大小，单位字节
/// @param bEnable: 设置一帧数据量大小是否生效
DTCCM_API int _DTCALL_ SetGrabFrameSizeEx(BOOL bEnable, ULONG uGrabFrameSize, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置SENSOR图像数据接口类型
///
/// @param byPortType：SENSOR图像数据接口类型，参见枚举类型“SENSOR_PORT”
/// @param uWidth：图像数据宽度
/// @param uHeight：图像数据高度
///
/// @retval DT_ERROR_OK：设置SENSOR图像数据接口类型成功
/// @retval DT_ERROR_FAILD：设置SENSOR图像数据接口类型失败
/// @retval DT_ERROR_PARAMETER_INVALID：无效的图像数据接口类型参数
///
/// @see SENSOR_PORT
DTCCM_API int _DTCALL_ SetSensorPort(BYTE byPortType, USHORT uWidth, USHORT uHeight, int iDevID = DEFAULT_DEV_ID);

/// @brief 复位SENSOR图像数据接口
///
/// @param byPortType：SENSOR图像数据接口类型，参见枚举类型“SENSOR_PORT”
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
///
/// @see SENSOR_PORT
DTCCM_API int _DTCALL_ ResetSensorPort(BYTE byPortType, int iDevID = DEFAULT_DEV_ID);

/*
@example:配置跳帧模式（跳帧模式下，GrabFrame接口抓取的是最新帧）
FrameBufferConfig bufcfg;
iRet = GetFrameBufferConfig(&bufcfg, m_nDevID);
if (iRet != DT_ERROR_OK)
{
	msg("GetFrameBufferConfig failed with err code:%d\r\n", iRet);
}
bufcfg.uBufferFrames = 3;
bufcfg.uMode = BUF_MODE_SKIP;
iRet = SetFrameBufferConfig(&bufcfg, m_nDevID);
if (iRet != DT_ERROR_OK)
{
	msg("GetFrameBufferConfig failed with err code:%d\r\n", iRet);
}
*/
/// @brief 配置FrameBuffer
///
/// @param pConfig：FrameBuffer配置结构体,该结构体说明参见imagekit.h头文件
///
/// @note 建议在InitDevice、InitIsp、Openvideo这些函数之前调用
DTCCM_API int _DTCALL_ SetFrameBufferConfig(FrameBufferConfig *pConfig, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取FrameBuffer的配置信息
///
/// @param pConfig：FrameBuffer配置结构体,该结构体说明参见imagekit.h头文件
DTCCM_API int _DTCALL_ GetFrameBufferConfig(FrameBufferConfig *pConfig, int iDevID = DEFAULT_DEV_ID);

/// @brief 清空测试盒的缓存
///
/// @retval DT_ERROR_OK：清空成功
/// @retval DT_ERROR_FAILD：清空失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ ResetFrameBuffer(int iDevID = DEFAULT_DEV_ID);

/// @brief 设置ROI。
///
/// @param roi_x0：起始点水平坐标，单位字节
/// @param roi_y0：起始点垂直坐标，单位字节
/// @param roi_hw：水平方向ROI图像宽度，单位字节
/// @param roi_vw：垂直方向ROI图像高度，单位字节
/// @param roi_hb：水平方向blank宽度，单位字节
/// @param roi_vb：垂直方向blank高度，单位字节
/// @param roi_hnum：水平方向ROI数量，单位字节
/// @param roi_vnum：垂直方向ROI数量，单位字节
/// @param roi_en：ROI使能
///
/// @retval DT_ERROR_OK：ROI设置成功
/// @retval DT_ERROR_FAILD：ROI设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
///
/// @note 该函数中指定宽度和水平位置是以字节为单位，并且保证宽度为4字节的整数倍。
///
/// @image html roi.png
DTCCM_API int _DTCALL_ SetRoi(USHORT roi_x0,
							  USHORT roi_y0,
							  USHORT roi_hw,
							  USHORT roi_vw,
							  USHORT roi_hb,
							  USHORT roi_vb,
							  USHORT roi_hnum,
							  USHORT roi_vnum,
							  BOOL roi_en,
							  int iDevID = DEFAULT_DEV_ID);

/// @brief 设置ROI（函数内部会申请图像处理需要的内存）
///
/// @param roi_x0：起始点水平坐标，单位像素
/// @param roi_y0：起始点垂直坐标，单位像素
/// @param roi_hw：水平方向ROI图像宽度，单位像素
/// @param roi_vw：垂直方向ROI图像高度，单位像素
/// @param roi_hb：水平方向blank宽度，单位像素
/// @param roi_vb：垂直方向blank高度，单位像素
/// @param roi_hnum：水平方向ROI数量，单位像素
/// @param roi_vnum：垂直方向ROI数量，单位像素
/// @param roi_en：ROI使能
///
/// @retval DT_ERROR_OK：ROI设置成功
/// @retval DT_ERROR_FAILD：ROI设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
///
/// @note 该函数中指定宽度和水平位置是以像素为单位，并且要保证宽度转为字节后是16字节的整数倍。
///
/// @image html roi.png
DTCCM_API int _DTCALL_ SetRoiEx(USHORT roi_x0,
								USHORT roi_y0,
								USHORT roi_hw,
								USHORT roi_vw,
								USHORT roi_hb,
								USHORT roi_vb,
								USHORT roi_hnum,
								USHORT roi_vnum,
								BOOL roi_en,
								int iDevID = DEFAULT_DEV_ID);

/// @brief 设置采集数据超时，单位：ms，如果该事件内没有采集到图像数据，GrabFrame函数将返回超时错误
///
/// @param uTimeOut：采集数据超时时间设置，单位ms
DTCCM_API int _DTCALL_ SetGrabTimeout(ULONG uTimeOut, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置ISP处理模式模式,参见enum ISP_MODE
///
/// @param byMode：ISP处理模式，0为normal，1为S2DFAST，2为S2DFAST GPU
// DTCCM_API int _DTCALL_ SetImageProcessMode(BYTE byMode,int iDevID=DEFAULT_DEV_ID);

/// @brief S2DFAST模式抓取数据，得到的是RGB数据
///
/// @param pBuf: 帧buffer
///
/// retval:DT_ERROR_FAILED:抓取失败，得到错误帧
/// retval:DT_ERROR_TIMEOUT:抓取超时，没有抓到数据
// DTCCM_API int _DTCALL_ S2DGrabFrameDirect(BYTE **ppBuf,FrameInfoEx *pInfo,int iDevID=DEFAULT_DEV_ID);

/// @brief 释放buf，每次调用S2DGrabFrameDirect抓到数据，显示完，都需释放

// DTCCM_API int _DTCALL_ S2DReleaseBufferDirect(BYTE *pBuf,int iDevID=DEFAULT_DEV_ID);

/// @brief 校准sensor接收，建议openvideo之后调用，校准成功再进行抓帧操作,建议超时时间大于1000ms
///
/// @param uTimeOut：校准超时时间设置，单位ms
///
/// @retval DT_ERROR_OK：校准成功，可以采集图像
/// @retval DT_ERROR_TIME_OUT：校准超时
DTCCM_API int _DTCALL_ CalibrateSensorPort(ULONG uTimeOut, int iDevID = DEFAULT_DEV_ID);

/// @brief 采集一帧图像，并且返回帧的一些信息，A通道和B通道都是使用GrabFrame函数获取图像数据，通过帧信息中的byChannel可以区分图像数据所属的通道
///
/// @param pInBuffer：采集图像BUFFER
/// @param uBufferSize：采集图像BUFFER大小，单位字节
/// @param pGrabSize：实际抓取的图像数据大小，单位字节
/// @param pInfo：返回的图像数据信息
///
/// @retval DT_ERROR_OK：采集一帧图像成功
/// @retval DT_ERROR_FAILD：采集一帧图像失败，可能不是完整的一帧图像数据
/// @retval DT_ERROR_TIME_OUT：采集超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
///
/// @note 调用该函数之前，请先根据图像大小获取到足够大的缓存区用于装载图像数据。\n
/// 同时，缓存区的大小也需要作为参数传入到GrabFrame函数，以防止异常情况下导致的内存操作越界问题。
DTCCM_API int _DTCALL_ GrabFrame(BYTE *pInBuffer, ULONG uBufferSize, ULONG *pGrabSize, FrameInfo *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief 采集一帧图像，并且返回帧的一些信息，通过帧信息结构体可以获取帧的时间戳、当前帧的ECC错误计数、CRC错误计数等
///
/// @param pInBuffer：采集图像BUFFER
/// @param uBufferSize：采集图像BUFFER大小，单位字节
/// @param pGrabSize：实际抓取的图像数据大小，单位字节
/// @param pInfo：返回的图像数据信息
///
/// @retval DT_ERROR_OK：采集一帧图像成功
/// @retval DT_ERROR_FAILD：采集一帧图像失败，可能不是完整的一帧图像数据
/// @retval DT_ERROR_TIME_OUT：采集超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
///
/// @note 调用该函数之前，请先根据图像大小获取到足够大的缓存区用于装载图像数据。\n
/// 同时，缓存区的大小也需要作为参数传入到GrabFrameEx函数，以防止异常情况下导致的内存操作越界问题。
DTCCM_API int _DTCALL_ GrabFrameEx(BYTE *pInBuffer, ULONG uBufferSize, ULONG *pGrabSize, FrameInfoEx *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief 图像抓拍设置，设置使能和一次抓拍的帧数,最大支持255(不建议客户使用，GrabHold和GrabRestar功能替代)
///
/// @param bSnapEn：图像抓拍使能
/// @param uSnapCount：一次抓拍的帧数
///
/// @retval DT_ERROR_OK：图像抓拍设置成功
/// @retval DT_ERROR_FAILD：图像抓拍设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetSnap(BOOL bSnapEn, UINT uSnapCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 启动一次抓拍(不建议客户使用，GrabHold和GrabRestart功能替代)
///
/// @retval DT_ERROR_OK：设置启动一次抓拍成功
/// @retval DT_ERROR_FAILD：设置启动一次抓拍失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ TriggerSnap(int iDevID = DEFAULT_DEV_ID);

/// @brief 重发上一帧图像（不建议使用）
///
/// @retval DT_ERROR_OK：设置重发上一帧图像成功
/// @retval DT_ERROR_FAILD：设置重发上一帧图像失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ ReSendImage(int iDevID = DEFAULT_DEV_ID);

/// @brief 获取采集图像的宽高信息，注意：*pWidth的单位为字节
///
/// @param pWidth：获取的采集图像宽度信息
/// @param pHeight：获取的采集图像高度信息
/// @param byChannel：A/B通道选择
///
/// @retval DT_ERROR_OK：获取采集图像的宽高信息成功
/// @retval DT_ERROR_FAILD：获取采集图像的宽高信息失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetGrabImageSize(USHORT *pWidth, USHORT *pHeight, BYTE byChannel, int iDevID = DEFAULT_DEV_ID);

/// @brief 返回实际抓取图像数据的大小（单位字节）
///
/// @param pGrabSize：返回实际抓取图像数据大小
DTCCM_API int _DTCALL_ CalculateGrabSize(ULONG *pGrabSize, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取像素位置数据中的字节位置
///
/// @param iPixelPos：像素位置
/// @param byImgFormat：图像数据格式
///
/// @retval 返回所给像素位置数据中的字节位置
DTCCM_API int _DTCALL_ GetByteOffset(int iPixelPos, BYTE byImgFormat, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置时间点，设置成功后，设备的时间戳值将从该时间点开始计时（单位us）
///
/// @param uTimeStamp: 时间点
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetTimerClock(double uTime, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取时间戳(单位us)
///
/// @param pTimeStamp: 时间戳值
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetTimeStamp(double *pTimeStamp, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置TestWindow功能，可以设置多个ROI区域
///
/// @param uFullWidth:全分辨率宽度，单位像素
/// @param uFullHeight:全分辨率高度
/// @param pRoi:testwindow结构体配置，参见“imagekit.h”
/// @param uCount:ROI个数设置
///
/// @retval DT_ERROR_OK：获取成功
/// @retval DT_ERROR_FAILD：获取失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetTestWindowRoi(UINT uFullWidth, UINT uFullHeight, DtRoi_t *pRoi, UINT uCount, int iDevID = DEFAULT_DEV_ID);

/// @brief TetsWindo功能使能
///
/// @param bEnable:TestWindow功能使能
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetTestWindowEnable(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @bfief 采集暂停，设备帧ID继续增加，如果继续抓帧，会返回-1000错误码
DTCCM_API int _DTCALL_ GrabHold(int iDevID = DEFAULT_DEV_ID);

/// @brief 采集重启
///
/// @param uDelayTime：设置数据流延时输出，单位us
/// @param uFrameNum:设置重启采集后采集帧数，达到设置的采集帧数，将自动GrabHold；如果设为0，将连续输出帧
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GrabRestart(double lfDelayTime, UINT uFrameNum = 0, int iDevID = DEFAULT_DEV_ID);

/// @brief 采集重启
///
/// @param uSkipFrameNum：设置数据流延时多少帧输出，单位帧
/// @param uFrameNum:设置重启采集后采集帧数，达到设置的采集帧数，将自动GrabHold；如果设为0，将连续输出帧
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GrabRestartWithSkip(UINT uSkipFrameNum, UINT uFrameNum = 0, int iDevID = DEFAULT_DEV_ID);

/// @brief 新的抓帧接口，比原采集接口（GrabFrame/GrabFrameEx）效率更高，使用该接口，用户无需申请buffer
DTCCM_API int _DTCALL_ GrabFrameDirect(BYTE **ppBuf, FrameInfoEx *pInfo, int iDevID = DEFAULT_DEV_ID);

/// @brief 采集帧是否到达标志,注意：GrabFrameIsArrived与GrabFrameDirect必须在同一个线程里面使用
///
/// @param pIsArrived：为TRUE表示帧已经采集到，为FALSE表示帧未采集到
DTCCM_API int _DTCALL_ GrabFrameIsArrived(BOOL *pIsArrived, int iDevID = DEFAULT_DEV_ID);
/** @} */ // end of group4

/** @defgroup group5 电源管理单元相关


* @{

*/
/// @brief 设置盒子端VDDIO电压，既Sensor Bank的电压，和Sensor的dovdd电压要一致
///
/// @param Voltage:设置电压值，单位mV
/// @param bOnOff:输出电压是否开启，TRUE开启，如果为TRUE，可提前开启盒子的vddio电压；若为FALSE，则盒子的vddio跟随DOVDD
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILD：设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetVddioVoltage(int Voltage, BOOL bOnOff, int iDevID = DEFAULT_DEV_ID);

/// @brief PMU复位，有可能导致系统电源重启，所有模块重新上电（建议不使用该接口）
///
/// @retval DT_ERROR_OK：PMU复位成功
/// @retval DT_ERROR_FAILD：PMU复位失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ PmuReset(int iDevID = DEFAULT_DEV_ID);

/// @brief 获取系统电源状态(电压，电流)
///
/// @param Power：系统电源类型，参见枚举类型“SYS_POWER”
/// @param Voltage：获取的系统电压值，单位mV
/// @param Current：获取的系统电流值，单位mA
/// @param iCount：要获取的系统电源的路数
///
/// @retval DT_ERROR_OK：获取系统电源状态成功
///	@retval DT_ERROR_COMM_ERROR：通讯错误
///
/// @see SYS_POWER
DTCCM_API int _DTCALL_ PmuGetSysPowerVA(SYS_POWER Power[], int Voltage[], int Current[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置电源模式，不同的工作状态下设置不同模式，出图模式必须进入工作模式（POWER_MODE_WORK），如要测试待机电流，进入待机模式(POWER_MODE_STANDBY)，要设置待机模式，才能获取到比较精准的电流值，同PmuSetCurrentRange
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Mode：电源模式，参见枚举类型“POWER_MODE”
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置电源模式成功
/// @retval DT_ERROR_FAILD：设置电源模式失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
/// @see POWER_MODE
DTCCM_API int _DTCALL_ PmuSetMode(SENSOR_POWER Power[], POWER_MODE Mode[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置电源电压
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Voltage：设置的电源电压值，单位mV
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置电源电压成功
/// @retval DT_ERROR_FAILED：设置电源电压失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetVoltage(SENSOR_POWER Power[], int Voltage[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取电源电压，如果能获取检测到的，尽量使用检测到的数据，否则返回电压设置值
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Voltage：获取的电源电压值，单位mV
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置电源电压成功
/// @retval DT_ERROR_FAILD：设置电源电压失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuGetVoltage(SENSOR_POWER Power[], int Voltage[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取远端电压采集设置使能开关值
///
/// @param Power：电源类型，参加枚举类型“SENSOR_POWER”
/// @param OnOff：是否开启远端电压采集，TRUE开启，FLASE关闭
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：获取远端电压采集成功
/// @retval DT_ERROR_FAILD：获取远端电压采集失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuGetRemoteOnOff(SENSOR_POWER Power[], BOOL OnOff[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置远端电压采集使能
///
/// @param Power：电源类型，参加枚举类型“SENSOR_POWER”
/// @param OnOff：是否开启远端电压采集，TRUE开启，FLASE关闭
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置远端电压采集成功
/// @retval DT_ERROR_FAILD：设置远端电压采集失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetRemoteOnOff(SENSOR_POWER Power[], BOOL OnOff[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取远端电压,获取的是检测值
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Voltage：获取的电源电压值，单位mv
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：获取远端电压成功
/// @retval DT_ERROR_FAILED：获取远端电压失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuGetRemoteVoltage(SENSOR_POWER Power[], int Voltage[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置电源电流量程，不同的工作状态下设置不同量程，出图模式要进入毫安档量程，如要测试待机电流进入uA量程(CURRENT_RANGE_UA)，才能获取到比较精准的电流值，同PmuSetMode
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Range：电源电流量程，参见枚举类型“CURRENT_RANGE”
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置电源电流量程成功
/// @retval DT_ERROR_FAILD：设置电源电流量程失败
///
/// @see SENSOR_POWER
/// @see CURRENT_RANGE
DTCCM_API int _DTCALL_ PmuSetCurrentRange(SENSOR_POWER Power[], CURRENT_RANGE Range[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取电源电流。
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Current：电流值,单位nA
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：获取电源电流成功
/// @retval DT_ERROR_FAILD：获取电源电流失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuGetCurrent(SENSOR_POWER Power[], int Current[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取电源电流(支持更大范围，更高精度)
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Current：电流值,单位nA
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：获取电源电流成功
/// @retval DT_ERROR_FAILD：获取电源电流失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuGetCurrentDouble(SENSOR_POWER Power[], double Current[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置电压上升沿，设定值(Rise)单位:mV/mS
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Rise：电压上升沿值，单位:mV/mS
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置电压上升沿成功
/// @retval DT_ERROR_FAILD：设置电压上升沿失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetRise(SENSOR_POWER Power[], int Rise[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置电流采集速度，设定值(SampleSpeed)10-500单位ms（默认设置是100）
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param SampleSpeed：电流采集速度，范围10-500
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置电流采集速度成功
/// @retval DT_ERROR_FAILD：设置电流采集速度失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetSampleSpeed(SENSOR_POWER Power[], int SampleSpeed[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置电源开关状态
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param OnOff：设置电源开关状态，TRUE为开启，FALSE为关闭
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置电源开关状态成功
/// @retval DT_ERROR_FAILD：设置电源开关状态失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetOnOff(SENSOR_POWER Power[], BOOL OnOff[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取电源开关状态
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param OnOff：获取电源开关状态，TRUE为开启，FALSE为关闭
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：获取电源开关状态成功
/// @retval DT_ERROR_FAILD：获取电源开关状态失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuGetOnOff(SENSOR_POWER Power[], BOOL OnOff[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置过流保护的电流限制,设定值(CurrentLimit)单位:mA
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param CurrentLimit：设置过流保护的电流限制值，单位mA
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置过流保护的电流限制成功
/// @retval DT_ERROR_FAILD：设置过流保护的电流限制失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetOcpCurrentLimit(SENSOR_POWER Power[], int CurrentLimit[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置OCP延时(过流时间的累加),设定值(Delay)单位:mS
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Delay：OCP延时设定值
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置OCP延时成功
/// @retval DT_ERROR_FAILD：设置OCP延时失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetOcpDelay(SENSOR_POWER Power[], int Delay[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置OCP关断时间(被判断过流保护后，关断一段时间),设定值(Hold)单位:mS
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param Hold：OCP关断时间
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：设置OCP关断时间成功
/// @retval DT_ERROR_FAILD：设置OCP关断时间失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuSetOcpHold(SENSOR_POWER Power[], int Hold[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取过流状态,返回信息(State)TRUE:过流保护,FALSE:正常
///
/// @param Power：电源类型，参见枚举类型“SENSOR_POWER”
/// @param State：获取到的过流状态
/// @param iCount：电源路数
///
/// @retval DT_ERROR_OK：获取Ocp状态成功成功
/// @retval DT_ERROR_FAILED：获取Ocp状态成功失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
///
/// @see SENSOR_POWER
DTCCM_API int _DTCALL_ PmuGetOcpState(SENSOR_POWER Power[], BOOL State[], int iCount, int iDevID = DEFAULT_DEV_ID);

/*
// 设置电流采集触发点（MU950(TOF)机型支持，当高于设置的触发点电流时，启动电流采集）
PmuCurrentMeasurement_t sPmuCurrentMeasurement;

// (2019/4/26 MU950(TOF)机型VPP路支持设置)
sPmuCurrentMeasurementAux.PowerType = POWER_AUX;
sPmuCurrentMeasurementAux.bPosEn = TRUE;
sPmuCurrentMeasurementAux.lfTriggerPoint = 0; //当电流大于TriggerPoint后，启动电流采集,设置0，则是连续采集电流模式，如果设置为1000mA，则大于1000mA的电流才会启动采集
memset(sPmuCurrentMeasurementAux.Rsv, 0x00, sizeof(sPmuCurrentMeasurementAux.Rsv));

iRet = PmuSetCurrentMeasurement(&sPmuCurrentMeasurementAux, m_nDevID);
if (iRet != DT_ERROR_OK)
{
msg("PmuSetCurrentMeasurement failed with err:%d\r\n", iRet);
return iRet;
}

*/
/// @brief 电流测试设置
///
/// @param pPmuSetCurrentMeasurement：设置电流测试相关参数的结构体，（20190425支持电流采集触发点功能，大于设置的触发值，则启动电流采集，MU950（tof）机型VPP电源支持）
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILED：设置失败
DTCCM_API int _DTCALL_ PmuSetCurrentMeasurement(PmuCurrentMeasurement_t *pPmuSetCurrentMeasurement, int iDevID = DEFAULT_DEV_ID);

/*
@example
PmuFilterCap_t sPmuFilter;

sPmuFilter.PowerType = POWER_DVDD;
sPmuFilter.Mode = FILTER_CAP_MANUAL_SET_MODE;
sPmuFilter.bEn = FALSE;

memset(sPmuFilter.Rsv, 0x00, sizeof(sPmuFilter.Rsv));
iRet = PmuSetFilterCap(&sPmuFilter, m_nDevID);

*/

/// @brief 滤波电容设置
///
/// @param pFilterCap：设置滤波电容（CP20 AVDD/DVDD支持）
///
/// @retval DT_ERROR_OK：设置成功
/// @retval DT_ERROR_FAILED：设置失败
DTCCM_API int _DTCALL_ PmuSetFilterCap(PmuFilterCap_t *pFilterCap, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置SENSOR电源开关。建议使用PmuSetPowerOnOff
///
/// @param bOnOff：SENSOR电源开关设置值，TRUE为开启，FALSE为关闭
///
/// @retval DT_ERROR_OK：设置SENSOR电源开关成功
/// @retval DT_ERROR_FAILD：设置SENSOR电源开关失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_OUT_OF_BOUND：参数超出了范围
DTCCM_API int _DTCALL_ SetSensorPowerOnOff(BOOL bOnOff, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取sensor端管脚电压值（CP20提供6路ADC采集，该接口返回转换后的电压值）
///
/// @param AdcChannel：ADC通道指定
/// @param Voltage[]：返回的电压值
/// @param iCount：AD路数
///
/// @retval DT_ERROR_OK：获取成功
/// @retval DT_ERROR_NOT_SUPPORTED:不支持
/// @retval DT_ERROR_FAILD：获取失败
DTCCM_API int _DTCALL_ PmuGetAdcConvertVoltage(ADC_CHANNEL AdcChannel[], int Voltage[], int iCount, int iDevID = DEFAULT_DEV_ID);
/** @} */ // end of group5

/******************************************************************************
初始化，控制相关
******************************************************************************/
/** @defgroup group6 初始化控制相关


* @{

*/
/// @brief 打开设备，只有打开成功后，设备才能操作;设备对象跟给的ID对应起来iDevID=1 则创建设备对象m_device[1]，iDevID=0 则创建设备对象m_device[0]
///
/// @param pszDeviceName：打开设备的名称
/// @param pDevID：返回打开设备的ID号
///
/// @retval DT_ERROR_OK：打开设备成功
/// @retval DT_ERROR_FAILD：打开设备失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
/// @retval DT_ERROR_PARAMETER_INVALID：参数无效
DTCCM_API int _DTCALL_ OpenDevice(const char *pszDeviceName, int *pDevID, int iDevID = DEFAULT_DEV_ID);

/// @brief 重新连接设备，usb3.0设备有效，复位后会重新连接，CloseDevice会失效，必须重新OpenDevice
///
/// @retval DT_ERROR_OK：重新连接成功
/// @retval DT_ERROR_FAILD：重新连接失败
DTCCM_API int _DTCALL_ DeviceReset(int iDevID = DEFAULT_DEV_ID);

/// @brief 以升级固件模式打开设备，只有打开成功后，设备才能操作
///
/// @param pszDeviceName：打开设备的名称
/// @param pDevID：返回打开设备的ID号
///
/// @retval DT_ERROR_OK：打开设备成功
/// @retval DT_ERROR_FAILD：打开设备失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
/// @retval DT_ERROR_PARAMETER_INVALID：参数无效
DTCCM_API int _DTCALL_ OpenDeviceUpgradeMode(const char *pszDeviceName, int *pDevID, int iDevID = DEFAULT_DEV_ID);

/// @brief 关闭设备，关闭设备后，不能再操作
///
/// @retval DT_ERROR_OK：关闭设备成功
/// @retval DT_ERROR_FAILD：关闭设备失败
DTCCM_API int _DTCALL_ CloseDevice(int iDevID = DEFAULT_DEV_ID);

/// @brief 初始化设备，该函数主要用于初始化设备的SENSOR接口类型，图像格式，图像宽高信息，同时还要求用户传入用于视频显示的窗口句柄
///
/// @param hWnd：显示A通道图像的窗口句柄
/// @param uImgWidth，uImgHeight：设置SENSOR输出的宽高信息（单位：像素，可能ROI之后的结果）
/// @param bySensorPortType：SENSOR输出接口类型，如：MIPI/并行
/// @param byImgFormat：图像数据格式，sesor原始格式，如：RAW/YUV
/// @param byChannel：A通道/B通道/AB同时工作
/// @param hWndEx：显示B通道图像的窗口句柄
///
/// @retval DT_ERROR_OK：初始化成功
/// @retval DT_ERROR_FAILD：初始化失败
/// @retval DT_ERROR_PARAMETER_INVALID：bySensorPort参数无效
///
/// @note InitDevice函数支持初始化双通道测试板（如DTLC2/UH910），如果须要使用这类测试板的B通道，请做如下额外操作：
/// @note byChannel参数传入CHANNEL_A|CHANNEL_B；hWndEx参数传入用于B通道视频显示的窗口句柄
DTCCM_API int _DTCALL_ InitDevice(HWND hWnd,
								  USHORT uImgWidth,
								  USHORT uImgHeight,
								  BYTE bySensorPortType,
								  BYTE byImgFormat,
								  BYTE byChannel = CHANNEL_A,
								  HWND hWndEx = NULL,
								  int iDevID = DEFAULT_DEV_ID);

/// @brief 获取设备上按钮的按键值
///
/// @param pKey：获取到的按键值
///
/// @retval DT_ERROR_OK：获取按键值成功
/// @retval DT_ERROR_FAILED：获取按键值失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ GetKey(DWORD *pKey, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置报警喇叭
///
/// @param uBeepTime：蜂鸣器鸣叫时间，单位为mS
/// @param iBeepCount：蜂鸣器鸣叫次数
///
/// @retval DT_ERROR_OK：设置报警喇叭成功
/// @retval DT_ERROR_FAILED：设置报警喇叭失败
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ SetAlarm(USHORT uBeepTime, int iBeepCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 初始化ISP
///
/// @param uImgWidth：图像数据宽度,单位像素
/// @param uHeight：图像数据高度
/// @param byImgFormat：图像数据格式，sensor原始输出格式，只支持imagekit.h已经定义的格式，如：RAW/YUV
/// @param byChannel：A/B通道选择
DTCCM_API int _DTCALL_ InitIsp(USHORT uImgWidth, USHORT uImgHeight, BYTE byImgFormat, BYTE byChannel = CHANNEL_A, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置ROI
///
/// @param roi_x0：起始点水平坐标，单位像素
/// @param roi_y0：起始点垂直坐标，单位像素
/// @param roi_hw：水平方向ROI图像宽度，单位像素
/// @param roi_vw：垂直方向ROI图像高度，单位像素
/// @param roi_hb：水平方向blank宽度，单位像素
/// @param roi_vb：水平方向blank高度，单位像素
/// @param roi_hnum：水平方向ROI数量，单位像素
/// @param roi_vnum：垂直方向ROI数量，单位像素
/// @param byImgFormat：图像数据格式，如：RAW/YUV
/// @param roi_en：ROI使能
///
/// @retval DT_ERROR_OK：ROI设置成功
/// @retval DT_ERROR_FAILD：ROI设置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
///
/// @note 该函数中指定宽度和水平位置是以像素为单位，并且要保证宽度转为字节后是16字节的整数倍。
///
/// @image html roi.png
DTCCM_API int _DTCALL_ InitRoi(USHORT roi_x0,
							   USHORT roi_y0,
							   USHORT roi_hw,
							   USHORT roi_vw,
							   USHORT roi_hb,
							   USHORT roi_vb,
							   USHORT roi_hnum,
							   USHORT roi_vnum,
							   BYTE byImgFormat,
							   BOOL roi_en,
							   int iDevID = DEFAULT_DEV_ID);

/// 霍尔传感器状态获取
DTCCM_API int _DTCALL_ GetHallState(BOOL *pHallState, int iDevID = DEFAULT_DEV_ID);
/** @} */ // end of group6

/******************************************************************************
AF相关
******************************************************************************/

/** @defgroup group7 AF相关


* @{

*/
/// @brief 获取AF器件的D/A芯片型号和器件地址
///
/// @param pDevType：获取AF器件的D/A芯片型号
/// @param pDevAddr：获取AF器件的D/A器件地址
DTCCM_API int _DTCALL_ GetAfDevType(UCHAR *pDevType, UCHAR *pDevAddr, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置AF器件的D/A芯片型号和器件地址
///
/// @param uDevType：设置AF器件的D/A芯片型号
/// @param uDevAddr：设置AF器件的D/A器件地址
DTCCM_API int _DTCALL_ SetAfDevType(UCHAR uDevType, UCHAR uDevAddr, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置AF器件的D/A的输出值
///
/// @param uValue：设置的AF器件的D/A输出值
/// @param uMode：AF器件I2C的模式
///
/// @retval DT_ERROR_OK：设置AF器件的D/A的输出值成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：I2C模式无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
/// @retval DT_ERROR_NOT_SUPPORTED：不支持的AF器件类型
/// @retval DT_ERROR_NOT_INITIALIZED：没有初始化AF器件的D/A器件地址
DTCCM_API int _DTCALL_ WriteAfValue(USHORT uValue, UCHAR uMode, int iDevID = DEFAULT_DEV_ID);

/// @brief 搜索AF器件中的D/A芯片
///
/// @retval DT_ERROR_OK：搜索AF器件中的D/A芯片成功
/// @retval DT_ERROR_FAILD：搜索AF器件中的D/A芯片失败
DTCCM_API int _DTCALL_ FindAfDevice(int iDevID = DEFAULT_DEV_ID);
/** @} */ // end of group7

/****************************************************************************************
LC/OS相关
****************************************************************************************/

/** @defgroup group8 LC/OS相关


* @{

*/
/// @brief G22 OS测试快速模式的时候用来绑定哪两个sensor是一起的，同一个G22的两个sensor的ID必须一样
///
/// @param uGroupID:设置ID号
// DTCCM_API int _DTCALL_ SetOsTestGroup(UINT uGroupID, int iDevID = DEFAULT_DEV_ID);

/// @brief LC/OS测试操作配置
///
/// @param Command：操作码，参见宏定义“OS/LC测试配置定义”
/// @param IoMask：有效管脚标识位，每字节的每bit对应一个管脚，如果这些bit为1，表示对应的管脚将参与测试
/// @param PinNum：管脚数，这个决定IoMask数组大小，一般情况下IoMask的字节数为：PinNum/8+(PinNum%8!=0)
///
/// @retval DT_ERROR_OK：LC/OS测试操作配置成功
/// @retval DT_ERROR_FAILD：LC/OS测试操作配置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ LC_OS_CommandConfig(DWORD Command, UCHAR IoMask[], int PinNum, int iDevID = DEFAULT_DEV_ID);

/// @brief OS测试参数配置
///
/// @param Voltage：测试电压，单位uV
/// @param HighLimit：Open测试标准数组，测试之前应该把每一个测试pin的开路标准初始化好，单位uV
/// @param LowLimit：Short测试标准数组，测试之前应该把每一个测试pin的开路标准初始化好，单位uV
/// @param PinNum：管脚数，这个决定HighLimit、LowLimit数组大小
/// @param PowerCurrent：电源pin电流，单位uA
/// @param GpioCurrent：GPIOpin电流，单位uA
///
/// @retval DT_ERROR_OK：OS测试参数配置成功
/// @retval DT_ERROR_FAILD：OS测试参数配置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ OS_Config(int Voltage, int HighLimit[], int LowLimit[], int PinNum, int PowerCurrent, int GpioCurrent, int iDevID = DEFAULT_DEV_ID);

/// @brief OS测试结果读取
///
/// @param VoltageH：正向pos测试结果，单位uV
/// @param VoltageH：反向pos测试结果，单位uV
/// @param Result：开短路测试结果，参见宏定义“OS测试结果定义”，底层只对反向的值做判断（NegEn）
/// @param PosEn：正向测试使能，若不使能，则VoltageH结果无效
/// @param NegEn：反向测试使能，若不使能，则VoltageL结果无效
/// @param PinNum：管脚数，这个决定VoltageH、VoltageL，Result数组大小
///
/// @retval DT_ERROR_OK：OS测试结果读取成功
/// @retval DT_ERROR_FAILD：OS测试结果读取失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ OS_Read(int VoltageH[], int VoltageL[], UCHAR Result[], BOOL PosEn, BOOL NegEn, int PinNum, int iDevID = DEFAULT_DEV_ID);

/// @brief LC测试参数配置
///
/// @param Voltage：LC测试电压，单位mV
/// @param Range：LC精度设置，Range为0精度是1uA，Range为1精度是10nA
///
/// @retval DT_ERROR_OK：LC参数参数配置成功
/// @retval DT_ERROR_FAILD：LC测试参数配置失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ LC_Config(USHORT Voltage, UCHAR Range, int iDevID = DEFAULT_DEV_ID);

/// @brief LC测试结果读取
///
/// @param CurrentH：正向测试结果单位nA
/// @param CurrentL：反向测试结果单位nA
/// @param PinNum：管脚数，这个决定CurrentH、CurrentL，数组大小
///
/// @retval DT_ERROR_OK：LC测试结果读取成功
/// @retval DT_ERROR_FAILD：LC测试结果读取失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ LC_Read(ULONG CurrentH[], ULONG CurrentL[], int PinNum, int iDevID = DEFAULT_DEV_ID);

/// @brief 开关状态值读取
///
/// @param Result：返回开关状态值
/// @param PinNum：管脚数，这个决定Result，数组大小
///
/// @retval DT_ERROR_OK：LC开关状态值读取成功
/// @retval DT_ERROR_FAILD：LC开关状态值读取失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ LC_ReadSwitch(UCHAR Result[], int PinNum, int iDevID = DEFAULT_DEV_ID);

/*
@example:
	double resis;
	ResisCalculateForIoSet ioSET[4];


	DWORD dwCmd = 0;
	if (m_bResisCommonGround)    //如果共地
	{
		dwCmd = RESIS_TEST_COMMON_GROUND;
	}
	ioSET[0] = PO1; // 测试PO1上电阻值

	iRet = GetIoResistance(dwCmd, ioSET, &resis, 1, m_nDevID);
	if (iRet == DT_ERROR_OK)
	{
		msg("po1=%lf\r\n", resis);
	}
	else
	{
		msg("GetIoResistance failed with error code:%d\r\n", iRet);
	}
*/
/// @brief 获取PO1-PO4上电阻值,单位欧姆
///
/// @param IoSet[]：设置待测管脚
/// @param Resis[]：返回各管脚上接的电阻值大小
/// @param iIoNum：管脚个数设置，建议用一个管脚测试，iIoNum=1
///
/// @retval DT_ERROR_OK：获取成功
/// @retval DT_ERROR_NOT_SUPPORTED：不支持
DTCCM_API int _DTCALL_ GetIoResistance(DWORD dwCommand, ResisCalculateForIoSet IoSet[], double Resis[], UINT uIoNum, int iDevID = DEFAULT_DEV_ID);

/** @} */ // end of group8

/** @defgroup group9 扩展IO


* @{

*/
/************************************************************************
 扩展IO
************************************************************************/
/// @brief 设置外部扩展的3.3V电压输出
///
/// @param bEnable：扩展电压输出使能
///
/// @retval DT_ERROR_OK：设置外部扩展的3.3V电压输出成功
/// @retval DT_ERROR_FAILD：设置外部扩展的3.3V电压输出失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetExtPowerEnable(BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置外部扩展的时钟输出
///
/// @param bOnOff：使能时钟输出
/// @param uHundKhz：设置时钟输出值,单位Khz
///
/// @retval DT_ERROR_OK：设置外部扩展时钟输出成功
/// @retval DT_ERROR_FAILD：设置外部扩展时钟输出失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetExtClock(BOOL bOnOff, USHORT uHundKhz, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置外部扩展GPIO模式
///
/// @param Pin[]：GPIO编号，参见宏定义“EXT_GPIO”
/// @param Mode[]：模式设置，参见枚举类型“EXT_GPIO_MODE”
/// @param iCount：设置IO个数
///
/// @retval DT_ERROR_OK：设置外部扩展GPIO模式成功
/// @retval DT_ERROR_FAILD：设置外部扩展GPIO模式失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetExtIoMode(EXT_GPIO Pin[], EXT_GPIO_MODE Mode[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 恒定电平输出模式时设置外部扩展GPIO电平
///
/// @param Pin[]：GPIO编号，参见宏定义“EXT_GPIO”
/// @param Level[]：GPIO的电平值，FALSE为低电平，TRUE为高电平
/// @param iCount：设置IO个数
///
/// @retval DT_ERROR_OK：设置外部扩展GPIO电平成功
/// @retval DT_ERROR_FAILD：设置外部扩展GPIO电平失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetExtIoLevel(EXT_GPIO Pin[], BOOL Level[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取外部扩展GPIO的电平
///
/// @param Pin[]：GPIO编号，参见宏定义“EXT_GPIO”
/// @param Level[]：GPIO的电平值，FALSE为低电平，TRUE为高电平
/// @param iCount：设置IO个数
///
/// @retval DT_ERROR_OK：获取外部扩展GPIO电平成功
/// @retval DT_ERROR_FAILD：获取外部扩展GPIO电平失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetExtIoLevel(EXT_GPIO Pin[], BOOL Level[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 扩展GPIO为高低电平交互输出时，高、低电平时间设置
///
/// @param Pin[]：GPIO编号，参见宏定义“EXT_GPIO”
/// @param HighLevelTime[]：对应扩展GPIO高电平时间，单位us
/// @param LowLevelTime[]：对应扩展GPIO低电平时间，单位us
/// @param iCount：设置IO个数
///
/// @retval DT_ERROR_OK：设置外部扩展GPIO高低电平时间成功
/// @DT_ERROR_FAILD设置外部扩展GPIO高低电平时间失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetExtIoLevelTime(EXT_GPIO Pin[], int HighLevelTime[], int LowLevelTime[], int iCount, int iDevID = DEFAULT_DEV_ID);

/// @brief 外部扩展I2C写
///
/// @param uDevAddr：设备地址
/// @param uRegAddr：设备寄存器
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：写入寄存器的数据块
/// @param uSize：写入寄存器的数据块大小,最大为60个字节
///
/// @retval DT_ERROR_OK：写成功
/// @retval DT_ERROR_FAILD：写失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ ExtI2cWrite(UCHAR uDevAddr, USHORT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, int iDevID = DEFAULT_DEV_ID);

/// @brief 外部扩展I2C读
///
/// @param uDevAddr：设备地址
/// @param uRegAddr：设备寄存器
/// @param uRegAddrSize：寄存器地址的字节数
/// @param pData：读寄存器的数据块
/// @param uSize：读寄存器的数据块大小,最大为60个字节
/// @param bNoStop：是否发出I2C的STOP命令，一般情况下默认为FALSE，bNoStop=TRUE表示写的过程不会有I2C的停止命令，bNoStop=FALSE有I2C的停止命令
///
/// @retval DT_ERROR_OK：读成功
/// @retval DT_ERROR_FAILD：读失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ ExtI2cRead(UCHAR uDevAddr, USHORT uRegAddr, UCHAR uRegAddrSize, BYTE *pData, USHORT uSize, BOOL bNoStop = FALSE, int iDevID = DEFAULT_DEV_ID);

/// @brief 设置外部触发输出接口电平
///
/// @param iTriggerOutPin：触发输出接口编号（美鸥力产品提供一个触发输出IO）
/// @param bFollowTrigIn：触发输出接口跟随使能信号，若为1则跟随TriggerIn信号电平，为0则为设置电平值
/// @param bLevel：电平值（0为输出低电平，1为输出高电平）
///
/// @retval DT_ERROR_OK：读成功
/// @retval DT_ERROR_FAILD：读失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ SetTriggerOutLevel(int iTriggerOutPin, BOOL bFollowTrigIn, BOOL bLevel, int iDevID = DEFAULT_DEV_ID);

/// @brief 获取外部触发输入接口电平
///
/// @param iTriggerInPin：触发输入接口编号（美鸥力产品提供一个触发输出IO）
/// @param pLevel：返回的电平值（0为输出低电平，1为输出高电平）
///
/// @retval DT_ERROR_OK：读成功
/// @retval DT_ERROR_FAILD：读失败
/// @retval DT_ERROR_COMM_ERROR：通讯错误
DTCCM_API int _DTCALL_ GetTriggerInLevel(int iTriggerInPin, BOOL *pLevel, int iDevID = DEFAULT_DEV_ID);

/** @} */ // end of group9

/*************************************************************************
温度获取和风扇控制相关
*************************************************************************/
/// 设备默认自动控制风扇转动
/// 设置风扇转动的上下限温度值,低于下限值则不转动风扇，高于下限值低于上限值，风扇低速运转，高于上限值，风扇高速运转
/// 注意：G40高于下限值，低于上限值，1个风扇转动，高于上限值，2个风扇转动
///
/// @param iUpperLimit：上限温度值，最大125
/// @param iLowerLimit：下限温度值
/// @bEnable：=TRUE,手动使能风扇转动，=FALSE,根据上下限温度值，自动调节设备风扇转动
DTCCM_API int _DTCALL_ SetTempRange(int iUpperLimit, int iLowerLimit, BOOL bEnable, int iDevID = DEFAULT_DEV_ID);

/// 读取当前温度值
///
/// @param pTemp：当前设备的温度值
DTCCM_API int _DTCALL_ GetCurrentTemp(int *pTemp, int iDevID = DEFAULT_DEV_ID);

/// 读取当前设置的温度范围
DTCCM_API int _DTCALL_ GetTempRange(int *pUpperLimit, int *pLowLimit, int iDevID = DEFAULT_DEV_ID);

/*************************************************************************
状态框
*************************************************************************/
/** @defgroup group10 调试状态框


* @{

*/

/// @brief 状态信息界面，监控设备出图后数据流的情况
///
/// @param hwnd：用户程序的句柄
DTCCM_API int _DTCALL_ ShowInternalStatusDialog(HWND hwnd, int iDevID = DEFAULT_DEV_ID);

/// @brief 状态信息界面，监控设备出图后数据流的情况
///
/// @param hwnd：用户程序的句柄
/// @param pRetHwnd：返回内部状态信息框界面的句柄
DTCCM_API int _DTCALL_ ShowInternalStatusDialogEx(HWND hwnd, HWND *pRetHwnd, int iDevID = DEFAULT_DEV_ID);

DTCCM_API int _DTCALL_ ShowInternalDebugDialog(HWND hwnd, int iDevID = DEFAULT_DEV_ID);
DTCCM_API int _DTCALL_ ShowInternalDebugDialogEx(HWND hwnd, HWND *pRetHwnd, int iDevID = DEFAULT_DEV_ID);

/// @brief 属性框界面
DTCCM_API int _DTCALL_ ShowMipiControllerDialog(HWND hwnd, HWND *pRetHwnd, int iDevID = DEFAULT_DEV_ID);

/// @brief 属性框界面
DTCCM_API int _DTCALL_ ShowFrameBufferControlDialog(HWND hwnd, HWND *pRetHwnd, int iDevID = DEFAULT_DEV_ID);
/** @} */ // end of group10

/** @defgroup group11 dtccmEasy接口


* @{

*/

/// @brief 用户可以在出现异常后，立即调用这个接口，会立即抓取当前设备的状态信息，并打印到log
///
/// @param report：参见DtDbgReport_t结构体描述
/// @note:避免频繁调用
DTCCM_API int _DTCALL_ DebugReport(DtDbgReport_t report, int iDevID = DEFAULT_DEV_ID);

/// @brief 读写SENSOR I2C接口（2019/5/23更新支持用户读过程中，写入数据超过4字节的需求）
///
/// @param pSensorI2cRw：SensorI2cRw结构体
///
/// @retval DT_ERROR_OK：操作成功
/// @retval DT_ERROR_COMM_ERROR：通讯错误
/// @retval DT_ERROR_PARAMETER_INVALID：参数无效
/// @retval DT_ERROR_TIME_OUT：通讯超时
/// @retval DT_ERROR_INTERNAL_ERROR：内部错误
DTCCM_API int _DTCALL_ ezSensorI2cRw(ezSensorI2cRw_t *pSensorI2cRw, int iDevID = DEFAULT_DEV_ID);

/** @} */ // end of group11

///< SetLibConfig函数说明
///< DEBUG模式下，编程人员想要单步调试，建议在构造函数中做如下调用
///< SetLibConfig(LIB_DEBUG_CFG, DBG_MODE_ENABLE);

///< uCtrl定义
#define LIB_CFG_CTRL_DEBUG 0x10 ///< DEBUG 模式配置
///< uParam定义
#define LIB_CFG_PARAM_ENABLE 1	///< 用户单步调试需要使能调试模式
#define LIB_CFG_PARAM_DISABLE 0 ///< Release版本关闭调试模式

///< Lib配置模式设置
DTCCM_API int _DTCALL_ SetLibConfig(UINT uCtrl, UINT uParam);

/****************************************************************************************
其他
****************************************************************************************/
///< dwCtrl定义
#define EXCTRL_FAN_CFG_LEVEL 2050 // 风扇档位配置

///< dwParam定义

typedef enum
{
	FAN_CFG_PARAM_AUTO = 0x00,	  // 自动模式
	FAN_CFG_PARAM_DISABLE = 0x01, // 关闭风扇
	FAN_CFG_PARAM_LEVEL1 = 0x02,  // 风扇档位1
	FAN_CFG_PARAM_LEVEL2 = 0x03,  // 风扇档位2
	FAN_CFG_PARAM_LEVEL3 = 0x04,  // 风扇档位3
	FAN_CFG_PARAM_LEVEL4 = 0x05,  // 风扇档位4
} FANCONFIG;

/* @example G40控制风扇

	// 设置风扇为自动模式
	int iRet = ExCtrl(EXCTRL_FAN_CFG_LEVEL, FAN_CFG_PARAM_AUTO, NULL, NULL, NULL, NULL, m_nDevID);
	if (iRet != DT_ERROR_OK)
	{
		msg("Fan ctrl failed with err : %d\r\n",iRet);
		return;
	}
	// 设置风扇为档位1
	int iRet = ExCtrl(EXCTRL_FAN_CFG_LEVEL, FAN_CFG_PARAM_LEVEL1, NULL, NULL, NULL, NULL, m_nDevID);
	if (iRet != DT_ERROR_OK)
	{
		msg("Fan ctrl failed with err : %d\r\n",iRet);
		return;
	}
*/
///< dwCtrl定义
#define EXCTRL_PMU_GET_ACTUAL_VOLTAGE 1004
#define EXCTRL_PMU_RESIS_VALUE_CTRL 1005
#define EXCTRL_PMU_CALIBRATION_RESIS_FINISH 1006 // 电阻校准完成
#define EXCTRL_PMU_SET_VPP_THRESHOLD_VALUE 1007
// 给用户的控制码,EXCTRL函数使用
#define EXCTRL_PMU_LDO_CTRL 1000 /* ExCtrl函数参数dwParam的低2位控制LDO，BIT0控制AVDD,BIT1控制AVDD2。为1是开启LDO,为0是关闭LDO */
#define EXCTRL_PMU_CAP_CTRL 1001 /* ExCtrl函数参数dwParam的低2位控制CAP，BIT0控制AVDD,BIT1控制AVDD2。为1是开启CAP,为0是关闭CAP */

#define EXCTRL_GET_LOSS_FRAME_INFO 1100 // 丢帧信息获取

// 帧丢失结构体
typedef struct _FrameLossInfo
{
	// 采集帧超时
	UINT uGrabTimeoutCnt;
	// 只丢1帧计数
	UINT uContinuous1FrameLossCnt;
	// 连续丢2帧计数
	UINT uContinuous2FrameLossCnt;
	// 连续丢3帧计数
	UINT uContinuous3FrameLossCnt;
	// 连续丢4帧计数
	UINT uContinuous4FrameLossCnt;

	UINT rsv1[16];
	// 丢帧百分比，和总帧数的百分比
	double lfTimeOutPercent;
	// 只丢1帧百分比
	double lfContinuous1FrameLossPercent;
	// 连续丢2帧百分比
	double lfContinuous2FrameLossPercent;
	// 连续丢3帧百分比
	double lfContinuous3FrameLossPercent;
	// 连续丢4帧百分比
	double lfContinuous4FrameLossPercent;
	// 总的丢帧百分比，有些帧可能在盒子里面已经丢弃了（采集带宽小于sensor带宽的时候）
	double lfAllFrameLossPercent;
	double rsv2[16];
} FrameLossInfo;

/*@example

	FrameLossInfo sFrameLossInfo;
	int iRet = ExCtrl(EXCTRL_GET_LOSS_FRAME_INFO,null,sFrameLossInfo,null,null,null,iDevID);
	if (iRet != DT_ERROR_OK)
		return iRet;
*/

// 扩展控制接口
// dwCtrl:	控制码，可能由派生类定义
// dwParam:	控制码相关参数
// pIn:		读取的数据
// pInSize:	读取数据的大小，字节数，调用者指定要求读取的字节数，如果成功，返回实际读取的字节数
// pOut:	写入的数据
// pOutSize:写入数据的大小，字节数，调用者指定要求写入的字节数，如果成功，返回实际写入的字节数
DTCCM_API int _DTCALL_ ExCtrl(DWORD dwCtrl, DWORD dwParam, PVOID pIn, int *pInSize, PVOID pOut, int *pOutSize, int iDevID = DEFAULT_DEV_ID);

#endif