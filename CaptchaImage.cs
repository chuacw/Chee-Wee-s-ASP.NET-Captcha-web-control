using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

// by Chee Wee Chua,
// Singapore, Apr 2025

namespace chuacw.TelligentCommunity
{
    internal class CaptchaImage : IDisposable
    {
        private Bitmap _Image;

        private Color BackColor;

        private Color BackNoiseColor;

        private string FontName;

        private Color ForeColor;

        private Color ForeNoiseColor;

        private int Height;

        private Random Random = new Random();

        private string Text;

        private int Width;

        public Bitmap Image
        {
            get
            {
                return this._Image;
            }
        }

        public CaptchaImage(string text, int width, int height, string fontName, Color foreColor, Color foreNoiseColor, Color backColor, Color backNoiseColor)
        {
            this.Text = text;
            this.ForeColor = foreColor;
            this.ForeNoiseColor = foreNoiseColor;
            this.BackColor = backColor;
            this.BackNoiseColor = backNoiseColor;
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException("width", (object)width, "Argument out of range, must be greater than zero.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException("height", (object)height, "Argument out of range, must be greater than zero.");
            }
            this.Width = width;
            this.Height = height;
            try
            {
                Font font = new Font(fontName, 12f);
                this.FontName = fontName;
                font.Dispose();
            }
            catch
            {
                this.FontName = FontFamily.GenericSerif.Name;
            }
            this.GenerateImage();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._Image.Dispose();
            }
        }

        ~CaptchaImage()
        {
            this.Dispose(false);
        }

        private void GenerateImage()
        {
            Font font;
            Bitmap bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            Graphics graphic = Graphics.FromImage(bitmap);
            Rectangle rectangle = new Rectangle(0, 0, this.Width, this.Height);
            GraphicsPath[] graphicsPath = new GraphicsPath[this.Text.Length];
            RectangleF[] bounds = new RectangleF[this.Text.Length];
            float single = 4.25f;
            Matrix matrix = new Matrix();
            int num = 0;
            float height = (float)(rectangle.Height + 1);
            StringFormat stringFormat = new StringFormat();
            graphic.SmoothingMode = SmoothingMode.AntiAlias;
            do
            {
                height -= 1f;
                font = new Font(this.FontName, height, FontStyle.Bold);
            }
            while (graphic.MeasureString(this.Text, font).Width > (float)rectangle.Width);
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, this.BackNoiseColor, this.BackColor);
            graphic.FillRectangle(hatchBrush, rectangle);
            for (int i = 0; i < this.Text.Length; i++)
            {
                graphicsPath[i] = new GraphicsPath();
                GraphicsPath graphicsPath1 = graphicsPath[i];
                char text = this.Text[i];
                graphicsPath1.AddString(text.ToString(), font.FontFamily, (int)font.Style, font.Size, rectangle, stringFormat);
                PointF[] pointF = new PointF[] { new PointF((float)this.Random.Next(rectangle.Width) / single, (float)this.Random.Next(rectangle.Height) / single), new PointF((float)rectangle.Width - (float)this.Random.Next(rectangle.Width) / single, (float)this.Random.Next(rectangle.Height) / single), new PointF((float)this.Random.Next(rectangle.Width) / single, (float)rectangle.Height - (float)this.Random.Next(rectangle.Height) / single), new PointF((float)rectangle.Width - (float)this.Random.Next(rectangle.Width) / single, (float)rectangle.Height - (float)this.Random.Next(rectangle.Height) / single) };
                PointF[] pointFArray = pointF;
                graphicsPath[i].Warp(pointFArray, rectangle, null, WarpMode.Perspective, 0f);
                bounds[i] = graphicsPath[i].GetBounds();
                bounds[i].Width = (float)Math.Round((double)bounds[i].Width);
            }
            hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, this.ForeNoiseColor, this.ForeColor);
            for (int j = 0; j < (int)graphicsPath.Length; j++)
            {
                int width = 0;
                if (j + 1 < (int)graphicsPath.Length)
                {
                    for (int k = j + 1; k < (int)graphicsPath.Length; k++)
                    {
                        width += (int)bounds[k].Width;
                    }
                }
                int width1 = rectangle.Width - width - num;
                if (bounds[j].Width >= (float)width1)
                {
                    matrix.Translate((float)num - bounds[j].Left, 0f);
                }
                else
                {
                    matrix.Translate((float)this.Random.Next(num, num + width1 - (int)bounds[j].Width) - bounds[j].Left, 0f);
                }
                graphicsPath[j].Transform(matrix);
                graphic.FillPath(hatchBrush, graphicsPath[j]);
                RectangleF rectangleF = graphicsPath[j].GetBounds();
                num = (int)(Math.Round((double)rectangleF.Right) - (double)bounds[j].Width * 0.1);
                matrix.Reset();
            }
            int num1 = (int)((float)(rectangle.Width * rectangle.Height) / 40f);
            for (int l = 0; l < num1; l++)
            {
                int num2 = this.Random.Next(rectangle.Width);
                int num3 = this.Random.Next(rectangle.Height);
                int num4 = this.Random.Next(4);
                int num5 = this.Random.Next(4);
                graphic.FillEllipse(hatchBrush, num2, num3, num4, num5);
            }
            font.Dispose();
            hatchBrush.Dispose();
            graphic.Dispose();
            this._Image = bitmap;
        }
    }
}