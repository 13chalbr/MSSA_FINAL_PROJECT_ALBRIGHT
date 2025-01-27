using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
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

    public partial class AddPlanetWindow : Window
    {
        private ObservableCollection<Planet> _planets;

        public AddPlanetWindow(ObservableCollection<Planet> planets)
        {
            InitializeComponent();
            _planets = planets;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                string selectedColorName = (ColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                Color selectedColor = (Color)ColorConverter.ConvertFromString(selectedColorName);

                Planet newPlanet = new Planet
                {
                    Name = NameTextBox.Text,
                    ColorName = selectedColorName,
                    Color = selectedColor,
                    XPos = Constants.AU * double.Parse(XPosTextBox.Text), // entered as AU, converted to CM
                    YPos = Constants.AU * double.Parse(YPosTextBox.Text),
                    ZPos = Constants.AU * double.Parse(ZPosTextBox.Text),
                    XVel = 27.7777778 * double.Parse(XVelTextBox.Text), // entered as KM/H, converted to CM/S
                    YVel = 27.7777778 * double.Parse(YVelTextBox.Text),
                    ZVel = 27.7777778 * double.Parse(ZVelTextBox.Text),
                    Mass = Constants.EARTHMASS * double.Parse(MassTextBox.Text) // entered as Earth mass, converted to Grams
                };
                _planets.Add(newPlanet);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please fill in all fields with valid numeric values.");
            }
        }

        private bool ValidateInputs()
        {
            return !string.IsNullOrWhiteSpace(NameTextBox.Text) &&
                   ColorComboBox.SelectedItem != null &&
                   IsNumeric(XPosTextBox.Text) &&
                   IsNumeric(YPosTextBox.Text) &&
                   IsNumeric(ZPosTextBox.Text) &&
                   IsNumeric(XVelTextBox.Text) &&
                   IsNumeric(YVelTextBox.Text) &&
                   IsNumeric(ZVelTextBox.Text) &&
                   IsNumeric(MassTextBox.Text);
        }

        private bool IsNumeric(string text)
        {
            return double.TryParse(text, out _);
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); // regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
