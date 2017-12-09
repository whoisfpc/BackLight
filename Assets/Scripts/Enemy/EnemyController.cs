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


    // Use this for initialization
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        maxDistance = Vector2.Distance(startPoint.position, endPoint.position);
        gotoEnd = true;
    }

    // Update is called once per frame
    private void Update()
    {
        var pos = transform.position;
        float dis;
        if (gotoEnd)
        {
            dis = Vector2.Distance(startPoint.position, pos);
        }
        else
        {
            dis = Vector2.Distance(endPoint.position, pos);
        }
        // turn
        if (dis >= maxDistance)
        {
            gotoEnd = !gotoEnd;
        }

        Vector2 dir;
        if (gotoEnd)
        {
            dir = endPoint.position - pos;
        }
        else
        {
            dir = startPoint.position - pos;
        }
        dir.Normalize();

        Move(dir);
    }

    private void Move(Vector2 dir)
    {
        rb2d.velocity = dir * speed;
    }
}
