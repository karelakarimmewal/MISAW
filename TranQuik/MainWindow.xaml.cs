using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using TranQuik.Configuration;
using TranQuik.Model;
using System.Windows.Media;

namespace TranQuik
{
    public partial class MainWindow : Window
    {
        // Cart Management
        private List<Product> cartProducts = new List<Product>(); // List to store products in the cart
        private List<string> productGroupNames;
        private List<int> productGroupIds;

        // Database Settings
        private LocalDbConnector localDbConnector;
        private ModelProcessing modelProcessing;
        private ProductDetails ProductDetails;

        // User Interface Elements
        private List<Button> productGroupButtons = new List<Button>(); // List of buttons for product groups
        //private SecondaryWindow secondaryWindow; // Secondary window reference

        // Application State
        public int SaleMode = 0; // Sale mode indicator
        private int currentIndex = 0; // Current index state
        private int batchSize = 15; // Batch size for data operations
        private int startIndex = 0; // Start index for data display
        private int visibleButtonCounts = 8; // Initial visible button count
        private int productButtonStartIndex = 0; // Start index for product buttons
        private int productButtonCount = 24; // Total count of product buttons

        // Financial Settings
        private int taxPercentage = 11; // Tax percentage
        private decimal discountPercentage = 0; // Discount percentage

        // Payment and Display Data
        private List<int> payTypeIDs = new List<int>(); // List of payment type IDs
        private List<string> displayNames = new List<string>(); // List of display names
        private List<bool> isAvailableList = new List<bool>(); // List of availability statuses

        // Cart and Customer Management
        private Dictionary<DateTime, HeldCart> heldCarts = new Dictionary<DateTime, HeldCart>(); // Dictionary of held carts
        private int nextCustomerId = 1; // Next available customer ID

        private SaleModePop SaleModePop;

        public MainWindow()
        {
            // Load application settings (if needed)
            Config.LoadAppSettings();
            modelProcessing = new ModelProcessing(this);
            InitializeComponent();

            // Handle the Loaded event to show SaleModePop after MainWindow is fully loaded
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            SaleModePop saleModeWindow = new SaleModePop(this); // Pass reference to MainWindow
            saleModeWindow.Topmost = true;
            saleModeWindow.ShowDialog(); // Show SaleModePop window as modal
        }

        public void ProductGroupLoad()
        {
            // Check if SaleMode is greater than zero
            if (SaleMode > 0)
            {
                try
                {
                    // Load product group names and IDs
                    List<string> productGroupNames;
                    List<int> productGroupIds;
                    modelProcessing.GetProductGroupNamesAndIds(out productGroupNames, out productGroupIds);
                    // Create buttons for each product group and add them to the WrapPanel
                    for (int i = 0; i < productGroupNames.Count; i++)
                    {
                        Button button = new Button
                        {
                            Content = productGroupNames[i],
                            Height = 50,
                            Width = 99,
                            FontWeight = FontWeights.Bold,
                            BorderThickness = new Thickness(0),
                            Tag = productGroupIds[i],
                            Foreground = (SolidColorBrush)FindResource("FontColor"),
                            Background = Brushes.Azure,
                            Effect = (System.Windows.Media.Effects.Effect)FindResource("DropShadowEffect"),
                            Style = (Style)FindResource("ButtonStyle")
                        };

                        button.Click += GroupClicked;
                        ProductGroupName.Children.Add(button);
                        productGroupButtons.Add(button);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
        private void GroupClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                int productGroupId = Convert.ToInt32(button.Tag);
                modelProcessing.LoadProductDetails(productGroupId);
            }
        }

    }
}
