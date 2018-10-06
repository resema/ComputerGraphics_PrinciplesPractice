using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace GraphicsBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polygon myPolygon = null;
        GraphPaper gp = null; 
        
  
        // Are we ready for interactions like slider-changes to alter the 
        // parts of our display (like polygons or images or arrows)? Probably not until those things 
        // have been constructed!
        bool ready = false;

        // Code to create and display objects goes here.
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeInteraction();

            // Now add some graphical items in the main Canvas, whose name is "GraphPaper"
            gp = this.FindName("Paper") as GraphPaper;

#region Triangles, segments, dots
            // A triangle, whose top point will be dragged by the slider. 
            myPolygon = new Polygon();
            //myPolygon.Points.Add(new Point(0, 0));
            myPolygon.Stroke = Brushes.Black;
            myPolygon.StrokeThickness = 0.25; // 0.25 mm thick line
            myPolygon.StrokeMiterLimit = 1; // no long pointy bits at vertices
            myPolygon.Fill = Brushes.LightSeaGreen;
            gp.Children.Add(myPolygon);
     
#endregion


            ready = true; // Now we're ready to have sliders and buttons influence the display.
        }

#region Interaction handling -- sliders and buttons
        /* Event handler for a click on button one */
        // Dualize button
        public void b1Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Dualize button clicked!\n");
            if (ready)
            {
                Polygon dual = new Polygon();
                int n = myPolygon.Points.Count;
                for (int i = 0; i < n; i++)
                {
                    double x = myPolygon.Points[i].X + myPolygon.Points[(i + 1) % n].X;
                    double y = myPolygon.Points[i].Y + myPolygon.Points[(i + 1) % n].Y;
                    dual.Points.Add(new Point(x / 2, y / 2));
                }
                dual.Stroke = Brushes.Black;
                dual.StrokeThickness = 0.25; // 0.25 mm thick line
                dual.StrokeMiterLimit = 1; // no long pointy bits at vertices
                dual.Fill = Brushes.LightSeaGreen;
                gp.Children.Remove(myPolygon);
                myPolygon = dual;
                gp.Children.Add(myPolygon);
            }
            e.Handled = true; // don't propagate the click any further
        }

        // Clear button
        public void b2Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Clear button clicked!\n");
            if (ready)
            {
                myPolygon.Points.Clear();
            }
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
            MouseLeftButtonDown += MouseButtonDown;
            MouseLeftButtonUp += MouseButtonUp;
            MouseMove += MouseMoveHandler;
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
#region MouseClick-n-Drag Handling

        public void MouseButtonUp(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseUp at " + ee.GetPosition(this));
            e.Handled = true;
        }

        public void MouseButtonDown(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            if (ready)
            {
                System.Windows.Input.MouseButtonEventArgs ee =
                  (System.Windows.Input.MouseButtonEventArgs)e;
                Debug.Print("MouseDown at " + ee.GetPosition(this));
                myPolygon.Points.Add(ee.GetPosition(gp));
            }
            e.Handled = true;
        }


        public void MouseMoveHandler(object sender, MouseEventArgs e)
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