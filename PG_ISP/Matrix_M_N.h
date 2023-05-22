#pragma once
#include "math.h"

class Matrix_M_N {
public:
	Matrix_M_N(int h, int w);
	Matrix_M_N(int h, int w, unsigned char* matrix);
	Matrix_M_N(int h, int w, const char* symbol);
	~Matrix_M_N() {}
	Matrix_M_N operator*(double muti);
	Matrix_M_N operator+(double add);
	void operator=(Matrix_M_N data);
	void operator=(double data);
	Matrix_M_N sub_matrix_y(int y1, int y2);
	Matrix_M_N sub_matrix_x(int x1, int x2);
	void sub_matrix_set_y(int y1, int y2, Matrix_M_N data);
	void sub_matrix_set_y(int y1, int y2, double data);
	void sub_matrix_set_x(int x1, int x2, Matrix_M_N data);
	double get(int h, int w);
	void set(int h, int w, double value);
	double max_ele();
	double min_ele();
	double mean();
	double std();
	int size();
	int height();
	int width();
	Matrix_M_N find(double data);
private:
	int Height;
	int Width;
	int Size;
	double element[50];
};
