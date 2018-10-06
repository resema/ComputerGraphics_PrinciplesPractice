using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Controls;
using System.Diagnostics;
using System.Text;

using System.Windows.Data;
using System.Windows.Documents;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

namespace GraphicsBook
{
    /// <summary>
    ///     Draws a piece of text at the specified position.
    /// The position is the center of the text box; default is
    /// the origin. The Text object is generally scaled by -1 in y, because on a GraphPaper, the y-axis points up,
    /// while in WPF, it points down; the -1 scale is required to compensate for the corresponding -1 scale
    /// in the GraphPaper object. But if a Text object is to be used in a GraphPaperAlt, where the y-axis points
    /// down, the scale factor is eliminated.  
    /// </summary>
    public class Text : TextBlock, INotifyPropertyChanged
    {
        protected const double initialStrokeThickness = 0.6;
        protected static readonly  SolidColorBrush initialColor = Brushes.Black;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.Black;
        protected static readonly Point initialPosition = new Point(0, 0);
        protected bool yUp = true; // set to false when the y-coordinate increases downwards. 
 
        /// <summary>
        ///     Identifies the Position dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position",
                typeof(Point), typeof(Text),
                new FrameworkPropertyMetadata(initialPosition,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(PositionValueChanged), null));


        /// <summary>
        ///     Gets or sets the Position of the Text
        /// </summary>
        public Point Position
        {
            set {
                SetValue(PositionProperty, value);
                //Debug.Print("Position change for text " + Position);
                ((TransformGroup)RenderTransform).Children[1] = new TranslateTransform(Position.X, Position.Y);
            }
            get { return (Point)GetValue(PositionProperty); }
        }

        private static void PositionValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Text myText = (Text)d;
            //Debug.Print("Position Changed");
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }



        /// <summary>
        ///     Initializes a new instance of Text. If yUp is set to false, the text will appear upside
        ///     down on a GraphPaper, but right-side up on a GraphPaperAlt
        /// </summary>
        public Text(bool yUp = true)
        {
            constructBasics(yUp);
            Inlines.Add(new Bold(new Run("Text")));
        }
        /// <summary>
        ///     Initializes a new instance of Text with the given string
        ///     If yUp is set to false, the text will appear upside
        ///     down on a GraphPaper, but right-side up on a GraphPaperAlt
        /// </summary>
        public Text(String message, bool yUp = true)
        {
            constructBasics(yUp);
            Inlines.Add(new Bold(new Run(message)));
        }

        /// <summary>
        ///     Initializes a new instance of Text with the given string
        /// at the specified location (for the center of the text-box).
        ///     If yUp is set to false, the text will appear upside
        ///     down on a GraphPaper, but right-side up on a GraphPaperAlt.
        /// </summary>
        public Text(String message, Point location, bool yUp = true)
        {
            constructBasics(yUp);
            Inlines.Add(new Bold(new Run(message)));
            Position = location;
        }

        /// <summary>
        ///     Construct all the  parts of a Text
        /// </summary>
        private void constructBasics(bool yUp)
        {
            FontSize = 3;
            TextWrapping = TextWrapping.Wrap;
            Background = Brushes.AntiqueWhite;
            TextAlignment = TextAlignment.Center;
            TransformGroup tg = new TransformGroup();
            if (yUp)
                tg.Children.Add(new ScaleTransform(1, -1));
            else
                tg.Children.Add(new ScaleTransform(1,  1));
            tg.Children.Add(new TranslateTransform(0, 0));
            RenderTransform = tg;
            this.yUp = yUp;
        }
    }
}


