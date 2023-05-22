// PreProcesslib.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "imgUD_Process.h"
#include "PreProcesslib.h"



using namespace PreProcess;
extern "C" _declspec(dllexport) void ImgPreProcess(unsigned char* inimg, unsigned char* outimg)
{
	imgProcUD(inimg,outimg);
}


