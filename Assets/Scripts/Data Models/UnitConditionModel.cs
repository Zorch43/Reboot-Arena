using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Data_Models
{
    public class UnitConditionModel
    {
        //basic condition properties
        public string Name { get; set; }
        public UnitController Owner { get; set; }//which unit applied this condition
        public UnitController Host { get; set; }//which unit has this condition
        //condition visual properties
        public string ConsoleLine { get; set; }
        public TextMeshProUGUI ConsoleLineController { get; set; }
        //TODO: battlefield visuals

        //condition duration properties
        public float Duration { get; set; }
        public float DurationElapsed { get; set; }
        //condition intensity properties
        public float Intensity { get; set; } = 1;
        public float CurrentIntensity { get; set; }

        //unit stat modifiers
        public float UnitMoveSpeedFlat { get; set; }//result is sum of unit stat and all condition modifiers
        public float UnitMoveSpeedProp { get; set; }//result is unit stat * (1 + sum of all condition modifiers)
        public float UnitTurnSpeedFlat { get; set; }//result is sum of unit stat and all condition modifiers
        public float UnitTurnSpeedProp { get; set; }//result is unit stat * (1 + sum of all condition modifiers)

        //extra unit modifiers
        public bool UnitHasJumpBoost { get; set; }//result is OR of all condition modifiers
        public bool UnitTargetedAbilityDisabled { get; set; }//result is OR of all condition modifiers
        public bool UnitActivatedAbilityDisabled { get; set; }//result is OR of all condition modifiers

        //weapon stat modifiers
        public float WeaponMaxRangeFlat { get; set; }
        public float WeaponMaxRangeProp { get; set; }
        public float WeaponMinRangeFlat { get; set; }
        public float WeaponMinRangeProp { get; set; }
        public float WeaponProjectileSpeedProp { get; set; }
        public float WeaponCooldownProp { get; set; }
        public float WeaponHealthDamageProp { get; set; }
        public float WeaponAmmoDamageProp { get; set; }
        public float WeaponInaccuracyFlat { get; set; }
        public float WeaponInaccuracyProp { get; set; }
        public bool WeaponPiercesUnits { get; set; }
        public bool WeaponPiercesWalls { get; set; }
        public float WeaponAmmoCostFlat { get; set; }
        public float WeaponAmmoCostProp { get; set; }
        public bool WeaponDisableFireWhileMoving { get; set; }
        public bool WeaponDisableAutoAttack { get; set; }
        public bool WeaponDisableTargetAttack { get; set; }
        //loadout modifiers
        public WeaponModel PrimaryWeaponReplacement { get; set; }//use the last non-null weapon from conditions
        public WeaponModel SecondaryWeaponReplacement { get; set; }
        //ability modifiers
        public UnitAbilityModel TargetedAbilityReplacement { get; set; }
        public UnitAbilityModel ActivatedAbilityReplacement { get; set; }
        //trigger events
        //condition triggers
        public event EventHandler<float> OnTimeElapsed;//triggered when time elapses
        public event EventHandler<UnitConditionModel> OnConditionStart;//triggered when this condition is applied, but not stacked
        public event EventHandler<UnitConditionModel> OnConditionStack;//triggered whent this condition is stacked
        public event EventHandler<UnitConditionModel> OnConditionEnd;//condition when condition ends
        //TODO: unit triggers
        

        //constructor
        public UnitConditionModel()
        {
            OnTimeElapsed += CountDown;
        }
        //public methods
        //event invokers (can't do it outside class?)
        public void DoTimeElapsed(object sender, float deltaTime)
        {
            OnTimeElapsed?.Invoke(sender, deltaTime);
        }
        public void DoConditionStart(object sender, UnitConditionModel condition)
        {
            OnConditionStart?.Invoke(sender, condition);
        }
        public void DoConditionStack(object sender, UnitConditionModel condition)
        {
            OnConditionStack?.Invoke(sender, condition);
        }
        public void DoConditionEnd(object sender, UnitConditionModel condition)
        {
            OnConditionEnd?.Invoke(sender, condition);
        }
        //private methods
        //decrease the remaining duration as time passes
        //only add when duration > 0
        private void CountDown(object sender, float deltaTime)
        {
            if(Duration > 0)
            {
                if (DurationElapsed >= Duration)
                {
                    //expire
                    var unit = sender as DroneController;
                    unit.RemoveCondition(this);
                }
                else
                {
                    DurationElapsed += deltaTime;
                }
            }
            
        }
    }
}
