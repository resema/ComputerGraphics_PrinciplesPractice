using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Harness2D
{
    class GPImage : Image
    {
        public Image(System.Uri source)
        {
            myImage.Width = 200/Canvas1.mm(1);
            
            // Create source
            BitmapImage myBitmapImage = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource =  source; //new Uri(@"C:\Documents and Settings\All Users\Documents\My Pictures\Sample Pictures\Water Lilies.jpg");

            // To save significant application memory, set the DecodePixelWidth or  
            // DecodePixelHeight of the BitmapImage value of the image source to the desired 
            // height or width of the rendered image. If you don't do this, the application will 
            // cache the image as though it were rendered as its normal size rather then just 
            // the size that is displayed.
            // Note: In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.
            myBitmapImage.DecodePixelWidth = myBitmapImage.PixelWidth;
            myBitmapImage.EndInit();
            //set image source
            myImage.Source = myBitmapImage;

            Canvas.SetLeft(myImage, 10);
            Canvas.SetBottom(myImage, 20);
            gp.Children.Add(myImage);

    }
}
