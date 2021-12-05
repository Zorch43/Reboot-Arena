using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDroneController : DroneController
{
    #region constants
    const float MIN_COLLAPSE_TIME = 1f;
    const string PARAM_COLLAPSE = "IsCollapsed";
    const string PARAM_BUILD = "BuildPoints";
    #endregion
    #region public fields
    public float InitialHP;
    public float InitialMP;
    #endregion
    #region private fields
    private CapsuleCollider collider;
    private int blockingUnits;
    private bool isCollapsed;
    private float collapseTime;
    #endregion
    #region properties
    public float BuildPoints { get; set; }
    #endregion
    #region unity methods
    protected override void OnStart()
    {
        base.OnStart();
        collider = GetComponent<CapsuleCollider>();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        float deltaTime = Time.deltaTime;

        //if collapsed, check if there are still units colliding with the turret's collider
        if(isCollapsed)
        {
            collapseTime += deltaTime;
            if(collapseTime > MIN_COLLAPSE_TIME && blockingUnits == 0)
            {
                //if not, expand
                Expand();
                collapseTime = 0;
            }
        }
        blockingUnits = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //if a friendly unit with a destination collides with this, collapse
        var unit = collision.collider.GetComponent<UnitController>();
        if(unit != null && unit.Data.Team == Data.Team && unit.IsMoving)
        {
            Collapse();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //detect units preventing the turret from expanding
        var unit = other.GetComponent<UnitController>();
        if(unit != null)
        {
            blockingUnits++;
        }
    }
    #endregion
    #region public methods
    public override float ReloadUnit(float amount)
    {
        var points = base.ReloadUnit(amount);
        BuildPoints += points;
        Animations.SetFloat(PARAM_BUILD, BuildPoints);

        return points;
    }
    public override void SpawnSetup(Vector3 position, int team, bool hideUI)
    {
        base.SpawnSetup(position, team, hideUI);
        Data.HP = InitialHP;
        Data.MP = InitialMP;
    }
    public void Collapse()
    {
        //set animation state
        Animations.SetBool(PARAM_COLLAPSE, true);
        //set collider as trigger
        collider.isTrigger = true;
        isCollapsed = true;
        Data.IsDamageable = false;
        Data.IsTargetable = false;
    }
    public void Expand()
    {
        //set animation state
        Animations.SetBool(PARAM_COLLAPSE, false);
        //set collider back to normal
        collider.isTrigger = false;
        isCollapsed = false;
        Data.IsDamageable = true;
        Data.IsTargetable = true;
    }
    #endregion
    #region private methods

    #endregion
}
