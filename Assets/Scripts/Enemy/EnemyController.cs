using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public float speed = 20f;
    public Transform startPoint;
    public Transform endPoint;
    private Rigidbody2D rb2d;

    private float maxDistance;
    private bool gotoEnd;
    private bool turnBack;
    private PolyNavAgent agent;


    // Use this for initialization
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        maxDistance = Vector2.Distance(startPoint.position, endPoint.position);
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

    private void Move(Vector2 dir)
    {
        rb2d.velocity = dir * speed;
    }
}
