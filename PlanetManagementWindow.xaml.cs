﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using System.Collections.Generic;

namespace MSSA_FINAL_PROJECT_WORKING
{
    public static class ColorHelper
    {
        public static List<Color> Colors = new List<Color>
        {
            System.Windows.Media.Colors.Red,
            System.Windows.Media.Colors.Green,
            System.Windows.Media.Colors.Blue,
            System.Windows.Media.Colors.Yellow,
            System.Windows.Media.Colors.Violet, // Changed from Colors.Purple to Colors.Violet
            System.Windows.Media.Colors.Orange,
            System.Windows.Media.Colors.Brown,
            System.Windows.Media.Colors.Gray,
            System.Windows.Media.Colors.Black,
            System.Windows.Media.Colors.White
        };

        public static List<string> ColorNames = new List<string>
        {
            "Red",
            "Green",
            "Blue",
            "Yellow",
            "Purple", // You can keep the name as "Purple" if you prefer
            "Orange",
            "Brown",
            "Gray",
            "Black",
            "White"
        };
    }

    public partial class PlanetManagementWindow : Window
    {
        public ObservableCollection<Planet> Planets { get; set; }
        private bool simulationRun = false;
        private int planetNumber;
        private List<double> WorkingSystemPositionVelocity_CONCAT;
        private List<double> SystemMass_CONCAT;

        public List<double> SystemPosVel_PAST { get; set; }
        public List<double> SystemTime_PAST { get; set; }

        public PlanetManagementWindow()
        {
            InitializeComponent();
            Planets = new ObservableCollection<Planet>();
            PlanetDataGrid.ItemsSource = Planets;
        }

        private void RunSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            // Filter out empty rows
            var validPlanets = Planets.Where(p => !string.IsNullOrWhiteSpace(p.Name)).ToList();

            planetNumber = validPlanets.Count;
            WorkingSystemPositionVelocity_CONCAT = new List<double>();
            SystemMass_CONCAT = new List<double>();

            foreach (Planet planet in validPlanets)
            {
                WorkingSystemPositionVelocity_CONCAT.Add(planet.XPos);
                WorkingSystemPositionVelocity_CONCAT.Add(planet.YPos);
                WorkingSystemPositionVelocity_CONCAT.Add(planet.ZPos);
                WorkingSystemPositionVelocity_CONCAT.Add(planet.XVel);
                WorkingSystemPositionVelocity_CONCAT.Add(planet.YVel);
                WorkingSystemPositionVelocity_CONCAT.Add(planet.ZVel);
                SystemMass_CONCAT.Add(planet.Mass);
            }

            RunSimulationWindow runSimulationWindow = new RunSimulationWindow(planetNumber, WorkingSystemPositionVelocity_CONCAT, SystemMass_CONCAT)
            {
                Owner = this
            };
            runSimulationWindow.SimulationCompleted += OnSimulationCompleted;
            runSimulationWindow.Show();
        }

        private void OnSimulationCompleted(object sender, EventArgs e)
        {
            simulationRun = true;
            AnimateButton.IsEnabled = true;
            WriteDataButton.IsEnabled = true;
            SimulationStatusCheckBox.IsChecked = true;
            SimulationStatusCheckBox.Background = Brushes.Green;
        }

        private void AnimateButton_Click(object sender, RoutedEventArgs e)
        {
            if (simulationRun)
            {
                // Prepare data for animation
                List<List<List<double>>> split_PAST = Methods.TransposePast(SystemPosVel_PAST, planetNumber);
                Methods.TransposeMixer(split_PAST, planetNumber, out List<List<double>> xList, out List<List<double>> yList, out List<List<double>> zList);

                // Filter out empty lists
                xList = xList.Where(list => list.Count > 0).ToList();
                yList = yList.Where(list => list.Count > 0).ToList();
                zList = zList.Where(list => list.Count > 0).ToList();

                // Get planet names and colors
                var planetNames = Planets.Select(p => p.Name).ToList();
                var planetColors = Planets.Select(p => p.Color).ToList();

                // Create and show the AnimationWindow with the required lists
                AnimationWindow animationWindow = new AnimationWindow(xList, yList, zList, planetNames, planetColors);
                animationWindow.Show();
            }
            else
            {
                MessageBox.Show("Please run the simulation first.");
            }
        }

        private void PlanetDataGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            ResetSimulationStatus();
        }

        private void ResetSimulationStatus()
        {
            simulationRun = false;
            AnimateButton.IsEnabled = false;
            WriteDataButton.IsEnabled = false;
            SimulationStatusCheckBox.IsChecked = false;
            SimulationStatusCheckBox.Background = Brushes.Red;
        }

        private void PlanetSystemPresetsButton_Click(object sender, RoutedEventArgs e)
        {
            PlanetSystemPresetsWindow presetsWindow = new PlanetSystemPresetsWindow(Planets);
            presetsWindow.Show();
        }

        private void ReadSystemDataButton_Click(object sender, RoutedEventArgs e)
        {
            ReadSystemDataWindow readDataWindow = new ReadSystemDataWindow(Planets);
            readDataWindow.Show();
        }

        private void WriteDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (simulationRun)
            {
                // Prepare data for CSV writing
                List<List<List<double>>> split_PAST = Methods.TransposePast(SystemPosVel_PAST, planetNumber);
                Methods.TransposeMixer(split_PAST, planetNumber, out List<List<double>> xList, out List<List<double>> yList, out List<List<double>> zList);

                WriteDataWindow writeDataWindow = new WriteDataWindow(xList, yList, zList);
                writeDataWindow.Show();
            }
            else
            {
                MessageBox.Show("Please run the simulation first.");
            }
        }
    }
}

