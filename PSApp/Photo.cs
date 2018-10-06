using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;


namespace GraphicsBook
{
    /// <summary>
    /// A class representing a photo, constructed by reading it from a file whose path is known.
    /// </summary>

    public class Photo : Image
    {
        /// <summary>
        /// Read the photo in the file referenced by the filePath into a BitmapImage, which can 
        /// then be displayed. 
        /// </summary>
        public Photo(String filePath)
        {
            BitmapImage myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            myBitmapImage.EndInit();
            this.Source = myBitmapImage;
            this.Width = Source.Width;
        }
    }
}
