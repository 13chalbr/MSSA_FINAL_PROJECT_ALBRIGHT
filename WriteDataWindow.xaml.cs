using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MSSA_FINAL_PROJECT_WORKING
{
    public partial class WriteDataWindow : Window
    {
        private List<List<double>> xList;
        private List<List<double>> yList;
        private List<List<double>> zList;

        public WriteDataWindow(List<List<double>> xList, List<List<double>> yList, List<List<double>> zList)
        {
            InitializeComponent();
            this.xList = xList;
            this.yList = yList;
            this.zList = zList;
        }

        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Please enter a valid file path.");
                return;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write data in the desired format
                    for (int i = 0; i < xList.Count; i++)
                    {
                        if (xList[i].Count > 0 && yList[i].Count > 0 && zList[i].Count > 0)
                        {
                            writer.WriteLine($"Planet {i + 1} Coords:");
                            writer.WriteLine(string.Join(",", xList[i])); // X coordinates
                            writer.WriteLine(string.Join(",", yList[i])); // Y coordinates
                            writer.WriteLine(string.Join(",", zList[i])); // Z coordinates
                            writer.WriteLine(); // Leave a space between planets
                        }
                    }
                }

                MessageBox.Show("Data successfully written to CSV file.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing to file: {ex.Message}");
            }
        }
    }
}
