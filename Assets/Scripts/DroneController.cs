using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class DroneController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public UnitClassTemplates.UnitClasses UnitClass;
    public GameObject UnitAppearance;
    public Animator Animations;
    public GameObject UnitEffects;
    public ResourceBarController HealthBar;
    public ResourceBarController AmmoBar;
    public WeaponController PrimaryWeaponMount;
    public WeaponController SecondaryWeaponMount;
    public int Team = -1;//TEMP
    public PickupController DeathLoot;
    public ParticleSystem RespawnEffect;
    public VariableEffect HealEffect;
    public VariableEffect ReloadEffect;
    public SpecialEffectController DeathExplosion;
    public ToolTipContentController ToolTip;
    public ParticleSystem JetStream;
    public bool CanAttack = true;
    public Vector3 TargetOffset;
    public MeshRecolorModel[] TeamColorParts;

    #endregion
    #region private fields
    protected Quaternion initialRotation;
    protected List<GameObject> conditionEffects = new List<GameObject>();
    #endregion
    #region properties
    public UnitModel Data { get; set; }
    public List<UnitConditionModel> Conditions { get; set; } = new List<UnitConditionModel>();
    
    public DroneController AutoTarget1 { get; set; }
    public DroneController AutoTarget2 { get; set; }
    public List<Action> DeathActions { get; set; } = new List<Action>();
    public virtual bool IsMoving
    {
        get
        {
            return false;
        }
    }
    public Vector3 TargetingPosition
    {
        get
        {
            return transform.position + TargetOffset;
        }
    }

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }
    protected virtual void OnStart()
    {
        //TEMP: initialize data model
        if (Data == null)
        {
            //get data class from class name
            Data = new UnitModel(UnitClassTemplates.GetClassByName(UnitClass));
        }

        initialRotation = UnitEffects.transform.rotation;

        //set ownership of weapons
        if (Data.UnitClass.PrimaryWeapon != null)
        {
            Data.UnitClass.PrimaryWeapon.Owner = this;
        }
        if (Data.UnitClass.SecondaryWeapon != null)
        {
            Data.UnitClass.SecondaryWeapon.Owner = this;
        }
        if (Data.UnitClass.TargetedAbility?.AbilityWeapon != null)
        {
            Data.UnitClass.TargetedAbility.AbilityWeapon.Owner = this;
        }
        if (Data.UnitClass.ActivatedAbility?.AbilityWeapon != null)
        {
            Data.UnitClass.ActivatedAbility.AbilityWeapon.Owner = this;
        }

        //TEMP: set team from public property
        if (Team >= 0)
        {
            Data.Team = Team;
        }
        //TEMP: set teamcolor
        Recolor(Data.Team);

        //apply passive conditions
        foreach (var c in Data.UnitClass.PassiveConditions)
        {
            ApplyCondition(c);
        }
    }
    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
    protected virtual void OnUpdate()
    {
        float deltaTime = Time.deltaTime;

        DoWeaponCooldown(deltaTime);
        DoUnitAction();

        UnitEffects.transform.rotation = Camera.main.transform.rotation;//orient unit UI towards camera

        //update unit status
        //update resource bars
        HealthBar.UpdateBar(Data.UnitClass.MaxHP, Data.HP);
        AmmoBar.UpdateBar(Data.UnitClass.MaxMP, Data.MP);

        //update tooltip
        UpdateTooltip();

        //death check
        if (Data.HP <= 0)
        {
            Kill();
        }
        //update conditions
        var tempConditions = new List<UnitConditionModel>(Conditions);
        foreach (var c in tempConditions)
        {
            c.DoTimeElapsed(this, deltaTime);
        }
    }

    #endregion
    #region events
    public event EventHandler<DroneController> OnKillEnemy;
    public void DoKillEnemy(object sender, DroneController target)
    {
        OnKillEnemy?.Invoke(sender, target);
        var tempList = new List<UnitConditionModel>(Conditions);
        foreach (var c in tempList)
        {
            c.DoKillEnemy(sender, target);
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
        Destroy(gameObject);
    }
    
    public virtual void SpawnSetup(Vector3 position, int team, bool hideUI)
    {
        Data = new UnitModel(UnitClassTemplates.GetClassByName(UnitClass));
        Data.Team = team;
        transform.position = position;

        if (hideUI)
        {
            AmmoBar.gameObject.SetActive(false);
        }
        var map = GetComponentInParent<MapController>();
        map.RegisterUnit(this);

        DeathActions.Add(DoLootDrop);
        DeathActions.Add(DoDeathExplosion);
        DeathActions.Add(UnRegister);
        
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
    public virtual float HealUnit(float amount)
    {
        //heal the unit by amount, up to max hp
        float oldHealth = Data.HP;
        Data.HP += amount;
        Data.HP = Mathf.Min(Data.UnitClass.MaxHP, Data.HP);
        //scale and play the heal effect
        var amountHealed = Data.HP - oldHealth;
        float effectStrength = amountHealed;
        HealEffect.PlayEffect(effectStrength);
        //return amount healed
        return amountHealed;
    }
    public virtual float DamageUnit(float amount)
    {
        if(amount <= 0)
        {
            return HealUnit(-amount);
        }
        else if (Data.IsDamageable)
        {
            float oldHealth = Data.HP;
            Data.HP -= amount;
            Data.HP = Math.Max(0, Data.HP);

            var damage = oldHealth - Data.HP;
            return damage;
        }
        else
        {
            return 0;
        }
    }
    public virtual float ReloadUnit(float amount)
    {
        //resupply the unit by amount, up to max mp
        float oldAmmo = Data.MP;
        Data.MP += amount;
        Data.MP = Mathf.Min(Data.UnitClass.MaxMP, Data.MP);
        //scale and play the reload effect
        var amountLoaded = Data.MP - oldAmmo;
        float effectStrength = amountLoaded;
        ReloadEffect.PlayEffect(effectStrength);
        //return amount loaded
        return amountLoaded;
    }
    public virtual float DrainUnit(float amount)
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
    public virtual void StopMoving()
    {
        //construct is immobile
    }
    public bool HasLineOfSight(Vector3 target)
    {
        var pos = TargetingPosition;

        var hits = Physics.RaycastAll(pos, target - pos, Vector3.Distance(pos, target));
        foreach (var h in hits)
        {
            var unit = h.collider.GetComponent<DroneController>();
            if (!h.collider.CompareTag("NonBlocking") && !h.collider.isTrigger
                && unit == null && Vector3.Distance(h.point, target) > 0.01f)
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
    public bool HasTrueLineOfSight(DroneController target)
    {
        var pos = TargetingPosition;
        var targetPos = target.TargetingPosition;

        var hits = Physics.RaycastAll(pos, targetPos - pos, Vector3.Distance(pos, targetPos));
        foreach (var h in hits)
        {
            var unit = h.collider.GetComponent<DroneController>();
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
    public void ApplyCondition(UnitConditionModel condition)
    {
        //check conditions for pre-existing instance of conition
        UnitConditionModel foundCondition = null;
        for (int i = 0; i < Conditions.Count; i++)
        {
            if (Conditions[i].Name == condition.Name)
            {
                foundCondition = Conditions[i];
            }
        }
        //if found, stack condition, trigger stack event
        if (foundCondition != null)
        {
            foundCondition.DoConditionStack(this, condition);
        }
        //else add condition to list and trigger application event
        else
        {
            Conditions.Add(condition);
            //generate and register visual effect
            condition.DoConditionStart(this, condition);
            if (!string.IsNullOrWhiteSpace(condition.VisualEffectName))
            {
                var visualEffect = Resources.Load<GameObject>(condition.VisualEffectName);
                condition.VisualEffectController = Instantiate(visualEffect, UnitEffects.transform);
                conditionEffects.Add(condition.VisualEffectController);
            }

            //if valid, add  console status
            var unit = this as UnitController;
            if (unit?.SpawnSlot?.Controller != null && !string.IsNullOrWhiteSpace(condition.ConsoleEffectName))
            {
                condition.ConsoleEffectController = unit.SpawnSlot.Controller.StatusConsole.CreateStatusLine(condition.ConsoleEffectName);
            }
        }

    }
    public void RemoveCondition(UnitConditionModel condition)
    {
        //check that condition is applied
        UnitConditionModel foundCondition = null;
        for (int i = 0; i < Conditions.Count; i++)
        {
            if (Conditions[i].Name == condition.Name)
            {
                foundCondition = Conditions[i];
            }
        }
        if (foundCondition != null)
        {
            //remove visual effet, if it exists
            if (foundCondition.VisualEffectController != null)
            {
                conditionEffects.Remove(foundCondition.VisualEffectController);
                Destroy(foundCondition.VisualEffectController);
            }
            //if relevant, remove console status
            var unit = this as UnitController;
            if (unit?.SpawnSlot?.Controller != null)
            {
                unit.SpawnSlot.Controller.StatusConsole.RemoveStatusLine(foundCondition.ConsoleEffectController);
            }

            Conditions.Remove(foundCondition);
            foundCondition.DoConditionEnd(this, foundCondition);
        }
    }
    #region stat calculators
    public float GetMoveSpeed()
    {
        float flat = 0;
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            flat += c.UnitMoveSpeedFlat;
            prop += c.UnitMoveSpeedProp;
        }
        return Data.UnitClass.MoveSpeed * prop + flat;
    }
    public float GetTurnSpeed()
    {
        float flat = 0;
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            flat += c.UnitTurnSpeedFlat;
            prop += c.UnitTurnSpeedProp;
        }
        return Data.UnitClass.TurnSpeed * prop + flat;
    }
    public bool GetJumpBoost()
    {
        foreach (var c in Conditions)
        {
            if (c.UnitHasJumpBoost)
            {
                return true;
            }
        }
        return false;
    }
    public bool GetTargetedAbilityDisabled()
    {
        foreach (var c in Conditions)
        {
            if (c.UnitTargetedAbilityDisabled)
            {
                return true;
            }
        }
        return false;
    }
    public bool GetActivatedAbilityDisabled()
    {
        foreach (var c in Conditions)
        {
            if (c.UnitActivatedAbilityDisabled)
            {
                return true;
            }
        }
        return false;
    }
    public float GetWeaponMaxRange(WeaponModel weapon)
    {
        float flat = 0;
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            flat += c.WeaponMaxRangeFlat * c.Stacks;
            prop += c.WeaponMaxRangeProp * c.Stacks;
        }
        return weapon.MaxRange * prop + flat;
    }
    public float GetWeaponMinRange(WeaponModel weapon)
    {
        float flat = 0;
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            flat += c.WeaponMinRangeFlat * c.Stacks;
            prop += c.WeaponMinRangeProp * c.Stacks;
        }
        return weapon.MinRange * prop + flat;
    }
    public float GetWeaponProjectileSpeed(WeaponModel weapon)
    {
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            prop += c.WeaponProjectileSpeedProp * c.Stacks;
        }
        return weapon.ProjectileSpeed * prop;
    }
    public float GetWeaponCoolDown(WeaponModel weapon)
    {
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            prop += c.WeaponCooldownProp * c.Stacks;
        }
        return weapon.Cooldown * prop;
    }
    public float GetWeaponHealthDamage(WeaponModel weapon)
    {
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            prop += c.WeaponHealthDamageProp * c.Stacks;
        }
        return weapon.HealthDamage * prop;
    }
    public float GetWeaponAmmoDamage(WeaponModel weapon)
    {
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            prop += c.WeaponAmmoDamageProp * c.Stacks;
        }
        return weapon.AmmoDamage * prop;
    }
    public float GetWeaponInaccuracy(WeaponModel weapon)
    {
        float flat = 0;
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            flat += c.WeaponInaccuracyFlat * c.Stacks;
            prop += c.WeaponInaccuracyProp * c.Stacks;
        }
        return weapon.InAccuracy * prop + flat;
    }
    public bool GetWeaponPiercesWalls(WeaponModel weapon)
    {
        if (!weapon.PiercesWalls)
        {
            foreach (var c in Conditions)
            {
                if (c.WeaponPiercesWalls)
                {
                    return true;
                }
            }
        }
        
        return weapon.PiercesWalls;
    }
    public bool GetWeaponPiercesUnits(WeaponModel weapon)
    {
        if (!weapon.PiercesUnits)
        {
            foreach (var c in Conditions)
            {
                if (c.WeaponPiercesUnits)
                {
                    return true;
                }
            }
        }
        
        return weapon.PiercesUnits;
    }
    public float GetWeaponAmmoCost(WeaponModel weapon)
    {
        float flat = 0;
        float prop = 1;
        //get sum of flat and proportional modifiers from conditions
        foreach (var c in Conditions)
        {
            flat += c.WeaponAmmoCostFlat * c.Stacks;
            prop += c.WeaponAmmoCostProp * c.Stacks;
        }
        return weapon.AmmoCost * prop + flat;
    }
    public bool GetCanAutoAttack(WeaponModel weapon)
    {
        if (weapon.CanAutoAttack)
        {
            foreach (var c in Conditions)
            {
                if (c.WeaponDisableAutoAttack)
                {
                    return false;
                }
            }
        }
        
        return weapon.CanAutoAttack;
    }
    public bool GetCanTargetAttack(WeaponModel weapon)
    {
        if (weapon.CanTargetAttack)
        {
            foreach (var c in Conditions)
            {
                if (c.WeaponDisableTargetAttack)
                {
                    return false;
                }
            }
        }
        
        return weapon.CanTargetAttack;
    }
    public bool GetCanFireWhileMoving(WeaponModel weapon)
    {
        if (weapon.FireWhileMoving)
        {
            foreach (var c in Conditions)
            {
                if (c.WeaponDisableFireWhileMoving)
                {
                    return false;
                }
            }
        }
        
        return weapon.FireWhileMoving;
    }

    #endregion
    #endregion
    #region private methods
    protected virtual void DoUnitAction()
    {
        //autoattack
        DoAutoAttacks();
    }
    protected virtual bool DoAutoAttacks()
    {
        //if ambidextrous:
        if (Data.UnitClass.IsAmbidextrous)
        {
            //get an auto-attack target for primary and secondary weapon
            //prefer existing auto-attack targets
            AutoTarget1 = GetAutoAttackTarget(Data.UnitClass.PrimaryWeapon, AutoTarget1, IsMoving, true);
            Quaternion primaryRotation = new Quaternion();
            float primaryArc = 360;
            if(AutoTarget1 != null)
            {
                var flatTarget = new Vector3(AutoTarget1.transform.position.x, transform.position.y, AutoTarget1.transform.position.z);//only look on the y-axis
                primaryRotation = Quaternion.LookRotation(flatTarget - transform.position);
                primaryArc = Data.UnitClass.PrimaryWeapon.FiringArc;
            }
            AutoTarget2 = GetAutoAttackTarget(Data.UnitClass.SecondaryWeapon, AutoTarget2, IsMoving, true, primaryRotation, primaryArc);
            
            //if either the primary or secondary weapons have targets
            if(AutoTarget1 != null || AutoTarget2 != null)
            {
                //do pre attack(s) and stop agent steering
                if(AutoTarget1 != null && AutoTarget2 != null)
                {
                    DoPreAttacks(AutoTarget1.transform.position, AutoTarget2.TargetingPosition, false);
                }
                else if(AutoTarget1 != null)
                {
                    DoPreAttack(Data.UnitClass.PrimaryWeapon, AutoTarget1.TargetingPosition, false);
                }
                else if (AutoTarget2 != null)
                {
                    DoPreAttack(Data.UnitClass.SecondaryWeapon, AutoTarget2.TargetingPosition, false);
                }
                //disable agent turning
                return true;
            }
            else
            {
                //else re-enable agent steering
                return false;
            }
            
        }
        else
        {
            return DoAutoAttack();
        }

        //if single-weapon: do a single attack, preferring the primary target
    }
    protected virtual bool DoAutoAttack()
    {
        //get weapon to attack with
        var activeWeapon = GetActiveWeapon(AutoTarget1, IsMoving, true);
        //check existing autoattack target
        //if invalid, get new target
        if (activeWeapon == null)
        {
            AutoTarget1 = GetAutoAttackTarget();
            activeWeapon = GetActiveWeapon(AutoTarget1, IsMoving, true);
        }
        if(activeWeapon != null)
        {
            DoPreAttack(activeWeapon, AutoTarget1.TargetingPosition, false);
            //disable agent turning
            return true;
        }
        else
        {
            return false;
        }

    }
    protected void DoPreAttacks(Vector3 primaryTarget, Vector3 secondaryTarget, bool stopMoving)
    {
        //turn towards the target whose weapon has the smaller arc (if equal, turn towards primary target)
        var primaryWeapon = Data.UnitClass.PrimaryWeapon;
        var secondaryWeapon = Data.UnitClass.SecondaryWeapon;
        DoPreAttack(primaryWeapon, primaryTarget, stopMoving, primaryWeapon.FiringArc <= secondaryWeapon.FiringArc);
        DoPreAttack(secondaryWeapon, secondaryTarget, stopMoving, secondaryWeapon.FiringArc < primaryWeapon.FiringArc);
    }
    protected void DoPreAttack(WeaponModel activeWeapon, Vector3 target, bool stopMoving, bool turnUnit = true)
    {
        if(activeWeapon == null)
        {
            return;
        }
        
        //turn towards target
        if (turnUnit)
        {
            var flatTarget = new Vector3(target.x, transform.position.y, target.z);//only turn on the y-axis
            var targetRotation = Quaternion.LookRotation(flatTarget - transform.position);
            var remainingAngle = Quaternion.Angle(targetRotation, transform.rotation);
            if (!Data.UnitClass.IsAmbidextrous || activeWeapon.FiringArc/2 < remainingAngle)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Min(GetTurnSpeed() * Time.deltaTime, 1));
            }
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
    }
    protected virtual WeaponController GetWeaponController(WeaponModel activeWeapon)
    {
        WeaponController weaponMount = PrimaryWeaponMount;
        if (activeWeapon == Data.UnitClass.SecondaryWeapon)
        {
            weaponMount = SecondaryWeaponMount;
        }
        return weaponMount;
    }
    protected virtual void DoAttack(WeaponModel activeWeapon, Vector3 target)
    {
        if(activeWeapon != null && activeWeapon.IsCooledDown())
        {
            var weaponMount = GetWeaponController(activeWeapon);

            weaponMount.Fire(this, activeWeapon, transform.parent.gameObject, target);
            activeWeapon.StartCooldown();
            Data.MP -= GetWeaponAmmoCost(activeWeapon);
        }
    }
    protected bool CanAttackWithWeapon(WeaponModel weapon, DroneController target, bool isMoving, bool isAutoAttack)
    {
        return weapon != null 
            && target != null
            && target != this
            && target.Data.IsTargetable
            && (!isMoving || GetCanFireWhileMoving(weapon))
            && (!isAutoAttack || GetCanAutoAttack(weapon))
            && (isAutoAttack || GetCanTargetAttack(weapon))
            && (GetWeaponAmmoCost(weapon) <= Data.MP)
            && (Data.Team == target.Data.Team == weapon.TargetsAllies())
            && (weapon.AmmoDamage >= 0 || (!target.Data.UnitClass.IncompatibleAmmo && target.Data.UnitClass.MaxMP - target.Data.MP > -weapon.AmmoDamage))
            && (weapon.HealthDamage >= 0 || target.Data.HP < target.Data.UnitClass.MaxHP)
            && (GetWeaponMaxRange(weapon) >= Vector3.Distance(target.transform.position, transform.position))
            && (!weapon.NeedsLineOfSight() || HasLineOfSight(target.TargetingPosition));
    }
    protected bool CanAttackWithWeapon(WeaponModel weapon, Vector3 target, bool isMoving, bool isAutoAttack)
    {
        return weapon != null
            && (!isMoving || weapon.FireWhileMoving)
            && (!isAutoAttack || GetCanAutoAttack(weapon))
            && (isAutoAttack || GetCanTargetAttack(weapon))
            && (GetWeaponAmmoCost(weapon) <= Data.MP)
            && (GetWeaponMaxRange(weapon) >= Vector3.Distance(target, transform.position))
            && (!weapon.NeedsLineOfSight() || HasLineOfSight(target));
    }
    protected virtual WeaponModel GetActiveWeapon(DroneController target, bool isMoving, bool isAutoAttack)
    {
        if (CanAttackWithWeapon(Data.UnitClass.PrimaryWeapon, target, isMoving, isAutoAttack))
        {
            return Data.UnitClass.PrimaryWeapon;
        }
        if (CanAttackWithWeapon(Data.UnitClass.SecondaryWeapon, target, isMoving, isAutoAttack))
        {
            return Data.UnitClass.SecondaryWeapon;
        }

        return null;
    }
    protected virtual WeaponModel GetActiveWeapon(Vector3 target, bool isMoving, bool isAutoAttack)
    {
        if (CanAttackWithWeapon(Data.UnitClass.PrimaryWeapon, target, isMoving, isAutoAttack))
        {
            return Data.UnitClass.PrimaryWeapon;
        }
        if (CanAttackWithWeapon(Data.UnitClass.SecondaryWeapon, target, isMoving, isAutoAttack))
        {
            return Data.UnitClass.SecondaryWeapon;
        }

        return null;
    }
    protected void DoWeaponCooldown(float time)
    {
        Data.UnitClass.PrimaryWeapon.DoCooldown(time);
        Data.UnitClass.SecondaryWeapon?.DoCooldown(time);
        Data.UnitClass.TargetedAbility?.AbilityWeapon?.DoCooldown(time);
    }

    protected DroneController GetAutoAttackTarget(bool autoAttackOnly = true)
    {
        DroneController bestTarget = null;
        float bestDistance = 0;
        var map = GetComponentInParent<MapController>();
        var units = map.Units;
        foreach(var u in units)
        {
            //get active weapon for each unit
            //if active weapon is valid
            //save best distance
            var distance = Vector3.Distance(transform.position, u.transform.position);
            if(bestTarget == null || distance < bestDistance)
            {
                var activeWeapon = GetActiveWeapon(u, IsMoving, autoAttackOnly);
                if(activeWeapon != null)
                {
                    bestTarget = u;
                    bestDistance = distance;
                }
            }
        }
        return bestTarget;
    }
    protected DroneController GetAutoAttackTarget(WeaponModel weapon, DroneController target, bool isMoving, bool autoAttackOnly = true, Quaternion otherRotation = new Quaternion(), float otherArc = 360)
    {
        //if doing autoattack (and not an attack-move or defense), only weapons that can autoattack get targets
        if(weapon == null || (autoAttackOnly && !GetCanAutoAttack(weapon)))
        {
            return null;
        }
        if (target != null)
        {
            if (CanAttackWithWeapon(weapon, target, isMoving, autoAttackOnly))
            {
                return target;
            }
        }
        //get a new target
        DroneController newTarget = null;
        //get the nearest valid target and attack it
        var map = GetComponentInParent<MapController>();
        var units = map.Units;
        float bestDistance = 0;
        foreach (var u in units)
        {
            var distance = Vector3.Distance(u.transform.position, transform.position);
            //prefer closer units
            if ((newTarget == null || distance < bestDistance) && CanAttackWithWeapon(weapon, u, isMoving, autoAttackOnly))
            {
                //get weapon within arc
                float arc = Mathf.Max(weapon.FiringArc, otherArc);
                if (arc < 360)
                {
                    //calculate angle to potential target
                    var flatTarget = new Vector3(u.TargetingPosition.x, transform.position.y, u.TargetingPosition.z);//only look on the y-axis
                    var targetRotation = Quaternion.LookRotation(flatTarget - transform.position);
                    var angle = Quaternion.Angle(targetRotation, otherRotation);
                    if (angle > arc)
                    {
                        continue;//skip this as a valid target
                    }
                }
                bestDistance = distance;
                newTarget = u;
            }
        }
        return newTarget;
    }
    protected void DoLootDrop()
    {
        var loot = Instantiate(DeathLoot, transform.parent);
        loot.transform.position = transform.position + new Vector3(0, .5f, 0);
        loot.ThrowPack();
    }
    protected void DoDeathExplosion()
    {
        DeathExplosion.Instantiate(transform.parent, transform.position);
    }
    protected void UnRegister()
    {
        var map = GetComponentInParent<MapController>();
        if(map != null)
        {
            map.UnRegisterUnit(this);
        }
    }
    protected void Recolor(int team)
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
    protected virtual void UpdateTooltip()
    {
        ToolTip.Header = Data.UnitClass.Name;
        ToolTip.Body = Data.UnitClass.Description;
        var primaryWeapon = Data.UnitClass.PrimaryWeapon;
        var secondaryWeapon = Data.UnitClass.SecondaryWeapon;
        var primaryWeaponInfo = string.Format("Primary Weapon: {0} ({1} damage, {2}/s)", primaryWeapon.Name, primaryWeapon.HealthDamage, 1 / primaryWeapon.Cooldown);
        string secondaryWeaponInfo = "";
        if(secondaryWeapon != null)
        {
            secondaryWeaponInfo = string.Format("Secondary Weapon: {0} ({1} damage, {2}/s)", secondaryWeapon.Name, secondaryWeapon.HealthDamage, 1 / secondaryWeapon.Cooldown);
        }

        ToolTip.Stats = new string[]
        {
            primaryWeaponInfo, secondaryWeaponInfo
        };
        ToolTip.AmmoCost = Data.MP + "/" + Data.UnitClass.MaxMP;
        ToolTip.HealthCost = Data.HP + "/" + Data.UnitClass.MaxHP;
    }
    #endregion

}
