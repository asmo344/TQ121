// optical_sensor.h : main header file for the optical_sensor DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// Coptical_sensorApp
// See optical_sensor.cpp for the implementation of this class
//

class Coptical_sensorApp : public CWinApp
{
public:
	Coptical_sensorApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
