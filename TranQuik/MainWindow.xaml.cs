using System.Windows;
using TranQuik.Configuration;

namespace TranQuik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Config.LoadAppSettings();
        }
    }
}
