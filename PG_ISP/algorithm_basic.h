#pragma once
#include <cstdint>

#ifndef ALHORITHM_BASIC_H
#define ALHORITHM_BASIC_H

#define PICK_W_SIZE (8)

#define MAX_ONCE_BLOCK_NUMBER (500)
#define TH_SCALE (5+5)
#define STEP_TWO_JUDGE_TH (6)
#define STEP_THREE_JUDGE_TH (4)
#define STEP_FOUR_JUDGE_TH (1)

#define TH_CLASS1_GB_DEPUTY_NUMBER (175)
#define TH_CLASS2_GB_DEPUTY_NUMBER (200)
#define TH_CLASS3_GB_DEPUTY_NUMBER (225)
#define TH_CLASS4_GB_DEPUTY_NUMBER (250)
#define TH_CLASS5_GB_DEPUTY_NUMBER (254)

#define TH_CLASS1_BB_DEPUTY_NUMBER (217)
#define TH_CLASS2_BB_DEPUTY_NUMBER (204)
#define TH_CLASS3_BB_DEPUTY_NUMBER (51)
#define TH_CLASS4_BB_DEPUTY_NUMBER (38)
#define TH_CLASS5_BB_DEPUTY_NUMBER (10)

#define MAX_IMAGE_NUMBER (40)//48
#define MASK_DEBUG (0)

#define JUDGE_MASK_SIZE (15) //15
#define MASK_TH (100*JUDGE_MASK_SIZE*JUDGE_MASK_SIZE) //90

#define REMOVE_EDGE_MASK_SIZE (5)
#define REMOVE_EDGE_MASK_TH (90*REMOVE_EDGE_MASK_SIZE*REMOVE_EDGE_MASK_SIZE)
#define OFFSET_PIXEL_PERCENTAGE (6)

#define PIXEL_VALUE_TH  (90) //90

typedef enum
{
	Class1 = 0,
	Class2 = 1,
	Class3 = 2,
	Class4 = 3,
	Class5 = 4,
	Class6 = 5,
	Class7 = 6,
	Class8 = 7,
}e_BBMapOfClass;

typedef enum
{
	Step1 = 0,
	Step2,
	Step3,
	Step4,
	Step5,
	Step6,
	Step7,
}e_AlgoStep;

typedef enum
{
	Top = 0,
	Bottom,
	Left,
	Right
}e_BoderArea;

typedef struct
{
	uint8_t aTotalNumofScale[TH_SCALE];
	uint8_t aBlockCoordinateOfScale[TH_SCALE][MAX_ONCE_BLOCK_NUMBER];
	uint8_t* aBB1Map;
	uint8_t* aBB2Map;
	uint8_t* aBB3Map;
	uint8_t* aBB4Map;
	uint8_t* aBB5Map;
	uint8_t* aBB6Map;
	uint8_t* aBB7Map;
	uint8_t* aBB8Map;
	uint8_t* aBB9Map;
	uint8_t* aBB10Map;
	uint8_t* aBB11Map;
}t_PixelofCluster;

static t_PixelofCluster t_BlockCluster;
static t_PixelofCluster t_ResultCluster;

ref class algorithm_basic
{
public:
	static uint16_t InitialResultArraySize(uint16_t nImageW, uint16_t nImageH);
	static void Step1PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pResult);
	static void Step2PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pOriBBMap, uint8_t* pInputBBMap, uint8_t* pOutputBBMap, uint8_t nBBClass);
	static void Step3PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pOriBBMap, uint8_t* pInputBBMap, uint8_t* pOutputBBMap, uint8_t nBBClass);
	static void Step4PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pOriBBMap, uint8_t* pInputBBMap, uint8_t* pOutputBBMap, uint8_t nBBClass);
	static uint16_t Step5PixelOfCluster(uint16_t nImageW, uint16_t nImageH, uint8_t* pResult);
private:
};
#endif