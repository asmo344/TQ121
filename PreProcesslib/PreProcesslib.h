#pragma once


#ifndef PREPROCESS_H
#define PREPROCESS_H

#include "imgUD_Process.h"


namespace PreProcess 
{
	extern "C" _declspec(dllexport) void ImgPreProcess(unsigned char* inimg, unsigned char* outimg);
}



#endif
