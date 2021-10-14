using Assets.Scripts.Data_Models;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region constants

    #endregion
    #region public fields
    public GameObject Barrel;
    public ProjectileController Projectile;
    public AudioSource FiringSound;
    #endregion
    #region private fields
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region public methods

    public void Fire(UnitController unit, WeaponModel data, GameObject map, Vector3 target)
    {
        for(int i = 0; i < data.ProjectileBurstSize; i++)
        {
            var projectile = Instantiate(Projectile, map.transform);
            projectile.transform.position = Barrel.transform.position;
            projectile.TargetLocation = CalcFiringSolution(data, target, i);
            projectile.Weapon = data;
            projectile.AllyTeam = unit.Data.Team;
        }

        //FiringSound.PlayOneShot(FiringSound.clip);
        if (!FiringSound.isPlaying)
        {
            FiringSound.Play();//TODO: if firing speed is fast, instead loop a clip
        }
        
    }
    #endregion
    #region private methods
    //only makes sense for beams and non-arcing projectiles
    private Vector3 CalcFiringSolution(WeaponModel data, Vector3 perfectTarget, int projectileNumber = 0)
    {
        //get angle deviation from perfect
        float totalDeviation = 0;
        if(data.ProjectileBurstSize > 1 && data.ProjectileBurstSpread > 0.1f)
        {
            //calculate burst deviation, based on burst size, spread, and the projectile number
            totalDeviation += projectileNumber * data.ProjectileBurstSpread / data.ProjectileBurstSize - data.ProjectileBurstSpread / 2;
        }
        if(data.InAccuracy > 0.1f)
        {
            totalDeviation += Random.Range(-data.InAccuracy, data.InAccuracy);
        }
        if(Mathf.Abs(totalDeviation) > 0.1f)
        {
            var firingVector = perfectTarget - Barrel.transform.position;
            firingVector = Quaternion.AngleAxis(totalDeviation, Vector3.up) * firingVector;

            var targetPos = firingVector + Barrel.transform.position;
            return targetPos;
        }
        else
        {
            return perfectTarget;
        }
    }
    #endregion
}
