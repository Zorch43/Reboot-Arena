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
    private const float PERSONAL_SPACE = 0.02f;
    private const float ORDER_RADIUS = 1f;
    private const float MIN_ATTACK_RANGE = 1f;
    #endregion
    #region public fields
    public UnitMovementController Locomotion;
    public TextMeshPro MinimapNumber;
    public TextMeshPro UnitNumber;
    public SpriteRenderer Selector;
    public WeaponController AbilityWeaponMount;
    public Sprite Portrait;
    public Sprite Symbol;
    public UnitVoiceController UnitVoice;
    
    #endregion
    #region private fields
    private float hitBoxSize;
    private new SphereCollider collider;
    private float zoneMultiplier = 1;
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
            return Locomotion.HasPath;
        }
    }
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
        Locomotion.Speed = Data.UnitClass.MoveSpeed;
        //set agent turn speed
        Locomotion.TurnSpeed = (float)(Data.UnitClass.TurnSpeed * 180 / Math.PI);

        //TEMP: set team from public property
        if(Team >= 0)
        {
            Data.Team = Team;
        }
        collider = GetComponent<SphereCollider>();
        hitBoxSize = collider.radius;
        //set teamcolor
        Recolor(Data.Team);
        MiniMapIcon.color = TeamTools.GetTeamColor(Data.Team);
        //cache drone template
        droneTemplate = Resources.Load<DroneController>(Data.UnitClass.SpecialAbility.DroneTemplate);
        //set death actions
        DeathActions.Add(()=>
        {
            CancelOrders();
        });
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;
        //update selection state
        Selector.gameObject.SetActive(SpawnSlot.IsSelected);
        
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

    #endregion
    #region public methods
    public void SpawnSetup(Vector3 position, int team, UnitSlotModel slot, bool hideUI)
    {
        Data = new UnitModel(UnitClassTemplates.GetClassByName(UnitClass));
        Data.Team = team;
        //disable the agent while manually placing the unit?
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
        var map = GetComponentInParent<MapController>();
        map.RegisterUnit(this);

        DeathActions.Add(SpawnSlot.DoUnitDeath);
        DeathActions.Add(DoLootDrop);
        DeathActions.Add(DoDeathExplosion);
        DeathActions.Add(UnRegister);

        //if unit has a rally point, issue a move order to the rally point
        DoMove(slot.RallyPoint ?? transform.position, false);
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
        if (!IsMoving || Vector3.Distance(location, Locomotion.Destination) > ORDER_RADIUS)
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
        CancelOrders();
        var specialAbility = Data.UnitClass.SpecialAbility;
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
            //TODO: immediately activate ability
        }
    }
    //cancel all orders given to this unit
    public void CancelOrders(bool cancelTracker = true)
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
        //remove action tracker
        if(ActionTracker != null && cancelTracker)
        {
            ActionTracker.CancelAction();
        }
        ActionTracker = null;
    }
    public void DoMove(Vector3 location, bool cancelOrders = false)
    {
        if (!IsMoving || Vector3.Distance(location, Locomotion.Destination) > ORDER_RADIUS)
        {
            if (cancelOrders)
            {
                CancelOrders();
            }
            
            Locomotion.StartPath(location);
        } 
    }
    public override void StopMoving()
    {
        Locomotion.StopPath();
        
        zoneMultiplier = 1;
    }
    #endregion
    #region private methods
    protected override void DoUnitAction()
    {
        bool hasCommandTarget = false;
        if(CommandTarget != null && Data.Team == CommandTarget.Data.Team)
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
