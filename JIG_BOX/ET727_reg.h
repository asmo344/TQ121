// ET727_reg.h

#pragma once
#ifndef ET727_REG_H
#define ET727_REG_H

#define MODE_SEL_REG 0xA

#define CIS_STATUS_REG 0xB
#define EXPO_STS_BIT 0
#define CHIP_RDY_BIT 4
#define EFUSE_RDY_BIT 5
#define FRM_RDY_BIT 6
#define REG_RDY_BIT 7

#define EXPO_H_REG 0x13
#define EXPO_L_REG 0x14

#define LINELENGTH_H_REG 0x16
#define LINELENGTH_L_REG 0x17

#define REG_IF_CLK_00 0x50
#define REG_IF_CLK_01 0x51
#define REG_DIGI_CLK_SET 0x60
#endif