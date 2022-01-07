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
        private UnitController _currentUnit;
        private UnitClassModel _nextUnitClass;
        #endregion
        #region properties
        public bool IsDirty { get; set; }//whether the model's data has been changed recently
        public int SlotNumber { get; set; }
        public float RespawnProgress { get; set; }//percentage of respawn timer completed
        public UnitController CurrentUnit 
        {
            get
            {
                return _currentUnit;
            }
            set
            {
                if(_currentUnit != value)
                {
                    IsDirty = true;
                }
                _currentUnit = value;
            }
        }//displays shorthand unit status of current unit
        public UnitClassModel NextUnitClass
        {
            get
            {
                return _nextUnitClass;
            }
            set
            {
                if(_nextUnitClass?.ClassId != value?.ClassId)
                {
                    IsDirty = true;
                }
                _nextUnitClass = value;
            }
        }//unit class that this slot will spawn once current unit dies
        public UnitClassModel CurrentUnitClass
        {
            get
            {
                if(CurrentUnit != null)
                {
                    return CurrentUnit.Data.UnitClass;
                }
                else
                {
                    return NextUnitClass;
                }
            }
        }
        public Vector3? RallyPoint { get; set; }
        public bool FolowRallyPoint { get; set; } = true;
        public bool IsSelected { get; set; }
        #endregion
        #region public methods
        public void DoUnitDeath()
        {
            //deselect unit (the slot can still be selected later)
            IsSelected = false;
            //start respawning
            RespawnProgress = 0;
        }
        public UnitController GetNextUnitTemplate()
        {
            return ResourceList.GetUnitTemplate(NextUnitClass.ClassId);
        }
        public bool ShouldChangeClass()
        {
            return CurrentUnit != null && CurrentUnit.Data.UnitClass.ClassId != NextUnitClass.ClassId;
        }
        public bool CanChangeClass()
        {
            return CurrentUnit == null;//TODO: detect spawn field quickswap effect
        }
        #endregion
    }
}
