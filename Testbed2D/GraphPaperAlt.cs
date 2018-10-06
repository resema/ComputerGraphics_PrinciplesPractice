// Canvas1.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Reflection;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics; 

namespace GraphicsBook
{
//    public partial class GraphPaper : Canvas
    public class GraphPaperAlt : Canvas
    {
        double graphSize = 100; // Graph paper extends 100mm to each side of origin.
        double gridSize = 5; // Lines on graph paper are 5mm apart. 

        public GraphPaperAlt()
        {

             TransformGroup g = new TransformGroup();
             g.Children.Add(new ScaleTransform(1.0, 1.0)); // in GraphPaper class, this is (1.0, -1.0)
             g.Children.Add(new ScaleTransform(mm(1), mm(1)));
             g.Children.Add(new TranslateTransform(mm(graphSize), mm(graphSize)));
             this.RenderTransform = g;

             buildGraphPaperLook();
         }

        /*
         * Convert millimeters to WPF units
         */
        public static double mm(double xInMM)
        {
            return (xInMM/25.4) * 96.0; // 96 WPF units = inch; inch = 25.4 mm.  
        }

        // Convert WPF units to mm
        public static double wpf(double xInWPF)
        {
            return (xInWPF / 96.0) * 25.4; // 96 WPF units = inch; inch = 25.4 mm.  
        }

        // Setup up graph-paper look
        private void buildGraphPaperLook()
        {
            // Create a Path to be drawn to the screen.
            Path myPath = new Path();
            myPath.Stroke = Brushes.LightBlue;
            myPath.StrokeThickness = 1/mm(1); // That's one WPF unit; tends to be about 1 pixel on today's displays
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 245, 245, 255);
            myPath.Fill = mySolidColorBrush;

            // The "graph paper" region of the canvas extends from -100mm to 100mm. 
            // Create the rectangle geometry to add to the Path
            RectangleGeometry myRectGeometry = new RectangleGeometry();
            myRectGeometry.Rect = new Rect(-graphSize, -graphSize, 2 * graphSize, 2 * graphSize);

            GeometryGroup myGeometryGroup = new GeometryGroup();
            myGeometryGroup.Children.Add(myRectGeometry);

            // Create the line geometry (thin graphpaper lines) to add to the Path
            int lastLine = (int)(graphSize / gridSize);
            for (int i = -lastLine; i <= lastLine; i++)
            {
                Debug.Write("Constructing Grid line " + i + "\n");
                LineGeometry myLineGeometry = new LineGeometry();
                myLineGeometry.StartPoint = new Point(i * gridSize, -graphSize);
                myLineGeometry.EndPoint = new Point(i * gridSize, graphSize);
                myGeometryGroup.Children.Add(myLineGeometry);
                myLineGeometry = new LineGeometry();
                myLineGeometry.StartPoint = new Point(-graphSize, i * gridSize);
                myLineGeometry.EndPoint = new Point(graphSize, i * gridSize);
                myGeometryGroup.Children.Add(myLineGeometry);
            }
            myPath.Data = myGeometryGroup;
            this.Children.Add(myPath);

            // Create the coordinate axes
            mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 245, 245, 255);

            Arrow xAxis = new Arrow(
                    new Point(-0.1 * graphSize, 0),
                    new Point(0.93 * graphSize, 0),
                    Arrow.endtype.END);
            xAxis.Stroke = Brushes.Blue;
            xAxis.StrokeThickness = 2/mm(1);
            xAxis.Fill = mySolidColorBrush;
            xAxis.ToolTip = null;
            Arrow yAxis = new Arrow( new Point(0, -0.1*graphSize),
                    new Point(0, 0.9 * graphSize),
                    Arrow.endtype.END);
            yAxis.Stroke = Brushes.Blue;
            yAxis.StrokeThickness = 2/mm(1);
            yAxis.Fill = mySolidColorBrush;
            yAxis.ToolTip = null; 
            Children.Add(xAxis);
            Children.Add(yAxis);


            TextBlock tBlock = new TextBlock();
			tBlock.TextWrapping = TextWrapping.Wrap;
	        tBlock.Background = Brushes.AntiqueWhite;
            tBlock.TextAlignment = TextAlignment.Center;
            tBlock.RenderTransform = new ScaleTransform(1/mm(1), 1/mm(1));  // in GraphPaper, second arg is -1/mm(1)
            tBlock.Inlines.Add(new Bold(new Run("Y")));
            Canvas.SetTop(tBlock, 0.95*graphSize);
            Canvas.SetLeft(tBlock, 0);
            Children.Add(tBlock);

            tBlock = new TextBlock();
            tBlock.TextWrapping = TextWrapping.Wrap;
            tBlock.Background = Brushes.AntiqueWhite;
            tBlock.TextAlignment = TextAlignment.Center;
            tBlock.RenderTransform = new ScaleTransform(1/mm(1), 1/mm(1));
            tBlock.Inlines.Add(new Bold(new Run("X")));
            Canvas.SetLeft(tBlock, 0.95 * graphSize);
            Canvas.SetTop(tBlock, 0);
            Children.Add(tBlock);

            //Origin of coord system.
            Ellipse e = new Ellipse();
            e.Fill = Brushes.Blue; //mySolidColorBrush;
            double originSize = 1.0;
            SetLeft(e, -originSize);
            SetTop(e, -originSize);
            e.Width = 2*originSize;
            e.Height = 2*originSize;
            Children.Add(e);
        }
    }
}