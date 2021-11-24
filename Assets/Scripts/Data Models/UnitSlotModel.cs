using Assets.Scripts.Data_Templates;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data_Models
{
    public class UnitSlotModel
    {
        #region private fields
        private UnitClassModel respawnClass;
        #endregion
        #region properties
        public int SlotNumber;
        public float RespawnProgress { get; set; }//percentage of respawn timer completed
        public UnitController CurrentUnit 
        {
            get;
            set;
        }//displays shorthand unit status of current unit
        public UnitClassModel NextUnitClass
        {
            get
            {
                if (respawnClass != null)
                {
                    return respawnClass;
                }
                else if(CurrentUnit != null)
                {
                    return CurrentUnit.Data.UnitClass;
                }
                return null;
            }
            set
            {
                respawnClass = value;
            }
        }//unit class that this slot will spawn once current unit dies
        public Vector3? RallyPoint { get; set; }
        public bool IsSelected { get; set; }
        #endregion
        #region public methods
        public void DoUnitDeath()
        {
            IsSelected = false;
            RespawnProgress = 0;
            if(respawnClass == null && CurrentUnit != null)
            {
                respawnClass = CurrentUnit.Data.UnitClass;
            }
            CurrentUnit = null;
        }
        public UnitController GetNextUnitTemplate()
        {
            return ResourceList.GetUnitTemplate(NextUnitClass.ClassId);
        }
        
        #endregion
    }
}
