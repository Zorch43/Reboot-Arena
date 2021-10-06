using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupController : MonoBehaviour
{
    #region constants
    const float MIN_ARC_HEIGHT = .64f;
    const float MAX_ARC_HEIGHT = 2;
    const float MIN_SCATTER_RADIUS = .32f;
    const float MAX_SCATTER_RADIUS = .96f;
    const float DECAY_TIME = 20;
    #endregion
    #region public fields

    #endregion
    #region private fields
    private bool isMoving;
    private Vector3 velocity;
    private float decayTimer;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        decayTimer = DECAY_TIME;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isMoving)
        {
            float elapsedTime = Time.deltaTime;
            transform.position += velocity * elapsedTime;
            velocity += Physics.gravity * elapsedTime;
        }
        else
        {
            decayTimer -= Time.deltaTime;
            if(decayTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //if the collision is with a unit,
        var unit = other.gameObject.GetComponent<UnitController>();
        if (unit != null)
        {
            //check whether the pickup effect can be applied to that unit
            if (CanApplyEffectToUnit(unit))
            {
                //if it can, apply the effect and consume the pickup
                ApplyEffectToUnit(unit);
                Destroy(gameObject);
            }
        }
        else if (!other.isTrigger)
        {
            isMoving = false;
            velocity = new Vector3();
        }
    }
    #endregion
    #region public methods
    public abstract bool CanApplyEffectToUnit(UnitController unit);
    public abstract void ApplyEffectToUnit(UnitController unit);
    public void ThrowPack()
    {
        velocity = RandomTrajectory();
        isMoving = true;
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
    #endregion
}
