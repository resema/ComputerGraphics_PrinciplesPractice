using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Diagnostics;

namespace GraphicsBook
{
    /// <summary>
    ///     A class for representing a point of contact in a multi-touch interaction. Visually represented by a 
    ///     small pink sphere, draggable by left-mouse interaction. 
    ///     
    /// A Contact is a contact between a finger and some object (in this application, a photo); the parent
    /// interactor (which is basically a holder for a collection of one or two Contacts) keeps track of
    /// that controlled object. 
    /// 
    /// The Contact's only jobs are to record its starting position during a drag,  report
    /// each bit of drag to the parent Interactor, and report when the drag is complete. 
    /// </summary>
    class Contact : Shape
    {
        #region members and constructors
        // appearance data
        protected EllipseGeometry myEllipse;
        protected const double initialStrokeThickness = 0.4;
        protected const double initialRadius = 4;
        protected static readonly  SolidColorBrush initialColor = Brushes.Plum;
        protected static readonly SolidColorBrush initialStrokeColor = Brushes.DarkMagenta;
        protected static readonly Point initialPosition = new Point(0, 0);

        // related objects
        protected Canvas _parent;
        protected Interactor _interactor;
        
        // interaction data
        protected Point _dragCenter = new Point(0, 0);
        protected Point _dragStart = new Point(0, 0);

        /// <summary>
        ///     Gets a value that represents the Geometry of the Contact.
        /// </summary>
        protected  override Geometry DefiningGeometry
        {
            get
            {
                return myEllipse;
            }
        }
        /// <summary>
        ///     Initializes a new instance of Contact.
        /// </summary>
        private Contact(Canvas parent)
        {
            _sourceTag = "NO ASSOCIATED ELEMENT";
            _parent = parent;
            myEllipse = new EllipseGeometry();
            myEllipse.RadiusX = initialRadius;
            myEllipse.RadiusY = initialRadius;
            myEllipse.Center = new Point(0, 0);
        }
        /// <summary>
        ///     Initializes a new instance of Contact at the given Point
        /// </summary>
        public Contact(Canvas parent, Interactor i,  Point init, String sourceTag)
        {
            _sourceTag = sourceTag;
            _interactor = i;
            _parent = parent;

            constructBasics();
            myEllipse.Center = init;
            setToolTip();
        }
        #endregion

        #region event and drag handling
        private bool _IsDown; // Is the mouse down right now?
        private String _sourceTag; // tag for source-name. 

        /// <summary>
        ///     Respond to a right-button click by telling the parent Interactor to remove this Contact. 
        /// </summary>
        void Contact_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _interactor.removeContact(this);
            e.Handled = true;
            Debug.Print("removing contact");
        }

        /// <summary>
        ///     Check that a start to a drag on this Contact really did occur within the Contact's geometry; 
        ///     If so, record the starting position of the drag (in the parent Canvas's coord. system), and
        ///     "capture" the mouse, so that all further mouse events go to this object until it's released.
        /// </summary>
        private void Contact_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            Debug.Print("Preview Mouse Down");
            if (e.Source != this) // ONLY OK becaue this element is a single shape with no children!
            {
                Debug.Print("Preview Mouse Down Bad Source");
                return;
            }
            _IsDown = true;
            this.CaptureMouse();
            _dragStart = e.GetPosition(_parent);
            _dragCenter = myEllipse.Center;

            e.Handled = true;
            Debug.Print("Preview Mouse Down DONE");
        }

        /// <summary>
        ///    When the drag is complete, release the mouse so that other drag operations can be handled.     
        /// </summary>
        private void Contact_PreviewMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
        {
            Debug.Print("Preview Mouse Left Up");
            if (e.Source != this) // ONLY OK becaue this element is a single shape with no children!
            {
                Debug.Print("Preview Mouse Up Bad Source");
                return;
            }
            if (_IsDown)
            {
                _IsDown = false;
                e.Handled = true;
                this.ReleaseMouseCapture();
            }
            Debug.Print("Preview Mouse Left Up DONE");
        }

        /// <summary>
        ///    During a drag, compute the vector from the drag-start to the current point, and update the Contact's geometry;
        ///    also tell the parent Interactor that the drag happened. 
        /// </summary>
        private void Contact_PreviewMouseMove(Object sender, MouseEventArgs e)
        {
            //Debug.Print("Contact_PreviewMouseMove");
            if (_IsDown)
            {
                Vector motion = e.GetPosition(_parent) - _dragStart;
                myEllipse.Center = _dragCenter + (motion);
                _interactor.contactMoved(this, myEllipse.Center);
            }
        }

        /// <summary>
        ///    Return the current position of the Contact.  
        /// </summary>
        public Point getPosition()
        {
            return myEllipse.Center;
        }

        #endregion


        #region helpers
        /// <summary>
        ///     Construct all the  parts of a Contact
        /// </summary>
        private void constructBasics()
        {
            myEllipse = new EllipseGeometry();
            myEllipse.RadiusX = initialRadius;
            myEllipse.RadiusY = initialRadius;
            myEllipse.Center = initialPosition;
            StrokeThickness = initialStrokeThickness;
            Stroke = initialStrokeColor;
            Fill = initialColor;
            this.PreviewMouseRightButtonDown += new MouseButtonEventHandler(Contact_MouseRightButtonDown);
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Contact_PreviewMouseLeftButtonDown);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(Contact_PreviewMouseLeftButtonUp);
            this.PreviewMouseMove += new MouseEventHandler(Contact_PreviewMouseMove);
        }


        /// <summary>
        ///     Set up a tool tip so that debugging contacts is easier. 
        /// </summary>
        private void setToolTip()
        {
            ToolTip tt = new ToolTip();
            tt.Content = _sourceTag + ", (" + myEllipse.Center.X + ", " + myEllipse.Center.Y + ")";
            this.ToolTip = tt;
        }
        #endregion

    }
}


