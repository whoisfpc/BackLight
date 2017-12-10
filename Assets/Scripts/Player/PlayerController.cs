using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicLight2D;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public float speed = 5f;
    [Range(0.1f, 1f)]
    public float speedRatioInLight = 0.5f;
    private Rigidbody2D rb2d;
    private float currentSpeed;
    private float inlightCount = 0;
    private bool isStop = false;

    // Use this for initialization
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentSpeed = speed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isStop) return;
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        var dir = new Vector2(h, v);
        Move(dir);
    }

    public void Reset()
    {
        currentSpeed = speed;
        inlightCount = 0;
        isStop = false;
    }

    public void Stop()
    {
        isStop = true;
        Move(Vector2.zero);
    }

    public void Resume()
    {
        isStop = false;
    }

    private void Move(Vector2 dir)
    {
        rb2d.velocity = dir * currentSpeed;
        var vel = rb2d.velocity;
        if (vel == Vector2.zero)
        {
            anim.SetTrigger("MoveStop");
        }
        else if (Mathf.Abs(vel.x) > Mathf.Abs(vel.y))
        {
            if (vel.x > 0)
            {
                anim.SetTrigger("MoveRight");
            }
            else
            {
                anim.SetTrigger("MoveLeft");
            }
        }
        else
        {
            if (vel.y > 0)
            {
                anim.SetTrigger("MoveUp");
            }
            else
            {
                anim.SetTrigger("MoveDown");
            }
        }
    }

    public void OnEnterLight(GameObject go, DynamicLight light)
    {
        if (go != gameObject)
        {
            return;
        }
        inlightCount++;
        if (inlightCount > 0)
            currentSpeed = speed * speedRatioInLight;
    }

    public void OnExitLight(GameObject go, DynamicLight light)
    {
        if (go != gameObject)
        {
            return;
        }
        inlightCount--;
        if (inlightCount < 0) inlightCount = 0;
        if (inlightCount == 0)
            currentSpeed = speed;
    }
}
