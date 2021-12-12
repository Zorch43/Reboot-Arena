using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPackController : PickupController
{
    #region constants

    #endregion
    #region public fields
    public float AmmoAmount;
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
        return unit.Data.MP < unit.Data.UnitClass.MaxMP;
    }

    public override void ApplyEffectToUnit(UnitController unit)
    {
        unit.ReloadUnit(AmmoAmount * unit.Data.UnitClass.AmmoPickupEfficiency);
    }
    #endregion
    #region private methods

    #endregion
}
