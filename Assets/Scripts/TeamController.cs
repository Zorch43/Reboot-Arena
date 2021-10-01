using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    #region constants
    public const float BASE_RESPAWN_TIME = 30;
    #endregion
    #region public fields
    public UnitSlotController[] UnitSlots;//9-12 unit slots
    public UnitController[] TeamUnits;//unit templates avaialable to spawn (6-9)
    public SpawnPointController DefaultSpawnPoint;//starting spawnpoint
    public int Team;
    #endregion
    #region private fields

    #endregion
    #region properties
    public SpawnPointController SpawnPoint { get; set; }//current spawnpoint
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        SpawnPoint = DefaultSpawnPoint;
        SpawnPoint.ControllingTeam = Team;
        //fill slots with first unit and spawn all slots
        for(int i = 0; i < UnitSlots.Length; i++)
        {
            var s = UnitSlots[i];
            s.NextUnitClass = TeamUnits[0];
            s.RespawnProgress = 0.99f - i * 0.25f/BASE_RESPAWN_TIME;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //advance respawn progress on all slots
        foreach(var s in UnitSlots)
        {
            if(s.RespawnProgress < 1)
            {
                s.RespawnProgress += Time.deltaTime/BASE_RESPAWN_TIME;
                if (s.RespawnProgress >= 1)
                {
                    SpawnPoint.SpawnUnit(s);
                }
            }
        }
    }
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
