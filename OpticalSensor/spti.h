/*++

Copyright (c) 1992  Microsoft Corporation

Module Name:

    spti.h

Abstract:

    These are the structures and defines that are used in the
    SPTI.C. 

Author:

Revision History:

--*/

typedef struct _SCSI_PASS_THROUGH_WITH_BUFFERS {
    SCSI_PASS_THROUGH spt;
    ULONG             Filler;      // realign buffers to double word boundary
    UCHAR             ucSenseBuf[32];
    UCHAR             ucDataBuf[512];
} SCSI_PASS_THROUGH_WITH_BUFFERS, *PSCSI_PASS_THROUGH_WITH_BUFFERS;

typedef struct _SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER {
    SCSI_PASS_THROUGH_DIRECT sptd;
    ULONG             Filler;      // realign buffer to double word boundary
    UCHAR             ucSenseBuf[32];
} SCSI_PASS_THROUGH_DIRECT_WITH_BUFFER, *PSCSI_PASS_THROUGH_DIRECT_WITH_BUFFER;
     
/*
VOID
PrintError(ULONG);

VOID
PrintDataBuffer(PUCHAR, ULONG);

VOID
PrintInquiryData(PVOID);

PUCHAR
AllocateAlignedBuffer(ULONG, ULONG);

VOID
PrintStatusResults(BOOL, DWORD, PSCSI_PASS_THROUGH_WITH_BUFFERS, ULONG);

VOID
PrintSenseInfo(PSCSI_PASS_THROUGH_WITH_BUFFERS);

VOID
GetInquiryData();

VOID 
ReadCapacity();

VOID
ReadSectors(ULONG, USHORT, PUCHAR, BOOL);

UCHAR
WriteSectors(ULONG, USHORT, PUCHAR);


VOID 
SCSICmd(PUCHAR);
*/

#define FS_UNSPECIFIED 0
#define FAT12 1
#define FAT16 2
#define FAT32 3

#define FMT_SUCCESS 0
#define FMT_WRITE_PROTECT 1
#define FMT_OTHER_ACCESS_ERROR 2
#define FMT_BAD_CLUSTER_SIZE 3

 
USHORT
Myatoi(char *);

#define BUFSIZE MAX_PATH  
#define  GOOD 0
#define  BAD  1
#define  MAXBadBlk 1024        

//
// Command Descriptor Block constants.
//

#define CDB6GENERIC_LENGTH                   6
#define CDB10GENERIC_LENGTH                  10

#define SETBITON                             1
#define SETBITOFF                            0
//
// Mode Sense/Select page constants.
//

#define MODE_PAGE_ERROR_RECOVERY        0x01
#define MODE_PAGE_DISCONNECT            0x02
#define MODE_PAGE_FORMAT_DEVICE         0x03
#define MODE_PAGE_RIGID_GEOMETRY        0x04
#define MODE_PAGE_FLEXIBILE             0x05
#define MODE_PAGE_VERIFY_ERROR          0x07
#define MODE_PAGE_CACHING               0x08
#define MODE_PAGE_PERIPHERAL            0x09
#define MODE_PAGE_CONTROL               0x0A
#define MODE_PAGE_MEDIUM_TYPES          0x0B
#define MODE_PAGE_NOTCH_PARTITION       0x0C
#define MODE_SENSE_RETURN_ALL           0x3f
#define MODE_SENSE_CURRENT_VALUES       0x00
#define MODE_SENSE_CHANGEABLE_VALUES    0x40
#define MODE_SENSE_DEFAULT_VAULES       0x80
#define MODE_SENSE_SAVED_VALUES         0xc0
#define MODE_PAGE_DEVICE_CONFIG         0x10
#define MODE_PAGE_MEDIUM_PARTITION      0x11
#define MODE_PAGE_DATA_COMPRESS         0x0f

//
// SCSI CDB operation codes
//

#define SCSIOP_TEST_UNIT_READY     0x00
#define SCSIOP_REZERO_UNIT         0x01
#define SCSIOP_REWIND              0x01
#define SCSIOP_REQUEST_BLOCK_ADDR  0x02
#define SCSIOP_REQUEST_SENSE       0x03
#define SCSIOP_FORMAT_UNIT         0x04
#define SCSIOP_READ_BLOCK_LIMITS   0x05
#define SCSIOP_REASSIGN_BLOCKS     0x07
#define SCSIOP_READ6               0x08
#define SCSIOP_RECEIVE             0x08
#define SCSIOP_WRITE6              0x0A
#define SCSIOP_PRINT               0x0A
#define SCSIOP_SEND                0x0A
#define SCSIOP_SEEK6               0x0B
#define SCSIOP_TRACK_SELECT        0x0B
#define SCSIOP_SLEW_PRINT          0x0B
#define SCSIOP_SEEK_BLOCK          0x0C
#define SCSIOP_PARTITION           0x0D
#define SCSIOP_READ_REVERSE        0x0F
#define SCSIOP_WRITE_FILEMARKS     0x10
#define SCSIOP_FLUSH_BUFFER        0x10
#define SCSIOP_SPACE               0x11
#define SCSIOP_INQUIRY             0x12
#define SCSIOP_VERIFY6             0x13
#define SCSIOP_RECOVER_BUF_DATA    0x14
#define SCSIOP_MODE_SELECT         0x15
#define SCSIOP_RESERVE_UNIT        0x16
#define SCSIOP_RELEASE_UNIT        0x17
#define SCSIOP_COPY                0x18
#define SCSIOP_ERASE               0x19
#define SCSIOP_MODE_SENSE          0x1A
#define SCSIOP_START_STOP_UNIT     0x1B
#define SCSIOP_STOP_PRINT          0x1B
#define SCSIOP_LOAD_UNLOAD         0x1B
#define SCSIOP_RECEIVE_DIAGNOSTIC  0x1C
#define SCSIOP_SEND_DIAGNOSTIC     0x1D
#define SCSIOP_MEDIUM_REMOVAL      0x1E
#define SCSIOP_READ_CAPACITY       0x25
#define SCSIOP_READ                0x28
#define SCSIOP_WRITE               0x2A
#define SCSIOP_SEEK                0x2B
#define SCSIOP_LOCATE              0x2B
#define SCSIOP_WRITE_VERIFY        0x2E
#define SCSIOP_VERIFY              0x2F
#define SCSIOP_SEARCH_DATA_HIGH    0x30
#define SCSIOP_SEARCH_DATA_EQUAL   0x31
#define SCSIOP_SEARCH_DATA_LOW     0x32
#define SCSIOP_SET_LIMITS          0x33
#define SCSIOP_READ_POSITION       0x34
#define SCSIOP_SYNCHRONIZE_CACHE   0x35
#define SCSIOP_COMPARE             0x39
#define SCSIOP_COPY_COMPARE        0x3A
#define SCSIOP_WRITE_DATA_BUFF     0x3B
#define SCSIOP_READ_DATA_BUFF      0x3C
#define SCSIOP_CHANGE_DEFINITION   0x40
#define SCSIOP_READ_SUB_CHANNEL    0x42
#define SCSIOP_READ_TOC            0x43
#define SCSIOP_READ_HEADER         0x44
#define SCSIOP_PLAY_AUDIO          0x45
#define SCSIOP_PLAY_AUDIO_MSF      0x47
#define SCSIOP_PLAY_TRACK_INDEX    0x48
#define SCSIOP_PLAY_TRACK_RELATIVE 0x49
#define SCSIOP_PAUSE_RESUME        0x4B
#define SCSIOP_LOG_SELECT          0x4C
#define SCSIOP_LOG_SENSE           0x4D


#pragma pack( push, before_include1 )
#pragma pack(1)

typedef struct _FAT16_BPB {
    UCHAR  BS_jmpBoot[3];    // 0
    UCHAR  BS_OEMName[8];    // 3
    USHORT BPB_BytsPerSec;    // 11
    UCHAR  BPB_SecPerClus;    // 13
    USHORT BPB_RsvdSecCnt;    // 14
    UCHAR  BPB_NumFATs;       // 16
    USHORT BPB_RootEntCnt;    // 17
    USHORT BPB_TotSec16;      // 19
    UCHAR  BPB_Media;         // 21
    USHORT BPB_FATSz16;       // 22
    USHORT BPB_SecPerTrk;     // 24
    USHORT BPB_NumHeads;      // 26
    ULONG  BPB_HiddSec;       // 28
    ULONG  BPB_TotSec32;      // 32
    UCHAR  BS_DrvNum;        // 36
    UCHAR  BS_Reserved1;     // 37
    UCHAR  BS_BootSig;       // 38
    ULONG  BS_VolID;         // 39
    UCHAR  BS_VolLab[11];    // 43
    UCHAR  BS_FilSysType[8]; // 54
    UCHAR  BS_Reserved2[450];
} FAT16_BPB, *PFAT16_BPB;

typedef struct _FAT32_BPB {
    UCHAR  BS_jmpBoot[3];    // 0
    UCHAR  BS_OEMName[8];    // 3
    USHORT BPB_BytsPerSec;    // 11
    UCHAR  BPB_SecPerClus;    // 13
    USHORT BPB_RsvdSecCnt;    // 14
    UCHAR  BPB_NumFATs;       // 16
    USHORT BPB_RootEntCnt;    // 17
    USHORT BPB_TotSec16;      // 19
    UCHAR  BPB_Media;         // 21
    USHORT BPB_FATSz16;       // 22
    USHORT BPB_SecPerTrk;     // 24
    USHORT BPB_NumHeads;      // 26
    ULONG  BPB_HiddSec;       // 28
    ULONG  BPB_TotSec32;      // 32
    ULONG  BPB_FATSz32;       // 36
    USHORT BPB_ExtFlags;      // 40
    USHORT BPB_FSVer;         // 42
    ULONG  BPB_RootClus;      // 44
    USHORT BPB_FSInfo;        // 48
    USHORT BPB_BkBootSec;     // 50
    UCHAR  BPB_Reserved[12];  // 52
    UCHAR  BS_DrvNum;        // 64
    UCHAR  BS_Reserved1;     // 65
    UCHAR  BS_BootSig;       // 66
    ULONG  BS_VolID;         // 67
    UCHAR  BS_VolLab[11];    // 71
    UCHAR  BS_FilSysType[8]; // 82
    UCHAR  BS_Reserved3[422];
} FAT32_BPB, *PFAT32_BPB;

typedef struct _NTFS_BPB {
    UCHAR  BS_jmpBoot[3];    // 0
    UCHAR  BS_OEMName[8];    // 3
	UCHAR  BSReserved1[61];  // 11
    ULONG  BS_VolID;         // 72
    UCHAR  BS_Reserved3[436];// 76
} NTFS_BPB, *PNTFS_BPB;

// For FAT32 only
typedef struct _FSI {
    ULONG LeadSig;
    UCHAR Reserved1[480];	
    ULONG StructSig;
    ULONG Free_Count;
    ULONG Nxt_free;
    UCHAR Reserved2[12];
    ULONG TrailSig;
} FSI, *PFSI;	

typedef struct _MBS {
    UCHAR MBS_BootCmd[0x1BE];
    UCHAR MBS_BootID;        // 1BE
    UCHAR MBS_StartHead;     // 1BF
    UCHAR MBS_StartSec;      // 1C0
    UCHAR MBS_StartCyl;      // 1C1
    UCHAR MBS_SystemID;      // 1C2
    UCHAR MBS_EndHead;       // 1C3
    UCHAR MBS_EndSec;        // 1C4
    UCHAR MBS_EndCyl;        // 1C5
    ULONG MBS_StartLogSec;   // 1C6
    ULONG MBS_PartSize;      // 1CA    

    UCHAR MBS_BootID1;        // 1CE
    UCHAR MBS_StartHead1;     // 1CF
    UCHAR MBS_StartSec1;      // 1D0
    UCHAR MBS_StartCyl1;      // 1D1
    UCHAR MBS_SystemID1;      // 1D2
    UCHAR MBS_EndHead1;       // 1D3
    UCHAR MBS_EndSec1;        // 1D4
    UCHAR MBS_EndCyl1;        // 1D5
    ULONG MBS_StartLogSec1;   // 1D6
    ULONG MBS_PartSize1;      // 1DA

    UCHAR MBS_BootID2;        // 1DE
    UCHAR MBS_StartHead2;     // 1DF
    UCHAR MBS_StartSec2;      // 1E0
    UCHAR MBS_StartCyl2;      // 1E1
    UCHAR MBS_SystemID2;      // 1E2
    UCHAR MBS_EndHead2;       // 1E3
    UCHAR MBS_EndSec2;        // 1E4
    UCHAR MBS_EndCyl2;        // 1E5
    ULONG MBS_StartLogSec2;   // 1E6
    ULONG MBS_PartSize2;      // 1EA

    UCHAR MBS_BootID3;        // 1EE
    UCHAR MBS_StartHead3;     // 1EF
    UCHAR MBS_StartSec3;      // 1F0
    UCHAR MBS_StartCyl3;      // 1F1
    UCHAR MBS_SystemID3;      // 1F2
    UCHAR MBS_EndHead3;       // 1F3
    UCHAR MBS_EndSec3;        // 1F4
    UCHAR MBS_EndCyl3;        // 1F5
    ULONG MBS_StartLogSec3;   // 1F6
    ULONG MBS_PartSize3;      // 1FA
    USHORT MBS_Sig;          //1FE must be 0xAA55
} MBS, *PMBS;


//attribute
#define SECTOR_SIZE_512 0x04
#define SECTOR_SIZE_2048 0x08

#pragma pack( pop, before_include1 )
