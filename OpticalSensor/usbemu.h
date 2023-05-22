
#ifndef _USBEMU_H
#define _USBEMU_H

#include "ModuleDef.h"
#include "fps_stru.h"

//
// RET STATUS
//
#define EMU_GET_DEVICE_SUCCESS_EGIS                 1              // Get the correct device
#define EMU_GET_DEVICE_SUCCESS_YINS					2
#define EMU_GET_DEVICE_SUCCESS_TY7805               3
#define EMU_FALSE_NO_DEVICE_HANDLE             0xFFFFE001     // Fail to get the device handle
#define EMU_FALSE_GET_CAPACITY                 0xFFFFE002     // Fail to get the cappacity
#define EMU_FALSE_GET_INQUIRY                  0xFFFFE003     // Fail to get the inquiry

//
// USB storage Enumerate function
//
int USBStorageEnumerate(
	VendorDeviceHandle *VendorDeviceHandle,
	DeviceInfoDesciptor *DeviceInfo,
	BYTE *DeviceTotal
	);




#endif
