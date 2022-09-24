using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Data_Templates
{
    public static class ConditionTemplates
    {
        public static UnitConditionModel CreateReloadCondition()
        {
            var template = new UnitConditionModel()
            {
                Name = "Reloading",
                Duration = 5,
                ConsoleEffectName = "Reloading",
                VisualEffectName = ResourceList.EFFECT_RELOAD,
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
            UpdateConsoleProgress_RemainingTime(template, false);

            return template;
        }
        public static UnitConditionModel CreateKillStreakPassive()
        {
            var template = new UnitConditionModel()
            {
                Name = "Kill Streak"
            };
            template.OnKillEnemy += (sender, target) =>
            {
                var unit = sender as DroneController;
                unit.ApplyCondition(CreateKillStreakCondition());
            };
            return template;
        }
        public static UnitConditionModel CreateKillStreakCondition()
        {
            var template = new UnitConditionModel()
            {
                Name = "KillStreakEffect",
                Duration = 10,
                ConsoleEffectName = "Kill Streak",
                VisualEffectName = ResourceList.EFFECT_KILLSTREAK,
                //increase weapon damage
                WeaponHealthDamageProp = 0.2f,//+20% weapon damage per stack
            };
            OnStack_StackIntensity(template, 1, 0);
            OnStack_RefreshDuration(template);
            UpdateConsoleProgress_RemainingTime(template, true);
            return template;
        }
        #region event builders
        private static void OnStack_StackIntensity(UnitConditionModel condition, int addStacks, int maxStacks)
        {
            condition.OnConditionStack += (sender, stack) =>
            {
                condition.Stacks += addStacks;
                if (maxStacks > 0 && condition.Stacks > maxStacks)
                {
                    condition.Stacks = maxStacks;
                }
            };
        }
        private static void OnStack_RefreshDuration(UnitConditionModel condition)
        {
            condition.OnConditionStack += (sender, stack) =>
            {
                condition.DurationElapsed = 0;
            };
        }
        private static void OnStack_StackDuration(UnitConditionModel condition)
        {
            condition.OnConditionStack += (sender, stack) =>
            {
                condition.DurationElapsed -= stack.Duration;
            };
        }
        private static void UpdateConsoleProgress_RemainingTime(UnitConditionModel condition, bool showStacks)
        {
            condition.OnTimeElapsed += (sender, deltaTime) =>
            {
                if (condition.ConsoleEffectController != null && condition.Duration > 0)
                {
                    string stacks = "";
                    if (showStacks)
                    {
                        stacks = string.Format(" x{0}", (int)condition.Stacks);
                    }
                    condition.ConsoleEffectController.text = String.Format("{0}{1}: {2:f1}s", 
                        condition.ConsoleEffectName, stacks, condition.Duration - condition.DurationElapsed);
                }
            };
        }
        #endregion

    }
}
