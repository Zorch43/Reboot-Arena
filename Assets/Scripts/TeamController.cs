using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    #region constants
    public const float BASE_RESPAWN_TIME = 30;
    #endregion
    #region public fields
    public UnitSlotManager UnitSlotManager;//9-12 unit slots
    public UnitController[] UnitTemplates;//unit templates avaialable to spawn (6-9)
    public SpawnPointController DefaultSpawnPoint;//starting spawnpoint
    public int Team;
    #endregion
    #region private fields

    #endregion
    #region properties
    public SpawnPointController SpawnPoint { get; set; }//current spawnpoint
    public List<UnitSlotModel> UnitSlots { get; set; } = new List<UnitSlotModel>();//unit slots
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        SpawnPoint = DefaultSpawnPoint;
        SpawnPoint.ControllingTeam = Team;
        if(UnitSlotManager != null)
        {
            //fill slots with first unit and spawn all slots
            for (int i = 0; i < UnitSlotManager.UnitSlots.Count; i++)
            {
                var s = UnitSlotManager.UnitSlots[i];
                UnitSlots.Add(s.Data);
                s.Data.SlotNumber = i + 1;
                s.Data.NextUnitClass = UnitTemplates[0];
                s.Data.RespawnProgress = 0.99f - i * 0.25f / BASE_RESPAWN_TIME;
            }
        }
        else
        {
            for(int i = 0; i < 9; i++)
            {
                var slot = new UnitSlotModel();
                UnitSlots.Add(slot);
                slot.SlotNumber = i + 1;
                slot.NextUnitClass = UnitTemplates[0];
                slot.RespawnProgress = 0.99f - i * 0.25f / BASE_RESPAWN_TIME;
            }
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
