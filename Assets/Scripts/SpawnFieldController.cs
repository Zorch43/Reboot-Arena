using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFieldController : MonoBehaviour
{
    public SpawnPointController SpawnPoint;
    private void OnTriggerStay(Collider other)
    {
        var unit = other.gameObject.GetComponent<UnitController>();
        if (unit?.Data.Team == SpawnPoint.ControllingTeam)
        {
            unit.Data.Restore();
        }
    }
}
