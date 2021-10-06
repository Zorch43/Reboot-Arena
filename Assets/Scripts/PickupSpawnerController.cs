using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawnerController : MonoBehaviour
{
    #region constants
    const float MIN_ROTATION_SPEED = -120;//minimum magnitude of the rotation applied to the spawned pickup
    const float MAX_ROTATION_SPEED = 120;//maximum magnitude of the rotation applied to the spawned pickup
    
    #endregion
    #region public fields
    public GameObject Pickup;
    public float RespawnTime;
    public Collider PickupCollider;//detects when a unit tries to grab the pickup;
    public PickupController PickupPack;//what kind of pickup the spawned pickup is
    public int PackCount;//how many packs of that pickup to apply.  packs that can't be applied are scattered.
    #endregion
    #region private fields
    private Vector3 pickupRotation;
    private float respawnTimer;
    #endregion
    #region properties

    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        //generate random rotation (within bounds)
        pickupRotation = new Vector3(
            Random.Range(MIN_ROTATION_SPEED, MAX_ROTATION_SPEED), 
            Random.Range(MIN_ROTATION_SPEED, MAX_ROTATION_SPEED), 
            Random.Range(MIN_ROTATION_SPEED, MAX_ROTATION_SPEED));
    }

    // Update is called once per frame
    void Update()
    {
        //if pickup is spawned, rotate it
        if (Pickup.activeSelf)
        {
            var deltaRotation = pickupRotation * Time.deltaTime;
            Pickup.transform.rotation *= Quaternion.AngleAxis(deltaRotation.x, Vector3.right);
            Pickup.transform.rotation *= Quaternion.AngleAxis(deltaRotation.y, Vector3.up);
            Pickup.transform.rotation *= Quaternion.AngleAxis(deltaRotation.z, Vector3.forward);
        }
        else
        {
            //if pickup is not spawned, tick down the respawn timer
            respawnTimer -= Time.deltaTime;
            if(respawnTimer <= 0)
            {
                respawnTimer = 0;
                //if respawn timer is done, respawn the pickup
                Pickup.SetActive(true);
                PickupCollider.enabled = true;
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the collider belongs to a unit,
        var unit = other.gameObject.GetComponent<UnitController>();
        if(unit != null)
        {
            for(int i = 0; i < PackCount; i++)
            {
                if (PickupPack.CanApplyEffectToUnit(unit))
                {
                    //perform the pickup's effect on that unit
                    PickupPack.ApplyEffectToUnit(unit);
                }
                else
                {
                    //apply as much of the effect as possible, then scatter the remaining pickups
                    var scatterPack = Instantiate(PickupPack, transform.parent);
                    scatterPack.transform.position = Pickup.transform.position;
                    scatterPack.ThrowPack();
                }
            }
            //disable the pickup and the collider
            Pickup.SetActive(false);
            PickupCollider.enabled = false;
            //start the respawn timer
            respawnTimer = RespawnTime;
        }

    }
    #endregion
    #region public methods

    #endregion
    #region private methods
    
    #endregion
}
