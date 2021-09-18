using System;
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
        public string Portrait { get; set; }
        #region stats
        public int MaxHP { get; set; }//maximum health points
        public int MaxMP { get; set; }//maximum munition points
        public float MoveSpeed { get; set; }//base move speed
        public float TurnSpeed { get; set; }//base turn speed
        #endregion
        //TODO: primary weapon
        public WeaponModel PrimaryWeapon { get; set; }
        public WeaponModel SecondaryWeapon { get; set; }
        //TODO: secondary weapon
        //TODO: active abilities
        //TODO: passive abilities
        #endregion
    }
}
