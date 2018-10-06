using System;
using System.Windows;
using System.Windows.Media;

namespace GraphicsBook
{
    /// <summary>
    ///     An Interactor represents the contact between a hand (one or two fingers) and a photo in the photo-sorting app. 
    ///     It is associated with a controlled object (the photo), and contains two Contacts, one of which may be null; when 
    ///     the other is deleted, the Interactor tells its parent to delete the Interactor. 
    /// </summary>

    class Interactor
    {
        private Contact _c1 = null;
        private Contact _c2 = null;
        private PhotoDisplay _photoDisplay = null;
        private FrameworkElement _controlled = null;
        private Point _startPoint = new Point(0,0); //location where current interaction started (or re-started,in case of new/lost contact)
        private Vector _startVector = new Vector(0, 0); // difference between initial contacts for two-point motion.
        private Transform _initialTransform = null; // render transform for _controlled at start (or restart) of interaction

        /// <summary>
        ///     Construct an Interactor, given the photo-display on which the interaction takes place, the the 
        ///     mouse-click event that caused the interactor to be created. 
        /// </summary>
        public Interactor(PhotoDisplay pd, System.Windows.Input.MouseButtonEventArgs e)
        {
            _photoDisplay = pd;
            _controlled = (FrameworkElement)e.OriginalSource;
            _c1 = newContact(e);
            initializeInteraction();
        }

        /// <summary>
        /// If there's only one contact so far, add a second, if the 
        /// new click was on the same photo as before. 
        /// </summary>
        public void addContact(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_c2 == null)
            {
                if (_controlled == (FrameworkElement)e.OriginalSource)
                {
                    _c2 = newContact(e);
                    initializeInteraction();
                }
            }
        }


        /// <summary>
        /// Create a new Contact at the location given by the MouseEventArgs
        /// </summary>
        private Contact newContact(System.Windows.Input.MouseButtonEventArgs e)
        {
            Contact c = new Contact(_photoDisplay, this, e.GetPosition(_photoDisplay), (String) _controlled.Tag);
            _photoDisplay.Children.Add(c);
            return c;
        }

        /// <summary>
        /// In response to a motion of one of the contacts, adjust the RenderTransform on the controlled photo. 
        /// </summary>
        public void contactMoved(Contact c, Point p)
        {
            // need cases for one- and two-point transforms. 
            if (_c2 == null)  // there's only one contact point, so this is a translation...
            {
                Vector v = p - _startPoint;
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(_initialTransform);
                tg.Children.Add(new TranslateTransform(v.X, v.Y));

                _controlled.RenderTransform = tg;
            }
            else
            {
                // two-point motion. 
                // scale is ratio between current diff-vec and old diff-vec.
                // perform scale around starting mid-point. 
                // translation = diff between current midpoint and old midpoint.

                Point pp = getMidpoint(); // in world coords. 
                Point qq = _startPoint; 
                pp = _photoDisplay.TranslatePoint(pp, (UIElement) _controlled.Parent);
                qq = _photoDisplay.TranslatePoint(qq, (UIElement) _controlled.Parent);
                Vector motion = pp - qq;

                Vector contactDiff = _c2.getPosition() - _c1.getPosition();
                double scaleFactor = contactDiff.Length / _startVector.Length;
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(_initialTransform);
                tg.Children.Add(new ScaleTransform(scaleFactor, scaleFactor, qq.X,qq.Y));
                tg.Children.Add(new TranslateTransform(motion.X, motion.Y));

                _controlled.RenderTransform = tg;
            }
        }

        /// <summary>
        /// Remove the given contact, and if it's the last one, delete yourself. 
        /// </summary>

        public void removeContact(Contact c)
        {
            // if one contact, kill off interactor
            _photoDisplay.Children.Remove(c);
            if (_c2 == null)
            {
                this._photoDisplay.finishInteraction();
            }

            // if two contacts, 
            //     remove contact1, and move contact2 to contact1 
            //     reset start-point for interaction
            else
            {
                if (c == _c1)
                {
                    _c1 = _c2;
                }
                _c2 = null; // whether it was C1 or C2 that was clicked!
                initializeInteraction();
            }
            
        }

        /// <summary>
        /// Do the geometric work necessary to start manipulating a photo: record the 
        /// initial positions of the Contact or Contacts, and the current RenderTransform of the controlled Photo. 
        /// </summary>
        private void initializeInteraction()
        {
            _initialTransform = _controlled.RenderTransform;

            if (_c2 == null)
            {
                _startPoint = _c1.getPosition();
            }
            else
            {
                _startPoint = getMidpoint();
                _startVector = _c2.getPosition() - _c1.getPosition();
            }
        }

        /// <summary>
        /// Return the point midway between the two contacts. Will throw an exception if 
        /// there's only one contact. 
        /// </summary>
        private Point getMidpoint()
        {
            return _c1.getPosition() + 0.5 * (_c2.getPosition() - _c1.getPosition());
        }
    }
}
