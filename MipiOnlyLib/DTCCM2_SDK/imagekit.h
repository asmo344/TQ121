#ifndef __IMAGEKIT_H__
#define __IMAGEKIT_H__

/****************************************************************************************
 *
 * imagekitϵ�и���Ӳ���汾����
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
 * SENSOR�ĳ�ʼ����������
 *
 ****************************************************************************************/

typedef struct _SensorTab
{
	/// @brief SENSOR���
	USHORT width; ///< SENSOR���
	/// @brief SENSOR�߶�
	USHORT height; ///< SENSOR�߶�
	/// @brief SENSOR��������
	BYTE type; ///< SENSOR��������
	/// @brief SENSOR��RESET��PWDN��������
	BYTE pin; ///< SENSOR��RESET��PWDN��������
	/// @brief SENSOR��������ַ
	BYTE SlaveID; ///< SENSOR��������ַ
	/// @brief SENSOR��I2Cģʽ
	BYTE mode; ///< SENSOR��I2Cģʽ
	/// @brief SENSOR��־�Ĵ���1.
	USHORT FlagReg; ///< SENSOR��־�Ĵ���1.
	/// @brief SENSOR��־�Ĵ���1��ֵ
	USHORT FlagData; ///< SENSOR��־�Ĵ���1��ֵ
	/// @brief SENSOR��־�Ĵ���1������ֵ
	USHORT FlagMask; ///< SENSOR��־�Ĵ���1������ֵ
	/// @brief SENSOR��־�Ĵ���2.
	USHORT FlagReg1; ///< SENSOR��־�Ĵ���2.
	/// @brief SENSOR��־�Ĵ���2��ֵ
	USHORT FlagData1; ///< SENSOR��־�Ĵ���2��ֵ
	/// @brief SENSOR��־�Ĵ���2������ֵ
	USHORT FlagMask1; ///< SENSOR��־�Ĵ���2������ֵ
	/// @brief SENSOR������
	char name[64]; ///< SENSOR������

	/// @brief ��ʼ��SENSOR���ݱ�
	USHORT *ParaList; ///< ��ʼ��SENSOR���ݱ�
	/// @brief ��ʼ��SENSOR���ݱ��С����λ�ֽ�
	USHORT ParaListSize; ///< ��ʼ��SENSOR���ݱ��С����λ�ֽ�

	/// @brief SENSOR����Sleepģʽ�Ĳ�����
	USHORT *SleepParaList; ///< SENSOR����Sleepģʽ�Ĳ�����
	/// @brief SENSOR����Sleepģʽ�Ĳ������С����λ�ֽ�
	USHORT SleepParaListSize; ///< SENSOR����Sleepģʽ�Ĳ������С����λ�ֽ�

	/// @brief SENSOR������ݸ�ʽ��YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	BYTE outformat; ///< SENSOR������ݸ�ʽ��YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	/// @brief SENSOR������ʱ��MCLK��0:12M; 1:24M; 2:48M.
	UINT mclk; ///< SENSOR������ʱ��MCLK��0:12M; 1:24M; 2:48M.
	/// @brief SENSOR��AVDD��ѹֵ
	UINT avdd; ///< SENSOR��AVDD��ѹֵ
	/// @brief SENSOR��AVDD2��ѹֵ
	UINT avdd2; ///< SENSOR��AVDD2��ѹֵ
	/// @brief SENSOR��DVDD��ѹֵ
	UINT dvdd; ///< SENSOR��DVDD��ѹֵ
	/// @brief SENSOR��DOVDD��ѹֵ
	UINT dovdd; ///< SENSOR��DOVDD��ѹֵ
	/// @brief SENSOR��AFVCC��ѹֵ
	UINT afvcc; ///< SENSOR��AFVCC��ѹֵ
	/// @brief SENSOR��VPP2��ѹֵ
	UINT vpp2; ///< SENSOR��VPP2��ѹֵ
	/// @brief SENSOR��OISVDD��ѹֵ
	UINT oisvdd; ///< SENSOR��OISVDD��ѹֵ

	/// @brief SENSOR�����ݽӿ�����
	BYTE port; ///< SENSOR�����ݽӿ�����
	USHORT Ext0;
	USHORT Ext1;
	USHORT Ext2;

	/// @brief AF��ʼ��������
	USHORT *AF_InitParaList; ///< AF��ʼ��������
	/// @brief AF��ʼ���������С����λ�ֽ�
	USHORT AF_InitParaListSize; ///< AF��ʼ���������С����λ�ֽ�

	/// @brief AF_AUTO������
	USHORT *AF_AutoParaList; ///< AF_AUTO������
	/// @brief AF_AUTO�������С����λ�ֽ�
	USHORT AF_AutoParaListSize; ///< AF_AUTO�������С����λ�ֽ�

	/// @brief AF_FAR������
	USHORT *AF_FarParaList; ///< AF_FAR������
	/// @brief AF_FAR�������С����λ�ֽ�
	USHORT AF_FarParaListSize; ///< AF_FAR�������С����λ�ֽ�

	/// @brief AF_NEAR������
	USHORT *AF_NearParaList; ///< AF_NEAR������
	/// @brief AF_NEAR�������С����λ�ֽ�
	USHORT AF_NearParaListSize; ///< AF_NEAR�������С����λ�ֽ�

	/// @brief �ع������
	USHORT *Exposure_ParaList; ///< �ع������
	/// @brief �ع�������С����λ�ֽ�
	USHORT Exposure_ParaListSize; ///< �ع�������С����λ�ֽ�

	/// @brief ���������
	USHORT *Gain_ParaList; ///< ���������
	/// @brief ����������С����λ�ֽ�
	USHORT Gain_ParaListSize; ///< ����������С����λ�ֽ�

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

		Exposure_ParaList = NULL; // �ع�
		Exposure_ParaListSize = 0;

		Gain_ParaList = NULL; // ����
		Gain_ParaListSize = 0;
	}
} SensorTab, *pSensorTab;

// ����SensorTab, ����SensorTab2����
///////////////////////////////////////////////////////////
typedef struct _SensorTab2
{
	/// @brief SENSOR���
	UINT width; ///< SENSOR���
	/// @brief SENSOR�߶�
	UINT height; ///< SENSOR�߶�

	UINT Quick_w; ///< Quick View ���
	UINT Quick_h; ///< Quick View �߶�

	/// @brief SENSOR��������
	UINT type; ///< SENSOR��������
	/// @brief SENSOR��RESET��PWDN��������
	UINT pin; ///< SENSOR��RESET��PWDN��������
	/// @brief SENSOR��������ַ
	UINT SlaveID; ///< SENSOR��������ַ
	/// @brief SENSOR��I2Cģʽ
	UINT mode; ///< SENSOR��I2Cģʽ
	/// @brief SENSOR��־�Ĵ���1.
	UINT FlagReg; ///< SENSOR��־�Ĵ���1.
	/// @brief SENSOR��־�Ĵ���1��ֵ
	UINT FlagData; ///< SENSOR��־�Ĵ���1��ֵ
	/// @brief SENSOR��־�Ĵ���1������ֵ
	UINT FlagMask; ///< SENSOR��־�Ĵ���1������ֵ
	/// @brief SENSOR��־�Ĵ���2.
	UINT FlagReg1; ///< SENSOR��־�Ĵ���2.
	/// @brief SENSOR��־�Ĵ���2��ֵ
	UINT FlagData1; ///< SENSOR��־�Ĵ���2��ֵ
	/// @brief SENSOR��־�Ĵ���2������ֵ
	UINT FlagMask1; ///< SENSOR��־�Ĵ���2������ֵ
	/// @brief SENSOR������
	char name[64]; ///< SENSOR������

	/// @brief ��ʼ��SENSOR���ݱ�
	UINT *ParaList; ///< ��ʼ��SENSOR���ݱ�
	/// @brief ��ʼ��SENSOR���ݱ��С����λ�ֽ�
	UINT ParaListSize; ///< ��ʼ��SENSOR���ݱ��С����λ�ֽ�

	/// @brief SENSOR����Sleepģʽ�Ĳ�����
	UINT *SleepParaList; ///< SENSOR����Sleepģʽ�Ĳ�����
	/// @brief SENSOR����Sleepģʽ�Ĳ������С����λ�ֽ�
	UINT SleepParaListSize; ///< SENSOR����Sleepģʽ�Ĳ������С����λ�ֽ�

	/// @brief SENSOR����Quick Viewģʽ�Ĳ�����
	UINT *QuickParaList; ///< SENSOR����Quick Viewģʽ�Ĳ�����
	/// @brief SENSOR����Quick Viewģʽ�Ĳ������С����λ�ֽ�
	UINT QuickParaListSize; ///< SENSOR����Quick Viewģʽ�Ĳ������С����λ�ֽ�

	/// @brief SENSOR������ݸ�ʽ��YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	UINT outformat; ///< SENSOR������ݸ�ʽ��YUV//0:YCbYCr;	//1:YCrYCb;	//2:CbYCrY;	//3:CrYCbY.
	/// @brief SENSOR������ʱ��MCLK��0:12M; 1:24M; 2:48M.
	UINT mclk; ///< SENSOR������ʱ��MCLK��0:12M; 1:24M; 2:48M.
	/// @brief SENSOR��AVDD��ѹֵ
	UINT avdd; ///< SENSOR��AVDD��ѹֵ
	/// @brief SENSOR��DOVDD��ѹֵ
	UINT dovdd; ///< SENSOR��DOVDD��ѹֵ
	/// @brief SENSOR��DVDD��ѹֵ
	UINT dvdd; ///< SENSOR��DVDD��ѹֵ

	/// @brief SENSOR�����ݽӿ�����
	UINT port; ///< SENSOR�����ݽӿ�����
	UINT Ext0;
	UINT Ext1;
	UINT Ext2;

	/// @brief AF��ʼ��������
	UINT *AF_InitParaList; ///< AF��ʼ��������
	/// @brief AF��ʼ���������С����λ�ֽ�
	UINT AF_InitParaListSize; ///< AF��ʼ���������С����λ�ֽ�

	/// @brief AF_AUTO������
	UINT *AF_AutoParaList; ///< AF_AUTO������
	/// @brief AF_AUTO�������С����λ�ֽ�
	UINT AF_AutoParaListSize; ///< AF_AUTO�������С����λ�ֽ�

	/// @brief AF_FAR������
	UINT *AF_FarParaList; ///< AF_FAR������
	/// @brief AF_FAR�������С����λ�ֽ�
	UINT AF_FarParaListSize; ///< AF_FAR�������С����λ�ֽ�

	/// @brief AF_NEAR������
	UINT *AF_NearParaList; ///< AF_NEAR������
	/// @brief AF_NEAR�������С����λ�ֽ�
	UINT AF_NearParaListSize; ///< AF_NEAR�������С����λ�ֽ�

	/// @brief �ع������
	UINT *Exposure_ParaList; ///< �ع������
	/// @brief �ع�������С����λ�ֽ�
	UINT Exposure_ParaListSize; ///< �ع�������С����λ�ֽ�

	/// @brief ���������
	UINT *Gain_ParaList; ///< ���������
	/// @brief ����������С����λ�ֽ�
	UINT Gain_ParaListSize; ///< ����������С����λ�ֽ�

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

		Exposure_ParaList = NULL; // �ع�
		Exposure_ParaListSize = 0;

		Gain_ParaList = NULL; // ����
		Gain_ParaListSize = 0;
	}
} SensorTab2, *pSensorTab2;

/** @defgroup group2 ISP���
@{

*/

/** @name SENSOR���ͼ�����Ͷ���(SensorTab::type��ȡֵ����)
@{

*/
/* SENSOR���ͼ�����Ͷ���(SensorTab::type��ȡֵ����)�� ����ʹ�ö�Ӧ��IMAGE_FORMATö������ */
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

#define D_BGR24 0x20 ///< ����˳��ΪB��G��R����8bit
#define D_BGR32 0x21 ///< ����˳��ΪB��G��R��0��8bit
#define D_P10 0x24	 ///< һ������ռ�����ֽڣ�LSB��0��1023��һ������MIPI_RAW10ת��
#define D_P12 0x25	 ///< һ������ռ�����ֽڣ�LSB��0��4095��һ������MIPI_RAW12ת��

#define D_G8 0x28
#define D_G10 0x29
#define D_GRAY8 0x2a
// #define D_RGB24             0x0b
// #define D_HISPI_SP			0x09    //aptina hispi packet sp.
/** @} */

/** @name RAWתRGB�㷨����
@{

*/
/* RAWתRGB�㷨���� */
#define RAW2RGB_NORMAL 0
#define RAW2RGB_SMOOTH 1
#define RAW2RGB_SHARP 2
/** @} */

/** @name YUVͼ��4�������ʽ����
@{

*/
/* RAW��YUVͼ��4�������ʽ����(SensorTab::outformat��ȡֵ����) */
/// YUVͼ��4�������ʽ���塣
enum OUTFORMAT_YUV
{
	OUTFORMAT_YCbYCr = 0, ///< YCbYCr�����ʽ
	OUTFORMAT_YCrYCb,	  ///< YCrYCb�����ʽ
	OUTFORMAT_CbYCrY,	  ///< CbYCrY�����ʽ
	OUTFORMAT_CrYCbY,	  ///< CrYCbY�����ʽ
};
/** @} */

/** @name RAWͼ��4�������ʽ����
@{

*/
/// RAWͼ��4�������ʽ���塣
enum OUTFORMAT_RGB
{
	OUTFORMAT_RGGB = 0, ///< RGGB�����ʽ
	OUTFORMAT_GRBG,		///< GRBG�����ʽ
	OUTFORMAT_GBRG,		///< GBRG�����ʽ
	OUTFORMAT_BGGR,		///< BGGR�����ʽ
};
/** @} */

/* ��ϵͳ֧�ֵ�RAW��ʽ��YUV��ʽ���� */
/** @name ֧�ֵ�RAW��ʽ����
@{

*/
/// ֧�ֵ�RAW��ʽ��
enum RAW_FORMAT
{
	RAW_RGGB = 0, ///< RAW��ʽ��RGGB����
	RAW_GRBG,	  ///< RAW��ʽ��GRBG����
	RAW_GBRG,	  ///< RAW��ʽ��GBRG����
	RAW_BGGR,	  ///< RAW��ʽ��BGGR����
};
/** @} */

/** @name ֧�ֵ�YUV��ʽ����
@{

*/
/// ֧�ֵ�YUV��ʽ��
enum YUV_FORMAT
{
	YUV_YCBYCR = 0, ///< YUV��ʽ��YCBYCR����
	YUV_YCRYCB,		///< YUV��ʽ��YCRYCB����
	YUV_CBYCRY,		///< YUV��ʽ��CBYCRY����
	YUV_CRYCBY,		///< YUV��ʽ��CRYCBY����
};
/** @} */

/** @name ͼ����ģʽѡ��
@{

*/
/// ͼ����ģʽѡ��
enum ISP_MODE
{
	NORMAL = 0,	 /// ��ͨ�Ĵ���ģʽ��ץ֡�õ�����raw����
	S2DFAST,	 /// S2DFASTģʽ��ץ֡�õ�����RGB����
	S2DFAST_GPU, /// S2DFAST_GPUģʽ��GPU����ͼ����ץ֡�õ�����RGB����
};

/** @} */

/** @name ͼ���ʽ����
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

	FORMAT_BGR24 = 0x20, ///< ����˳��ΪB��G��R����8bit
	FORMAT_BGR32 = 0x21, ///< ����˳��ΪB��G��R��0��8bit
	FORMAT_P10 = 0x24,	 ///< һ������ռ�����ֽڣ�LSB��0��1023��һ������MIPI_RAW10ת��
	FORMAT_P12 = 0x25,	 ///< һ������ռ�����ֽڣ�LSB��0��4095��һ������MIPI_RAW12ת��
	FORMAT_G8 = 0x28,	 ///< ֻȡGֵ��8bit
	FORMAT_G10 = 0x29,	 ///< ֻȡGֵ��16bit
	FORMAT_GRAY8 = 0x2a,
};
/** @} */

/** @brief ROI�ṹ������
@{

*/
typedef struct DtRoi_s
{
	UINT x; ///< ROI��ʼ��X����ֵ
	UINT y; ///< ROI��ʼ��Y����ֵ
	UINT w; ///< ROI���
	UINT h; ///< ROI�߶�
} DtRoi_t;

/** @} */

/** @brief ͼ�����ݽṹ������
@{

*/
typedef struct DtImage_s
{
	IMAGE_FORMAT format;   ///< ͼ���ʽ
	RAW_FORMAT rawFmt;	   ///< RAW��ʽϸ��
	YUV_FORMAT yuvFmt;	   ///< YUV��ʽϸ��
	UINT width;			   ///< ͼ��ߴ�
	UINT height;		   ///< ͼ��ߴ�
	BYTE *data;			   ///< ͼ������
	unsigned int dataSize; ///< buffer�ռ��С
	UINT resv[8];		   ///< ����32�ֽ�
} DtImage_t;
/** @} */

/** @} */ // end of group2

/** @defgroup group3 SENSOR���


* @{

*/

/** @name SENSOR�Ĵ����������и����Ŀ����ֶ���
@{

*/
/* SENSOR�Ĵ����������и����Ŀ����ֶ��� */

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

/** @name SENSOR��Դ��ѹѡ���壨dtccmʹ�õĶ��壩
@{

*/
/* SENSOR��Դ��ѹѡ���� */
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

/** @name SENSOR����ʱ��ѡ����
@{

*/
/* SENSOR����ʱ��ѡ���� */
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

/** @name ��SENSORģ��ͨ������(Ŀǰֻ��DTLC2�д���CHANNEL_B)
@{

*/
/* ��SENSORģ��ͨ������(Ŀǰֻ��DTLC2/UH920�д���CHANNEL_B) **/
#define CHANNEL_A 0x01	/// ֻʹ��Aͨ��
#define CHANNEL_B 0x02	// ֻ����Bͨ��
#define CHANNEL_AB 0x03 // ABͨ��ͬʱʹ��
/** @} */

/** @name SensorEnable�����У�ʹ��SENSORʱ��RESET/PWDN�ܽŵĵ�ƽ״̬����
@{

*/
/* SensorEnable�����У�ʹ��SENSORʱ��RESET/PWDN�ܽŵĵ�ƽ״̬���� */
#define RESET_H 0x02
#define RESET_L 0x00
#define PWDN_H 0x01
#define PWDN_L 0x00
#define PWDN2_H 0x04
#define PWDN2_L 0x00
/** @} */

/** @name ֧�ֵ�SENSOR���ݽӿڶ���
@{

*/
/// ����֧�ֵ�SENSOR���ݽӿ����͡�
typedef enum
{
	SENSOR_PORT_MIPI = 0x00,			   ///< MIPI�ӿ�
	SENSOR_PORT_PARA = 0x01,			   ///< ����ͬ���ӿ�
	SENSOR_PORT_MTK_SERIAL = 0x02,		   ///< MTK��˾�Ĵ��нӿ�
	SENSOR_PORT_SPI = 0x03,				   ///< SPI�ӿ�
	SENSOR_PORT_SIM = 0x04,				   ///< ģ��ͼ�����ڲ���
	SENSOR_PORT_HISPI = 0x05,			   ///< Aptina��HISPI�ӿ�,֧��packet sp��ʽ
	SENSOR_PORT_ZX_SERIAL = 0x06,		   ///< չѶ�Ĵ��нӿ�
	SENSOR_PORT_SAMSUNG_DVS = 0x07,		   ///< ����DVS
	SENSOR_PORT_HISILICON_LVDS = 0x08,	   ///< ��˼LVDS
	SENSOR_PORT_SMARTSENS_DDR_4BIT = 0x09, ///< ˼��΢DDR 4bit

	SENSOR_PORT_SLVS_EC = 0x40, ///< sony slvs-ec

	SENSOR_PORT_SONY_LVDS = 0x81,	   ///< ���� LVDS
	SENSOR_PORT_SMARTSENS_LVDS = 0x82, ///< ˼���� LVDS
	SENSOR_PORT_PANASONIC_LVDS = 0x85, ///< ���� LVDS
	SENSOR_PORT_SUPERPIX_LVDS = 0x86,  ///< ˼�ȿ� LVDS
} SENSOR_PORT;
/** @} */

/** @name ���ڰ汾ʹ�õĺ궨��
@{

*/
/* ���ڰ汾ʹ�õĺ궨�� */
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
/** @name �������Խӿ��еĸ��ֹܽŹ���
@{

*/
/* �������Խӿ��еĸ��ֹܽŹ��� */
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
	PIN_SPI_SCK, ///< ���Խӿ� ����SPI�ӿ�
	PIN_SPI_CS,
	PIN_SPI_SDI,
	PIN_SPI_SDO,
	PIN_SPI_SDA,
	PIN_CLK_ADJ_200K, ///< ���0-200Khz�Ŀɵ���ʱ��Ƶ��
	PIN_CLK_ADJ_18M,  ///< ���0-18Mhz�Ŀɵ���ʱ��Ƶ��
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

/** @name �������Խӿڹܽ�����(���)
@{

*/
/* �������Խӿڹܽ�����(���) */
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

/* MIPI ctrl ��չ�ṹ�� */
typedef struct MipiCtrlEx_s
{
	BYTE byPhyType; ///< mipi phy set (d-phy_1.5G/d-phy_2.5G,c-phy_2.0G

	BYTE byLaneCnt; ///< lane�������ã���1/2/4lane

	DWORD dwCtrl; ///< MIPI ctrl,ʹ��λ���壬��MIPI_CTRL_XXXX����

	UINT uVc; ///< ���ý��յ�ͼ��ͨ���ţ�0/1/2/3

	BOOL bVCFilterEn; ///< ʹ�ܹ�������������ͨ��

	UINT uPackID; ///< ʹ�������ID��

	BOOL bPackIDEn; ///< ʹ�ܵ�ǰ���õ�ID����� */

	/* ���������0 */
	BYTE resv[62];

} MipiCtrlEx_t;

/** @name MIPI���������Ե�λ����
@{

*/
/* MIPI���������Ե�λ���� */
#define MIPI_CTRL_LP_EN 1						///< �������LP״̬
#define MIPI_CTRL_AUTO_START (1 << 1)			///< ���ֲ���źź��Զ�����������OS����
#define MIPI_CTRL_NON_CONT (1 << 2)				///< ʹ�÷�����ʱ��
#define MIPI_CTRL_FULL_CAP (1 << 3)				///< �������ݰ���ȡ��������ͷ��CRC16У�飬������ÿ��ͼ����������6�ֽ�
#define MIPI_CTRL_CLK_LP_CHK (1 << 4)			///< ��CLK Lane��LP״̬���м�⣬ǿ��Ҫ��MIPI TX�˵�Clk Lane�������һ��LP״̬
#define MIPI_CTRL_CLK_LP01_CHK (1 << 6)			///< ��CLK Lane��LP-10״̬���м�飬���û�����״̬������һֱ�ȴ�
#define MIPI_CTRL_DAT_LP01_CHK (1 << 7)			///< ��DATA Lane��LP-10״̬���м�飬���û�����״̬������һֱ�ȴ�
#define MIPI_CTRL_LRTE_EN (1 << 15)				///< ����LRTEģʽ
#define MIPI_CTRL_LANE_CNT_DET_MANUAL (1 << 16) ///< lane/trio�����ֶ����ã���ʹ�����Զ����lane/trio����
/** @} */

/** @} */

/** @name MIPI Phy typeѡ��
@{

*/
/* MIPI Phy typeѡ�� */
#define MIPI_DPHY_1_5G 0 ///< 1.5G���²���deskewУ׼��DPHYѡ��
#define MIPI_DPHY_2_5G 1 ///< 1.5G���ϴ�deskewУ׼��DPHYѡ��
#define MIPI_CPHY 2		 ///< CPHYѡ��
/** @} */

/** @name ͬ�����нӿ����Ե�λ����
@{

*/
/* ͬ�����нӿ����Ե�λ���� */
#define PARA_PCLK_RVS (1 << 3)	///< PCLKȡ��
#define PARA_VSYNC_RVS (1 << 4) ///< VSYNCȡ��
#define PARA_HSYNC_RVS (1 << 5) ///< HSYNCȡ��
#define PARA_AUTO_POL (1 << 6)	///< VSYNC,HSYNC�����Զ�ʶ��
/** @} */

/** @name ͬ�����нӿ�,3bit����ѡ��λ��
@{

*/
/* 3bit����ѡ��λ�� */
#define PARA_BW_8BIT 0
#define PARA_BW_10BIT 1
#define PARA_BW_12BIT 2
#define PARA_BW_16BIT 3
/** @} */

/** @name HiSPI�ӿ����Ե�λ����
@{

*/
/* HiSPI�ӿ����Ե�λ���� */
// 2bit����ѡ��λ��
#define HISPI_WW_10BIT 0
#define HISPI_WW_12BIT 1
#define HISPI_WW_14BIT 2
#define HISPI_WW_16BIT 3
/** @} */

/** @name ģ��ͼ��ģ�����Ե�λ����
@{

*/
/* ģ��ͼ��ģ�����Ե�λ���� */
// 2bit����ѡ��ģ��ͼ�����ʽ
#define SIM_STYLE1 0 ///< ����̶���ɫ
#define SIM_STYLE2 1 ///< ˮƽ����
#define SIM_STYLE3 2 ///< ��ֱ����
#define SIM_STYLE4 3 ///< ÿ֡����
/** @} */

/** @brief SLVS-EC�ӿ�����
@{
*/
/* ��������SLVS-EC�ӿ� */
typedef struct SlvsECCtrl_s
{
	UINT uLaneNum; // lane����

	UINT uXvsSize;

	UINT uXhsSize;

	UINT Rsv[16]; // ����

} SlvsECCtrl_t;

/** @} */

/****************************************************************************************
 *
 * I2C�������
 *
 ****************************************************************************************/
/*
 * �����ļĴ�����дģʽ
 * I2C mode definiton
 * when read or write by I2c ,should use this definiton...
 * Normal Mode:8 bit address,8 bit data,
 * Samsung Mode:8 bit address,8 bit data,but has a stop between slave ID and addr...
 * Micron:8 bit address,16bit data...
 * Stmicro:16bit addr ,8bit data,such as eeprom and stmicro sensor...
 */
/** @name I2Cģʽ����
@{

*/
/// I2Cģʽ���塣
enum I2CMODE
{
	I2CMODE_NORMAL = 0, ///< 8 bit addr,8 bit value
	I2CMODE_SAMSUNG,	///< 8 bit addr,8 bit value,Stopen
	I2CMODE_MICRON,		///< 8 bit addr,16 bit value
	I2CMODE_STMICRO,	///< 16 bit addr,8 bit value, (eeprom also)
	I2CMODE_MICRON2,	///< 16 bit addr,16 bit value
};

/// sensor i2c��������ѡ��
enum I2CPULLUPRESISTOR
{
	PULLUP_RESISTOR_1_5K = 0,  ///< 1.5K pull up resistor
	PULLUP_RESISTOR_4_7K = 1,  ///< 4.7K pull up resistor
	PULLUP_RESISTOR_0_56K = 2, ///< CMU958/DMU956/DMU927��Щ������560ŷķ�������裬������560ŷķ�������裬���Ժ�ʵ�ʻ�ʹ��1.14K��������
	PULLUP_RESISTOR_CLOSED = 3,
	PULLUP_RESISTOR_1_14K = 4,
	PULLUP_RESISTOR_0_375K = 5,
	PULLUP_RESISTOR_0_407K = 6,
	PULLUP_RESISTOR_0_5K = 7,
};
/** @} */

/** @name i2c/i3cģʽ����
@{
*/
typedef enum SensorBusType
{
	I2C_BUS = 0,
	I3C_SDR_BUS = 1
};

/**@} */

/**@brief I3C���������
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

/**@brief I3C����
@{

*/

typedef struct I3CConfig_s
{
	BYTE byStaticAddr; //< ��̬��ַ��һ��Ϊ��I2C�ĵ�ַ
	I3CCmdDef uCmd;	   //< I3C�����룬�綯̬�����ַ�����д��󳤶����õ����������һ����1���ֽڣ�������4���ֽ�Ӧ���㹻��
	UINT uCmdSize;	   //< ��̬�����ַ����Ͷ�д�����������Ϊ1�ֽ�
	UINT uSize;		   //< ��������size�Ĵ�С
	BYTE *pData;	   //< ��������
} I3CConfig_t;
/** @} */

/** @name SPIģʽ����
@{

*/
/// SPIģʽ����
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

#define MASTER_CTRL_DATA_SHIFT (1 << 0) ///< ������λģʽ  0: MSB �ȳ�, 1: LSB �ȳ�
#define MASTER_CTRL_CPOL (1 << 1)		///< ʱ�Ӽ��ԣ�Clock polarity�� - SPI����ʱ��ʱ���źŵĵ�ƽ״̬[ 1: ����ʱ�ߵ�ƽ ��0: ����ʱ�͵�ƽ ]

/* ʱ����λ��Clock phase�� - SPI��SCLK�ڼ������ز���
CPHA=0����ʾ��һ�����أ�
CPOL=0��idleʱ����ǵ͵�ƽ����һ�����ؾ��Ǵӵͱ䵽�ߣ������������أ�
����CPOL=1��idleʱ����Ǹߵ�ƽ����һ�����ؾ��ǴӸ߱䵽�ͣ��������½��أ�
CPHA=1����ʾ�ڶ������أ�
CPOL=0��idleʱ����ǵ͵�ƽ���ڶ������ؾ��ǴӸ߱䵽�ͣ��������½��أ�
CPOL=1��idleʱ����Ǹߵ�ƽ����һ�����ؾ��Ǵӵͱ䵽�ߣ������������أ�
*/
#define MASTER_CTRL_CPHA (1 << 2)
#define MASTER_CTRL_DELAY (1 << 3) ///< 0:�Ĵ������������ʱ��1���Ĵ���д�������ʱ����ʱֵ��data�и���

#define MASTER_CTRL_THREE_WIRE (1 << 4)	   ///< 0:������ģʽ��1������ģʽ
#define MASTER_CTRL_RW_ENDGE_DIFF (1 << 5) ///< ��д�����Ƿ�һ�£�1���෴��0��һ��
#define MASTER_CTRL_CS_HIGH (1 << 6)	   ///< Ƭѡ����Ч��1�Ǹ���Ч��0�ǵ���Ч

/* SPI���ýṹ�� */
typedef struct MasterSpiConfig_s
{
	double fMhz;	///< ����SPI��ʱ��
	BYTE byWordLen; ///< Word length in bits. 0�� 8bit ��1��16bit����ʱ��Ч��������Ĭ��Ϊ0
	BYTE byCtrl;	///< ֧�ֵ�λ�����룺MASTER_CTRL_DATA_SHIFT/MASTER_CTRL_CPOL/ MASTER_CTRL_CPHA/MASTER_CTRL_DELAY
	BYTE Rsv[64];	///< ���� */
} MasterSpiConfig_t;

typedef enum
{
	USB2_0 = 0,
	USB3_0 = 1,
	USB3_1 = 2,

	FIBRE = 20 /// ���˲�Ʒ
} DevLinkType;

/** @brief �豸���������ṹ��
@{
*/
/* �����豸��������Ϣ */
typedef struct DevLinkStatus_s
{
	BOOL bLinkOk;		  //< ָʾ��ǰlink�Ƿ�����(�Ѿ������豸)
	double lfLinkRate;	  //< Link �ٶȣ���λMbps
	DevLinkType LinkType; //< Link ����
	ULONG uTransLineNum;  //< ��ǰ�豸�����߸�������USB���߹����߸���
	ULONG uPortMask;	  //< ��ǰ�豸���MASK��Ϣ��BIT0Ϊ1����ʾ���0ʹ�ã�BIT1Ϊ1����ʾ���1ʹ��
	ULONG Rsv[30];
} DevLinkStatus_t;
/** @} */

/** @brief �豸��Ϣ�����ṹ��
@{
*/
/* �����豸����Ϣ */
typedef struct DevInfo_s
{
	/* ԭʼ֡�����Ӳɼ�������֡��,��Щ֡��sensorԭʼ����� */
	UINT uOrgFrameCnt;

	/* ���Զ������ײ�GrabFrame�ӿڻ�ȡ����ȷ֡��������Щ֡������һ����飬�������Ƿ�ƥ�䣬ȷ������ȷ�ԣ�
һ������£�uOrgFrameCnt >= uFrameOkCnt + uFrameProblemCnt; */
	UINT uFrameOkCnt;

	/* �ɼ�������֡��������Щ֡�����ǿ�߲�ƥ�������crc��ecc�ȴ����֡ */
	UINT uFrameProblemCnt;

	/* ���ύ/�������Ч֡����,��GrabFrame�ӿ�������ϲ�����ߵ�֡���� */
	UINT uFrameOutCnt;

	/* ���� */
	UINT Rsv[64];
} DevInfo_t;
/** @} */

/** @brief ��ȡMIPI״̬��Ϣ
@{
*/
typedef struct MipiStatusInfo_s
{
	/* ecc corrected cnt */
	UINT uEccCorrectedCnt;

	/* packets per frame */
	UINT uPacketsPerFrame;

	/* lane lock state,bit0-bit3���α�ʾlane1-lane4��״̬ */
	UINT uLaneLockState;

	/* ���� */
	UINT Rsv[128];

} MipiStatusInfo_t;

/** @} */

/** @brief ץ֡��������
@{
*/
/* �û������ö���������ƥ���crc�ȴ���֡���� */
typedef struct FrameFilter_s
{
	/* ����crc�����֡���� */
	bool bCrcErrorFilter;

	/* ��size��ƥ���֡���� */
	bool bSizeErrorFilter;

	/* ���� */
	UINT Rsv[64];

} FrameFilter_t;

/** @} */

/** @} */ // end of group3

/****************************************************************************************
 *
 * ͼ�����ݲɼ����
 *
 ****************************************************************************************/

/** @defgroup group4 ͼ�����ݲɼ����
@{

*/

/** @name FrameBufferģʽ����
@{

*/
///< FrameBufferģʽ����

#define BUF_MODE_NORMAL 0 ///< һ��ģʽ������Ч���൱��FIFO����������������\n
						  // ������������ʱ���µ�֡�����ᱻд�뵽���棻

#define BUF_MODE_SKIP 1 ///< ��֡ģʽ�������е�֡��������֡��Ŷӡ�����

#define BUF_MODE_NEWEST 2 ///< NEWESTģʽ��Ŀǰֻ��PCI-E�ӿڵĻ�����Ч��\n
						  // GrabFrame����ȡ���»��浽��֡��������������\n
						  // ����Ч��SKIPģʽ
/** @} */

/** @brief Buffer��Ϣ�����ṹ��
@{
*/
/* ��������FrameBuffer */
typedef struct _FrameBufferConfig
{
	ULONG uMode;		 ///< frame bufferģʽѡ��,��BUF_MODE_XXXX
	ULONG uBufferSize;	 ///< �豸�е�֡�����С(�ֽ�)���豸�̶����û�������Ч
	ULONG uUpLimit;		 ///< �豸������������(�ֽ�)�������������������ʱ���µ�֡��������
	ULONG uBufferFrames; ///< ������֡������,ֻ��BUF_MODE_NORMALģʽ��Ч
	BOOL bLite;			 ///< �Ƿ�ʹ�ý��յ��ڴ����뷽ʽ��ISPʹ�õ��ڴ潫��Ԥ������
	ULONG resv[14];		 ///< ���������0
} FrameBufferConfig;
/** @} */

/** @brief ֡�����Ϣ����֡���ݶ�Ӧ�������������
@{
*/
/* ֡�����Ϣ����֡���ݶ�Ӧ������������� */
typedef struct sFrameInfo
{
	BYTE byChannel;		///< ͼ��ͨ����ʶ��ֻ��UH920/DTLC2֧��
	USHORT uWidth;		///< ͼ��Ŀ�ȣ���λ�ֽ�
	USHORT uHeight;		///< ͼ��ĸ߶ȣ���λ�ֽ�
	UINT uDataSize;		///< ��������С����λ�ֽ�
	UINT64 uiTimeStamp; ///< ֡��ʼʱ���ֵ����λus
} FrameInfo;
/** @} */
/** @brief ��չ��֡��Ϣ�ṹ��
@{
*/
// ��չ��֡��Ϣ�ṹ��
typedef struct sFrameInfoEx
{
	BYTE byChannel;		  ///< ͼ��ͨ����ʶ��ֻ��UH920/DTLC2֧��
	BYTE byMipiVcChannel; ///< MIPI����ͨ���ţ�ֻ��MIPI�ӿ���Ч
	BYTE resvl[2];		  ///< ����2�ֽڣ����0
	BYTE byImgFormat;	  ///< ͼ���ʽ��D_RAW8��D_RAW10...
	USHORT uWidth;		  ///< ͼ��Ŀ�ȣ���λ�ֽ�
	USHORT uHeight;		  ///< ͼ��ĸ߶ȣ���λ�ֽ�
	UINT uDataSize;		  ///< ��������С����λ�ֽ�
	UINT uFrameTag;		  ///< ֡��ʶ
	double fFSTimeStamp;  ///< ֡��ʼ��ʱ�������λus
	double fFETimeStamp;  ///< ֡������ʱ�������λus
	UINT uEccErrorCnt;	  ///< ÿ֡��ECC���������ֻ��MIPI�ӿ���Ч
	UINT uCrcErrorCnt;	  ///< ÿ֡��CRC���������ֻ��MIPI�ӿ���Ч
	UINT uFrameID;		  ///< ֡����
	UINT resv[60];		  ///< ���������0
} FrameInfoEx;
/** @} */
/** @name FrameInfoEx::uFrameTagλ�������
@{

*/
/* FrameInfoEx::uFrameTag */
/* �ɼ��Ѿ���ʼ */
#define FRM_INFO_TAG_STARTED 1

/* ֡�Ѿ��ɼ���� */
#define FRM_INFO_TAG_GRAB_OK (1 << 1)

/* ֡�Ѿ�������� */
#define FRM_INFO_TAG_PROC_OK (1 << 2)

/* ֡�Ѿ���������Ч�����ܲɼ��ʹ�����һ�룬���������ٴ���Ҳ�����ύ */
#define FRM_INFO_TAG_BAD (1 << 4)

/* ���ܳ�����һЩ���󣬵������������� */
#define FRM_INFO_TAG_ERR (1 << 5)

/* Restart֮��ĵ�һ֡ */
#define FRM_INFO_TAG_FIRST (1 << 6)

/* TestWindowʹ�ܱ�־ */
#define FRM_INFO_TAG_TW (1 << 7)
/** @} */

/** @name Ԥ�����ڶ���
@{

*/
/* Ԥ�����ڶ��� */
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
	WORK_STANDBY_CURRENT_TEST = 0x40, /// ���������������ģʽ
	WORK_OS_TEST = 0x60
} WorkMode;
/** @} */

/** @} */ // end of group4

/****************************************************************************************
 *
 * ��Դ�������
 *
 ****************************************************************************************/
/** @defgroup group5 ��Դ����Ԫ���

@{

*/

/* ֡�����Ϣ����֡���ݶ�Ӧ������������� */
/** @name SENSOR��Ҫ�ĵ�Դ���Ͷ���
@{

*/
/* ����SENSOR��Ҫ�ĵ�Դ���� */
/// ����SENSOR��Ҫ�ĵ�Դ���͡�
typedef enum
{
	/* Aͨ������ֻ��һ��ͨ��ʱ */
	POWER_AVDD = 0,	 ///< AVDD
	POWER_DOVDD = 1, ///< DOVDD
	POWER_DVDD = 2,	 ///< DVDD
	POWER_AFVCC = 3, ///< AFVCC
	POWER_VPP = 4,	 ///< VPP

	/* Bͨ��,(Bͨ����Դ���壬ֻ��UH920ʹ��) */
	POWER_AVDD_B = 5,  ///< Bͨ��AVDD
	POWER_DOVDD_B = 6, ///< Bͨ��DOVDD
	POWER_DVDD_B = 7,  ///< Bͨ��DVDD
	POWER_AFVCC_B = 8, ///< Bͨ��AFVCC
	POWER_VPP_B = 9,   ///< Bͨ��VPP

	/* �����ӵĵ�Դͨ������ */
	POWER_OISVDD = 10,
	POWER_AVDD2 = 11,
	POWER_AUX1 = 12,
	POWER_AUX2 = 13,
	POWER_VPP2 = 14,

	/* CC16���Ͷ���ĵ�Դ */
	POWER_SENSOR0 = 40,
	POWER_SENSOR1 = 41,
	POWER_SENSOR2 = 42,
	POWER_SENSOR3 = 43,
	POWER_VDDIO = 70
} SENSOR_POWER;

#define POWER_AUX POWER_AUX1

/** @} */

/** @name SENSORϵͳ��Դ���Ͷ���
@{

*/
/* ����ϵͳ��Դ���� */
/// ����ϵͳ��Դ���͡�
typedef enum
{
	POWER_SYS_12V = 0, ///< 12Vϵͳ��Դ
	POWER_SYS_5V,	   ///< 5Vϵͳ��Դ
	POWER_SYS_3_3V	   ///< 3.3Vϵͳ��Դ
} SYS_POWER;
/** @} */

/** @name SENSOR��Դģʽ����
@{

*/
/* �����Դģʽ */
/// �����Դģʽ��
typedef enum
{
	POWER_MODE_WORK = 0, ///< SENSOR��ԴΪ����ģʽ
	POWER_MODE_STANDBY,	 ///< SENSOR��ԴΪ����ģʽ
	POWER_MODE_PWDN		 ///< SENSOR��ԴΪ����ģʽ
} POWER_MODE;
/** @} */

/** @name �����������̶���
@{

*/
/* ��������������� */
/// ��������������̡�
typedef enum
{
	CURRENT_RANGE_MA = 0, ///< ������������ΪmA
	CURRENT_RANGE_UA,	  ///< ������������ΪuA
	CURRENT_RANGE_NA	  ///< ������������ΪnA
} CURRENT_RANGE;
/** @} */

/** @name ��������ģʽ
@{

*/
/* �����������ģʽ */
/// �����������ģʽ��
typedef enum
{
	MEASUREMENT_MODE_NORMAL = 0,		///< ��ͨģʽ����ģʽ�£�lfTriggerPoint���õ�ֵʧЧ
	MEASUREMENT_MODE_TRIGGER_POINT = 1, ///< �����ɼ�ʹ�ô�����ģʽ��TOF������ɼ�����ʹ��
} CURRENT_MEASUREMENT_MODE;
/** @} */

/** @name ������������
@{
*/
typedef struct PmuCurrentMeasurement_s
{
	SENSOR_POWER PowerType;				  ///< ��Դ����
	CURRENT_MEASUREMENT_MODE MeasureMode; ///< �μ�CURRENT_MEASUREMENT_MODE����
	UINT uStatisticsNum;				  ///< ����ͳ�Ƹ����������ɼ�ʹ�ô�����ģʽʱ����Ч
	double lfTriggerPoint;				  ///< �����ɼ�����ֵ	����λ��mA�����ȣ�1mA��ֻ��MU950(TOF)����֧�ִ������������ã�ֻ֧��PosEN=TRUE
	BOOL bPosEn;						  ///< ��ʶ����ֵ���������Ч��ΪTrue�������TriggerPoint�����ɼ�������ΪFalse����С��TriggerPoint�����ɼ�����
	UINT Rsv[32];						  ///< ����
} PmuCurrentMeasurement_t;
/** @} */

/** @name �˲���������
@{

*/
/* �����˲�����ģʽ */
typedef enum
{
	FILTER_CAP_AUTO_SET_MODE = 0,	///< �Զ�����ģʽ
	FILTER_CAP_MANUAL_SET_MODE = 1, ///< ��ģʽ�û����ֶ����ÿ���/�ر��˲�����
} FILTER_CAP_MODE;
/** @} */

/** @name �˲���������
@{
*/
typedef struct PmuFilterCap_s
{
	SENSOR_POWER PowerType; ///< ��Դ����
	FILTER_CAP_MODE Mode;	///< ģʽ����
	BOOL bEn;				///< �ֶ�����ģʽ�����ã�true�򿪣�false�ر�
	UINT Rsv[16];			///< ����
} PmuFilterCap_t;
/**@} */

/** @name ADC�ɼ�
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
 * �������ܶ���(dtTest.exeʹ�õĶ���)
 *
 ****************************************************************************************/
/** @defgroup group6 ��ʼ���������
@{

*/
/** @name �������ܶ���
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
 * ��ϵͳ֧�ֵ�AF�����ͺŶ���
 *
 ****************************************************************************************/
/** @defgroup group7 AF���
@{

*/
/** @name ֧�ֵ�AF�����ͺŶ���
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
 * OS/LC���
 *
 ****************************************************************************************/
/** @defgroup group8 LC/OS���
@{

*/
/** @name OS/LC�������ö���
@{

*/
/* OS/LC�������ö��壬��LC_OS_CommandConfig������ʹ�� */

#define OS_CFG_TEST_ENA (1 << 7)  ///< OS����ʹ��
#define LC_CFG_TEST_ENA (1 << 6)  ///< LC����ʹ��
#define OS_CFG_CHANNEL_A (1 << 5) ///< ʹ�ܲ���Aͨ��OS����(DTLC2����֧�֣��������Ͳ���Ч)
#define OS_CFG_CHANNEL_B (1 << 4) ///< ʹ�ܲ���Bͨ��OS����(DTLC2����֧�֣��������Ͳ���Ч)
#define OS_CFG_HIGH (1 << 3)	  ///< ʹ������OS����
#define OS_CFG_LOW (1 << 2)		  ///< ʹ�ܸ���OS����
#define LC_CFG_HIGH (1 << 1)	  ///< ʹ������LC����
#define LC_CFG_LOW (1 << 0)		  ///< ʹ�ܸ���LC����

#define OS_CFG_DOUBLE (1 << 8) ///< Gϵ�й��˲�Ʒ��G42/G22������֧��A,B����C��Dһ�����OS����ģʽ��˫��ģ�鹲�ص�ʱ��ʹ�ã�
							   ///< ����ע��:��ģʽ����Ҫ��ͬ������֤2��ģ��ͬʱ����OS���Ի��˳�OS����

#define OS_CFG_QUICK (1 << 15) ///< �ݲ�֧��
/** @} */
/** @name OS/LC���Խ�����壬OS_Read�������صĽ��
@{

*/
/* OS/LC���Խ�����壬OS_Read�������صĽ�� */
#define OS_TEST_RESULT_PASS 0		// ͨ������
#define OS_TEST_RESULT_NG 1			// δͨ������
#define OS_TEST_RESULT_FAILED 0xfe	// ����ʧ��
#define OS_TEST_RESULT_INVALID 0xff // ������Ч
/** @} */

/** @} */ // end of group8

/************************************************************************
*
*�ⲿ��չIO����
*
/************************************************************************/
/** @defgroup group9 ��չIO
@{

*/
/** @name �ⲿ��չIO�ܽŶ���
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

/** @name �ⲿ��չIOģʽ����
@{

*/
typedef enum
{
	GPIO_INPUT = 0, ///< ����ģʽ
	GPIO_OUTPUT,	///< ���ƽ���ģʽ
	GPIO_OUTPUT_PP, ///< �ߵ͵�ƽ�������

} EXT_GPIO_MODE;
/** @} */

/** @} */ // end of group9

/* ���Ա�����Ϣ������ģ�� */
enum DtDbgRePortPart_e
{
	/* Ĭ�ϻ�δ�����ܷ��� */
	PART_DEFAULT = 0x0,

	/* ������� */
	PART_CONTROL = 0x10,

	/* ��������ɼ���� */
	PART_STREAM = 0x20,

	/* mipi */
	PART_MIPI = 0x30,

	/* GPIO������� */
	PART_GPIO = 0x40,

	/* ͼ��Ч����� */
	PART_IMAGE = 0x50,

	/* power��� */
	PART_POWER = 0x60,

	/* open/short test��� */
	PART_OS_TEST = 0x70,

	/* ��������������Ϣ */
	PART_MAIN_CONTROL = 0x80
};

/* ���Ա�����Ϣ������ */
enum DtDbgRePoportLevel_e
{
	/* ��Ϣ����ʾ */
	LEVEL_INFO = 0x10,

	/* �������� */
	LEVEL_PROBLEM = 0x40
};

/* ���Ա��� */
typedef struct DtDbgReport_s
{
	/* ���Ա���Ĺ��ܷ��� */
	DtDbgRePortPart_e Part;

	/* ���Ա���ļ��� */
	DtDbgRePoportLevel_e Level;

	/* �Ƿ�ǿ�Ƽ�¼���ļ��������Ƿ���log�ļ���¼ */
	bool bForce;

	/* ����64�ֽ� */
	UINT resv[16];

	/* ���Ա��������ı����ݣ������뵽log�ļ����� */
	char text[128];
} DtDbgReport_t;

// �����֧�ֲ��Ե���Ĺܽ�
enum ResisCalculateForIoSet
{
	PO1 = 0, // ���PO1
	PO2 = 1, // ���PO2
	PO3 = 2, // ���PO3
	PO4 = 3	 // ���PO4
};

#define RESIS_TEST_COMMON_GROUND 1 << 0

/****************************************************************************************
 *
 * һЩSDK�ӿں�����ʹ�õ��ĺ궨��(dtccmʹ�õĶ���)
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
easy ��Դ�ӿ���ض���
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

#define POWER_CONFIG_ONOFF 1 << 0		 // ��������
#define POWER_CONFIG_VOLTAGE 1 << 1		 // ��ѹֵ����
#define POWER_CONFIG_CURRENTLIMIT 1 << 2 // ����ֵ����
#define POWER_CONFIG_SLOPE 1 << 3		 // б������
#define POWER_CONFIG_SPEED 1 << 4		 // �����ٶ�����
#define POWER_CONFIG_RANGE 1 << 5		 // ��������

typedef struct _ezPowerSequence
{
	UINT uSequence; // ��ѹʱ��,��ѹ�ϵ���µ����ȼ����ã���0Ϊ���ȼ���ߣ�1��֮��������ȼ�һ�£���ͬʱ�ϵ���µ�
	double lfDelay; // ��ʱ����,ms
	UINT rsv[16];
} ezPowerSequence;

// ��Դͨ����ÿ����Դͨ���������ء���������ѹ������б�ʵ�����
typedef struct _ezPowerChannel
{
	BOOL bOnOff;			  // ����
	double lfVolt;			  // ��ѹ,mV
	double lfCurrentLimit;	  // ����ֵ,uA
	double lfSlope;			  // б��,mV/ms
	CURRENT_RANGE range;	  // ��������,nA,uA,mA
	UINT uSpeed;			  // ���������ٶ�,ms
	ezPowerSequence sequence; // ʱ�����
	UINT uConfigCode;		  // ����������,ͨ��λ����ķ�ʽָʾ��Щ������Ҫ���õ��豸
	UINT uStateCode;		  // ״̬��
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

#define SENSOR_I2C_RD_NO_STOP 1 << 0 // I2C���׶β�����stopָ��

/** @name SensorI2cRw�ṹ��
@{
*/
typedef struct ezSensorI2cRw_s
{
	UINT uCtrl;		  ///< ������,SENSOR_I2C_xxx
	BYTE bySlaveAddr; ///< ��������ַ
	BYTE *pWrData;	  ///< д�����ݿ�
	UINT uWrSize;	  ///< д�����ݿ���ֽ���
	BYTE *pRdData;	  ///< �������ݿ�
	UINT uRdSize;	  ///< �������ݿ��ֽ���
	UINT rsv[16];	  ///< ����
} ezSensorI2cRw_t;
/** @} */

// ���幦�ܿ�����

typedef enum
{
	EZ_POWER_ON = 0x10,	 // ��Դ�ϵ����:�ϵ�ʱ�򣬵�ѹ���ã�б�����ã����̵�һ�����óɹ�
	EZ_POWER_UP = 0x11,	 // ��Դ�µ�
	EZ_POWER_GET = 0x12, // ��ȡ��Դ������״̬
} ezCtrl;

/******/
/* ָ��ģ������ */
typedef struct FpmConfig_s
{
	BYTE byMode;
	UINT uIrqCnt; ///< ���ü���Ϊһ���ж���,����Ϊ320��uIrqCnt=4����һ�ζ�ȡ320*4�����ݸ����жϺ�
	BYTE rsv[64]; ///< ����
} FpmConfig_t;

/****************************************************************************************
 *
 * ���ô�����
 *
 ****************************************************************************************/

#define DT_ERROR_NO_ACTION 8			   ///< �޶���
#define DT_ERROR_IGNORED 7				   ///< �������Ե��ˣ�����Ҫ�κζ���
#define DT_ERROR_NEED_OTHER 6			   ///< ��Ҫ�������ݺͲ���
#define DT_ERROR_NEXT_STAGE 5			   ///< ���������һ�׶Σ�ֻ����˲��ֶ���
#define DT_ERROR_BUSY 4					   ///< ��æ(��һ�β������ڽ�����)���˴β������ܽ���
#define DT_ERROR_WAIT 3					   ///< ��Ҫ�ȴ�(���в���������������)�������ٴγ���
#define DT_ERROR_IN_PROCESS 2			   ///< ���ڽ��У��Ѿ���������
#define DT_ERROR_OK 1					   ///< �����ɹ�
#define DT_ERROR_FAILED 0				   ///< ����ʧ��
#define DT_ERROR_INTERNAL_ERROR -1		   ///< �ڲ�����
#define DT_ERROR_UNKNOW -1				   ///< δ֪����
#define DT_ERROR_NOT_SUPPORTED -2		   ///< ��֧�ָù���
#define DT_ERROR_NOT_INITIALIZED -3		   ///< ��ʼ��δ���
#define DT_ERROR_PARAMETER_INVALID -4	   ///< ������Ч
#define DT_ERROR_PARAMETER_OUT_OF_BOUND -5 ///< ����Խ��
#define DT_ERROR_UNENABLED -6			   ///< δʹ��
#define DT_ERROR_UNCONNECTED -7			   ///< δ���ӵ��豸
#define DT_ERROR_NOT_VALID -8			   ///< ������Ч
#define DT_ERROR_UNPLAY -9				   ///< �豸û��
#define DT_ERROR_NOT_STARTED -10		   ///< δ����
#define DT_ERROR_NOT_STOPPED -11		   ///< δֹͣ
#define DT_ERROR_NOT_READY -12			   ///< δ׼����
#define DT_ERROR_DESCR_FAULT -20		   ///< ���������
#define DT_ERROR_NAME_FAULT -21			   ///< ���������
#define DT_ERROR_VALUE_FAULT -22		   ///< ����ĸ�ֵ
#define DT_ERROR_LIMITED -28			   ///< ������
#define DT_ERROR_FUNCTION_INVALID -29	   ///< ������Ч
#define DT_ERROR_IN_AUTO -30			   ///< ���Զ������У��ֶ���ʽ��Ч
#define DT_ERROR_DENIED -31				   ///< �������ܾ�
#define DT_ERROR_BAD_ALIGNMENT -40		   ///< ƫ�ƻ��ַδ����
#define DT_ERROR_ADDRESS_INVALID -41	   ///< ��ַ��Ч
#define DT_ERROR_SIZE_INVALID -42		   ///< ���ݿ��С��Ч
#define DT_ERROR_OVER_LOAD -43			   ///< ����������
#define DT_ERROR_UNDER_LOAD -44			   ///< ����������
#define DT_ERROR_BUFFER_SMALL -52		   ///< Buffer�ռ�̫С
#define DT_ERROR_CHECKED_FAILED -50		   ///< ��顢У��ʧ��
#define DT_ERROR_UNUSABLE -51			   ///< ������
#define DT_ERROR_BID_INVALID -52		   ///< ҵ��ID��Ч��ƥ��

/* IO���洢���豸���  */
#define DT_ERROR_TIME_OUT -1000		   ///< ��ʱ����
#define DT_ERROR_IO_ERROR -1001		   ///< Ӳ��IO����
#define DT_ERROR_COMM_ERROR -1002	   ///< ͨѶ����
#define DT_ERROR_BUS_ERROR -1003	   ///< ���ߴ���
#define DT_ERROR_FORMAT_INVALID -1004  ///< ��ʽ����
#define DT_ERROR_CONTENT_INVALID -1005 ///< ������Ч
#define DT_ERROR_BAD_CHECKSUM -1006	   ///< ����У�����
#define DT_ERROR_I2C_FAULT -1010	   ///< I2C���ߴ���
#define DT_ERROR_I2C_ACK_TIMEOUT -1011 ///< I2C�ȴ�Ӧ��ʱ
#define DT_ERROR_I2C_BUS_TIMEOUT -1012 ///< I2C�ȴ����߶�����ʱ������SCL���ⲿ������Ϊ�͵�ƽ
#define DT_ERROR_SPI_FAULT -1020	   ///< SPI���ߴ���
#define DT_ERROR_UART_FAULT -1030	   ///< UART���ߴ���
#define DT_ERROR_GPIO_FAULT -1040	   ///< GPIO���ߴ���
#define DT_ERROR_USB_FAULT -1050	   ///< USB���ߴ���
#define DT_ERROR_PCI_FAULT -1060	   ///< PCI���ߴ���
#define DT_ERROR_PHY_FAULT -1070	   ///< ��������
#define DT_ERROR_LINK_FAULT -1080	   ///< ��·�����
#define DT_ERROR_TRANS_FAULT -1090	   ///< ��������

#define DT_ERROR_NO_DEVICE_FOUND -1100			   ///< û�з����豸/// @brief δ�ҵ��߼��豸
#define DT_ERROR_NO_LOGIC_DEVICE_FOUND -1101	   ///< δ�ҵ��߼��豸
#define DT_ERROR_DEVICE_IS_OPENED -1102			   ///< �豸�Ѿ���
#define DT_ERROR_DEVICE_IS_CLOSED -1103			   ///< �豸�Ѿ��ر�
#define DT_ERROR_DEVICE_IS_DISCONNECTED -1104	   /// �豸�Ѿ��Ͽ�����
#define DT_ERROR_DEVICE_IS_OPENED_BY_ANOTHER -1105 /// �豸�Ѿ�������������
#define DT_ERROR_KERNEL_DRIVER_PROBLEM -1106	   /// �ں�������������
#define DT_ERROR_SOCKET_PROBLEM -1107			   /// �����׽ӳ�������

#define DT_ERROR_NO_MEMORY -1200			///< û���㹻ϵͳ�ڴ�
#define DT_ERROR_MEM_FAULT -1201			///< �洢����д����������޷�������д
#define DT_ERROR_WRITE_PROTECTED -1202		///< д����������д
#define DT_ERROR_FILE_CREATE_FAILED -1300	///< �����ļ�ʧ��
#define DT_ERROR_FILE_INVALID -1301			///< �ļ���ʽ��Ч
#define DT_ERROR_FILE_READ_FAILED -1302		///< ��ȡ�ļ�ʧ��
#define DT_ERROR_FILE_WRITE_FAILED -1303	///< д���ļ�ʧ��
#define DT_ERROR_FILE_OPEN_FAILED -1304		///< ���ļ�ʧ��
#define DT_ERROR_FILE_CHECKSUM_FAILED -1305 ///< ��ȡ���ݽϼ�ʧ��

#define DT_ERROR_GRAB_FAILED -1600		  ///< ���ݲɼ�ʧ��
#define DT_ERROR_LOST_DATA -1601		  ///< ���ݶ�ʧ��������
#define DT_ERROR_EOF_ERROR -1602		  ///< δ���յ�֡������
#define DT_ERROR_GRAB_IS_OPENED -1603	  ///< ���ݲɼ������Ѿ���
#define DT_ERROR_GRAB_IS_CLOSED -1604	  ///< ���ݲɼ������Ѿ��ر�
#define DT_ERROR_GRAB_IS_STARTED -1605	  ///< ���ݲɼ��Ѿ�����
#define DT_ERROR_GRAB_IS_STOPPED -1606	  ///< ���ݲɼ��Ѿ�ֹͣ
#define DT_ERROR_GRAB_IS_RESTARTING -1607 ///< ���ݲɼ���������
#define DT_ERROR_GRAB_IS_HOLD -1608		  ///< ���ݲɼ��Ѿ���ͣ
#define DT_ERROR_FRAME_IS_OUTDATED -1609  ///< �ɼ���֡�Ѿ���ʱ
#define DT_ERROR_ROI_PARAM_INVALID -1610  ///< ���õ�ROI������Ч
#define DT_ERROR_ROI_NOT_SUPPORTED -1611  ///< ROI���ܲ�֧��
#define DT_ERROR_GRAB_IS_DROPPED -1613	  ///< ����������Ƶ������Ҫ��֡
#define DT_ERROR_CALIBRATE_FAILED -1614	  ///< У׼ʧ��

/****************************************************************************************
 *
 * ���ڷ�������д����������ʱ�������ش�����
 *
 ****************************************************************************************/
#define CHECK_RETURN(_FUN_)         \
	{                               \
		int iChkRet = _FUN_;        \
		if (iChkRet != DT_ERROR_OK) \
			return iChkRet;         \
	}

#endif // __IMAGEKIT_H__
