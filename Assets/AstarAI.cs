using Pathfinding;
using Pathfinding.RVO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarAI : MonoBehaviour
{
    public Transform Target;
    private float maxOrderRadius = 3f;
    private float currentOrderRadius = 0.01f;
    private Seeker seeker;
    private RVOController avoider;
    private bool stopped;
    private bool startTimer;
    private float stopTime = 4f;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        avoider = GetComponent<RVOController>();
        seeker.StartPath(transform.position, Target.position, OnPathComplete);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped && Vector3.Distance(Target.position, transform.position) < maxOrderRadius)
        {
            if (Vector3.Distance(Target.position, transform.position) < currentOrderRadius)
            {
                stopped = true;
            }
            else
            {
                currentOrderRadius += Time.deltaTime * maxOrderRadius / stopTime;
            }
        }
        if (stopped)
        {
            seeker.StartPath(transform.position, transform.position, OnPathComplete);
        }
        else
        {
            seeker.StartPath(transform.position, Target.position);
        }
    }
    public void OnPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
    }
}
