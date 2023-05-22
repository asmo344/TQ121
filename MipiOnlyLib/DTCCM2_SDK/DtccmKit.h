#pragma once

#ifndef DTKIT_EXPORTS
#define DTKIT_EXPORTS extern "C" __declspec(dllimport)
#endif

#include <stdint.h>

namespace dtkit
{
    // λ���
    enum DEPTH
    {
        DEPTH_8U = 0,
        DEPTH_8S = 1,
        DEPTH_16U = 2,
        DEPTH_16S = 3,
        DEPTH_32S = 4,
        DEPTH_32F = 5,
        DEPTH_64F = 6,
    };

    // BAYERͼ�����з�ʽ
    enum BAYER
    {
        BAYER_BG,
        BAYER_GB,
        BAYER_RG,
        BAYER_GR,
    };

    enum YUV422
    {
        YUV_CBYCRY,
        YUV_CRYCBY,
        YUV_YCBYCR,
        YUV_YCRYCB,
    };

    struct SIZE
    {
        int width;
        int height;
    };

    struct POINT
    {
        int x;
        int y;
    };

    struct LINE
    {
        POINT p1;
        POINT p2;
    };

    struct RECT
    {
        POINT top_left;
        SIZE size;
    };

    struct SFR_RESULT
    {
        int area;          // �ڿ����
        RECT bound_rect;   // ��Χ�ڿ����Ӿ���
        POINT center;      // �����ڿ�����ĵ�
        float sfr[4];      // �ڿ顰�������ϡ��ıߵ�SFRͳ��ֵ
        RECT edge_rect[4]; // ���������ߵ�С����
    };

    struct EDGE_SFR_RESULT
    {
        int lenght;      // �ߵĳ���
        LINE edge;       // �ߵ�����
        POINT center;    // �ߵ��е�
        double sfr[256]; // ����Ƶ�ʵ�SFRֵ��Ϊ�˱��������ڴ���鷳�����ﶨ��Ϊ��̬����
        int sfr_count;   // ���ϸ���Ƶ�ʵ�SFRֵ���У���Ч��SFRֵ���������ܴ���256����ע�⣺������Ч���ݸ���Ϊ41������ÿ��SFRֵ��Ӧ��Ƶ��ֵΪ1 / 40, 2 / 40, 3 /40 ... 40 / 40
        RECT edge_rect;  // ���б߿��С����
    };

    struct IMAGE
    {
        DEPTH depth;           // ͼ��λ���
        int channels;          // ͼ��ͨ����
        SIZE size;             // ͼ��ߴ�
        unsigned char *buffer; // ͼ������
    };

    struct IMAGE_ROI : IMAGE
    {
        RECT roi;
    };

    struct DtImageHead
    {
        DtImageHead()
        {
            magic[0] = 'd';
            magic[1] = 't';
            magic[2] = 't';
            magic[3] = 'w';
            head_size = sizeof(DtImageHead);
        }

        uint8_t magic[4];
        uint32_t head_size;
        uint32_t fromat;    // ͼ���ʽ����δʹ�ã�
        uint32_t type;      // ͼ������
        uint32_t width;     // ͼ����
        uint32_t height;    // ͼ��߶�
        uint32_t size;      // ����ROI�����������
        uint32_t full_size; // ����ͼ���������
        uint32_t resv[8];
    };

    DTKIT_EXPORTS double timeStamp(); // ʱ�������λ���룩

    DTKIT_EXPORTS int initDtKit(int device_id, void *param = nullptr); // ��ʼ����

    DTKIT_EXPORTS void unInitDtKit(); // ����ʼ����

    DTKIT_EXPORTS void bayer2gray(IMAGE src, IMAGE dst, BAYER bayer_type); // ��bayerͼ��ת����gray�Ҷ�ͼ��Ҳ���Ǽ��ת����YUV��ʽ������ȡY����

    DTKIT_EXPORTS void bayer2grayRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // ��bayerͼ��ת����gray�Ҷ�ͼ��Ҳ���Ǽ��ת����YUV��ʽ������ȡY����

    DTKIT_EXPORTS void bayer2rgb(IMAGE src, IMAGE dst, BAYER bayer_type); // ��bayerͼ��ת����rgb

    DTKIT_EXPORTS void bayer2rgbRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // ����ROI����bayerͼ��ת����rgb

    DTKIT_EXPORTS void bayer2green(IMAGE src, IMAGE dst, BAYER bayer_type); // ��bayerͼ�������ɫͨ����ֵ

    DTKIT_EXPORTS void bayer2greenRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // ����ROI����bayerͼ�������ɫͨ����ֵ

    DTKIT_EXPORTS void yuv4222rgbRoi(IMAGE_ROI src, IMAGE_ROI dst, YUV422 yuv422_type); // ��YUV422ת��λrbg

    DTKIT_EXPORTS void mipi10ToRawRoi(IMAGE_ROI src, IMAGE_ROI dst); // ��mipi10ת��raw8��raw16

    DTKIT_EXPORTS void mipi12ToRawRoi(IMAGE_ROI src, IMAGE_ROI dst); // ��mipi12ת��raw8��raw16

    DTKIT_EXPORTS void mipi14ToRawRoi(IMAGE_ROI src, IMAGE_ROI dst); // ��mipi14ת��raw8��raw16

    DTKIT_EXPORTS void mipi10ToRgbRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // ��mipi10ֱ��ת��rgb24

    DTKIT_EXPORTS void raw16ToRaw8Roi(IMAGE_ROI src, IMAGE_ROI dst); // ��raw16ת��raw8

    DTKIT_EXPORTS void p10ToRaw8Roi(IMAGE_ROI src, IMAGE_ROI dst); // ��p10ת��raw8

    DTKIT_EXPORTS void p12ToRaw8Roi(IMAGE_ROI src, IMAGE_ROI dst); // ��p12ת��raw8

    DTKIT_EXPORTS void raw8ToP16Roi(IMAGE_ROI src, IMAGE_ROI dst, int shift /* ���Ƶ�λ�� */); // ��raw8ת��Ϊp9 ~ p16

    DTKIT_EXPORTS void saveImage(const char *picture_path, IMAGE image); // ����ΪͼƬ�ļ�

    DTKIT_EXPORTS bool saveImageAsync(const char *picture_path, IMAGE image); // �첽ͼƬ���档��Զ���̣����ļ�������ʽͼ�񱣴棬����Ӳ��д���Ż�

    DTKIT_EXPORTS bool saveTwImage(const char *picture_path, IMAGE_ROI images[], int count); // ����̱�����ROI��һ�����Ŷ����dttw���ļ�

    DTKIT_EXPORTS bool loadTwImageHead(const char *picture_path, DtImageHead *pHead); // ��dttw�ļ���ȡ�ļ�ͷ��Ϣ�����Ը���ͷ����Ϣ�������ʵ����ڴ��Զ���������ͼ������

    DTKIT_EXPORTS bool loadTwImage(const char *picture_path, int image_buffer_size, IMAGE *pImage); // ��ȡһ��dttw�ļ���IMAGE

    DTKIT_EXPORTS void getCacheMemorySize(size_t *pTotal, size_t *pFree, size_t *pQueueCount); // ��ȡȫ������������ǰ���л������� ��ǰ������г���

    DTKIT_EXPORTS int calcSfr(
        IMAGE gray_image,            // �������ĻҶ�ͼ��
        RECT search_roi,             // �ڿ���������
        SIZE edge_size,              // ���б�Ե�ľ��δ�С
        SFR_RESULT *result,          // ÿ���ڿ��SFR�������
        int result_count,            // ��������ɵģ��ڿ�SFR�����������
        float gamma = 1,             // gamma
        float freqency = 0.125,      // Ƶ��
        float mtf = 0.5,             // mtf
        bool result_type = true,     // SFR������Ϊ MTF(true)���ͻ��� Cycle Pxiel(false)����
        IMAGE *mark_image = nullptr, // �����Ϊ�գ�����ͼƬ�ϱ�ע�ڿ�������Ϣ���˲���δ�����ܻ�ȡ����������
        bool save_bound_rect = false // �Ƿ񱣴汻���е�С�ڿ�����Ե���˲���δ�����ܻ�ȡ����������
    );

    DTKIT_EXPORTS int calcEdgeSfr(
        IMAGE gray_image,           // �������ĻҶ�ͼ��
        RECT search_roi,            // ��Ե��������
        SIZE edge_size,             // ���б�Ե�ľ��δ�С
        EDGE_SFR_RESULT *result,    // ÿ���ߵ�SFR�������
        int result_count,           // ��������ɵģ���ԵSFR�����������
        IMAGE *mark_image = nullptr // �����Ϊ�գ�����ͼƬ�ϱ�ע�ڿ�������Ϣ���˲���δ�����ܻ�ȡ����������
    );
}
