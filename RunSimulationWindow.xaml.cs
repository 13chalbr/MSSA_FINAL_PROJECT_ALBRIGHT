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
    public partial class RunSimulationWindow : Window
    {
        public event EventHandler<double> SimulationCompleted;
        private int planetNumber;
        private List<double> WorkingSystemPositionVelocity_CONCAT;
        private List<double> SystemMass_CONCAT;
        private double timeStep;

        public List<double> SystemPosVel_PAST { get; set; }
        public List<double> SystemTime_PAST { get; set; }

        public RunSimulationWindow(int planetNumber, List<double> WorkingSystemPositionVelocity_CONCAT, List<double> SystemMass_CONCAT,  double timeStep)
        {
            InitializeComponent();
            this.planetNumber = planetNumber;
            this.WorkingSystemPositionVelocity_CONCAT = WorkingSystemPositionVelocity_CONCAT;
            this.SystemMass_CONCAT = SystemMass_CONCAT;
            this.timeStep = timeStep;

            SystemPosVel_PAST = new List<double>();
            SystemTime_PAST = new List<double>();
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            double simulationTime = 86400 * int.Parse(SimulationTimeTextBox.Text); // User inputs days, converts to seconds
            double timeStep = 3600 * int.Parse(TimeStepTextBox.Text); // user inputs hours, converts to seconds
            double t = 0; // Placeholder for simulation beginning time (seconds)
            int stepNum = (int)Math.Round((simulationTime - t) / timeStep);

            // Puts initial conditions into "PAST" lists for PosVel and Time.
            SystemPosVel_PAST = SystemPosVel_PAST.Concat(WorkingSystemPositionVelocity_CONCAT).ToList();
            SystemTime_PAST.Add(t);

            ProgressBarWindow progressBarWindow = new ProgressBarWindow();
            progressBarWindow.Show();

            await Task.Run(() =>
            {
                for (int i = 0; i < stepNum; i++)
                {
                    WorkingSystemPositionVelocity_CONCAT = Methods.RK4Compute(in WorkingSystemPositionVelocity_CONCAT, in SystemMass_CONCAT, in timeStep, in planetNumber);
                    t += timeStep;
                    SystemPosVel_PAST = SystemPosVel_PAST.Concat(WorkingSystemPositionVelocity_CONCAT).ToList(); // Adds to past for collection
                    SystemTime_PAST.Add(t);  // Adds to past for collection
                }
            });

            progressBarWindow.Close();
            MessageBox.Show($"Simulation completed and data stored. \nNumber of planets: {planetNumber}.\nNumber of calculated intervals: {stepNum}.");
            SimulationCompleted?.Invoke(this, timeStep);

            // Pass the data back to the PlanetManagementWindow
            ((PlanetManagementWindow)Owner).SystemPosVel_PAST = SystemPosVel_PAST;
            ((PlanetManagementWindow)Owner).SystemTime_PAST = SystemTime_PAST;

            this.Close();
        }
    }
}
