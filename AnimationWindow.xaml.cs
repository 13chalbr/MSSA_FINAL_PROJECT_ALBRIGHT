using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HelixToolkit.Wpf;

namespace MSSA_FINAL_PROJECT_WORKING
{

    public partial class AnimationWindow : Window
    {
        private TranslateTransform3D[] transforms;
        private Point3DCollection[] trajectories;
        private BillboardTextVisual3D[] labels;
        private DateTime startTime;
        private bool animationStarted;
        private bool isPaused = false;
        private double animationSpeed = 1.0;
        private double elapsedTime;
        private double totalSimulationTime; // New field to store total simulation time in seconds
        private double timeStep; // New field to store time step in seconds
        private ProgressBarWindow progressBarWindow; // New field for ProgressBarWindow

        public AnimationWindow(List<List<double>> xList, List<List<double>> yList, List<List<double>> zList, List<string> planetNames, List<Color> planetColors, double totalSimulationTime, double timeStep)
        {
            InitializeComponent();

            this.totalSimulationTime = totalSimulationTime; // Initialize the total simulation time
            this.timeStep = timeStep; // Initialize the time step
                                      // Show the progress bar window
            progressBarWindow = new ProgressBarWindow();
            progressBarWindow.Show();

            var viewport = new HelixViewport3D();
            var modelGroup = new Model3DGroup();

            // Create and add axes
            var axes = new LinesVisual3D
            {
                Color = Colors.Black,
                Thickness = 1 // Reduced thickness
            };
            axes.Points.Add(new Point3D(-50, 0, 0));
            axes.Points.Add(new Point3D(50, 0, 0)); // X axis
            axes.Points.Add(new Point3D(0, -50, 0));
            axes.Points.Add(new Point3D(0, 50, 0)); // Y axis
            axes.Points.Add(new Point3D(0, 0, -50));
            axes.Points.Add(new Point3D(0, 0, 50)); // Z axis

            // Add tick marks and values for X axis
            for (double i = -50; i <= 50; i++)
            {
                axes.Points.Add(new Point3D(i, -0.1, 0));
                axes.Points.Add(new Point3D(i, 0.1, 0));

                if (i % 5 == 0) // Add value labels at every 5th tick mark
                {
                    var xTickLabel = new BillboardTextVisual3D
                    {
                        Text = i.ToString(),
                        Position = new Point3D(i, -0.5, 0),
                        Foreground = new SolidColorBrush(Colors.Black)
                    };
                    viewport.Children.Add(xTickLabel);
                }
            }

            // Add tick marks and values for Y axis
            for (double i = -50; i <= 50; i++)
            {
                axes.Points.Add(new Point3D(-0.1, i, 0));
                axes.Points.Add(new Point3D(0.1, i, 0));

                if (i % 5 == 0) // Add value labels at every 5th tick mark
                {
                    var yTickLabel = new BillboardTextVisual3D
                    {
                        Text = i.ToString(),
                        Position = new Point3D(-0.5, i, 0),
                        Foreground = new SolidColorBrush(Colors.Black)
                    };
                    viewport.Children.Add(yTickLabel);
                }
            }

            // Add tick marks and values for Z axis
            for (double i = -50; i <= 50; i++)
            {
                axes.Points.Add(new Point3D(0, -0.1, i));
                axes.Points.Add(new Point3D(0, 0.1, i));

                if (i % 5 == 0) // Add value labels at every 5th tick mark
                {
                    var zTickLabel = new BillboardTextVisual3D
                    {
                        Text = i.ToString(),
                        Position = new Point3D(0, -0.5, i),
                        Foreground = new SolidColorBrush(Colors.Black)
                    };
                    viewport.Children.Add(zTickLabel);
                }
            }

            viewport.Children.Add(axes);

            // Add axis labels
            var xLabel = new BillboardTextVisual3D
            {
                Text = "X (units)",
                Position = new Point3D(50, 0, 0),
                Foreground = new SolidColorBrush(Colors.Red), // Changed color for better visibility
                FontSize = 16 // Increased font size for better visibility
            };
            var xLabelNegative = new BillboardTextVisual3D
            {
                Text = "X (units)",
                Position = new Point3D(-50, 0, 0),
                Foreground = new SolidColorBrush(Colors.Red), // Changed color for better visibility
                FontSize = 16 // Increased font size for better visibility
            };
            var yLabel = new BillboardTextVisual3D
            {
                Text = "Y (units)",
                Position = new Point3D(0, 50, 0),
                Foreground = new SolidColorBrush(Colors.Green), // Changed color for better visibility
                FontSize = 16 // Increased font size for better visibility
            };
            var yLabelNegative = new BillboardTextVisual3D
            {
                Text = "Y (units)",
                Position = new Point3D(0, -50, 0),
                Foreground = new SolidColorBrush(Colors.Green), // Changed color for better visibility
                FontSize = 16 // Increased font size for better visibility
            };
            var zLabel = new BillboardTextVisual3D
            {
                Text = "Z (units)",
                Position = new Point3D(0, 0, 50),
                Foreground = new SolidColorBrush(Colors.Blue), // Changed color for better visibility
                FontSize = 16 // Increased font size for better visibility
            };
            var zLabelNegative = new BillboardTextVisual3D
            {
                Text = "Z (units)",
                Position = new Point3D(0, 0, -50),
                Foreground = new SolidColorBrush(Colors.Blue), // Changed color for better visibility
                FontSize = 16 // Increased font size for better visibility
            };
            viewport.Children.Add(xLabel);
            viewport.Children.Add(xLabelNegative);
            viewport.Children.Add(yLabel);
            viewport.Children.Add(yLabelNegative);
            viewport.Children.Add(zLabel);
            viewport.Children.Add(zLabelNegative);

            // Define planet trajectories using xList, yList, and zList
            var planetTrajectories = new List<dynamic>();

            for (int i = 0; i < xList.Count; i++)
            {
                var points = new Point3DCollection();
                for (int j = 0; j < xList[i].Count; j++)
                {
                    points.Add(new Point3D(xList[i][j], yList[i][j], zList[i][j]));
                }

                planetTrajectories.Add(new
                {
                    Name = planetNames[i],
                    Color = planetColors[i],
                    Points = points
                });

                // Update progress bar on the UI thread
                double progress = (double)(i + 1) / xList.Count;
                Dispatcher.Invoke(() => progressBarWindow.ProgressBar.Value = progress * 100);
            }

            transforms = new TranslateTransform3D[planetTrajectories.Count];
            trajectories = new Point3DCollection[planetTrajectories.Count];
            labels = new BillboardTextVisual3D[planetTrajectories.Count];

            for (int i = 0; i < planetTrajectories.Count; i++)
            {
                var planet = planetTrajectories[i];

                var linesVisual = new LinesVisual3D
                {
                    Color = planet.Color,
                    Thickness = 2
                };

                for (int j = 0; j < planet.Points.Count - 1; j++)
                {
                    linesVisual.Points.Add(planet.Points[j]);
                    linesVisual.Points.Add(planet.Points[j + 1]);
                }

                viewport.Children.Add(linesVisual);

                // Create a sphere to represent the planet with a solid color
                var sphere = new SphereVisual3D
                {
                    Radius = 0.05,
                    Material = new DiffuseMaterial(new SolidColorBrush(planet.Color))
                };
                viewport.Children.Add(sphere);

                // Create a TranslateTransform3D to animate the planet
                var transform = new TranslateTransform3D();
                sphere.Transform = transform;
                transforms[i] = transform;
                trajectories[i] = planet.Points;

                // Add labels that follow the planet
                var label = new BillboardTextVisual3D
                {
                    Text = planet.Name,
                    Foreground = new SolidColorBrush(planet.Color)
                };
                viewport.Children.Add(label);
                labels[i] = label;
            }

            viewport.Children.Add(new ModelVisual3D { Content = modelGroup });

            // Add the viewport to the window
            MainGrid.Children.Add(viewport);

            // Add legend
            var legend = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10)
            };

            foreach (var planet in planetTrajectories)
            {
                var legendItem = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };

                var colorBox = new Border
                {
                    Background = new SolidColorBrush(planet.Color),
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5)
                };

                var legendText = new TextBlock
                {
                    Text = planet.Name,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };

                legendItem.Children.Add(colorBox);
                legendItem.Children.Add(legendText);
                legend.Children.Add(legendItem);
            }

            MainGrid.Children.Add(legend);

            // Close the progress bar window once the animation window launches
            progressBarWindow.Close();

            // Start the animation
            startTime = DateTime.Now;
            animationStarted = true;

            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!animationStarted || isPaused) return;

            elapsedTime = (DateTime.Now - startTime).TotalSeconds * animationSpeed * 86400; // Adjust elapsed time to days
            var duration = totalSimulationTime; // Use the total simulation time

            // Check if the animation has reached the end
            if (elapsedTime >= duration)
            {
                elapsedTime = 0; // Reset elapsed time
                startTime = DateTime.Now; // Reset start time
            }

            // Calculate elapsed time in days
            double elapsedDays = elapsedTime / 86400.0;

            // Update the elapsed time display
            ElapsedTimeTextBlock.Text = $"Elapsed Time: {elapsedDays:F2} days";

            // Update the progress bar
            double progress = elapsedTime / duration;
            progressBarWindow.ProgressBar.Value = progress * 100;

            for (int i = 0; i < transforms.Length; i++)
            {
                var points = trajectories[i];
                var t = elapsedTime / duration;
                var segment = (int)(t * (points.Count - 1));
                var segmentT = (t * (points.Count - 1)) - segment;

                if (segment < points.Count - 1)
                {
                    var p1 = points[segment];
                    var p2 = points[segment + 1];
                    var position = new Point3D(
                        p1.X + (p2.X - p1.X) * segmentT,
                        p1.Y + (p2.Y - p1.Y) * segmentT,
                        p1.Z + (p2.Z - p1.Z) * segmentT
                    );
                    transforms[i].OffsetX = position.X;
                    transforms[i].OffsetY = position.Y;
                    transforms[i].OffsetZ = position.Z;

                    // Update label position
                    labels[i].Position = new Point3D(position.X, position.Y + 0.3, position.Z);
                }
            }

        }

        private void PauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                isPaused = false;
                PauseResumeButton.Content = "Pause";
                startTime = DateTime.Now - TimeSpan.FromSeconds(elapsedTime / (animationSpeed * 86400)); // Adjust start time to account for pause duration
            }
            else
            {
                isPaused = true;
                PauseResumeButton.Content = "Resume";
            }
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            animationSpeed = e.NewValue;
        }
    }
}
