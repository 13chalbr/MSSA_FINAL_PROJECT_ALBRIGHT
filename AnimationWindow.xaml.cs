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

        public AnimationWindow(List<List<double>> xList, List<List<double>> yList, List<List<double>> zList, List<string> planetNames, List<Color> planetColors)
        {
            InitializeComponent();

            var viewport = new HelixViewport3D();
            var modelGroup = new Model3DGroup();

            // Create and add axes
            var axes = new LinesVisual3D
            {
                Color = Colors.Black,
                Thickness = 2
            };
            axes.Points.Add(new Point3D(-10, 0, 0));
            axes.Points.Add(new Point3D(10, 0, 0)); // X axis
            axes.Points.Add(new Point3D(0, -10, 0));
            axes.Points.Add(new Point3D(0, 10, 0)); // Y axis
            axes.Points.Add(new Point3D(0, 0, -10));
            axes.Points.Add(new Point3D(0, 0, 10)); // Z axis

            // Add tick marks for X axis
            for (double i = -10; i <= 10; i++)
            {
                axes.Points.Add(new Point3D(i, -0.1, 0));
                axes.Points.Add(new Point3D(i, 0.1, 0));
            }

            // Add tick marks for Y axis
            for (double i = -10; i <= 10; i++)
            {
                axes.Points.Add(new Point3D(-0.1, i, 0));
                axes.Points.Add(new Point3D(0.1, i, 0));
            }

            // Add tick marks for Z axis
            for (double i = -10; i <= 10; i++)
            {
                axes.Points.Add(new Point3D(0, -0.1, i));
                axes.Points.Add(new Point3D(0, 0.1, i));
            }

            viewport.Children.Add(axes);

            // Add axis labels
            var xLabel = new BillboardTextVisual3D
            {
                Text = "X (units)",
                Position = new Point3D(10, 0, 0),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            var yLabel = new BillboardTextVisual3D
            {
                Text = "Y (units)",
                Position = new Point3D(0, 10, 0),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            var zLabel = new BillboardTextVisual3D
            {
                Text = "Z (units)",
                Position = new Point3D(0, 0, 10),
                Foreground = new SolidColorBrush(Colors.Black)
            };
            viewport.Children.Add(xLabel);
            viewport.Children.Add(yLabel);
            viewport.Children.Add(zLabel);

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

                // Create a sphere to represent the planet
                var sphere = new SphereVisual3D
                {
                    Radius = 0.2,
                    Fill = new SolidColorBrush(planet.Color)
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

            // Start the animation
            startTime = DateTime.Now;
            animationStarted = true;
            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!animationStarted || isPaused) return;

            elapsedTime = (DateTime.Now - startTime).TotalSeconds * animationSpeed;
            var duration = 10.0; // Total duration of the animation in seconds

            // Restart elapsed time if it exceeds the duration
            elapsedTime %= duration;

            // Update the elapsed time display
            ElapsedTimeTextBlock.Text = $"Elapsed Time: {elapsedTime:F2} seconds";

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

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            isPaused = true;
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            isPaused = false;
            startTime = DateTime.Now - TimeSpan.FromSeconds(elapsedTime / animationSpeed); // Adjust start time to account for pause duration
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            animationSpeed = e.NewValue;
        }
    }
}
