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
        private List<Preset> _presets;
        private PlanetManagementWindow _parentWindow;

        public PlanetSystemPresetsWindow(ObservableCollection<Planet> planets, PlanetManagementWindow parentWindow)
        {
            InitializeComponent();
            _planets = planets;
            _parentWindow = parentWindow;
            LoadPresets();
        }

        private void LoadPresets()
        {
            _presets = PresetDatabase.LoadPresets();
            PresetsListBox.ItemsSource = _presets;
            PresetsListBox.Items.Refresh(); // Refresh the ListBox to reflect changes
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (PresetsListBox.SelectedItem is Preset selectedPreset)
            {
                _planets.Clear();
                foreach (var planet in selectedPreset.Planets)
                {
                    _planets.Add(planet);
                }
                _parentWindow.ResetSimulationStatus(); // Reset the simulation status
                this.Close();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PresetsListBox.SelectedItem is Preset selectedPreset)
            {
                _presets.Remove(selectedPreset);
                PresetDatabase.DeletePreset(selectedPreset);
                PresetsListBox.Items.Refresh(); // Refresh the ListBox to reflect changes
            }
            else
            {
                MessageBox.Show("Please select a preset to delete.");
            }
        }

        private void PresetsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PresetsListBox.SelectedItem is Preset selectedPreset)
            {
                StringBuilder details = new StringBuilder();
                details.AppendLine($"Preset Name: {selectedPreset.Name}");
                details.AppendLine($"Description: {selectedPreset.Description}");
                details.AppendLine("Planets:");

                foreach (var planet in selectedPreset.Planets)
                {
                    details.AppendLine($"Name: {planet.Name}, Color: {planet.ColorName}, X Pos: {planet.XPos}, Y Pos: {planet.YPos}, Z Pos: {planet.ZPos}, X Vel: {planet.XVel}, Y Vel: {planet.YVel}, Z Vel: {planet.ZVel}, Mass: {planet.Mass}");
                }

                PresetDetailsTextBlock.Text = details.ToString();
            }
        }
    }
}
