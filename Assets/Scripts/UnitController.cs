using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : DroneController
{
    #region constants
    private const float MIN_ATTACK_RANGE = 1f;
    #endregion
    #region public fields
    public MovementController Locomotion;
    public TextMeshPro MinimapNumber;
    public TextMeshPro UnitNumber;
    public SpriteRenderer Selector;
    public WeaponController AbilityWeaponMount;
    public Sprite Portrait;
    public Sprite Symbol;
    public UnitVoiceController UnitVoice;
    public ParticleSystem BoostEffect;
    #endregion
    #region private fields
    private float hitBoxSize;
    private new SphereCollider collider;
    private DroneController droneTemplate;
    #endregion
    #region properties
    public DroneController CommandTarget { get; set; }
    public Vector3? ForceTarget { get; set; }
    public Vector3? AbilityTarget { get; set; }
    public Vector3? AttackMoveDestination { get; set; }
    public IActionTracker ActionTracker { get; set; }
    public UnitSlotModel SpawnSlot { get; set; }
    public override bool IsMoving
    {
        get
        {
            return Locomotion.IsMoving();
        }
    }
    #endregion
    #region unity methods
    protected override void OnStart()
    {
        //base
        base.OnStart();
        //set agent speed
        Locomotion.Speed = Data.UnitClass.MoveSpeed;
        //set agent turn speed
        Locomotion.TurnSpeed = (float)(Data.UnitClass.TurnSpeed * 180 / Math.PI);

        collider = GetComponent<SphereCollider>();
        hitBoxSize = collider.radius;

        //cache drone template
        droneTemplate = Resources.Load<DroneController>(Data.UnitClass.SpecialAbility.DroneTemplate);

        //set movement graph
        if (Data.UnitClass.HasJumpBoost)
        {
            Locomotion.CanJump = true;
        }
        //set death actions
        DeathActions.Add(() =>
        {
            CancelOrders();
        });
    }
    protected override void OnUpdate()
    {
        //base
        base.OnUpdate();
        float deltaTime = Time.deltaTime;
        //update selection state
        Selector.gameObject.SetActive(SpawnSlot.IsSelected);
        //update boosted movement speed
        if(Data.MP > 0)
        {
            Locomotion.Speed = Data.UnitClass.MoveSpeed + Data.UnitClass.SpeedBoostPower;
        }
        else
        {
            Locomotion.Speed = Data.UnitClass.MoveSpeed;
        }
        //consume fuel
        bool isMoving = IsMoving;
        if (isMoving)
        {
            DrainUnit(deltaTime * Data.UnitClass.FuelConsumption);
            
        }
        //start/stop boost effect
        if(isMoving && Data.UnitClass.SpeedBoostPower > 0.1f && Data.MP > 0)
        {
            BoostEffect.Play();
        }
        else
        {
            BoostEffect.Stop();
        }
    }
    #endregion
    #region public methods
    public void SpawnSetup(Vector3 position, int team, UnitSlotModel slot, bool hideUI)
    {
        Data = new UnitModel(UnitClassTemplates.GetClassByName(UnitClass));
        Data.Team = team;
        //disable the agent while manually placing the unit?
        transform.position = new Vector3(position.x, 0, position.z);//TODO: figure out what's floating units on spawn
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
        var map = GetComponentInParent<MapController>();
        map.RegisterUnit(this);

        DeathActions.Add(SpawnSlot.DoUnitDeath);
        DeathActions.Add(DoLootDrop);
        DeathActions.Add(DoDeathExplosion);
        DeathActions.Add(UnRegister);
        //if unit has a rally point, issue a move order to the rally point
        if (slot.RallyPoint != null)
        {
            DoMove(slot.RallyPoint ?? transform.position, false);
        }
        else
        {
            Locomotion.Stop();
        }
        
    }
    public void DoAttack(DroneController target)
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
        if (!IsMoving)
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
    public void DoSpecialAbility(Vector3 location, IActionTracker actionTracker = null)
    {
        var specialAbility = Data.UnitClass.SpecialAbility;
        if (!specialAbility.IsNonInterrupting)
        {
            CancelOrders();
        }
        
        //if ability is targeted, activate ability at location
        if (specialAbility.IsTargetedAbility)
        {
            if(actionTracker != null)
            {
                ActionTracker = actionTracker.StartAction(this);
                if (ActionTracker != null)
                {
                    AbilityTarget = location;
                }
            }
            else
            {
                AbilityTarget = location;
            }
        }
        else
        {
            //pay cost now
            if(Data.MP >= specialAbility.AmmoCostInstant)
            {
                DrainUnit(specialAbility.AmmoCostInstant);
                //do non-targeted parts of the ability
                //self heal
                HealUnit(specialAbility.SelfHeal);
                //drop loot
                if (specialAbility.LootDrop.Length > 0)
                {
                    foreach (var p in specialAbility.LootDrop)
                    {
                        var loot = Instantiate(ResourceList.GetPickupTemplate(p));
                        loot.transform.position = transform.position + new Vector3(0, 1.1f, 0);
                        loot.ThrowPack();
                    }
                }
            }
            
        }
        
    }
    //cancel all orders given to this unit
    public void CancelOrders(bool cancelTracker = true)
    {
        //stop unit
        StopMoving();
        //remove attack-move destination
        AttackMoveDestination = null;
        //stop command attack
        CommandTarget = null;
        //stop force attack
        ForceTarget = null;
        //stop targeting ability
        AbilityTarget = null;
        //remove action tracker
        if(ActionTracker != null && cancelTracker)
        {
            ActionTracker.CancelAction();
        }
        ActionTracker = null;
    }
    public void DoMove(Vector3 location, bool cancelOrders = false)
    {
        if (cancelOrders)
        {
            CancelOrders();
        }
        Locomotion.StartPath(location);
    }
    public override void StopMoving()
    {
        Locomotion.Stop();
    }
    #endregion
    #region private methods
    protected override void DoUnitAction()
    {
        bool hasCommandTarget = false;
        if(CommandTarget != null && Data.Team == CommandTarget.Data.Team)
        {
            //perform support action (if it exists)
            if (Data.UnitClass.PrimaryWeapon.TargetsAllies() 
                || (Data.UnitClass.SecondaryWeapon != null &&  Data.UnitClass.SecondaryWeapon.TargetsAllies()))
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
            base.DoUnitAction();
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
            targetPosition = CommandTarget.TargetingPosition;
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

            WeaponModel activeWeapon = null;
            if(CommandTarget != null)
            {
                activeWeapon = GetActiveWeapon(CommandTarget, false, false);
            }
            else
            {
                activeWeapon = GetActiveWeapon(target, false, false);
            }

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
                    var flatTarget = new Vector3(target.x, transform.position.y, target.z);//only look on the y-axis
                    var commandRotation = Quaternion.LookRotation(flatTarget - transform.position);
                    //get other weapon
                    if (activeWeapon == Data.UnitClass.SecondaryWeapon)
                    {
                        var otherWeapon = Data.UnitClass.PrimaryWeapon;
                        AutoTarget1 = GetAutoAttackTarget(otherWeapon, AutoTarget1, IsMoving, true, commandRotation, activeWeapon.FiringArc);
                        if (AutoTarget1 != null)
                        {
                            DoPreAttack(otherWeapon, AutoTarget1.TargetingPosition, true, false);
                        }
                    }
                    else
                    {
                        var otherWeapon = Data.UnitClass.PrimaryWeapon;
                        AutoTarget2 = GetAutoAttackTarget(otherWeapon, AutoTarget2, IsMoving, true, commandRotation, activeWeapon.FiringArc);
                        if (AutoTarget2 != null)
                        {
                            DoPreAttack(otherWeapon, AutoTarget2.TargetingPosition, true, false);
                        }
                    }
                }
                return true;
            }
            else//move to engage
            {
                var flatDistance = Vector2.Distance(new Vector2(target.x, target.z), new Vector2(transform.position.x, transform.position.z));
                if(flatDistance  > MIN_ATTACK_RANGE)
                {
                    DoMove(target, false);
                }
                else
                {
                    StopMoving();
                }
                
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
    protected override bool DoAutoAttacks()
    {
        bool hasTarget = base.DoAutoAttacks();
        if (hasTarget)
        {
            //disable agent turning
            Locomotion.CanTurn = false;
        }
        else
        {
            //else re-enable agent steering
            Locomotion.CanTurn = true;
        }
        return hasTarget;
    }
    protected override bool DoAutoAttack()
    {
        bool hasTarget = base.DoAutoAttack();
        if (hasTarget)
        {
            //disable agent turning
            Locomotion.CanTurn = false;
        }
        else
        {
            //else re-enable agent steering
            Locomotion.CanTurn = true;
        }
        return hasTarget;
    }
    
    protected override WeaponController GetWeaponController(WeaponModel activeWeapon)
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
    protected override void DoAttack(WeaponModel activeWeapon, Vector3 target)
    {
        if(activeWeapon != null && activeWeapon.IsCooledDown())
        {
            var weaponMount = GetWeaponController(activeWeapon);

            bool success = true;
            var specialAbility = Data.UnitClass.SpecialAbility;
            if (activeWeapon == specialAbility.AbilityWeapon)
            {
                if (specialAbility.IsBuildAbility)
                {
                    if((ActionTracker == null && BuildTools.IsValidBuildSite(target, 0.5f)) 
                        || (ActionTracker != null && ActionTracker.FinishAction()))
                    {
                        //get drone template and spawn it at the ability target
                        var drone = Instantiate(droneTemplate, transform.parent);
                        drone.SpawnSetup(target, Data.Team, false);
                    }
                    else
                    {
                        success = false;
                    }
                }
                if (!specialAbility.IsContinuous)
                {
                    CancelOrders();
                }
            }
            if (success)
            {
                weaponMount.Fire(this, activeWeapon, transform.parent.gameObject, target);
                activeWeapon.StartCooldown();
                Data.MP -= activeWeapon.AmmoCost;
            }
        }
    }
    protected override WeaponModel GetActiveWeapon(DroneController target, bool isMoving, bool isAutoAttack)
    {
        if (AbilityTarget != null && Data.UnitClass.SpecialAbility.IsWeaponAbility)
        {
            var abilityWeapon = Data.UnitClass.SpecialAbility.AbilityWeapon;
            if (CanAttackWithWeapon(abilityWeapon, target, isMoving, isAutoAttack))
            {
                return abilityWeapon;
            }
        }
        else
        {
            return base.GetActiveWeapon(target, isMoving, isAutoAttack);
        }

        return null;
    }
    protected override WeaponModel GetActiveWeapon(Vector3 target, bool isMoving, bool isAutoAttack)
    {
        if (AbilityTarget != null && Data.UnitClass.SpecialAbility.IsWeaponAbility)
        {
            var abilityWeapon = Data.UnitClass.SpecialAbility.AbilityWeapon;
            if (CanAttackWithWeapon(abilityWeapon, target, isMoving, isAutoAttack))
            {
                return abilityWeapon;
            }
        }
        else
        {
            return base.GetActiveWeapon(target, isMoving, isAutoAttack);
        }

        return null;
    }
    protected override void UpdateTooltip()
    {
        base.UpdateTooltip();
        var stats = new List<string>(ToolTip.Stats);
        stats.Add(string.Format("Special Ability: {0}", Data.UnitClass.SpecialAbility.Name));
        ToolTip.Stats = stats.ToArray();
    }
    #endregion

}
