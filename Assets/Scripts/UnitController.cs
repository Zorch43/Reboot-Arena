using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    #region constants
    private const float PERSONAL_SPACE = 0.02f;
    private const float ORDER_RADIUS = 0.32f;
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
    public WeaponController WeaponMount;
    public int Team = -1;//TEMP
    public Sprite Portrait;
    public Sprite Symbol;
    public PickupController DeathLoot;
    public MeshRenderer[] TeamColorParts;
    public ParticleSystem JetStream;
    #endregion
    #region private fields
    private Quaternion initialRotation;
    private float hitBoxSize = 0.16f;
    private float zoneMultiplier = 1;
    #endregion
    #region properties
    public Vector3 SpacingVector { get; set; }
    public UnitModel Data { get; set; }
    public UnitController CommandTarget { get; set; }
    public UnitController AutoTarget { get; set; }
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

        //TEMP: set teamcolor
        Recolor(Data.Team);
        MiniMapIcon.color = TeamTools.GetTeamColor(Data.Team);
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;
        var collider = GetComponent<SphereCollider>();
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

        //TODO; move unit status elements to UI layer
        //keep unit effects on unit (maybe as partical effects?)
        //UnitEffects.transform.rotation = initialRotation;//reset rotation of unit effect sprites
        UnitEffects.transform.forward = transform.position - Camera.main.transform.position;//orient unit UI towards camera
        MiniMapIcon.transform.rotation = initialRotation;//reset rotation of minimap icon
        //update resource bars
        HealthBar.UpdateBar(Data.UnitClass.MaxHP, Data.HP);
        AmmoBar.UpdateBar(Data.UnitClass.MaxMP, Data.MP);
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
                Agent.ResetPath();
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
    }
    
    #endregion
    #region private methods
    //when right-clicking on a unit
    private void DoUnitAction()
    {
        bool hasCommandTarget = false;
        if(CommandTarget == null)
        {
            //autoattack
            
        }
        else if(Data.Team == CommandTarget.Data.Team)
        {
            //perform support action (if it exists)
            //else clear unit as target
            CommandTarget = null;
        }
        else
        {
           hasCommandTarget =  DoCommandAttack();
        }

        if (!hasCommandTarget)
        {
            //autoattack
            DoAutoAttack();
        }
    }
    //if the command target is within range, attack it.
    //if command target is outside the primary weapon's range, move to engage
    private bool DoCommandAttack()
    {
        //perform attack action on unit
        //if can attack (not counting cooldowns), do so
        var activeWeapon = GetActiveWeapon(CommandTarget, false, false);
        if (activeWeapon != null)
        {
            
            //turn towards target
            var targetRotation = Quaternion.LookRotation(CommandTarget.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Min(Data.UnitClass.TurnSpeed * Time.deltaTime, 1));
            if (Quaternion.Angle(targetRotation, transform.rotation) < 5)
            {
                //once facing target, fire weapon
                DoAttack(activeWeapon, CommandTarget);
                Agent.ResetPath();//stop moving
            }
            return true;
        }
        else//move to engage
        {
            //move into position
            Agent.SetDestination(CommandTarget.transform.position);
            return false;
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
            //turn towards target
            var targetRotation = Quaternion.LookRotation(AutoTarget.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Min(Data.UnitClass.TurnSpeed * Time.deltaTime, 1));
            if (Quaternion.Angle(targetRotation, transform.rotation) < 5)
            {
                //once facing target, fire weapon
                DoAttack(activeWeapon, AutoTarget);
            }
            //disable agent turning
            Agent.angularSpeed = 0;
        }
        else
        {
            Agent.angularSpeed = Data.UnitClass.TurnSpeed/Mathf.PI * 180;
        }

    }
    private void DoAttack(WeaponModel activeWeapon, UnitController target)
    {
        if(activeWeapon != null && activeWeapon.IsCooledDown())
        {
            //if (activeWeapon == Data.UnitClass.SecondaryWeapon)
            //{
            //    //DEBUG: get line of sight to see what's blocking the shot
            //    var hits = Physics.RaycastAll(new Ray(transform.position, CommandTarget.transform.position));
            //    string debugString = "Obstacles: ";
            //    foreach (var h in hits)
            //    {
            //        debugString += "[" + h.collider.tag + "]";
            //    }
            //    Debug.Log(debugString);
            //}
            WeaponMount.Fire(target, activeWeapon);
            activeWeapon.StartCooldown();
            Data.MP -= activeWeapon.AmmoCost;
            //Debug.Log(string.Format("{0} attacks {1} with {2} at range {3} (max range {4})",
            //    gameObject.name,target.gameObject.name,activeWeapon.Name, 
            //    Vector3.Distance(transform.position, target.transform.position), activeWeapon.Range));
        }
    }
    private int CanAttackWithWeapon(WeaponModel weapon, UnitController target, bool isMoving, bool isAutoAttack)
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
        else if(weapon.Range < Vector3.Distance(transform.position, target.transform.position)){
            reason = 3;
        }
        else if(weapon.NeedsLineOfSight() && !HasLineOfSight(target.transform.position))
        {
            reason = 2;
        }
        else if (!weapon.IsCooledDown())
        {
            reason = 1;
        }

        return reason;
    }
    private WeaponModel GetActiveWeapon(UnitController target, bool isMoving, bool isAutoAttack)
    {
        if(target != null)
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
        return null; ;
    }
    private void DoWeaponCooldown(float time)
    {
        Data.UnitClass.PrimaryWeapon.DoCooldown(time);
        Data.UnitClass.SecondaryWeapon.DoCooldown(time);
    }

    private UnitController GetAutoAttackTarget()
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
                    if ((!Data.UnitClass.PrimaryWeapon.NeedsLineOfSight() && Data.UnitClass.PrimaryWeapon.CanAutoAttack)
                        || (!Data.UnitClass.SecondaryWeapon.NeedsLineOfSight() && Data.UnitClass.SecondaryWeapon.CanAutoAttack)
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
    private bool HasLineOfSight(Vector3 target)
    {
        var pos = transform.position;

        var hits = Physics.RaycastAll(pos, target - pos, Vector3.Distance(pos, target));
        foreach (var h in hits)
        {
            var obj = h.collider.GetComponent<UnitController>();
            if (!h.collider.CompareTag("NonBlocking") && !h.collider.isTrigger && obj == null)
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
    private void DoLootDrop()
    {
        var loot = Instantiate(DeathLoot, transform.parent);
        loot.transform.position = transform.position + new Vector3(0, .32f, 0);
        loot.ThrowPack();
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
    }
    #endregion

}
