using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace TranQuik.Model
{
    public class ModelProcessing
    {
        private LocalDbConnector localDbConnector;
        private MainWindow mainWindow;

        public ModelProcessing(MainWindow mainWindow)
        {
            this.localDbConnector = new LocalDbConnector(); // Instantiate LocalDbConnector
            this.mainWindow = mainWindow; // Assign the MainWindow instance
        }

        public void GetProductGroupNamesAndIds(out List<string> productGroupNames, out List<int> productGroupIds)
        {
            productGroupNames = new List<string>();
            productGroupIds = new List<int>();

            try
            {
                using (MySqlConnection connection = localDbConnector.GetMySqlConnection())
                {
                    string query = @"
                        SELECT PD.ProductDeptID, PD.ProductDeptName
                        FROM ProductDept PD
                        WHERE PD.ProductDeptActivate = 1";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string groupName = reader["ProductDeptName"].ToString();
                        int groupId = Convert.ToInt32(reader["ProductDeptID"]);

                        productGroupNames.Add(groupName);
                        productGroupIds.Add(groupId);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving product group data from the database.", ex);
            }
        }
        public void LoadProductDetails(int productGroupId)
        {
            string query = (productGroupId == -1)
                ? @"
                    SELECT P.`ProductDeptID`, PD.`ProductDeptName`, P.`ProductCode`, P.`ProductName`, P.`ProductName2`, PP.`ProductPrice`, PP.`SaleMode`
                    FROM Products P
                    JOIN ProductPrice PP ON P.`ProductID` = PP.`ProductID`
                    JOIN ProductDept PD ON PD.`ProductDeptID` = P.`ProductDeptID`
                    WHERE P.`ProductActivate` = 1 AND PP.`SaleMode` = @SaleModeID
                    ORDER BY P.`ProductName`;"
                            : @"
                    SELECT P.`ProductDeptID`, PD.`ProductDeptName`, P.`ProductCode`, P.`ProductName`, P.`ProductName2`, PP.`ProductPrice`, PP.`SaleMode`
                    FROM Products P
                    JOIN ProductPrice PP ON P.`ProductID` = PP.`ProductID`
                    JOIN ProductDept PD ON PD.`ProductDeptID` = P.`ProductDeptID`
                    WHERE P.`ProductActivate` = 1 AND PP.`SaleMode` = @SaleModeID AND PD.ProductDeptID = @ProductDeptID
                    ORDER BY P.`ProductName`;";

            using (MySqlConnection connection = localDbConnector.GetMySqlConnection())
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@SaleModeID", mainWindow.SaleMode);

                if (productGroupId != -1)
                    command.Parameters.AddWithValue("@ProductDeptID", productGroupId);

                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                mainWindow.MainContentProduct.Children.Clear(); // Clear existing product buttons

                while (reader.Read())
                {
                    string productName = reader["ProductName"].ToString();
                    int productId = Convert.ToInt32(reader["ProductCode"]);
                    decimal productPrice = Convert.ToDecimal(reader["ProductPrice"]);

                    // Create product instance
                    Product product = new Product(productId, productName, productPrice);

                    // Determine the image path
                    string imgFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string imagePath = Path.Combine(imgFolderPath, "Image", $"{productName}.jpg");

                    // Create the product button
                    Button productButton = CreateProductButton(product, imagePath);

                    productButton.Click += ProductButton_Click;

                    // Add the product button to the wrap panel
                    mainWindow.MainContentProduct.Children.Add(productButton);
                }

                reader.Close();
            }
        }

        private Button CreateProductButton(Product product, string imagePath)
        {
            // Check if image creation is allowed based on application setting
            bool allowImage = bool.Parse(Properties.Settings.Default["_AppAllowImage"].ToString());

            // Create product button
            Button productButton = new Button
            {
                Height = 118,
                Width = 100, // Set fixed width
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0.8),
                Tag = product // Assign product instance to Tag property
            };

            // Create a StackPanel to hold the image and text
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add text to the stack panel
            TextBlock textBlock = new TextBlock
            {
                Text = product.ProductName,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 3)
            };
            stackPanel.Children.Add(textBlock);

            // Check if image creation is allowed
            if (allowImage && File.Exists(imagePath))
            {
                // Load the image
                BitmapImage image = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                Image img = new Image
                {
                    Source = image,
                    Width = 100,
                    Height = 100,
                };
                stackPanel.Children.Insert(0, img); // Insert image at the beginning
            }

            // Set the content of the button to the stack panel
            productButton.Content = stackPanel;

            return productButton;
        }


        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle button click event
            Button clickedButton = (Button)sender;
            Product product = (Product)clickedButton.Tag as Product;
            if (product != null)
            {
                Console.WriteLine($"Product added to cart: {product.ProductId}, {product.ProductName}, Price: {product.ProductPrice}");
            }
            // Implement logic for handling the click on the product button
        }

    }
}
