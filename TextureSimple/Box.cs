using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Controls;
using System.Diagnostics;


using System.Windows.Data;


namespace GraphicsBook
{
    /// <summary>
    ///     Draws a rectangle between two points
    /// </summary>
    public class Box : Shape
    {
        // A few constants for the class
        protected Polygon myBox = new Polygon();
        protected RectangleGeometry myRectGeometry = new RectangleGeometry();

        protected const double initialStrokeThickness = 0.0;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.DarkSeaGreen;
        protected static readonly SolidColorBrush initialFillColor = Brushes.DarkSeaGreen;
        protected static readonly Point initialStartPoint = new Point(0, 0);
        protected static readonly Point initialEndPoint = new Point(20, 40);


        /// <summary>
        ///     Identifies the StartPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint",
                typeof(Point), typeof(Box),
                new FrameworkPropertyMetadata(initialStartPoint,
                        FrameworkPropertyMetadataOptions.AffectsMeasure, 
                        new PropertyChangedCallback(StartPointValueChanged), null));


        /// <summary>
        ///     Gets or sets the x-coordinate of the Box StartPoint point.
        /// </summary>
        public Point StartPoint
        {
            set {

                Point u = (Point)GetValue(StartPointProperty);
                Point v = (Point)value;
                if ((u.X != v.X) || (u.Y != v.Y) || true)
                {
                    SetValue(StartPointProperty, value);
                    setToolTip();
                }
            }
            get { return (Point)GetValue(StartPointProperty); }
        }
 
        private static void StartPointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Box myBox = (Box)d;
            myBox.setToolTip();
//            myBox.StartPoint = (Point) e.NewValue;
            //Debug.Print("StartPoint Changed");
        }


        public static readonly DependencyProperty EndPointProperty =
           DependencyProperty.Register("EndPoint",
               typeof(Point), typeof(Box),
               new FrameworkPropertyMetadata(initialEndPoint,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(EndPointValueChanged), null));
        public Point EndPoint
        {
            set { SetValue(EndPointProperty, value); setToolTip(); }
            get { return (Point)GetValue(EndPointProperty); }
        }


        private static void EndPointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Box myBox = (Box)d;
            myBox.setToolTip();
          //  myBox.EndPoint = (Point)e.NewValue;
            //Debug.Print("EndPoint Changed");
        }


        
        /// <summary>
        ///     Gets a value that represents the Geometry of the Box.
        /// </summary>
        protected  override Geometry DefiningGeometry
        {
            get
            {
                myRectGeometry.Rect = new Rect(StartPoint.X, StartPoint.Y, Math.Abs(EndPoint.X-StartPoint.X), Math.Abs(EndPoint.Y-StartPoint.Y));
                return myRectGeometry;
            }
        }

        /// <summary>
        ///     Initializes a new instance of Box.
        /// </summary>
        public Box()
        {
            constructBasics();
            setToolTip();
        }
        /// <summary>
        ///     Initializes a new instance of Box with specified Points as start and end-points
        /// </summary>
        public Box(Point sp, Point ep)
        {
            constructBasics();
            StartPoint = sp;
            EndPoint = ep;
            setToolTip();
        }
        /// <summary>
        ///     Initializes a new instance of Box with specified Dots as start and end-points
        /// </summary>
        public Box(Dot sp, Dot ep)
        {
            constructBasics();
            StartPoint = sp.Position;
            EndPoint = ep.Position;
            setToolTip();
            Binding binding = new Binding("Position");
            binding.Source = sp;
            this.SetBinding(Box.StartPointProperty, binding);
            Binding binding2 = new Binding("Position");
            binding2.Source = ep;
            this.SetBinding(Box.EndPointProperty, binding2);
        }
        /// <summary>
        ///     Initializes a new instance of Box with a Dot as StartPoint and a Point as EndPoint
        /// </summary>
        public Box(Dot sp, Point ep)
        {
            constructBasics();
            StartPoint = sp.Position;
            EndPoint = ep;
            setToolTip();
            Binding binding = new Binding("Position");
            binding.Source = sp;
            this.SetBinding(Box.StartPointProperty, binding);
        }
        /// <summary>
        ///     Construct all the  parts of a Box
        /// </summary>
        private void constructBasics()
        {
//            myRectangle = new Polygon();
 //           myRectangle.Points.Add(new Point(0, 10));
 //           myRectangle.Points.Add(new Point(10, 0));
 //           myRectangle.Points.Add(new Point(-10, 0));
 //           myRectangle.Stroke = Brushes.Black;

            //myBox.StrokeThickness = 1; // 1 mm thick line
            //myBox.Fill = Brushes.LightSeaGreen;
            
            
            //myRectangle.StartPoint = StartPoint;
            //myRectangle.EndPoint = EndPoint; 
            StrokeThickness = initialStrokeThickness;
            Stroke = initialStrokeColor;
            Fill = initialFillColor; 
            ToolTip tt = new ToolTip();
            this.ToolTip = tt;
        }
        private void setToolTip()
        {
            ((ToolTip)this.ToolTip).Content = "Start = (" + StartPoint.X + ", " + StartPoint.Y + "); End = (" + EndPoint.X + ", " + EndPoint.Y + ")";
        }

    }
}
