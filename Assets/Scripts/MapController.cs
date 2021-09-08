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
    public SpriteRenderer TilePrototype;
    public Vector2 Size;
    #endregion
    #region private fields
    List<List<SpriteRenderer>> mapTiles = new List<List<SpriteRenderer>>();
    #endregion
    #region properties
    public MapModel LoadedMap { get; set; }
    #endregion
    #region unity methods

    // Start is called before the first frame update
    void Start()
    {
        //build map from data
        //create demo map
        //LoadedMap = MapTemplates.getBasicRandomMap(20, 20, 0.20);
        //UpdateMap();
        var collider = GetComponent<BoxCollider>();
        collider.size = Size;
        collider.center = Size / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //mouse interface
        //move camera if mouse is near the edge of the viewport
        
        //get map position of mouse click
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //var mapPoint = hit.point;
                //Debug.Log(string.Format("Left-Clicked on map: world coordinates [{0},{1}", mapPoint.x, mapPoint.y));
                //TODO: selection
                var selection = hit.transform.GetComponent<UnitController>();
                if (selection != null)
                {
                    SelectUnits(new List<UnitController>() { selection });
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var mapPoint = hit.point;
                Debug.Log(string.Format("Right-Clicked on map: world coordinates [{0},{1}", mapPoint.x, mapPoint.y));
                //TODO: action
                var allUnits = GetComponentsInChildren<UnitController>();
                foreach(var u in allUnits)
                {
                    if (u.Data.IsSelected)
                    {
                        u.Data.WayPoints = new List<Vector2>() { mapPoint };
                        u.Data.IsMoving = true;
                    }
                }
            }
        }
    }
    #endregion
    #region public methods
    public void UpdateMap()
    {
        //clear old map tiles
        foreach(var r in mapTiles)
        {
            foreach(var t in r)
            {
                Destroy(t);
            }
        }
        mapTiles.Clear();

        //render new map
        for(int x = 0; x < LoadedMap.MapTiles.Count; x++)
        {
            mapTiles.Add(new List<SpriteRenderer>());
            for(int y = 0; y <LoadedMap.MapTiles[x].Count; y++)
            {
                var tile = Instantiate(TilePrototype, transform);
                tile.transform.position = new Vector2(x*0.32f, y*0.32f);
                //assign sprite
                var loadedSprite = Resources.Load(System.IO.Path.Combine(TERRAIN_SPRITE_PATH, LoadedMap.MapTiles[x][y].SpriteName));
                tile.sprite = Sprite.Create(loadedSprite as Texture2D, tile.sprite.rect, tile.sprite.pivot);
                mapTiles[x].Add(tile);
            }
        }
    }
    public void SelectUnits(List<UnitController> units, bool replace = true)
    {
        var allUnits = GetComponentsInChildren<UnitController>();
        if (replace)
        {
            foreach(var u in allUnits)
            {
                u.Data.IsSelected = false;
            }
        }
        foreach(var u in units)
        {
            u.Data.IsSelected = true;
        }
    }
    #endregion
    #region private methods
    
    #endregion
}
