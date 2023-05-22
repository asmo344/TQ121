

#ifndef _CISCMD_H
#define _CISCMD_H

#include "ModuleDef.h"
#include "fps_stru.h"
#include "fw_opcode.h"
#include "Command_Packet.h"

#pragma pack( push, before_include1 )
#pragma pack(1)

/*
 * Register context
 */
typedef struct _cisRegisterContext {
	WORD        register_rw_type;           // 0: discrect read/write, 1: continuous read/write

#define       DISCRE_RW            0
#define       CONTINU_RW           1

	WORD        register_number;            // read/write number . [address][data] is one number
	BYTE        *register_address_data;     // [address][data][address][data][address][data] ......
} cisRegisterContext, *PcisRegisterContext;


FPS_STATUS CIS_FPSSetRegisters(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in WORD                reg_type,            // Register read/write type
	__in WORD                bufferlen,           // data buffer length 
	__in BYTE*               regbuffer            // data buffer pointer
);

FPS_STATUS CIS_FPSGetRegisters(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
);

FPS_STATUS CIS_FPSProcessStandby(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
);

FPS_STATUS CIS_FPSProcessRst(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
);

FPS_STATUS CIS_FPSGetOTGver(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
);

FPS_STATUS CIS_FPSWriteRegister(
	VendorDeviceHandle    *VendorDeviceHandle,
	DeviceInfo            *device_Info,           // Device path, device information
	cisRegisterContext    *reg_context
);

FPS_STATUS CIS_FPSReadRegister(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
);

FPS_STATUS CIS_FPSStandbycmd(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
);

FPS_STATUS CIS_FPSRstcmd(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
);

FPS_STATUS CIS_FPSReadOTGver(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
);

FPS_STATUS FPSGetFramesA(
	__in   VendorDeviceHandle  *VendorDeviceHandle,
	__in   DeviceInfo     *device_Info,                 // device path, device information 
	__in   int            datasize,
	__out  BYTE           *imagebuffer                  // image buffer pointer
);

FPS_STATUS FPSGetFramesB(
	__in   VendorDeviceHandle  *VendorDeviceHandle,
	__in   DeviceInfo     *device_Info,                 // device path, device information 
	__in   BYTE           page_num,
	__in   BYTE           page_size,
	__out  BYTE           *imagebuffer                  // image buffer pointer
);

FPS_STATUS FPS_Start_Capture(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in int                 image_width,
	__in int                 image_height,
	__in BYTE                bit_mun 
);

FPS_STATUS FPS_Start_TestPattern(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in int                 image_width,
	__in int                 image_height,
	__in BYTE                bit_mun
);

FPS_STATUS CIS_Normal_Command(
	VendorDeviceHandle *VendorDeviceHandle,
	DeviceInfo         *device_Info,           // Device path, device information
	BYTE               cmd_opcode,  
	BYTE*              cmdbuffer               // data buffer pointer
);

/* T7805JA */
FPS_STATUS UsbTransmit(
	__in   VendorDeviceHandle* VendorDeviceHandle,
	__in   DeviceInfo* device_Info,		// device path, device information
	__in   BYTE* DataBuffer,
	__in   DWORD DataLength,
	__in   BYTE SCSI_CMD
);

FPS_STATUS UsbTransmitReceive(
	__in   VendorDeviceHandle* VendorDeviceHandle,
	__in   DeviceInfo* device_Info,		// device path, device information
	__out  BYTE* DataBuffer,
	__in   BYTE SCSI_CMD
);

FPS_STATUS GetFwVer(
	__in VendorDeviceHandle*  VendorDeviceHandle,
	__in DeviceInfo*          device_Info,           // Device path, device information
	__in BYTE                 reg_type,              // Register read/write type
	__in WORD                 bufferlen,             // data buffer length 
	__in BYTE*                regbuffer              // data buffer pointer
);

FPS_STATUS spiMode(
	__in VendorDeviceHandle*  VendorDeviceHandle,
	__in DeviceInfo*          device_Info,           // Device path, device information
	__in BYTE                 reg_type,              // Register read/write type
	__in WORD                 bufferlen,             // data buffer length 
	__in BYTE*                regbuffer              // data buffer pointer
);
/**/

FPS_STATUS CIS_GetFwVer(
	VendorDeviceHandle*       VendorDeviceHandle,
	DeviceInfo*               device_Info,           // Device path, device information
	cisRegisterContext*       reg_context
);

FPS_STATUS CIS_spiMode(
	VendorDeviceHandle*       VendorDeviceHandle,
	DeviceInfo*               device_Info,           // Device path, device information
	cisRegisterContext*       reg_context
);
#endif