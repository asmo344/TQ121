#include "stdafx.h"
#include "Matrix_M_N.h"

Matrix_M_N::Matrix_M_N(int h, int w)
{
	Height = h;
	Width = w;
	Size = Height * Width;
	//element = new double[Size];
	for (int sz = 0; sz < Size; sz++)
		element[sz] = 0;
}

Matrix_M_N::Matrix_M_N(int h, int w, unsigned char* matrix)
{
	Height = h;
	Width = w;
	Size = Height * Width;
	//element = new double[Size];
	for (int sz = 0; sz < Size; sz++)
		element[sz] = matrix[sz];
}

Matrix_M_N::Matrix_M_N(int h, int w, const char* symbol)
{
	Height = h;
	Width = w;
	Size = Height * Width;

	int ele;
	//element = new double[Size];
	if (!strncmp(symbol, "ZERO", 4))
		ele = 0;
	else if (!strncmp(symbol, "ONE", 3))
		ele = 1;
	else
		ele = 0;

	for (int sz = 0; sz < Size; sz++)
		element[sz] = ele;
}


Matrix_M_N Matrix_M_N::operator*(double muti)
{
	Matrix_M_N tmp(Height, Width);
	for (int x = 1; x <= Width; x++)
	{
		for (int y = 1; y <= Height; y++)
		{
			tmp.set(y, x, muti * element[(y - 1) * Width + (x - 1)]);
		}
	}
	return tmp;
}

Matrix_M_N Matrix_M_N::operator+(double add)
{
	Matrix_M_N tmp(Height, Width);
	for (int x = 1; x <= tmp.width(); x++)
	{
		for (int y = 1; y <= tmp.height(); y++)
		{
			tmp.set(y, x, add * element[(y - 1) * Width + (x - 1)]);
		}
	}
	return tmp;
}

void Matrix_M_N::operator=(Matrix_M_N data)
{
	for (int x = 1; x <= Width; x++)
	{
		for (int y = 1; y <= Height; y++)
		{
			element[(y - 1) * Width + (x - 1)] = data.get(y, x);
		}
	}
}

void Matrix_M_N::operator=(double data)
{
	for (int sz = 0; sz < Size; sz++)
		element[sz] = data;
}

Matrix_M_N Matrix_M_N::sub_matrix_y(int y1, int y2)
{
	// y2 > y1;
	Matrix_M_N matrix_tmp(y2 - y1 + 1, Width);
	for (int x = 1; x <= Width; x++)
	{
		for (int y = 1; y <= y2 - y1 + 1; y++)
		{
			matrix_tmp.set(y, x, element[(y - 2 + y1) * Width + (x - 1)]);
		}
	}
	return matrix_tmp;
}

Matrix_M_N Matrix_M_N::sub_matrix_x(int x1, int x2)
{
	// x2 > x1;
	Matrix_M_N matrix_tmp(Height, x2 - x1 + 1);
	for (int x = 1; x <= x2 - x1 + 1; x++)
	{
		for (int y = 1; y <= Height; y++)
		{
			matrix_tmp.set(y, x, element[y * Width + (x + x1 - 2)]);
		}
	}
	return matrix_tmp;
}

void Matrix_M_N::sub_matrix_set_y(int y1, int y2, Matrix_M_N data)
{
	// y2 > y1;
	for (int x = 1; x <= Width; x++)
	{
		for (int y = y1; y <= y2; y++)
		{
			element[(y - 1) * Width + (x - 1)] = data.get(y - y1 + 1, x);
		}
	}
}

void Matrix_M_N::sub_matrix_set_y(int y1, int y2, double data)
{
	// y2 > y1;
	for (int x = 1; x <= Width; x++)
	{
		for (int y = y1; y <= y2; y++)
		{
			element[(y - 1) * Width + (x - 1)] = data;
		}
	}
}

void Matrix_M_N::sub_matrix_set_x(int x1, int x2, Matrix_M_N data)
{
	// x2 > x1;
	for (int x = 1; x <= Width; x++)
	{
		for (int y = 1; y <= Height; y++)
		{
			element[(y - 1) * Width + (x - 1)] = data.get(y, x - x1 + 1);
		}
	}
}

double Matrix_M_N::get(int h, int w)
{
	return element[(h - 1)*Width + (w - 1)];
}

void Matrix_M_N::set(int h, int w, double value)
{
	element[(h - 1)*Width + (w - 1)] = value;
}

double Matrix_M_N::max_ele()
{
	double tmp = element[0];
	for (int sz = 0; sz < Size; sz++)
	{
		if (element[sz] > tmp)
			tmp = element[sz];
	}
	return tmp;
}

double Matrix_M_N::min_ele()
{
	double tmp = element[0];
	for (int sz = 0; sz < Size; sz++)
	{
		if (element[sz] < tmp)
			tmp = element[sz];
	}

	return tmp;
}

double Matrix_M_N::mean()
{
	double tmp = 0;
	for (int sz = 0; sz < Size; sz++)
	{
		tmp += element[sz];
	}
	return tmp / Size;
}

double Matrix_M_N::std()
{
	double mean_tmp = mean(), std_tmp = 0;
	for (int sz = 0; sz < Size; sz++) {
		std_tmp += (element[sz] - mean_tmp) * (element[sz] - mean_tmp);
	}
	std_tmp /= (Size - 1);
	std_tmp = sqrt(std_tmp);

	return std_tmp;
}

int Matrix_M_N::size()
{
	return Size;
}

int Matrix_M_N::height()
{
	return Height;
}

int Matrix_M_N::width()
{
	return Width;
}

Matrix_M_N Matrix_M_N::find(double data)
{
	int size = 0;
	for (int x = 1; x <= Width; x++)
	{
		for (int y = 1; y <= Height; y++)
		{
			if (data == element[(y - 1) * Width + (x - 1)])
				size++;
		}
	}

	Matrix_M_N matrix(size, 3);

	size = 1;
	for (int x = 1; x <= Width; x++)
	{
		for (int y = 1; y <= Height; y++)
		{
			if (data == element[(y - 1) * Width + (x - 1)])
			{
				matrix.set(size, 1, y);
				matrix.set(size, 2, x);
				matrix.set(size, 3, data);
				size++;
			}
		}
	}

	return matrix;
}