using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TestTwinCoreProject.Utility
{
    public class CaptchaImage
    {
        private string text; 
        private int width; 
        private int height; 
        public Bitmap Image { get; set; } 

        public CaptchaImage(string s, int width, int height)
        {
            text = s;
            this.width = width;
            this.height = height;
            GenerateImage();
        }
        // создаем изображение
        private void GenerateImage()
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Random random = new Random();
            using Graphics g = Graphics.FromImage(bitmap);
            // отрисовка строки
            g.DrawString(text, new Font("Akaya Kanadaka", height / 3, FontStyle.Italic),
                                Brushes.LightGray, new RectangleF(0, 0, width, height));
            for (int i = 0; i < 5; i++)
                g.DrawLine(new Pen(Brushes.Cyan,random.Next(1,5)), 0, random.Next(0, height / 2), width - random.Next(0, width), height - random.Next(0, height));

            g.Dispose();

            Image = bitmap;
        }

    }
}
