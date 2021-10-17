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
    public Collider Collider;
    public SpecialEffectController ExplosionEffect;
    public ParticleSystem BulletEffect;
    #endregion
    #region private fields
    private float distanceTravelled;
    private Vector3 firingVector;
    private Vector3 targetPos;
    private float maxBeamDuration;
    private float currentBeamDuration;
    private Vector3 initialVelocity;
    private float projectileSizeFactor;
    #endregion
    #region properties
    public WeaponModel Weapon { get; set; }//the weapon that is firing this projectile
    public int AllyTeam { get; set; }//don't damage units of this team
    public Vector3 TargetLocation { get; set; }//location that the weapon is targeting
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        targetPos = TargetLocation; //TODO: apply inaccuracy for arcing attacks
        //face towards target
        firingVector = (targetPos - transform.position).normalized;
        transform.forward = firingVector;
        maxBeamDuration = Mathf.Min(MAX_BEAM_DURATION, Weapon.Cooldown/2);
        if (Weapon.ArcingAttack)
        {
            initialVelocity = TrajectoryTools.GetInitialVelocity(transform.position, targetPos, TrajectoryTools.MIN_ARC_HEIGHT, -Physics.gravity.y);
        }
        if(Weapon.ProjectileSpeed < 0.1)
        {
            FireBeam(firingVector);
        }
        else
        {
            projectileSizeFactor = (Weapon.ProjectileEndSize - Weapon.ProjectileStartSize) / Weapon.MaxRange;
            SetProjectileSize(Weapon.ProjectileStartSize);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.deltaTime;

        //travel towards target position
        //distance determined by projectile speed
        if (Weapon.ArcingAttack)
        {
            var distance3d = initialVelocity * elapsedTime;
            var distance2d = new Vector3(distance3d.x, 0, distance3d.z);

            transform.position += distance3d;
            initialVelocity += Physics.gravity * elapsedTime;

            distanceTravelled += distance2d.magnitude;
            SetProjectileSize();

            //projectile will destroy itself when it hits a unit, an obstacle, or the ground
        }
        else if (Weapon.ProjectileSpeed > 0.1f)
        {
            var distance = firingVector * Weapon.ProjectileSpeed * elapsedTime;
            transform.position += distance;
            
            //update total distance travelled.
            distanceTravelled += distance.magnitude;
            SetProjectileSize();
            //if distance exceeds weapon range, destroy this projectile
            if(distanceTravelled > Weapon.MaxRange)
            {
                DoMaxRangeReached();
            }
        }
        //if projectile speed is 0, it's instant, damage has already been dealt in start(), keep the beam for its duration
        else
        {
            currentBeamDuration += elapsedTime;
            if (currentBeamDuration >= maxBeamDuration)
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
        if (other.tag != "NonBlocking" && !other.isTrigger && unit == null)
        {
            if (!DoImpact(transform.position))
            {
                Destroy(gameObject);
            }
        }
        else if (unit != null && unit.Data.Team != AllyTeam)
        {
            if (!DoImpact(transform.position, unit))
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion
    #region public methods

    #endregion
    #region private methods
    private bool DoImpact(Vector3 impactPosition)
    {
        //projectile has collided with a non-unit

        //if the project creates explosions, explode
        if (Weapon.Explodes)
        {
            DoExplosion(impactPosition);
        }

        //return whether the projectile should continue
        return Weapon.PiercesWalls;
    }
    private bool DoImpact(Vector3 impactPosition, UnitController unit)
    {
        //projectile has collided with a unit

        //if the projectile creates an explosion, explode at location
        if (Weapon.Explodes)
        {
            DoExplosion(impactPosition);
        }
        //otherwise damage the unit
        else
        {
            unit.Data.HP -= Weapon.Damage / Weapon.ProjectileBurstSize;
        }

        //return whether the projectile should continue
        return Weapon.PiercesUnits;
    }
    private void DoMaxRangeReached()
    {
        //if projectile explodes, explode
        if (Weapon.Explodes)
        {
            DoExplosion(firingVector * Weapon.MaxRange);
        }
        //destroy the projectile
        Destroy(gameObject);
    }
    private void DoExplosion(Vector3 position)
    {
        //spherecast at location
        var hits2 = Physics.OverlapSphere(position, Weapon.ExplosionSize);
        //damage all enemy units for the weapon damage amount
        foreach(var h in hits2)
        {
            //var unit = h.collider.GetComponent<UnitController>();
            var unit = h.GetComponent<UnitController>();
            if (unit != null && unit.Data.Team != AllyTeam)
            {
                unit.Data.HP -= Weapon.Damage / Weapon.ProjectileBurstSize;
            }
        }
        //play explosion effect
        var explosion = ExplosionEffect.Instantiate(transform.parent, position);
        explosion.transform.localScale = new Vector3(Weapon.ExplosionSize, Weapon.ExplosionSize, Weapon.ExplosionSize);
    }
    private void FireBeam(Vector3 direction)
    {
        //pulse a (sphere)raycast, getting all colliders in path of beam
        var beamHits = Physics.SphereCastAll(transform.position, Weapon.ProjectileStartSize/2, direction, Weapon.MaxRange);
        Vector3 endPoint = firingVector * Weapon.MaxRange; //if the beam does not collide with anything that can stop it, draw it out to its full length
        //for each object in the path of the beam, determine whether the beam can continue
        //if it can, draw the line out to that collision point
        foreach (var h in beamHits)
        {
            var unit = h.collider.GetComponent<UnitController>();
            if(unit != null && unit.Data.Team != AllyTeam)
            {
                if (!DoImpact(h.point, unit))
                {
                    endPoint = h.point;
                    break;
                }
            }
            else
            {
                if (!DoImpact(h.point))
                {
                    endPoint = h.point;
                    break;
                }
            }
        }
        var beamLength = (endPoint - transform.position).magnitude;
        Line.SetPositions(new Vector3[] { new Vector3(), new Vector3(0, 0, beamLength) });
        var size = Weapon.ProjectileStartSize;
        Line.startWidth = size;
        Line.endWidth = size;
        Collider.enabled = false;
        //TODO: play beam particle effect
        var particleShape = BulletEffect.shape;
        particleShape.radius = beamLength;
        var particleMain = BulletEffect.main;
        particleMain.startSize = 2 * size;
    }
    private void SetProjectileSize()
    {
        var size = Weapon.ProjectileStartSize + distanceTravelled * projectileSizeFactor;
        SetProjectileSize(size);
    }
    private void SetProjectileSize(float size)
    {
        transform.localScale = new Vector3(size, size, size);
        Line.startWidth = size;
        Line.endWidth = size;
        var particleShape = BulletEffect.shape;
        particleShape.radius = 2 * size;
        var particleMain = BulletEffect.main;
        particleMain.startSize = 2 * size;
    }
    #endregion
}
