using System;
using System.Windows.Controls;
using System.Windows.Media;

using System.IO;

namespace GraphicsBook
{
    class PhotoDisplay : Canvas
    {
        /// <summary>
        ///     Initializes a new instance of PhotoDisplay.
        /// </summary>

        private string _sourcePath;            // where to find the images
        private Interactor _interactor = null; // instance to represent the current contacts/interaction, if any.  

        /// <summary>
        /// Build a 600 x 600 photo display, and pop up a dialog so the user can navigate to a folder full of photos; load
        /// those photos and display them, and allow for multi-touch-like interaction with them. 
        /// </summary>
        public PhotoDisplay(String photoDirName) : base()
        {
            SetSourcePath();
            SetupInteraction();

            Canvas background = new Canvas();

            background.Height = 600;
            background.Width = 600;
            background.ClipToBounds = false;
            // color set to allow visualization of background canvas
            background.Background = Brushes.Beige;
            background.Tag = "Background";

            string[] filePaths = new String[0]; 

            if (_sourcePath != null)
            {
                filePaths = Directory.GetFiles(_sourcePath, "*.jpg");
            }

            int c = 0;
            // load photos, diagonally offset from each other. If there are too many photos,
            // they may march off-screen. 
            foreach (String filePath in filePaths)
            {
                Photo p = new Photo(filePath);
                p.Tag = "Photo " + c.ToString();
                p.RenderTransform = new MatrixTransform(1.0, 0.0, 0.0, 1.0, 20 * c, 20 * c);
                background.Children.Add(p);
                c++;
            }
            this.Children.Add(background);
        }

        /// <summary>
        /// Set a default path for the photo folder, and then ask the user for an alternative, and if s/he chooses one, use it. 
        /// </summary>
        private void SetSourcePath()
        {
            System.Windows.Forms.FolderBrowserDialog openFolderDlg = new System.Windows.Forms.FolderBrowserDialog();
            openFolderDlg.RootFolder = Environment.SpecialFolder.MyComputer;
            if (openFolderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(openFolderDlg.SelectedPath))
                {
                    _sourcePath = openFolderDlg.SelectedPath;
                }
                else
                {
                    _sourcePath = null;
                }
            }
        }

        /// <summary>
        /// Add a handler to watch for right-clicks so that we can create an Interactor if the user right-clicks on a photo.
        /// </summary>
        private void SetupInteraction()
        {
            this.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(PhotoDisplay_MouseRightButtonDown);
        }

        /// <summary>
        /// Delete the interactor; called when the user deletes the last Contact. 
        /// </summary>
        public void finishInteraction()
        {
            _interactor = null;
        }

        /// <summary>
        /// In the event of a right-button click, try to create an interactor if there's not one already. 
        /// If there is one already, try to add a second Contact for it (if there's not one already). 
        /// </summary>
        void PhotoDisplay_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_interactor == null)
            {
                _interactor = new Interactor(this, e);
            }
            else 
            {
                _interactor.addContact(e);
            }
        }

    }
}
