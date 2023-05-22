using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tyrafos;

namespace PG_UI2
{
    public class RegionOfInterest : IDisposable
    {
        //private bool gDrawAutoSize = false;
        //private Size gOriginalFrameSize = new Size();
        //private PictureBox gPictureBox = null;
        private List<(PictureBox PicBox, bool DrawAutoSize, Size OriginalFrameSize)> gDrawList = new List<(PictureBox PicBox, bool DrawAutoSize, Size OriginalFrameSize)>();

        public RegionOfInterest()
        {
            RatioOfPaintOnPictureBox = 1.0;
            StepX = 1;
            StepY = 1;
        }

        public RegionOfInterest(Rectangle roiRect, int stepX = 1, int stepY = 1) : this()
        {
            Rectangle = roiRect;
            StepX = stepX;
            StepY = stepY;
        }

        public double RatioOfPaintOnPictureBox { get; set; }

        public Rectangle Rectangle { get; set; }

        public int StepX { get; set; }

        public int StepY { get; set; }

        public void AddPictureBox(PictureBox pictureBox, bool drawAutoSize = false, Size originFrameSize = new Size())
        {
            gDrawList.Add((pictureBox, drawAutoSize, originFrameSize));
            gDrawList[gDrawList.Count - 1].PicBox.Paint += PictureBox_Paint;
            if (gDrawList[gDrawList.Count - 1].PicBox.InvokeRequired)
            {
                gDrawList[gDrawList.Count - 1].PicBox.BeginInvoke(new Action(() =>
                {
                    gDrawList[gDrawList.Count - 1].PicBox.Refresh();
                }));
            }
            else
                gDrawList[gDrawList.Count - 1].PicBox.Refresh();
        }

        public void ClearPictureBox()
        {
            for (int i = 0; i < gDrawList.Count; i++)
            {
                gDrawList[i].PicBox.Paint -= PictureBox_Paint;
            }
            gDrawList.Clear();
        }

        public void Dispose()
        {
            ClearPictureBox();
        }

        public Frame<T> GetRoiFrame<T>(Frame<T> source) where T : struct
        {
            var sourcePixels = source.Pixels;
            var roiSize = Rectangle.Size;
            var roiPosition = Rectangle.Location;
            var stepX = StepX;
            var stepY = StepY;

            if (source.Size == roiSize && stepX == 1 && stepY == 1)
                return source;

            var buf_width = ((roiSize.Width - 1) / stepX) + 1;
            var bufLengthY = ((roiSize.Height - 1) / stepY) + 1;

            var roiPixels = new T[buf_width * bufLengthY];
            for (var jj = 0; jj < bufLengthY; jj++)
            {
                for (var ii = 0; ii < buf_width; ii++)
                {
                    var roiP = jj * buf_width + ii;
                    var sourceP = (roiPosition.Y + (jj * stepY)) * source.Width + roiPosition.X + (ii * stepX);
                    roiPixels[roiP] = sourcePixels[sourceP];
                }
            }
            var metaData = source.MetaData;
            metaData.FrameSize = new Size(buf_width, bufLengthY);
            return new Frame<T>(roiPixels, metaData, source.Pattern);
        }

        public Frame<T>[] GetRoiFrames<T>(Frame<T>[] sources) where T : struct
        {
            var frames = new Frame<T>[sources.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = GetRoiFrame(sources[i]);
            }
            return frames;
        }

        public void RemovePictureBox(PictureBox pictureBox)
        {
            gDrawList.RemoveAll(x =>
            {
                if (x.PicBox == pictureBox)
                {
                    x.PicBox.Paint -= PictureBox_Paint;
                    return true;
                }
                else
                    return false;
            });
        }

        public void RemovePictureBoxAt(int index)
        {
            gDrawList[index].PicBox.Paint -= PictureBox_Paint;
            gDrawList.RemoveAt(index);
        }

        public int GetPictureBoxCount()
        {
            return gDrawList.Count;
        }

        public override string ToString()
        {
            return $"X={Rectangle.X}, Width={Rectangle.Width}, StepX={StepX}; Y={Rectangle.Y}, Height={Rectangle.Height}, StepY={StepY}";
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Red, 1))
            {
                var pic = (PictureBox)sender;
                var drawAutoSize = gDrawList.Find(n => n.PicBox == pic).DrawAutoSize;
                var originalFrameSize = gDrawList.Find(n => n.PicBox == pic).OriginalFrameSize;

                var x = (int)(Rectangle.X * RatioOfPaintOnPictureBox);
                var y = (int)(Rectangle.Y * RatioOfPaintOnPictureBox);
                var width = (int)(Rectangle.Width * RatioOfPaintOnPictureBox);
                var heigth = (int)(Rectangle.Height * RatioOfPaintOnPictureBox);

                if (drawAutoSize)
                {
                    x = (int)(Rectangle.X);
                    y = (int)(Rectangle.Y);
                    width = (int)(Rectangle.Width);
                    heigth = (int)(Rectangle.Height);
                }

                if (drawAutoSize && !originalFrameSize.IsEmpty)
                {
                    if (pic.Width < originalFrameSize.Width || pic.Height < originalFrameSize.Height)
                    {
                        var n1 = Math.Floor((float)originalFrameSize.Width / pic.Width);
                        var n2 = Math.Floor((float)originalFrameSize.Height / pic.Height);
                        var n = Math.Max(n1, n2);
                        if (n < 1) n = 1;
                        x = (int)((float)x / n);
                        y = (int)((float)y / n);
                        width = (int)((float)width / n);
                        heigth = (int)((float)heigth / n);
                    }
                }
                Rectangle rect = new Rectangle(x, y, width, heigth);
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
    }
}
