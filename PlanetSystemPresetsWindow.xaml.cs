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
    public partial class PlanetSystemPresetsWindow : Window
    {
        private ObservableCollection<Planet> _planets;

        public PlanetSystemPresetsWindow(ObservableCollection<Planet> planets)
        {
            InitializeComponent();
            _planets = planets;
            // Placeholder for future code to populate the presets list
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for future code to replace the contents of the datagridview
            this.Close();
        }
    }
}
