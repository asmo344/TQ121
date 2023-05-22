#ifndef _FPSSTRU_H
#define _FPSSTRU_H

#include    <winioctl.h>
#include    <ntddscsi.h>

#include    "WNASPI32.h"
#include    "spti.h"


typedef int	FPS_STATUS;

//
// Return Code
//

#define SEVERITY_SUCESS                 0x00
#define SEVERITY_FAIL                   0x01

#define FACILITY_GLOBAL                 0x00

const FPS_STATUS OK = ((SEVERITY_SUCESS << 31) | (FACILITY_GLOBAL << 16) | 1);

//
// Storage parameter
//
#define MAX_SCSI_DATA_PACKET_SIZE                  512

//
// For vendor driver , we create a vendor handle pointer
//
typedef struct _VendorDeviceHandle{
	//
	// OS Type
	//
	BOOL      bAdmin;
	WORD      OS_VERSION;

#define   Win98_Me                  1
#define   Win2K_XP                  2
#define   WinVistHorn               3
#define   WinXP                     4
#define   Win2K                     5
#define   Win98                     6
#define   WinMe                     7 
#define   WinXP_SP2                 8

	//
	// File Handle Buffer
	//
	HANDLE  fileHandle;
	BYTE    TargetId;
	BYTE    AdapterId;
	BYTE    DeviceIndex;
	BYTE    Vendor;
	BYTE    LunNO;

} VendorDeviceHandle, *PVendorDeviceHandle;


#define INQUIRY_VENDOR_IDENTIFICATION        8      // 8-15
#define INQUIRY_PRODUCT_IDENTIFICATION       16     // 16-31
#define INQUIRY_PRODUCT_REVISION             4      // 32-35

typedef struct _DeviceInfoDesciptor{
	HANDLE  DeviceHandle;
	BYTE    VendorVIDS[INQUIRY_VENDOR_IDENTIFICATION];       // Vendor identification (ASCII)
	BYTE    VendorPIDS[INQUIRY_PRODUCT_IDENTIFICATION];      // Product identification (ASCII)
	BYTE    Revision[INQUIRY_PRODUCT_REVISION];              // Product revision level
} DeviceInfoDesciptor, *PDeviceInfoDesciptor;



//*****************************
// Data Type 
//*****************************
typedef void *LPSRB;
typedef void(*PFNPOST)();

typedef short     SWORD;
typedef	bool(*pfnMediaChange_APId)(UINT Mode);
typedef	void(*pfnMediaChange_KeyPos)(PUCHAR Show96, BYTE* Key);
typedef	void(*pfnFeatureEncoded_Key)(BYTE* AssignedKey);
typedef	DWORD(*pfnGetASPI32SupportInfo)();
typedef	DWORD(*pfnSendASPI32Command)(LPSRB);
typedef int(*pfnGetDevicePath)(LPGUID, LPTSTR*);
typedef HANDLE(*pfnOpenDevice)(LPCTSTR);
typedef BOOL(*pfnUnPlugUsbDevice)(char*);
typedef HANDLE(*pfnOpenDeviceN)(BYTE&, BYTE&, BYTE, BYTE);
typedef BOOL(*pfnUserModeCmd)(HANDLE, BYTE, BYTE*, BYTE*);
typedef SWORD(*pfnSetRegister)(BYTE* Image, BYTE Gain, BYTE VRB, BYTE VRT);

// Bit Map
#define BIT0      0x01
#define BIT1      0x02
#define BIT2      0x04
#define BIT3      0x08
#define BIT4      0x10
#define BIT5      0x20
#define BIT6      0x40
#define BIT7      0x80


#pragma pack( push, before_include1 )
#pragma pack(1)

//
// Command Definition
//
#define GET_SET_REG_DATA                        0x1
#define GET_RAW_IMAGE_DATA_SCSI_LIKE            0x2
#define SET_LED_LIGHT_CONTROL                   0x3

//
// Command 1 - set/get the 
//
typedef struct _SetGetRegCommand01 {
	// Command Header 
	BYTE              SIGNA;
	BYTE              SIGNB;
	BYTE              COMDTYPE;
	BYTE              COMDLENGTH;

	// Command Parameter
	BYTE              REG_TYPE;        // register type,CPU (TYPE=0),CIS(TYPE=1)
	BYTE              REG_NUM;         // store/load registry number
	BYTE              REGDF_TYPE;      // register data format (byte:1 ;WORD:2 ; DOWRD:4) 
	BYTE              DATA_FLAG;       // register data setting (1); getting (0)

	// Register Address 
	BYTE              REG_ADDR3;
	BYTE              REG_ADDR2;
	BYTE              REG_ADDR1;
	BYTE              REG_ADDR0;

	// Register Data
	BYTE              REG_DATA3;
	BYTE              REG_DATA2;
	BYTE              REG_DATA1;
	BYTE              REG_DATA0;

	//  DWORD             REG_ADDR;        // register address
	//  DWORD             REG_DATA;        // register data
}SetGetRegCommand01, *PSetGetRegCommand01;

#pragma pack( pop, before_include1 )

#define CPU_REG_TYPE                       0       // register type of CPU
#define CIS_REG_TYPE                       1       // register type of CIS

#define REGDF_BYTE_TYPE                    1       // register data format of "BYTE"
#define REGDF_WORD_TYPE                    2       // register data format of "WORD"  , 2 bytes 
#define REGDF_DOWRD_TYPE                   4       // register data format of "DWORD" , 4 bytes

#define REG_DATA_WRITE_TYPE                1       // write a register data                   
#define REG_DATA_READ_TYPE                 0       // read a register data, the return data to input endpoint buffer


#pragma pack( push, before_include1 )
#pragma pack(1)

//
// Finger Printer Sensor Parameter
//
typedef struct _FPSParameter {
	WORD         fps_framewidth;
	WORD         fps_frameheight;
} FPSParameter, *PFPSParameter;


//
// Device Information 
//
typedef struct _DeviceInfo {
	CHAR                device_path[256];
	CHAR                device_reg[256];      // the register should be set into device
	FPSParameter        fps_parameter;        // the device detail information 
} DeviceInfo, *PDeviceInfo;

//
// Reconstruction image information
//
typedef struct _FrameInfo {
	BYTE         framenum;             // capture frame number  
	BYTE         pixelshiftperbyte;    // TBD 
	BYTE         column;               // image column
	BYTE         row;                  // image row
} FrameInfo, *PFrameInfo;

//
// Reconstruction image information
//
typedef struct _ReconstructimageInfo {   //(TBD)
	INT          image_intensity_touch;
	INT          image_intensity_leave;
	BYTE         readybit_timeout;
	DWORD        finger_detect_timeout;
	DWORD        csvout_detect_timeout;
	DWORD        reconstruct_timeout;
	INT          fullhight;                                      // Full Image hight
	INT          fullwidth;                                      // Full Image width  
	INT          outputheight;
	INT          outputstatus;
} ReconstructimageInfo, *PReconstructimageInfo;


typedef struct _SENSOR_REG {
    unsigned char       RegAddr;
    unsigned char       RegType;
    unsigned char       RegValue;
    unsigned char       RegValueDefault;
    unsigned char       RegValueMask;
} SENSOR_REG;

#pragma pack( pop, before_include1 )

#endif