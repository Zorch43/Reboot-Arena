using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarNavmeshPathfinder2 : MonoBehaviour
{
    public Transform Target;
    private Seeker seeker;
    private RichAI pathfinder;
    private Rigidbody body;
    private bool stopped;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        pathfinder = GetComponent<RichAI>();
        body = GetComponent<Rigidbody>();
        seeker.StartPath(transform.position, Target.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {
            var distance = Vector3.Distance(transform.position, Target.position);
            if (distance < 0.55f)
            {
                //seeker.StartPath(transform.position, transform.position);
                seeker.enabled = false;
                pathfinder.enabled = false;
                body.mass = 0.2f;
                //body.isKinematic = true;
                //pathfinder.canMove = false;
                
                stopped = true;
            }
        }
        
    }
}
