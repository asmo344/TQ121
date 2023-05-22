//
// FILENAME.
//      SCP.h - SCSI Command Processor Header File
//
//      $PATH:
//
// FUNCTIONAL DESCRIPTION.
//      General Command packet Format for storage SCSI command 
//
//
// MODIFICATION HISTORY.
//
// NOTICE.
//      Copyright (C) 2000-2017 MoreDNA All Rights Reserved.
//

#ifndef _SCPINC_H
#define _SCPINC_H

#include "ModuleDef.h"
#include "fps_stru.h"

//
// General SCSI Command
//

BOOL SCSI_SendScsiCommand(
	VendorDeviceHandle* Devicehandle,
	pfnSendASPI32Command StorageSendASPI32Command,
	BYTE CmdLen,
	BOOL fIn0_Out1,
	WORD XferLen,
	BYTE* Cmd,
	BYTE* Buffer
	);

BOOL SCSI_SendScsiCommandA(
	VendorDeviceHandle* Devicehandle,
	pfnSendASPI32Command StorageSendASPI32Command,
	BYTE CmdLen,
	BOOL fIn0_Out1,
	WORD XferLen,
	BYTE* Cmd,
	BYTE* Buffer
	);

BOOL VENDOR_SendCommand(
	VendorDeviceHandle* VendorDeviceHandle,
	LPVOID SendCommand,
	BYTE* Buffer,
	ULONG length
	);

#endif    //_SCPINC_H
