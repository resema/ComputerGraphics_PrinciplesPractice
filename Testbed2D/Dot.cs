using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Controls;
using System.Diagnostics;

namespace GraphicsBook
{
    /// <summary>
    ///     Represents a clickable dot on screen, possibly with ToolTip, possibly Draggable. 
    /// </summary>
    public class Dot : Shape, INotifyPropertyChanged
    {
        protected EllipseGeometry myEllipse;
        protected const double initialStrokeThickness = 0.4;
        protected const double initialRadius = 1.5;
        protected static readonly  SolidColorBrush initialColor = Brushes.Plum;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.DarkMagenta;
        protected static readonly Point initialPosition = new Point(0, 0);

        /// <summary>
        ///     Identifies the Position dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position",
                typeof(Point), typeof(Dot),
                new FrameworkPropertyMetadata(initialPosition,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(PositionValueChanged), null));


        /// <summary>
        ///     Gets or sets the Position of the Dot
        /// </summary>
        public Point Position
        {
            set
            {
                Point u = (Point)GetValue(PositionProperty);
                Point v = (Point)value;
                if ((u.X != v.X) || (u.Y != v.Y) || true)
                {
                    SetValue(PositionProperty, value);
                    SetValue(XProperty, v.X);
                    SetValue(YProperty, v.Y);
                    setToolTip();
                }
            }
            get { return (Point)GetValue(PositionProperty); }
        }

        /// <summary>
        /// respond to a dependency-property change
        /// </summary>
        /// <param name="d">This Dot</param>
        /// <param name="e">unused</param>
        private static void PositionValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dot myDot = (Dot)d;
            myDot.Position = (Point) myDot.GetValue(PositionProperty);
            myDot.setToolTip();

            //Debug.Print("Position Changed: " + myDot.Position);
        }

        /// <summary>
        ///     Identifies the X-coordinate dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X",
                typeof(double), typeof(Dot),
                new FrameworkPropertyMetadata(initialPosition.X,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(XValueChanged), null));


        /// <summary>
        ///     Gets or sets the X-coordinate of the Dot
        /// </summary>
        public double X
        {
            set
            {
                {
                    SetValue(XProperty, value);
                    Point p = Position;
                    p.X = (double)value;
                    SetValue(PositionProperty, p);
                    setToolTip();
                }
            }
            get { return (double)GetValue(XProperty); }
        }

        private static void XValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Print("X Value Changed: " );
            
            Dot myDot = (Dot)d;
            Point p = myDot.Position;
            p.X = (double)e.NewValue;
            myDot.SetValue(PositionProperty, p);
            myDot.setToolTip();
        }



        /// <summary>
        ///     Identifies the Y-coordinate dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y",
                typeof(double), typeof(Dot),
                new FrameworkPropertyMetadata(initialPosition.Y,
                        FrameworkPropertyMetadataOptions.AffectsMeasure,
                        new PropertyChangedCallback(YValueChanged), null));


        /// <summary>
        ///     Gets or sets the Y-coordinate of the Dot
        /// </summary>
        public double Y
        {
            set
            {
                if ((double)GetValue(YProperty) != (double)value)
                {
                    SetValue(YProperty, value);
                    Point p = Position;
                    p.Y = (double)value;
                    SetValue(PositionProperty, p);
                    setToolTip();
                }
            }
            get { return (double)GetValue(YProperty); }
        }

        private static void YValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dot myDot = (Dot)d;
            Point p = myDot.Position;
            p.Y = (double)e.NewValue;
            myDot.SetValue(PositionProperty, p);
            myDot.setToolTip();
            //Debug.Print("Y Changed");
        }

        /// <summary>
        ///     Gets a value that represents the Geometry of the Dot.
        /// </summary>
        protected  override Geometry DefiningGeometry
        {
            get
            {
                myEllipse.Center = Position;
                return myEllipse;
            }
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
        ///     Initializes a new instance of Dot.
        /// </summary>
        public Dot()
        {
            myEllipse = new EllipseGeometry();
            myEllipse.RadiusX = initialRadius;
            myEllipse.RadiusY = initialRadius;
            myEllipse.Center = Position;
        }
        /// <summary>
        ///     Construct all the  parts of a Dot
        /// </summary>
        private void constructBasics()
        {
            myEllipse = new EllipseGeometry();
            myEllipse.RadiusX = initialRadius;
            myEllipse.RadiusY = initialRadius;
            myEllipse.Center = initialPosition;
            StrokeThickness = initialStrokeThickness;
            Stroke = initialStrokeColor;
            Fill = initialColor;
        }

        /// <summary>
        /// Build a tool tip that gives the location of the Dot
        /// </summary>
        private void setToolTip()
        {
            ToolTip tt = new ToolTip();
            tt.Content = "(" + Position.X + ", " + Position.Y + ")";
            this.ToolTip = tt;
        }

        /// <summary>
        ///     Initializes a new instance of Dot at the same location as the argument Dot
        /// </summary>
        /// <param name="init">The Dot whose location should be copied</param>
        public Dot(Dot init)
        {
            constructBasics();
            Position = init.Position;
            //Debug.Print("Pos = " + Position);
            myEllipse.Center = init.Position;
            setToolTip();
        }

        /// <summary>
        ///     Initializes a new instance of Dot at the same location as the argument Point
        /// </summary>
        /// <param name="init">The Point whose location should be copied</param>
        public Dot(Point init)
        {
            constructBasics();
            myEllipse.Center = init;
            Position = init;
            setToolTip();

        }

  
        /// <summary>
        /// Initialize a Dot at the given (x,y) location
        /// </summary>
        /// <param name="x">The x-coordinate for the Dot</param>
        /// <param name="y">The y-coordinate for the Dot</param>

        public Dot(double x, double y)
        {
            constructBasics();
            Point p = new Point(x, y);
            myEllipse.Center = p;
            Position = p;
            
            setToolTip();
        }

        /// <summary>
        ///     Make this point draggable within the named Canvas; if it's 
        ///     not actually located within the given Canvas, results are 
        ///     undefined. 
        /// </summary>
        public void MakeDraggable(Canvas theCanvas)
        {
            //Debug.Print("MakeDraggable INIT");
            _canvas = theCanvas;
            _canvas.PreviewMouseDown += DPreviewMouseLeftButtonDown;
            _canvas.PreviewMouseUp += DPreviewMouseLeftButtonUp;
            _canvas.PreviewMouseMove += DPreviewMouseMove;
        }

        private Canvas _canvas;
        private Point _StartPoint; // Where did the mouse start off from?
        private double _OriginalLeft; // What was the element's original x offset?
        private double _OriginalTop; // What was the element's original y offset?
        private bool _IsDown; // Is the mouse down right now?
        private bool _IsDragging; // Are we actually dragging the shape around?
        private Rectangle _OverlayElement; // What is it that we're using to show where the shape will end up?

        private void DPreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            //Debug.Print("Preview Mouse Down");
            if (e.Source != this) // ONLY OK because this element is a single shape with no children!
            {
                //Debug.Print("Preview Mouse Down Bad Source");
                return;
            }
            _IsDown = true;
            _StartPoint = e.GetPosition(_canvas);
            //Debug.Print("StartPoint" + _StartPoint);
            _canvas.CaptureMouse();
            e.Handled = true;
            //Debug.Print("Preview Mouse Down DONE");
            
        }

        private void DPreviewMouseMove(Object sender, MouseEventArgs e)
        {
            if (_IsDown)
            {
                if ( (!_IsDragging ) & 
                   (Math.Abs(e.GetPosition(_canvas).X - _StartPoint.X) > SystemParameters.MinimumHorizontalDragDistance | 
                    Math.Abs(e.GetPosition(_canvas).Y - _StartPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    //Debug.Print("DragStarted");
                    DragStarted();

                }
                if (_IsDragging)
                {
                    DragMoved();
                }
            }
        }

        private void DragStarted()
        {
            _IsDragging = true;
            _OriginalLeft = Position.X;
            _OriginalTop = Position.Y;

            VisualBrush brush= new VisualBrush(this);
            brush.Opacity = 0.5;
            _OverlayElement = new Rectangle();
            double size = myEllipse.RadiusX * 2 + StrokeThickness;
            _OverlayElement.Width = size; // this.RenderSize.Width;
            _OverlayElement.Height = size; //this.RenderSize.Height;
            _OverlayElement.Fill = brush;
            Canvas.SetLeft(_OverlayElement, _OriginalLeft - size/2); // - Width/2);
            Canvas.SetTop(_OverlayElement, _OriginalTop - size/2); //-Height/2);
            _canvas.Children.Add(_OverlayElement);
        }

        private void DragMoved()
        {
            //Debug.Print("DragMoved");
            Point currentPosition = System.Windows.Input.Mouse.GetPosition(_canvas);
            double size = myEllipse.RadiusX * 2 + StrokeThickness;
            double elementLeft = (currentPosition.X - _StartPoint.X) + _OriginalLeft; // -Width / 2;
            double elementTop = (currentPosition.Y - _StartPoint.Y) + _OriginalTop; // - Height/2;
            Canvas.SetLeft(_OverlayElement, elementLeft - size/2);
            Canvas.SetTop(_OverlayElement, elementTop-size/2);
            //Debug.Print("DragMovedDone");
        }

        private void DPreviewMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
        {
            //Debug.Print("ButtonUp");
            if (_IsDown)
            {
                DragFinished();
                e.Handled = true;
            }
        }

        private void DragFinished()
        {
            System.Windows.Input.Mouse.Capture(null);
            if (_IsDragging)
            {
                double size = myEllipse.RadiusX * 2 + StrokeThickness;
            
                _canvas.Children.Remove(_OverlayElement);
                Position = new Point(Canvas.GetLeft(_OverlayElement)+size/2, Canvas.GetTop(_OverlayElement)+size/2);
            }
            _IsDragging = false;
            _IsDown = false;
        }
    }
}


