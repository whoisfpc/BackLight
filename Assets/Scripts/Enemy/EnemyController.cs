using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public Transform points;
    public RouteMap route;
    public enum RouteMap {Trunback, Loop};

    private bool turnBack;
    private int startPointIndex;
    private PolyNavAgent agent;

    // Use this for initialization
    private void Start()
    {
        turnBack = false;
        startPointIndex = 1;
        agent = GetComponent<PolyNavAgent>();
        
        switch (route)
        {
           case RouteMap.Trunback:
                agent.SetDestination(points.GetChild(startPointIndex).position, TrunbackReachPoint);
               break;
           case RouteMap.Loop:
                agent.SetDestination(points.GetChild(startPointIndex).position, LoopReachPoint);
               break;
        }
    }

    private void TrunbackReachPoint(bool reach)
    {
        if(reach)
        {
            if( startPointIndex == points.childCount-1 )
            {
                turnBack = true;
            }

            if( startPointIndex == 0 )
            {
                turnBack = false;
            }

            if( turnBack == false )
            {
                startPointIndex += 1;
            }
            else
            {
                startPointIndex -= 1;
            }

            agent.SetDestination(points.GetChild(startPointIndex).position, TrunbackReachPoint);
        }
    }

    private void LoopReachPoint(bool reach)
    {
        if(reach)
        {
            startPointIndex += 1;
            if( startPointIndex == points.childCount )
            {
                startPointIndex = 0;
            }

            agent.SetDestination(points.GetChild(startPointIndex).position, LoopReachPoint);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
