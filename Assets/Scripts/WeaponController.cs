using Assets.Scripts.Data_Models;
using System.Collections;
using System.Collections.Generic;
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
    GameObject map;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        map = GetComponentInParent<MapController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region public methods
    public void Fire(UnitController target, WeaponModel weapon)
    {
        var projectile = Instantiate(Projectile, map.transform);
        projectile.Target = target;
        projectile.Weapon = weapon;
        projectile.transform.position = Barrel.transform.position;
        //FiringSound.PlayOneShot(FiringSound.clip);
        FiringSound.Play();
    }
    #endregion
    #region private methods

    #endregion
}
