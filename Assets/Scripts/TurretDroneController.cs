using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDroneController : DroneController
{
    #region constants

    #endregion
    #region public fields
    public float Stage1BuildPoints;
    public float Stage2BuildPoints;
    public float Stage3BuildPoints;
    public float InitialHP;
    public float InitialMP;
    #endregion
    #region private fields

    #endregion
    #region properties
    public float BuildPoints { get; set; }
    #endregion
    #region unity methods

    #endregion
    #region public methods
    public override float ReloadUnit(float amount)
    {
        var points = base.ReloadUnit(amount);
        BuildPoints += points;
        return points;
    }
    public override void SpawnSetup(Vector3 position, int team, bool hideUI)
    {
        base.SpawnSetup(position, team, hideUI);
        Data.HP = InitialHP;
        Data.MP = InitialMP;
    }
    #endregion
    #region private methods

    #endregion
}
