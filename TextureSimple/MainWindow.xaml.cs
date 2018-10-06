using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

using System.Diagnostics;

namespace GraphicsBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GraphPaper gp = null;

        static int sourceSize = 50;
        static int targetSize = 100;
        static double pscale = 1;
        Point lowerLeft = new Point(-sourceSize * pscale, 0); // lower left corner of texture image
        Point upperRight = new Point(0, sourceSize * pscale); // upper-right corner of texture image
        Point A = new Point(0, 0);
        Point B = new Point(pscale * targetSize, 0);
        Point C = new Point(pscale * targetSize / 2, pscale * targetSize); //* Math.Sqrt(3.0)/2

        Polygon rightTriangle = null;
        Dot Ad = new Dot(new Point(-pscale * sourceSize, 0));
        Dot Bd = new Dot(new Point(0, 6));
        Dot Cd = new Dot(new Point(-pscale * sourceSize / 2, .287 * pscale * sourceSize)); //* Math.Sqrt(3.0) / 2

        double [,] textureImage = new double[sourceSize,sourceSize];
        double[,] displayImage = new double[targetSize, targetSize];
        double[,,] barycentricCoords = new double[targetSize,targetSize,3];
        Box[,] displayRects = null; 

        // Are we ready for interactions like slider-changes to alter the 
        // parts of our display (like polygons or images or arrows)? Probably not until those things 
        // have been constructed!
        bool ready = false;

        // Code to create and display objects goes here.
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();

            // Now add some graphical items in the main Canvas, whose name is "GraphPaper"
            gp = this.FindName("Paper") as GraphPaper;

            // Initialize triangles, texture image, display image
            initTextureImage();
            DrawImage(lowerLeft.X, lowerLeft.Y, textureImage, sourceSize, sourceSize);
            double[] s = new double[3];
            barycentricCoordinates(A.X, A.Y, s);
            barycentricCoordinates(B.X, B.Y, s);
            barycentricCoordinates(C.X, C.Y, s);

            initializeBarycentricCoords();
            initDisplayImage();

            displayRects = DrawImage(0, 0, displayImage, targetSize, targetSize);
            updateDisplayImage();

            rightTriangle = makeTri(A,B,C);
            initDraggableTriangle();
            gp.Children.Add(rightTriangle);

            // fix so that when the dots move, the DisplayImage is updated...
           
            ready = true; // Now we're ready to have sliders and buttons influence the display.
        }

        #region Interaction handling -- sliders and buttons
        /* Event handler for a click on button one */
        public void b1Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Button one clicked!\n");
            e.Handled = true; // don't propagate the click any further
        }


        void slider1change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Print("Slider changed, ready = " + ready + ", and val = " + e.NewValue + ".\n");
            e.Handled = true;
            if (ready)
            {
                //PointCollection p = myTriangle.Points.Clone();
                //Debug.Print(myTriangle.Points.ToString());
                //Point u = p[0];
                //u.X = e.NewValue;
                //p[0] = u;
            }
        }

        public void b2Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Button two clicked!\n");
            e.Handled = true; // don't propagate the click any further
        }
        #endregion

        #region Menu, command, and keypress handling

        protected static RoutedCommand ExitCommand;

        protected void InitializeCommands()
        {
            InputGestureCollection inp = new InputGestureCollection();
            inp.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            ExitCommand = new RoutedCommand("Exit", typeof(MainWindow), inp);
            CommandBindings.Add(new CommandBinding(ExitCommand, CloseApp));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseApp));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandHandler));
        }

        void NewCommandHandler(Object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("You selected the New command",
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

        }

        // Announce keypresses, EXCEPT for CTRL, ALT, SHIFT, CAPS-LOCK, and "SYSTEM" (which is how Windows 
        // seems to refer to the "ALT" keys on my keyboard) modifier keys
        // Note that keypresses that represent commands (like ctrl-N for "new") get trapped and never get
        // to this handler.
        void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if ((e.Key != Key.LeftCtrl) &&
                (e.Key != Key.RightCtrl) &&
                (e.Key != Key.LeftAlt) &&
                (e.Key != Key.RightAlt) &&
                (e.Key != Key.System) &&
                (e.Key != Key.Capital) &&
                (e.Key != Key.LeftShift) &&
                (e.Key != Key.RightShift))
            {
                MessageBox.Show(String.Format("[{0}]  {1} received @ {2}",
                                        e.Key,
                                        e.RoutedEvent.Name,
                                        DateTime.Now.ToLongTimeString()),
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }

        void CloseApp(Object sender, ExecutedRoutedEventArgs args)
        {
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Really Exit?",
                                Title,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question)
               ) Close();
        }
        #endregion //Menu, command and keypress handling

       private Box[,] DrawImage(double x, double y, double[,] image, int rows, int cols)
        {
            Box[,] boxes = new Box[rows, cols];

            // Draw an image with 10x10 rectangular blocks as "pixels"
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Point p1 = new Point(pscale * i + x, pscale * j + y);
                    Point p2 = new Point(pscale * (i + 1) + x , pscale * (j + 1) + y);

                    boxes[i,j] = new Box(p1, p2);
                    Box b = boxes[i, j];
                    if (image[i, j] == 0)
                    {
                        b.Fill = Brushes.Black;
                        
                    }
                    else if (image[i, j] == 1)
                    {
                        b.Fill = Brushes.White;
                   
                    }
                    else 
                    {
                        b.Fill = null;
                    }
                    gp.Children.Add(b);
                }
            }
            return boxes;
        }

        private Polygon makeTri(Point A, Point B, Point C)
        {
            Polygon u = new Polygon();
            u.Points.Add(A);
            u.Points.Add(B);
            u.Points.Add(C);
            u.Stroke = Brushes.Black;
            u.StrokeThickness = 1; // 1 mm thick line
            u.Fill = null;
            return u;
        }

        private void initializeBarycentricCoords()
        {
            // For each pixel center in the right-hand image, determine
            // its barycentric coords wrt the right-hand triangle.
            double[] s = new double[3];
            for (int i = 0; i < targetSize; i++)
            {
                for (int j = 0; j < targetSize; j++)
                {
                    barycentricCoordinates(pscale*(i+1/2.0), pscale*(j + 1/2.0), s);
                    for (int k = 0; k < 3; k++)
                    {
                        barycentricCoords[i, j, k] = s[k];
                    }
                }
            }
        }
        public Vector perp(Vector v)
        {
            return new Vector(-v.Y, v.X);
        }

        public void barycentricCoordinates(double x, double y, double[] s)
        {
            Point center = A + (1/3.0) * (B - A) + (1/3.0)* (C - A);

            Vector v1 = perp(C - B); 
            Vector v2 = perp(A - C);
            Vector v3 = perp(B - A);
            Point P = new Point(x,y);
            double alpha = DotProd(P-B, v1) / DotProd(A-B, v1);
            double beta = DotProd(P-C, v2)/DotProd(B-C, v2);
            double gamma = DotProd(P-A, v3) / DotProd(C-A, v3);

            s[0] = alpha;
            s[1] = beta;
            s[2] = gamma;
        }
        public double DotProd(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
        private Point imageCoords(Point LL, Point UR, int imSize, Point P)
        // Return a point with coords between 0 and imSize (if P is within image, of course)
        {
            Vector v = P - LL;
            Vector w = UR - LL;
            return new Point(imSize * v.X / w.X, imSize * v.Y / w.Y);
        }

        void updateDisplayImage()
        {
            Point Ai = imageCoords(lowerLeft, upperRight, sourceSize, Ad.Position);
            Point Bi = imageCoords(lowerLeft, upperRight, sourceSize, Bd.Position);
            Point Ci = imageCoords(lowerLeft, upperRight, sourceSize, Cd.Position);

            double[] s = new double[3];
            for (int i = 0; i < targetSize; i++)
            {
                for (int j = 0; j < targetSize; j++)
                {
                    double alpha = barycentricCoords[i, j, 0];
                    double beta  = barycentricCoords[i, j, 1];
                    double gamma = barycentricCoords[i, j, 2];
                    if ((alpha < 0) || (beta < 0) || (gamma < 0))
                    {
                        continue;
                    }
                    Point T = Ai + beta * (Bi - Ai) + gamma * (Ci - Ai);
                    double u = T.X - 0.5;
                    double v = T.Y - 0.5;

                    int ui = (int) Math.Round(u);
                    int vi = (int) Math.Round(v);
                    if ((ui >= 0) && (ui < sourceSize) && (vi >= 0) && (vi < sourceSize))
                    {
                        if (textureImage[ui, vi] == 0)
                        {
                            displayRects[i, j].Fill = Brushes.Black;
                        }
                        else
                        {
                            displayRects[i, j].Fill = Brushes.White;
                        }
                    }
                }
            }

        }
        void initTextureImage()
        {
            // data at location [i,j] corresponds to pixel at location (i,j), assuming that 
            // image is display w/ lower-left pixel centered at (0,0), so $i$ is the x-coord, and j is the y-coord. 
            for (int i = 0; i < sourceSize; i++)
            {
                for (int j = 0; j < sourceSize; j++)
                {
                    int t = i+j;
                    t = t - 2 * (t / 2);
                    textureImage[i, j] = t;
                }
            }
        }
        void initDisplayImage()
        {
            for (int i = 0; i < targetSize; i++)
            {
                for (int j = 0; j < targetSize; j++)
                {
                    displayImage[i, j] = 2;
                }
            }
        }

 
        void initDraggableTriangle()
        {
            Ad.MakeDraggable(gp);
            Ad.Fill = Brushes.Green;
            Bd.MakeDraggable(gp);
            Bd.Fill = Brushes.Green;
            Cd.MakeDraggable(gp);
            Cd.Fill = Brushes.Green;
            Segment s = new Segment(Ad, Bd);
            s.Stroke = Brushes.Red;
            gp.Children.Add(s);
            s = new Segment(Bd, Cd);
            s.Stroke = Brushes.Red;
            gp.Children.Add(s);
            s = new Segment(Cd, Ad);
            s.Stroke = Brushes.Red;
            gp.Children.Add(s);
            // add dots last so they're easy to click/drag
            gp.Children.Add(Ad);
            gp.Children.Add(Bd);
            gp.Children.Add(Cd);

            DependencyPropertyDescriptor dotDescr = DependencyPropertyDescriptor.FromProperty(Dot.PositionProperty, typeof(Dot));

            if (dotDescr != null)
            {
                dotDescr.AddValueChanged(Ad, OnDotMove);
                dotDescr.AddValueChanged(Bd, OnDotMove);
                dotDescr.AddValueChanged(Cd, OnDotMove);
            } 
            
        }

        protected void OnDotMove(object sender, EventArgs args)
        {
            updateDisplayImage();
        }

    }
}

