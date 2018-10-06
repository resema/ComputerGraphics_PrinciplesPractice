using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics; 

namespace GraphicsBook
{
    
    /*
     * A geometric object that can be drawn to make a Mesh in a canvas. 
     * Mesh shape is specified by an array of points, and an array of edges (which 
     * are specified as pairs of point-indices). So a typical model of a square would have
     * ptsArray = {new Point(0,0), new Point(10, 0), new Point(10, 10), new Point (0, 10)};
     * edgeArray = { (0,1), (1, 2), (2, 3), (3, 0) }
     * where these are written in a compact form to make them readable. 
     * A mesh is displayed by drawing lots of tiny disks (for the vertices), and then all the edges. 
     */
    /// <summary>
    /// A geometric object that can be drawn to make a Mesh in a canvas. 
    /// Mesh shape is specified by an array of points, and an array of edges (which 
    /// are specified as pairs of point-indices). So a typical model of a square would have
    /// ptsArray = {new Point(0,0), new Point(10, 0), new Point(10, 10), new Point (0, 10)};
    /// edgeArray = { (0,1), (1, 2), (2, 3), (3, 0) }
    /// where these are written in a compact form to make them readable. 
    /// A mesh is displayed by drawing lots of tiny disks (for the vertices), and then all the edges. 
    /// </summary>
    public class Mesh : Shape
    {
        protected GeometryGroup myGeometryGroup = new GeometryGroup();
        protected const double initialStrokeThickness = 0.7;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.Black;
        protected static readonly SolidColorBrush initialFillColor = new SolidColorBrush();
            
        protected static readonly Point initialStartPoint = new Point(0, 0);
        protected static readonly Point initialEndPoint = new Point(20, 40);
        /// <summary>
        /// The mesh's geometry, a group of Lines and Points
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                return myGeometryGroup;
            }
        }

        /// <summary>
        /// Build a mesh from a collection of nverts Points and nedges edges, described by pairs of point-array indices
        /// 
        /// </summary>
        /// <param name="nverts">The number of vertices in the mesh</param>
        /// <param name="nedges">The number of edges in the mesh</param>
        /// <param name="vertices">The list of (at least nverts) vertices</param>
        /// <param name="edges">A collection of nedges (int, int) pairs, where each int is between 0 and nverts-1.</param>
        public Mesh(int nverts, int nedges, Point[] vertices, int[,] edges)
        {
            StrokeThickness = initialStrokeThickness;
            Stroke = initialStrokeColor;
            Fill = initialFillColor;

            initialFillColor.Color = Color.FromArgb(255, 74, 204, 74);
            for (int i = 0; i < nedges; i++)
            {
                LineGeometry myLineGeometry = new LineGeometry();
                myLineGeometry = new LineGeometry();
                int s = edges[i, 0];
                int e = edges[i, 1];
                myLineGeometry.StartPoint = vertices[s];
                myLineGeometry.EndPoint = vertices[e];
                myGeometryGroup.Children.Add(myLineGeometry);
            }
            for (int i = 0; i < vertices.Length; i++) 
            {
                Point p = vertices[i];
                ToolTip tt = new ToolTip();
                tt.Content = "Vertex " + i;
                EllipseGeometry myEllipseGeometry = new EllipseGeometry();
                //myEllipseGeometry.ToolTip = tt;
                myEllipseGeometry.Center = p;
                myEllipseGeometry.RadiusX = 1;  // Canvas1.mm(2);
                myEllipseGeometry.RadiusY = 1; // Canvas1.mm(2);
                myGeometryGroup.Children.Add(myEllipseGeometry);
            }

        }
    }
}
