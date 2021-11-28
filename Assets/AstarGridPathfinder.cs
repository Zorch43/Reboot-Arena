using Pathfinding;
using Pathfinding.RVO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarGridPathfinder : MonoBehaviour
{

    

    private Seeker seeker;
    private RVOController avoider;
    private bool occupiesSpace = true;//whether unit can move into occupied spaces
    private bool isStopped = true;
    private Vector3 moveTarget;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        avoider = GetComponent<RVOController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //raycast to the point on the terrain plane
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, 0);
            float enter = 0;
            if (plane.Raycast(ray, out enter))
            {
                //target point
                var mapPoint = ray.GetPoint(enter);

                moveTarget = mapPoint;
                StartPath(mapPoint);
            }
        }
    }
    public void StartPath(Vector3 position)
    {
        var path = seeker.StartPath(transform.position, position, OnPathComplete);
        ((ABPath)path).calculatePartial = true;
    }
    public void OnPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        var wayPoints = p.path;
        if (wayPoints.Count > 0)
        {
            var startNode = wayPoints[0];
            startNode.Tag = 0;
            occupiesSpace = false;
            isStopped = false;
            if (wayPoints.Count == 1)
            {
                var endNode = wayPoints[wayPoints.Count - 1];
                endNode.Tag = 1;//occupied
                occupiesSpace = true;
            }
        }
        else
        {
            //stopped
            isStopped = true;
            occupiesSpace = true;
            //TODO: mark current node as occupied
        }
        if (occupiesSpace)
        {
            SetTag(1);
            avoider.enabled = false;
        }
        else
        {
            UnSetTag(1);
            avoider.enabled = true;
            StartPath(moveTarget);
        }
    }
    private void SetTag(int tag)
    {
        int bitTag = 1 << tag;
        seeker.traversableTags = seeker.traversableTags | bitTag;
    }
    private void UnSetTag(int tag)
    {
        int bitTag = 1 << tag;
        seeker.traversableTags = seeker.traversableTags & (~bitTag);
    }
}
