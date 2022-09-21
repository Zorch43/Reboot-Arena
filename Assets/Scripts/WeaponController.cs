using Assets.Scripts.Data_Models;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject[] Mounts;
    public GameObject[] Barrels;
    public ProjectileController Projectile;
    public AudioSource FiringSound;
    public ParticleSystem MuzzleFlashEffect;
    #endregion
    #region private fields
    private int currentBarrel;
    #endregion
    #region properties

    #endregion
    #region unity methods

    #endregion
    #region public methods

    public void Fire(DroneController unit, WeaponModel data, GameObject map, Vector3 target)
    {
        for(int i = 0; i < data.ProjectileBurstSize; i++)
        {
            var projectile = Instantiate(Projectile, map.transform);
            projectile.transform.position = Barrels[currentBarrel].transform.position;
            projectile.TargetLocation = CalcFiringSolution(data, target, i);
            projectile.Weapon = data;
            projectile.AllyTeam = unit.Data.Team;
            currentBarrel++;
            if(currentBarrel >= Barrels.Length)
            {
                currentBarrel = 0;
            }
        }

        FiringSound.Play();//TODO: if firing speed is fast, instead loop a clip

        MuzzleFlashEffect.transform.position = Barrels[currentBarrel].transform.position;
        MuzzleFlashEffect.Play();
    }
    public bool TraverseMounts(WeaponModel weapon, Vector3 target, Quaternion facing)
    {
        bool ready = false;
        //rotate mounts to face the target.
        foreach(var m in Mounts)
        {
            //turn towards target
            var flatTarget = new Vector3(target.x, m.transform.position.y, target.z);//only turn on the y-axis
            var targetRotation = Quaternion.LookRotation(flatTarget - transform.position);
            
            var currentRotation = Quaternion.Lerp(m.transform.rotation, targetRotation, Mathf.Min(weapon.TraversalSpeed * Time.deltaTime, 1));
            //contrain to arc - easy version: prevent turret from traversing if at traversal bounds
            var traverseAngle = Quaternion.Angle(currentRotation, facing);
            if(Mathf.Abs(traverseAngle) <  weapon.FiringArc/ 2)
            {
                m.transform.rotation = currentRotation;
            }
            //else
            //{
            //    //return true if cannot traverse anymore
            //    ready = true;
            //}
            var remainingAngle = Quaternion.Angle(targetRotation, m.transform.rotation);

            //return true if facing the target
            if (remainingAngle < 2)
            {
                ready = true;
            }
        }

        //return false if not facing the target
        return ready;

    }
    #endregion
    #region private methods
    //only makes sense for beams and non-arcing projectiles
    private Vector3 CalcFiringSolution(WeaponModel weapon, Vector3 perfectTarget, int projectileNumber = 0)
    {
        float inaccuracy = weapon.Owner.GetWeaponInaccuracy(weapon);
        //get angle deviation from perfect
        float totalDeviation = 0;
        if(weapon.ProjectileBurstSize > 1 && weapon.ProjectileBurstSpread > 0.1f)
        {
            //calculate burst deviation, based on burst size, spread, and the projectile number
            totalDeviation += projectileNumber * weapon.ProjectileBurstSpread / weapon.ProjectileBurstSize - weapon.ProjectileBurstSpread / 2;
        }
        if(inaccuracy > 0.1f)
        {
            totalDeviation += Random.Range(-inaccuracy, inaccuracy);
        }
        if(Mathf.Abs(totalDeviation) > 0.1f)
        {
            var firingVector = perfectTarget - Barrels[currentBarrel].transform.position;
            firingVector = Quaternion.AngleAxis(totalDeviation, Vector3.up) * firingVector;

            var targetPos = firingVector + Barrels[currentBarrel].transform.position;
            return targetPos;
        }
        else
        {
            return perfectTarget;
        }
    }
    #endregion
}
