using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicLight2D;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    [Range(0.1f, 1f)]
    public float speedRatioInLight = 0.5f;
    private Rigidbody2D rb2d;
    private float currentSpeed;

    // Use this for initialization
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentSpeed = speed;
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
        rb2d.velocity = dir * currentSpeed;
    }

    public void OnEnterLight(GameObject go, DynamicLight light)
    {
        if (go != gameObject)
        {
            return;
        }
        Debug.Log("on enter: " + go.name + ", " + light.name);
        currentSpeed = speed * speedRatioInLight;
    }

    public void OnExitLight(GameObject go, DynamicLight light)
    {
        if (go != gameObject)
        {
            return;
        }
        Debug.Log("on exit: " + go.name + ", " + light.name);
        currentSpeed = speed;
    }
}
