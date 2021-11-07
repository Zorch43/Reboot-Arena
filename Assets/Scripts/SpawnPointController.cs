using Assets.Scripts.Data_Models;
using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointController : MonoBehaviour
{
    #region constants
    private const float FREE_SPACE_RADUS = 0.5f;
    private const float SEARCH_GRID_SIZE = 0.125f;
    #endregion
    #region public fields
    public SpriteRenderer TeamColor;
    public SpriteRenderer MinimapMarker;
    public ParticleSystem SpawnFieldSfx;
    public BoxCollider RespawnArea;
    #endregion
    #region private fields
    
    private int _team;
    #endregion
    #region properties
    public int ControllingTeam
    {
        get
        {
            return _team;
        }
        set
        {
            _team = value;
            TeamColor.color = TeamTools.GetTeamColor(_team);
            MinimapMarker.color = TeamColor.color;
            var particleMain = SpawnFieldSfx.main;
            particleMain.startColor = TeamColor.color;
            if (_team == -1)
            {
                MinimapMarker.gameObject.SetActive(false);
                RespawnArea.gameObject.SetActive(false);
            }
            else
            {
                MinimapMarker.gameObject.SetActive(true);
                RespawnArea.gameObject.SetActive(true);
            }
        }
    }
    #endregion
    #region unity methods
    
    #endregion
    #region public methods
    public UnitController SpawnUnit(UnitSlotModel unitSlot, bool hideUI)
    {
        //instantiate the unit on the map in an empty space in the respawn zone

        //starting from the center of the spawn field, search for an empty space (that is also within bounds of the spawn zone)
        Vector3 testPoint = RespawnArea.bounds.center;
        Vector3 nextNeighbor = new Vector3(0, 0, 1);
        
        while (true)
        {
            //test testPoint
            //var ray = new Ray(testPoint + new Vector3(0, 4), Vector3.down);
            if (RespawnArea.bounds.Contains(testPoint))
            {
                var hits = Physics.SphereCastAll(testPoint, FREE_SPACE_RADUS, Vector3.down);
                bool open = true;
                foreach (var h in hits)
                {
                    if (!h.collider.isTrigger && h.collider.gameObject.name != "Terrain")
                    {
                        open = false;
                        break;
                    }
                }
                if (open)
                {
                    //spawn unit and stop looking for an open spot
                    var spawnedUnit = Instantiate(unitSlot.NextUnitClass, transform.parent);
                    spawnedUnit.SpawnSetup(testPoint, _team, unitSlot, hideUI);

                    return spawnedUnit;
                }
            }
            
            // update testpoint
            testPoint = RespawnArea.bounds.center + nextNeighbor * SEARCH_GRID_SIZE;
            //update nextNeightbor
            if (nextNeighbor.x >= 0 && nextNeighbor.z > 0)
            {
                nextNeighbor += new Vector3(1, 0, -1);

            }
            else if (nextNeighbor.x > 0 && nextNeighbor.z <= 0)
            {
                nextNeighbor += new Vector3(-1, 0, -1);
            }
            else if (nextNeighbor.x <= 0 && nextNeighbor.z < 0)
            {
                nextNeighbor += new Vector3(-1, 0, 1);
            }
            else if(nextNeighbor.x < 0 && nextNeighbor.z >= 0)
            {
                nextNeighbor += new Vector3(1, 0, 1);
                if (nextNeighbor.x == 0)
                {
                    //spiral out
                    nextNeighbor.z += 1;
                    if (nextNeighbor.z == 100)
                    {
                        Debug.LogError("Could not find free space to spawn unit");
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("Search grid failed");
                break;
            }
        }
        return null;
    }
    #endregion
    #region private methods

    #endregion
}
