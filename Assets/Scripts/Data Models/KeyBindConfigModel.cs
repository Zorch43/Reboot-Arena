﻿using Assets.Scripts.Utility;
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
            AbilityGrenade,
            AbilityTurret,
            AbilityNanoPack,
            ClassMenuToggle,
            ClassSwitchTrooper,
            ClassSwitchFabricator,
            ClassSwitchRanger,
            ClassSwitchQuickTrooper,
            ClassSwitchQuickFabricator,
            ClassSwitchQuickRanger,
            CameraPanLeft,
            CameraPanRight,
            CameraPanUp,
            CameraPanDown,
            CameraRotateCW,
            CameraRotateCCW,
            CameraTiltUp,
            CameraTiltDown,
            CameraZoomIn,
            CameraZoomOut,
            CameraReset,
            SelectAll
        }
        
        #endregion
        #region keybinds
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
        public KeyBindModel AbilityTurretKey { get; set; }
        public KeyBindModel AbilityNanoPackKey { get; set; }
        public KeyBindModel ClassMenuToggle { get; set; }
        public KeyBindModel ClassSwitchTrooper { get; set; }
        public KeyBindModel ClassSwitchFabricator { get; set; }
        public KeyBindModel ClassSwitchRanger { get; set; }
        public KeyBindModel ClassSwitchQuickTrooper { get; set; }
        public KeyBindModel ClassSwitchQuickFabricator { get; set; }
        public KeyBindModel ClassSwitchQuickRanger { get; set; }
        public KeyBindModel CameraPanLeft { get; set; }
        public KeyBindModel CameraPanRight { get; set; }
        public KeyBindModel CameraPanUp { get; set; }
        public KeyBindModel CameraPanDown { get; set; }
        public KeyBindModel CameraRotateCW { get; set; }
        public KeyBindModel CameraRotateCCW { get; set; }
        public KeyBindModel CameraTiltUp { get; set; }
        public KeyBindModel CameraTiltDown { get; set; }
        public KeyBindModel CameraZoomIn { get; set; }
        public KeyBindModel CameraZoomOut { get; set; }
        public KeyBindModel CameraReset { get; set; }
        public KeyBindModel SelectAll { get; set; }
        #endregion
        public List<KeyBindModel> AllKeyBinds { get; set; }

        public List<KeyCode> ValidHeldKeys { get; set; }

        public KeyBindConfigModel()
        {
            GameMenuKey = new KeyBindModel(KeyBindId.GameMenu, EventList.EventNames.OnInputKeyGameMenuOpen, 
                "Game Menu", "Open the in-game menu", KeyCode.Escape);
            //unit slots
            UnitSlot1Key = new KeyBindModel(KeyBindId.UnitSlot1, EventList.EventNames.OnInputKeySelectSlot1, 
                "Unit Slot 1", "Select the unit in slot 1", KeyCode.Alpha1);
            UnitSlot2Key = new KeyBindModel(KeyBindId.UnitSlot2, EventList.EventNames.OnInputKeySelectSlot2, 
                "Unit Slot 2", "Select the unit in slot 2", KeyCode.Alpha2);
            UnitSlot3Key = new KeyBindModel(KeyBindId.UnitSlot3, EventList.EventNames.OnInputKeySelectSlot3, 
                "Unit Slot 3", "Select the unit in slot 3", KeyCode.Alpha3);
            UnitSlot4Key = new KeyBindModel(KeyBindId.UnitSlot4, EventList.EventNames.OnInputKeySelectSlot4, 
                "Unit Slot 4", "Select the unit in slot 4", KeyCode.Alpha4);
            UnitSlot5Key = new KeyBindModel(KeyBindId.UnitSlot5, EventList.EventNames.OnInputKeySelectSlot5, 
                "Unit Slot 5", "Select the unit in slot 5", KeyCode.Alpha5);
            UnitSlot6Key = new KeyBindModel(KeyBindId.UnitSlot6, EventList.EventNames.OnInputKeySelectSlot6, 
                "Unit Slot 6", "Select the unit in slot 6", KeyCode.Alpha6);
            UnitSlot7Key = new KeyBindModel(KeyBindId.UnitSlot7, EventList.EventNames.OnInputKeySelectSlot7, 
                "Unit Slot 7", "Select the unit in slot 7", KeyCode.Alpha7);
            UnitSlot8Key = new KeyBindModel(KeyBindId.UnitSlot8, EventList.EventNames.OnInputKeySelectSlot8, 
                "Unit Slot 8", "Select the unit in slot 8", KeyCode.Alpha8);
            UnitSlot9Key = new KeyBindModel(KeyBindId.UnitSlot9, EventList.EventNames.OnInputKeySelectSlot9, 
                "Unit Slot 9", "Select the unit in slot 9", KeyCode.Alpha9);

            //advanced commands
            AttackMoveModeKey = new KeyBindModel(KeyBindId.AttackMoveMode, EventList.EventNames.OnInputKeyAttackMoveMode,
                "Set Attack-Move Mode", "Activate the attack-move command mode.  Set location with left-click, cancel with right-click.", KeyCode.V);
            AttackMoveKey = new KeyBindModel(KeyBindId.AttackMove, /*EventList.EventNames.OnInputKeyAttackMove,*/ 
                "Attack-Move", "Selected units will attack-move to cursor location");//key bind not set
            ForceAttackModeKey = new KeyBindModel(KeyBindId.ForceAtackMode, EventList.EventNames.OnInputKeyForceAttackMode, "Set Force-Attack Mode", 
                "Activate the force-attack command mode.  Set force-attack with left-click, cancel with right-click", KeyCode.F);
            ForceAttackKey = new KeyBindModel(KeyBindId.ForceAttack, EventList.EventNames.OnInputKeyForceAttack, "Force-Attack", 
                "Selected units will force-attack the cursor location", KeyCode.LeftControl, KeyCode.Mouse1);
            StopActionKey = new KeyBindModel(KeyBindId.StopAction, EventList.EventNames.OnInputKeyCancel, "Stop", 
                "Selected units stop whatever they were doing", KeyCode.C);
            SetRallyPointModeKey = new KeyBindModel(KeyBindId.SetRallyPointMode, EventList.EventNames.OnInputKeyRallyMode, "Set Rally-Point Mode",
                "Activate rally-point mode.  Set the rally point for selected units with left-click, cancel with right-click.", KeyCode.R);
            SetRallyPointKey = new KeyBindModel(KeyBindId.SetRallyPoint, EventList.EventNames.OnInputKeyRally, "Set Rally-Point", 
                "Set the rally-point to the cursor position for all selected units.", KeyCode.LeftControl, KeyCode.R);

            //special abilities
            AbilityGrenadeKey = new KeyBindModel(KeyBindId.AbilityGrenade, EventList.EventNames.OnInputKeyAbilityGrenade, "Grenade", 
                "Throw a high-explosive grenade at the targeted location", KeyCode.G);
            AbilityTurretKey = new KeyBindModel(KeyBindId.AbilityTurret, EventList.EventNames.OnInputKeyAbilityTurret, "Turret", 
                "Start building a turret drone armed with a plasma gun.", KeyCode.T);
            AbilityNanoPackKey = new KeyBindModel(KeyBindId.AbilityNanoPack, EventList.EventNames.OnInputKeyAbilityNanoPack, 
                "Throw NanoPack", "Throw a freshly-printed nanopack nearby.", KeyCode.N);

            //class menu
            ClassMenuToggle = new KeyBindModel(KeyBindId.ClassMenuToggle, EventList.EventNames.OnInputKeyOpenClassMenu, "Class Menu", 
                "Open or close the class menu to change what class selected units will respawn as", KeyCode.Tab);
            //TODO: add separate keybind to close class menu
            ClassSwitchTrooper = new KeyBindModel(KeyBindId.ClassSwitchTrooper, EventList.EventNames.OnInputKeySwitchTrooper, "Trooper", 
                "Change class to Trooper", KeyCode.G);
            ClassSwitchQuickTrooper = new KeyBindModel(KeyBindId.ClassSwitchQuickTrooper, EventList.EventNames.OnInputKeyQuickSwitchTrooper,
                "Quick-Switch to Trooper", "Instantly change all selected units' next class to Trooper", KeyCode.LeftControl, KeyCode.G);
            ClassSwitchFabricator = new KeyBindModel(KeyBindId.ClassSwitchFabricator, EventList.EventNames.OnInputKeySwitchFabricator, "Fabricator",
                "Change class to Fabricator", KeyCode.T);
            ClassSwitchQuickFabricator = new KeyBindModel(KeyBindId.ClassSwitchQuickFabricator, EventList.EventNames.OnInputKeyQuickSwitchFabricator, 
                "Quick-Switch to Fabricator", "Instantly change all selected units' next class to Fabricator", KeyCode.LeftControl, KeyCode.T);
            ClassSwitchRanger = new KeyBindModel(KeyBindId.ClassSwitchRanger, EventList.EventNames.OnInputKeySwitchRanger, "Ranger",
                "Change class to Ranger", KeyCode.N);
            ClassSwitchQuickRanger = new KeyBindModel(KeyBindId.ClassSwitchQuickFabricator, EventList.EventNames.OnInputKeyQuickSwitchRanger, 
                "Quick-Switch to Ranger", "Instantly change all selected units' next class to Ranger", KeyCode.LeftControl, KeyCode.N);

            //camera controls
            CameraPanDown = new KeyBindModel(KeyBindId.CameraPanDown, EventList.EventNames.OnInputKeyCameraDown, 
                "Pan Down", "Pan the main camera down", KeyCode.S, true);
            CameraPanLeft = new KeyBindModel(KeyBindId.CameraPanLeft, EventList.EventNames.OnInputKeyCameraLeft, 
                "Pan Left", "Pan the main camera left", KeyCode.A, true);
            CameraPanRight = new KeyBindModel(KeyBindId.CameraPanRight, EventList.EventNames.OnInputKeyCameraRight, 
                "Pan Right", "Pan the main camera right", KeyCode.D, true);
            CameraPanUp = new KeyBindModel(KeyBindId.CameraPanUp, EventList.EventNames.OnInputKeyCameraUp, 
                "Pan Up", "Pan the main camera up", KeyCode.W, true);
            CameraRotateCCW = new KeyBindModel(KeyBindId.CameraRotateCCW, EventList.EventNames.OnInputKeyCameraRotateCCW, 
                "Rotate CCW", "Rotate the main camera counter-clockwise around the focus point", KeyCode.Q, true);
            CameraRotateCW = new KeyBindModel(KeyBindId.CameraRotateCW, EventList.EventNames.OnInputKeyCameraRotateCW, 
                "Rotate CW", "Rotate the main camera clockwise around the focus point", KeyCode.E, true);
            CameraTiltDown = new KeyBindModel(KeyBindId.CameraTiltDown, EventList.EventNames.OnInputKeyCameraTiltDown, 
                "Tilt Down", "Tilt the main camera down", KeyCode.Z, true);
            CameraTiltUp = new KeyBindModel(KeyBindId.CameraTiltUp, EventList.EventNames.OnInputKeyCameraTiltUp, 
                "Tilt Up", "Tilt the maincamera up", KeyCode.X, true);
            CameraZoomIn = new KeyBindModel(KeyBindId.CameraZoomIn, EventList.EventNames.OnInputKeyCameraZoomIn, 
                "Zoom In", "Zoom the main camera in", KeyBindModel.KeyCodeExtra.MouseWheelUp, true);
            CameraZoomOut = new KeyBindModel(KeyBindId.CameraZoomOut, EventList.EventNames.OnInputKeyCameraZoomOut, 
                "Zoom Out", "Zoom the main camera out", KeyBindModel.KeyCodeExtra.MouseWheelDown, true);
            CameraReset = new KeyBindModel(KeyBindId.CameraReset, EventList.EventNames.OnInputKeyCameraReset,
                "Reset", "Reset main camera orientation", KeyCode.Backspace);

            //selection
            SelectAll = new KeyBindModel(KeyBindId.SelectAll, EventList.EventNames.OnInputKeySelectAll,
                "Select All", "Selectall of your units in view", KeyCode.LeftControl, KeyCode.A);

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
                AbilityGrenadeKey,
                AbilityTurretKey,
                ClassMenuToggle,
                ClassSwitchFabricator,
                ClassSwitchQuickFabricator,
                ClassSwitchTrooper,
                ClassSwitchQuickTrooper,
                ClassSwitchRanger,
                ClassSwitchQuickRanger,
                CameraPanDown,
                CameraPanUp,
                CameraPanLeft,
                CameraPanRight,
                CameraRotateCW,
                CameraRotateCCW,
                CameraTiltUp,
                CameraTiltDown,
                CameraZoomIn,
                CameraZoomOut,
                CameraReset,
                SelectAll
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
