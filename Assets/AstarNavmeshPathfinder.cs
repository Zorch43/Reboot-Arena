using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarAI : MonoBehaviour
{
    public Transform Target;
    private Vector3 currentTarget;
    private Seeker seeker;
    private RichAI pathfinder;
    private RVOController avoider;
    private NavmeshCut cutter;
    private bool stopped;
    private float timer;
    private RecastGraph graph;
    private AstarAI[] otherUnits;
    // Start is called before the first frame update
    void Start()
    {
        var ai = AstarPath.active;
        AstarPath.OnGraphsUpdated += Repath;// new OnScanDelegate(Repath);
        otherUnits = transform.parent.GetComponentsInChildren<AstarAI>();
        graph = AstarPath.active.graphs[0] as RecastGraph;
        seeker = GetComponent<Seeker>();
        pathfinder = GetComponent<RichAI>();
        avoider = GetComponent<RVOController>();
        cutter = GetComponent<NavmeshCut>();
        currentTarget = Target.transform.position;
        var path = seeker.StartPath(transform.position, currentTarget, OnPathComplete);
        ((ABPath)(path)).calculatePartial = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!stopped && Vector3.Distance(Target.position, transform.position) < maxOrderRadius)
        //{
        //    if (Vector3.Distance(Target.position, transform.position) < currentOrderRadius)
        //    {
        //        stopped = true;
        //    }
        //    else
        //    {
        //        //currentOrderRadius += Time.deltaTime * maxOrderRadius / stopTime;
        //    }
        //}
        //bool shouldCut = false;
        var distance = Vector3.Distance(currentTarget, transform.position);
        if(distance < 1f)
        {
            avoider.enabled = false;
        }
        if (distance < 0.1f && !stopped)
        {
            //shouldCut = true;
            stopped = true;
            cutter.enabled = true;
            
            seeker.enabled = false;
            pathfinder.enabled = false;

            Bounds b = GetComponent<Collider>().bounds;
            GraphUpdateObject guo = new GraphUpdateObject(b);
            AstarPath.active.UpdateGraphs(guo);
            AstarPath.active.FlushGraphUpdates();
            //foreach (var u in otherUnits)
            //{
            //    u.Repath();
            //}
        }
        //else if (distance < 0.1f)
        //{
        //    stopped = true;
        //}
        //if (shouldCut)
        //{
        //    cutter.enabled = true;
        //    //add secondary graph to travesable graphs
        //    //seeker.graphMask = 1 << 1;
        //}
        //if (stopped)
        //{
        //    //disable pathfinding
        //    seeker.enabled = false;
        //    pathfinder.enabled = false;
            

        //}
        //else
        //{
        //    //timer += Time.deltaTime;
        //    //if (timer > 0.25f)
        //    //{
        //    //    timer = 0;
        //    //    var path = seeker.StartPath(transform.position, currentTarget, OnPathComplete);
        //    //    ((ABPath)(path)).calculatePartial = true;
        //    //}
        //}
        
    }
    public void OnPathComplete(Path p)
    {
        //update path
        if(p.vectorPath.Count > 0)
        {
            currentTarget = p.vectorPath[p.vectorPath.Count - 1];
            //Debug.LogWarning("MovementTarget: " + currentTarget.ToString());
        }
        
    }
    public void Repath(AstarPath ai)
    {
        Debug.LogWarning("Repathing " + gameObject.name);
        if (!stopped)
        {
            //recalculate path in response to navmesh changes
            var path = seeker.StartPath(transform.position, currentTarget, OnPathComplete);
            ((ABPath)(path)).calculatePartial = true;
            
        }
    }
}
