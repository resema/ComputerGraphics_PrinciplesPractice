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
    ///     Draws a straight line between two points
    /// </summary>
    public class Segment : Shape
    {
        // A few constants for the class
        protected LineGeometry myLine;

        protected const double initialStrokeThickness = 0.6;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.DarkSeaGreen;
        protected static readonly Point initialStartPoint = new Point(0, 0);
        protected static readonly Point initialEndPoint = new Point(20, 40);


        /// <summary>
        ///     Identifies the StartPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint",
                typeof(Point), typeof(Segment),
                new FrameworkPropertyMetadata(initialStartPoint,
                        FrameworkPropertyMetadataOptions.AffectsMeasure, 
                        new PropertyChangedCallback(StartPointValueChanged), null));


        /// <summary>
        ///     Gets or sets the x-coordinate of the ArrowLine StartPoint point.
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
            Segment myEdge = (Segment)d;
            myEdge.setToolTip();
            //myEdge.StartPoint = (Point) e.NewValue;
            //Debug.Print("StartPoint Changed");
        }


        public static readonly DependencyProperty EndPointProperty =
           DependencyProperty.Register("EndPoint",
               typeof(Point), typeof(Segment),
               new FrameworkPropertyMetadata(initialEndPoint,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(EndPointValueChanged), null));

        /// <summary>
        ///     Gets or sets the x-coordinate of the Segment EndPoint point.
        /// </summary>
        public Point EndPoint
        {
            set { SetValue(EndPointProperty, value); setToolTip(); }
            get { return (Point)GetValue(EndPointProperty); }
        }


        private static void EndPointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Segment myEdge = (Segment)d;
            myEdge.setToolTip();
          //  myEdge.EndPoint = (Point)e.NewValue;
            //Debug.Print("EndPoint Changed");
        }


        
        /// <summary>
        ///     Gets a value that represents the Geometry of the Segment.
        /// </summary>
        protected  override Geometry DefiningGeometry
        {
            get
            {
                myLine.StartPoint = StartPoint;
                myLine.EndPoint = EndPoint;
                //Debug.Print("DefiningGeometry Called");
                return myLine;
            }
        }

        /// <summary>
        ///     Initializes a new instance of Segment.
        /// </summary>
        public Segment()
        {
            constructBasics();
            setToolTip();
        }
        /// <summary>
        ///     Initializes a new instance of Segment with specified Points as start and end-points
        /// </summary>
        public Segment(Point sp, Point ep)
        {
            constructBasics();
            StartPoint = sp;
            EndPoint = ep;
            setToolTip();
        }
        /// <summary>
        ///     Initializes a new instance of Segment with specified Dots as start and end-points
        /// </summary>
        public Segment(Dot sp, Dot ep)
        {
            constructBasics();
            StartPoint = sp.Position;
            EndPoint = ep.Position;
            setToolTip();
            Binding binding = new Binding("Position");
            binding.Source = sp;
            this.SetBinding(Segment.StartPointProperty, binding);
            Binding binding2 = new Binding("Position");
            binding2.Source = ep;
            this.SetBinding(Segment.EndPointProperty, binding2);
        }
        /// <summary>
        ///     Initializes a new instance of Segment with a Dot as StartPoint and a Point as EndPoint
        /// </summary>
        public Segment(Dot sp, Point ep)
        {
            constructBasics();
            StartPoint = sp.Position;
            EndPoint = ep;
            setToolTip();
            Binding binding = new Binding("Position");
            binding.Source = sp;
            this.SetBinding(Segment.StartPointProperty, binding);
        }
        /// <summary>
        ///     Construct all the  parts of a Segment
        /// </summary>
        private void constructBasics()
        {
            myLine = new LineGeometry();
            myLine.StartPoint = StartPoint;
            myLine.EndPoint = EndPoint; 
            StrokeThickness = initialStrokeThickness;
            Stroke = initialStrokeColor;
            ToolTip tt = new ToolTip();
            this.ToolTip = tt;
        }
        /// <summary>
        /// Build a tool tip that gives the start and end locations of the segment.
        /// </summary>
        private void setToolTip()
        {
            ((ToolTip)this.ToolTip).Content = "Start = (" + StartPoint.X + ", " + StartPoint.Y + "); End = (" + EndPoint.X + ", " + EndPoint.Y + ")";
        }

    }
}
