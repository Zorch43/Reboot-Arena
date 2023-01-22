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
    private float maxRange;
    private float projectileSpeed;
    private bool pierceWalls;
    private bool pierceUnits;
    private float healthDamage;
    private float ammoDamage;
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
        //calculate and cache relevant weapon stats
        maxRange = Weapon.Owner.GetWeaponMaxRange(Weapon);
        projectileSpeed = Weapon.Owner.GetWeaponProjectileSpeed(Weapon);
        pierceWalls = Weapon.Owner.GetWeaponPiercesWalls(Weapon);
        pierceUnits = Weapon.Owner.GetWeaponPiercesUnits(Weapon);
        healthDamage = Weapon.Owner.GetWeaponHealthDamage(Weapon);
        ammoDamage = Weapon.Owner.GetWeaponAmmoDamage(Weapon);
        //face towards target
        firingVector = (targetPos - transform.position).normalized;
        transform.forward = firingVector;
        maxBeamDuration = Mathf.Min(MAX_BEAM_DURATION, Weapon.Cooldown/2);
        if (Weapon.ArcingAttack)
        {
            initialVelocity = TrajectoryTools.GetInitialVelocity(transform.position, targetPos, TrajectoryTools.MIN_ARC_HEIGHT, -Physics.gravity.y);
        }
        if(projectileSpeed < 0.1 )
        {
            FireBeam(firingVector);
        }
        else
        {
            projectileSizeFactor = (Weapon.ProjectileEndSize - Weapon.ProjectileStartSize) / maxRange;
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
        else if (projectileSpeed > 0.1f)
        {
            var distance = projectileSpeed * elapsedTime * firingVector;
            transform.position += distance;
            
            //update total distance travelled.
            distanceTravelled += distance.magnitude;
            SetProjectileSize();
            //if distance exceeds weapon range, destroy this projectile
            if(distanceTravelled > maxRange)
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
        if (!other.isTrigger)
        {
            var unit = other.gameObject.GetComponent<DroneController>();
            if (!other.CompareTag("NonBlocking") && unit == null)
            {
                if (!DoImpact(transform.position))
                {
                    Destroy(gameObject);
                }
            }
            else if (unit != null && unit.Data.Team == AllyTeam == Weapon.TargetsAllies())
            {
                if (!DoImpact(transform.position, unit))
                {
                    Destroy(gameObject);
                }
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
        return pierceWalls;
    }
    private bool DoImpact(Vector3 impactPosition, DroneController unit)
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
            unit.DamageUnit(healthDamage / Weapon.ProjectileBurstSize);
            unit.DrainUnit(ammoDamage / Weapon.ProjectileBurstSize);

            if (unit.Data.HP <= 0)
            {
                Weapon.Owner.DoKillEnemy(Weapon.Owner, unit);
            }
        }

        //return whether the projectile should continue
        return pierceUnits;
    }
    private void DoMaxRangeReached()
    {
        //if projectile explodes, explode
        if (Weapon.Explodes)
        {
            DoExplosion(firingVector * maxRange);
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
            //var unit = h.collider.GetComponent<DroneController>();
            var unit = h.GetComponent<DroneController>();
            if (unit != null && unit.Data.Team != AllyTeam)
            {
                unit.DamageUnit(healthDamage / Weapon.ProjectileBurstSize);
                unit.DrainUnit(ammoDamage / Weapon.ProjectileBurstSize);

                if (unit.Data.HP <= 0)
                {
                    Weapon.Owner.DoKillEnemy(Weapon.Owner, unit);
                }
            }
        }
        //play explosion effect
        var explosion = ExplosionEffect.Instantiate(transform.parent, position);
        explosion.transform.localScale = new Vector3(Weapon.ExplosionSize, Weapon.ExplosionSize, Weapon.ExplosionSize);
    }
    private void FireBeam(Vector3 direction)
    {
        //pulse a (sphere)raycast, getting all colliders in path of beam
        var beamHits = Physics.SphereCastAll(transform.position, Weapon.ProjectileStartSize/2, direction, maxRange);
        //TODO: sort hits by distance
        var sortedBeamHits = SortHitsByDistance(beamHits);
        //for each object in the path of the beam, determine whether the beam can continue
        //if it can, draw the line out to that collision point
        var beamLength = maxRange;
        DroneController target = null;
        foreach (var h in sortedBeamHits)
        {
            if (!h.collider.isTrigger)
            {
                var unit = h.collider.GetComponent<DroneController>();
                if (unit == null && !h.collider.gameObject.CompareTag("NonBlocking"))
                {
                    if (!DoImpact(h.point))
                    {
                        beamLength = h.distance;
                        break;
                    }
                }
                else if (unit != null && unit != Weapon.Owner 
                    && unit.Data.Team == AllyTeam == Weapon.TargetsAllies())
                {
                    if (!DoImpact(h.point, unit))
                    {
                        beamLength = h.distance;
                        target = unit;
                        break;
                    }
                }
            }
        }
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
    private List<RaycastHit> SortHitsByDistance(RaycastHit[] allHits)
    {
        var list = new List<RaycastHit>(allHits);
        list.Sort(CompareHitsByDistance);

        return list;
    }
    private int CompareHitsByDistance(RaycastHit a, RaycastHit b)
    {
        if(a.distance < b.distance)
        {
            return -1;
        }
        else if(a.distance > b.distance)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    #endregion
}
