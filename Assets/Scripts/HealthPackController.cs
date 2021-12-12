using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackController : PickupController
{
    #region constants

    #endregion
    #region public fields
    public float HealPower;
    #endregion
    #region private fields

    #endregion
    #region properties

    #endregion
    #region unity methods
    //private void OnTriggerEnter(Collider other)
    //{
    //    DoTriggerEnter(other);
    //}
    #endregion
    #region public methods
    public override bool CanApplyEffectToUnit(UnitController unit)
    {
        return unit.Data.HP < unit.Data.UnitClass.MaxHP;
    }

    public override void ApplyEffectToUnit(UnitController unit)
    {
        unit.HealUnit(HealPower * unit.Data.UnitClass.HealthPickupEfficiency);
    }
    #endregion
    #region private methods

    #endregion
}
