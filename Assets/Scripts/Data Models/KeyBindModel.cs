using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Data_Models
{
    public class KeyBindModel : IComparable
    {
        #region constants
        public enum KeyCodeExtra
        {
            None,
            MouseWheelUp,
            MouseWheelDown
        }
        #endregion
        #region properties
        public KeyBindConfigModel.KeyBindId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public KeyCode HeldKey { get; set; }
        public KeyCode PressedKey { get; set; }
        public KeyCodeExtra PressedKeyExtra { get; set; }
        public EventList.EventNames EventName { get; set; }
        public UnityEvent BoundEvent
        {
            get
            {
                return EventList.GetEvent(EventName);
            }
        }
        public UnityAction BoundAction
        {
            set
            {
                BoundEvent.AddListener(value);
            }
        }
        public bool IsContinuous { get; set; }//whether the keybind can be held to activate continuously
        #endregion
        #region constructors
        public KeyBindModel(KeyBindConfigModel.KeyBindId id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
            
        }
        public KeyBindModel(KeyBindConfigModel.KeyBindId id, EventList.EventNames eventName, string name, string description, KeyCode pressedKey, bool isContinuous = false) : this(id,name,description)
        {
            EventName = eventName;
            PressedKey = pressedKey;
            HeldKey = KeyCode.None;
            IsContinuous = isContinuous;
        }
        public KeyBindModel(KeyBindConfigModel.KeyBindId id, EventList.EventNames eventName, string name, string description, KeyCodeExtra pressedKey, bool isContinuous = false) : this(id,name,description)
        {
            EventName = eventName;
            PressedKeyExtra = pressedKey;
            HeldKey = KeyCode.None;
            IsContinuous = isContinuous;
        }
        public KeyBindModel(KeyBindConfigModel.KeyBindId id, EventList.EventNames eventName, string name, string description, KeyCode heldKey, KeyCode pressedKey, bool isContinuous = false) : this(id, eventName, name,description,pressedKey,isContinuous)
        {
            HeldKey = heldKey;
        }
        public KeyBindModel(KeyBindConfigModel.KeyBindId id, EventList.EventNames eventName, string name, string description, KeyCode heldKey, KeyCodeExtra pressedKey, bool isContinuous = false) : this(id, eventName, name, description, pressedKey, isContinuous)
        {
            HeldKey = heldKey;
        }
        #endregion
        #region public methods
        public bool TryInvoke()
        {
            if (IsPressed())
            {
                BoundAction.Invoke();//activate the main action
                EventList.GetEvent(EventName).Invoke();
                return true;
            }
            return false;
        }
        public bool IsPressed()
        {
            if(HeldKey == KeyCode.None || Input.GetKey(HeldKey))
            {
                if(PressedKeyExtra != KeyCodeExtra.None)
                {
                    return (IsContinuous && ExtraInput.GetKey(PressedKeyExtra)) || ExtraInput.GetKeyDown(PressedKeyExtra);
                }
                else
                {
                    return (IsContinuous && Input.GetKey(PressedKey)) || Input.GetKeyDown(PressedKey);
                }
            }
            return false;
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
                if (HeldKey != KeyCode.None && other.HeldKey == KeyCode.None)
                {
                    return -1;
                }
                else if (HeldKey == KeyCode.None && other.HeldKey != KeyCode.None)
                {
                    return 1;
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
        #endregion
    }
}
