using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb2d;

    // Use this for initialization
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        var dir = new Vector2(h, v);
        Move(dir);
    }

    public void Reset()
    {
        Debug.Log("player reset");
    }

    private void Move(Vector2 dir)
    {
        rb2d.velocity = dir * speed;
    }

    public void myListener_onEnter(GameObject go)
    {
        //Filter by Hash
        if (gameObject.GetHashCode() == go.GetHashCode())
        {
            print(go.name + " --> OnEnter() event");
        }
    }

    public void myListener_onExit(GameObject go)
    {
        if (gameObject.GetHashCode() == go.GetHashCode())
        {
            print(go.name + " --> OnExit() event");
        }
    }

    public void myListener_onInside(GameObject go)
    {
        if (gameObject.GetHashCode() == go.GetHashCode())
            print(go.name + " --> OnInside() event");
    }
}
