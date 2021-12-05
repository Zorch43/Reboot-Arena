using System;
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
        public bool IsTargetable { get; set; } = true;//whether this unit can be a valid target
        public bool IsDamageable { get; set; } = true;//whetehr this unit can be damaged
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
