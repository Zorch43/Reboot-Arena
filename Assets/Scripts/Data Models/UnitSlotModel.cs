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
        private UnitController _unit;
        private UnitController _unitTemplate;
        private bool _isSelected;
        private int _slotNumber;
        private float _respawnProgress;
        #endregion
        #region properties
        public UnitSlotController Controller { get; set; }//UI element this model is connected to
        public UnitController Unit { get; set; }//displays shorthand unit status of current unit
        public UnitController UnitTemplate { get; set; }
        public int SlotNumber
        {
            get
            {
                return _slotNumber;
            }
            set
            {
                _slotNumber = value;
                if(Controller != null)
                {
                    Controller.SlotNumberLabel.text = value.ToString();
                }
            }
        }
        public float RespawnProgress
        {
            get
            {
                return _respawnProgress;
            }
            set
            {
                _respawnProgress = value;
                if(Controller != null)
                {
                    Controller.UpdateRespawnProgress(value);
                }
            }
        }
        
        //public UnitClassModel CurrentUnitClass
        //{
        //    get
        //    {
        //        if(CurrentUnit != null)
        //        {
        //            return CurrentUnit.Data.UnitClass;
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        CurrentUnit = ResourceList.GetUnitTemplate(value.ClassId);
        //    }
        //}
        public Vector3? RallyPoint { get; set; }
        public bool FolowRallyPoint { get; set; } = true;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if(Controller != null)
                {
                    Controller.SelectionIndicator.SetActive(value);
                }
            }
        }
        #endregion
        #region public methods
        public void DoUnitDeath()
        {
            //deselect unit (the slot can still be selected later)
            IsSelected = false;
            //start respawning
            RespawnProgress = 0;
        }
        #endregion
    }
}
