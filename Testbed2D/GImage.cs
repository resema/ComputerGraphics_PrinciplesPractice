using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media.Imaging;

using System.Windows.Media.Animation;
using System.IO;

namespace GraphicsBook
{
    /// <summary>
    ///     Creates an Image Shape suitable for display in a GraphPaper Canvas
    ///     and based on a BitmapSource as Source. A GImage stores RGBA data
    ///     with 0-255 values. 
    /// </summary>
    public class GImage : Image
    {
        protected static readonly Point initialPosition = new Point(0, 0);
        protected TransformGroup myTransformGroup = new TransformGroup(); 
        protected BitmapSource bmpSource;
        protected double dpi = 96.0d;

        /// <summary>
        ///     Identifies the Position dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position",
                typeof(Point), typeof(GImage),
                new FrameworkPropertyMetadata(initialPosition,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(PositionValueChanged), null));

        /// <summary>
        ///     Gets or sets the Position of the GImage
        /// </summary>
        public Point Position
        {
            set { SetValue(PositionProperty, value); }
            get { return (Point)GetValue(PositionProperty); }
        }

        private static void PositionValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GImage myGImage = (GImage)d;
            //myGImage.setToolTip();
            //Debug.Print("Position Changed");
            myGImage.UpdateRenderTransform();
        }

        /// <summary>
        ///     Read an image from a file that's provided by the user via a file-chooser dialog. 
        /// </summary>
        public GImage()
        {
            BitmapImage myBitmapImage = LoadImage();
            if (myBitmapImage == null)
            {
                throw new Exception();
            }
            bmpSource = convertBitmapImage(myBitmapImage);
            this.Source = bmpSource;
            BuildRenderTransform();
        }

        /// <summary>
        ///     Create an instance of GImage with nRows rows and nCols columns, starting 
        ///     from pixel data of type PixelFormats.Bgra32 and with 
        ///     the pixel data provided in row-major order in the pixelArray. Image
        ///     will be 96 dpi. 
        /// </summary>
        public GImage(int nRows, int nCols, byte[] pixelArray)
        {
            bmpSource = BitmapSource.Create(nCols, nRows, dpi, dpi,
                PixelFormats.Bgra32, null, pixelArray, nCols * 4);
            Source = bmpSource;
            Position = new Point(0, 0);
            BuildRenderTransform();
        }

        
        /// <summary>
        ///     Create an instance of GImage with nRows rows and nCols columns, starting 
        ///     from an byte[nRows, nCols, 4] array of pixel data, where
        ///     pixelArray[r, c, 0] is the red value,
        ///     pixelArray[r, c, 1] is the green value,
        ///     pixelArray[r, c, 2] is the blue value, and
        ///     pixelArray[r, c, 3] is the alpha value (almost always 255). 
        /// </summary>
        public GImage(byte[, ,] pixelArray)
        {
            if (pixelArray.GetLength(2) != 4)
            {
                throw new Exception();
            }
            dpi = 96;
            int nRows = pixelArray.GetLength(0);
            int nCols = pixelArray.GetLength(1);
            int stride = nCols * 4;
            int size = nRows * stride;

            byte[] pixels = new byte[size];

            for (int y = 0; y < nRows; ++y)
            {
                for (int x = 0; x < nCols; ++x)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        pixels[y * stride + x * 4 + i] = pixelArray[y, x, 2-i]; 
                    }
                    pixels[y * stride + x * 4 + 3] = pixelArray[y, x, 3]; 
                }
            }
            bmpSource = BitmapSource.Create(nCols, nRows, dpi, dpi, PixelFormats.Bgra32, null, pixels, stride);
            Source = bmpSource;
            this.Width = bmpSource.Width;
            this.Height = bmpSource.Height;

            Position = new Point(0, 0);
            BuildRenderTransform();
        }



        /// <summary>
        ///     Create an instance of GImage by reading in the image from the named file. 
        ///     If the file is not accessible, raise an Exception. If the "isResource" flag is true, 
        ///     then the filename should be relative to the source directory of the project. Typically
        ///     you might have an "images" folder with an image called image1.jpg in it; in that case, you'd
        ///     use GImage("images/image1.jpg"). [Note: in Visual Studio, the "Build" property of image1.jpg would 
        ///     need to be set to "Resource".]
        ///     
        /// Alternatively, if isResource is false, the filename should be a complete path to the file in
        /// question, something like "\\cifs.cs.brown.edu\dfs\home\jfh\My Documents\My Pictures\bump1.png" 
        /// or "C:\libraries\G3D9\data\image\airplane.png". 
        /// 
        /// </summary>
        public GImage(String filename, bool isResource=true)
        {
            BitmapImage myBitmapImage = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            myBitmapImage.BeginInit();
            String appUri = @"pack://application:,,,/" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ";";
            String path = appUri + "component/" + filename; // images/mona2.jpg";
            myBitmapImage.UriSource = new Uri(path);

            myBitmapImage.EndInit();

            bmpSource = convertBitmapImage(myBitmapImage);
            this.Source = bmpSource;
            this.Width = bmpSource.Width;
            this.Height = bmpSource.Height;
            BuildRenderTransform();

        }

        protected FormatConvertedBitmap convertBitmapImage(BitmapImage myBitmapImage)
        {
            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = myBitmapImage;
            newFormatedBitmapSource.DestinationFormat = PixelFormats.Bgra32;
            newFormatedBitmapSource.EndInit();
            return newFormatedBitmapSource;
        }

        /// <summary>
        ///     Get the array of RGBA pixels that constitute this image,
        ///     organized in a Height x Width x 4 array where each group
        ///     of four contains A, B, G, R values as a byte. 
        /// </summary>
        public byte[, ,] GetPixelArray()
        {
            int stride = bmpSource.PixelWidth * 4;
            int size = bmpSource.PixelHeight * stride;
            byte[] pixels = new byte[size];
            bmpSource.CopyPixels(pixels, stride, 0);
            byte[, ,] pixelArray = new byte[bmpSource.PixelHeight, bmpSource.PixelWidth, 4];

            for (int y = 0; y < bmpSource.PixelHeight; ++y)
            {
                for (int x = 0; x < bmpSource.PixelWidth; ++x)
                {
                    // swap MSFTs BGRA to our RGBA:
                    for (int i = 0; i < 3; i++)
                    {
                        pixelArray[y, x, 2-i] = pixels[y * stride + x * 4 + i];
                    }
                    pixelArray[y, x, 3] = pixels[y * stride + x * 4 + 3];
                }
            }
            return pixelArray;
        }

        public int PixelWidth()
        {
            return bmpSource.PixelWidth;
        }

        public int PixelHeight()
        {
            return bmpSource.PixelHeight;
        }

        /// <summary>
        ///     Replace the image with one whose pixels are given in the 
        ///     nRows x nCols x 4 array of bytes called "pixelArray"; the data at pixels[r, c, *]
        ///     are the R, G, B, and A values for the pixel, stored as values from 0 to 255. 
        /// </summary>
        public void  SetPixelArray(byte[, ,]pixelArray, int nRows, int nCols)
        {
            int stride = nCols * 4;
            int size = nRows * stride;
            byte[] pixels = new byte[size];

            for (int y = 0; y < nRows; ++y)
            {
                for (int x = 0; x < nCols; ++x)
                {
                    // Swap our RGBA format into MSFT BGRA format:
                    for (int i = 0; i < 3; i++)
                    {
                        pixels[y * stride + x * 4 + i] = pixelArray[y, x, 2 - i]; 
                    }
                    pixels[y * stride + x * 4 + 3] = pixelArray[y, x, 3];
                }
            }
            bmpSource = BitmapSource.Create(nCols, nRows, dpi, dpi, PixelFormats.Bgra32, null, pixels, stride);
            Source = bmpSource;
            this.Width = bmpSource.Width;
            this.Height = bmpSource.Height;

        }

        /// <summary>
        /// Build the render transform, which flips the y-coordinate, because the coordinates of a GraphPaper
        /// have y increasing upwards, while those of a Canvas have y increase downwards. WPF's image-loading 
        /// expects the latter, so we have to compensate. 
        /// </summary>
        protected void BuildRenderTransform()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);
            myTransformGroup.Children.Add(new ScaleTransform(1, -1));
            myTransformGroup.Children.Add(new TranslateTransform(Position.X, Position.Y));
            RenderTransform = myTransformGroup;
        }

        /// <summary>
        /// Update the RenderTransform in response to a change in the Position property of the GImage. 
        /// </summary>
        protected void UpdateRenderTransform()
        {
            RenderTransformOrigin = new Point(0.5, 0.5);
            TranslateTransform t = ((TranslateTransform)myTransformGroup.Children[1]);
            t.X = Position.X;
            t.Y = Position.Y;
        }


        // this loads an image from a dialog into TraceImage (which is an Image specified in a .xaml file, but could be any Image).
        protected BitmapImage LoadImage()
        {
            var od = new System.Windows.Forms.OpenFileDialog();
            od.Title = "Image Chooser";
            od.CheckFileExists = true;
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BitmapImage img = Helpers.LoadSerializedImage(Helpers.FileBytes(od.FileName));
                return img;
            }
            return null;
        }

        // this are some helper functions for loading images
        protected static class Helpers
        {
            public static byte[] FileBytes(string file)
            {
                if (file == null || file == "" || !System.IO.File.Exists(file))
                    return null;
                try
                {
                    using (var FileIOStream = new System.IO.FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        System.IO.BinaryReader br = new System.IO.BinaryReader(FileIOStream);
                        long numBytes = new FileInfo(file).Length;
                        var bytes = br.ReadBytes((int)numBytes);
                        return bytes;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public static BitmapImage LoadSerializedImage(byte[] sourceBytes)
            {
                if (sourceBytes == null || sourceBytes.Length == 0)
                    return null;
                var ms = new MemoryStream(sourceBytes);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }
    }
}


