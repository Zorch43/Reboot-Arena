using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data_Models
{
    public class KeyBindModel : IComparable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public KeyCode HeldKey { get; set; }
        public KeyCode PressedKey { get; set; }

        public KeyBindModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public KeyBindModel(string name, string description, KeyCode pressedKey)
        {
            Name = name;
            Description = description;
            PressedKey = pressedKey;
            HeldKey = KeyCode.None;
        }
        public KeyBindModel(string name, string description, KeyCode heldKey, KeyCode pressedKey)
        {
            Name = name;
            Description = description;
            PressedKey = pressedKey;
            HeldKey = heldKey;
        }
        public override bool Equals(object obj)
        {
            var other = obj as KeyBindModel;

            return other != null && HeldKey == other.HeldKey && PressedKey == other.PressedKey;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            //a keybind is prioritized over another if it shares a pressed key, but has a held key (and the other does not)
            //otherwise, order doesn't matter
            var other = obj as KeyBindModel;
            if(obj != null)
            {
                if(PressedKey == other.PressedKey)
                {
                    if(HeldKey != KeyCode.None && other.HeldKey == KeyCode.None)
                    {
                        return 1;
                    }
                    else if(HeldKey == KeyCode.None && other.HeldKey != KeyCode.None)
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }
    }
}
