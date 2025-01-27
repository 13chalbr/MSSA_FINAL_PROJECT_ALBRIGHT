using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Interaction logic for ReadSystemDataWindow.xaml
    /// </summary>
    public partial class ReadSystemDataWindow : Window
    {
        private ObservableCollection<Planet> _planets;

        public ReadSystemDataWindow(ObservableCollection<Planet> planets)
        {
            InitializeComponent();
            _planets = planets;
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("All data will be replaced. Do you want to continue?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _planets.Clear();
                // Placeholder for future code to read data from CSV and populate the datagridview
                this.Close();
            }
        }
    }
}
