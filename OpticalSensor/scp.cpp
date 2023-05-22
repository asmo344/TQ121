//
// FILENAME.
//      SCP.cpp - SCSI Command Processor
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
//

#include "stdafx.h"
#include "ModuleDef.h"
#include "fps_stru.h"

SCSI_PASS_THROUGH_WITH_BUFFERS  sptwb;
SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER  sptdwb;
SRB_ExecSCSICmd ExecSRB;

//
// FUNCTION NAME.
//      SCSI_SendScsiCommand
//
// FUNCTIONAL DESCRIPTION.
//      General SCSI command
//
//
// ENTRY PARAMETERS.
//      Devicehandle             - Input,Ptr to DeviceHandle Structure
//      StorageSendASPI32Command - Input,ASPI Command 
//      CmdLen                   - Input,Command Length
//      fIn0_Out1                - Input,Control the command IN/OUT Flag
//      XferLen                  - Input,Data Length
//      Cmd                      - Input,CBW Command  
//      Buffer                   - Input/Output,Ptr to Data Buffer 
//      Time                     - Input,Timeout value   
//
// EXIT PARAMETERS.
//      Function Return          ¡V EFI status code.
//      Buffer                   - Input/Output,Ptr to Data Buffer 
//

BOOL SCSI_SendScsiCommand(
	VendorDeviceHandle* VendorDeviceHandle,
	pfnSendASPI32Command StorageSendASPI32Command,
	BYTE CmdLen,
	BOOL fIn0_Out1,
	WORD XferLen,
	BYTE* Cmd,
	BYTE* Buffer
	)
{
	ULONG Time = 3;

	//
	//  For WIN2K or WINXP OS  
	//
	if ((VendorDeviceHandle->OS_VERSION == Win2K_XP))
	{
		BOOL status2 = 0;
		ULONG length = 0, returned = 0;

		ZeroMemory(&sptdwb, sizeof(SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER));

		//
		//  Fill the SCSI format Data  
		//
		sptdwb.sptd.Length = sizeof(SCSI_PASS_THROUGH_DIRECT);
		sptdwb.sptd.PathId = 0;
		sptdwb.sptd.TargetId = 1;
		sptdwb.sptd.Lun = 0;
		sptdwb.sptd.CdbLength = CmdLen;
		sptdwb.sptd.SenseInfoLength = 0;
		sptdwb.sptd.DataIn = (UCHAR)fIn0_Out1;
		sptdwb.sptd.DataTransferLength = XferLen;
		sptdwb.sptd.TimeOutValue = Time;
		sptdwb.sptd.DataBuffer = Buffer;
		sptdwb.sptd.SenseInfoOffset = offsetof(SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER, ucSenseBuf);

		sptdwb.sptd.Cdb[0] = Cmd[0];
		sptdwb.sptd.Cdb[1] = Cmd[1];
		sptdwb.sptd.Cdb[2] = Cmd[2];
		sptdwb.sptd.Cdb[3] = Cmd[3];
		sptdwb.sptd.Cdb[4] = Cmd[4];
		sptdwb.sptd.Cdb[5] = Cmd[5];

		if (CmdLen > 6){
			sptdwb.sptd.Cdb[6] = Cmd[6];
			sptdwb.sptd.Cdb[7] = Cmd[7];
			sptdwb.sptd.Cdb[8] = Cmd[8];
			sptdwb.sptd.Cdb[9] = Cmd[9];
		}

		if (CmdLen > 10){
			sptdwb.sptd.Cdb[10] = Cmd[10];
			sptdwb.sptd.Cdb[11] = Cmd[11];
		}

		if (CmdLen > 12){
			sptdwb.sptd.Cdb[12] = Cmd[12];
			sptdwb.sptd.Cdb[13] = Cmd[13];
			sptdwb.sptd.Cdb[14] = Cmd[14];
			sptdwb.sptd.Cdb[15] = Cmd[15];
		}

		length = sizeof(SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER);

		status2 = DeviceIoControl(
			VendorDeviceHandle->fileHandle,
			IOCTL_SCSI_PASS_THROUGH_DIRECT,
			&sptdwb,
			sizeof(SCSI_PASS_THROUGH_DIRECT),
			&sptdwb,
			length,
			&returned,
			FALSE);
		return status2;
	}
	else{
		//
		//  For WINME or WIN98 OS (TBD) 
		//
	}
	return true;
}

//
// FUNCTION NAME.
//      SCSI_SendScsiCommandA
//
// FUNCTIONAL DESCRIPTION.
//      General SCSI command
//
//
// ENTRY PARAMETERS.
//      Devicehandle             - Input,Ptr to DeviceHandle Structure
//      StorageSendASPI32Command - Input,ASPI Command 
//      CmdLen                   - Input,Command Length
//      fIn0_Out1                - Input,Control the command IN/OUT Flag
//      XferLen                  - Input,Data Length
//      Cmd                      - Input,CBW Command  
//      Buffer                   - Input/Output,Ptr to Data Buffer 
//      Time                     - Input,Timeout value   
//
// EXIT PARAMETERS.
//      Function Return          ¡V EFI status code.
//      Buffer                   - Input/Output,Ptr to Data Buffer 
//

BOOL SCSI_SendScsiCommandA(
	VendorDeviceHandle* VendorDeviceHandle,
	pfnSendASPI32Command StorageSendASPI32Command,
	BYTE CmdLen,
	BOOL fIn0_Out1,
	WORD XferLen,
	BYTE* Cmd,
	BYTE* Buffer
	)
{
	ULONG Time = 512;
	//
	//  For WIN2K or WINXP OS  
	//
	if ((VendorDeviceHandle->OS_VERSION == Win2K_XP)){
		BOOL status2 = 0;
		ULONG length = 0, returned = 0;

		ZeroMemory(&sptdwb, sizeof(SCSI_PASS_THROUGH_WITH_BUFFERS));

		//
		//  Fill the SCSI format Data  
		//
		sptdwb.sptd.Length = sizeof(SCSI_PASS_THROUGH);
		sptdwb.sptd.PathId = 0;
		sptdwb.sptd.TargetId = 1;
		sptdwb.sptd.Lun = 0;
		sptdwb.sptd.CdbLength = CmdLen;
		sptdwb.sptd.SenseInfoLength = 0;
		sptdwb.sptd.DataIn = (UCHAR)fIn0_Out1;
		sptdwb.sptd.DataTransferLength = XferLen;
		sptdwb.sptd.TimeOutValue = Time;
		sptdwb.sptd.DataBuffer = Buffer;
		sptdwb.sptd.SenseInfoOffset = offsetof(SCSI_PASS_THROUGH_WITH_BUFFERS, ucSenseBuf);

		sptdwb.sptd.Cdb[0] = Cmd[0];
		sptdwb.sptd.Cdb[1] = Cmd[1];
		sptdwb.sptd.Cdb[2] = Cmd[2];
		sptdwb.sptd.Cdb[3] = Cmd[3];
		sptdwb.sptd.Cdb[4] = Cmd[4];
		sptdwb.sptd.Cdb[5] = Cmd[5];

		if (CmdLen > 6){
			sptdwb.sptd.Cdb[6] = Cmd[6];
			sptdwb.sptd.Cdb[7] = Cmd[7];
			sptdwb.sptd.Cdb[8] = Cmd[8];
			sptdwb.sptd.Cdb[9] = Cmd[9];
		}

		if (CmdLen > 10){
			sptdwb.sptd.Cdb[10] = Cmd[10];
			sptdwb.sptd.Cdb[11] = Cmd[11];
		}

		if (CmdLen > 12){
			sptdwb.sptd.Cdb[12] = Cmd[12];
			sptdwb.sptd.Cdb[13] = Cmd[13];
			sptdwb.sptd.Cdb[14] = Cmd[14];
			sptdwb.sptd.Cdb[15] = Cmd[15];
		}

		length = sizeof(SCSI_PASS_THROUGH_WITH_BUFFERS);

		status2 = DeviceIoControl(
			VendorDeviceHandle->fileHandle,
			IOCTL_SCSI_PASS_THROUGH,
			&sptdwb,
			sizeof(IOCTL_SCSI_PASS_THROUGH),
			&sptdwb,
			length,
			&returned,
			FALSE);
		return status2;
	}
	else{
		//
		//  For WINME or WIN98 OS (TBD) 
		//
	}
	return true;
}

