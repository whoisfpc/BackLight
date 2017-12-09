using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicLight2D;

public class Game : MonoBehaviour
{
    public static Game instance;

    private Vector3 startPoint;
    private Vector3 endPoint;

    private PlayerController mainPlayer;
    private List<EnemyController> enemys = new List<EnemyController>();

    private void Awake()
    {
        instance = this;
        mainPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform.position;
        endPoint = GameObject.FindGameObjectWithTag("EndPoint").transform.position;
        enemys.AddRange(FindObjectsOfType<EnemyController>());
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        var bonefires = GameObject.FindGameObjectsWithTag("Bonefire");
        foreach (var bonefire in bonefires)
        {
            var light = bonefire.GetComponent<DynamicLight>();
            light.useEvents = true;
            light.OnEnterFieldOfView += mainPlayer.OnEnterLight;
            light.OnExitFieldOfView += mainPlayer.OnExitLight;
        }
    }

    public void Restart()
    {
        mainPlayer.transform.position = startPoint;
        mainPlayer.Reset();
        foreach (var enemy in enemys)
        {
            enemy.Reset();
        }
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
