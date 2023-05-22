//
// FILENAME.
//      cis_command.cpp - CMOS sensor command  
//
//      $PATH:
//
// FUNCTIONAL DESCRIPTION.
//      Interface of CMOS Sensor Module Application 
//      Processing the gerenal or special SCSI command Packet 
//
// MODIFICATION HISTORY.
//
// NOTICE.
//      Copyright (C) 2000-2015 EgisTec All Rights Reserved.
//l

#include "stdafx.h"
#include "cis_cmd.h"



FPS_STATUS FPS_Start_Capture(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in int                 image_width,
	__in int                 image_height,
	__in BYTE              bit_mun
	)
{
	BYTE cmd_opcode;
	BYTE cmd_buffer[512];

	cmd_opcode = SCSI_HM_CIS_START_CAPTURE;
	memset(cmd_buffer, 0, 512);
	cmd_buffer[0] = bit_mun;
	cmd_buffer[1] = (image_width & 0xff00) >> 8;
	cmd_buffer[2] = image_width & 0x00ff;
	cmd_buffer[3] = (image_height & 0xff00) >> 8;
	cmd_buffer[4] = image_height & 0x00ff;
	return CIS_Normal_Command(VendorDeviceHandle, device_Info, cmd_opcode, cmd_buffer);
}

FPS_STATUS FPS_Start_TestPattern(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in int                 image_width,
	__in int                 image_height,
	__in BYTE              bit_mun
)
{
	BYTE cmd_opcode;
	BYTE cmd_buffer[512];

	cmd_opcode = SCSI_HM_CIS_START_TESTPATTERN;
	memset(cmd_buffer, 0, 512);
	cmd_buffer[0] = bit_mun;
	cmd_buffer[1] = (image_width & 0xff00) >> 8;
	cmd_buffer[2] = image_width & 0x00ff;
	cmd_buffer[3] = (image_height & 0xff00) >> 8;
	cmd_buffer[4] = image_height & 0x00ff;
	return CIS_Normal_Command(VendorDeviceHandle, device_Info, cmd_opcode, cmd_buffer);
}

//
// FUNCTION NAME.
//      CIS_FPSSetRegisters
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS CIS_FPSSetRegisters(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in WORD                reg_type,            // Register read/write type
	__in WORD                bufferlen,           // data buffer length 
	__in BYTE*               regbuffer            // data buffer pointer
	)
{
	cisRegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return CIS_FPSWriteRegister(VendorDeviceHandle, device_Info, &reg_context);
}


//
// FUNCTION NAME.
//      CIS_FPSGetRegisters
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS CIS_FPSGetRegisters(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
	)
{
	cisRegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return CIS_FPSReadRegister(VendorDeviceHandle, device_Info, &reg_context);
}

FPS_STATUS CIS_FPSProcessStandby(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
)
{
	cisRegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return CIS_FPSStandbycmd(VendorDeviceHandle, device_Info, &reg_context);
}

FPS_STATUS CIS_FPSProcessRst(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
)
{
	cisRegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return CIS_FPSRstcmd(VendorDeviceHandle, device_Info, &reg_context);
}

FPS_STATUS CIS_FPSGetOTGver(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
)
{
	cisRegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return CIS_FPSReadOTGver(VendorDeviceHandle, device_Info, &reg_context);
}

//
// FUNCTION NAME.
//      FPSWriteRegister
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS CIS_FPSWriteRegister(
	VendorDeviceHandle *VendorDeviceHandle,
	DeviceInfo         *device_Info,         // Device path, device information
	cisRegisterContext    *reg_context
	)
{

	CHAR* device_path = device_Info->device_path;
	WORD wRegType = reg_context->register_rw_type;
	WORD wRegCount = reg_context->register_number;
	BYTE* bUserBuffer = reg_context->register_address_data;
	int k;

	BYTE bOnputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k<wRegCount; k++)
	{
		bOnputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_WRITE_REG;   // CIS CMOS sensor write register OPCODE

	Command_PacketStru.nCommnadDataLength = 16;          //[TBD]
	Command_PacketStru.nResponseDataLength = 0;
	Command_PacketStru.nWriteDataLength = wRegCount;
	Command_PacketStru.OutBuffer = bOnputBuffer;
	Command_PacketStru.InBuffer = NULL;

	// 
	// currently, not use command buffer in storage. (TBD)
	//

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}
	return OK;
}

//
// FUNCTION NAME.
//      FPSReadRegister
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS CIS_FPSReadRegister(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
	)
{

	CHAR* device_path = device_Info->device_path;
	WORD wRegType = reg_context->register_rw_type;             //  
	WORD wRegCount = reg_context->register_number;
	BYTE* bUserBuffer = reg_context->register_address_data;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k<wRegCount; k++)
	{
		bInputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet`
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_READ_REG;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;                 // TBD
	Command_PacketStru.nResponseDataLength = (DWORD)wRegCount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = bInputBuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	for (k = 0; k<wRegCount; k++)
	{
		bUserBuffer[k] = Command_PacketStru.InBuffer[k];
	}
	return OK;
}

FPS_STATUS CIS_FPSStandbycmd(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
)
{

	CHAR* device_path = device_Info->device_path;
	WORD wRegType = reg_context->register_rw_type;             //  
	WORD wRegCount = reg_context->register_number;
	BYTE* bUserBuffer = reg_context->register_address_data;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k < wRegCount; k++)
	{
		bInputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet`
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_PROCESS_STANDBY;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;                 // TBD
	Command_PacketStru.nResponseDataLength = (DWORD)wRegCount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = bInputBuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	for (k = 0; k < wRegCount; k++)
	{
		bUserBuffer[k] = Command_PacketStru.InBuffer[k];
	}
	return OK;
}

FPS_STATUS CIS_FPSRstcmd(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
)
{

	CHAR* device_path = device_Info->device_path;
	WORD wRegType = reg_context->register_rw_type;             //  
	WORD wRegCount = reg_context->register_number;
	BYTE* bUserBuffer = reg_context->register_address_data;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k < wRegCount; k++)
	{
		bInputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet`
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_PROCESS_RST;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;                 // TBD
	Command_PacketStru.nResponseDataLength = (DWORD)wRegCount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = bInputBuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	for (k = 0; k < wRegCount; k++)
	{
		bUserBuffer[k] = Command_PacketStru.InBuffer[k];
	}
	return OK;
}

FPS_STATUS CIS_FPSReadOTGver(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	cisRegisterContext  *reg_context
)
{

	CHAR* device_path = device_Info->device_path;
	WORD wRegType = reg_context->register_rw_type;             //  
	WORD wRegCount = reg_context->register_number;
	BYTE* bUserBuffer = reg_context->register_address_data;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k < wRegCount; k++)
	{
		bInputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet`
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_GET_OTG_VER;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;                 // TBD
	Command_PacketStru.nResponseDataLength = (DWORD)wRegCount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = bInputBuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	for (k = 0; k < wRegCount; k++)
	{
		bUserBuffer[k] = Command_PacketStru.InBuffer[k];
	}
	return OK;
}

FPS_STATUS FPSGetFramesA(
	__in   VendorDeviceHandle  *VendorDeviceHandle,
	__in   DeviceInfo     *device_Info,                 // device path, device information 
	__in   int            datasize,
	__out  BYTE           *imagebuffer                  // image buffer pointer
	)
{

	CHAR* device_path = device_Info->device_path;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;
	INT totalimagecount;
	INT totalSCSIcount; 

	totalimagecount =  datasize;
	totalSCSIcount = totalimagecount / 512;   // totalpixel

	//
	// image width, height
	//
	imagebuffer[3] = totalSCSIcount & 0x000000FF;
	imagebuffer[2] = (totalSCSIcount >> 8) & 0x000000FF;
	imagebuffer[1] = (totalSCSIcount >> 16) & 0x000000FF;
	imagebuffer[0] = (totalSCSIcount >> 24) & 0x000000FF;

	// 
	// Init command packet
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_GET_IMAGE;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;                 // TBD
	Command_PacketStru.nResponseDataLength = (DWORD)totalimagecount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = imagebuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	return OK;
}


FPS_STATUS FPSGetFramesB(
	__in   VendorDeviceHandle  *VendorDeviceHandle,
	__in   DeviceInfo     *device_Info,                 // device path, device information 
	__in   BYTE           page_num,
	__in   BYTE           page_size,
	__out  BYTE           *imagebuffer                  // image buffer pointer
	)
{

	CHAR* device_path = device_Info->device_path;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;
	INT totalimagecount;
	INT totalSCSIcount; 
	INT datasize;

	datasize = page_size * 1024;
	totalimagecount =  datasize;
	totalSCSIcount = totalimagecount / 512;   // totalpixel

	//
	// image width, height
	//

	imagebuffer[0] = page_num;
	imagebuffer[1] = page_size;

	// 
	// Init command packet
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_GET_IMAGE;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;                 // TBD
	Command_PacketStru.nResponseDataLength = (DWORD)totalimagecount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = imagebuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	return OK;
}


//
// FUNCTION NAME.
//      CIS_Normal_Command
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS CIS_Normal_Command(
	VendorDeviceHandle *VendorDeviceHandle,
	DeviceInfo         *device_Info,           // Device path, device information
	BYTE               cmd_opcode,  
	BYTE*              cmdbuffer               // data buffer pointer
	)
{
	int k;

	CHAR* device_path = device_Info->device_path;

	BYTE bOnputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k<MAX_SCSI_DATA_PACKET_SIZE; k++)
	{
		bOnputBuffer[k] = cmdbuffer[k];
	}

	// 
	// Init command packet
	//
	Command_PacketStru.OpCode = cmd_opcode;				// 

	Command_PacketStru.nCommnadDataLength = 16;			//[TBD]
	Command_PacketStru.nResponseDataLength = 0;
	Command_PacketStru.nWriteDataLength = MAX_SCSI_DATA_PACKET_SIZE;
	Command_PacketStru.OutBuffer = bOnputBuffer;
	Command_PacketStru.InBuffer = NULL;

	// 
	// currently, not use command buffer in storage. (TBD)
	//

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}
	return OK;
}

/* T7805JA */
FPS_STATUS UsbTransmit(
	__in   VendorDeviceHandle* VendorDeviceHandle,
	__in   DeviceInfo* device_Info,		// device path, device information
	__in   BYTE* DataBuffer,
	__in   DWORD DataLength,
	__in   BYTE SCSI_CMD
)
{

	CHAR* device_path = device_Info->device_path;
	BYTE* bUserBuffer = DataBuffer;

	BYTE bOnputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (int k = 0; k < min(MAX_SCSI_DATA_PACKET_SIZE, DataLength); k++)
	{
		bOnputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet
	//
	Command_PacketStru.OpCode = SCSI_CMD;

	Command_PacketStru.nCommnadDataLength = 16;          //[TBD]
	Command_PacketStru.nResponseDataLength = 0;
	Command_PacketStru.nWriteDataLength = MAX_SCSI_DATA_PACKET_SIZE;
	Command_PacketStru.OutBuffer = bOnputBuffer;
	Command_PacketStru.InBuffer = NULL;

	// 
	// currently, not use command buffer in storage. (TBD)
	//

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}
	return OK;
}

FPS_STATUS UsbTransmitReceive(
	__in   VendorDeviceHandle* VendorDeviceHandle,
	__in   DeviceInfo* device_Info,		// device path, device information
	__out  BYTE* DataBuffer,
	__in   BYTE SCSI_CMD
	)
{
	CHAR* device_path = device_Info->device_path;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	memcpy(bInputBuffer, DataBuffer, MAX_SCSI_DATA_PACKET_SIZE);

	// 
	// Init command packet
	//
	Command_PacketStru.OpCode = SCSI_CMD;

	Command_PacketStru.nCommnadDataLength = 16;
	//Command_PacketStru.nCommnadDataLength = 0xff;
	Command_PacketStru.nResponseDataLength = (DWORD)MAX_SCSI_DATA_PACKET_SIZE;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = bInputBuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}
	memcpy(DataBuffer, bInputBuffer, MAX_SCSI_DATA_PACKET_SIZE);
	return OK;
}

FPS_STATUS GetFwVer(
	__in VendorDeviceHandle*  VendorDeviceHandle,
	__in DeviceInfo*          device_Info,           // Device path, device information
	__in BYTE                 reg_type,              // Register read/write type
	__in WORD                 bufferlen,             // data buffer length 
	__in BYTE*                regbuffer              // data buffer pointer
)
{
	cisRegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return CIS_GetFwVer(VendorDeviceHandle, device_Info, &reg_context);
}

FPS_STATUS spiMode(
	__in VendorDeviceHandle*  VendorDeviceHandle,
	__in DeviceInfo*          device_Info,           // Device path, device information
	__in BYTE                 reg_type,              // Register read/write type
	__in WORD                 bufferlen,             // data buffer length 
	__in BYTE*                regbuffer              // data buffer pointer
)
{
	cisRegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return CIS_spiMode(VendorDeviceHandle, device_Info, &reg_context);
}

FPS_STATUS CIS_GetFwVer(
	VendorDeviceHandle*       VendorDeviceHandle,
	DeviceInfo*               device_Info,           // Device path, device information
	cisRegisterContext*       reg_context
)
{
	CHAR* device_path = device_Info->device_path;
	WORD wRegType = reg_context->register_rw_type;
	WORD wRegCount = reg_context->register_number;
	BYTE* bUserBuffer = reg_context->register_address_data;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k < wRegCount; k++)
	{
		bInputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet`
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_GETFWVER;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;
	Command_PacketStru.nResponseDataLength = (DWORD)wRegCount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = bInputBuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	for (k = 0; k < wRegCount; k++)
	{
		bUserBuffer[k] = Command_PacketStru.InBuffer[k];
	}
	return OK;
}

FPS_STATUS CIS_spiMode(
	VendorDeviceHandle*       VendorDeviceHandle,
	DeviceInfo*               device_Info,           // Device path, device information
	cisRegisterContext*       reg_context
)
{
	CHAR* device_path = device_Info->device_path;
	WORD wRegType = reg_context->register_rw_type;
	WORD wRegCount = reg_context->register_number;
	BYTE* bUserBuffer = reg_context->register_address_data;

	INT k;
	BYTE bCmdData[256];
	BYTE bInputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k < wRegCount; k++)
	{
		bInputBuffer[k] = bUserBuffer[k];
	}

	// 
	// Init command packet`
	//
	Command_PacketStru.OpCode = SCSI_HM_CIS_SPIMODE;

	//[TBD] Command_PacketStru.nCommnadDataLength = 0;
	Command_PacketStru.nCommnadDataLength = 16;
	Command_PacketStru.nResponseDataLength = (DWORD)wRegCount;
	Command_PacketStru.OutBuffer = NULL;
	Command_PacketStru.InBuffer = bInputBuffer;
	Command_PacketStru.nWriteDataLength = 0;

	Command_PacketStru.CmdData = bCmdData;

	ErrorCode = PassCommandPacket(VendorDeviceHandle, device_path, &Command_PacketStru);
	if (ErrorCode != OK)
	{
		return ErrorCode;
	}

	for (k = 0; k < wRegCount; k++)
	{
		bUserBuffer[k] = Command_PacketStru.InBuffer[k];
	}
	return OK;
}