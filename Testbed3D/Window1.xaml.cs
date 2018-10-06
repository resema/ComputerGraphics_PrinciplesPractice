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
        bool ready = false; // are we ready for slider-changes, etc?
        ModelVisual3D sphere;
        int latSteps = 10; // number of steps in grid for sphere mesh, latitude
        int lonSteps = 10; // number of steps in grid for sphere mesh, latitude
        MaterialGroup texturedMaterial = new MaterialGroup();
        MaterialGroup boringMaterial = new MaterialGroup();
        bool isTextured = true; 

        public Window1()
        {
            InitializeComponent();

            // Create an "interesting" material for texture mapping
            texturedMaterial.Children.Add(ShapeGenerator.GetSimpleMaterial(Colors.LightSalmon));
            DiffuseMaterial textMaterial = new DiffuseMaterial();
            TextBlock text = new TextBlock();
            text.Foreground = Brushes.Black;
            text.FontSize = 8;
            text.Text = "&";
            text.FontWeight = FontWeights.Bold;
            textMaterial.Brush = new VisualBrush(text);
            texturedMaterial.Children.Add(textMaterial);

            // Create a "boring" alternative material for texture mapping
            boringMaterial.Children.Add(ShapeGenerator.GetSimpleMaterial(Colors.LightGreen));

            // create sphere with fun texture
            sphere = ShapeGenerator.GenerateUnitSphere(30, 30, texturedMaterial);
            sphere.Transform = new TranslateTransform3D(1, 1, 1);
            mainViewport.Children.Add(sphere);

            // create a cube with fun texture
            ModelVisual3D cube1 = ShapeGenerator.GenerateUnitCube(texturedMaterial);
            cube1.Transform = new TranslateTransform3D(4, .5, 1);
            mainViewport.Children.Add(cube1);

            // create a cube with simple texture
            ModelVisual3D cube2 = ShapeGenerator.GenerateUnitCube(ShapeGenerator.GetSimpleMaterial(Colors.Blue));
            cube2.Transform = new TranslateTransform3D(1, .5, 3);
            mainViewport.Children.Add(cube2);

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
            ready = true;
        }

        void slider1change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Print("Slider changed, ready = " + ready + ", and val = " + e.NewValue + ".\n");
            e.Handled = true;
            if (ready)
            {
                latSteps = (int)e.NewValue;

                mainViewport.Children.Remove(sphere);
                sphere = ShapeGenerator.GenerateUnitSphere(latSteps, lonSteps, texturedMaterial);
                sphere.Transform = new TranslateTransform3D(1, 1, 1);
                mainViewport.Children.Add(sphere);
            }
        }

        void slider2change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Print("Slider changed, ready = " + ready + ", and val = " + e.NewValue + ".\n");
            e.Handled = true;
            if (ready)
            {
                lonSteps = (int)e.NewValue;

                mainViewport.Children.Remove(sphere);
                sphere = ShapeGenerator.GenerateUnitSphere(latSteps, lonSteps, texturedMaterial);
                sphere.Transform = new TranslateTransform3D(1, 1, 1);
                mainViewport.Children.Add(sphere);
            }
        }

        public void b1Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Button two clicked!\n");
            e.Handled = true; // don't propagate the click any further
            if (isTextured)
            {
                ((GeometryModel3D)sphere.Content).Material = boringMaterial;
                isTextured = false;
            }
            else
            {
                ((GeometryModel3D)sphere.Content).Material = texturedMaterial;
                isTextured = true;
            }
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
