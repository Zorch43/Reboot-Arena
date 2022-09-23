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
                ConsoleLine = "Reloading",
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
            CreateConsoleProgress_RemainingTime(template, false);
            //TODO: set battlefield visual effect

            return template;
        }

        public static UnitConditionModel CreateKillStreakCondition()
        {
            var template = new UnitConditionModel()
            {
                Duration = 5,
                ConsoleLine = "Kill Streak",
                //increase weapon damage
                WeaponHealthDamageProp = 0.2f,//+20% weapon damage per stack
            };
            template.OnConditionStack += StackIntensity;
            template.OnConditionStack += StackRefreshDuration;
            CreateConsoleProgress_RemainingTime(template, true);
            return template;
        }
        #region event builders
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
        private static void CreateConsoleProgress_RemainingTime(UnitConditionModel condition, bool showStacks)
        {
            condition.OnTimeElapsed += (sender, deltaTime) =>
            {
                if (condition.ConsoleLineController != null && condition.Duration > 0)
                {
                    string stacks = "";
                    if (showStacks)
                    {
                        stacks = string.Format(" x{0:d}", condition.Intensity);
                    }
                    condition.ConsoleLineController.text = String.Format("{0}{1}: {2:f1}s", 
                        condition.ConsoleLine, stacks, condition.Duration - condition.DurationElapsed);
                }
            };
        }
        #endregion

    }
}
