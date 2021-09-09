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
    #endregion
    #region properties
    public Vector2 Size
    {
        get
        {
            var box = Terrain.GetComponent<BoxCollider>();
            return new Vector2(box.size.x, box.size.y);
        }
    }
    #endregion
    #region unity methods

    // Start is called before the first frame update
    void Start()
    {

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
                Debug.Log(string.Format("Right-Clicked on map: world coordinates [{0},{1}", mapPoint.x, mapPoint.z));
                //TODO: action
                var allUnits = GetComponentsInChildren<UnitController>();
                foreach(var u in allUnits)
                {
                    //if (u.Data.IsSelected)
                    //{
                    //    u.Data.WayPoints = new List<Vector2>() { mapPoint };
                    //    u.Data.IsMoving = true;
                    //}
                    u.Agent.destination = mapPoint;
                }
            }
        }
    }
    #endregion
    #region public methods
   
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
