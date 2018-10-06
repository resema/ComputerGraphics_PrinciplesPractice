using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics; 

namespace GraphicsBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polygon myPolygon = new Polygon();
        Polygon mySubdivPolygon = new Polygon();
        bool isSubdivided = false;
        GraphPaper gp = null;   

        // Are we ready for interactions like slider-changes to alter the 
        // parts of our display (like polygons or images or arrows)? Probably not until those things 
        // have been constructed!
        bool ready = false;
        // Code to create and display objects goes here.
 
        /// <summary>
        /// Create a window containing a polygon (with no vertices) to which the user can ad verts with 
        /// left-clicks. A click on the "subdivide" button makes a subdivided polygon appear in dark red, 
        /// with the original in black. Subsequent clicks make the red polygon black, and create a new 
        /// sub-sub-divided polygon, and so on. 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeInteraction();

            // Now add some graphical items in the main Canvas, whose name is "GraphPaper"
            gp = this.FindName("Paper") as GraphPaper;
            initPoly(myPolygon, Brushes.Black);
            initPoly(mySubdivPolygon, Brushes.Firebrick);
            gp.Children.Add(myPolygon);
            gp.Children.Add(mySubdivPolygon);

            ready = true; // Now we're ready to have sliders and buttons influence the display.
        }
        /// <summary>
        /// Initialize the polygon p to have a standard stroke-thickness and mitered corners,
        /// and give it the Stroke style specified by the brush b. 
        /// </summary>
        /// <param name="p">A polygon whose properties we set</param>
        /// <param name="b">The stroke style to use</param>
        private void initPoly(Polygon p, SolidColorBrush b)
        {
            p.Stroke = b;
            p.StrokeThickness = 0.5; // 0.25 mm thick line
            p.StrokeMiterLimit = 1; // no long pointy bits at vertices
            p.Fill = null;
        }

#region Interaction handling
        /// <summary>
        /// Handle clicks on the "subdivide" button. If the current polygon has been subdivided, 
        /// make the subdivided polygon the current one, and create a more finely subdivided one to be the "subdivided" 
        /// polygon. 
        /// 
        /// If the current polygon has not been subdivided, then subdivide it (assuming it has more than zero
        /// points), and set "isSubdivided" to true, so that further left-clicks are disabled. 
        /// 
        /// Assign colors to the current and subdivided polygons as well. 
        /// </summary>
        /// <param name="sender">The "Subdivide" button</param>
        /// <param name="e">The click-event</param>
        public void b1Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Subdivide button clicked!\n");
            if (isSubdivided)
            {
                myPolygon.Points = mySubdivPolygon.Points;
                mySubdivPolygon.Points = new PointCollection();
            }

            int n = myPolygon.Points.Count;
            if (n > 0)
            {
                isSubdivided = true;
            }
        
            for (int i = 0; i < n; i++)
            {
                int lasti = (i + (n - 1)) % n ; // index of previous point
                int nexti = (i + 1) % n; // index of next point.
                double x = (1.0f / 3.0f) * myPolygon.Points[lasti].X + (2.0f / 3.0f) * myPolygon.Points[i].X;
                double y = (1.0f / 3.0f) * myPolygon.Points[lasti].Y + (2.0f / 3.0f) * myPolygon.Points[i].Y;
                mySubdivPolygon.Points.Add(new Point(x, y));
            
                x = (1.0f / 3.0f) * myPolygon.Points[nexti].X + (2.0f / 3.0f) * myPolygon.Points[i].X;
                y = (1.0f / 3.0f) * myPolygon.Points[nexti].Y + (2.0f / 3.0f) * myPolygon.Points[i].Y;
                mySubdivPolygon.Points.Add(new Point(x, y));
            }
            e.Handled = true; // don't propagate the click any further
        }

        // Clear button
        /// <summary>
        /// Handle clicks on the "Clear" button: set isSubdivided to false, and remove both the current
        /// polygon and the subdivided one (in the sense of removing all their vertices). 
        /// </summary>
        /// <param name="sender">The "Clear" button</param>
        /// <param name="e">The click-event</param>
        public void b2Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Clear button clicked!\n");
            
            myPolygon.Points.Clear();
            mySubdivPolygon.Points.Clear();
            isSubdivided = false;
            
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

        protected void InitializeInteraction()
        {
            MouseLeftButtonDown += MouseButtonDownA;
            MouseLeftButtonUp += MouseButtonUpA;
            MouseMove += RESPOND_MouseMoveA;
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
#region Mouse Event Handling
        
        public void MouseButtonUpA(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseUp at " + ee.GetPosition(this));
            e.Handled = true;
        }

        /// <summary>
        /// Handle a left mouse-click by adding a new vertex to the current polygon, as long as 
        /// no subdivision has yet occured
        /// </summary>
        /// <param name="sender">The GraphPaper object</param>
        /// <param name="e">The mouse-click event</param>
        public void MouseButtonDownA(object sender, RoutedEventArgs e)
        {
            Debug.Print("Mouse down");
            if (ready)
            {
                if (sender != this) return;
                System.Windows.Input.MouseButtonEventArgs ee =
                   (System.Windows.Input.MouseButtonEventArgs)e;
                Debug.Print("MouseDown at " + ee.GetPosition(this));
                if (!isSubdivided)
                {
                    myPolygon.Points.Add(ee.GetPosition(gp));
                }
            }
            e.Handled = true;
        }


        public void RESPOND_MouseMoveA(object sender, MouseEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseEventArgs ee =
              (System.Windows.Input.MouseEventArgs)e;
            // Uncommment following line to get a flood of mouse-moved messages. 
            // Debug.Print("MouseMove at " + ee.GetPosition(this));

            e.Handled = true;
        }
#endregion

    }
}