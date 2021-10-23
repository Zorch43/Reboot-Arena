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
        #region enum
        public enum KeyBindId
        {
            None,
            GameMenu,
            UnitSlot1,
            UnitSlot2,
            UnitSlot3,
            UnitSlot4,
            UnitSlot5,
            UnitSlot6,
            UnitSlot7,
            UnitSlot8,
            UnitSlot9,
            AttackMoveMode,
            AttackMove,
            ForceAtackMode,
            ForceAttack,
            StopAction,
            SetRallyPointMode,
            SetRallyPoint,
            AbilityGrenade
        }
        #endregion
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
            GameMenuKey = new KeyBindModel(KeyBindId.GameMenu, "Game Menu", "Open the in-game menu", KeyCode.Escape);
            //unit slots
            UnitSlot1Key = new KeyBindModel(KeyBindId.UnitSlot1, "Unit Slot 1", "Select the unit in slot 1", KeyCode.Alpha1);
            UnitSlot2Key = new KeyBindModel(KeyBindId.UnitSlot2, "Unit Slot 2", "Select the unit in slot 2", KeyCode.Alpha2);
            UnitSlot3Key = new KeyBindModel(KeyBindId.UnitSlot3, "Unit Slot 3", "Select the unit in slot 3", KeyCode.Alpha3);
            UnitSlot4Key = new KeyBindModel(KeyBindId.UnitSlot4, "Unit Slot 4", "Select the unit in slot 4", KeyCode.Alpha4);
            UnitSlot5Key = new KeyBindModel(KeyBindId.UnitSlot5, "Unit Slot 5", "Select the unit in slot 5", KeyCode.Alpha5);
            UnitSlot6Key = new KeyBindModel(KeyBindId.UnitSlot6, "Unit Slot 6", "Select the unit in slot 6", KeyCode.Alpha6);
            UnitSlot7Key = new KeyBindModel(KeyBindId.UnitSlot7, "Unit Slot 7", "Select the unit in slot 7", KeyCode.Alpha7);
            UnitSlot8Key = new KeyBindModel(KeyBindId.UnitSlot8, "Unit Slot 8", "Select the unit in slot 8", KeyCode.Alpha8);
            UnitSlot9Key = new KeyBindModel(KeyBindId.UnitSlot9, "Unit Slot 9", "Select the unit in slot 9", KeyCode.Alpha9);

            //advanced commands
            AttackMoveModeKey = new KeyBindModel(KeyBindId.AttackMoveMode, "Set Attack-Move Mode",
                "Activate the attack-move command mode.  Set location with left-click, cancel with right-click.",
                KeyCode.A);
            AttackMoveKey = new KeyBindModel(KeyBindId.AttackMove, "Attack-Move", "Selected units will attack-move to cursor location");//key bind not set
            ForceAttackModeKey = new KeyBindModel(KeyBindId.ForceAtackMode, "Set Force-Attack Mode",
                "Activate the force-attack command mode.  Set force-attack with left-click, cancel with right-click",
                KeyCode.F);
            ForceAttackKey = new KeyBindModel(KeyBindId.ForceAttack, "Force-Attack", "Selected units will force-attack the cursor location", KeyCode.LeftControl, KeyCode.Mouse1);
            StopActionKey = new KeyBindModel(KeyBindId.StopAction, "Stop", "Selected units stop whatever they were doing", KeyCode.S);
            SetRallyPointModeKey = new KeyBindModel(KeyBindId.SetRallyPointMode, "Set Rally-Point Mode",
                "Activate rally-point mode.  Set the rally point for selected units with left-click, cancel with right-click.",
                KeyCode.R);
            SetRallyPointKey = new KeyBindModel(KeyBindId.SetRallyPoint, "Set Rally-Point", "Set the rally-point to the cursor position for all selected units.", 
                KeyCode.LeftControl, KeyCode.R);

            //special abilities
            AbilityGrenadeKey = new KeyBindModel(KeyBindId.AbilityGrenade, "Grenade", "Throw a high-explosive grenade at the targeted location", KeyCode.G);

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

        public KeyBindModel GetKeyBindById(KeyBindId id)
        {
            if(id != KeyBindId.None)
            {
                foreach (var k in AllKeyBinds)
                {
                    if (id == k.Id)
                    {
                        return k;
                    }
                }
            }
            
            return null;
        }
        public KeyBindId GetKeyBindByName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                foreach (var k in AllKeyBinds)
                {
                    if (name == k.Name)
                    {
                        return k.Id;
                    }
                }
            }
            return KeyBindId.None;
        }

        public KeyBindId GetUnitSlotKeyBind(int slotNum)
        {
            switch (slotNum)
            {
                case 1:
                    return KeyBindId.UnitSlot1;
                case 2:
                    return KeyBindId.UnitSlot2;
                case 3:
                    return KeyBindId.UnitSlot3;
                case 4:
                    return KeyBindId.UnitSlot4;
                case 5:
                    return KeyBindId.UnitSlot5;
                case 6:
                    return KeyBindId.UnitSlot6;
                case 7:
                    return KeyBindId.UnitSlot7;
                case 8:
                    return KeyBindId.UnitSlot8;
                case 9:
                    return KeyBindId.UnitSlot9;
            }

            return KeyBindId.None;
        }
    }
}
