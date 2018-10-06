using System.Windows;

namespace GraphicsBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Content = new PhotoDisplay("none");
            Title = "Photo Sorter";
            Show();

        }
    }
}
