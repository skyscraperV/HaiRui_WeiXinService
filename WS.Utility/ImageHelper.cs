using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.Utility
{
    public static class ImageHelper
    {
        public static bool Add_FontMark(string fileName, string saveName, string text, string fontName, int fontSize, Color fontColor, float xPixel, float yPixel)
        {
            //try
            //{
            string path = fileName;
            System.Drawing.Image imgSrc = System.Drawing.Image.FromFile(path);

            using (Graphics g = Graphics.FromImage(imgSrc))
            {
                g.DrawImage(imgSrc, 0, 0, imgSrc.Width, imgSrc.Height);
                using (Font f = new Font(fontName, fontSize))
                {
                    using (Brush b = new SolidBrush(fontColor))
                    {
                        string addText = text;
                        g.DrawString(addText, f, b, xPixel, yPixel);

                        b.Dispose();
                    }
                    f.Dispose();
                }
                g.Dispose();
            }


            imgSrc.Save(saveName, System.Drawing.Imaging.ImageFormat.Jpeg);
            imgSrc.Dispose();
            return true;
            //}
            //catch (Exception)
            //{

            //    return false;
            //}


        }
        public static bool Add_ImageMark(string fileName, string markName, string saveName, int xPixel, int yPixel, int markWidth, int markHeight)
        {

            //try
            //{
            string path = fileName;
            System.Drawing.Image imgSrc = System.Drawing.Image.FromFile(path);
            System.Drawing.Image imgWarter = System.Drawing.Image.FromFile(markName);
            using (Graphics g = Graphics.FromImage(imgSrc))
            {
                //g.DrawImage(imgWarter, new Rectangle(imgSrc.Width - imgWarter.Width,
                //                                 imgSrc.Height - imgWarter.Height,
                //                                 imgWarter.Width,
                //                                 imgWarter.Height),
                //        0, 0, imgWarter.Width, imgWarter.Height, GraphicsUnit.Pixel);
                //g.DrawImage(imgWarter, new Rectangle(310,
                //                               545,
                //                               240,
                //                               240),
                //      0, 0, imgWarter.Width, imgWarter.Height, GraphicsUnit.Pixel);
                g.DrawImage(imgWarter, new Rectangle(xPixel,
                                              yPixel,
                                              markWidth,
                                              markHeight),
                     0, 0, imgWarter.Width, imgWarter.Height, GraphicsUnit.Pixel);


                g.Dispose();
            }

            string newpath = saveName;
            imgSrc.Save(newpath, System.Drawing.Imaging.ImageFormat.Jpeg);
            imgWarter.Dispose();

            imgSrc.Dispose();
            return true;
            //}
            //catch (Exception ex)
            //{

            //    return false;
            //}

        }
    }
}