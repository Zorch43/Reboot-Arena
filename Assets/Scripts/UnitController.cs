using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    #region constants
    private const float PERSONAL_SPACE = 0.02f;
    private const float ORDER_RADIUS = 1f;
    #endregion
    #region public fields
    public UnitClassTemplates.UnitClasses UnitClass;
    public GameObject UnitAppearance;
    public SpriteRenderer MiniMapIcon;
    public TextMeshPro MinimapNumber;
    public TextMeshPro UnitNumber;
    public GameObject UnitEffects;
    public SpriteRenderer Selector;
    public NavMeshAgent Agent;
    public ResourceBarController HealthBar;
    public ResourceBarController AmmoBar;
    public WeaponController PrimaryWeaponMount;
    public WeaponController SecondaryWeaponMount;
    public WeaponController AbilityWeaponMount;
    public int Team = -1;//TEMP
    public Sprite Portrait;
    public Sprite Symbol;
    public UnitVoiceController UnitVoice;
    public PickupController DeathLoot;
    public ParticleSystem JetStream;
    public ParticleSystem RespawnEffect;
    public VariableEffect HealEffect;
    public VariableEffect ReloadEffect;
    public SpecialEffectController DeathExplosion;
    public ToolTipContentController ToolTip;
    public MeshRecolorModel[] TeamColorParts;
    
    #endregion
    #region private fields
    private Quaternion initialRotation;
    private float hitBoxSize;
    private new SphereCollider collider;
    private float zoneMultiplier = 1;
    #endregion
    #region properties
    public Vector3 SpacingVector { get; set; }
    public UnitModel Data { get; set; }
    public UnitController CommandTarget { get; set; }
    public UnitController AutoTarget1 { get; set; }
    public UnitController AutoTarget2 { get; set; }
    public Vector3? ForceTarget { get; set; }
    public Vector3? AbilityTarget { get; set; }
    public Vector3? AttackMoveDestination { get; set; }
    public UnitSlotModel SpawnSlot { get; set; }
    public List<Action> DeathActions { get; set; } = new List<Action>();
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //TEMP: initialize data model
        if(Data == null)
        {
            //get data class from class name
            Data = new UnitModel(UnitClassTemplates.GetClassByName(UnitClass));
        }
        
        initialRotation = UnitEffects.transform.rotation;

        //set agent speed
        Agent.speed = Data.UnitClass.MoveSpeed;

        //TEMP: set team from public property
        if(Team >= 0)
        {
            Data.Team = Team;
        }
        collider = GetComponent<SphereCollider>();
        hitBoxSize = collider.radius;
        //TEMP: set teamcolor
        Recolor(Data.Team);
        MiniMapIcon.color = TeamTools.GetTeamColor(Data.Team);
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;
        //update selection state
        Selector.gameObject.SetActive(SpawnSlot.IsSelected);
        if (Agent.hasPath)
        {
            collider.radius = hitBoxSize + PERSONAL_SPACE;
        }
        else
        {
            //backingOff = false;
            collider.radius = hitBoxSize;
        }
        DoWeaponCooldown(elapsedTime);
        DoUnitAction();

        UnitEffects.transform.rotation = Camera.main.transform.rotation;//orient unit UI towards camera
        MiniMapIcon.transform.rotation = initialRotation;//reset rotation of minimap icon

        //update resource bars
        HealthBar.UpdateBar(Data.UnitClass.MaxHP, Data.HP);
        AmmoBar.UpdateBar(Data.UnitClass.MaxMP, Data.MP);

        //update tooltip
        UpdateTooltip();

        if(Data.HP <= 0)
        {
            Kill();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        var unit = collision.collider.GetComponent<UnitController>();
        if (Agent.hasPath && unit != null)
        {
            if(Vector3.Distance(transform.position, Agent.destination) <= ORDER_RADIUS
            || (!unit.Agent.hasPath && Vector3.Distance(unit.transform.position, Agent.destination) <= ORDER_RADIUS/2 * zoneMultiplier))
            {
                StopMoving();
                if(AttackMoveDestination != null 
                && (Vector3.Distance(transform.position, AttackMoveDestination ?? new Vector3()) <= ORDER_RADIUS
                || (!unit.Agent.hasPath && Vector3.Distance(unit.transform.position, AttackMoveDestination ?? new Vector3()) <= ORDER_RADIUS / 2 * zoneMultiplier)))
                {
                    AttackMoveDestination = null;//stop attack-move
                }
            }
            else
            {
                zoneMultiplier += 0.02f;
            }
        } 
    }

    #endregion
    #region public methods
    public void Kill()
    {
        foreach(var d in DeathActions)
        {
            d.Invoke();
        }
        SpawnSlot.IsSelected = false;
        Destroy(gameObject);
    }
    public void SpawnSetup(Vector3 position, int team, UnitSlotModel slot, bool hideUI)
    {
        Data = new UnitModel(UnitClassTemplates.GetClassByName(UnitClass));
        Data.Team = team;
        Agent.enabled = false;//disable the agent while manually placing the unit
        transform.position = position;
        SpawnSlot = slot;
        SpawnSlot.CurrentUnit = this;
        MinimapNumber.text = slot.SlotNumber.ToString();
        UnitNumber.text = slot.SlotNumber.ToString();

        if (hideUI)
        {
            MinimapNumber.gameObject.SetActive(false);
            UnitNumber.gameObject.SetActive(false);
            AmmoBar.gameObject.SetActive(false);
        }

        DeathActions.Add(SpawnSlot.DoUnitDeath);
        DeathActions.Add(DoLootDrop);
        DeathActions.Add(DoDeathExplosion);

        //if unit has a rally point, issue a move order to the rally point
        Agent.enabled = true;
        DoMove(slot.RallyPoint ?? transform.position, false);
    }
    public UnitModel GetData()
    {
        if(Data == null)
        {
            return new UnitModel(UnitClassTemplates.GetClassByName(UnitClass));
        }
        else
        {
            return Data;
        }
    }
    public float HealUnit(float amount)
    {
        //heal the unit by amount, up to max hp
        float oldHealth = Data.HP;
        Data.HP += amount;
        Data.HP = Mathf.Min(Data.UnitClass.MaxHP, Data.HP);
        //scale and play the heal effect
        var amountHealed = Data.HP - oldHealth;
        float effectStrength = amountHealed / 25;
        HealEffect.PlayEffect(effectStrength);
        //return amount healed
        return amountHealed;
    }
    public float DamageUnit(float amount)
    {
        if(amount <= 0)
        {
            return HealUnit(-amount);
        }
        float oldHealth = Data.HP;
        Data.HP -= amount;
        Data.HP = Math.Max(0, Data.HP);

        var damage = oldHealth - Data.HP;
        return damage;
    }
    public float ReloadUnit(float amount)
    {
        //resupply the unit by amount, up to max mp
        float oldAmmo = Data.MP;
        Data.MP += amount;
        Data.MP = Mathf.Min(Data.UnitClass.MaxMP, Data.MP);
        //scale and play the reload effect
        var amountLoaded = Data.MP - oldAmmo;
        float effectStrength = amountLoaded / 25;
        ReloadEffect.PlayEffect(effectStrength);
        //return amount loaded
        return amountLoaded;
    }
    public float DrainUnit(float amount)
    {
        if (amount <= 0)
        {
            return ReloadUnit(-amount);
        }
        float oldAmmo = Data.MP;
        Data.MP -= amount;
        Data.MP = Math.Max(0, Data.MP);

        var drain = oldAmmo - Data.MP;
        return drain;
    }
    public void DoAttack(UnitController target)
    {
        if(CommandTarget != target)
        {
            CancelOrders();//cancel all other orders
            CommandTarget = target;
        }
    }
    //move to the specified location, stopping to attack all enemies encountered on the way.  Cancel if another order is given
    public void DoAttackMove(Vector3 location)
    {
        if (!Agent.hasPath || Vector3.Distance(location, Agent.destination) > ORDER_RADIUS)
        {
            CancelOrders();//cancel all other orders
            //set the move-attack destination
            AttackMoveDestination = location;
        }
    }
    //attack the ground at the specified location. keep attacking until unit is given another order
    public void DoForceAttack(Vector3 location)
    {
        CancelOrders();//cancel all other orders
        //set force-attack target
        ForceTarget = location;
    }
    public void DoSpecialAbility(Vector3 location)
    {
        CancelOrders();
        var specialAbility = Data.UnitClass.SpecialAbility;
        //if ability is targeted, activate ability at location
        if (specialAbility.IsTargetedAbility)
        {
            AbilityTarget = location;
        }
        else
        {
            //TODO: immediately activate ability
        }
    }
    //cancel all orders given to this unit
    public void CancelOrders()
    {
        //stop unit
        //Agent.ResetPath();
        StopMoving();
        //remove attack-move destination
        AttackMoveDestination = null;
        //stop command attack
        CommandTarget = null;
        //stop force attack
        ForceTarget = null;
        //stop targeting ability
        AbilityTarget = null;

    }
    public void DoMove(Vector3 location, bool cancelOrders = false)
    {
        if (!Agent.hasPath || Vector3.Distance(location, Agent.destination) > ORDER_RADIUS)
        {
            if (cancelOrders)
            {
                CancelOrders();
            }
            
            Agent.SetDestination(location);
        } 
    }
    public void StopMoving()
    {
        Agent.ResetPath();
        
        zoneMultiplier = 1;
    }
    public bool HasLineOfSight(Vector3 target)
    {
        var pos = transform.position;

        var hits = Physics.RaycastAll(pos, target - pos, Vector3.Distance(pos, target));
        foreach (var h in hits)
        {
            var unit = h.collider.GetComponent<UnitController>();
            if (!h.collider.CompareTag("NonBlocking") && !h.collider.isTrigger
                && unit == null)
            {
                return false;
            }
        }
        return true;
        //if(hits.Length > 0)
        //{

        //}
        //return false;
    }
    public bool HasTrueLineOfSight(UnitController target)
    {
        var pos = transform.position;
        var targetPos = target.transform.position;

        var hits = Physics.RaycastAll(pos, targetPos - pos, Vector3.Distance(pos, targetPos));
        foreach (var h in hits)
        {
            var unit = h.collider.GetComponent<UnitController>();
            if(unit == target)
            {
                return true;
            }
            else if (!h.collider.CompareTag("NonBlocking")//pass through non-block objects 
                && !h.collider.isTrigger//pass through triggers
                && (unit == null || unit.Data.Team != Data.Team))//blocked by non-units, and enemy units that aren't your target
            {
                return false;
            }
        }
        return true;
    }
    #endregion
    #region private methods
    private void DoUnitAction()
    {
        bool hasCommandTarget = false;
        if(CommandTarget == null)
        {
            hasCommandTarget = DoCommandAttack();
            
        }
        else if(Data.Team == CommandTarget.Data.Team)
        {
            //perform support action (if it exists)
            if(Data.UnitClass.PrimaryWeapon.TargetsAllies() || Data.UnitClass.SecondaryWeapon.TargetsAllies())
            {
                hasCommandTarget = DoCommandAttack();
            }
            else
            {
                //else move to friendly unit's position
                DoMove(CommandTarget.transform.position, true);
                CommandTarget = null;
            }
        }
        else
        {
            hasCommandTarget = DoCommandAttack();
        }

        if (!hasCommandTarget)
        {
            //autoattack
            DoAutoAttacks();
        }
    }
    //if the command target is within range, attack it.
    //if command target is outside the primary weapon's range, move to engage
    //applies to both force-attacks and regular command attacks
    private bool DoCommandAttack()
    {
        Vector3? targetPosition = null;
        bool isAlly = false;
        DoAttackMove();//if unit is set to attack-move, this will choose a command target if available

        if(CommandTarget != null)
        {
            targetPosition = CommandTarget.transform.position;
            isAlly = CommandTarget.Data.Team == Data.Team;
        }
        else if(ForceTarget != null)
        {
            targetPosition = ForceTarget;
        }
        else if(AbilityTarget != null)
        {
            targetPosition = AbilityTarget;
        }

        if(targetPosition != null)
        {
            var target = targetPosition ?? new Vector3();

            var activeWeapon = GetActiveWeapon(target, false, false, isAlly);
           


            //perform attack action on unit
            //if can attack (not counting cooldowns), do so
            if (activeWeapon != null)
            {

                DoPreAttack(activeWeapon, target, true);
                //if ambidextrous:
                if (Data.UnitClass.IsAmbidextrous)
                {
                    //get an auto-attack target for the other weapon
                    //get rotation for command target
                    var flatTarget = new Vector3(AutoTarget1.transform.position.x, transform.position.y, AutoTarget1.transform.position.z);//only look on the y-axis
                    var commandRotation = Quaternion.LookRotation(flatTarget - transform.position);
                    //get other weapon
                    if (activeWeapon == Data.UnitClass.PrimaryWeapon)
                    {
                        var otherWeapon = Data.UnitClass.SecondaryWeapon;
                        AutoTarget2 = GetAutoAttackTarget(otherWeapon, AutoTarget2, Agent.hasPath, true, commandRotation, activeWeapon.FiringArc);
                        DoPreAttack(otherWeapon, AutoTarget2.transform.position, true, false);
                    }
                    else
                    {
                        var otherWeapon = Data.UnitClass.PrimaryWeapon;
                        AutoTarget1 = GetAutoAttackTarget(otherWeapon, AutoTarget1, Agent.hasPath, true, commandRotation, activeWeapon.FiringArc);
                        DoPreAttack(otherWeapon, AutoTarget1.transform.position, true, false);
                    }
                }
                return true;
            }
            else//move to engage
            {
                DoMove(target, false);
                return false;
            }
        }
        return false;
    }

    private void DoAttackMove()
    {
        //if unit has a attack-move destination,
        if(AttackMoveDestination != null)
        {
            //and does not already have a command target
            if(CommandTarget == null)
            {
                //auto-acquire a command target
                CommandTarget = GetAutoAttackTarget(false);
                
            }
            //if no targets found, move towards attack-move destination
            if(CommandTarget == null)
            {
                //Agent.SetDestination(AttackMoveDestination ?? new Vector3());
                DoMove(AttackMoveDestination ?? new Vector3(), false);
            }
        }
    }
    private void DoAutoAttacks()
    {
        //if ambidextrous:
        if (Data.UnitClass.IsAmbidextrous)
        {
            //get an auto-attack target for primary and secondary weapon
            //prefer existing auto-attack targets
            AutoTarget1 = GetAutoAttackTarget(Data.UnitClass.PrimaryWeapon, AutoTarget1, Agent.hasPath, true);
            Quaternion primaryRotation = new Quaternion();
            float primaryArc = 360;
            if(AutoTarget1 != null)
            {
                var flatTarget = new Vector3(AutoTarget1.transform.position.x, transform.position.y, AutoTarget1.transform.position.z);//only look on the y-axis
                primaryRotation = Quaternion.LookRotation(flatTarget - transform.position);
                primaryArc = Data.UnitClass.PrimaryWeapon.FiringArc;
            }
            AutoTarget2 = GetAutoAttackTarget(Data.UnitClass.SecondaryWeapon, AutoTarget2, Agent.hasPath, true, primaryRotation, primaryArc);
            
            //if either the primary or secondary weapons have targets
            if(AutoTarget1 != null || AutoTarget2 != null)
            {
                //do pre attack and stop agent steering
                if(AutoTarget1 != null && AutoTarget2 != null)
                {
                    DoPreAttacks(AutoTarget1.transform.position, AutoTarget2.transform.position, false);
                }
                else if(AutoTarget1 != null)
                {
                    DoPreAttack(Data.UnitClass.PrimaryWeapon, AutoTarget1.transform.position, false);
                }
                else if (AutoTarget2 != null)
                {
                    DoPreAttack(Data.UnitClass.SecondaryWeapon, AutoTarget2.transform.position, false);
                }
                //disable agent turning
                Agent.angularSpeed = 0;
            }
            else
            {
                //else re-enable agent steering
                Agent.angularSpeed = Data.UnitClass.TurnSpeed / Mathf.PI * 180;
            }
            
        }
        else
        {
            DoAutoAttack();
        }

        //if single-weapon: do a single attack, preferring the primary target
    }
    private void DoAutoAttack()
    {
        //get weapon to attack with
        var activeWeapon = GetActiveWeapon(AutoTarget1, Agent.hasPath, true);
        //check existing autoattack target
        //if invalid, get new target
        if (activeWeapon == null)
        {
            AutoTarget1 = GetAutoAttackTarget();
            activeWeapon = GetActiveWeapon(AutoTarget1, Agent.hasPath, true);
        }
        if(activeWeapon != null)
        {
            DoPreAttack(activeWeapon, AutoTarget1.transform.position, false);
            //disable agent turning
            Agent.angularSpeed = 0;
        }
        else
        {
            Agent.angularSpeed = Data.UnitClass.TurnSpeed/Mathf.PI * 180;
        }

    }
    private void DoPreAttacks(Vector3 primaryTarget, Vector3 secondaryTarget, bool stopMoving)
    {
        //turn towards the target whose weapon has the smaller arc (if equal, turn towards primary target)
        var primaryWeapon = Data.UnitClass.PrimaryWeapon;
        var secondaryWeapon = Data.UnitClass.SecondaryWeapon;
        DoPreAttack(primaryWeapon, primaryTarget, stopMoving, primaryWeapon.FiringArc >= secondaryWeapon.FiringArc);
        DoPreAttack(secondaryWeapon, secondaryTarget, stopMoving, secondaryWeapon.FiringArc > primaryWeapon.FiringArc);
    }
    private void DoPreAttack(WeaponModel activeWeapon, Vector3 target, bool stopMoving, bool turnUnit = true)
    {
        //turn towards target
        if (turnUnit)
        {
            var flatTarget = new Vector3(target.x, transform.position.y, target.z);//only turn on the y-axis
            var targetRotation = Quaternion.LookRotation(flatTarget - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Min(Data.UnitClass.TurnSpeed * Time.deltaTime, 1));
            var remainingAngle = Quaternion.Angle(targetRotation, transform.rotation);
        }
        var weaponMount = GetWeaponController(activeWeapon);
        //if facing the the target, and mounts are either also facing the target, or cannot rotate more, fire the weapon
        bool weaponReady = weaponMount.TraverseMounts(activeWeapon, target, transform.rotation);
        if (weaponReady)
        {
            //once facing target, fire weapon
            DoAttack(activeWeapon, target);
            if (stopMoving)
            {
                //Agent.ResetPath();//stop moving
                StopMoving();
            }
        }
        else
        {
            //Debug.Log("Remaining Angle: " + remainingAngle);
        }
    }
    private WeaponController GetWeaponController(WeaponModel activeWeapon)
    {
        WeaponController weaponMount = PrimaryWeaponMount;
        if (activeWeapon == Data.UnitClass.SecondaryWeapon)
        {
            weaponMount = SecondaryWeaponMount;
        }
        else if (activeWeapon == Data.UnitClass.SpecialAbility.AbilityWeapon)
        {
            weaponMount = AbilityWeaponMount;
        }
        return weaponMount;
    }
    private void DoAttack(WeaponModel activeWeapon, Vector3 target)
    {
        if(activeWeapon != null && activeWeapon.IsCooledDown())
        {
            var weaponMount = GetWeaponController(activeWeapon);

            weaponMount.Fire(this, activeWeapon, transform.parent.gameObject, target);
            activeWeapon.StartCooldown();
            Data.MP -= activeWeapon.AmmoCost;
            var specialAbility = Data.UnitClass.SpecialAbility;
            if (activeWeapon == specialAbility.AbilityWeapon && !specialAbility.IsContinuous)
            {
                CancelOrders();
            }
        }
    }
    private int CanAttackWithWeapon(WeaponModel weapon, Vector3 target, bool isMoving, bool isAutoAttack, bool isAlly)
    {
        //reason code:
        //0: can attack as normal
        //1: on cooldown
        //2: no line-of-sight
        //3: out of range
        //4: out of ammo
        //5: can't auto-attack
        //6: can't attack while moving
        //7: invalid unit team
        int reason = 0;
        if(weapon.TargetsAllies() != isAlly)
        {
            reason = 7;
        }
        if(isMoving && !weapon.FireWhileMoving)
        {
            reason = 6;
        }
        else if(isAutoAttack && !weapon.CanAutoAttack)
        {
            reason = 5;
        }
        else if(weapon.AmmoCost > Data.MP)
        {
            reason = 4;
        }
        else if(weapon.MaxRange < Vector3.Distance(transform.position, target)){
            reason = 3;
        }
        else if(weapon.NeedsLineOfSight() && !HasLineOfSight(target))
        {
            reason = 2;
        }
        else if (!weapon.IsCooledDown())
        {
            reason = 1;
        }

        return reason;
    }
    private WeaponModel GetActiveWeapon(Vector3 target, bool isMoving, bool isAutoAttack, bool isAlly)
    {
        if(AbilityTarget != null && Data.UnitClass.SpecialAbility.IsWeaponAbility)
        {
            var abilityWeapon = Data.UnitClass.SpecialAbility.AbilityWeapon;
            int abiltyReason = CanAttackWithWeapon(abilityWeapon, target, isMoving, isAutoAttack, isAlly);
            if (abiltyReason == 0 || abiltyReason == 1)
            {
                return abilityWeapon;
            }
        }
        else
        {
            int primaryReason = CanAttackWithWeapon(Data.UnitClass.PrimaryWeapon, target, isMoving, isAutoAttack, isAlly);
            if (primaryReason == 0 || primaryReason == 1)
            {
                return Data.UnitClass.PrimaryWeapon;
            }

            int secondaryReason = CanAttackWithWeapon(Data.UnitClass.SecondaryWeapon, target, isMoving, isAutoAttack, isAlly);
            if (secondaryReason == 0 || secondaryReason == 1)
            {
                return Data.UnitClass.SecondaryWeapon;
            }
        }
        
        return null;
    }
    private WeaponModel GetActiveWeapon(UnitController target, bool isMoving, bool isAutoAttack)
    {
        if(target != null)
        {
            bool isAlly = target.Data.Team == Data.Team;
            return GetActiveWeapon(target.transform.position, isMoving, isAutoAttack, isAlly);
        }
        return null;
    }
    private void DoWeaponCooldown(float time)
    {
        Data.UnitClass.PrimaryWeapon.DoCooldown(time);
        Data.UnitClass.SecondaryWeapon.DoCooldown(time);
        Data.UnitClass.SpecialAbility.AbilityWeapon.DoCooldown(time);
    }

    private UnitController GetAutoAttackTarget(bool autoAttackOnly = true)
    {
        UnitController target = null;
        UnitController bestTarget = null;
        //get the nearest valid target and attack it
        var map = transform.parent.gameObject;
        var units = map.GetComponentsInChildren<UnitController>();
        float bestDistance = 0;
        foreach(var u in units)
        {
            if(u.Data.Team != Data.Team)
            {
                //choose closest target in line-of-sight
                var distance = Vector3.Distance(u.transform.position, transform.position);
                if(target == null || distance < bestDistance)
                {
                    //prefer units in line-of-sight
                    bestDistance = distance;
                    target = u;
                    if ((!Data.UnitClass.PrimaryWeapon.NeedsLineOfSight() && (!autoAttackOnly || Data.UnitClass.PrimaryWeapon.CanAutoAttack))
                        || (!Data.UnitClass.SecondaryWeapon.NeedsLineOfSight() && (!autoAttackOnly || Data.UnitClass.SecondaryWeapon.CanAutoAttack))
                        || HasLineOfSight(u.transform.position))
                    {
                        bestTarget = u;
                    }
                }
            }
        }
        if(bestTarget == null)
        {
            return target;
        }
        else
        {
            return bestTarget;
        }
    }
    private UnitController GetAutoAttackTarget(WeaponModel weapon, UnitController target, bool isMoving, bool autoAttackOnly = true, Quaternion otherRotation = new Quaternion(), float otherArc = 360)
    {
        if (target != null)
        {
            bool isAlly = target.Data.Team == Data.Team;
            var reason = CanAttackWithWeapon(weapon, target.transform.position, isMoving, true, isAlly);
            if (reason == 0 || reason == 1)
            {
                return target;
            }
        }
        //get a new target
        UnitController newTarget = null;
        UnitController bestTarget = null;
        //get the nearest valid target and attack it
        var map = transform.parent.gameObject;
        var units = map.GetComponentsInChildren<UnitController>();
        float bestDistance = 0;
        foreach (var u in units)
        {
            if (u.Data.Team == Data.Team == weapon.TargetsAllies())
            {
                //choose closest target in line-of-sight
                //and within arc
                float arc = Mathf.Max(weapon.FiringArc, otherArc);
                if(arc < 360)
                {
                    //calculate angle to potential target
                    var flatTarget = new Vector3(u.transform.position.x, transform.position.y, u.transform.position.z);//only look on the y-axis
                    var targetRotation = Quaternion.LookRotation(flatTarget - transform.position);
                    var angle = Quaternion.Angle(targetRotation, otherRotation);
                    if(angle > arc)
                    {
                        continue;//skip this as a valid target
                    }
                }
                

                var distance = Vector3.Distance(u.transform.position, transform.position);
                if (newTarget == null || distance < bestDistance)
                {
                    //prefer units in line-of-sight
                    bestDistance = distance;
                    newTarget = u;
                    if ((!weapon.NeedsLineOfSight() && (!autoAttackOnly || weapon.CanAutoAttack))
                        || HasLineOfSight(u.transform.position))
                    {
                        bestTarget = u;
                    }
                }
            }
        }
        if (bestTarget == null)
        {
            return newTarget;
        }
        else
        {
            return bestTarget;
        }
    }
    private void DoLootDrop()
    {
        var loot = Instantiate(DeathLoot, transform.parent);
        loot.transform.position = transform.position + new Vector3(0, .32f, 0);
        loot.ThrowPack();
    }
    private void DoDeathExplosion()
    {
        DeathExplosion.Instantiate(transform.parent, transform.position);
    }
    private void Recolor(int team)
    {
        var color = TeamTools.GetTeamColor(team);
        foreach (var p in TeamColorParts)
        {
            p.Part.materials[p.TeamColorIndex].color = color;
        }
        var particleMain = JetStream.main;
        particleMain.startColor = color;
        var particleRespawn = RespawnEffect.main;
        particleRespawn.startColor = color;
    }
    private void UpdateTooltip()
    {
        ToolTip.Header = Data.UnitClass.Name;
        ToolTip.Body = Data.UnitClass.Description;
        var primaryWeapon = Data.UnitClass.PrimaryWeapon;
        var secondaryWeapon = Data.UnitClass.SecondaryWeapon;
        ToolTip.Stats = new string[]
        {
            string.Format("Primary Weapon: {0} ({1} damage, {2}/s)", primaryWeapon.Name, primaryWeapon.HealthDamage, 1/primaryWeapon.Cooldown),
            string.Format("Secondary Weapon: {0} ({1} damage, {2}/s)", secondaryWeapon.Name, secondaryWeapon.HealthDamage, 1/secondaryWeapon.Cooldown),
            string.Format("Special Ability: {0}", Data.UnitClass.SpecialAbility.Name)
        };
        ToolTip.AmmoCost = Data.MP + "/" + Data.UnitClass.MaxMP;
        ToolTip.HealthCost = Data.HP + "/" + Data.UnitClass.MaxHP;
    }
    #endregion

}
