﻿using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace CritzlezOS
{
    class MemeGenerator
    {
        private static WebClient getImg;
        private Bitmap meme;
        private readonly string FirstImageURL;
        private readonly string SecondImageURL;
        private readonly string ThirdImageURL;

        // The template meme filename (e.g. memeImages/templates/meme.jpg)
        private readonly string Template;

        Image FirstImage;
        Image SecondImage;
        Image ThirdImage;

        private ulong MessageId;

        static MemeGenerator()
        {
            getImg = new WebClient();
        }

        // All templates must be in memeImages/templates/
        public MemeGenerator(string template, SocketCommandContext Context, string image1 = null, string image2 = null, string image3 = null)
        {
            Template = "memeImages/templates/" + template;
            FirstImageURL = image1;
            SecondImageURL = image2;
            ThirdImageURL = image3;

            // These are used to name the images created by this generator for easier organization
            MessageId = Context.Message.Id;

            // Code below in this scope sets up all the images given
            meme = new Bitmap(Template);

            Stream imgTypeChecker;
            Stream imgTypeChecker2;
            Stream imgTypeChecker3;

            Bitmap saveImg;
            Bitmap saveImg2;
            Bitmap saveImg3;

            // These 3 if statements basically downloads the images
            if (FirstImageURL != null)
            {
                imgTypeChecker = getImg.OpenRead(FirstImageURL);
                saveImg = new Bitmap(imgTypeChecker);
                saveImg.Save( "memeImages/" + MessageId + " img.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                FirstImage = new Bitmap("memeImages/" + MessageId + " img.jpg");

            }

            if (SecondImageURL != null)
            {
                imgTypeChecker2 = getImg.OpenRead(SecondImageURL);
                saveImg2 = new Bitmap(imgTypeChecker2);
                saveImg2.Save("memeImages/" + MessageId + " img2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                SecondImage = new Bitmap("memeImages/" + MessageId + " img2.jpg");

            }

            if (ThirdImageURL != null)
            {
                imgTypeChecker3 = getImg.OpenRead(ThirdImageURL);
                saveImg3 = new Bitmap(imgTypeChecker3);
                saveImg3.Save("memeImages/" + MessageId + " img3.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                ThirdImage = new Bitmap("memeImages/" + MessageId + " img3.jpg");
            }

            // These saves the bitmap from the downloaded images
           

        }



        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        // This is a basic method to place two images on top of any meme template. Requires x,y coordinates for both images. (x,y) for 1st img, (xx,yy) for 2nd.
        // The origin is on the top left. Y increases towards the bottom. X increases towards the right.
        public string Generate2ImageMeme
        (
            int resizex,        int resizey, 
            int whereToPlacex,  int whereToPlacey,
            int resizexx,       int resizeyy,
            int whereToPlacexx, int whereToPlaceyy 
        )
        {
            try
            {
                Graphics memeG = Graphics.FromImage(meme);

                FirstImage = ResizeImage(FirstImage, resizex, resizey);
                SecondImage = ResizeImage(SecondImage, resizexx, resizeyy);

                // Places the origin of the images onto the given coordinates
                memeG.DrawImage(FirstImage, whereToPlacex, whereToPlacey);
                memeG.DrawImage(SecondImage, whereToPlacexx, whereToPlaceyy);

                meme.Save("memeImages/" + MessageId + ".jpg");

                return "memeImages/" + MessageId + ".jpg";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }

        }
    }
}
