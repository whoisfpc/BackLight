using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    private Vector3 startPoint;
    private Vector3 endPoint;

    private PlayerController mainPlayer;

    private void Awake()
    {
        mainPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform.position;
        endPoint = GameObject.FindGameObjectWithTag("EndPoint").transform.position;
    }

    public void Restart()
    {
        mainPlayer.transform.position = startPoint;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
#endif
    }
}
