using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    private bool gotoEnd;
    private bool turnBack;
    private PolyNavAgent agent;


    // Use this for initialization
    private void Start()
    {
        gotoEnd = true;
        turnBack = false;
        agent = GetComponent<PolyNavAgent>();
        agent.SetDestination(endPoint.position, ReachEnd);
    }

    private void ReachEnd(bool reach)
    {
        if (reach)
        {
            gotoEnd = false;
            turnBack = true;
        }
    }

    private void ReachStart(bool reach)
    {
        if (reach)
        {
            gotoEnd = true;
            turnBack = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (turnBack)
        {
            turnBack = false;
            if (gotoEnd)
            {
                agent.SetDestination(endPoint.position, ReachEnd);
            }
            else
            {
                agent.SetDestination(startPoint.position, ReachStart);
            }
        }
    }
}
