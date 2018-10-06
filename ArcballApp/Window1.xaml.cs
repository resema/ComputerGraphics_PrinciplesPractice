using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace GraphicsBook
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private RotateTransform3D m_cubeRotation = new RotateTransform3D();
        private ModelVisual3D m_cube1;
        private Interactor _interactor = null;

        private static readonly Color s_cube1Color = Colors.Orange;

        /// <summary>
        /// Build a scene with a floor, a wall, and cube. Set up interaction behavior so that a right-click 
        /// on the cube lets the user click and drag on a sphere to rotate the cube. 
        /// </summary>
        public Window1()
        {
            InitializeComponent();

            // ground plane
            mainViewport.Children.Add(CreatePlane(50, ShapeGenerator.GetSimpleMaterial(Colors.LightGray)));

            // wall plane
            ModelVisual3D rightWall = CreatePlane(50, ShapeGenerator.GetSimpleMaterial(Colors.Red));
            rightWall.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
            mainViewport.Children.Add(rightWall);

            // wall plane
            ModelVisual3D leftWall = CreatePlane(50, ShapeGenerator.GetSimpleMaterial(Colors.Green));
            leftWall.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -90));
            mainViewport.Children.Add(leftWall);

            // create a colored cube and place it in the scene
            m_cube1 = ShapeGenerator.GenerateUnitCube(ShapeGenerator.GetSimpleMaterial(s_cube1Color));
            m_cube1.Transform = new TranslateTransform3D(4, .5, 1);
            mainViewport.Children.Add(m_cube1);
            
            this.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(Window1_MouseRightButtonDown);
            this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(Window1_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(Window1_MouseLeftButtonUp);
            this.MouseMove += new System.Windows.Input.MouseEventHandler(Window1_MouseMove);
        }


        /// <summary>
        /// Check whether there's an Arcball present, and if so, let it handle the mouse-click.
        /// </summary>
        void Window1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_interactor != null)    
                _interactor.mouseLeftButtonDown(e);
        }

        /// <summary>
        /// Check whether there's an Arcball present, and if so, let it handle the mouse-move.
        /// </summary>
        void Window1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_interactor != null)
                _interactor.mouseMove(e);
        }

        /// <summary>
        /// Check whether there's an Arcball present, and if so, let it handle the mouse-release.
        /// </summary>
        void Window1_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_interactor != null)
                _interactor.mouseLeftButtonUp(e);
        }

        /// <summary>
        /// Check whether the right-click hit the cube. If so, toggle the presense of an Arcball interactor. 
        /// </summary>
        
        void Window1_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Check to see if the user clicked on cube1. If so, create a sphere around it.
            ModelVisual3D hit = GetHitTestResult(e.GetPosition(mainViewport));
            if (hit == m_cube1)
            {
                Debug.Print("Clicked Cube 1!");
                if (_interactor == null)
                {
                    _interactor = new Interactor(m_cube1, mainViewport, this);
                }
                else
                {
                    endInteraction();
                }
            }
            else if (_interactor != null)
            {
                _interactor.mouseRightButtonDown(e);
            }
        }

        public void endInteraction()
        {
            _interactor.Cleanup();
            _interactor = null;
        }

        /// <summary>
        /// If "location" is in the visible projection of some ModelVisual3D on the image plane, 
        /// return that object; else return null. 
        /// </summary>
        ModelVisual3D GetHitTestResult(Point location)
        {
            ModelVisual3D visual = null; // the object we hit

            HitTestResult result = VisualTreeHelper.HitTest(mainViewport, location);            
            if (result != null && result.VisualHit is ModelVisual3D)
            {
                visual = (ModelVisual3D)result.VisualHit;
            }
            return visual;
        }

        /// <summary>
        /// Creates a square plane of the given size with the given material for texture
        /// </summary>
        /// <param name="size">Size of the plane (should be a relatively large number)</param>
        /// <param name="material">texture used for the entire plane</param>
        /// <returns></returns>
        private static ModelVisual3D CreatePlane(double size, Material material)
        {
            ModelVisual3D plane = new ModelVisual3D();
            plane.Content = ShapeGenerator.CreateQuad(new Point3D(size, 0, -size),
                                                      new Point3D(-size, 0, -size),
                                                      new Point3D(-size, 0, size),
                                                      new Point3D(size, 0, size),
                                                      material);
            return plane;
        }
    }
}
