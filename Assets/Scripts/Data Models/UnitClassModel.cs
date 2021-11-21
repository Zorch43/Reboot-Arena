﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class UnitClassModel
    {
        #region properties
        public string Name { get; set; }
        public string Description { get; set; }
        #region stats
        public int MaxHP { get; set; }//maximum health points
        public int MaxMP { get; set; }//maximum munition points
        public float MoveSpeed { get; set; }//base move speed
        public float TurnSpeed { get; set; }//base turn speed
        #endregion
        //weapons
        public WeaponModel PrimaryWeapon { get; set; }
        public WeaponModel SecondaryWeapon { get; set; }
        //special ability
        public UnitAbilityModel SpecialAbility { get; set; }
        //passive abilities
        public bool IsAmbidextrous { get; set; }//whether the unit can fire primary and secondary weapons simultaneously, conditions permitting
        public float AutoRepairStrength { get; set; }//how much to heal per second, so long as unit can spend ammo
        public float AutoRepairEfficiency { get; set; } = 1;//how much MP each repaired HP costs
        #endregion
    }
}
