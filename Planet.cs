using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MSSA_FINAL_PROJECT_WORKING
{
    public class Planet : INotifyPropertyChanged
    {
        private string name;
        private Color color;
        private string colorName;
        private double xPos;
        private double yPos;
        private double zPos;
        private double xVel;
        private double yVel;
        private double zVel;
        private double mass;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        public string ColorName
        {
            get => colorName;
            set
            {
                colorName = value;
                Color = (Color)ColorConverter.ConvertFromString(colorName);
                OnPropertyChanged(nameof(ColorName));
            }
        }

        public double XPos
        {
            get => xPos;
            set
            {
                xPos = value;
                OnPropertyChanged(nameof(XPos));
            }
        }

        public double YPos
        {
            get => yPos;
            set
            {
                yPos = value;
                OnPropertyChanged(nameof(YPos));
            }
        }

        public double ZPos
        {
            get => zPos;
            set
            {
                zPos = value;
                OnPropertyChanged(nameof(ZPos));
            }
        }

        public double XVel
        {
            get => xVel;
            set
            {
                xVel = value;
                OnPropertyChanged(nameof(XVel));
            }
        }

        public double YVel
        {
            get => yVel;
            set
            {
                yVel = value;
                OnPropertyChanged(nameof(YVel));
            }
        }

        public double ZVel
        {
            get => zVel;
            set
            {
                zVel = value;
                OnPropertyChanged(nameof(ZVel));
            }
        }

        public double Mass
        {
            get => mass;
            set
            {
                mass = value;
                OnPropertyChanged(nameof(Mass));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
