using System;
using System.Collections.Generic;
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
    public partial class SavePresetWindow : Window
    {
        public string PresetName { get; private set; }
        public string PresetDescription { get; private set; }

        public SavePresetWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            PresetName = PresetNameTextBox.Text;
            PresetDescription = PresetDescriptionTextBox.Text;
            DialogResult = true;
            Close();
        }
    }
}
