using Discord.WebSocket;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace Discord_BotSharp
{
    class MemeGenerator
    {
        private static WebClient getImg;
        private Bitmap meme;
        private readonly string FirstImageURL;
        private readonly string SecondImageURL;
        private readonly string ThirdImageURL;

        // The full directory to the source meme file (e.g. c://haha/meme.jpg)
        private readonly string Source;

        // Just filename of the source meme file (e.g. meme.jpg)
        private readonly string SourceMemeName;

        Image FirstImage;
        Image SecondImage;
        Image ThirdImage;

        private ulong ChannelId;
        private SocketGuild Guild;

        static MemeGenerator()
        {
            getImg = new WebClient();
        }

        public MemeGenerator(string source, ulong channelId, SocketGuild guild, string image1 = null, string image2 = null, string image3 = null)
        {
            Source = source;
            FirstImageURL = image1;
            SecondImageURL = image2;
            ThirdImageURL = image3;

            // These are used to name the images created by this generator for easier organization
            ChannelId = channelId;
            Guild = guild;

            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (source[i] == '/' || source[i] == '\\')
                {
                    SourceMemeName = source.Substring(i + 1);
                    i = -1;
                }
            }

            if (source.Contains("http"))
            {
                Stream sourceImageChecker;
                Bitmap sourceSaveImg;
                sourceImageChecker = getImg.OpenRead(source);
                sourceSaveImg = new Bitmap(sourceImageChecker);
                sourceSaveImg.Save(Environment.CurrentDirectory + "/memeImages/" + ChannelId + " " + SourceMemeName + ".jpg", ImageFormat.Jpeg);
                Source = Environment.CurrentDirectory + "/memeImages/" + ChannelId + " " + SourceMemeName + ".jpg";
            }

            // Code below in this scope sets up all the images given
            meme = new Bitmap(Source);

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
                saveImg.Save(Environment.CurrentDirectory + "/memeImages/" + ChannelId + " img.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                FirstImage = new Bitmap(Environment.CurrentDirectory + "/memeImages/" + ChannelId + " img.jpg");

            }

            if (SecondImageURL != null)
            {
                imgTypeChecker2 = getImg.OpenRead(SecondImageURL);
                saveImg2 = new Bitmap(imgTypeChecker2);
                saveImg2.Save(Environment.CurrentDirectory + "/memeImages/" + ChannelId + " img2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                SecondImage = new Bitmap(Environment.CurrentDirectory + "/memeImages/" + ChannelId + " img2.jpg");

            }

            if (ThirdImageURL != null)
            {
                imgTypeChecker3 = getImg.OpenRead(ThirdImageURL);
                saveImg3 = new Bitmap(imgTypeChecker3);
                saveImg3.Save(Environment.CurrentDirectory + "/memeImages/" + ChannelId + " img3.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                ThirdImage = new Bitmap(Environment.CurrentDirectory + "/memeImages/" + ChannelId + " img3.jpg");
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

                memeG.DrawImage(FirstImage, whereToPlacex, whereToPlacey);
                memeG.DrawImage(SecondImage, whereToPlacexx, whereToPlaceyy);

                meme.Save(Environment.CurrentDirectory + "/memeImages/" + Guild + " - " + SourceMemeName);

                return Environment.CurrentDirectory + "/memeImages/" + Guild + " - " + SourceMemeName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }

        }

        public string Generate3ImageMeme
        (
           int resizex,         int resizey,
           int whereToPlacex,   int whereToPlacey,
           int resizexx,        int resizeyy,
           int whereToPlacexx,  int whereToPlaceyy,
           int resizexxx,       int resizeyyy,
           int whereToPlacexxx, int whereToPlaceyyy
        )
        {
            try
            {
                Graphics memeG = Graphics.FromImage(meme);

                FirstImage = ResizeImage(FirstImage, resizex, resizey);
                SecondImage = ResizeImage(SecondImage, resizexx, resizeyy);
                ThirdImage = ResizeImage(ThirdImage, resizexxx, resizeyyy);

                memeG.DrawImage(FirstImage, whereToPlacex, whereToPlacey);
                memeG.DrawImage(SecondImage, whereToPlacexx, whereToPlaceyy);
                memeG.DrawImage(ThirdImage, whereToPlacexxx, whereToPlaceyyy);
                Console.WriteLine(Environment.CurrentDirectory + "/memeImages/" + Guild + " - " + SourceMemeName);
                meme.Save(Environment.CurrentDirectory + "/memeImages/" + Guild + " - " + SourceMemeName);

                return Environment.CurrentDirectory + "/memeImages/" + Guild + " - " + SourceMemeName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}
