﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data_Models
{
    public class UnitModel
    {
        #region properties
        public UnitClassModel UnitClass { get; set; }
        public int Team { get; set; }//team ID
        public float HP { get; set; }//current health points
        public float MP { get; set; }//current munition points
        public bool IsMoving { get; set; }
        public bool IsActing { get; set; }
        public float ActionCooldown { get; set; }//time until another action can be performed
        public bool IsSelected { get; set; }
        #endregion
        public UnitModel (UnitClassModel unitClass)
        {
            UnitClass = unitClass;
            Restore();
        }
        public void Restore()
        {
            HP = UnitClass.MaxHP;
            MP = UnitClass.MaxMP;
        }
    }
}
