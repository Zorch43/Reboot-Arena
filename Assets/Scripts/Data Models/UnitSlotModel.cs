﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data_Models
{
    public class UnitSlotModel
    {
        #region private fields
        private UnitController respawnUnit;
        #endregion
        #region properties
        public int SlotNumber;
        public float RespawnProgress { get; set; }//percentage of respawn timer completed
        public UnitController CurrentUnit { get; set; }//displays shorthand unit status of current unit
        public UnitController NextUnitClass
        {
            get
            {
                if (respawnUnit != null)
                {
                    return respawnUnit;
                }
                else
                {
                    return CurrentUnit;
                }
            }
            set
            {
                respawnUnit = value;
            }
        }//unit class that this slot will spawn once current unit dies
        #endregion
        #region public methods
        public void DoUnitDeath()
        {
            RespawnProgress = 0;
            respawnUnit = NextUnitClass;
            CurrentUnit = null;
        }
        #endregion
    }
}