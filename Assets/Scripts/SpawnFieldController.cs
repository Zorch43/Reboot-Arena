using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFieldController : MonoBehaviour
{
    public SpawnPointController SpawnPoint;
    private void OnTriggerStay(Collider other)
    {
        var unit = other.gameObject.GetComponent<UnitController>();
        
        if (unit != null && unit.Data.Team == SpawnPoint.ControllingTeam)
        {
            //heal unit and restore ammo
            unit.HealUnit(unit.Data.UnitClass.MaxHP);
            unit.ReloadUnit(unit.Data.UnitClass.MaxMP);
        }

    }
}
