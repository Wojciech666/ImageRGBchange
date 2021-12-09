using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageRGBchange
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap resultImageGlobal;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb((byte)slColorR.Value, (byte)slColorG.Value, (byte)slColorB.Value);
            BitmapImage bitmap = new BitmapImage();
            bitmap = BitmapToImageSource(ChangeColor(resultImageGlobal, color));
            ImageViewer.Source = bitmap;
        }

        private void ImageBrowse_Click(object sender, RoutedEventArgs e)
        {
            string imagePath = filePath.Text;
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
            Bitmap resultImage = Resize(image, 400, 400);
            resultImageGlobal = resultImage;
            BitmapImage bitmap = new BitmapImage();
            bitmap = BitmapToImageSource(resultImage);
            ImageViewer.Source = bitmap;
        }

        public static Bitmap Resize(System.Drawing.Image image, int width, int height)
        {

            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
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

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static Bitmap ChangeColor(Bitmap scrBitmap, System.Drawing.Color color )
        {
            System.Drawing.Color newColor = color;
            System.Drawing.Color actualColor;            
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < scrBitmap.Width; i++)
            {
                for (int j = 0; j < scrBitmap.Height; j++)
                {
                    actualColor = scrBitmap.GetPixel(i, j);
                    newBitmap.SetPixel(i, j, ChannelSum(actualColor,color));
                }
            }
            return newBitmap;
        }

        public static System.Drawing.Color ChannelSum(System.Drawing.Color actualColor, System.Drawing.Color color) 
        {
           return System.Drawing.Color.FromArgb(Sum(color.R,actualColor.R), Sum(color.G, actualColor.G), Sum(color.B, actualColor.B));
        }

        public static byte Sum(byte a, byte b)
        {
            return (byte)(a + b);
        }
    }
}
