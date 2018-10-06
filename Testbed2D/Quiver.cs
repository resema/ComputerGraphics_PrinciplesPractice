using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Diagnostics; 

namespace GraphicsBook
{
    /// <summary>
    /// A quiver is a collection of arrows starting at specified vertices and pointing in specified directions. 
    /// </summary>
    public class Quiver : List<Shape> 
    {
        protected const double initialStrokeThickness = 0.7;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.Black;
        protected static readonly SolidColorBrush initialFillColor = new SolidColorBrush();

        /// <summary>
        /// Build a quiver of arrows, the ith one starting at vertices[i] and going to vertices[i] + arrows[i], 
        /// so that the "arrows" specify the directions of the arrows, and the "vertices" specify their basepoints. 
        /// </summary>
        /// <param name="vertices">an array of starting points for arrows</param>
        /// <param name="arrows">an array of direction vectors for the arrows</param>
        public Quiver(Point[] vertices, Vector[] arrows)
        {
            // assert vertices.length == arrows.length

            for (int i = 0; i < vertices.Length; i++)
            {
                Arrow p = new Arrow(vertices[i], vertices[i] + arrows[i]) ; // , 2, Arrow.endtype.END);
                ToolTip tt = new ToolTip();
                //tt.Content = "Basepoint = " + vertices[i] + "; direction = " + arrows[i] + "; tip = " + (vertices[i] + arrows[i]);
                tt.Content = String.Format("Direction = ({0:F3}..., {1:F3}...); Endpoint = ({2:F3}..., {3:F3}...);", arrows[i].X, arrows[i].Y, vertices[i].X + arrows[i].X, vertices[i].Y + arrows[i].Y );
                p.ToolTip = tt;
                Add(p);
            }
             
            // Create the ellipse for each vertex, and add a tooltip
            for (int i = 0; i < vertices.Length; i++)
            {
                Point p = vertices[i];
                double r = 1; // radius of disk
                ToolTip tt = new ToolTip();
                tt.Content = "Vertex " + i + String.Format(" = ({0:F3}..., {1:F3}...)", vertices[i].X, vertices[i].Y);
                Ellipse myEllipse = new Ellipse();
                myEllipse.Fill = Brushes.Green;
                myEllipse.ToolTip = tt;
                Canvas.SetLeft(myEllipse, p.X - r);
                Canvas.SetTop(myEllipse, p.Y - r);
                myEllipse.Width = 2 * r;
                myEllipse.Height = 2 * r;
                Add(myEllipse);
            }
        }
    }
}
