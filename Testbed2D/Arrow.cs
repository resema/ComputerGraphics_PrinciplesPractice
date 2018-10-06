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
    ///     Draws a straight line between two points with 
    ///     optional arrowheads on the ends.
    /// </summary>
    public class Arrow : Shape
    {
        // A few constants for the class
        protected PathGeometry myArrow;
        PathFigure myPathFigure;
        protected double size = 5;
        protected endtype ends = endtype.END; // default. 
        protected ToolTip tt = new ToolTip();
                   
        protected const double initialStrokeThickness = 0.6;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.DarkOrange;
        protected static readonly Point initialBasePoint = new Point(0, 0);
        protected static readonly Point initialTip = new Point(20, 40);

        public enum endtype { START = 1, END = 2, BOTH = 3 };

        /// <summary>
        ///     Identifies the BasePoint dependency property.
        /// </summary>
        public static readonly DependencyProperty BasePointProperty =
            DependencyProperty.Register("BasePoint",
                typeof(Point), typeof(Arrow),
                new FrameworkPropertyMetadata(initialBasePoint,
                        FrameworkPropertyMetadataOptions.AffectsMeasure, 
                        new PropertyChangedCallback(BasePointValueChanged), null));


        /// <summary>
        ///     Gets or sets the Arrow BasePoint.
        /// </summary>
        public Point BasePoint
        {
            set { 
                SetValue(BasePointProperty, value);
                setToolTip();
            }
            get { return (Point)GetValue(BasePointProperty); }
        }

        private static void BasePointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Arrow myArrow = (Arrow)d;
            myArrow.setToolTip();
            //Debug.Print("BasePoint Changed");
        }

        /// <summary>
        ///     Gets or sets the Arrow Tip.
        /// </summary>
        public static readonly DependencyProperty TipProperty =
           DependencyProperty.Register("Tip",
               typeof(Point), typeof(Arrow),
               new FrameworkPropertyMetadata(initialTip,
                       FrameworkPropertyMetadataOptions.AffectsMeasure));
        
        /// <summary>
        ///     Identifies the Tip dependency property..
        /// </summary>
        public Point Tip
        {
            set { SetValue(TipProperty, value); 
                setToolTip(); 
            }
            get { return (Point)GetValue(TipProperty); }
        }

        /// <summary>
        ///     Gets a value that represents the Geometry of the Arrow.
        /// </summary>
        protected  override Geometry DefiningGeometry
        {
            get
            {
                myArrow = new PathGeometry();
                myPathFigure = new PathFigure();
                myPathFigure.StartPoint = BasePoint;
                myPathFigure.Segments.Add(
                    new LineSegment(Tip,
                    true /* IsStroked */ ));
                if ((ends == endtype.END) || (ends == endtype.BOTH))
                {
                    addArrowhead(Tip, BasePoint);
                }
                if ((ends == endtype.START) || (ends == endtype.BOTH))
                {
                    myPathFigure.Segments.Add(
                        new LineSegment(BasePoint, false));
                    addArrowhead(BasePoint, Tip);
                }
                myArrow.Figures.Add(myPathFigure);
                return myArrow;
            }
        }

        /// <summary>
        /// Add an arrowhead at arrowEnd, with the other end of the arrow at "reference". 
        /// </summary>
        /// <param name="arrowEnd">Location of the arrowhead</param>
        /// <param name="reference">Other end of the arrow</param>
        private void addArrowhead(Point arrowEnd, Point reference)
        {
            Vector u = arrowEnd - reference;
            double len = u.Length;
            double factor = Math.Min(len / size, 1);
            u.Normalize();
            Vector v = new Vector(-u.Y, u.X); // 90 degrees from u, ccw. 
            double size1 = size * 0.99; // ugly hack to get around implementation error in Bezier curves
            Point end1 = arrowEnd + .01 * v;
            Point p = arrowEnd - size * u;
            Point p1 = arrowEnd - size * u - 0.01 * v;
            double widthFraction = .4;
            Point q = p + widthFraction * size * v;
            Point q1 = p + widthFraction * size1 * v + .01 * u;
            Point r = p - widthFraction * size * v;
            Point r1 = p - widthFraction * size1 * v + .01 * u;

            myPathFigure.Segments.Add(new BezierSegment(end1, p, q1, true));
            myPathFigure.Segments.Add(new LineSegment(q, true));
            myPathFigure.Segments.Add(new LineSegment(r, true));
            myPathFigure.Segments.Add(new LineSegment(r1, true));
            myPathFigure.Segments.Add(new BezierSegment(p, end1, arrowEnd, true));
        }

        /// <summary>
        ///     Initializes a new instance of Arrow, with both ends at the origin.
        /// </summary>
        public Arrow()
        {
            constructBasics();
            setToolTip();
        }

        /// <summary>
        ///    Initializes a new instance of Arrow with specified Points as start and end-points
        /// </summary>
        /// <param name="sp">The starting point of the arrow</param>
        /// <param name="ep">The endpoint of the arrow</param>
        public Arrow(Point sp, Point ep)
        {
            constructionHelper1(sp, ep, endtype.END);
        }

        /// <summary>
        /// Initializes a new instance of Arrow with specified Points as start and end-points,
        /// and with specified end types: endtype.START, endtype.END, or endtype.BOTH

        /// </summary>
        /// <param name="sp">Starting point</param>
        /// <param name="ep">Ending point</param>
        /// <param name="ends">The endtype constant indicating which end(s) should have arrowhead(s)</param>
        public Arrow(Point sp, Point ep, endtype ends)
        {
            constructionHelper1(sp, ep, ends);
        }

        /// <summary>
        /// Set the start and end point, and the arrowhead types for this arrow. 
        /// </summary>
        /// <param name="sp">Location of the arrow start</param>
        /// <param name="ep">Location of arrow end</param>
        /// <param name="endtypes">endtype constant saying which end(s) should have arrowhead(s)</param>
        private void constructionHelper1(Point sp, Point ep, endtype endtypes)
        {
            ends = endtypes;
            BasePoint = sp;
            Tip = ep;
            constructBasics();
            setToolTip();
            buildBindings(null, null);
        }        
       
        /// <summary>
        /// Build an arrow using Dots (manipulable) as starting and ending points
        /// 
        /// </summary>
        /// <param name="sp">The start of the arrow</param>
        /// <param name="ep">The end of the arrow</param>
        public Arrow(Dot sp, Dot ep)
        {
            constructionHelper2(sp, ep, endtype.END);
        }

        /// <summary>
        ///     Initializes a new instance of Arrow with specified Dots as start 
        ///     and end-points, and arrowheads where specified by the 
        ///     endtypes parameter.
        /// </summary>
        /// <param name="sp">The starting point</param>
        /// <param name="ep">The ending point</param>
        /// <param name="endtypes">endtype constant saying which end(s) should have arrowhead(s)</param>
        public Arrow(Dot sp, Dot ep, endtype endtypes)
        {
            constructionHelper2(sp, ep, endtypes);
        }

        /// <summary>
        ///     Initializes a new instance of Arrow with specified Dots as start 
        ///     and end-points, and arrowheads where specified by the 
        ///     endtypes parameter.
        /// </summary>
        /// <param name="sp">The starting point</param>
        /// <param name="ep">The ending point</param>
        /// <param name="endtypes">endtype constant saying which end(s) should have arrowhead(s)</param>
        private void constructionHelper2(Dot sp, Dot ep, endtype endtypes)
        {
            ends = endtypes;
            BasePoint = sp.Position;
            Tip = ep.Position;
            constructBasics();
            setToolTip();
            buildBindings(sp, ep);
        }        
        


        
        /// <summary>
        /// Build an arrow using a Dot (manipulable) as starting point, and a Point as ending point
        /// 
        /// </summary>
        /// <param name="sp">The start of the arrow</param>
        /// <param name="ep">The end of the arrow</param>
        public Arrow(Dot sp, Point ep)
        {
            constructionHelper3(sp, ep, endtype.END);
        }

        /// <summary>
        ///     Initializes a new instance of Arrow with a Dot as start point and a 
        ///     Point as end-point, and arrowheads where specified by the 
        ///     endtypes parameter.
        /// </summary>
        /// <param name="sp">The starting point</param>
        /// <param name="ep">The ending point</param>
        /// <param name="endtypes">endtype constant saying which end(s) should have arrowhead(s)</param>
        public Arrow(Dot sp, Point ep, endtype endtypes)
        {
            constructionHelper3(sp, ep, endtypes);
        }

        /// <summary>
        ///     Initializes a new instance of Arrow with a Dot as start point and a 
        ///     Point as end-point, and arrowheads where specified by the 
        ///     endtypes parameter.
        /// </summary>
        /// <param name="sp">The starting point</param>
        /// <param name="ep">The ending point</param>
        /// <param name="endtypes">endtype constant saying which end(s) should have arrowhead(s)</param>
        private void constructionHelper3(Dot sp, Point ep, endtype endtypes)
        {
            ends = endtypes;
            BasePoint = sp.Position;
            Tip = ep;
            constructBasics();
            setToolTip();
            buildBindings(sp, null);
        }        


        /// <summary>        
        ///     Initializes a new instance of Arrow with a Point as start point and a 
        ///     Dot as end-point.
        /// </summary>
        /// <param name="sp">The starting point</param>
        /// <param name="ep">The ending point</param>
        public Arrow(Point sp, Dot ep)
        {
            constructionHelper4(sp, ep, endtype.START);
        }

        /// <summary>        
        ///     Initializes a new instance of Arrow with a Point as start point and a 
        ///     Dot as end-point, and arrowheads where specified by the 
        ///     endtypes parameter.
        /// </summary>
        /// <param name="sp">The starting point</param>
        /// <param name="ep">The ending point</param>
        /// <param name="endtypes">endtype constant saying which end(s) should have arrowhead(s)</param>
        public Arrow(Point sp, Dot ep, endtype endtypes)
        {
            constructionHelper4(sp, ep, endtypes);
        }

        /// <summary>        
        ///     Initializes a new instance of Arrow with a Point as start point and a 
        ///     Dot as end-point, and arrowheads where specified by the 
        ///     endtypes parameter.
        /// </summary>
        /// <param name="sp">The starting point</param>
        /// <param name="ep">The ending point</param>
        /// <param name="endtypes">endtype constant saying which end(s) should have arrowhead(s)</param>
        public void constructionHelper4(Point sp, Dot ep, endtype endtypes)
        {
            ends = endtypes;
            BasePoint = sp;
            Tip = ep.Position;
            constructBasics();
            //setToolTip();
            buildBindings(null, ep);
        }        

        /// <summary>
        ///     Construct all the  parts of an Arrow
        /// </summary>
        private void constructBasics()
        {
            StrokeThickness = initialStrokeThickness;
            Stroke = initialStrokeColor;
            this.ToolTip = tt;
        }

        /// <summary>
        /// Construct a tool-tip that displays the basepoint and endpoint of the arrow. 
        /// </summary>
        private void setToolTip()
        {
            tt.Content = String.Format("BasePoint = ({0:F3}..., {1:F3}...); End = ({2:F3}..., {3:F3}...)", BasePoint.X ,BasePoint.Y, Tip.X, Tip.Y);
        }

        /// <summary>
        /// Establish the data bindings for the start and end points of an arrow, if they're Dots, so that 
        /// when the Dots move, the start and end points of the arrow update as well. 
        /// </summary>
        /// <param name="sp">The starting Dot (or null if it's a Point)</param>
        /// <param name="ep">The ending Dot (or null if it's a Point)</param>
        private void buildBindings(Dot sp, Dot ep)
        {
            if (sp != null)
            {
                Binding binding = new Binding("Position");
                binding.Source = sp;
                this.SetBinding(Arrow.BasePointProperty, binding);
            } 
            if (ep != null)
            {
                Binding binding = new Binding("Position");
                binding.Source = ep;
                this.SetBinding(Arrow.TipProperty, binding);
            }
        }

    }
}
