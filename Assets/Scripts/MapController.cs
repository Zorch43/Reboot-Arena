using Assets.Scripts.Data_Models;
using Assets.Scripts.Data_Templates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    #region constants
    public const string TERRAIN_SPRITE_PATH = "Sprites/Terrain";
    #endregion
    #region public fields
    public GameObject Terrain;
    #endregion
    #region private fields
    List<List<SpriteRenderer>> mapTiles = new List<List<SpriteRenderer>>();
    private bool makingRectSelection;
    private Vector2 rectSelectStart;
    private Vector2 rectSelectEnd;
    #endregion
    #region properties
    public Vector2 Size
    {
        get
        {
            var box = Terrain.GetComponent<BoxCollider>();
            return new Vector2(box.bounds.size.x, box.bounds.size.z) / 2;
        }
    }
    public List<DroneController> Units { get; private set; } = new List<DroneController>();
    #endregion
    #region unity methods
    #endregion
    #region public methods
    public void RegisterUnit(DroneController unit)
    {
        if (!Units.Contains(unit))
        {
            Units.Add(unit);
        }
    }
    public void UnRegisterUnit(DroneController unit)
    {
        if (Units.Contains(unit))
        {
            Units.Remove(unit);
        }
    }
    #endregion
    #region private methods
    
    #endregion
}
