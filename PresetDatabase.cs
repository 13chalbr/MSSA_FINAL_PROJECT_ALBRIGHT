using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;

namespace MSSA_FINAL_PROJECT_WORKING
{
    public class Preset : INotifyPropertyChanged
    {
        private string name;
        private string description;
        private List<Planet> planets;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public List<Planet> Planets
        {
            get => planets;
            set
            {
                planets = value;
                OnPropertyChanged(nameof(Planets));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class PresetDatabase
    {
        private static string filePath = "presets.json";

        public static List<Preset> LoadPresets()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Preset>>(json);
            }
            return new List<Preset>();
        }

        public static void SavePreset(Preset preset)
        {
            var presets = LoadPresets();
            presets.Add(preset);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(presets));
        }

        public static void DeletePreset(Preset preset)
        {
            var presets = LoadPresets();
            var presetToRemove = presets.FirstOrDefault(p => p.Name == preset.Name && p.Description == preset.Description);
            if (presetToRemove != null)
            {
                presets.Remove(presetToRemove);
                File.WriteAllText(filePath, JsonConvert.SerializeObject(presets));
            }
        }
    }
}
