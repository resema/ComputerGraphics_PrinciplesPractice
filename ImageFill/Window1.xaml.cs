
using System;
using System.Windows;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;

using System.Diagnostics;

namespace GraphicsBook
{
    /// <summary>
    /// Display and interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        GraphPaper gp = null;

        GImage original = null;
        GImage damaged = null;
        GImage restored = null;
        Text myText = null;
        int nRows;
        int nCols;
        bool[,] knownPixels = null;
        Random myRandom = new Random();


        int improvementCount = 0;
        double damageFraction = 0.0d;
        double blendRatio = 1.0d; 

        // Are we ready for interactions like slider-changes to alter the 
        // parts of our display (like polygons or images or arrows)? Probably not until those things 
        // have been constructed!
        bool ready = false;

        // Code to create and display objects goes here.
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

            
            #region Images

            initializeImages();

            gp.Children.Add(original);
            gp.Children.Add(damaged);
            gp.Children.Add(restored);
            #endregion
            #region Text label

            myText = new Text("Improvements: " + improvementCount.ToString());
            myText.Position = new Point(5, 5);
            gp.Children.Add(myText);

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
        public void ImproveClick(object sender, RoutedEventArgs e)
        {
            Debug.Print("Button one clicked!\n");
            improvementCount++;
            myText.Text = "Improvements: " + improvementCount.ToString();
            improveRestoredImage();
            e.Handled = true; // don't propagate the click any further
        }

        public void RestartClick(object sender, RoutedEventArgs e)
        {
            Debug.Print("Button two clicked!\n");
            improvementCount = 0;
            updateDamagedImage();
            updateRestoredImage();
            myText.Text = "Improvements: " + improvementCount.ToString();
            e.Handled = true; // don't propagate the click any further
        }

        void slider1change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Print("Slider changed, ready = " + ready + ", and val = " + e.NewValue + ".\n");
            e.Handled = true;
            if (ready)
            {
                improvementCount = 0;
                myText.Text = "Improvements: " + improvementCount.ToString();

                damageFraction = e.NewValue / 100;
                updateDamagedImage();
                updateRestoredImage();
            }
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

        #region Image construction and update helpers

        protected void initializeImages()
        {
            getOriginalGrayscaleImage();
            original.Position = new Point(-35, 5);
            knownPixels = new bool[nRows, nCols];
            buildDamagedImage();
            damaged.Position = new Point(-70, -95);
            buildRestoredImage();
            restored.Position = new Point(5, -95);
        }

        protected void getOriginalGrayscaleImage()
        {
            original = new GImage("images/mona2.jpg");

            byte[, ,] pixels = original.GetPixelArray();
            nRows = original.PixelHeight();
            nCols = original.PixelWidth();
            byte[, ,] gPixelArray = new byte[nRows, nCols, 4];
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    int grayValue = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        grayValue += pixels[i, j, k];
                    }
                    grayValue /= 3;
                    for (int k = 0; k < 3; k++)
                    {
                        gPixelArray[i, j, k] = (byte) grayValue;
                    }
                    gPixelArray[i, j, 3] = 255;
                }
            }
            original.SetPixelArray(gPixelArray, nRows, nCols);
        }

        protected void buildDamagedImage()
        {
            byte[] pixelVector = new byte[nRows * nCols * 4];
            damaged = new GImage(nRows, nCols, pixelVector);
            damaged.Position = new Point(-95.0, 0.0);
            updateDamagedImage();
        }

        protected void updateDamagedImage()
        {
            byte[, ,] pixels = original.GetPixelArray();
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    
                    if (myRandom.NextDouble() < damageFraction)
                    {
                        knownPixels[i, j] = false;
                        pixels[i, j, 0] = 0;
                        pixels[i, j, 1] = 0;
                        pixels[i, j, 2] = 255;
                        pixels[i, j, 3] = 255;
                    }
                    else
                    {
                        knownPixels[i, j] = true;
                    }
                }
            }
            damaged.SetPixelArray(pixels, nRows, nCols);
        }
        
        protected void improveRestoredImage()
        {
            byte[, ,] originalPixels = original.GetPixelArray();
            byte[, ,] pixels = restored.GetPixelArray();
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if (knownPixels[i, j])
                    {
                        pixels[i, j, 0] = originalPixels[i, j, 0];
                        pixels[i, j, 1] = originalPixels[i, j, 0];
                        pixels[i, j, 2] = originalPixels[i, j, 0];
                        pixels[i, j, 3] = 255;
                    }
                    else
                    {
                        double neighborSum = 0;
                        int neighborCount = 0;
                        getNeighbors(pixels, i, j, out neighborSum, out neighborCount);
                        double val = neighborSum / neighborCount;
                        pixels[i, j, 0] = (byte) Math.Round(((1 - blendRatio) * pixels[i, j, 0] + blendRatio * val));
                        pixels[i, j, 1] = pixels[i, j, 0];
                        pixels[i, j, 2] = pixels[i, j, 0];
                        pixels[i, j, 3] = 255;
                    }
                }
            }
            restored.SetPixelArray(pixels, nRows, nCols);
        }



        protected void buildRestoredImage()
        {
            byte[] pixelVector = new byte[nRows * nCols * 4];
            restored = new GImage(nRows, nCols, pixelVector);
            updateRestoredImage();
        }

        protected void updateRestoredImage()
        {
            byte[, ,] pixels = restored.GetPixelArray();

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    pixels[i, j, 0] = 0;
                    pixels[i, j, 1] = 0;
                    pixels[i, j, 2] = 0;
                    pixels[i, j, 3] = 255;
                }
            }
            restored.SetPixelArray(pixels, nRows, nCols);
        }



        protected void getNeighbors(byte[, ,] pixels, int i, int j, out double neighborSum, out int neighborCount)
        {
            neighborCount = 0;
            neighborSum = 0.0d;
            if ((i > 0))
            {
                neighborCount++;
                neighborSum += pixels[i - 1, j, 0];
            }
            if ((j > 0))
            {
                neighborCount++;
                neighborSum += pixels[i, j-1, 0];
            }
            if ((i+1 < nRows))
            {
                neighborCount++;
                neighborSum += pixels[i + 1, j, 0];
            }

            if ((j+1 < nCols))
            {
                neighborCount++;
                neighborSum += pixels[i, j+1, 0];
            }

        }
                        
        #endregion Image construction and update helpers
    #endregion
    }
}