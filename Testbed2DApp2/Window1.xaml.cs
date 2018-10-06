using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Diagnostics;

namespace GraphicsBook
{
    /// <summary>
    /// Display and interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        GraphPaperAlt gp = null;

        Polygon myTriangle = null;
        GImage myImage1 = null;
        GImage myImage2 = null;
        Mesh myMesh = null;
        Quiver myQuiver = null;

        // Are we ready for interactions like slider-changes to alter the 
        // parts of our display (like polygons or images or arrows)? Probably not until those things 
        // have been constructed!
        bool ready = false;

        // Code to create and display objects goes here.
        public Window1()
        {
            InitializeComponent();
            InitializeCommands();
 
            // Now add some graphical items in the main drawing area, whose name is "Paper"
            gp = this.FindName("Paper") as GraphPaperAlt;
            

            // Track mouse activity in this window
            MouseLeftButtonDown += MyMouseButtonDown;
            MouseLeftButtonUp += MyMouseButtonUp;
            MouseMove += MyMouseMove;

            #region Triangles, segments, dots
            // A triangle, whose top point will be dragged by the slider. 
            myTriangle = new Polygon();
            myTriangle.Points.Add(new Point(0, 10));
            myTriangle.Points.Add(new Point(10, 0));
            myTriangle.Points.Add(new Point(-10, 0));
            myTriangle.Stroke = Brushes.Black;
            myTriangle.StrokeThickness = 1; // 1 mm thick line
            myTriangle.Fill = Brushes.LightSeaGreen;
            gp.Children.Add(myTriangle);

            // A draggable Dot, which is the basepoint of an arrow.
            Dot dd = new Dot(new Point(-40, 60));
            dd.MakeDraggable(gp);
            gp.Children.Add(dd);

            Circle cc = new Circle(20, 20, 20); // This circle wasn't included in the first testbed!
            gp.Children.Add(cc);

            Arrow ee = new Arrow(dd, new Point(10, 10), Arrow.endtype.END);
            gp.Children.Add(ee);

            // a dot and a segment that's attached to it; the dot is animated
            Dot p1 = new Dot(new Point(20, 20));
            gp.Children.Add(p1);
            Point p2 = new Point(50, 50);
            Segment mySegment = new Segment(p1, p2);
            gp.Children.Add(mySegment);

            PointAnimation animaPoint1 = new PointAnimation(
                new Point(-20, -20),
                new Point(-40, 20),
                new Duration(new TimeSpan(0, 0, 5)));
            animaPoint1.AutoReverse = true;
            animaPoint1.RepeatBehavior = RepeatBehavior.Forever;
            p1.BeginAnimation(Dot.PositionProperty, animaPoint1);
            #endregion
            #region Images

            // And a photo from a file; note that because our Y-coordinate increases "up",
            // but WPF Y-coordinates increase "down", the photo shows up inverted. 
            // We'll add a second photo, right-side-up, afterwards, and a third that's 
            // created on-the-fly instead of read from a file. 

            myImage1 = new GImage("foo.jpg");
            myImage1.Width = GraphPaper.wpf(200);
            myImage1.Position = new Point(10, 40);
            
            gp.Children.Add(myImage1);

            // Create source
            // Now add a second image, based on first building an array of color values
            // Create source array
            byte[, ,] stripes = createStripeImageArray();

            myImage2 = new GImage(stripes);

            // Establish the width and height for this image on the GraphPaper
            myImage2.Width = GraphPaper.wpf(128);
            myImage2.Height = GraphPaper.wpf(128);

            myImage2.Position = new Point(-40, 20);
            gp.Children.Add(myImage2);

            #endregion
            #region Mesh, Quiver, and Text labels

            myMesh = this.createSampleMesh();
            gp.Children.Add(myMesh);

            Text myText = new Text("THIS IS TEXT", false); // the second arg says that y increases down
            myText.Position = new Point(20, 50);
            gp.Children.Add(myText);

            Text myText2 = new Text("THIS IS UPSIDE DOWN TEXT, because we didn't set the yUp flag to false"); 
            myText2.Position = new Point(-20, -50);
            gp.Children.Add(myText2);

            myQuiver = makeQuiver();
            foreach (Shape q in myQuiver)
            {
                gp.Children.Add(q);
            }

            #endregion
            ready = true; // Now we're ready to have sliders and buttons influence the display.
        }

        #region Interaction handling -- sliders and buttons

        /* Click-handling in the main graph-paper window */
        public void MyMouseButtonUp(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseUp at " + ee.GetPosition(this));
            e.Handled = true;
        }

        public void MyMouseButtonDown(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseDown at " + ee.GetPosition(this));
            e.Handled = true;
        }


        public void MyMouseMove(object sender, MouseEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseEventArgs ee =
              (System.Windows.Input.MouseEventArgs)e;
            // Uncommment following line to get a flood of mouse-moved messages. 
            // Debug.Print("MouseMove at " + ee.GetPosition(this));
            e.Handled = true;
        }

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
                PointCollection p = myTriangle.Points.Clone();
                Debug.Print(myTriangle.Points.ToString());
                Point u = p[0];
                u.X = e.NewValue;
                p[0] = u;
                myTriangle.Points = p;
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
            ExitCommand = new RoutedCommand("Exit", typeof(Window1), inp);
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

        #region Image, Mesh, and Quiver, construction helpers
        private byte[, ,] createStripeImageArray()
        {
            int width = 128;
            int height = 128;
            byte[, ,] pixelArray = new byte[width, height, 4];

            for (int y = 0; y < height; ++y)
            {
                int yIndex = y * width;
                for (int x = 0; x < width; ++x)
                {
                    byte b = (byte)(32 * (Math.Round((x + 2 * y) / 32.0)));
                    pixelArray[x, y, 0] = b;
                    pixelArray[x, y, 1] = b;
                    pixelArray[x, y, 2] = b;
                    pixelArray[x, y, 3] = 255;
                }
            }
            return pixelArray;
        }

        private int vectorIndex(int row, int col, int nrows, int ncols)
        {
            return col + row * ncols;
        }

        //        private Path createSampleMesh()
        private Mesh createSampleMesh()
        {
            int nrows = 4;
            int ncols = 6;
            int nverts = nrows * ncols;
            int nedges = nrows * (ncols - 1) + ncols * (nrows - 1);
            int baseX = -40;
            int baseY = 55;
            Point[] verts = new Point[nverts];
            int[,] edges = new int[nedges, 2];

            for (int y = 0; y < nrows; y++)
            {
                for (int x = 0; x < ncols; x++)
                {
                    verts[vectorIndex(y, x, nrows, ncols)] =
                        new Point(baseX + 10 * x, baseY + 10 * y + 5 * Math.Sin(2 * Math.PI * x / (ncols - 1)));
                }
            }

            int count = 0;
            for (int y = 0; y < nrows; y++)
            {
                for (int x = 0; x < ncols - 1; x++)
                {
                    edges[count, 0] = vectorIndex(y, x, nrows, ncols);
                    edges[count, 1] = vectorIndex(y, x + 1, nrows, ncols);
                    count++;
                }
            }
            for (int x = 0; x < ncols; x++)
            {
                for (int y = 0; y < nrows - 1; y++)
                {
                    edges[count, 0] = vectorIndex(y, x, nrows, ncols);
                    edges[count, 1] = vectorIndex(y + 1, x, nrows, ncols);
                    count++;
                }
            }
            Debug.Print("count = " + count + "\n");
            return new Mesh(nverts, count, verts, edges);
        }

        private Quiver makeQuiver()
        {
            int count = 10;
            Point[] verts = new Point[count];
            Vector[] arrows = new Vector[count];
            for (int i = 0; i < count; i++)
            {
                double th = 2 * Math.PI * i / count;
                verts[i] = new Point(-40 + 5 * Math.Cos(th), -40 + 5 * Math.Sin(th));
                arrows[i] = new Vector(20 * Math.Cos(th), 20 * Math.Sin(th));
            }
            return new Quiver(verts, arrows);
        }
        #endregion
    }
}