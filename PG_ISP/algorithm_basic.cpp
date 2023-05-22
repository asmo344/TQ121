#include "stdafx.h"
#include "algorithm_basic.h"

uint16_t RESULT_ARRAY_SIZE = 0;

void InitArrayOfCluster()
{
	for (uint16_t i = 0; (i) < TH_SCALE; i++)
	{
		t_BlockCluster.aTotalNumofScale[i] = 0;
		t_ResultCluster.aTotalNumofScale[i] = 0;
	}

	for (uint16_t i = 0; (i) < TH_SCALE; i++)
	{
		for (uint16_t j = 0; (j) < MAX_ONCE_BLOCK_NUMBER; j++)
		{
			t_BlockCluster.aBlockCoordinateOfScale[i][j] = 0;
			t_ResultCluster.aBlockCoordinateOfScale[i][j] = 0;
		}
	}
}

void UpdateBBMap(uint8_t* pSrcBBMap, uint8_t* pDstBBMap)
{
	for (uint16_t i = 0; (i) < RESULT_ARRAY_SIZE; i++)
	{
		pDstBBMap[i] = pSrcBBMap[i];
	}
}

#if 1 // à笲衡
uint8_t BorderConnectJudge(uint16_t nImageW, uint16_t nImageH, uint8_t* pBBindex, uint8_t nAlgoStep, uint8_t nIndex, uint8_t nBorderArea)
{
	uint8_t nTrueConnecCnt = 0;
	uint8_t nStep2TH = TH_CLASS3_BB_DEPUTY_NUMBER;

	if (nAlgoStep == Step2)
	{
		if (nBorderArea == Top)
		{

			//Block 8硈硄  璸计

			if (pBBindex[nIndex] != 0) //Start
			{
				if (pBBindex[nIndex - 1] != nStep2TH)//オ
				{
					nTrueConnecCnt++;
				}
				if (pBBindex[nIndex + nImageW - 1] != nStep2TH)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW - 1)) // End
			{
				if (pBBindex[nIndex + 1] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
				if (pBBindex[nIndex + nImageW + 1] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex + nImageW] != nStep2TH)//
			{
				nTrueConnecCnt++;
			}



		}
		else if (nBorderArea == Bottom)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != (nImageW * nImageH) - nImageW) //オ Start
			{
				if (pBBindex[nIndex - 1] != nStep2TH)//オ
				{
					nTrueConnecCnt++;
				}

				if (pBBindex[nIndex - nImageW - 1] != nStep2TH)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW * nImageH) - 1) // End
			{
				if (pBBindex[nIndex + 1] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
				if (pBBindex[nIndex - nImageW + 1] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex - nImageW] != nStep2TH)//
			{
				nTrueConnecCnt++;
			}


		}
		else if (nBorderArea == Left)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != 0) //Start
			{
				if (pBBindex[nIndex - nImageW] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
				if (pBBindex[nIndex - nImageW + 1] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW * nImageH) - nImageW)  // End
			{
				if (pBBindex[nIndex + nImageW] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
				if (pBBindex[nIndex + nImageW + 1] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
			}

			if (pBBindex[nIndex + 1] != nStep2TH)//
			{
				nTrueConnecCnt++;
			}


		}
		else
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != (nImageW - 1)) // End
			{
				if (pBBindex[nIndex - nImageW] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
				if (pBBindex[nIndex - nImageW - 1] != nStep2TH)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW * nImageH) - 1) //  End
			{
				if (pBBindex[nIndex + nImageW] != nStep2TH)//
				{
					nTrueConnecCnt++;
				}
				if (pBBindex[nIndex + nImageW - 1] != nStep2TH)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex - 1] != nStep2TH)//オ
			{
				nTrueConnecCnt++;
			}
		}
	}
	else if (nAlgoStep == Step3)
	{
		if (nBorderArea == Top)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != 0) //オ Start 
			{
				if (
					pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex + nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW - 1)) // End
			{
				if (
					pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex + nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
			}

			if (
				pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//
			{
				nTrueConnecCnt++;
			}


		}
		else if (nBorderArea == Bottom)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != (nImageW * nImageH) - nImageW) //オ Start
			{
				if (
					pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex - nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}

			if (pBBindex[nIndex] != (nImageW * nImageH) - 1) //  End
			{
				if (
					pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}

				if (
					pBBindex[nIndex - nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
			}
			if (
				pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//
			{
				nTrueConnecCnt++;
			}
		}
		else if (nBorderArea == Left)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != 0) //オ  Start
			{
				if (
					pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex - nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW * nImageH) - nImageW)  //  オ Start	
			{
				if (
					pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}

				if (
					pBBindex[nIndex + nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
			}
			if (
				pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//
			{
				nTrueConnecCnt++;
			}

		}
		else
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != (nImageW - 1)) // End
			{
				if (
					pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex - nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW * nImageH) - 1) //  End
			{
				if (
					pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}

				if (
					pBBindex[nIndex + nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (
				pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//オ
			{
				nTrueConnecCnt++;
			}

		}
	}
	else if (nAlgoStep == Step4)
	{
		if (nBorderArea == Top)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != 0) //オ Start 
			{
				if (
					pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex + nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW - 1)) // End
			{
				if (
					pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex + nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
			}

			if (
				pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//
			{
				nTrueConnecCnt++;
			}


		}
		else if (nBorderArea == Bottom)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != (nImageW * nImageH) - nImageW) //オ Start
			{
				if (
					pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex - nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}

			if (pBBindex[nIndex] != (nImageW * nImageH) - 1) //  End
			{
				if (
					pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex - nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}

			}

			if (
				pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//
			{
				nTrueConnecCnt++;
			}

		}
		else if (nBorderArea == Left)
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != 0) //オ  Start
			{
				if (
					pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex - nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW * nImageH) - nImageW)  //  オ Start		
			{
				if (
					pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}

				if (
					pBBindex[nIndex + nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
			}
			if (
				pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//
			{
				nTrueConnecCnt++;
			}

		}
		else
		{
			//Block 8硈硄  璸计
			if (pBBindex[nIndex] != (nImageW - 1)) // End
			{
				if (
					pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}
				if (
					pBBindex[nIndex - nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex - nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (pBBindex[nIndex] != (nImageW * nImageH) - 1) //  End
			{
				if (
					pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//
				{
					nTrueConnecCnt++;
				}

				if (
					pBBindex[nIndex + nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
					pBBindex[nIndex + nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
					)//オ
				{
					nTrueConnecCnt++;
				}
			}
			if (
				pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
				pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
				)//オ
			{
				nTrueConnecCnt++;
			}

		}
	}
	else
	{

	}
	return nTrueConnecCnt;
}
#endif //à笲衡

uint8_t ConnectJudge(uint16_t nImageW, uint16_t nImageH, uint8_t* pBBindex, uint16_t nAlgoStep, uint8_t nIndex)
{
	uint8_t nTrueConnecCnt = 0;
	uint8_t nStep2TH = TH_CLASS3_GB_DEPUTY_NUMBER;

	if (nAlgoStep == Step2)
	{
		//Block 8硈硄  璸计
		if (pBBindex[nIndex + 1] != nStep2TH)//
		{
			nTrueConnecCnt++;
		}
		if (pBBindex[nIndex - 1] != nStep2TH)//オ
		{
			nTrueConnecCnt++;
		}
		if (pBBindex[nIndex + nImageW] != nStep2TH)//
		{
			nTrueConnecCnt++;
		}
		if (pBBindex[nIndex - nImageW] != nStep2TH)//
		{
			nTrueConnecCnt++;
		}
		if (pBBindex[nIndex + nImageW + 1] != nStep2TH)//
		{
			nTrueConnecCnt++;
		}
		if (pBBindex[nIndex - nImageW + 1] != nStep2TH)//
		{
			nTrueConnecCnt++;
		}
		if (pBBindex[nIndex - nImageW - 1] != nStep2TH)//オ
		{
			nTrueConnecCnt++;
		}
		if (pBBindex[nIndex + nImageW - 1] != nStep2TH)//オ
		{
			nTrueConnecCnt++;
		}
	}
	else if (nAlgoStep == Step3)
	{
		//Block 8硈硄  璸计
		if (
			pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}

		if (
			pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//オ
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex - nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//オ
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex - nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex + nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//オ
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex + nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
	}
	else if (nAlgoStep == Step4)
	{
		//Block 8硈硄  璸计
		if (
			pBBindex[nIndex - nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}

		if (
			pBBindex[nIndex + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//オ
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex + nImageW] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex - nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//オ
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex - nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex - nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex + nImageW - 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW - 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW - 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW - 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW - 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//オ
		{
			nTrueConnecCnt++;
		}
		if (
			pBBindex[nIndex + nImageW + 1] == TH_CLASS1_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW + 1] == TH_CLASS2_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW + 1] == TH_CLASS3_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW + 1] == TH_CLASS4_BB_DEPUTY_NUMBER ||
			pBBindex[nIndex + nImageW + 1] == TH_CLASS5_BB_DEPUTY_NUMBER
			)//
		{
			nTrueConnecCnt++;
		}
	}
	else
	{

	}

	return nTrueConnecCnt;
}


uint16_t algorithm_basic::InitialResultArraySize(uint16_t nImageW, uint16_t nImageH)
{	
	RESULT_ARRAY_SIZE = ((nImageW / PICK_W_SIZE) * (nImageH / PICK_W_SIZE));

	t_BlockCluster.aBB1Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB2Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB3Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB4Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB5Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB6Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB7Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB8Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB9Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB10Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_BlockCluster.aBB11Map = new uint8_t[RESULT_ARRAY_SIZE];

	t_ResultCluster.aBB1Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB2Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB3Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB4Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB5Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB6Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB7Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB8Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB9Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB10Map = new uint8_t[RESULT_ARRAY_SIZE];
	t_ResultCluster.aBB11Map = new uint8_t[RESULT_ARRAY_SIZE];

	return RESULT_ARRAY_SIZE;
}

void algorithm_basic::Step1PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pResult)
{
	uint8_t aPickTh[4] = { 217,204,51,38 };

	InitArrayOfCluster();

	for (uint16_t i = 0; (i) < (nImageH) * (nImageW); i++)
	{
		if (pResult[i] > aPickTh[0])
		{
			t_BlockCluster.aBB1Map[i] = TH_CLASS1_BB_DEPUTY_NUMBER;
			t_BlockCluster.aBlockCoordinateOfScale[0][t_BlockCluster.aTotalNumofScale[0]] = i;
			t_BlockCluster.aTotalNumofScale[0]++;
		}
		else if (pResult[i] <= aPickTh[0] && pResult[i] > aPickTh[1])
		{
			t_BlockCluster.aBB1Map[i] = TH_CLASS2_BB_DEPUTY_NUMBER;
			t_BlockCluster.aBlockCoordinateOfScale[1][t_BlockCluster.aTotalNumofScale[1]] = i;
			t_BlockCluster.aTotalNumofScale[1]++;
		}
		else if (pResult[i] <= aPickTh[1] && pResult[i] > aPickTh[2])
		{
			t_BlockCluster.aBB1Map[i] = TH_CLASS3_GB_DEPUTY_NUMBER;
			t_BlockCluster.aBlockCoordinateOfScale[2][t_BlockCluster.aTotalNumofScale[2]] = i;
			t_BlockCluster.aTotalNumofScale[2]++;
		}
		else if (pResult[i] <= aPickTh[2] && pResult[i] > aPickTh[3])
		{
			t_BlockCluster.aBB1Map[i] = TH_CLASS4_BB_DEPUTY_NUMBER;
			t_BlockCluster.aBlockCoordinateOfScale[3][t_BlockCluster.aTotalNumofScale[3]] = i;
			t_BlockCluster.aTotalNumofScale[3]++;
		}
		else if (pResult[i] <= aPickTh[3])
		{
			t_BlockCluster.aBB1Map[i] = TH_CLASS5_BB_DEPUTY_NUMBER;
			t_BlockCluster.aBlockCoordinateOfScale[4][t_BlockCluster.aTotalNumofScale[4]] = i;
			t_BlockCluster.aTotalNumofScale[4]++;
		}
	}
}

void algorithm_basic::Step2PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pOriBBMap, uint8_t* pInputBBMap, uint8_t* pOutputBBMap, uint8_t nBBClass)
{
	uint8_t nNormalJudgeCnt = 0;
	uint8_t nBorderJudgeCnt = 0;
	uint8_t bBorderFlag = false;

	UpdateBBMap(pInputBBMap, pOutputBBMap);

	for (uint16_t i = 0; (i) < t_BlockCluster.aTotalNumofScale[nBBClass]; i++)
	{

		if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] < nImageW ||
			t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] >  ((nImageW * nImageH) - nImageW) ||
			(t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW) == 0 ||
			t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW == (nImageW - 1)
			)
		{
			bBorderFlag = true;
			if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] < nImageW) // 娩
			{
				//printf("Top\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step2, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Top);
			}
			else if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] > ((nImageW * nImageH) - nImageW)) //娩
			{
				//printf("Bottom\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step2, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Bottom);
			}
			else if ((t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW) == 0) //オ娩
			{
				//printf("Left\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step2, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Left);
			}
			else // 娩
			{
				//printf("Right\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step2, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Right);
			}
		}
		else
		{
			//printf("Normal\n");
			bBorderFlag = false;
			//printf("Non_Border\n");
			nNormalJudgeCnt = ConnectJudge(nImageW, nImageH, pOriBBMap, Step2, i);
		}

		//printf("Judge\n");
		// 耞 BB or GB
		if (nBBClass == Class4)
		{
			if (nNormalJudgeCnt >= STEP_TWO_JUDGE_TH && bBorderFlag == false)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS4_GB_DEPUTY_NUMBER;
				//printf("GB4\n");
			}
			if (nBorderJudgeCnt >= (STEP_TWO_JUDGE_TH / 2) && bBorderFlag == true)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS4_GB_DEPUTY_NUMBER;
				//printf("B_GB4\n");
			}
			bBorderFlag = false;
			nNormalJudgeCnt = 0;
			nBorderJudgeCnt = 0;
		}
		else
		{
			//printf("GB2\n");
			if (nNormalJudgeCnt >= STEP_TWO_JUDGE_TH && bBorderFlag == false)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS2_GB_DEPUTY_NUMBER;

			}
			if (nBorderJudgeCnt >= (STEP_TWO_JUDGE_TH / 2) && bBorderFlag == true)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS2_GB_DEPUTY_NUMBER;
				//printf("B_GB2\n");
			}
			bBorderFlag = false;
			nNormalJudgeCnt = 0;
			nBorderJudgeCnt = 0;
		}
	}
}

void algorithm_basic::Step3PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pOriBBMap, uint8_t* pInputBBMap, uint8_t* pOutputBBMap, uint8_t nBBClass)
{
	uint8_t nNormalJudgeCnt = 0;
	uint8_t nBorderJudgeCnt = 0;
	uint8_t bBorderFlag = false;

	UpdateBBMap(pInputBBMap, pOutputBBMap);

	for (uint16_t i = 0; (i) < t_BlockCluster.aTotalNumofScale[nBBClass]; i++)
	{
		//娩矪瞶

		if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] < nImageW ||
			t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] > ((nImageW * nImageH) - nImageW) ||
			(t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW) == 0 ||
			t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW == (nImageW - 1)
			)
		{
			bBorderFlag = true;

			if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] < nImageW) // 娩
			{
				//printf("B_GB3_T\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step3, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Top);
			}
			else if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] > ((nImageW * nImageH) - nImageW)) //娩
			{
				//printf("B_GB3_B\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step3, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Bottom);
			}
			else if ((t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW) == 0) //オ娩
			{
				//printf("B_GB3_L\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step3, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Left);
			}
			else // 娩
			{
				//printf("B_GB3_R\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step3, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Right);
			}
		}
		else
		{
			//Block 8硈硄  耞
			bBorderFlag = false;
			nNormalJudgeCnt = ConnectJudge(nImageW, nImageH, pOriBBMap, Step3, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]);
		}
		// 耞 BB or GB
			//printf("nNormalJudgeCnt=%d\n", nNormalJudgeCnt);
		if (nNormalJudgeCnt >= STEP_THREE_JUDGE_TH && bBorderFlag == false)
			//if (nNormalJudgeCnt <  STEP_THREE_JUDGE_TH && bBorderFlag == false)
		{
			pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS3_BB_DEPUTY_NUMBER;
			//printf("GB3\n");
		}
		if (nBorderJudgeCnt >= (STEP_THREE_JUDGE_TH / 2) && bBorderFlag == true)
			//if (nBorderJudgeCnt < STEP_THREE_JUDGE_TH + 1 && bBorderFlag == true)
		{
			//printf("B_GB3\n");
			pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS3_BB_DEPUTY_NUMBER;

		}
		bBorderFlag = false;
		nNormalJudgeCnt = 0;
		nBorderJudgeCnt = 0;
	}
}

void algorithm_basic::Step4PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pOriBBMap, uint8_t* pInputBBMap, uint8_t* pOutputBBMap, uint8_t nBBClass)
{
	uint8_t nNormalJudgeCnt = 0;
	uint8_t nBorderJudgeCnt = 0;
	uint8_t bBorderFlag = false;

	UpdateBBMap(pInputBBMap, pOutputBBMap);

	for (uint16_t i = 0; (i) < t_BlockCluster.aTotalNumofScale[nBBClass]; i++)
	{
		//娩矪瞶
		if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] < nImageW ||
			t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] > ((nImageW * nImageH) - nImageW) ||
			(t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW) == 0 ||
			t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW == (nImageW - 1)
			)
		{
			bBorderFlag = true;
			if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] < nImageW) // 娩
			{
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step4, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Top);
			}
			else if (t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] > ((nImageW * nImageH) - nImageW))//娩
			{
				//printf("5Bottom\n");
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step4, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Bottom);
			}
			else if ((t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i] % nImageW) == 0) //オ娩
			{
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step4, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Left);
			}
			else // 娩
			{
				nBorderJudgeCnt = BorderConnectJudge(nImageW, nImageH, pOriBBMap, Step4, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i], Right);
			}
		}
		else
		{
			//Block 8硈硄  耞
			bBorderFlag = false;
			nNormalJudgeCnt = ConnectJudge(nImageW, nImageH, pOriBBMap, Step4, t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]);
		}
		// 耞 BB or GB
		if (nBBClass == Class1)
		{
			if (nNormalJudgeCnt < STEP_FOUR_JUDGE_TH && bBorderFlag == false)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS1_GB_DEPUTY_NUMBER;
				//printf("GB1\n");
			}
			if (nBorderJudgeCnt < (STEP_FOUR_JUDGE_TH) && bBorderFlag == true)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS1_GB_DEPUTY_NUMBER;
				//printf("B_GB1\n");
			}
			bBorderFlag = false;
			nNormalJudgeCnt = 0;
			nBorderJudgeCnt = 0;
		}
		else if (nBBClass == Class5)
		{
			if (nNormalJudgeCnt < STEP_FOUR_JUDGE_TH && bBorderFlag == false)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS5_GB_DEPUTY_NUMBER;
				//printf("GB5\n");
			}
			if (nBorderJudgeCnt < (STEP_FOUR_JUDGE_TH) && bBorderFlag == true)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS5_GB_DEPUTY_NUMBER;
				//printf("B_GB5\n");
			}
			bBorderFlag = false;
			nNormalJudgeCnt = 0;
			nBorderJudgeCnt = 0;
		}
		else if (nBBClass == Class4)
		{
			if (nNormalJudgeCnt < STEP_FOUR_JUDGE_TH && bBorderFlag == false)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS4_GB_DEPUTY_NUMBER;
				//printf("RGB4\n");
			}
			if (nBorderJudgeCnt < (STEP_FOUR_JUDGE_TH) && bBorderFlag == true)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS4_GB_DEPUTY_NUMBER;
				//printf("RB_GB4\n");
			}
			bBorderFlag = false;
			nNormalJudgeCnt = 0;
			nBorderJudgeCnt = 0;
		}
		else if (nBBClass == Class2)
		{
			if (nNormalJudgeCnt < STEP_FOUR_JUDGE_TH && bBorderFlag == false)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS2_GB_DEPUTY_NUMBER;
				//printf("RGB2\n");
			}
			if (nBorderJudgeCnt < (STEP_FOUR_JUDGE_TH) && bBorderFlag == true)
			{
				pOutputBBMap[t_BlockCluster.aBlockCoordinateOfScale[nBBClass][i]] = TH_CLASS2_GB_DEPUTY_NUMBER;
				//printf("RB_GB2\n");
			}
			bBorderFlag = false;
			nNormalJudgeCnt = 0;
			nBorderJudgeCnt = 0;
		}
	}
}

uint16_t algorithm_basic::Step5PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pResult)
{
	uint16_t nSumBB = 0;
	float nPerc = 0;

	for (uint16_t i = 0; (i) < nImageH * nImageW; i++)
	{
		if (pResult[i] == TH_CLASS1_BB_DEPUTY_NUMBER)
		{
			t_ResultCluster.aTotalNumofScale[Class1]++;
		}
		else if (pResult[i] == TH_CLASS2_BB_DEPUTY_NUMBER)
		{
			t_ResultCluster.aTotalNumofScale[Class2]++;
		}
		else if (pResult[i] == TH_CLASS3_BB_DEPUTY_NUMBER)
		{
			t_ResultCluster.aTotalNumofScale[Class3]++;
		}
		else if (pResult[i] == TH_CLASS4_BB_DEPUTY_NUMBER)
		{
			t_ResultCluster.aTotalNumofScale[Class4]++;
		}
		else if (pResult[i] == TH_CLASS5_BB_DEPUTY_NUMBER)
		{
			t_ResultCluster.aTotalNumofScale[Class5]++;
		}
	}

	nSumBB = t_ResultCluster.aTotalNumofScale[Class1] + t_ResultCluster.aTotalNumofScale[Class2] + t_ResultCluster.aTotalNumofScale[Class3]
		+ t_ResultCluster.aTotalNumofScale[Class4] + t_ResultCluster.aTotalNumofScale[Class5];

	nPerc = (float(nSumBB) / (RESULT_ARRAY_SIZE));
	return (nPerc * 100);
}
