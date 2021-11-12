using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    #region constants
    
    const float CAPTURE_DISTANCE = 6;
    const float CRITCAL_HP = 100;//seek healing if health falls below threshold
    const float MISSING_HP = 50;//seek healing after battle if at least this much health is missing
    const float CRITICAL_MP = 25;//seek reload if ammo falls below threshold
    const float MISSING_MP = 50;//seek reload after battle if at least this much ammo is missing
    #endregion
    #region public fields
    public TeamController Team;
    public GameObjectiveController GameObjective;
    public CommandController CommandInterface;
    public MapController Map;
    #endregion
    #region private fields
    private float tacticTime;
    private string strategicStance = "";
    #endregion
    #region properties
    public AIConfigModel Config { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        //run action timers
        float deltaTime = Time.deltaTime;
        tacticTime += deltaTime;

        //if time for a tactical option
        if (tacticTime >= Config.Speed)
        {
            tacticTime = 0;
            //get all units
            var slotUnits = new List<UnitController>();

            foreach (var t in GameObjective.Teams)
            {
                foreach (var s in t.UnitSlots)
                {
                    if (s.CurrentUnit != null)
                    {
                        slotUnits.Add(s.CurrentUnit);
                    }
                }
            }
            SetStrategicStance(slotUnits);//pick stance
            //var teamUnits = new List<UnitController>();
            for (int i = 0; i < slotUnits.Count; i++)
            {
                var u = slotUnits[i];
                if (u.Data.Team == Team.Team)
                {
                    DoTacticalActionExplicit(u, slotUnits.ToArray(), strategicStance);
                }
            }
        }
    }
    #endregion
    #region public methods
    public void DoTacticalActionExplicit(UnitController selectedUnit, UnitController[] allUnits, string stance)
    {
        //hard-coded behavior tree, doesn't rely on scoring so heavily
        //only valid for Trooper, maybe have individual ai controllers for each class in the future?

        var pickupDispensers = Map.GetComponentsInChildren<PickupSpawnerController>();
        var healthPacks = Map.GetComponentsInChildren<HealthPackController>();
        var ammoPacks = Map.GetComponentsInChildren<AmmoPackController>();
        var roll = Random.Range(0, 1f);
        //0: if retreating, retreat to base
        if(stance == "Retreat" && roll < Config.Difficulty + Config.RetreatMod)
        {
            DoRetreat(selectedUnit);
        }
        //1: if at low health, and near a health pack, go get it
        //1.1: if not near a health pack, retreat to base
        else if (selectedUnit.Data.HP < CRITCAL_HP && roll < Config.Difficulty + Config.CriticalHealMod)
        {
            //get health
            Vector3 healthLocation = new Vector3();
            float healthScore = ScoreGetHealth(selectedUnit, pickupDispensers, healthPacks, out healthLocation);
            selectedUnit.DoMove(healthLocation, true);
        }
        //2: if out of ammo, go get ammo, no matter how far away
        else if (selectedUnit.Data.MP < CRITICAL_MP && roll < Config.Difficulty + Config.CriticalAmmoMod)
        {
            //get ammo
            Vector3 ammoLocation = new Vector3();
            float ammoScore = ScoreGetAmmo(selectedUnit, pickupDispensers, ammoPacks, out ammoLocation);
            selectedUnit.DoMove(ammoLocation, true);
        }
        else
        {
            UnitController attackTarget = null;
            float attackScore = ScoreAttack(selectedUnit, allUnits, out attackTarget);
            Vector3 grenadeTarget = new Vector3();
            float grenadeScore = ScoreFragGrenade(selectedUnit, allUnits, out grenadeTarget);

            //3: if facing a group of enemies, fire grenade if able
            if (grenadeScore > 0 && grenadeScore > attackScore && roll < Config.Difficulty + Config.SpecialMod)
            {
                selectedUnit.DoSpecialAbility(grenadeTarget);
            }
            //4: shoot the closest enemy with the lowest health in range, in true line-of-sight
            else if (attackScore > 0 && roll < Config.Difficulty)
            {
                selectedUnit.DoAttack(attackTarget);
            }
            //5: if rushing, rush
            else if (stance == "Rush" && GameObjective.Objective.CurrOwner != Team.Team)//if the point is not owned 
            {
                //Debug.Log("Unit " + selectedUnit.SpawnSlot.SlotNumber + " Rushing point");
                DoRush(selectedUnit);
            }
            //6: if any health missing, get health pack
            else if (selectedUnit.Data.UnitClass.MaxHP - selectedUnit.Data.HP >= MISSING_HP && roll < Config.Difficulty + Config.HealMod)
            {
                //Debug.Log("Unit " + selectedUnit.SpawnSlot.SlotNumber + " GettingHealth");
                //get health
                Vector3 healthLocation = new Vector3();
                float healthScore = ScoreGetHealth(selectedUnit, pickupDispensers, healthPacks, out healthLocation);
                selectedUnit.DoMove(healthLocation, true);
            }
            //7: if any ammo missing, get ammo
            else if (selectedUnit.Data.UnitClass.MaxMP - selectedUnit.Data.MP >= MISSING_MP && roll < Config.Difficulty + Config.AmmoMod )
            {
                //Debug.Log("Unit " + selectedUnit.SpawnSlot.SlotNumber + " Getting Ammo");
                //get ammo
                Vector3 ammoLocation = new Vector3();
                float ammoScore = ScoreGetAmmo(selectedUnit, pickupDispensers, ammoPacks, out ammoLocation);
                selectedUnit.DoMove(ammoLocation, true);
            }
            //TODO: else defend point?
            else
            {
                var distanceToPoint = Vector3.Distance(selectedUnit.transform.position, GameObjective.transform.position);
                if (distanceToPoint > 3)
                {
                    //Debug.Log(string.Format("Unit {0} Reinforcing (Distance: {1:F2})", selectedUnit.SpawnSlot.SlotNumber, distanceToPoint));
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
    #endregion
    #region private methods
    #region scoring
    //action scoring methods
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
                if (primaryWeapon.HealthDamage/primaryWeapon.Cooldown >= u.Data.HP)
                {
                    unitsKilled += 1;
                }

                targetScore += unitsKilled + (primaryWeapon.HealthDamage / primaryWeapon.Cooldown) / 100 * damageReduction;
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
        return score;
    }
    private float ScoreFragGrenade(UnitController unit, UnitController[] allUnits, out Vector3 targetLocation)
    {
        bool hasTarget = false;
        targetLocation = new Vector3();
        float score = 0;
        float internalScore = 0;
        var abilityWeapon = unit.Data.UnitClass.SpecialAbility.AbilityWeapon;
        float engageDistance = Mathf.Min(Config.Speed, 2) * unit.Data.UnitClass.MoveSpeed + abilityWeapon.MaxRange;
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
                            if (abilityWeapon.HealthDamage >= o.Data.HP)
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

                    //prefer units with low health
                    targetInternalScore -= u.Data.HP / 100;
                    //prefer units near death
                    
                    if(abilityWeapon.HealthDamage >= u.Data.HP)
                    {
                        unitsKilled += 1;
                    }

                    //prefer stopped units
                    if (u.Agent.hasPath)
                    {
                        targetInternalScore += 1;
                    }
                    targetScore += targetsHit * abilityWeapon.HealthDamage / 100 + unitsKilled;
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
                targetScore += Mathf.Min(unit.Data.UnitClass.MaxHP - unit.Data.HP, d.PackCount * 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, Config.Speed), 2);
                
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
            targetScore += Mathf.Min(unit.Data.UnitClass.MaxHP - unit.Data.HP, 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, Config.Speed), 2);

            if (!hasTarget || targetScore > score)
            {
                hasTarget = true;
                score = targetScore;
                pickupLocation = p.transform.position;
            }
        }
        //score retreat
        //prioritize proximity (by movement speed)
        var spawnPoint = GameObjective.GetAISpawnPoint(Team.Team);
        float retreatTime = Vector3.Distance(unit.transform.position, spawnPoint) / unit.Data.UnitClass.MoveSpeed;
        //prioritize magnitude - up to max missing health
        float retreatScore = (unit.Data.UnitClass.MaxHP - unit.Data.HP) / 100 / Mathf.Pow(Mathf.Max(retreatTime, Config.Speed), 2);
        if(!hasTarget || retreatScore > score)
        {
            hasTarget = true;
            score = retreatScore;
            pickupLocation = spawnPoint;
        }
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
                targetScore += Mathf.Min(unit.Data.UnitClass.MaxMP - unit.Data.MP, d.PackCount * 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, Config.Speed), 2);

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
            targetScore += Mathf.Min(unit.Data.UnitClass.MaxMP - unit.Data.MP, 100) / 100 / Mathf.Pow(Mathf.Max(moveScore, Config.Speed), 2);

            if (!hasTarget || targetScore > score)
            {
                hasTarget = true;
                score = targetScore;
                pickupLocation = p.transform.position;
            }
        }
        //score retreat
        //prioritize proximity (by movement speed)
        var spawnPoint = GameObjective.GetAISpawnPoint(Team.Team);
        float retreatTime = Vector3.Distance(unit.transform.position, spawnPoint) / unit.Data.UnitClass.MoveSpeed;
        //prioritize magnitude - up to max missing health
        float retreatScore = (unit.Data.UnitClass.MaxMP - unit.Data.MP) / 100 / Mathf.Pow(Mathf.Max(retreatTime, Config.Speed), 2);
        if (!hasTarget || retreatScore > score)
        {
            hasTarget = true;
            score = retreatScore;
            pickupLocation = spawnPoint;
        }
        return score;
    }
    #endregion
    #region acting
    private void SetStrategicStance(List<UnitController> allUnits)
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
        if ((teamPointUnits + teamReserveUnits + objectiveScore >= enemyPointUnits || teamPointUnits + teamReserveUnits >= 8) && (enemyPointUnits > 0 || objectiveScore > 1.1f))
        {
            //attack
            if(strategicStance != "Rush")
            {
                Debug.Log("Stance: Rush");
            }
            
            strategicStance = "Rush";

        }
        else if (teamPointUnits + teamReserveUnits <= enemyPointUnits / 2 && teamPointUnits < 4)
        {
            //retreat
            if (strategicStance != "Retreat")
            {
                Debug.Log("Stance: Retreat");
            }
            
            strategicStance = "Retreat";

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
    
    private void DoRush(UnitController unit)
    {
        var objectivePoint = GameObjective.GetAIObjective();
        var distanceFromPoint = Vector3.Distance(objectivePoint, unit.transform.position);
        var timeToPoint = (distanceFromPoint - 2) / unit.Data.UnitClass.MoveSpeed;
        
        if(distanceFromPoint > 2)
        {
            //if unit is near the point, rush it
            if (timeToPoint < 2)
            {
                //single.DoAttackMove(objectivePoint);
                unit.DoMove(objectivePoint, true);
            }
            //if they're far away, attack-move towards the point
            else
            {
                //single.DoAttackMove(objectivePoint);
                unit.DoMove(objectivePoint, true);
            }
        }
        
    }
   
    private void DoRetreat(UnitController unit)
    {
        var spawnPoint = GameObjective.GetAISpawnPoint(Team.Team);
        unit.DoMove(spawnPoint, true);
    }
    #endregion
    #endregion
}
