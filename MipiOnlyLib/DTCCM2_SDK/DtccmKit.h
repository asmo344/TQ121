#pragma once

#ifndef DTKIT_EXPORTS
#define DTKIT_EXPORTS extern "C" __declspec(dllimport)
#endif

#include <stdint.h>

namespace dtkit
{
    // 位深度
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

    // BAYER图像排列方式
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
        int area;          // 黑块面积
        RECT bound_rect;   // 包围黑块的外接矩形
        POINT center;      // 完整黑块的中心点
        float sfr[4];      // 黑块“右下左上”的边的SFR统计值
        RECT edge_rect[4]; // 框中四条边的小矩形
    };

    struct EDGE_SFR_RESULT
    {
        int lenght;      // 边的长度
        LINE edge;       // 边的坐标
        POINT center;    // 边的中点
        double sfr[256]; // 各个频率的SFR值。为了避免申请内存的麻烦，这里定义为静态数组
        int sfr_count;   // 以上各个频率的SFR值当中，有效的SFR值个数，不能大于256个。注意：假如有效数据个数为41个，则每个SFR值对应得频率值为1 / 40, 2 / 40, 3 /40 ... 40 / 40
        RECT edge_rect;  // 框中边框的小矩形
    };

    struct IMAGE
    {
        DEPTH depth;           // 图像位深度
        int channels;          // 图像通道数
        SIZE size;             // 图像尺寸
        unsigned char *buffer; // 图像数据
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
        uint32_t fromat;    // 图像格式（暂未使用）
        uint32_t type;      // 图像类型
        uint32_t width;     // 图像宽度
        uint32_t height;    // 图像高度
        uint32_t size;      // 所有ROI区域的数据量
        uint32_t full_size; // 完整图像的数据量
        uint32_t resv[8];
    };

    DTKIT_EXPORTS double timeStamp(); // 时间戳（单位毫秒）

    DTKIT_EXPORTS int initDtKit(int device_id, void *param = nullptr); // 初始化库

    DTKIT_EXPORTS void unInitDtKit(); // 反初始化库

    DTKIT_EXPORTS void bayer2gray(IMAGE src, IMAGE dst, BAYER bayer_type); // 将bayer图像转换成gray灰度图像，也就是间接转换成YUV格式，再提取Y分量

    DTKIT_EXPORTS void bayer2grayRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // 将bayer图像转换成gray灰度图像，也就是间接转换成YUV格式，再提取Y分量

    DTKIT_EXPORTS void bayer2rgb(IMAGE src, IMAGE dst, BAYER bayer_type); // 将bayer图像转换成rgb

    DTKIT_EXPORTS void bayer2rgbRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // 根据ROI区域将bayer图像转换成rgb

    DTKIT_EXPORTS void bayer2green(IMAGE src, IMAGE dst, BAYER bayer_type); // 将bayer图像进行绿色通道插值

    DTKIT_EXPORTS void bayer2greenRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // 根据ROI区域将bayer图像进行绿色通道插值

    DTKIT_EXPORTS void yuv4222rgbRoi(IMAGE_ROI src, IMAGE_ROI dst, YUV422 yuv422_type); // 将YUV422转换位rbg

    DTKIT_EXPORTS void mipi10ToRawRoi(IMAGE_ROI src, IMAGE_ROI dst); // 将mipi10转成raw8或raw16

    DTKIT_EXPORTS void mipi12ToRawRoi(IMAGE_ROI src, IMAGE_ROI dst); // 将mipi12转成raw8或raw16

    DTKIT_EXPORTS void mipi14ToRawRoi(IMAGE_ROI src, IMAGE_ROI dst); // 将mipi14转成raw8或raw16

    DTKIT_EXPORTS void mipi10ToRgbRoi(IMAGE_ROI src, IMAGE_ROI dst, BAYER bayer_type); // 将mipi10直接转成rgb24

    DTKIT_EXPORTS void raw16ToRaw8Roi(IMAGE_ROI src, IMAGE_ROI dst); // 将raw16转成raw8

    DTKIT_EXPORTS void p10ToRaw8Roi(IMAGE_ROI src, IMAGE_ROI dst); // 将p10转成raw8

    DTKIT_EXPORTS void p12ToRaw8Roi(IMAGE_ROI src, IMAGE_ROI dst); // 将p12转成raw8

    DTKIT_EXPORTS void raw8ToP16Roi(IMAGE_ROI src, IMAGE_ROI dst, int shift /* 左移的位数 */); // 将raw8转换为p9 ~ p16

    DTKIT_EXPORTS void saveImage(const char *picture_path, IMAGE image); // 保存为图片文件

    DTKIT_EXPORTS bool saveImageAsync(const char *picture_path, IMAGE image); // 异步图片保存。针对多进程，大文件，并发式图像保存，做了硬盘写入优化

    DTKIT_EXPORTS bool saveTwImage(const char *picture_path, IMAGE_ROI images[], int count); // 多进程保存多个ROI到一个度信定义的dttw的文件

    DTKIT_EXPORTS bool loadTwImageHead(const char *picture_path, DtImageHead *pHead); // 从dttw文件获取文件头信息，可以根据头部信息来申请适当的内存以读出和容纳图像数据

    DTKIT_EXPORTS bool loadTwImage(const char *picture_path, int image_buffer_size, IMAGE *pImage); // 读取一个dttw文件到IMAGE

    DTKIT_EXPORTS void getCacheMemorySize(size_t *pTotal, size_t *pFree, size_t *pQueueCount); // 获取全部缓存量，当前空闲缓存量， 当前缓存队列长度

    DTKIT_EXPORTS int calcSfr(
        IMAGE gray_image,            // 待分析的灰度图像
        RECT search_roi,             // 黑块搜索区域
        SIZE edge_size,              // 框中边缘的矩形大小
        SFR_RESULT *result,          // 每个黑块的SFR分析结果
        int result_count,            // 最多能容纳的，黑块SFR分析结果个数
        float gamma = 1,             // gamma
        float freqency = 0.125,      // 频率
        float mtf = 0.5,             // mtf
        bool result_type = true,     // SFR计算结果为 MTF(true)类型还是 Cycle Pxiel(false)类型
        IMAGE *mark_image = nullptr, // 如果不为空，则在图片上标注黑块和相关信息。此参数未来可能会取消，请慎用
        bool save_bound_rect = false // 是否保存被框中的小黑块的左边缘。此参数未来可能会取消，请慎用
    );

    DTKIT_EXPORTS int calcEdgeSfr(
        IMAGE gray_image,           // 待分析的灰度图像
        RECT search_roi,            // 边缘搜索区域
        SIZE edge_size,             // 框中边缘的矩形大小
        EDGE_SFR_RESULT *result,    // 每条边的SFR分析结果
        int result_count,           // 最多能容纳的，边缘SFR分析结果个数
        IMAGE *mark_image = nullptr // 如果不为空，则在图片上标注黑块和相关信息。此参数未来可能会取消，请慎用
    );
}
