/*
 * FILENAME.
 *      fw_opcode.h - FPS firmware operating code
 *
 *      $PATH:
 *
 * FUNCTIONAL DESCRIPTION.
 *      The header define the firmware operating code for FPS.
 *      In theSTM32F407 project, the Special Vendor SCSI command is in the 
 *      "usbd_msc_scsi.h"  
 *
 * MODIFICATION HISTORY.
 *
 * NOTICE.
 *      Copyright (C) 2000-2014 EgisTec All Rights Reserved.
 */

#ifndef _FW_OPCODE_H_
#define _FW_OPCODE_H_

#include "ModuleDef.h"


/*
#define SVSCSI_FPS_STORE_IMAGE         0xE0
#define SVSCSI_FPS_IMAGE_EXTRACT       0xE1
#define SVSCSI_FPS_TWO_FPS_FEATURE     0xE2


#define SVSCSI_FPS_STORE_IMAGE         0xE0      // store a finger image to STMF4 controller memory   (DEBUG)
#define SVSCSI_FPS_IMAGE_EXTRACT       0xE1      // extract image template                            (DEBUG) 
#define SVSCSI_FPS_TWO_FPS_FEATURE     0xE2      // match two FPS image template                      (DEBUG)
#define SVSCSI_FPS_GET_MEM_DATA        0xE3      // Get memory data for STMF4 controller              (DEBUG)  
#define SVSCSI_FPS_MATCH_DATA          0xE4      // Capture the match                                 (DEBUG)   
*/

/*
 * (0xE4) FPS Match Data sub-function
 */
/*
#define SVSCI_0xE4_IMAGE_DATA_TYPE     0x01      // Capture the image data  
#define SVSCI_0xE4_FEATURE_TYPE        0x02      // Capture the finger template
*/

#define SCSI_FPS_READ_REG              0xF0      // read fingerprint register 
#define SCSI_FPS_WRITE_REG             0xF1      // write fingerprint register 
#define SCSI_FPS_GET_IMAGE             0xF2      // get the single image

/*
 * HeartBeat SCSI command
 */
#define SCSI_HB_GET_SPOOFING_RATE      0xF3      // get anti-spoofing rate 
#define SCSI_HB_READ_REG               0xF4      // heartbeat read register 
#define SCSI_HB_WRITE_REG              0xF5      // heartbeat write register
#define SCSI_HB_GET_DATA               0xF6      // heartbeat read register 
#define SCSI_HB_CONTRL_RESET           0xF7      // control the reset I/O pin 

/*
 * CIS SCSI command
 */
#define SCSI_HM_CIS_READ_REG           0xE0      // read CMOS sensor register
#define SCSI_HM_CIS_WRITE_REG          0xE1      // write fingerprint register
#define SCSI_HM_CIS_GET_IMAGE          0xE2      // get a single image 
#define SCSI_HM_CIS_START_CAPTURE      0xE3      // start capture image 
#define SCSI_HM_CIS_GET_OTG_VER        0xE4      // get the otg version
#define SCSI_HM_CIS_PROCESS_STANDBY    0xE5      // process standby or wake up
#define SCSI_HM_CIS_PROCESS_RST        0xE6      // reset sensor
#define SCSI_HM_CIS_START_TESTPATTERN  0xE7      // start testpattern

#define SCSI_HM_CIS_GETFWVER           0xC0      // read fw ver: 0xA-
#define SCSI_HM_CIS_SPIMODE            0xC1      // change spi mode, mode0,1,2,3 and clk = 6Mhz, 12Mhz, 24Mhz
#endif