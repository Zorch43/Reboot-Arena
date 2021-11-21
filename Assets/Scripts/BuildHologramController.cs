using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHologramController : MonoBehaviour, IActionTracker
{
    #region constants

    #endregion
    #region public fields
    public GameObject ValidHologram;
    public GameObject InvalidHologram;
    public SphereCollider Collider;
    #endregion
    #region private fields

    #endregion
    #region properties
    public bool IsPlaced { get; set; }
    public bool IsValidPlacement { get; private set; }
    public UnitController Owner { get; set; }
    #endregion
    #region unity methods

    // Update is called once per frame
    void Update()
    {
        //test placement
        if(Time.deltaTime > 0)
        {
            IsValidPlacement = true;
            var hits = Physics.OverlapSphere(transform.position, Collider.radius);
            foreach (var h in hits)
            {
                if (BlocksConstruction(h.gameObject))
                {
                    IsValidPlacement = false;
                    break;
                }
            }
            //if placement is valid, show valid hologram
            //else show invalid hologram
            ValidHologram.gameObject.SetActive(IsValidPlacement);
            InvalidHologram.gameObject.SetActive(!IsValidPlacement);
        }
    }
   
    #endregion
    #region public methods
    public IActionTracker StartAction(UnitController owner)
    {
        BuildHologramController placedHologram = null;
        //if valid:
        if (IsValidPlacement)
        {
            //cancel build of any placed build holograms blocked by new hologram
            var hits = Physics.OverlapSphere(transform.position, Collider.radius);
            foreach(var h in hits)
            {
                if (h != Collider && h.CompareTag("Hologram"))
                {
                    var hologram = h.GetComponent<BuildHologramController>();
                    if(hologram != null)
                    {
                        hologram.CancelAction();
                    }
                }
            }
            //instantiate copy of hologram
            placedHologram = Instantiate(this, transform.parent);
            placedHologram.transform.position = transform.position;
            placedHologram.IsPlaced = true;
            placedHologram.Owner = owner;
        }
        else
        {
            //if invalid: 
            //cancel build mode
            CancelAction();
        }

        return placedHologram;
    }
    public bool FinishAction()
    {
        Destroy(gameObject);
        return IsValidPlacement;
    }
    public void CancelAction()
    {
        if(IsPlaced && Owner != null)
        {
            Owner.CancelOrders(false);
            Owner = null;
        }
        Destroy(gameObject);
    }
    #endregion
    #region private methods
    private bool BlocksConstruction(GameObject obj)
    {
        //constructed drones block construction (units don't)
        //walls block construction (Terrain doesn't)
        //spawn fields block construction
        //objectives block construction
        //pickup spawners block construction (packs don't)
        if(obj.CompareTag("Drone")
            || obj.CompareTag("Wall")
            || obj.CompareTag("SpawnPoint")
            || obj.CompareTag("Objective")
            || obj.CompareTag("Pickup")
            || (IsPlaced && obj.CompareTag("Hologram") && obj != gameObject))
        {
            return true;
        }

        //units don't block construction
        //build holograms don't block construction
        //terrain doesn't block construction
        //bullets/projectiles don't block construction
        //health/ammo packs don't block construction

        return false;
    }
    #endregion
}
