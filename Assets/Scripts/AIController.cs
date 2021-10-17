using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    #region constants
    const float STRATEGY_ACTION_SPEED = 3;
    const float TACTICAL_ACTION_SPEED = 5f;
    const float ENGAGE_DISTANCE = 5;
    #endregion
    #region public fields
    public TeamController Team;
    public GameObjectiveController GameObjective;
    public CommandController CommandInterface;
    public MapController Map;
    #endregion
    #region private fields
    private float strategyTime;
    private float tacticTime;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        CommandInterface.AITeam = Team.Team;
    }

    // Update is called once per frame
    void Update()
    {
        strategyTime += Time.deltaTime;
        if(strategyTime >= STRATEGY_ACTION_SPEED)
        {
            strategyTime = 0;
            foreach(var u in Team.UnitSlots)
            {
                if(u.CurrentUnit != null  && !u.CurrentUnit.Agent.hasPath)
                {
                    CommandInterface.GiveUnitMoveOrder(u.CurrentUnit, GameObjective.GetAIObjective());
                }
            }
        }
        tacticTime += Time.deltaTime;
        if(tacticTime >= TACTICAL_ACTION_SPEED)
        {
            tacticTime = 0;
            //pick a better target if a low-health enemy unit is in range
            foreach (var u in Team.UnitSlots)
            {
                if (u.CurrentUnit != null)
                {
                    EngageWeakTarget(u.CurrentUnit);
                }
            }
        }
    }
    #endregion
    #region public methods
    public UnitController EngageWeakTarget(UnitController unit)
    {
        var allUnits = Map.GetComponentsInChildren<UnitController>();
        float lowestHealth = 1000;
        UnitController targetUnit = null;
        foreach(var u in allUnits)
        {
            if(u.Data.Team != Team.Team && Vector3.Distance(unit.transform.position, u.transform.position) <= ENGAGE_DISTANCE 
                && (targetUnit == null || u.Data.HP < lowestHealth))
            {
                lowestHealth = u.Data.HP;
                targetUnit = u;
            }
        }
        if(targetUnit != null)
        {
            CommandInterface.GiveUnitAttackOrder(unit, targetUnit);
        }
        return targetUnit;
    }
    public UnitController GetRandomUnit()
    {
        var livingUnits = new List<UnitController>();
        foreach(var u in Team.UnitSlotManager.UnitSlots)
        {
            if (u.Data.CurrentUnit != null)
            {
                livingUnits.Add(u.Data.CurrentUnit);
            }
        }
        if(livingUnits.Count > 0)
        {
            var selectedUnit = livingUnits[Random.Range(0, livingUnits.Count - 1)];
            return selectedUnit;
        }
        else
        {
            return null;
        }
    }
    #endregion
    #region private methods

    #endregion
}
