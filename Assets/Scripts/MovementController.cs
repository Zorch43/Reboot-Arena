using Assets.Scripts.Interfaces;
using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour, IMove
{
    #region constants
    const float ORDER_RADIUS = 1f;
    const float ANCHOR_DRIFT = 2.5f;
    const float LIGHT_MASS = 0.2f;
    const float LIGHT_DRAG = 0.05f;
    const float HEAVY_MASS = 10;
    const float HEAVY_DRAG = 10;
    const float STOP_TIME = 0.5f;
    #endregion
    #region public fields
    public UnitController Controller;
    public Rigidbody Body;
    public Seeker PathfinderSeeker;
    public RichAI Pathfinder;
    public RVOController Avoider;
    #endregion
    #region private fields
    private float normalMass = 1;
    private float normalDrag = 10f;
    private List<UnitController> unitCollisions;
    private bool isStopped = true;
    private Vector3 anchorPosition;
    private Vector3 destination;
    private float stopTimer;
    #endregion
    #region properties
    public float Speed
    {
        get
        {
            return Pathfinder.maxSpeed;
        }
        set
        {
            Pathfinder.maxSpeed = value;
            Pathfinder.acceleration = value * 2;
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
    public bool CanJump
    {
        get
        {
            return (PathfinderSeeker.graphMask & (1 << 1)) == 1 << 1;
        }
        set
        {
            if (value)
            {
                PathfinderSeeker.graphMask = (1 << 1);
            }
            else
            {
                PathfinderSeeker.graphMask = 1;
            }
        }
    }
    #endregion
    #region unity methods
    // Start is called before the first frame update
    void Start()
    {
        normalMass = Body.mass;
        normalDrag = Body.drag;
        unitCollisions = new List<UnitController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsMoving())
        {
            Avoider.enabled = true;
            if (HasArrived())
            {
                GradualStop(Time.deltaTime);
            }
        }
        else 
        {
            Avoider.enabled = false;
            if (Vector3.Distance(transform.position, anchorPosition) > ANCHOR_DRIFT)
            {
                StartPath(anchorPosition);
            }  
        }
    }
    void FixedUpdate()
    {
        if (!IsMoving())
        {
            var blockMode = ShouldBlock();
            if (blockMode < 0)
            {
                Block();
            }
            else if(blockMode > 0)
            {
                UnBlock();
            }
            else
            {
                NormalizeBody();
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        var unit = collision.collider.GetComponent<UnitController>();
        if(unit != null)
        {
            unitCollisions.Add(unit);
        }
    }
    void OnCollisionExit(Collision collision)
    {
        var unit = collision.collider.GetComponent<UnitController>();
        if (unit != null)
        {
            unitCollisions.Remove(unit);
        }
    }
    #endregion
    #region public methods
    public void GradualStop(float deltaTime)
    {
        stopTimer += deltaTime;
        if(stopTimer > STOP_TIME)
        {
            Stop();
            stopTimer = 0;
        }
    }
    #region IMove implementation 
    public void Block()
    {
        Body.mass = HEAVY_MASS;
        Body.drag = HEAVY_DRAG;
    }

    public Vector3 GetDestination()
    {
        return destination;
    }

    public bool IsMoving()
    {
        return !isStopped;
    }

    public int ShouldBlock()
    {
        foreach(var u in unitCollisions)
        {
            if(u.Data.Team != Controller.Data.Team)
            {
                return -1;
            }
            else if (u.IsMoving)
            {
                return 1;
            }
        }
        return 0;
    }

    public void StartPath(Vector3 destination)
    {
        Pathfinder.enabled = true;
        PathfinderSeeker.enabled = true;
        //isStopped = false;
        NormalizeBody();
        PathfinderSeeker.StartPath(transform.position, destination, OnPathComplete);
        if (Controller.Data.UnitClass.HasJumpBoost)
        {
            Body.constraints = ~RigidbodyConstraints.FreezePositionY & Body.constraints;
        }
    }

    public void Stop()
    {
        Pathfinder.enabled = false;
        PathfinderSeeker.enabled = false;
        isStopped = true;
        anchorPosition = transform.position;
        destination = anchorPosition;
        Body.constraints = RigidbodyConstraints.FreezePositionY | Body.constraints;
    }

    public void UnBlock()
    {
        Body.mass = LIGHT_MASS;
        Body.drag = LIGHT_DRAG;
    }
    public bool HasArrived()
    {
        if (IsMoving())
        {
            return Vector3.Distance(transform.position, GetDestination()) < ORDER_RADIUS;
        }
        else
        {
            return true;
        }
    }
    #endregion
    public void NormalizeBody()
    {
        Body.mass = normalMass;
        Body.drag = normalDrag;
    }

    
    #endregion
    #region private methods
    private void OnPathComplete(Path path)
    {
        var wayPoints = path.vectorPath;
        if(wayPoints.Count > 1)
        {
            destination = wayPoints[wayPoints.Count - 1];
            isStopped = false;
        }
        
    }

    #endregion



}
