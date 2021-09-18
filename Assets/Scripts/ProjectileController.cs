using Assets.Scripts.Data_Models;
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
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;
        previousLoc = transform.position;

        //travel towards target position
        //distance determined by projectile speed
        if (Weapon.ProjectileSpeed > 0)
        {
            transform.position += firingVector * Weapon.ProjectileSpeed * elapsedTime;
            //check for hit on any unit

            //if hit detected, do damage (if hitting a unit), and destroy the projectile

            //update total distance travelled.
            //if distance exceeds weapon range, destroy this projectile
        }
        //if projectile speed is 0, it's instant, draw a beam to the target
        else
        {
            if(!damageDealt)
            {
                Line.SetPositions(new Vector3[] { new Vector3(), new Vector3(0, 0, (targetPos - transform.position).magnitude) });
                //do weapon damage to target
                Target.Data.HP -= Weapon.Damage;
                damageDealt = true;
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
    #endregion
    #region public methods

    #endregion
    #region private methods

    #endregion
}
