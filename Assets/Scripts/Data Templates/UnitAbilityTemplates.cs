using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Templates
{
    public static class UnitAbilityTemplates
    {
        public static UnitAbilityModel CreateFragGrenade()
        {
            var ability = new UnitAbilityModel()
            {
                Name = "Grenade",
                Description = "Throw an explosive grenade at the target location",
                Icon = ResourceList.ICON_FRAG_GRENADE,
                Cursor = ResourceList.CURSOR_FRAG_GRENADE,
                Marker = ResourceList.MARKER_FRAG_GRENADE,
                IsTargetedAbility = true,
                IsWeaponAbility = true,
                GroupActivationRule = UnitAbilityModel.GroupActivationType.Single,
                ConsiderMostAmmoInGroup = true,
                ConsiderLeastHealthInGroup = true,
                ConsiderLeastMoveDistanceToTarget = true,
                AbilityWeapon = WeaponTemplates.CreateFragGrenade(),
                AmmoCostInstant = 100
            };
            return ability;
        }
    }
}
