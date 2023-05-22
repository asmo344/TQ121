//
// FILENAME.
//      FPSCOMD - FPS Command, 
//
//      $PATH:
//
// FUNCTIONAL DESCRIPTION.
//      Interface of FPS Sensor Module Application 
//      Processing the gerenal or special SCSI command Packet 
//
// MODIFICATION HISTORY.
//
// NOTICE.
//      Copyright (C) 2000-2015 EgisTec All Rights Reserved.
//

#include "stdafx.h"
#include "FPSCOMD.h"

//
// FUNCTION NAME.
//      FPSStoreMem
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS FPSStoreMem(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in WORD                reg_type,            // Register read/write type
	__in WORD                bufferlen,           // data buffer length 
	__in BYTE*               regbuffer            // data buffer pointer
	)
{
	RegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return FPSStoreImageData(VendorDeviceHandle, device_Info, &reg_context);
}


//
// Set Extract Image 
//
FPS_STATUS SetExtractImage(
    __in VendorDeviceHandle* VendorDeviceHandle,
    __in DeviceInfo*         device_Info,         // Device path, device information
    __in WORD                reg_type,            // Register read/write type
    __in WORD                bufferlen,           // data buffer length 
    __in BYTE*               regbuffer            // data buffer pointer
   )
{
	RegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return FPSStoreImageData(VendorDeviceHandle, device_Info, &reg_context);
}



//
// FUNCTION NAME.
//      FPSSetRegisters
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS FPSSetRegisters(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*         device_Info,         // Device path, device information
	__in WORD                reg_type,            // Register read/write type
	__in WORD                bufferlen,           // data buffer length 
	__in BYTE*               regbuffer            // data buffer pointer
	)
{
	RegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return FPSWriteRegister(VendorDeviceHandle, device_Info, &reg_context);
}


//
// FUNCTION NAME.
//      FPSGetRegisters
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS FPSGetRegisters(
	__in VendorDeviceHandle* VendorDeviceHandle,
	__in DeviceInfo*       device_Info,           // Device path, device information
	__in BYTE              reg_type,              // Register read/write type
	__in WORD              bufferlen,             // data buffer length 
	__in BYTE*             regbuffer              // data buffer pointer
	)
{
	RegisterContext reg_context;
	reg_context.register_rw_type = (WORD)reg_type;
	reg_context.register_address_data = regbuffer;
	reg_context.register_number = (WORD)bufferlen;

	return FPSReadRegister(VendorDeviceHandle, device_Info, &reg_context);
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

FPS_STATUS FPSWriteRegister(
	VendorDeviceHandle *VendorDeviceHandle,
	DeviceInfo         *device_Info,         // Device path, device information
	RegisterContext    *reg_context
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
	Command_PacketStru.OpCode = SCSI_FPS_WRITE_REG;

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

FPS_STATUS FPSSetReset(
	VendorDeviceHandle *VendorDeviceHandle,
	DeviceInfo         *device_Info,         // Device path, device information
	BYTE gpio_status
	)
{
	CHAR* device_path = device_Info->device_path;
	WORD wRegCount = 5;
	int k;

	BYTE bOnputBuffer[MAX_SCSI_DATA_PACKET_SIZE];
	FPS_STATUS ErrorCode;
	Command_Packet Command_PacketStru;

	for (k = 0; k<wRegCount; k++)
	{
		bOnputBuffer[k] = 0;
	}
	bOnputBuffer[0] = gpio_status;

	// 
	// Init command packet
	//
	Command_PacketStru.OpCode = SCSI_HB_CONTRL_RESET;
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

FPS_STATUS FPSReadRegister(
	VendorDeviceHandle  *VendorDeviceHandle,
	DeviceInfo          *device_Info,           // Device path, device information
	RegisterContext     *reg_context
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
	Command_PacketStru.OpCode = SCSI_FPS_READ_REG;

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


//
// FUNCTION NAME.
//      FPSGetImage
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
//      VendorDeviceHandle  : vendor device handle
//      device_Info : device information
//      frameinfo   : frame information structure 
//      imagebuffer : image buffer
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      

FPS_STATUS FPSGetFrames(
	__in   VendorDeviceHandle  *VendorDeviceHandle,
	__in   DeviceInfo     *device_Info,                 // device path, device information 
	__in   FrameInfo      *frameinfo,                   // frame information
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

	totalimagecount = (frameinfo->column)*(frameinfo->row);
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
	Command_PacketStru.OpCode = SCSI_FPS_GET_IMAGE;

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
