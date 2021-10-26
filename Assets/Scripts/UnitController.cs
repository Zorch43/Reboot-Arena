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
    public MeshRenderer[] TeamColorParts;
    
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
    public UnitController AutoTarget { get; set; }
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
            Data = new UnitModel(UnitClassTemplates.GetTrooperClass());
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
        Selector.gameObject.SetActive(Data.IsSelected);
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
                //Agent.ResetPath();
                StopMoving();
                AttackMoveDestination = null;//stop attack-move: destination reached
                zoneMultiplier = 1;
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
        Destroy(gameObject);
    }
    public void SpawnSetup(Vector3 position, int team, UnitSlotModel slot, bool hideUI)
    {
        Data = new UnitModel(UnitClassTemplates.GetTrooperClass());//TEMP: decide on either model-first or gameobject first
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
            return new UnitModel(UnitClassTemplates.GetTrooperClass());
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
    public float ReloadUnit(float amount)
    {
        //resupply the unit by amount, up to max mp
        float oldAmmo = Data.HP;
        Data.MP += amount;
        Data.MP = Mathf.Min(Data.UnitClass.MaxMP, Data.MP);
        //scale and play the reload effect
        var amountLoaded = Data.MP - oldAmmo;
        float effectStrength = amountLoaded / 25;
        ReloadEffect.PlayEffect(effectStrength);
        //return amount loaded
        return amountLoaded;
    }
    //move to the specified location, stopping to attack all enemies encountered on the way.  Cancel if another order is given
    public void DoAttackMove(Vector3 location)
    {
        CancelOrders();//cancel all other orders
        //set the move-attack destination
        AttackMoveDestination = location;
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
    //set the rally point for this unit's slot.  when another unit spawns from this slot, order it to the rally point
    public void SetRallypoint(Vector3 location)
    {
        SpawnSlot.RallyPoint = location;
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
    }
    public bool HasLineOfSight(Vector3 target, bool unitsBlockLOS = false)
    {
        var pos = transform.position;

        var hits = Physics.RaycastAll(pos, target - pos, Vector3.Distance(pos, target));
        foreach (var h in hits)
        {
            var obj = h.collider.GetComponent<UnitController>();
            if (!h.collider.CompareTag("NonBlocking") && !h.collider.isTrigger
                && (obj == null || (unitsBlockLOS && obj.Data.Team != Data.Team)))
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
    #endregion
    #region private methods
    //when right-clicking on a unit
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
            //else clear command attack
            CommandTarget = null;
        }
        else
        {
            hasCommandTarget = DoCommandAttack();
        }

        if (!hasCommandTarget)
        {
            //autoattack
            DoAutoAttack();
        }
    }
    //if the command target is within range, attack it.
    //if command target is outside the primary weapon's range, move to engage
    //applies to both force-attacks and regular command attacks
    private bool DoCommandAttack()
    {
        Vector3? targetPosition = null;

        DoAttackMove();//if unit is set to attack-move, this will choose a command target if available

        if(CommandTarget != null)
        {
            targetPosition = CommandTarget.transform.position;
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
            var activeWeapon = GetActiveWeapon(target, false, false);

            //perform attack action on unit
            //if can attack (not counting cooldowns), do so
            if (activeWeapon != null)
            {

                DoPreAttack(activeWeapon, target, true);
                return true;
            }
            else//move to engage
            {
                //move into position
                //TODO: only do if not in range
                //Agent.SetDestination(target);
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

    private void DoAutoAttack()
    {
        //get weapon to attack with
        var activeWeapon = GetActiveWeapon(AutoTarget, Agent.hasPath, true);
        //check existing autoattack target
        //if invalid, get new target
        if (activeWeapon == null)
        {
            AutoTarget = GetAutoAttackTarget();
            activeWeapon = GetActiveWeapon(AutoTarget, Agent.hasPath, true);
        }
        if(activeWeapon != null)
        {
            DoPreAttack(activeWeapon, AutoTarget.transform.position, false);
            //disable agent turning
            Agent.angularSpeed = 0;
        }
        else
        {
            Agent.angularSpeed = Data.UnitClass.TurnSpeed/Mathf.PI * 180;
        }

    }
    private void DoPreAttack(WeaponModel activeWeapon, Vector3 target, bool stopMoving)
    {
        //turn towards target
        var flatTarget = new Vector3(target.x, transform.position.y, target.z);//only turn on the y-axis
        var targetRotation = Quaternion.LookRotation(flatTarget - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Min(Data.UnitClass.TurnSpeed * Time.deltaTime, 1));
        var remainingAngle = Quaternion.Angle(targetRotation, transform.rotation);
        if (remainingAngle < 5)
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
    private void DoAttack(WeaponModel activeWeapon, Vector3 target)
    {
        if(activeWeapon != null && activeWeapon.IsCooledDown())
        {
            WeaponController weaponMount = PrimaryWeaponMount;
            if(activeWeapon == Data.UnitClass.SecondaryWeapon)
            {
                weaponMount = SecondaryWeaponMount;
            }
            else if(activeWeapon == Data.UnitClass.SpecialAbility.AbilityWeapon)
            {
                weaponMount = AbilityWeaponMount;
            }

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
    private int CanAttackWithWeapon(WeaponModel weapon, Vector3 target, bool isMoving, bool isAutoAttack)
    {
        //reason code:
        //0: can attack as normal
        //1: on cooldown
        //2: no line-of-sight
        //3: out of range
        //4: out of ammo
        //5: can't auto-attack
        //6: can't attack while moving
        int reason = 0;
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
    private WeaponModel GetActiveWeapon(Vector3 target, bool isMoving, bool isAutoAttack)
    {
        if(AbilityTarget != null && Data.UnitClass.SpecialAbility.IsWeaponAbility)
        {
            var abilityWeapon = Data.UnitClass.SpecialAbility.AbilityWeapon;
            int abiltyReason = CanAttackWithWeapon(abilityWeapon, target, isMoving, isAutoAttack);
            if (abiltyReason == 0 || abiltyReason == 1)
            {
                return abilityWeapon;
            }
        }
        else
        {
            int primaryReason = CanAttackWithWeapon(Data.UnitClass.PrimaryWeapon, target, isMoving, isAutoAttack);
            if (primaryReason == 0 || primaryReason == 1)
            {
                return Data.UnitClass.PrimaryWeapon;
            }

            int secondaryReason = CanAttackWithWeapon(Data.UnitClass.SecondaryWeapon, target, isMoving, isAutoAttack);
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
            return GetActiveWeapon(target.transform.position, isMoving, isAutoAttack);
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
            p.material.color = color;
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
            string.Format("Primary Weapon: {0} ({1} damage, {2}/s)", primaryWeapon.Name, primaryWeapon.Damage, 1/primaryWeapon.Cooldown),
            string.Format("Secondary Weapon: {0} ({1} damage, {2}/s)", secondaryWeapon.Name, secondaryWeapon.Damage, 1/secondaryWeapon.Cooldown),
            string.Format("Special Ability: {0}", Data.UnitClass.SpecialAbility.Name)
        };
        ToolTip.AmmoCost = Data.MP + "/" + Data.UnitClass.MaxMP;
        ToolTip.HealthCost = Data.HP + "/" + Data.UnitClass.MaxHP;
    }
    #endregion

}
