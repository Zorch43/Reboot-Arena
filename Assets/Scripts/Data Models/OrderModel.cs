using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data_Models
{
    public class OrderModel
    {
        public enum OrderType
        {
            Move,//rmb empty location
            Attack,//rmb enemy unit
            Support,//rmb allied unit
            TargetedAbility,//shft+rmb location or unit
            SelfAbility,//z
            ForceAttack,//ctrl+rmb location or unit
            ForceMove//shft+rmb location(?)
        }

        public OrderType CurrentOrder { get; set; }
        public Vector3 TargetedLocation { get; set; }
        public UnitController TargetedUnit { get; set; }
    }
}
