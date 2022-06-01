using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    #region constants
    public const float BASE_RESPAWN_TIME = 30;
    public const float MIN_QUEUE_TIME = 0.1f;//minimum time between spawns
    #endregion
    #region public fields
    public UnitSlotManager UnitSlotManager;//9 unit slots
    public SpawnPointController DefaultSpawnPoint;//starting spawnpoint
    public int Team;
    #endregion
    #region private fields
    private float queueTime;
    #endregion
    #region properties
    public bool HideUnitUI
    {
        get;
        set;
    }
    public SpawnPointController SpawnPoint { get; set; }//current spawnpoint
    public List<UnitSlotModel> UnitSlots { get; set; } = new List<UnitSlotModel>();//unit slots
    public List<UnitClassModel> UnitClasses { get; set; }//available classes
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //get list of unit classes that this team can use
        SpawnPoint = DefaultSpawnPoint;
        SpawnPoint.ControllingTeam = Team;

        
        int classes = UnitClasses.Count;

        if (UnitSlotManager != null)
        {
            //setup class menu
            int slots = UnitSlotManager.UnitSlots.Count;
            //fill slots with first unit and spawn all slots
            for (int i = 0; i < slots; i++)
            {
                var s = UnitSlotManager.UnitSlots[i];
                UnitSlots.Add(s.Data);
                s.Data.SlotNumber = i + 1;
                s.Data.UnitTemplate = ResourceList.GetUnitTemplate(UnitClasses[Mathf.Min(i / (slots / classes), classes - 1)].ClassId);
                s.Data.RespawnProgress = 1f;
            }
        }
        else
        {
            int slots = 9;
            for (int i = 0; i < 9; i++)
            {
                var slot = new UnitSlotModel();
                UnitSlots.Add(slot);
                slot.SlotNumber = i + 1;
                slot.UnitTemplate = ResourceList.GetUnitTemplate(UnitClasses[Mathf.Min(i / (slots / classes), classes - 1)].ClassId);
                slot.RespawnProgress = 1f;
            }
        }

        //SpawnPoint.MassSpawn(UnitSlots, HideUnitUI);
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        queueTime += deltaTime;
        //advance respawn progress on all slots
        foreach(var s in UnitSlots)
        {
            if(s.Unit == null)
            {
                s.RespawnProgress += deltaTime/BASE_RESPAWN_TIME;
                if (s.RespawnProgress >= 1 && queueTime >= MIN_QUEUE_TIME)
                {
                    queueTime = 0;
                    var unit = SpawnPoint.SpawnUnit(s, HideUnitUI);
                }
            }
        }
    }
    #endregion
    #region public methods
    public void SetUnitClasses(UnitClassTemplates.UnitClasses[] classList)
    {
        UnitClasses = new List<UnitClassModel>();
        foreach(var c in classList)
        {
            UnitClasses.Add(UnitClassTemplates.GetClassByName(c));
        }

        
    }
    #endregion
    #region private methods

    #endregion
}
