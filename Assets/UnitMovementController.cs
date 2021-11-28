using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementController : MonoBehaviour
{
    #region constants
    const int OCCUPIED_TAG = 1;
    #endregion
    #region public fields
    public AIPath Pathfinder;
    public Seeker Seeker;
    public RVOController Avoider;
    #endregion
    #region private fields

    private bool occupiesSpace = true;//whether unit can move into occupied spaces
    private Vector3 moveTarget;
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
    #endregion
    #region unity methods
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    #endregion
    #region public methods
    public void StartPath(Vector3 position)
    {
        var path = Seeker.StartPath(transform.position, position, OnPathComplete);
        ((ABPath)path).calculatePartial = true;
    }
    public void StopPath()
    {
        var path = Seeker.StartPath(transform.position, transform.position, OnPathComplete);
    }
    #endregion
    #region private methods
    private void OnPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        var wayPoints = p.path;
        if (wayPoints.Count > 0)
        {
            var startNode = wayPoints[0];
            startNode.Tag = 0;
            occupiesSpace = false;
            if (wayPoints.Count == 1)
            {
                var endNode = wayPoints[wayPoints.Count - 1];
                endNode.Tag = 1;//occupied
                occupiesSpace = true;
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
            StartPath(moveTarget);//TODO: see if there's an alternative to calling this repeatedly
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
    #endregion
}
