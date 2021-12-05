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
        public KeyBindConfigModel.KeyBindId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public KeyCode HeldKey { get; set; }
        public KeyCode PressedKey { get; set; }
        public Action BoundAction { get; set; }
        public bool IsContinuous { get; set; }//whether the keybind can be held to activate continuously
        public bool IsExclusive { get; set; }//whether the activation of this keybind prevents activation of other commands

        public KeyBindModel(KeyBindConfigModel.KeyBindId id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public KeyBindModel(KeyBindConfigModel.KeyBindId id, string name, string description, KeyCode pressedKey)
        {
            Id = id;
            Name = name;
            Description = description;
            PressedKey = pressedKey;
            HeldKey = KeyCode.None;
        }
        public KeyBindModel(KeyBindConfigModel.KeyBindId id, string name, string description, KeyCode heldKey, KeyCode pressedKey)
        {
            Id = id;
            Name = name;
            Description = description;
            PressedKey = pressedKey;
            HeldKey = heldKey;
        }
        public bool TryInvoke()
        {
            if (IsPressed())
            {
                BoundAction.Invoke();
                return true;
            }
            return false;
        }
        public bool IsPressed()
        {
            return (HeldKey == KeyCode.None || Input.GetKey(HeldKey)) && ((IsContinuous && Input.GetKey(PressedKey)) || Input.GetKeyDown(PressedKey));
        }
        public override bool Equals(object obj)
        {
            var other = obj as KeyBindModel;

            return other.Id == Id;
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
        public override string ToString()
        {
            string output = "";
            if(HeldKey != KeyCode.None)
            {
                output += HeldKey.ToString() + " + ";
            }
            output += PressedKey.ToString();

            return output;
        }
    }
}
