using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSlotController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    //TODO: slot UI
    #endregion
    #region private fields
    private UnitController respawnUnit;
    #endregion
    #region properties
    public float RespawnProgress { get; set; }//percentage of respawn timer completed
    public UnitController CurrentUnit { get; set; }//displays shorthand unit status of current unit
    public UnitController NextUnitClass {
        get
        {
            if(respawnUnit != null)
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
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
