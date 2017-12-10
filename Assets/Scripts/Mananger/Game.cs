using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicLight2D;

public class Game : MonoBehaviour
{
    public static Game instance;

    public GameObject SuccessPanel;
    public GameObject FailPanel;

    private Vector3 startPoint;

    private PlayerController mainPlayer;
    private List<EnemyController> enemys = new List<EnemyController>();
    private bool isStop = false;

    private void Awake()
    {
        instance = this;
        mainPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform.position;
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
        isStop = false;
    }

    public void Success()
    {
        Stop();
        SuccessPanel.SetActive(true);
    }

    public void Fail()
    {
        Stop();
        FailPanel.SetActive(true);
    }

    public void Stop()
    {
        mainPlayer.Stop();
        foreach (var enemy in enemys)
        {
            enemy.Stop();
        }
        isStop = true;
    }

    public void Resume()
    {
        mainPlayer.Resume();
        foreach (var enemy in enemys)
        {
            enemy.Resume();
        }
        isStop = false;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isStop) Resume(); else Stop();
        }
#endif
    }
}
