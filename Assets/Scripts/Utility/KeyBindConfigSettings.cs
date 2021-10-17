using Assets.Scripts.Data_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utility
{
    public static class KeyBindConfigSettings
    {
        public static KeyBindConfigModel KeyBinds { get; set; }

        public static void LoadFromFile()
        {
            //TODO: load keybind settings from settings
            KeyBinds = new KeyBindConfigModel();
        }

        public static void SaveToFile()
        {
            //TODO: save keybinds to settings
        }
    }
}
