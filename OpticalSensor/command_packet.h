//
// FILENAME.
//      Command_Packet.h - FPS Command Packet HEADER
//
//      $PATH:
//
// FUNCTIONAL DESCRIPTION. 
//
//
// MODIFICATION HISTORY.
//
// NOTICE.
//      Copyright (C) 2000-2015 EgisTec All Rights Reserved.
//


#ifndef _COMMAND_PACKET_H
#define _COMMAND_PACKET_H

#include "ModuleDef.h"
#include "fps_stru.h"
#include "cis_cmd.h"

#define SPT_SENSE_LENGTH       32

#pragma pack( push, before_include1 )
#pragma pack(1)

//
// Command Packet structure
//

typedef struct _COMMAND_PACKET
{
	BYTE  OpCode;
	BYTE* CmdData;
	DWORD nCommnadDataLength;
	DWORD nResponseDataLength;
	DWORD nWriteDataLength;

	BYTE* OutBuffer;
	BYTE* InBuffer;
} Command_Packet, *PCommand_Packet;

#pragma pack( pop, before_include1 )

FPS_STATUS PassCommandPacket(
	__in VendorDeviceHandle *VendorDeviceHandle,
	__in CHAR               szDevicePath[256],
	__in PCommand_Packet    pCommandPacket
	);

#endif
