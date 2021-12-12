using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementController : MonoBehaviour
{
    #region constants
    const int OCCUPIED_TAG = 1;
    const float NODE_SIZE = 1;
    const float PATH_INTERVAL = 0.25f;
    #endregion
    #region public fields
    public AIPath Pathfinder;
    public Seeker Seeker;
    public RVOController Avoider;
    #endregion
    #region private fields
    private GridGraph grid;
    private bool occupiesSpace = true;//whether unit can move into occupied spaces
    private GraphUpdateShape boundsShape;
    private GraphNode occupiedNode;
    private Path currentPath;
    private float pathTimer;
    #endregion
    #region properties
    public bool HasPath
    {
        get
        {
            return !occupiesSpace;
        }
    }
    public bool CanTurn 
    {
        get
        {
            return Pathfinder.enableRotation;
        }
        set
        {
            Pathfinder.enableRotation = value;
        }
    }

    public float Speed
    {
        get
        {
            return Pathfinder.maxSpeed;
        }
        set
        {
            Pathfinder.maxSpeed = value;
        }
    }
    public float TurnSpeed
    {
        get
        {
            return Pathfinder.rotationSpeed;
        }
        set
        {
            Pathfinder.rotationSpeed = value;
        }
    }
    public Vector3 Destination { get; private set; }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        grid = AstarPath.active.graphs[0] as GridGraph;
        
    }

    // Update is called once per frame
    void Update()
    {
        //float deltaTime = Time.deltaTime;
        //pathTimer += deltaTime;
        //if(pathTimer > PATH_INTERVAL)
        //{
        //    pathTimer = 0;
        //    StartPath(Destination, true);
        //}
        //if (currentPath != null && currentPath.IsDone())
        //{
        //    //if any of the path's nodes are unwalkable or non-traversable, restart path
        //    var wayPoints = currentPath.path;
        //    //check path
        //    int i = 0;
        //    while (i < wayPoints.Count && wayPoints.Count > 1)
        //    {
        //        //if on the next node is nearer than the first node, remove the first node
        //        //repeat until first node is the nearest
        //        if (i == 0 
        //            && Vector3.Distance(transform.position, (Vector3)wayPoints[i].position) 
        //            > Vector3.Distance(transform.position, (Vector3)wayPoints[i + 1].position))
        //        {
        //            //remove 
        //            wayPoints.RemoveAt(i);
        //        }
        //        else
        //        {
        //            //check untravelled path for unwalkable or nontraversable nodes
        //            var n = wayPoints[i];
        //            if(!IsNodeValid(n))
        //            {
        //                //if found, recalculate node
        //                StartPath(Destination, true);
        //                break;
        //            }

        //            i++;
        //        }
        //    }
        //    //if near to the end of the path, ot the path has only 1 node, claim the last node
        //    if (wayPoints.Count == 1)
        //    {
        //        var endPoint = wayPoints[0];
        //        ClaimNode(endPoint);
        //    }
        //}

    }
    #endregion
    #region public methods
    public void StartPath(Vector3 position, bool repath = false)
    {
        if (!repath)
        {
            Destination = position;
        }
        currentPath = Seeker.StartPath(transform.position, position, OnPathComplete);
        ((ABPath)currentPath).calculatePartial = true;
    }
    public void StopPath()
    {
        var path = Seeker.StartPath(transform.position, transform.position, OnPathComplete);
        Destination = transform.position;
    }
    #endregion
    #region private methods
    private void OnPathComplete(Path p)
    {
        //currentPath = p;
        //var waypoints = p.path;
        //FreeNode(p.path[0]);
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        var wayPoints = p.path;
        if (wayPoints.Count > 0)
        {

            Destination = (Vector3)wayPoints[wayPoints.Count - 1].position;

            if (wayPoints.Count == 1)
            {
                var endNode = wayPoints[wayPoints.Count - 1];
                //endNode.Tag = 1;//occupied
                //TODO: allow unit to take up multiple spot for better resolution
                MarkOccupiedNodes(endNode, true);
                occupiesSpace = true;
            }
            else
            {
                var startNode = wayPoints[0];
                //startNode.Tag = 0;
                MarkOccupiedNodes(startNode, false);
                occupiesSpace = false;
            }
        }
        else
        {
            occupiesSpace = true;
        }
        if (occupiesSpace)
        {
            SetTag(1);
            Avoider.enabled = false;
        }
        else
        {
            UnSetTag(1);
            Avoider.enabled = true;
            //Avoider.enabled = false;

            StartPath(Destination, true);//TODO: see if there's an alternative to calling this repeatedly
        }
    }
    private void SetTag(int tag)
    {
        int bitTag = 1 << tag;
        Seeker.traversableTags = Seeker.traversableTags | bitTag;
    }
    private void UnSetTag(int tag)
    {
        int bitTag = 1 << tag;
        Seeker.traversableTags = Seeker.traversableTags & (~bitTag);
    }
    private void MarkOccupiedNodes(GraphNode n, bool markOccupied)
    {
        if (markOccupied)
        {
            n.Tag = OCCUPIED_TAG;
        }
        else
        {
            n.Tag = 0;
        }
    }
    private bool IsNodeValid(GraphNode n)
    {
        if(n == occupiedNode)
        {
            return true;
        }
        return n.Walkable && n.Tag != OCCUPIED_TAG;
    }
    private void ClaimNode(GraphNode n)
    {
        MarkOccupiedNodes(n, true);
        SetTag(OCCUPIED_TAG);
        Avoider.enabled = false;
    }
    private void FreeNode(GraphNode n)
    {
        MarkOccupiedNodes(n, false);
        UnSetTag(OCCUPIED_TAG);
        Avoider.enabled = true;
    }
    #endregion
}
