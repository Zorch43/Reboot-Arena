using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using System.Collections;
using System.Collections.Generic;
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
    public SpriteRenderer TeamColorRenderer;
    public GameObject UnitEffects;
    public SpriteRenderer Selector;
    public NavMeshAgent Agent;
    public NavMeshObstacle Obstacle;
    public ResourceBarController HealthBar;
    public ResourceBarController AmmoBar;
    public WeaponController WeaponMount;
    #endregion
    #region private fields
    private Quaternion initialRotation;
    private bool backingOff;
    private float hitBoxSize = 0.16f;
    private float zoneMultiplier = 1;
    private Vector3 pendingDestination;
    #endregion
    #region properties
    public Vector3 SpacingVector { get; set; }
    public UnitModel Data { get; set; }
    public UnitController CommandTarget { get; set; }
    public UnitController AutoTarget { get; set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //TEMP: initialize data model
        Data = new UnitModel(UnitClassTemplates.GetTrooperClass());
        initialRotation = UnitEffects.transform.rotation;
        //TEMP: randomize health and ammo
        //Data.HP = (int)(Random.value * Data.UnitClass.MaxHP);
        //Data.MP = (int)(Random.value * Data.UnitClass.MaxMP);
        //TEMP: randomize team
        Data.Team = Random.Range(0, 2);
        if(Data.Team > 0)
        {
            TeamColorRenderer.color = new Color(0.5f, 0, 0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;
        var collider = GetComponent<SphereCollider>();
        //update selection state
        Selector.gameObject.SetActive(Data.IsSelected);
        //TODO: manual turning
        if (Agent.hasPath)
        {
            
            collider.radius = hitBoxSize + PERSONAL_SPACE;
            
            //var wayPoint = Agent.nextPosition;

            //var vector = wayPoint - transform.position;

            ////if not also attacking, turn towards the next waypoint
            //float angle = Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg - 90;
            //Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            //UnitAppearance.transform.rotation = Quaternion.Slerp(UnitAppearance.transform.rotation, q, Time.deltaTime * Data.UnitClass.TurnSpeed);
 
        }
        else
        {
            //backingOff = false;
            collider.radius = hitBoxSize;
        }
        DoWeaponCooldown(elapsedTime);
        DoUnitAction();
        //if (!Obstacle.enabled && !Agent.enabled)
        //{
        //    Agent.enabled = true;
        //    Agent.destination = pendingDestination;
        //}
        //TODO; move unit status elements to UI layer
        //keep unit effects on unit (maybe as partical effects?)
        UnitEffects.transform.rotation = initialRotation;//reset rotation of unit effect sprites
        //update resource bars
        HealthBar.UpdateBar(Data.UnitClass.MaxHP, Data.HP);
        AmmoBar.UpdateBar(Data.UnitClass.MaxMP, Data.MP);
        if(Data.HP <= 0)
        {
            Destroy(gameObject);
        }
        ////move towards next waypoint
        //if (Data.IsMoving)
        //{
        //    var vector = Data.WayPoints[0] - (Vector2)transform.position;
        //    var moveVector = vector.normalized * elapsedTime * Data.UnitClass.MoveSpeed;

        //    if(moveVector.magnitude >= vector.magnitude)
        //    {
        //        transform.position = Data.WayPoints[0];
        //        Data.WayPoints.RemoveAt(0);
        //        if(Data.WayPoints.Count < 1)
        //        {
        //            Data.IsMoving = false;
        //        }
        //    }
        //    else
        //    {
        //        transform.position += (Vector3)moveVector;
        //    }
        //    //if not also attacking, turn towards the next waypoint
        //    if (!Data.IsAttacking)
        //    {
        //        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg - 90;
        //        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //        UnitAppearance.transform.rotation = Quaternion.Slerp(UnitAppearance.transform.rotation, q, Time.deltaTime * Data.UnitClass.TurnSpeed);
        //    }
        //}
        //if(SpacingVector.magnitude > 0)
        //{
        //    backingOff = true;
        //    Agent.destination = (transform.position + SpacingVector).normalized * PERSONAL_SPACE;
        //    SpacingVector = new Vector3();
        //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        //whenever this unit collides with another unit,
        //check this unit's destination.
        //var unit = collision.collider.GetComponent<UnitController>();
        //if (Agent.hasPath && unit != null && !unit.Agent.hasPath && collision.collider.bounds.Contains(Agent.destination))
        //{
        //    //if the destination is covered by that (stationary) unit, stop moving.
        //    //Agent.ResetPath();
        //    //back up a little bit so that units are not in constant collision (personal space)
        //    var backOffVector = (transform.position - unit.transform.position).normalized * PERSONAL_SPACE;
            
        //    Agent.SetDestination(transform.position + backOffVector);
        //}

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
    private void OnCollisionExit(Collision collision)
    {
        
    }
    #endregion
    #region public methods
    
    
    #endregion
    #region private methods
    //when right-clicking on a unit
    private void DoUnitAction()
    {
        if(CommandTarget == null)
        {
            //autoattack
            
        }
        if(Data.Team == CommandTarget.Data.Team)
        {
            //perform friendly action (if it exists)
            //else clear unit as target
            CommandTarget = null;
        }
        else
        {
            //perform attack action on unit
            //if can attack (not counting cooldowns), do so
            var activeWeapon = GetActiveWeapon();
            if(activeWeapon != null)
            {
                //turn towards target
                var targetRotation = Quaternion.LookRotation(CommandTarget.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Mathf.Min(Data.UnitClass.TurnSpeed * Time.deltaTime, 1));
                if (Quaternion.Angle(targetRotation, transform.rotation) < 5)
                {
                    //once facing target, fire weapon
                    DoAttack(activeWeapon);
                    Agent.ResetPath();//stop moving //TODO: fire on the move if unit can strafe
                }
                
            }
            else
            {
                //move into position
                Agent.SetDestination(CommandTarget.transform.position);

                //TODO: get line-of-sight
            }
            
        }
    }
    private void DoAttack(WeaponModel activeWeapon)
    {
        if(activeWeapon != null && activeWeapon.IsCooledDown())
        {
            //TODO: spawn attack projectile instead
            //CurrentTarget.Data.HP -= activeWeapon.Damage;//do damage
            WeaponMount.Fire(CommandTarget, activeWeapon);
            activeWeapon.StartCooldown();
            Data.MP -= activeWeapon.AmmoCost;
        }
    }
    private int CanAttackWithWeapon(WeaponModel weapon)
    {
        //reason code:
        //0: can attack as normal
        //1: on cooldown
        //2: out of range
        //3: out of ammo
        int reason = 0;
        if(weapon.AmmoCost > Data.MP)
        {
            reason = 3;
        }
        else if(weapon.Range < Vector3.Distance(transform.position, CommandTarget.transform.position)){
            reason = 2;
        }
        else if (!weapon.IsCooledDown())
        {
            reason = 1;
        }

        return reason;
    }
    private WeaponModel GetActiveWeapon()
    {
        int primaryReason = CanAttackWithWeapon(Data.UnitClass.PrimaryWeapon);
        if(primaryReason == 0 || primaryReason == 1)
        {
            return Data.UnitClass.PrimaryWeapon;
        }

        int secondaryReason = CanAttackWithWeapon(Data.UnitClass.SecondaryWeapon);
        if (secondaryReason == 0 || secondaryReason == 1)
        {
            return Data.UnitClass.SecondaryWeapon;
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
        //get the nearest valid target and attack it
        var map = transform.parent.gameObject;
        var units = map.GetComponentsInChildren<UnitController>();
        UnitController autoTarget = null;
        float bestDistance = 0;
        foreach(var u in units)
        {
            if(u.Data.Team != Data.Team)
            {
                //TODO: make sure that line-of-sight exists
                var distance = Vector3.Distance(u.transform.position, transform.position);
                if(autoTarget == null || distance < bestDistance)
                {
                    bestDistance = distance;
                }
            }
        }
        return target;
    }
    #endregion

}
