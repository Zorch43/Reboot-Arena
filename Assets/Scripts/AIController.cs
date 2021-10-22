using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    #region constants
    const float STRATEGY_ACTION_SPEED = 3;//delay between strategic actions
    const float STRATEGY_ACTION_PROBABILITY = 1;
    const float TACTICAL_ACTION_SPEED = 1f;//delay between tactical actions
    const float TACTICAL_ACTION_PROBABILITY = 1;
    const float ENGAGE_DISTANCE = 5;
    const float WEAK_ENEMY_HP = 200;//focus fire on enemies with health below 200
    const float RETREAT_HP = 150;//start retreating to base if health falls below 150
    const float SEEK_HEALING_HP = .5f;//seek healing at half health
    const float SEEK_RELOAD_MP = .25f;//seek reload at 1/4 ammo
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
        #region outline
        //AI strategy outline:
        //every strategy action, direct all relevant units to:

        //move at least  x-1 units to the objective, where x is the number of enemy units near the point

        //while there are more than x/2 units contesting/holding the point, send constant reinforcements

        //if contesting the point, and unit count falls below x/2, retreat to base

        //AI tactics outline:
        //every tactical action, pick a unit and perform the best action:

        //attack the closest unit with the lowest health within true line-of-sight (enemy units are considered blocking if weapon doesn't penetrate or arc)
        //if it makes sense to use unit's special ability, do so

        //if health falls below threshold, seek a healthpack

        //if ammo falls below threshold, seek an ammopack
        #endregion
        //run action timers
        float deltaTime = Time.deltaTime;
        strategyTime += deltaTime;
        tacticTime += deltaTime;

        //if time for a strategic action
        if (strategyTime >= STRATEGY_ACTION_SPEED)
        {
            strategyTime = 0;
            //if probability check passes
            if (Random.Range(0, 1f) < STRATEGY_ACTION_PROBABILITY)
            {
                //get all units
                var allUnits = Map.GetComponentsInChildren<UnitController>();

                //score possible strategic actions:

                //rush objective
                float rushScore = ScoreRushPoint(allUnits);
                //retreat
                float retreatScore = ScoreRetreatFromPoint(allUnits);
                //mass tactical
                float massTacticalScore = ScoreMassTactical(allUnits);

                //do best scoring action
                if (rushScore >= retreatScore && rushScore >= massTacticalScore)
                {
                    //rush the point
                    DoRush(allUnits);
                }
                else if(retreatScore >= massTacticalScore && retreatScore >= rushScore)
                {
                    //retreat from point
                    DoRetreat(allUnits);
                }
                else
                {
                    DoMassTactical(allUnits);
                }
            }
        }

        //if time for a tactical option
        if (tacticTime >= TACTICAL_ACTION_SPEED)
        {
            tacticTime = 0;
            //if probability check passes
            if (Random.Range(0, 1f) < TACTICAL_ACTION_PROBABILITY)
            {
                //pick a unit to perform a tactical action with
                //get all units
                var allUnits = Map.GetComponentsInChildren<UnitController>();
                var teamUnits = new List<UnitController>();
                foreach (var u in allUnits)
                {
                    if (u.Data.Team == Team.Team)
                    {
                        teamUnits.Add(u);
                    }
                }
                if (teamUnits.Count > 0)
                {
                    var selectedUnit = teamUnits[Random.Range(0, teamUnits.Count - 1)];
                    DoTacticalAction(selectedUnit, allUnits);
                }
            }
        }



        #region old code
        //strategyTime += Time.deltaTime;
        //if(strategyTime >= STRATEGY_ACTION_SPEED)
        //{
        //    strategyTime = 0;
        //    foreach(var u in Team.UnitSlots)
        //    {
        //        if(u.CurrentUnit != null  && !u.CurrentUnit.Agent.hasPath)
        //        {
        //            CommandInterface.GiveAttackMoveOrder(new List<UnitController>() { u.CurrentUnit }, GameObjective.GetAIObjective());
        //        }
        //    }
        //}
        //tacticTime += Time.deltaTime;
        //if(tacticTime >= TACTICAL_ACTION_SPEED)
        //{
        //    tacticTime = 0;
        //    //pick a better target if a low-health enemy unit is in range
        //    foreach (var u in Team.UnitSlots)
        //    {
        //        if (u.CurrentUnit != null)
        //        {
        //            EngageWeakTarget(u.CurrentUnit);
        //        }
        //    }
        //}
        #endregion
    }
    #endregion
    #region public methods
    public void DoTacticalAction(UnitController selectedUnit, UnitController[] allUnits)
    {
        //score tactical actions with that unit, and get targets for those actions
        List<ScoredAction> actionList = new List<ScoredAction>();
        //regular attack
        UnitController attackTarget = null;
        float attackScore = ScoreAttack(selectedUnit, allUnits, out attackTarget);
        var attackAction = new ScoredAction()
        {
            Score = attackScore,
            DoAction = () =>
            {
                Debug.Log("Tactical order: Attack");
                CommandInterface.GiveUnitAttackOrder(selectedUnit, attackTarget);
            }
        };
        actionList.Add(attackAction);

        //special ability
        Vector3 abilityTarget = new Vector3();
        float abilityScore = ScoreFragGrenade(selectedUnit, allUnits, out abilityTarget);
        var abilityAction = new ScoredAction()
        {
            Score = abilityScore,
            DoAction = () =>
            {
                Debug.Log("Tactical Order: use Ability");
                selectedUnit.DoSpecialAbility(abilityTarget);
            }
        };
        actionList.Add(abilityAction);

        var pickupDispensers = Map.GetComponentsInChildren<PickupSpawnerController>();
        var healthPacks = Map.GetComponentsInChildren<HealthPackController>();
        var ammoPacks = Map.GetComponentsInChildren<AmmoPackController>();
        //get health
        Vector3 healthLocation = new Vector3();
        float healthScore = ScoreGetHealth(selectedUnit, pickupDispensers, healthPacks, out healthLocation);
        var healthAction = new ScoredAction()
        {
            Score = healthScore,
            DoAction = () =>
            {
                Debug.Log("Tactical Order: Get Health");
                selectedUnit.DoMove(healthLocation, true);
            }
        };
        actionList.Add(healthAction);
        //get ammo
        Vector3 ammoLocation = new Vector3();
        float ammoScore = ScoreGetAmmo(selectedUnit, pickupDispensers, ammoPacks, out ammoLocation);
        var ammoAction = new ScoredAction()
        {
            Score = ammoScore,
            DoAction = () =>
            {
                Debug.Log("Tactical Order: Get Ammo");
                selectedUnit.DoMove(ammoLocation, true);
            }
        };
        actionList.Add(ammoAction);
        //pick the best action based on score, then perform it
        ScoredAction bestAction = null;
        foreach (var a in actionList)
        {
            if (bestAction == null || bestAction.Score < a.Score)
            {
                bestAction = a;
            }
        }
        //do best action
        bestAction.DoAction.Invoke();
    }
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
    #region scoring
    //action scoring methods
    //strategic actions
    private float ScoreRushPoint(UnitController[] allUnits)
    {
        //determine the state of the objective
        //determine how many units of this team, and opposing team(s) are near the point
        //find units on this team not near the objective
        //if team units at point + reserve units >= enemy units at point, rush the point
        var objectivePoint = GameObjective.GetAIObjective();

        int teamPointUnits = 0;
        int teamReserveUnits = 0;
        int enemyPointUnits = 0;
        float score = 1;//prioritize attacking

        foreach(var u in allUnits)
        {
            var distanceToPoint = Vector3.Distance(objectivePoint, u.transform.position);
            var moveDistance = Mathf.Min(distanceToPoint - ENGAGE_DISTANCE, 0);
            var travelTime = moveDistance / u.Data.UnitClass.MoveSpeed;
            if (u.Data.Team == Team.Team)
            {
                if(travelTime < 1)
                {
                    teamPointUnits++;
                }
                else
                {
                    teamReserveUnits++;
                }
            }
            else
            {
                if(travelTime < 1)
                {
                    enemyPointUnits++;
                }
            }
        }

        score += teamPointUnits;
        score += teamReserveUnits;
        score -= enemyPointUnits;
        //TODO: prioritize getting on the point if the point is in transisition

        return score;
    }
    private float ScoreRetreatFromPoint(UnitController[] allUnits)
    {
        //determine state of objective
        //determine how many units of this team, and opposing team(s) are near the point
        //if number of team units at the point is <= half of enemy units at point, retreat to spawnpoint

        var objectivePoint = GameObjective.GetAIObjective();

        int teamPointUnits = 0;
        int teamReserveUnits = 0;
        int enemyPointUnits = 0;
        float score = 0;

        foreach (var u in allUnits)
        {
            var distanceToPoint = Vector3.Distance(objectivePoint, u.transform.position);
            var moveDistance = Mathf.Min(distanceToPoint - ENGAGE_DISTANCE, 0);
            var travelTime = moveDistance / u.Data.UnitClass.MoveSpeed;
            if (u.Data.Team == Team.Team)
            {
                if (travelTime < 1)
                {
                    teamPointUnits++;
                }
                else
                {
                    teamReserveUnits++;
                }
            }
            else
            {
                if (travelTime < 1)
                {
                    enemyPointUnits++;
                }
            }
        }


        score += enemyPointUnits;
        score -= teamPointUnits * 2;

        return score;
    }
    private float ScoreMassTactical(UnitController[] allUnits)
    {
        var objectivePoint = GameObjective.GetAIObjective();

        int teamPointUnits = 0;
        int teamReserveUnits = 0;
        int enemyPointUnits = 0;
        float score = 0;

        foreach (var u in allUnits)
        {
            var distanceToPoint = Vector3.Distance(objectivePoint, u.transform.position);
            var moveDistance = Mathf.Min(distanceToPoint - ENGAGE_DISTANCE, 0);
            var travelTime = moveDistance / u.Data.UnitClass.MoveSpeed;
            if (u.Data.Team == Team.Team)
            {
                if (travelTime < 1)
                {
                    teamPointUnits++;
                }
                else
                {
                    teamReserveUnits++;
                }
            }
            else
            {
                if (travelTime < 1)
                {
                    enemyPointUnits++;
                }
            }
        }


        score -= enemyPointUnits;
        score += teamPointUnits * 2;

        return score;
    }
    //tactical actions
    private float ScoreAttack(UnitController unit, UnitController[] allUnits, out UnitController targetUnit)
    {
        targetUnit = null;
        float score = 0;

        foreach(var u in allUnits)
        {
            if(u.Data.Team != Team.Team)
            {
                float targetScore = 0;
                //score target
                //prefer targets within (true) line-of-sight
                if (unit.HasLineOfSight(u.transform.position))
                {
                    targetScore += 1;
                }
                //prefer close units
                targetScore += (ENGAGE_DISTANCE - Vector3.Distance(u.transform.position, unit.transform.position))/ENGAGE_DISTANCE;
                //prefer units with low health
                targetScore += 1 - u.Data.HP / 500;

                if(targetUnit == null || targetScore > score)
                {
                    targetUnit = u;
                    score = targetScore;
                }
            }
        }
        //make final weight adjustments
        score /= 4;//number of conditions
        return score;
    }
    private float ScoreFragGrenade(UnitController unit, UnitController[] allUnits, out Vector3 targetLocation)
    {
        bool hasTarget = false;
        targetLocation = new Vector3();
        float score = 0;
        if(unit.Data.MP >= unit.Data.UnitClass.SpecialAbility.AmmoCostInstant)
        {
            foreach (var u in allUnits)
            {
                if (u.Data.Team != Team.Team)
                {
                    float targetScore = 0;
                    //score target
                    //prefer targets within (true) line-of-sight
                    if (unit.HasLineOfSight(u.transform.position))
                    {
                        targetScore += 1;
                    }
                    //prefer close units
                    targetScore += (ENGAGE_DISTANCE - Vector3.Distance(u.transform.position, unit.transform.position))/ENGAGE_DISTANCE;
                    //prefer units with low health
                    targetScore += 1 - u.Data.HP / 500;
                    //prefer targets with other units close to them
                    foreach(var o in allUnits)
                    {
                        if(o.Data.Team != Team.Team && o != u 
                            && Vector3.Distance(u.transform.position, o.transform.position)
                            <= unit.Data.UnitClass.SpecialAbility.AbilityWeapon.ExplosionSize)
                        {
                            targetScore += 0.5f;
                        }
                    }

                    if (!hasTarget || targetScore > score)
                    {
                        hasTarget = true;
                        score = targetScore;
                        targetLocation = u.transform.position;
                    }
                }
            }
            //make final weight adjustments
            score /= 4;//number of conditions
        }
        else
        {
            score = -1000;
        }
        
        return score;
    }
    private float ScoreGetHealth(UnitController unit, PickupSpawnerController[] dispensers, HealthPackController[] packs, out Vector3 pickupLocation)
    {
        //score all full dispensers and healthpacks to get the best health location
        //score dispensers
        bool hasTarget = false;
        pickupLocation = new Vector3();
        float score = 0;
        //dispensers
        foreach(var d in dispensers)
        {
            //dispenser must be a health spawner
            //dispenser must be full
            if(d.PickupPack.GetComponent<HealthPackController>() != null && d.Pickup.activeSelf)
            {
                //score health
                float targetScore = 0;
                //prioritize proximity (by movement speed)
                targetScore += 3 - Vector3.Distance(unit.transform.position, d.transform.position) / unit.Data.UnitClass.MoveSpeed;
                //prioritize magnitude - up to max missing health
                targetScore += Mathf.Min(unit.Data.UnitClass.MaxHP - unit.Data.HP, d.PackCount * 100)/ 100;
                
                if(!hasTarget || targetScore > score)
                {
                    hasTarget = true;
                    score = targetScore;
                    pickupLocation = d.transform.position;
                }
                
            }
        }
        //packs
        foreach(var p in packs)
        {
            //score health
            float targetScore = 0;
            //prioritize proximity (by movement speed)
            targetScore += 3 - Vector3.Distance(unit.transform.position, p.transform.position) / unit.Data.UnitClass.MoveSpeed;
            //prioritize magnitude - up to max missing health
            targetScore += Mathf.Min(unit.Data.UnitClass.MaxHP - unit.Data.HP, 100) / 100;

            if (!hasTarget || targetScore > score)
            {
                hasTarget = true;
                score = targetScore;
                pickupLocation = p.transform.position;
            }
        }

        score /= 4;//final weighting
        return score;
    }

    private float ScoreGetAmmo(UnitController unit, PickupSpawnerController[] dispensers, AmmoPackController[] packs, out Vector3 pickupLocation)
    {
        //score all full dispensers and healthpacks to get the best health location
        //score dispensers
        bool hasTarget = false;
        pickupLocation = new Vector3();
        float score = 0;
        //dispensers
        foreach (var d in dispensers)
        {
            //dispenser must be a health spawner
            //dispenser must be full
            if (d.PickupPack.GetComponent<AmmoPackController>() != null && d.Pickup.activeSelf)
            {
                //score health
                float targetScore = 0;
                //prioritize proximity (by movement speed)
                targetScore += 3 - Vector3.Distance(unit.transform.position, d.transform.position) / unit.Data.UnitClass.MoveSpeed;
                //prioritize magnitude - up to max missing health
                targetScore += Mathf.Min(unit.Data.UnitClass.MaxMP - unit.Data.MP, d.PackCount * 100) / 100;

                if (!hasTarget || targetScore > score)
                {
                    hasTarget = true;
                    score = targetScore;
                }

            }
        }
        //packs
        foreach (var p in packs)
        {
            //score health
            float targetScore = 0;
            //prioritize proximity (by movement speed)
            targetScore += 3 - Vector3.Distance(unit.transform.position, p.transform.position) / unit.Data.UnitClass.MoveSpeed;
            //prioritize magnitude - up to max missing health
            targetScore += Mathf.Min(unit.Data.UnitClass.MaxMP - unit.Data.MP, 100) / 100;

            if (!hasTarget || targetScore > score)
            {
                hasTarget = true;
                score = targetScore;
            }
        }

        score /= 4;//final weighting
        return score;
    }
    #endregion
    #region acting
    private void DoRush(UnitController[] allUnits)
    {
        Debug.Log("Rushing Point!");
        var objectivePoint = GameObjective.GetAIObjective();
        foreach(var u in allUnits)
        {
            if(u.Data.Team == Team.Team)
            {
                var distanceFromPoint = Vector3.Distance(objectivePoint, u.transform.position);
                var timeToPoint = distanceFromPoint / u.Data.UnitClass.MoveSpeed;
                //if unit is near the point, rush it
                if(timeToPoint < 2)
                {
                    u.DoMove(objectivePoint, true);
                }
                //if they're far away, attack-move towards the point
                else
                {
                    u.DoAttackMove(objectivePoint);
                }
            }
        }
    }
    private void DoRetreat(UnitController[] allUnits)
    {
        Debug.Log("Retreating...");
        var spawnPoint = GameObjective.GetAISpawnPoint();
        foreach(var u in allUnits)
        {
            if(u.Data.Team == Team.Team)
            {
                u.DoMove(spawnPoint, true);
            }
        }
    }
    private void DoMassTactical(UnitController[] allUnits)
    {
        Debug.Log("Mass Tactical!");

        foreach (var u in allUnits)
        {
            if (u.Data.Team == Team.Team)
            {
                DoTacticalAction(u, allUnits);
            }
        }
    }
    #endregion
    #endregion
}
