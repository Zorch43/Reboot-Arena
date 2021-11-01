using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    #region constants
    const float STRATEGY_ACTION_SPEED = 3;//delay between strategic actions
    const float STRATEGY_ACTION_PROBABILITY = 1;
    const float TACTICAL_ACTION_SPEED = 1;//delay between tactical actions
    const float TACTICAL_ACTION_PROBABILITY = 1f;
    const float MASS_TACTICAL_ACTION_PROBABILITY = TACTICAL_ACTION_PROBABILITY * 1f;
    const float ENGAGE_DISTANCE = 15;
    const float CAPTURE_DISTANCE = 6;
    const float WEAK_ENEMY_HP = 200;//focus fire on enemies with health below 200
    const float RETREAT_HP = 150;//start retreating to base if health falls below 150
    const float SEEK_HEALING_HP = .25f;//seek healing at half health
    const float SEEK_RELOAD_MP = .1f;//seek reload at 1/4 ammo
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
    private string strategicStance = "";
    #endregion
    #region properties
    public float Difficulty = 0.1f;
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {

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
        //strategyTime += deltaTime;
        tacticTime += deltaTime;

        //if time for a strategic action
        //if (strategyTime >= STRATEGY_ACTION_SPEED)
        //{
        //    strategyTime = 0;
        //    //if probability check passes
        //    if (Random.Range(0, 1f) < STRATEGY_ACTION_PROBABILITY)
        //    {
        //        //get all units
        //        var allUnits = Map.GetComponentsInChildren<UnitController>();

        //        ////score possible strategic actions:

        //        ////rush objective
        //        //float rushScore = ScoreRushPoint(allUnits);
        //        ////retreat
        //        //float retreatScore = ScoreRetreatFromPoint(allUnits);
        //        ////mass tactical
        //        //float massTacticalScore = ScoreMassTactical(allUnits);

        //        ////do best scoring action
        //        //if (rushScore >= retreatScore && rushScore >= massTacticalScore)
        //        //{
        //        //    //rush the point
        //        //    DoRush(allUnits);
        //        //}
        //        //else if(retreatScore >= massTacticalScore && retreatScore >= rushScore)
        //        //{
        //        //    //retreat from point
        //        //    DoRetreat(allUnits);
        //        //}
        //        //else
        //        //{
        //        //    DoMassTactical(allUnits);
        //        //}
        //        DoStrategicMovement(allUnits);
        //    }
        //}

        //if time for a tactical option
        if (tacticTime >= TACTICAL_ACTION_SPEED)
        {
            tacticTime = 0;
            //if probability check passes
            if (Random.Range(0, 1f) < TACTICAL_ACTION_PROBABILITY)
            {
                //pick a unit to perform a tactical action with
                //get all units
                var slotUnits = new List<UnitController>();
                foreach(var t in GameObjective.Teams)
                {
                    foreach(var s in t.UnitSlots)
                    {
                        if(s.CurrentUnit != null)
                        {
                            slotUnits.Add(s.CurrentUnit);
                        }
                    }
                }
                DoStrategicMovement(slotUnits.ToArray(), false);//pick stance

                //var teamUnits = new List<UnitController>();
                foreach (var u in slotUnits)
                {
                    if (u.Data.Team == Team.Team)
                    {
                        //teamUnits.Add(u);
                        //DoTacticalAction(u, allUnits);
                        DoTacticalActionExplicit(u, slotUnits.ToArray(), strategicStance);
                    }
                }
                //if (teamUnits.Count > 0)
                //{
                //    var selectedUnit = teamUnits[Random.Range(0, teamUnits.Count - 1)];
                //    DoTacticalAction(selectedUnit, allUnits);
                //}
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
        
        if(strategicStance != "Retreat")
        {
            //regular attack
            UnitController attackTarget = null;
            float attackScore = ScoreAttack(selectedUnit, allUnits, out attackTarget);
            var attackAction = new ScoredAction()
            {
                Score = attackScore,
                DoAction = () =>
                {
                    //Debug.Log("Tactical order: Attack");
                    selectedUnit.DoAttack(attackTarget);
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
                    //Debug.Log("Tactical Order: Use Ability");
                    selectedUnit.DoSpecialAbility(abilityTarget);
                }
            };
            actionList.Add(abilityAction);
        }

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
                //Debug.Log("Tactical Order: Get Health");
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
                //Debug.Log("Tactical Order: Get Ammo");
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
        if(bestAction.Score >= 0.1f)
        {
            bestAction.DoAction.Invoke();
        }
        else
        {
            //Debug.Log("No Tactical Options...");
        }
    }
    public void DoTacticalActionExplicit(UnitController selectedUnit, UnitController[] allUnits, string stance)
    {
        //hard-coded behavior tree, doesn't rely on scoring so heavily
        //only valid for Trooper, maybe have individual ai controllers for each class in the future?

        var pickupDispensers = Map.GetComponentsInChildren<PickupSpawnerController>();
        var healthPacks = Map.GetComponentsInChildren<HealthPackController>();
        var ammoPacks = Map.GetComponentsInChildren<AmmoPackController>();
        var roll = Random.Range(0, 1f);
        //0: if retreating, retreat to base
        if(stance == "Retreat")
        {
            DoRetreat(selectedUnit);
        }
        //1: if at low health, and near a health pack, go get it
        //1.1: if not near a health pack, retreat to base
        else if (selectedUnit.Data.HP < RETREAT_HP)
        {
            if(roll < Difficulty)
            {
                //get health
                Vector3 healthLocation = new Vector3();
                float healthScore = ScoreGetHealth(selectedUnit, pickupDispensers, healthPacks, out healthLocation);
                selectedUnit.DoMove(healthLocation, true);
            }
        }
        //2: if out of ammo, go get ammo, no matter how far away
        else if (selectedUnit.Data.MP < 50f)
        {
            if(roll < Difficulty)
            {
                //get ammo
                Vector3 ammoLocation = new Vector3();
                float ammoScore = ScoreGetAmmo(selectedUnit, pickupDispensers, ammoPacks, out ammoLocation);
                selectedUnit.DoMove(ammoLocation, true);
            }
        }
        else
        {
            UnitController attackTarget = null;
            float attackScore = ScoreAttack(selectedUnit, allUnits, out attackTarget);
            Vector3 grenadeTarget = new Vector3();
            float grenadeScore = ScoreFragGrenade(selectedUnit, allUnits, out grenadeTarget);

            //3: if facing a group of enemies, fire grenade if able
            if (grenadeScore > 0 && grenadeScore > attackScore)
            {
                if(roll < Difficulty)
                {
                    selectedUnit.DoSpecialAbility(grenadeTarget);
                }
            }
            //4: shoot the closest enemy with the lowest health in range, in true line-of-sight
            else if (attackScore > 0)
            {
                if(roll < Difficulty)
                {
                    selectedUnit.DoAttack(attackTarget);
                }
            }
            //5: if rushing, rush
            else if (stance == "Rush" && GameObjective.Objective.CurrOwner != Team.Team)//if the point is not owned 
            {
                //Debug.Log("Unit " + selectedUnit.SpawnSlot.SlotNumber + " Rushing point");
                DoRush(selectedUnit);
            }
            //6: if any health missing, get health pack
            else if (selectedUnit.Data.UnitClass.MaxHP - selectedUnit.Data.HP >= 50 )
            {
                if(roll < Difficulty)
                {
                    //Debug.Log("Unit " + selectedUnit.SpawnSlot.SlotNumber + " GettingHealth");
                    //get health
                    Vector3 healthLocation = new Vector3();
                    float healthScore = ScoreGetHealth(selectedUnit, pickupDispensers, healthPacks, out healthLocation);
                    selectedUnit.DoMove(healthLocation, true);
                }
            }
            //7: if any ammo missing, get ammo
            else if (selectedUnit.Data.UnitClass.MaxMP - selectedUnit.Data.MP >= 50 )
            {
                if(roll < Difficulty)
                {
                    //Debug.Log("Unit " + selectedUnit.SpawnSlot.SlotNumber + " Getting Ammo");
                    //get ammo
                    Vector3 ammoLocation = new Vector3();
                    float ammoScore = ScoreGetAmmo(selectedUnit, pickupDispensers, ammoPacks, out ammoLocation);
                    selectedUnit.DoMove(ammoLocation, true);
                }
            }
            //TODO: else defend point?
            else/* if(stance == "Defend")*/
            {
                
                var distanceToPoint = Vector3.Distance(selectedUnit.transform.position, GameObjective.transform.position);
                if (distanceToPoint > 3)
                {
                    //Debug.Log(string.Format("Unit {0} Reinforcing (Distance: {1:F2})", selectedUnit.SpawnSlot.SlotNumber, distanceToPoint));
                    var altMagnitude = selectedUnit.transform.position.magnitude;
                    DoRush(selectedUnit);
                }
                else
                {
                    //Debug.Log(string.Format("Unit {0} Defending (Distance: {1:F2}", selectedUnit.SpawnSlot.SlotNumber, distanceToPoint));
                    selectedUnit.StopMoving();
                }

            }
        }
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

        float score = 0;

        foreach(var u in allUnits)
        {
            var distanceToPoint = Vector3.Distance(objectivePoint, u.transform.position);
            var moveDistance = Mathf.Min(distanceToPoint - CAPTURE_DISTANCE, 0);
            var travelTime = moveDistance / u.Data.UnitClass.MoveSpeed;
            if (u.Data.Team == Team.Team)
            {
                if(travelTime < 1)
                {
                    score++;
                }
                else
                {
                    score += 1/travelTime;
                }
            }
            else
            {
                if(travelTime < 1)
                {
                    score--;
                }
                else
                {
                    score -= 1 / travelTime;
                }
            }
        }
        //prioritize getting on the point if the point is not owned
        if(GameObjective.Objective.CurrOwner != Team.Team)
        {
            score += 1;
        }

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
        float score = 1;

        foreach (var u in allUnits)
        {
            var distanceToPoint = Vector3.Distance(objectivePoint, u.transform.position);
            var moveDistance = Mathf.Min(distanceToPoint - CAPTURE_DISTANCE, 0);
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
        score /= 2 * teamPointUnits + 1;

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
            var moveDistance = Mathf.Min(distanceToPoint - CAPTURE_DISTANCE, 0);
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
        float internalScore = 0;
        var primaryWeapon = unit.Data.UnitClass.PrimaryWeapon;
        float engageDistance = primaryWeapon.MaxRange;
        
        foreach (var u in allUnits)
        {
            if(u.Data.Team != Team.Team)
            {
                float distanceToTarget = Vector3.Distance(u.transform.position, unit.transform.position);
                float targetScore = 0;
                float targetInternalScore = 0;
                //score target
                //prefer targets within (true) line-of-sight
                //hard disqualifiers
                if (!unit.HasTrueLineOfSight(u) || distanceToTarget > engageDistance)
                {
                    continue;
                }
                //prefer close units (account for inaccuracy, spread, falloff, slow projectiles, etc)
                float damageReduction = Mathf.Max(1, 1/(Mathf.Tan(primaryWeapon.InAccuracy) * distanceToTarget * 2));
                //targetScore += (engageDistance - Vector3.Distance(u.transform.position, unit.transform.position))/engageDistance;
                ////prefer units close to the objective
                //targetScore += (CAPTURE_DISTANCE - Vector3.Distance(u.transform.position, GameObjective.GetAIObjective())) / CAPTURE_DISTANCE;
                //prefer units with low health
                targetInternalScore -= u.Data.HP / 100;
                //prefer units near death
                float unitsKilled = 0;
                if (primaryWeapon.Damage/primaryWeapon.Cooldown >= u.Data.HP)
                {
                    unitsKilled += 1;
                }

                targetScore += unitsKilled + (primaryWeapon.Damage / primaryWeapon.Cooldown) / 100 * damageReduction;
                //targetScore -= primaryWeapon.AmmoCost / primaryWeapon.Cooldown;

                if (targetUnit == null || targetScore + targetInternalScore > score + internalScore)
                {
                    targetUnit = u;
                    score = targetScore;
                    internalScore = targetInternalScore;
                }
            }
        }
        //make final weight adjustments
        //score /= 4;//number of conditions
        return score;
    }
    private float ScoreFragGrenade(UnitController unit, UnitController[] allUnits, out Vector3 targetLocation)
    {
        bool hasTarget = false;
        targetLocation = new Vector3();
        float score = 0;
        float internalScore = 0;
        var abilityWeapon = unit.Data.UnitClass.SpecialAbility.AbilityWeapon;
        float engageDistance = Mathf.Min(TACTICAL_ACTION_SPEED, 2) * unit.Data.UnitClass.MoveSpeed + abilityWeapon.MaxRange;
        if (unit.Data.MP >= unit.Data.UnitClass.SpecialAbility.AmmoCostInstant)
        {
            foreach (var u in allUnits)
            {
                if (u.Data.Team != Team.Team)
                {
                    
                    float targetScore = 0;
                    float targetInternalScore = 0;
                    float unitsKilled = 0;
                    float targetsHit = 1;
                    float distanceToTarget = Vector3.Distance(u.transform.position, unit.transform.position);
                    //score target
                    //prefer targets within (true) line-of-sight, and in range
                    //hard disqualifiers
                    if (distanceToTarget > engageDistance)
                    {
                        continue;
                    }
                    
                    //prefer targets with other units close to them
                    
                    foreach (var o in allUnits)
                    {
                        if (o.Data.Team != Team.Team && o != u
                            && Vector3.Distance(u.transform.position, o.transform.position)
                            <= unit.Data.UnitClass.SpecialAbility.AbilityWeapon.ExplosionSize)
                        {
                            targetsHit += 1f;
                            if (abilityWeapon.Damage >= o.Data.HP)
                            {
                                unitsKilled += 1f;
                            }
                        }
                    }
                    if(targetsHit < 3)
                    {
                        continue;
                    }
                    //prefer targets already within range
                    if (distanceToTarget < abilityWeapon.MaxRange)
                    {
                        targetInternalScore += 1;
                    }
                    ////prefer close units
                    //targetScore += (engageDistance - Vector3.Distance(u.transform.position, unit.transform.position))/engageDistance;
                    ////prefer units close to the objective
                    //targetScore += (CAPTURE_DISTANCE - Vector3.Distance(u.transform.position, GameObjective.GetAIObjective())) / CAPTURE_DISTANCE;


                    //prefer units with low health
                    targetInternalScore -= u.Data.HP / 100;
                    //prefer units near death
                    
                    if(abilityWeapon.Damage >= u.Data.HP)
                    {
                        unitsKilled += 1;
                    }

                    
                    //prefer stopped units
                    if (u.Agent.hasPath)
                    {
                        targetInternalScore += 1;
                    }
                    targetScore += targetsHit * abilityWeapon.Damage / 100 + unitsKilled;
                    //targetScore -= abilityWeapon.AmmoCost;

                    if (!hasTarget || targetScore + targetInternalScore > score + internalScore)
                    {
                        hasTarget = true;
                        score = targetScore;
                        internalScore = targetInternalScore;
                        targetLocation = u.transform.position;
                    }
                }
            }
            //make final weight adjustments
            //score /= 4;//number of normal conditions
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
                float moveScore = Vector3.Distance(unit.transform.position, d.transform.position) / unit.Data.UnitClass.MoveSpeed;
                //prioritize magnitude - up to max missing health
                targetScore += Mathf.Min(unit.Data.UnitClass.MaxHP - unit.Data.HP, d.PackCount * 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, TACTICAL_ACTION_SPEED), 2);
                
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
            float moveScore = Vector3.Distance(unit.transform.position, p.transform.position) / unit.Data.UnitClass.MoveSpeed;
            //prioritize magnitude - up to max missing health
            targetScore += Mathf.Min(unit.Data.UnitClass.MaxHP - unit.Data.HP, 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, TACTICAL_ACTION_SPEED), 2);

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
                float moveScore = Vector3.Distance(unit.transform.position, d.transform.position) / unit.Data.UnitClass.MoveSpeed;
                //prioritize magnitude - up to max missing health
                targetScore += Mathf.Min(unit.Data.UnitClass.MaxMP - unit.Data.MP, d.PackCount * 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, TACTICAL_ACTION_SPEED), 2);

                if (!hasTarget || targetScore > score)
                {
                    hasTarget = true;
                    score = targetScore;
                    pickupLocation = d.transform.position;
                }

            }
        }
        //packs
        foreach (var p in packs)
        {
            //score health
            float targetScore = 0;
            //prioritize proximity (by movement speed)
            float moveScore = Vector3.Distance(unit.transform.position, p.transform.position) / unit.Data.UnitClass.MoveSpeed;
            //prioritize magnitude - up to max missing health
            targetScore += Mathf.Min(unit.Data.UnitClass.MaxMP - unit.Data.MP, 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, TACTICAL_ACTION_SPEED), 2);

            if (!hasTarget || targetScore > score)
            {
                hasTarget = true;
                score = targetScore;
                pickupLocation = p.transform.position;
            }
        }

        score /= 6;//final weighting
        return score;
    }
    #endregion
    #region acting
    private void DoStrategicMovement(UnitController[] allUnits, bool doMove)
    {
        var objectivePoint = GameObjective.GetAIObjective();
        var teamSpawnPoint = GameObjective.GetAISpawnPoint(Team.Team);

        float teamPointUnits = 0;
        float teamReserveUnits = 0;
        float enemyPointUnits = 0;
        float enemyReserveUnits = 0;

        foreach (var u in allUnits)
        {
            var distanceToPoint = Vector3.Distance(objectivePoint, u.transform.position);
            var moveDistance = Mathf.Min(distanceToPoint - CAPTURE_DISTANCE, 0);
            var travelTime = moveDistance / u.Data.UnitClass.MoveSpeed;
            if (u.Data.Team == Team.Team)
            {
                if (travelTime < 1)
                {
                    teamPointUnits++;
                }
                else
                {
                    teamReserveUnits += 1;
                }
            }
            else
            {
                if (travelTime < 1)
                {
                    enemyPointUnits++;
                }
                else
                {
                    enemyReserveUnits += 1;
                }
            }
        }
        float objectiveScore = 1;
        if (GameObjective.Objective.CurrOwner != Team.Team)
        {
            objectiveScore += 1f;
            if (GameObjective.Objective.CurrOwner == -1)
            {
                objectiveScore += 1f;
            }
        }
        if (teamPointUnits + teamReserveUnits + objectiveScore >= enemyPointUnits && (enemyPointUnits > 0 || objectiveScore > 1.1f))
        {
            //attack
            if(strategicStance != "Rush")
            {
                Debug.Log("Stance: Rush");
            }
            
            strategicStance = "Rush";
            if (doMove)
            {
                DoRush(allUnits);
            }

        }
        else if (teamPointUnits + teamReserveUnits <= enemyPointUnits / 2)
        {
            //retreat
            if (strategicStance != "Retreat")
            {
                Debug.Log("Stance: Retreat");
            }
            
            strategicStance = "Retreat";
            if (doMove)
            {
                DoRetreat(allUnits);
            }

        }
        else if (enemyPointUnits == 0 && enemyReserveUnits > 0)
        {
            if (strategicStance != "Defend")
            {
                Debug.Log("Stance: Defend");
            }

            strategicStance = "Defend";
        }
        else
        {
            if (strategicStance != "Idle")
            {
                Debug.Log("Stance: Idle");
            }
            strategicStance = "Idle";
        }
    }
    private void DoRush(UnitController[] allUnits)
    {
        strategicStance = "Rush";
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
                    //u.DoMove(objectivePoint, true);
                }
            }
        }
    }
    private void DoRush(UnitController single)
    {
        var objectivePoint = GameObjective.GetAIObjective();
        var distanceFromPoint = Vector3.Distance(objectivePoint, single.transform.position);
        var timeToPoint = (distanceFromPoint - 2) / single.Data.UnitClass.MoveSpeed;
        
        if(distanceFromPoint > 2)
        {
            //if unit is near the point, rush it
            if (timeToPoint < 2)
            {
                //single.DoAttackMove(objectivePoint);
                single.DoMove(objectivePoint, true);
            }
            //if they're far away, attack-move towards the point
            else
            {
                //single.DoAttackMove(objectivePoint);
                single.DoMove(objectivePoint, true);
            }
        }
        
    }
    private void DoRetreat(UnitController[] allUnits)
    {
        strategicStance = "Retreat";
        Debug.Log("Retreating...");
        var spawnPoint = GameObjective.GetAISpawnPoint(Team.Team);
        foreach(var u in allUnits)
        {
            if(u.Data.Team == Team.Team)
            {
                u.DoMove(spawnPoint, true);
            }
        }
    }
    private void DoRetreat(UnitController singleUnit)
    {
        var spawnPoint = GameObjective.GetAISpawnPoint(Team.Team);
        singleUnit.DoMove(spawnPoint, true);
    }
    private void DoMassTactical(UnitController[] allUnits)
    {
        strategicStance = "Defend";
        Debug.Log("Mass Tactical!");

        foreach (var u in allUnits)
        {
            if (u.Data.Team == Team.Team && Random.Range(0, 1f) < MASS_TACTICAL_ACTION_PROBABILITY)
            {
                DoTacticalAction(u, allUnits);
            }
        }
    }
    #endregion
    #endregion
}
