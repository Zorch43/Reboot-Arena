using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
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
            var mapUnits = Map.Units;//new List<UnitController>();

            SetStrategicStance(mapUnits);//pick stance
            //var teamUnits = new List<UnitController>();
            for (int i = 0; i < mapUnits.Count; i++)
            {
                var u = mapUnits[i] as UnitController;
                if (u != null && u.Data.Team == Team.Team)
                {
                    DoTacticalActionExplicit(u, mapUnits.ToArray(), strategicStance);
                }
            }
        }
    }
    #endregion
    #region public methods
    public void DoTacticalActionExplicit(UnitController selectedUnit, DroneController[] allDrones, string stance)
    {
        //hard-coded behavior tree, doesn't rely on scoring so heavily

        //set unit classes
        ReassignClasses();

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
            DroneController attackTarget = null;
            float attackScore = ScoreAttack(selectedUnit, allDrones, out attackTarget);
            DroneController reloadTarget = null;
            float restoreAmmoScore = ScoreRestoreAmmo(selectedUnit, allDrones, out reloadTarget);
            Vector3 grenadeTarget = new Vector3();
            float grenadeScore = ScoreFragGrenade(selectedUnit, allDrones, out grenadeTarget);
            Vector3 buildTarget = new Vector3();
            float buildScore = ScoreBuildTurret(selectedUnit, allDrones, out buildTarget);

            //3.1: if facing a group of enemies, fire grenade if able
            if (grenadeScore > 0 && grenadeScore > attackScore && roll < Config.Difficulty + Config.SpecialMod)
            {
                selectedUnit.DoSpecialAbility(grenadeTarget);
            }
            //3.2: restore ammo to allies
            else if (restoreAmmoScore > 0 && roll < Config.Difficulty + Config.SupportMod)
            {
                selectedUnit.DoAttack(reloadTarget);
            }
            //3.3: build turret
            else if (buildScore > 0 && selectedUnit.Data.MP > 300 && roll < Config.Difficulty + Config.SpecialMod)
            {
                selectedUnit.DoSpecialAbility(buildTarget);
            }
            //3.4: Throw nanopack
            else if (ScoreThrowNanoPack(selectedUnit, allDrones) > 0 && roll < Config.Difficulty + Config.SpecialMod)
            {
                selectedUnit.DoSpecialAbility(new Vector3());
            }
            //4: shoot the closest enemy with the lowest health in range, in true line-of-sight
            else if (attackScore > 0 && roll < Config.Difficulty + Config.AttackMod)
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
    private float ScoreAttack(UnitController unit, DroneController[] allDrones, out DroneController targetDrone)
    {
        targetDrone = null;
        float score = 0;
        float internalScore = 0;
        var activeWeapon = unit.Data.UnitClass.PrimaryWeapon;
        //get weapon used for attacking (even support units should have one)
        if (activeWeapon.TargetsAllies())
        {
            activeWeapon = unit.Data.UnitClass.SecondaryWeapon;
        }
        float engageDistance = activeWeapon.MaxRange;
        
        foreach (var u in allDrones)
        {
            if(u.Data.Team != Team.Team && u.Data.IsDamageable && u.Data.IsTargetable)
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
                float damageReduction = Mathf.Max(1, 1/(Mathf.Tan(activeWeapon.InAccuracy) * distanceToTarget * 2));
                //targetScore += (engageDistance - Vector3.Distance(u.transform.position, unit.transform.position))/engageDistance;
                ////prefer units close to the objective
                //targetScore += (CAPTURE_DISTANCE - Vector3.Distance(u.transform.position, GameObjective.GetAIObjective())) / CAPTURE_DISTANCE;
                //prefer units with low health
                targetInternalScore -= u.Data.HP / 100;
                //prefer units near death
                float unitsKilled = 0;
                if (activeWeapon.HealthDamage/activeWeapon.Cooldown >= u.Data.HP)
                {
                    unitsKilled += 1;
                }

                targetScore += unitsKilled + (activeWeapon.HealthDamage / activeWeapon.Cooldown) / 100 * damageReduction;
                //targetScore -= primaryWeapon.AmmoCost / primaryWeapon.Cooldown;

                if (targetDrone == null || targetScore + targetInternalScore > score + internalScore)
                {
                    targetDrone = u;
                    score = targetScore;
                    internalScore = targetInternalScore;
                }
            }
        }
        //make final weight adjustments
        return score;
    }
    private float ScoreRestoreAmmo(UnitController unit, DroneController[] allDrones, out DroneController targetDrone)
    {
        targetDrone = null;
        float score = 0;
        float internalScore = 0;
        WeaponModel activeWeapon = null;
        var primaryWeapon = unit.Data.UnitClass.PrimaryWeapon;
        var secondaryWeapon = unit.Data.UnitClass.SecondaryWeapon;
        //get weapon used for restoring ammo (if it exists)
        if (primaryWeapon.AmmoDamage < 0)
        {
            activeWeapon = primaryWeapon;
        }
        else if(secondaryWeapon != null && secondaryWeapon.AmmoDamage < 0)
        {
            activeWeapon = secondaryWeapon;
        }

        if(activeWeapon != null)
        {
            float engageDistance = activeWeapon.MaxRange;// activeWeapon.MaxRange;//TODO: add engage distance to weapons/unit

            foreach (var u in allDrones)
            {
                if (u.Data.Team == Team.Team)
                {
                    float distanceToTarget = Vector3.Distance(u.transform.position, unit.transform.position);
                    float targetScore = 0;
                    float targetInternalScore = 0;
                    //score target
                    //prefer targets with missing ammo
                    //hard disqualifiers
                    if ((u.CompareTag("Unit") && distanceToTarget > engageDistance) || u.Data.MP/u.Data.UnitClass.MaxMP > 0.5f)
                    {
                        continue;
                    }
                    //prefer close units (account for inaccuracy, spread, falloff, slow projectiles, etc)
                    //float damageReduction = Mathf.Max(1, 1 / (Mathf.Tan(activeWeapon.InAccuracy) * distanceToTarget * 2));

                    //prefer units with lots of remaining capacity or low ammo
                    targetScore += (u.Data.UnitClass.MaxMP - u.Data.MP) / 100;
                    

                    targetScore += (-activeWeapon.AmmoDamage / activeWeapon.Cooldown) / 100;
                    if (u.CompareTag("Drone"))
                    {
                        //prefer drones with missing health
                        targetScore += (u.Data.UnitClass.MaxHP - u.Data.HP) / 100;
                        targetScore -= distanceToTarget;
                    }
                    else
                    {
                        //prefer units with high health
                        targetScore += u.Data.HP / 100;
                    }


                    if (targetDrone == null || targetScore + targetInternalScore > score + internalScore)
                    {
                        targetDrone = u;
                        score = targetScore;
                        internalScore = targetInternalScore;
                    }
                }
            }
        }
        else
        {
            score = -1000;
        }
        
        //make final weight adjustments
        return score;
    }
    private float ScoreFragGrenade(UnitController unit, DroneController[] allDrones, out Vector3 targetLocation)
    {
        bool hasTarget = false;
        targetLocation = new Vector3();
        float score = 0;
        float internalScore = 0;
        
        if (unit.Data.UnitClass.SpecialAbility.Name == "Grenade" && unit.Data.MP >= unit.Data.UnitClass.SpecialAbility.AmmoCostInstant)
        {
            var abilityWeapon = unit.Data.UnitClass.SpecialAbility.AbilityWeapon;

            float engageDistance = Mathf.Min(Config.Speed, 2) * unit.Data.UnitClass.MoveSpeed + abilityWeapon.MaxRange;
            foreach (var u in allDrones)
            {
                if (u.Data.Team != Team.Team && u.Data.IsDamageable)
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
                    foreach (var o in allDrones)
                    {
                        if (o.Data.Team != Team.Team && o != u && o.Data.IsDamageable
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
                    if (!u.IsMoving)
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
                        targetLocation = u.TargetingPosition;
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
    private float ScoreBuildTurret(UnitController unit, DroneController[] allUnits, out Vector3 buildSite)
    {
        float score = -1;
        float internalScore = 0;
        buildSite = new Vector3();
        if(unit.Data.UnitClass.SpecialAbility.Name == "Turret" && unit.Data.MP > unit.Data.UnitClass.SpecialAbility.AmmoCostInstant)
        {
            //search area near unit for valid build sites

            for(float searchRadius = 1; searchRadius <= 10; searchRadius += 1)
            {
                for(float searchAngle = 0; searchAngle < 360; searchAngle += 60 / searchRadius)
                {
                    //score each valid build site
                    float currentScore = 0;
                    float currentInternalScore = 0;
                    //TODO: raycast down to get height
                    Vector3 currentSite = unit.transform.position + new Vector3(Mathf.Cos(searchAngle), 0, Mathf.Sin(searchAngle)) * searchRadius;
                    currentSite.y = 0;
                    if(BuildTools.IsValidBuildSite(currentSite, 0.5f))
                    {
                        //hard preferences
                        //must be within engagement range and within line-of-sight of an objective
                        var distanceToObjective = Vector3.Distance(GameObjective.GetAIObjective(), unit.transform.position);
                        if(distanceToObjective > 10)
                        {
                            continue;
                        }
                        currentScore -= 1;
                        //prefer sites closer to current position
                        //currentScore -= 0.1f * searchRadius;
                        //prefer sites close to objectives
                        
                        currentScore += (10 - distanceToObjective) * 0.2f;
                        //prefer sites without too many enemies in the immediate vicinity
                        float enemyCount = 0;
                        float allyCount = 0;
                        foreach(var u in allUnits)
                        {
                            var distance = Vector3.Distance(u.transform.position, currentSite);
                            if(distance <= 10)
                            {
                                float unitScore = 0.5f;
                                if (u is UnitController)
                                {
                                    unitScore = 1;
                                }
                                if (u.Data.Team == Team.Team)
                                {
                                    allyCount += unitScore;
                                }
                                else
                                {
                                    enemyCount += unitScore;
                                }
                            }
                            
                        }
                        //enemy numbers can be tolerated if near allies
                        currentScore += Mathf.Min(allyCount - enemyCount, 0);
                        //enemy numbers can be tolerated if unit has high health and ammo
                        //currentScore += (300 - unit.Data.MP)/100;
                        //return best build site

                        if (currentScore + currentInternalScore > score + internalScore)
                        {
                            score = currentScore;
                            internalScore = currentInternalScore;
                            buildSite = currentSite;
                        }
                    }
                }
            } 
        }
        else
        {
            score = -1000;
        }
        return score;
    }
    private float ScoreThrowNanoPack(UnitController unit, DroneController[] allDrones)
    {
        float score = 0;
        var specialAbility = unit.Data.UnitClass.SpecialAbility;
        if (specialAbility.Name == "NanoPack" && unit.Data.MP > specialAbility.AmmoCostInstant)
        {
            foreach (var d in allDrones)
            {
                var u = d as UnitController;
                if (u != null)
                {
                    var totalHealth = u.Data.HP;
                    var missingHealth = u.Data.UnitClass.MaxHP - totalHealth;
                    if (missingHealth > 100)
                    {
                        var distance = Vector3.Distance(unit.transform.position, u.transform.position);
                        if (distance < 5)
                        {

                            if (u.Data.Team == unit.Data.Team)
                            {
                                //prioritize throwing while there are allied units that need healing nearby
                                score += 1;
                                //Prioritize when nearby allied units are at critical health
                                if (totalHealth < 100)
                                {
                                    score += 1;
                                }
                                //prioritize when this unit needs health
                                if (u == unit)
                                {
                                    score += 1;
                                }
                            }
                            else
                            {
                                //de-prioritize when there are enemy units with missing health nearby
                                score -= 1;
                            }


                        }
                    }

                }
            }

            ////set score target
            //float scoreTarget = 2;
            ////score target increases as avaialble ammo decreases
            //scoreTarget += (unit.Data.UnitClass.MaxMP - unit.Data.MP) / specialAbility.AmmoCostInstant;
            //score -= scoreTarget;
        }
        else
        {
            score = -1000;
        }
        //return score
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
    private void ReassignClasses()
    {
        var slots = Team.UnitSlots;
        //on the offense, aim to have at least 12 (44%) offensive weight among slots
        //on the defense, aim to have at least 12 (44%) defensive weight among slots
        //always aim to have between 6 and 12 (22% - 44%) support weight

        //get total class-role weighting for living units
        var respawningSlots = new List<UnitSlotModel>();
        var activeSlots = new List<UnitSlotModel>();
        foreach(var s in slots)
        {
            if(s.CurrentUnit == null)
            {
                //sort by respawn progress here
                for(int i = 0; i <= respawningSlots.Count; i++)
                {
                    if(i == respawningSlots.Count)
                    {
                        respawningSlots.Add(s);
                        break;
                    }
                    var slot = respawningSlots[i];
                    if(s.RespawnProgress >= slot.RespawnProgress)
                    {
                        respawningSlots.Insert(i, s);
                        break;
                    }
                }
            }
            else
            {
                activeSlots.Add(s);
            }
        }
        float activeOffense, activeDefense, activeSupport;
        var activeWeight = GetRoleWeighting(slots, true, out activeOffense, out activeDefense, out activeSupport);
        //prioritize changing unit classes to ensure the desired ratio most of the time
        float minOffenseWeight, minDefenseWeight, minSupportWeight, maxSupportWeight;
        minSupportWeight = 6/27f;
        maxSupportWeight = 12 / 27f;
        if(strategicStance == "Rush")
        {
            minOffenseWeight = 12 / 27f;
            minDefenseWeight = 0;
        }
        else
        {
            minOffenseWeight = 0;
            minDefenseWeight = 12 / 27f;
        }
        //look at respawning slots first, closer respawns first
        int activeCount = activeSlots.Count;
        foreach(var c in respawningSlots)
        {
            float a = activeOffense / activeCount;
            float d = activeDefense / activeCount;
            float s = activeSupport / activeCount;
            activeCount++;
            UnitClassModel bestClass = null;
            float bestScore = 0;
            //assign best unit to each respawning slot
            foreach(var u in Team.UnitClasses)
            {
                float testScore = 0;
                //score each available class
                float testA = (activeOffense + u.AttackerWeight) / activeCount;
                float testD = (activeDefense + u.DefenderWeight) / activeCount;
                float testS = (activeSupport + u.SupportWeight) / activeCount;
                //prefer classes that bring class role weights into desired balance
                //if min role balance not achieved, score weight increases
                if(a < minOffenseWeight)
                {
                    testScore += testA - a;
                }
                if(d < minDefenseWeight)
                {
                    testScore += testD - d;
                }
                if(s < minSupportWeight)
                {
                    testScore += testS - s;
                }
                //if max role balance achieved, score weight decreases
                else if (s > maxSupportWeight)
                {
                    testScore -= testS - s;
                }

                //evaluate score
                if(testScore > bestScore || bestClass == null)
                {
                    bestScore = testScore;
                    bestClass = u;
                }
                
            }
            //add chosen class role weights to active role weights
            activeOffense += bestClass.AttackerWeight;
            activeDefense += bestClass.DefenderWeight;
            activeSupport += bestClass.SupportWeight;
            //assign class to slot
            c.NextUnitClass = bestClass;
        }


        //TODO: if final balance still skewed, assign respawn classes to living units
    }
    //returns total weight
    private float GetRoleWeighting(List<UnitSlotModel> slots, bool current, out float offense, out float defense, out float support)
    {
        float a = 0;
        float d = 0;
        float s = 0;
        foreach(var c in slots)
        {
            UnitClassModel unitClass = null;
            if (current)
            {
                unitClass = c.CurrentUnitClass;
            }
            else
            {
                unitClass = c.NextUnitClass;
            }
            a += unitClass.AttackerWeight;
            d += unitClass.DefenderWeight;
            s += unitClass.SupportWeight;
        }
        offense = a;
        defense = d;
        support = s;
        return a + d + s;
    }
    private void SetStrategicStance(List<DroneController> allUnits)
    {
        var objectivePoint = GameObjective.GetAIObjective();
        var teamSpawnPoint = GameObjective.GetAISpawnPoint(Team.Team);

        float teamPointUnits = 0;
        float teamReserveUnits = 0;
        float enemyPointUnits = 0;
        float enemyReserveUnits = 0;

        foreach (var u in allUnits)
        {
            float unitWeight = 0.5f;
            if(u is UnitController)
            {
                unitWeight = 1;
            }
            var distanceToPoint = Vector3.Distance(objectivePoint, u.transform.position);
            var moveDistance = Mathf.Min(distanceToPoint - CAPTURE_DISTANCE, 0);
            var travelTime = moveDistance / u.Data.UnitClass.MoveSpeed;
            if (u.Data.Team == Team.Team)
            {
                if (travelTime < 1)
                {
                    teamPointUnits += unitWeight;
                }
                else
                {
                    teamReserveUnits += unitWeight;
                }
            }
            else
            {
                if (travelTime < 1)
                {
                    enemyPointUnits += unitWeight;
                }
                else
                {
                    enemyReserveUnits += unitWeight;
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
