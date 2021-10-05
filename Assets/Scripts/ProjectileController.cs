using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    #region constants
    const float MAX_BEAM_DURATION = 0.5f;//when drawing a beam, this is the maximum duration to draw the beam
    #endregion
    #region public fields
    public LineRenderer Line;
    public BoxCollider Collider;
    #endregion
    #region private fields
    private float distanceTravelled;
    private Vector3 previousLoc;
    private Vector3 firingVector;
    private Vector3 targetPos;
    private float maxBeamDuration;
    private float currentBeamDuration;
    private bool damageDealt;
    private bool trajectoryCalculated;
    private Vector3 initialVelocity;
    #endregion
    #region properties
    public WeaponModel Weapon { get; set; }
    public UnitController Target { get; set; }//target unit
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        targetPos = Target.transform.position;
        //face towards target
        firingVector = (targetPos - transform.position).normalized;
        transform.forward = firingVector;
        maxBeamDuration = Mathf.Min(MAX_BEAM_DURATION, Weapon.Cooldown/2);
        if (Weapon.ArcingAttack)
        {
            initialVelocity = TrajectoryTools.GetInitialVelocity(transform.position, Target.transform.position, TrajectoryTools.MIN_ARC_HEIGHT, -Physics.gravity.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;
        previousLoc = transform.position;

        //travel towards target position
        //distance determined by projectile speed
        if (Weapon.ArcingAttack)
        {
            transform.position += initialVelocity * elapsedTime;
            initialVelocity += Physics.gravity * elapsedTime;

            //projectile will destroy itself when it hits a unit, an obstacle, or the ground
        }
        else if (Weapon.ProjectileSpeed > 0)
        {
            transform.position += firingVector * Weapon.ProjectileSpeed * elapsedTime;
            //check for hit on any unit

            //if hit detected, do damage (if hitting a unit), and destroy the projectile (handled in OnCOllisionEnter)

            //update total distance travelled.
            distanceTravelled += (firingVector * Weapon.ProjectileSpeed * elapsedTime).magnitude;
            //if distance exceeds weapon range, destroy this projectile
            if(distanceTravelled > Weapon.Range)
            {
                Destroy(gameObject);
            }
        }
        //if projectile speed is 0, it's instant, draw a beam to the target
        else
        {
            if(!damageDealt)
            {

                //do weapon damage to first enemy in path to target
                var hits = Physics.RaycastAll(transform.position, firingVector, Vector3.Distance(transform.position, targetPos));
                foreach (var h in hits)
                {
                    var obj = h.collider.GetComponent<UnitController>();
                    if (obj?.Data.Team == Target.Data.Team)//TODO: hit any kind of enemy
                    {
                        Target.Data.HP -= Weapon.Damage;
                        //draw line to first enemy in path to target
                        Line.SetPositions(new Vector3[] { new Vector3(), new Vector3(0, 0, (obj.transform.position - transform.position).magnitude) });
                        damageDealt = true;
                    }
                }
                if (!damageDealt)
                {
                    //draw line to target
                    Line.SetPositions(new Vector3[] { new Vector3(), new Vector3(0, 0, (targetPos - transform.position).magnitude) });
                    Target.Data.HP -= Weapon.Damage;
                    damageDealt = true;
                }
            }
            
            currentBeamDuration += elapsedTime;
            if(currentBeamDuration >= maxBeamDuration)
            {
                //if beam duration exceeded, destroy projectile
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        } 
    }
    private void OnTriggerEnter(Collider other)
    {
        var unit = other.gameObject.GetComponent<UnitController>();
        if (!other.isTrigger && unit == null)
        {
            Destroy(gameObject);
        }
        else if (unit.Data.Team == Target.Data.Team)
        {
            unit.Data.HP -= Weapon.Damage;
            Destroy(gameObject);
        }
    }

    #endregion
    #region public methods

    #endregion
    #region private methods
    //private float GetInitialVelocityForHeight(float desiredHeight)
    //{

    //}
    #endregion
}