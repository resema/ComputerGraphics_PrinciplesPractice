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
    ///     Represents a Circle on screen, possibly with ToolTip. 
    /// </summary>
    public class Circle : Shape
    {
        protected EllipseGeometry myEllipse;
        protected const double initialStrokeThickness = 0.4;
        protected const double initialRadius = 1.5;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.MidnightBlue;
        protected static readonly Point center = new Point(0, 0);

       
        /// <summary>
        ///     Gets or sets the Position of the Circle
        /// </summary>
        public Point Position
        {
            set
            {
                myEllipse.Center = value;
                setToolTip();

/*                if ((u.X != v.X) || (u.Y != v.Y) || true)
                {
                    SetValue(PositionProperty, value);
                    SetValue(XProperty, v.X);
                    SetValue(YProperty, v.Y);
                    setToolTip();
                }
 * */
            }
            get { return myEllipse.Center; }
        }

        /// <summary>
        ///     Gets or sets the radius of the Circle
        /// </summary>
        public double Radius
        {
            set
            {
                myEllipse.RadiusX = value;
                myEllipse.RadiusY = value;
                setToolTip();

            }
            get { return myEllipse.RadiusX; }
        }

 

        /// <summary>
        ///     Gets a value that represents the Geometry of the Circle.
        /// </summary>
        protected  override Geometry DefiningGeometry
        {
            get
            {
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
        ///     Initializes a new instance of Circle with the default radius and position. 
        /// </summary>
        public Circle()
        {
            myEllipse = new EllipseGeometry();
            myEllipse.RadiusX = initialRadius;
            myEllipse.RadiusY = initialRadius;
            myEllipse.Center = Position;
        }
        /// <summary>
        ///     Construct all the  parts of a Circle
        /// </summary>
        private void constructBasics()
        {
            myEllipse = new EllipseGeometry();
            myEllipse.RadiusX = initialRadius;
            myEllipse.RadiusY = initialRadius;
            myEllipse.Center = center;
            StrokeThickness = initialStrokeThickness;
            Stroke = initialStrokeColor;
            //Fill = initialColor;
        }

        /// <summary>
        /// Build a tooltip giving the center and radius of the circle
        /// </summary>
        private void setToolTip()
        {
            ToolTip tt = new ToolTip();
            tt.Content = "Circle with center (" + Position.X + ", " + Position.Y + ") and radius " + Radius + ".";
            this.ToolTip = tt;
        }
        /// <summary>
        ///     Initializes a new instance of Circle at the same location as the argument Circle
        /// </summary>

        public Circle(Circle init)
        {
            constructBasics();
            Position = init.Position;
            //Debug.Print("Pos = " + Position);
            myEllipse.Center = init.Position;
            setToolTip();
        }


        /// <summary>
        ///     Initializes a new instance of Circle at the given Point with the given radius
        /// </summary>
        /// <param name="init">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        public Circle(Point init, double r)
        {
            constructBasics();
            myEllipse.Center = init;
            myEllipse.RadiusX = r;
            myEllipse.RadiusY = r;
            Position = init;
            setToolTip();

        }


        /// <summary>
        /// Build a circle with center (x, y) and radius r
        /// </summary>
        /// <param name="x">The x-coordinate of the circle's center</param>
        /// <param name="y">The y-coordinate of the circle's center</param>
        /// <param name="r">The circle's radius</param>
        public Circle(double x, double y, double r)
        {
            constructBasics();
            Point p = new Point(x, y);
            myEllipse.Center = p;
            Position = p;
            myEllipse.RadiusX = r;
            myEllipse.RadiusY = r;
            
            setToolTip();
        }
    }
}


