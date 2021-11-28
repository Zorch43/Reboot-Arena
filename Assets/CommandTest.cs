using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTest : MonoBehaviour
{
    public GameObject Units;
    private Seeker[] units;
    private float minClearance = 1.5f;
    private float maxRadius = 5f;
    // Start is called before the first frame update
    void Start()
    {
        units = Units.GetComponentsInChildren<Seeker>();
    }

    // Update is called once per frame
    void Update()
    {
        //on right-click, move all units to the clicked location
        if (Input.GetMouseButton(1))
        {
            //raycast to the point on the terrain plane
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, 0);
            float enter = 0;
            if(plane.Raycast(ray, out enter))
            {
                //central order
                var mapPoint = ray.GetPoint(enter);

                var formation = new List<Vector3>();
                //get formation
                foreach(var u in units)
                {
                    formation.Add(u.transform.position);
                }

                //get a destination for each unit
                PathUtilities.GetPointsAroundPointWorld(mapPoint, AstarPath.active.graphs[0] as IRaycastableGraph, formation, maxRadius, minClearance);
                //assign a destination for each unit
                for(int i = 0; i < units.Length; i++)
                {
                    var u = units[i];
                    u.StartPath(u.transform.position, formation[i]);
                }
            }
        }
    }
}
