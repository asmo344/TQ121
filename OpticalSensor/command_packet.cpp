//
// FILENAME.
//      Command_Packet - Command Packet file
//
//      $PATH:
//
// FUNCTIONAL DESCRIPTION.
//      It is for USB Speical command package format.
//      then send the command and data to external USB storage from DeviceIoControl Interface
//
// MODIFICATION HISTORY.
//
//
// NOTICE.
//      Copyright (C) 2000-2015 EgisTec All Rights Reserved.
//

#include "stdafx.h"
#include "Command_Packet.h"


//
// FUNCTION NAME.
//      PassCommandPacket
//
// FUNCTIONAL DESCRIPTION.
//    
// ENTRY PARAMETERS.
//
// EXIT PARAMETERS.
//      Function Return (FPS_STATUS) 
//      
//

FPS_STATUS PassCommandPacket(
	__in VendorDeviceHandle   *VendorDeviceHandle,
	__in CHAR				    szDevicePath[256],
	__in PCommand_Packet      pCommandPacket
	)
{
	FPS_STATUS ErrorCode = OK;
	SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER sptdwb;

	DWORD nCommnadDataLength = pCommandPacket->nCommnadDataLength;
	DWORD nResponseDataLength = pCommandPacket->nResponseDataLength;
	DWORD nWriteDataLength = pCommandPacket->nWriteDataLength;
	BYTE  OpCode = pCommandPacket->OpCode;
	BYTE* CmdData = pCommandPacket->CmdData;
	BYTE* outBuffer = pCommandPacket->OutBuffer;
	BYTE* inBuffer = pCommandPacket->InBuffer;

	HANDLE hActiveDevice = VendorDeviceHandle->fileHandle;

	BYTE cmd[16];
	memset(cmd, 0, sizeof(cmd));

	//
	// USB Storage Driver Solution
	//
	ZeroMemory(&sptdwb, sizeof(SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER));

	//WriteBuffer = AllocateAlignedBuffer(DataBufLen, alignmentMask, &pUnAlignedBuffer);
	//memcpy(WriteBuffer, DataBuffer, DataBufLen);

	sptdwb.sptd.Length = sizeof(SCSI_PASS_THROUGH_DIRECT);
	sptdwb.sptd.PathId = 0;
	sptdwb.sptd.TargetId = 1;
	sptdwb.sptd.Lun = 0;
	sptdwb.sptd.CdbLength = nCommnadDataLength;
	sptdwb.sptd.SenseInfoLength = SPT_SENSE_LENGTH;
	sptdwb.sptd.TimeOutValue = 5000; // ms
	sptdwb.sptd.SenseInfoOffset = offsetof(SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER, ucSenseBuf);
	//sptdwb.sptd.DataIn = SCSI_IOCTL_DATA_OUT;
	//sptdwb.sptd.DataBuffer = DataBuffer;

	if (outBuffer != NULL)                                      // [TBD] ,it means the outBuffer have vlaue
	{
		sptdwb.sptd.DataIn = SCSI_IOCTL_DATA_OUT;
		sptdwb.sptd.DataBuffer = outBuffer;
		sptdwb.sptd.DataTransferLength = nWriteDataLength;
	}
	else                                                        // [TBD] ,it means the inBuffer have vlaue
	{
		sptdwb.sptd.DataIn = SCSI_IOCTL_DATA_IN;
		sptdwb.sptd.DataBuffer = inBuffer;
		sptdwb.sptd.DataTransferLength = nResponseDataLength;
		//cmd[1] = inBuffer[0];                                   // [TBD] read one byte 
		//cmd[2] = inBuffer[1];                                   // [TBD] read one byte 
		//cmd[3] = inBuffer[2];                                   // [TBD] read one byte 
		//cmd[4] = inBuffer[3];                                   // [TBD] read one byte 

		for (int i = 0; i < nCommnadDataLength - 1; i++)
		{
			cmd[i + 1] = inBuffer[i];
		}
	}

	int length;

	cmd[0] = OpCode;
	length = min(sizeof(cmd), sizeof(sptdwb.sptd.Cdb));	
	memcpy(sptdwb.sptd.Cdb, cmd, length);

	length = sizeof(SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER);
	ULONG returned = 0;
	BOOL status = 0;

	status = DeviceIoControl(	
		hActiveDevice,                            // Device handle 
		IOCTL_SCSI_PASS_THROUGH_DIRECT,           // IO controller code
		&sptdwb,                                  // Input Buffer Pointer  
		length,                                   // Input Buffer Length
		&sptdwb,                                  // Ouput Buffer Pointer
		length,                                   // Ouput Buffer Length
		&returned,                                // Return Value 
		FALSE);

	return status;
}