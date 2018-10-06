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
    /// Display and interaction logic for Window1.xaml, a three-point-circle-drawing program. 
    /// </summary>
    public partial class Window1 : Window
    {
        GraphPaper gp = null;

        Dot[] myDots = null; 
        Random autoRand = new System.Random( );
        Circle myCircle = null;

       
        /// <summary>
        /// Create and display the objects (a set of axes, three points, and a circle passing through the three points).  
        /// </summary>
        public Window1()
        {
            InitializeComponent();
            InitializeCommands();
            // Now add some graphical items in the main Canvas, whose name is "Paper"
            gp = this.FindName("Paper") as GraphPaper;
            

            // Track mouse activity in this window
            MouseLeftButtonDown += MyMouseButtonDown;
            MouseLeftButtonUp += MyMouseButtonUp;
            MouseMove += MyMouseMove;

            myDots = new Dot[3];
            myDots[0] = new Dot(new Point(-40, 60));
            myDots[1] = new Dot(new Point(40, 60));
            myDots[2] = new Dot(new Point(40, -60));
            for (int i = 0; i < 3; i++)
            {
                myDots[i].MakeDraggable(gp);
                gp.Children.Add(myDots[i]);
            }
            myCircle = new Circle(new Point(0, 0), 0.0);
            updateCircle(myDots[0].Position, myDots[1].Position, myDots[2].Position);
            gp.Children.Add(myCircle);
        }

        /// <summary>
        /// Find a circle passing through P, Q, and R, if possible. If the circle is a straight line, or two 
        /// points are too nearly identical, jiggle the points slightly to get a very nearly correct circle.
        /// </summary> 
        protected void updateCircle(Point P, Point Q, Point R)
        {
            if ((P.X == Q.X) & (P.Y == Q.Y))
            {
                P.X += .001 * (2 * autoRand.NextDouble() - 1);
                P.Y += .001 * (2 * autoRand.NextDouble() - 1);
            }
            if ((Q.X == R.X) & (Q.Y == R.Y))
            {
                R.X += .001 * (2 * autoRand.NextDouble() - 1);
                R.Y += .001 * (2 * autoRand.NextDouble() - 1);
            }
            Point A = P + (Q - P) / 2.0;
            Point B = Q + (R - Q) / 2.0;
            Vector v = Q - P;
            double tmp = v.X;
            v.X = v.Y;
            v.Y = -tmp;
            v.Normalize();
            Vector w = R - Q;
            w.Normalize();
            tmp = w.X;
            w.X = w.Y;
            w.Y = -tmp;
            // Want to solve [v; w][t, -s]' = B - A, where [v; w] is a matrix whose columns are v and w. If we call 
            // that [a  b]
            //      [c  d]
            // then its inverse is
            //   k . [d  -b]
            //       [-c  a]
            // where k = 1/ (ad - bc). We can multiply this by the vector B-A to get t and -s. 
            double a = v.X; double b = w.X; double c = v.Y; double d = w.Y;
            double det = a * d - b * c;
            // If the det is too small (or zero), we can fix it by moving the x-coordinates of v and w by 1/1000, 
            // which is too small to have any visual impact. 
            if (Math.Abs(det) < 1e-9)
            {
                a += .001;
                c += .001;
            }
            Vector target = (P - R) / 2.0;
            double t =  -(  d * target.X - b * target.Y);
            double s =   (-c * target.X + a * target.Y);
            t /= det;
            Point C = A + t * v;
            double r = (P - C).Length;
            myCircle.Position = C;
            myCircle.Radius = r;
        }


        #region Interaction handling -- sliders and buttons

        /* Click-handling in the main graph-paper window */
        /// <summary>
        /// Detect mouse-release events on the graph-paper and do nothing with them. 
        /// </summary> 
        public void MyMouseButtonUp(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseUp at " + ee.GetPosition(this));
            e.Handled = true;
        }

        /// <summary>
        /// Detect mouse-click events on the graph-paper and do nothing with them. 
        /// </summary> 
        public void MyMouseButtonDown(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseDown at " + ee.GetPosition(this));
            e.Handled = true;
        }


        /// <summary>
        /// Detect mouse-move events on the graph-paper and do nothing with them. 
        /// </summary> 
        public void MyMouseMove(object sender, MouseEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseEventArgs ee =
              (System.Windows.Input.MouseEventArgs)e;
            // Uncommment following line to get a flood of mouse-moved messages. 
            // Debug.Print("MouseMove at " + ee.GetPosition(this));
            e.Handled = true;
        }

        /// <summary>
        /// When the "Redraw Circle" button is clicked, recompute the center and radius of the circle,
        /// prompting it to be redrawn. 
        /// </summary> 
        public void DrawCircleClick(object sender, RoutedEventArgs e)
        {
            Debug.Print("RedrawCircle clicked!\n");
            e.Handled = true; // don't propagate the click any further
            updateCircle(myDots[0].Position, myDots[1].Position, myDots[2].Position);
        }

        /// <summary>
        /// When the "Reset" button is clicked, place the dots in their initial position, 
        /// and recompute the center and radius of the circle,
        /// prompting all to be redrawn. 
        /// </summary> 
        public void ResetClick(object sender, RoutedEventArgs e)
        {
            Debug.Print("Reset button clicked!\n");
            e.Handled = true; // don't propagate the click any further
            myDots[0].Position = new Point(-40,  60);
            myDots[1].Position = new Point( 40,  60);
            myDots[2].Position = new Point( 40, -60);
            updateCircle(myDots[0].Position, myDots[1].Position, myDots[2].Position);
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
    }
}