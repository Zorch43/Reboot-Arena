using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class UnitAbilityModel
    {
        #region constants
        public enum GroupActivationType
        {
            Single,//the "Best" unit in the group fires the ability
            Inactive,//all units in the group that have not already toggled the ability activate the ability.
                     //if all units are active, deactivate the ability instead.
            All//all units activate the ability
        }
        #endregion
        //identity
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }//name of the icon to use for the ability icon
        public string Cursor { get; set; }//targeting cursor, if it needs one
        public string Marker { get; set; }//ability marker, if it needs one
        //Type
        public bool IsTargetedAbility { get; set; }//whether the ability needs a target.
        public bool IsWeaponAbility { get; set; }//whether the ability fires the ability weapon
        public PickupController.PickupType[] LootDrop { get; set; }
        public float SelfHeal { get; set; }//amount to heal self by when activated
        public bool IsBuildAbility { get; set; }//whether the ability deploys a drone
        public string DroneTemplate { get; set; }//name of drone prefab to deploy
        public string DroneHologram { get; set; }//hologram to display when picking build spot for drone
        public bool IsContinuous { get; set; }//whether the effect repeats until it can't be activated anymore.  if false, only fires once
        public bool IsToggledAbility { get; set; }//whether the ability activates a passive ability until it is deactivated
        public bool IsNonInterrupting { get; set; }//whether activating the ability cancels the unit's other orders
        //ability properties
        public float AmmoCostInstant { get; set; }//the ammo cost for activating the ability (toggles, single-fire abilities)
        public float AmmoCostContinuous { get; set; }//the ammo cost for each repeat activation (effects during toggle, multi-fire abilities)
        //effects
        public WeaponModel AbilityWeapon { get; set; }//if the ability uses any weapon stats, use these stats.

        //group activation rules
        public GroupActivationType GroupActivationRule { get; set; }
        //if group activation role is Single, consider these properties when picking the unit that will fire the ability
        public bool ConsiderLeastTotalDistanceToTarget { get; set; }
        public bool ConsiderLeastMoveDistanceToTarget { get; set; }
        public bool ConsiderMostAmmoInGroup { get; set; }
        public bool ConsiderLeastAmmoInGroup { get; set; }
        public bool ConsiderMostHealthInGroup { get; set; }
        public bool ConsiderLeastHealthInGroup { get; set; }

        //event names
        public EventList.EventNames EventNameUI { get; set; }//name of the event triggered by clicking on icon
        public EventList.EventNames EventNameKey { get; set; }//name of the event triggered by tapping hotkey
        public EventList.EventNames EventNameSet { get; set; }//name of event triggered by setting ability target
    }
}
