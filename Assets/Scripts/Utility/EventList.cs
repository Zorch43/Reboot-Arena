using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Utility
{
    public static class EventList
    {
        #region enum
        public enum EventNames
        {
            None,
            //input events
            //game menu
            OnInputKeyGameMenuOpen,
            OnInputUIGameMenuOpen,
            //camera input
            OnInputKeyCameraReset,
            //key camera movement
            OnInputKeyCameraUp,
            OnInputKeyCameraRight,
            OnInputKeyCameraDown,
            OnInputKeyCameraLeft,
            //mouse camera movement
            OnInputMouseCameraUp,
            OnInputMouseCameraRight,
            OnInputMouseCameraDown,
            OnInputMouseCameraLeft,
            //key camera fast movement
            OnInputKeyCameraUpFast,
            OnInputKeyCameraRightFast,
            OnInputKeyCameraDownFast,
            OnInputKeyCameraLeftFast,
            //mouse camera fast movement
            OnInputMouseCameraUpFast,
            OnInputMouseCameraRightFast,
            OnInputMouseCameraDownFast,
            OnInputMouseCameraLeftFast,
            //camera rotation
            OnInputKeyCameraRotateCW,
            OnInputKeyCameraRotateCCW,
            //camera tilt
            OnInputKeyCameraTiltUp,
            OnInputKeyCameraTiltDown,
            //camera zoom
            OnInputKeyCameraZoomIn,
            OnInputKeyCameraZoomOut,
            //minimap jump and drag
            OnInputMouseMinimapJump,
            OnInputMouseMinimapDrag,
            //selection
            OnInputSelect,
            OnInputSelectAdd,
            OnInputSelectRemove,
            OnInputSelectAreaStart,//may not need this one, but better to have it in place in case I do
            OnInputSelectAreaFinish,
            OnInputSelectClass,
            OnInputKeySelectAll,
            //slot selection
            OnInputKeySelectSlot1,
            OnInputKeySelectSlot2,
            OnInputKeySelectSlot3,
            OnInputKeySelectSlot4,
            OnInputKeySelectSlot5,
            OnInputKeySelectSlot6,
            OnInputKeySelectSlot7,
            OnInputKeySelectSlot8,
            OnInputKeySelectSlot9,
            OnInputKeySelectSlotAny,//for when any slot selection key is pressed, rather than a specific one
            OnInputUISelectSlot1,
            OnInputUISelectSlot2,
            OnInputUISelectSlot3,
            OnInputUISelectSlot4,
            OnInputUISelectSlot5,
            OnInputUISelectSlot6,
            OnInputUISelectSlot7,
            OnInputUISelectSlot8,
            OnInputUISelectSlot9,
            OnInputUISelectSlotAny,//for when any slot selection icon is pressed, rather than a specific one
            //orders
            //basic orders
            OnInputMove,
            OnInputAttack,
            OnInputSupport,
            //special orders
            //attack-move
            OnInputUIAttackMoveMode,
            OnInputKeyAttackMoveMode,
            OnInputAttackMoveSet,
            OnInputKeyAttackMove,
            //force attack
            OnInputUIForceAttackMode,
            OnInputKeyForceAttackMode,
            OnInputForceAttackSet,
            OnInputKeyForceAttack,
            //set rally point
            OnInputUIRallyMode,
            OnInputKeyRallyMode,
            OnInputRallySet,
            OnInputKeyRally,
            //cancel orders
            OnInputUICancel,
            OnInputKeyCancel,
            //class menu
            OnInputKeyOpenClassMenu,
            OnInputUIOpenClassMenu,
            OnInputKeyCloseClassMenu,
            OnInputUICloseClassMenu,
            OnInputKeySwitchTrooper,
            OnInputKeySwitchRanger,
            OnInputKeySwitchFabricator,
            OnInputKeySwitchClass4,//placeholder for new classes
            OnInputKeySwitchClass5,
            OnInputKeySwitchClass6,
            OnInputKeySwitchClass7,
            OnInputKeySwitchClass8,
            OnInputKeySwitchClass9,
            OnInputUISwitchTrooper,
            OnInputUISwitchRanger,
            OnInputUISwitchFabricator,
            OnInputUISwitchClass4,
            OnInputUISwitchClass5,
            OnInputUISwitchClass6,
            OnInputUISwitchClass7,
            OnInputUISwitchClass8,
            OnInputUISwitchClass9,
            OnInputKeyQuickSwitchTrooper,
            OnInputKeyQuickSwitchRanger,
            OnInputKeyQuickSwitchFabricator,
            OnInputKeyQuickSwitchClass4,
            OnInputKeyQuickSwitchClass5,
            OnInputKeyQuickSwitchClass6,
            OnInputKeyQuickSwitchClass7,
            OnInputKeyQuickSwitchClass8,
            OnInputKeyQuickSwitchClass9,
            //Special abilities
            //grenade
            OnInputKeyAbilityGrenade,
            OnInputUIAbilityGrenade,
            OnInputAbilityGrenadeSet,
            //turret
            OnInputKeyAbilityTurret,
            OnInputUIAbilityTurret,
            OnInputAbilityTurretSet,
            //nanopack
            OnInputKeyAbilityNanoPack,
            OnInputUIAbilityNanoPack,
            OnInputAbilityNanoPackSet,//ability doesn't need a target, but just in case it ever does in the future
            //placeholder 4
            OnInputKeyAbility4,
            OnInputUIAbility4,
            OnInputAbility4Set,
            //placeholder 5
            OnInputKeyAbility5,
            OnInputUIAbility5,
            OnInputAbility5Set,
            //placeholder 6
            OnInputKeyAbility6,
            OnInputUIAbility6,
            OnInputAbility6Set,
            //placeholder 7
            OnInputKeyAbility7,
            OnInputUIAbility7,
            OnInputAbility7Set,
            //placeholder 8
            OnInputKeyAbility8,
            OnInputUIAbility8,
            OnInputAbility8Set,
            //placeholder 9
            OnInputKeyAbility9,
            OnInputUIAbility9,
            OnInputAbility9Set
        }
        #endregion
        #region fields
        private static Dictionary<string, UnityEvent> eventDictionary;
        #endregion
        #region public methods
        public static UnityEvent GetEvent(string eventName)
        {
            EventNames name;
            if(Enum.TryParse(eventName, out name))
            {
                return GetEvent(name);
            }
            else
            {
                Debug.LogError(string.Format("Event name '{0}' not recognized."));
                return null;
            }
        }
        public static UnityEvent GetEvent(EventNames eventName)
        {
            UnityEvent result;
            if(eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, UnityEvent>();
            }
            if(!eventDictionary.TryGetValue(eventName.ToString(), out result))
            {
                result = new UnityEvent();
                eventDictionary.Add(eventName.ToString(), result);
            }
            return result;
        }
        #endregion
    }
}
