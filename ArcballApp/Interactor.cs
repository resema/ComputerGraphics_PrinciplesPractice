using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace GraphicsBook
{
    // An object to represent an arcball interaction. 
    // Upon construction, displays a transparent sphere
    // Click-and-drag operations on that sphere result in arcball interactions. 
    // 

    class Interactor
    {
        private Point3D _startPoint = new Point3D(0,0,0); //location where current interaction started (or re-started,in case of new/lost contact)
        private Transform3D _initialTransform = null; // render transform for _controlled at start (or restart) of interaction
        private Viewport3D _viewport3D;           // the viewport in which the arcball resides.
        private ModelVisual3D _controlled;        // the object being controlled by the arcball. 
        private ModelVisual3D _sphere = null;     // the geometry of the ball.
        private Window1 _window1;                 // The Window1 in which the viewport3D sits (i.e., the main window for this program)
        private const double _radius = 2.5;       // Radius of interaction sphere.

        private Boolean _inDrag = false;          // True if the arcball is being dragged by the mouse.

        /// <summary>
        /// Create an arcball interactor to control the "controlled" object in the given Viewport3D, which is 
        /// part of the given Window1.  
        /// </summary>
        public Interactor(ModelVisual3D controlled, Viewport3D vp, Window1 w)
        {
            _viewport3D = vp;
            _window1 = w;
            _controlled = controlled;
            initializeInteraction();
        }

        /// <summary>
        /// A right button-click means that we're done rotating. The right-button click that indicates that we're 
        /// ready to START rotating is handled by the Window1 mouse code. 
        /// </summary>
        public void mouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            _window1.endInteraction();
        }

        /// <summary>
        /// If the mouse-click was on the arcball, initiate a rotation-interaction; otherwise ignore the click.  
        /// </summary>
        public void mouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            // start a drag-process:
            //    Record the initial click-point
            //    Recode the current object transformation
            //    Set _inDrag = true
            ModelVisual3D hit = GetHitTestResult(e.GetPosition(_viewport3D));
            // hit should be sphere!
            if (hit != _sphere)
            {
                Debug.Print("Left-click outside sphere");
            }
            else
            {
                Debug.Print("Hit start");
                if (!_inDrag)
                {
                    _startPoint = spherePointFromMousePosition(e.GetPosition(_viewport3D));
                    _initialTransform = _controlled.Transform;
                    _inDrag = true;
                }
            }
        }

        /// <summary>
        /// At the end of a mouse-drag, stop the interaction process. Even if _inDrag is false already, this 
        /// does no harm. 
        /// </summary>
        public void mouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            _inDrag = false;
        }

        /// <summary>
        /// Update the transformation on the controlled object as the dragged-mouse moves. (Ignore if the motion's not part of a drag). 
        /// 
        /// </summary>
        public void mouseMove(System.Windows.Input.MouseEventArgs e)
        {
            // if not in drag, ignore
            // if in drag, 
            //   note that current point may not lie on sphere!
            //   compute rotation taking start-point to current point
            //   concatenate that with initial transform

            if (_inDrag)
            {
                Debug.Print("In drag!");
                Point3D currPoint = spherePointFromMousePosition(e.GetPosition(_viewport3D));
                Point3D origin = new Point3D(0, 0, 0);
                GeneralTransform3D tt = _initialTransform.Inverse;

                Vector3D vec1 = tt.Transform(_startPoint) - tt.Transform(origin);
                Vector3D vec2 = tt.Transform(currPoint) - tt.Transform(origin);
                Debug.Print("  vec1: " + vec1.ToString());
                Debug.Print("  vec2: " + vec2.ToString());
                vec1.Normalize();
                vec2.Normalize();
                // Now build an arcball interaction to take vec1 to vec2 and past it (i.e., double the rotation):

                double angle = 2*Math.Acos(Vector3D.DotProduct(vec1, vec2));
                Vector3D axis = Vector3D.CrossProduct(vec1, vec2);
                RotateTransform3D rotateTransform = new RotateTransform3D();
                rotateTransform.Rotation = new AxisAngleRotation3D(axis, 180 * angle/Math.PI);
                Debug.Print("axis: " + axis.ToString());
                Debug.Print("angle: " + angle.ToString());
               
                Transform3DGroup tg = new Transform3DGroup();
                tg.Children.Add(rotateTransform);
                tg.Children.Add(_initialTransform);

                _controlled.Transform = tg;
            }
        }

        /// <summary>
        /// If the ray from the eye through the mouse-click meets the interaction
        /// sphere, return the first intersection. If not, returns the sphere 
        /// point closest to that ray. 
        /// </summary>
        /// <param name="mousePoint"></param>
        /// <returns></returns>
        private Point3D spherePointFromMousePosition(Point mousePoint)
        {
            HitTestResult result = VisualTreeHelper.HitTest(_viewport3D, mousePoint);
            if (result.VisualHit == _sphere)
            {
                RayHitTestResult rayResult = result as RayHitTestResult;
                if (rayResult != null)
                {
                    RayMeshGeometry3DHitTestResult rayMeshResult =
                        rayResult as RayMeshGeometry3DHitTestResult;
                    return rayMeshResult.PointHit;
                }
                else
                {
                    throw new Exception("hit sphere but missed sphere somehow");
                }
            }
            else // ray misses sphere
            {
                Point3D eye = ((PerspectiveCamera)_viewport3D.Camera).Position;

                return new Point3D(0, 0.5, 0);
            }
        }

        /// <summary>
        /// Prepare for arcball interaction. We must build the spherical arcball around the object center, and 
        /// record the transform on the controlled object, so that we can place our rotate AFTER that. 
        /// </summary>
        private void initializeInteraction()
        {
            _initialTransform = _controlled.Transform;

            Rect3D bounds = _controlled.Content.Bounds;
            Point3D center = new Point3D(bounds.X + bounds.SizeX / 2, bounds.Y + bounds.SizeY / 2, bounds.Z + bounds.SizeZ / 2);
            Point3D worldCenter = _controlled.Transform.Transform(center);

            _sphere = ShapeGenerator.GenerateUnitSphere(30, 30, CreateInterestingTexture("Sphere", Color.FromArgb(96, 0, 0, 255)));

            Transform3DGroup sphereTransforms = new Transform3DGroup();
            sphereTransforms.Children.Add(new ScaleTransform3D(_radius*2,_radius*2,_radius*2));
            sphereTransforms.Children.Add(new TranslateTransform3D(worldCenter - (new Point3D(0,0,0))));
            _sphere.Transform = sphereTransforms;
                        
            _viewport3D.Children.Add(_sphere);
        }

        /// <summary>
        /// Remove the interactor-sphere from the scene in preparation for deletion of this interactor.
        /// </summary>
        public  void Cleanup()
        {
            _viewport3D.Children.Remove(_sphere);
            _initialTransform = null;
        }

        /// <summary>
        /// Create an "interesting" material for texture mapping.
        /// </summary>
        public static MaterialGroup CreateInterestingTexture(string text, Color c)
        {
            MaterialGroup texturedMaterial = new MaterialGroup();
            texturedMaterial.Children.Add(ShapeGenerator.GetSimpleMaterial(c));
            DiffuseMaterial textMaterial = new DiffuseMaterial();

            return texturedMaterial;
        }

        /// <summary>
        /// If "location" is in the visible projection of some ModelVisual3D on the image plane, 
        /// return that object; else return null. 
        /// </summary>
        ModelVisual3D GetHitTestResult(Point location)
        {
            HitTestResult result = VisualTreeHelper.HitTest(_viewport3D, location);
            if (result != null && result.VisualHit is ModelVisual3D)
            {
                ModelVisual3D visual = (ModelVisual3D)result.VisualHit;
                return visual;
            }
            return null;
        }
    }
}
