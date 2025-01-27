using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class EditPlanetWindow : Window
    {
        public Planet Planet { get; private set; }

        public EditPlanetWindow(Planet planet)
        {
            InitializeComponent();
            Planet = planet;
            DataContext = Planet;

            // Clear existing items before setting ItemsSource
            ColorComboBox.Items.Clear();
            ColorComboBox.ItemsSource = ColorHelper.ColorNames;
            ColorComboBox.SelectedItem = planet.ColorName;

            // Populate the TextBoxes with the planet's current values
            NameTextBox.Text = planet.Name;
            XPosTextBox.Text = (planet.XPos / Constants.AU).ToString(); // Convert back to AU
            YPosTextBox.Text = (planet.YPos / Constants.AU).ToString();
            ZPosTextBox.Text = (planet.ZPos / Constants.AU).ToString();
            XVelTextBox.Text = (planet.XVel / 27.7777778).ToString(); // Convert back to KM/H
            YVelTextBox.Text = (planet.YVel / 27.7777778).ToString();
            ZVelTextBox.Text = (planet.ZVel / 27.7777778).ToString();
            MassTextBox.Text = (planet.Mass / Constants.EARTHMASS).ToString(); // Convert back to Earth mass
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                Planet.Name = NameTextBox.Text;
                Planet.ColorName = ColorComboBox.SelectedItem.ToString();
                Planet.Color = (Color)ColorConverter.ConvertFromString(Planet.ColorName);
                Planet.XPos = Constants.AU * double.Parse(XPosTextBox.Text); // Convert back to CM
                Planet.YPos = Constants.AU * double.Parse(YPosTextBox.Text);
                Planet.ZPos = Constants.AU * double.Parse(ZPosTextBox.Text);
                Planet.XVel = 27.7777778 * double.Parse(XVelTextBox.Text); // Convert back to CM/S
                Planet.YVel = 27.7777778 * double.Parse(YVelTextBox.Text);
                Planet.ZVel = 27.7777778 * double.Parse(ZVelTextBox.Text);
                Planet.Mass = Constants.EARTHMASS * double.Parse(MassTextBox.Text); // Convert back to Grams

                DialogResult = true;
                Close();
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
