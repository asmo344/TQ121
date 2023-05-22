#include <cstdio>
#include <cstdlib>
#include "stdafx.h"
#include "Jugement.h"
#include <iostream>

using namespace PG_ISP;

void Jugement::ImageStaticEV(int* pImage, uint8_t nImageW, uint8_t nImageH, 
    int* nNumOfImage, int* nDonut, int* nD500, int* nD400)
{
    uint16_t nImageLength = nImageW * nImageH;
    uint16_t** a2DArray = NULL;
    a2DArray = new uint16_t * [nImageW];
    for (int idx = 0; idx < nImageW; idx++)
    {
        a2DArray[idx] = new uint16_t[nImageH];
    }

    uint16_t aJudgeTH1Cnt[3] = { 0 };
    uint16_t aJudgeTH2Cnt[3] = { 0 };
    uint16_t aJudgeTH3Cnt[3] = { 0 };
    uint32_t nC_ArraySum = 0;
    uint16_t nC_ArrayAvg = 0;
    uint16_t nC_ArrayMax = 0;
    uint8_t nResult = false;
    uint16_t nNumberofImage = 0, nSumofImage = 0;
    uint16_t nOutPutD500, nOutPutD400, nOutPutNumberofImage, nOutPutDonuts;

    // 1維 轉 2 維
    for (uint16_t idx = 0; idx < nImageLength; idx++)
    {

        a2DArray[idx / nImageW][idx % nImageH] = pImage[idx];
        //printf(" a2DArray[idx]=%d \n", a2DArray[idx / IMAGE_W][idx % IMAGE_W]);
    }

    //處理2維
    for (uint16_t X = 0; X < nImageH; X++)
    {
        for (uint16_t Y = 0; Y < nImageW; Y++)
        {
            if ((Y < Y_A_TH) || (Y > Y_B_TH))// Step1  Y 36 180
            {
                if (a2DArray[X][Y] > ALL_TH_1)
                {
                    aJudgeTH1Cnt[0]++;
                }
                if (a2DArray[X][Y] > ALL_TH_2)
                {
                    aJudgeTH2Cnt[0]++; // 
                }
                if (a2DArray[X][Y] > ALL_TH_3)
                {
                    aJudgeTH3Cnt[0]++; // 
                }
            }
            else if ((X < X_A_TH) || (X > X_B_TH))// Step2  X 36 180
            {
                if (a2DArray[X][Y] > ALL_TH_1)
                {
                    aJudgeTH1Cnt[0]++;
                }
                if (a2DArray[X][Y] > ALL_TH_2)
                {
                    aJudgeTH2Cnt[0]++; // 
                }
                if (a2DArray[X][Y] > ALL_TH_3)
                {
                    aJudgeTH3Cnt[0]++; // 
                }
            }
            else if (     // Step 3 PQRS
                ((X + (1 * Y)) < P_RANGE) || ((X - (1 * Y)) > Q_RANGE) ||
                ((Y - (1 * X)) < R_RANGE) || ((X + (1 * Y)) < S_RANGE)
                )
            {
                if (a2DArray[X][Y] > ALL_TH_1)
                {
                    //aJudgeTH1Cnt[0]++; 
                }
                if (a2DArray[X][Y] > ALL_TH_2)
                {
                    //aJudgeTH2Cnt[0]++; // 
                }
            }

            //Step 4 C
            if ((X >= C_RANGE_START && X <= C_RANGE_END) &&
                (Y >= C_RANGE_START && Y <= C_RANGE_END)
                )
            {
                nC_ArraySum += a2DArray[X][Y];
                nC_ArrayMax = max(a2DArray[X][Y], nC_ArrayMax);

                if (a2DArray[X][Y] >= IMAGE_DIA_TH) // 1023
                {
                    nNumberofImage++;
                }
            }
            else
            {
                if (a2DArray[X][Y] >= IMAGE_CNT_TH)
                {
                    nSumofImage++;
                }
            }
        }
    }
    nC_ArrayAvg = nC_ArraySum / (C_SIZE * C_SIZE);

    // Out Vallue
    if (nSumofImage > IMAGE_STOP_TH)
    {
        nOutPutDonuts = 1000;
    }
    else
    {
        nOutPutDonuts = 0;
    }
    nOutPutD400 = aJudgeTH1Cnt[0];//D400
    nOutPutD500 = aJudgeTH2Cnt[0];//D500
    nOutPutNumberofImage = nNumberofImage; // 大於 1023 個數
   // nOutPutDonuts  // Donuts

    *nNumOfImage = nOutPutNumberofImage;
    *nDonut = nOutPutDonuts;
    *nD500 = nOutPutD500;
    *nD400 = nOutPutD400;

    // release pointer
    delete[] a2DArray;    
}