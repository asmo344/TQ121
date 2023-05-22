#pragma once
#include <cstdint>

namespace PG_ISP
{
    public ref class Jugement
    {
    public:
        // ROI RANDGE
        static unsigned int X_A_TH = 36;
        static unsigned int X_B_TH = 180;
        static unsigned int Y_A_TH = 36;
        static unsigned int Y_B_TH = 180;
        static unsigned int P_RANGE = 72;
        static unsigned int Q_RANGE = 144;
        static unsigned int R_RANGE = 144;
        static unsigned int S_RANGE = 360;
        static unsigned int C_RANGE_START = 72;
        static unsigned int C_RANGE_END = 143;
        static unsigned int C_SIZE = 72;

        // THRESHOLD
        static unsigned int IMAGE_DIA_TH = 1023;
        static unsigned int IMAGE_CNT_TH = 1000;
        static unsigned int IMAGE_STOP_TH = 100;
        static unsigned int BACKGROUND_TH = 10;
        static unsigned int ALL_TH_1 = 400;
        static unsigned int ALL_TH_2 = 500;
        static unsigned int ALL_TH_3 = 600;

        static void ImageStaticEV(int* pImage, uint8_t nImageW, uint8_t nImageH, 
            int* nNumOfImage, int* nDonut, int* nD500, int* nD400);
    private:
    };
}

