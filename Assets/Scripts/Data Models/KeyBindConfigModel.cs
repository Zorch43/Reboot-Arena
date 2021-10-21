using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data_Models
{
    public class KeyBindConfigModel
    {
        public KeyBindModel GameMenuKey { get; set; }
        //unit slots
        public KeyBindModel UnitSlot1Key { get; set; }
        public KeyBindModel UnitSlot2Key { get; set; }
        public KeyBindModel UnitSlot3Key { get; set; }
        public KeyBindModel UnitSlot4Key { get; set; }
        public KeyBindModel UnitSlot5Key { get; set; }
        public KeyBindModel UnitSlot6Key { get; set; }
        public KeyBindModel UnitSlot7Key { get; set; }
        public KeyBindModel UnitSlot8Key { get; set; }
        public KeyBindModel UnitSlot9Key { get; set; }
        //advanced commands
        public KeyBindModel AttackMoveModeKey { get; set; }
        public KeyBindModel AttackMoveKey { get; set; }
        public KeyBindModel ForceAttackModeKey { get; set; }
        public KeyBindModel ForceAttackKey { get; set; }
        public KeyBindModel StopActionKey { get; set; }
        public KeyBindModel SetRallyPointModeKey { get; set; }
        public KeyBindModel SetRallyPointKey { get; set; }
        //ability hotkeys
        public KeyBindModel AbilityGrenadeKey { get; set; }

        public List<KeyBindModel> AllKeyBinds { get; set; }

        public List<KeyCode> ValidHeldKeys { get; set; }

        public KeyBindConfigModel()
        {
            GameMenuKey = new KeyBindModel("Game Menu", "Open the in-game menu", KeyCode.Escape);
            //unit slots
            UnitSlot1Key = new KeyBindModel("Unit Slot 1", "Select the unit in slot 1", KeyCode.Alpha1);
            UnitSlot2Key = new KeyBindModel("Unit Slot 2", "Select the unit in slot 2", KeyCode.Alpha2);
            UnitSlot3Key = new KeyBindModel("Unit Slot 3", "Select the unit in slot 3", KeyCode.Alpha3);
            UnitSlot4Key = new KeyBindModel("Unit Slot 4", "Select the unit in slot 4", KeyCode.Alpha4);
            UnitSlot5Key = new KeyBindModel("Unit Slot 5", "Select the unit in slot 5", KeyCode.Alpha5);
            UnitSlot6Key = new KeyBindModel("Unit Slot 6", "Select the unit in slot 6", KeyCode.Alpha6);
            UnitSlot7Key = new KeyBindModel("Unit Slot 7", "Select the unit in slot 7", KeyCode.Alpha7);
            UnitSlot8Key = new KeyBindModel("Unit Slot 8", "Select the unit in slot 8", KeyCode.Alpha8);
            UnitSlot9Key = new KeyBindModel("Unit Slot 9", "Select the unit in slot 9", KeyCode.Alpha9);

            //advanced commands
            AttackMoveModeKey = new KeyBindModel("Set Attack-Move Mode",
                "Activate the attack-move command mode.  Set location with left-click, cancel with right-click.",
                KeyCode.A);
            AttackMoveKey = new KeyBindModel("Attack-Move", "Selected units will attack-move to cursor location");//key bind not set
            ForceAttackModeKey = new KeyBindModel("Set Force-Attack Mode",
                "Activate the force-attack command mode.  Set force-attack with left-click, cancel with right-click",
                KeyCode.F);
            ForceAttackKey = new KeyBindModel("Force-Attack", "Selected units will force-attack the cursor location", KeyCode.LeftControl, KeyCode.Mouse1);
            StopActionKey = new KeyBindModel("Stop", "Selected units stop whatever they were doing", KeyCode.S);
            SetRallyPointModeKey = new KeyBindModel("Set Rally-Point Mode",
                "Activate rally-point mode.  Set the rally point for selected units with left-click, cancel with right-click.",
                KeyCode.R);
            SetRallyPointKey = new KeyBindModel("Set Rally-Point", "Set the rally-point to the cursor position for all selected units.", 
                KeyCode.LeftControl, KeyCode.R);

            //special abilities
            AbilityGrenadeKey = new KeyBindModel("Throw Frag Grenade", "Throw a high-explosive grenade at the targeted location", KeyCode.G);

            //integration
            AllKeyBinds = new List<KeyBindModel>()
            {
                GameMenuKey,
                UnitSlot1Key,
                UnitSlot2Key,
                UnitSlot3Key,
                UnitSlot4Key,
                UnitSlot5Key,
                UnitSlot6Key,
                UnitSlot7Key,
                UnitSlot8Key,
                UnitSlot9Key,
                AttackMoveModeKey,
                AttackMoveKey,
                ForceAttackModeKey,
                ForceAttackKey,
                StopActionKey,
                SetRallyPointModeKey,
                SetRallyPointKey,
                AbilityGrenadeKey
            };
            AllKeyBinds.Sort();

            ValidHeldKeys = new List<KeyCode>() 
            { 
                KeyCode.LeftAlt,
                KeyCode.RightAlt,
                KeyCode.LeftCommand,
                KeyCode.RightCommand,
                KeyCode.LeftControl,
                KeyCode.RightControl,
                KeyCode.LeftShift, 
                KeyCode.RightShift 
            };
        }
    }
}
