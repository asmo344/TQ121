#pragma once
#include <stdio.h>
#include <string>
using namespace System;
#define MAX_NAME_LEN 8
#define MAX_FINGERS 5
namespace ClrPbLibraryR08 {
	public ref class PbLibrary
	{
	public:
		static int PbTest();
		static void PbInit();
		static int PbEnrollStart();
		static int PbEnroll(Byte* pixles, unsigned int rows, unsigned int cols);
		static void PbEnrollEnd(int fid);
		static int PbTemplateWrite(int id, const char* name, const char* filePath);
		static int PbTemplateRead(const char* filePath, char *name, int len);
		static int PbVerify(Byte* pixles, unsigned int rows, unsigned int cols, int* matching_id);
	};
}
