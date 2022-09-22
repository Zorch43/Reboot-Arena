using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data_Models;

namespace Assets.Scripts.Data_Templates
{
    public static class ConditionTemplates
    {
        public static UnitConditionModel CreateReloadCondition()
        {
            var template = new UnitConditionModel()
            {
                Duration = 5,
                //unit can't move while reloading (-100% speed)
                UnitMoveSpeedProp = -1,
                //unit can't attack while reloading
                WeaponDisableAutoAttack = true,
                WeaponDisableTargetAttack = true,
                //unit can't use abilities while reloading
                UnitActivatedAbilityDisabled = true,
                UnitTargetedAbilityDisabled = true
            };
            //add expiration effect
            template.OnConditionEnd += (sender, condition) =>
            {
                var unit = sender as DroneController;
                unit.ReloadUnit(unit.Data.UnitClass.MaxMP);
            };
            //TODO: set battlefield visual effect
            //TODO: set portrait visual effect
            return template;
        }
        public static UnitConditionModel CreateKillStreakCondition()
        {
            var template = new UnitConditionModel()
            {
                Duration = 5,
                //increase weapon damage
                WeaponHealthDamageProp = 0.2f,//+20% weapon damage per stack
            };
            template.OnConditionStack += StackIntensity;
            template.OnConditionStack += StackRefreshDuration;
            return template;
        }

        private static void StackIntensity(object sender, UnitConditionModel condition)
        {
            condition.CurrentIntensity += condition.Intensity;
        }
        private static void StackRefreshDuration(object sender, UnitConditionModel condition)
        {
            condition.DurationElapsed = condition.Duration;
        }
        private static void StackDuration(object sender, UnitConditionModel condition)
        {
            condition.DurationElapsed += condition.Duration;
        }
    }
}
