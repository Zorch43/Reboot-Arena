using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupController : MonoBehaviour
{
    #region constants
    const float MIN_ARC_HEIGHT = 0.5f;
    const float MAX_ARC_HEIGHT = 2f;
    const float MIN_SCATTER_RADIUS = 0.5f;
    const float MAX_SCATTER_RADIUS = 2f;
    const float DECAY_TIME = 20;
    const float RE_COLLIDE_TIMER = 0.25f;
    const float STOP_TIME = 0.25f;
    public enum PickupType
    {
        NanoPack,
        AmmoPack
    }
    #endregion
    #region public fields
    public SoundPointController SoundPointTemplate;
    public AudioClip PickupSound;
    #endregion
    #region private fields
    private float decayTimer;
    private float stopTimer;
    private float lastCollision = 0;
    private BoxCollider boxCollider;
    private Rigidbody body;
    private List<Collider> ignoredColliders = new List<Collider>();
    private float maxVelocity;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        decayTimer = DECAY_TIME;
        boxCollider = GetComponent<BoxCollider>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        decayTimer -= deltaTime;
        if (decayTimer <= 0)
        {
            Destroy(gameObject);
        }
        if (!body.isKinematic)
        {
            var currentVelocity = body.velocity.magnitude;
            if(currentVelocity > maxVelocity)
            {
                body.velocity = body.velocity.normalized * maxVelocity;
            }
            if (currentVelocity < 0.001f)
            {
                stopTimer += deltaTime;
                if (stopTimer > STOP_TIME)
                {
                    body.isKinematic = true;
                    boxCollider.isTrigger = true;
                }
            }
        }
        
        ////re-enable collisions
        //lastCollision += deltaTime;
        //if (lastCollision >= RE_COLLIDE_TIMER)
        //{
        //    foreach (var c in ignoredColliders)//reset ignored colliders
        //    {
        //        if (c != null)
        //        {
        //            Physics.IgnoreCollision(c, boxCollider, false);
        //        }
        //    }
        //    ignoredColliders.Clear();
        //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerOrCollisionEnter(collision.collider);
    }
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerOrCollisionEnter(other);
    }
    #endregion
    #region public methods
    public abstract bool CanApplyEffectToUnit(UnitController unit);
    public abstract void ApplyEffectToUnit(UnitController unit);
    public void ThrowPack()
    {
        var velocity = RandomTrajectory();
        maxVelocity = velocity.magnitude;
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(velocity, ForceMode.VelocityChange);

    }
    #endregion
    #region private methods
    private Vector3 RandomTrajectory()
    {
        var angle = Random.Range(1, 360);
        var angleVector = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        var targetPositon = angleVector * Random.Range(MIN_SCATTER_RADIUS, MAX_SCATTER_RADIUS) + transform.position;
        var velocity = TrajectoryTools.GetInitialVelocity(transform.position, targetPositon,
                        Random.Range(MIN_ARC_HEIGHT, MAX_ARC_HEIGHT), -Physics.gravity.y);
        return velocity;
    }
    private void OnTriggerOrCollisionEnter(Collider collider)
    {
        var unit = collider.GetComponent<UnitController>();
        if (unit != null)
        {

            //check whether the pickup effect can be applied to that unit
            if (CanApplyEffectToUnit(unit))
            {
                //if it can, apply the effect and consume the pickup
                ApplyEffectToUnit(unit);
                //deploy consumption sound to map
                SoundPointTemplate.Instantiate(PickupSound, transform.parent, transform.position);
                //remove pickup
                Destroy(gameObject);
            }
            //else
            //{
            //    lastCollision = 0;
            //    Physics.IgnoreCollision(collider, boxCollider);
            //    ignoredColliders.Add(collider);
            //}
        }
    }
    #endregion
}
