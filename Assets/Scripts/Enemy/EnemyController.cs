﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public enum RouteMap
    {
        Trunback, Loop
    }

    public enum EnemyState
    {
        Patrol,
        Catching,
        CatchNoTarget
    }

    public enum EnemyEvent
    {
        PlayerEnterView,
        PlayerExitView
    }

    public Animator anim;
    public Transform points;
    public RouteMap route;
    public Transform spot;

    private bool turnBack;
    private int startPointIndex;
    private PolyNavAgent agent;
    private StateMachine stateMachine;
    private Vector3 resetPosition;
    private Quaternion resetSpotRotation;
    private Transform target;
    private bool isStop = false;
    

    // Use this for initialization
    private void Start()
    {
        resetPosition = transform.position;
        resetSpotRotation = spot.transform.rotation;
        turnBack = false;
        startPointIndex = 1;
        agent = GetComponent<PolyNavAgent>();

        var patrolState = new State()
        {
            enemyState = EnemyState.Patrol,
            onEnter = StartPatrol,
            onUpdate = () => {},
            onExit = () => {}
        };

        var catchingState = new State()
        {
            enemyState = EnemyState.Catching,
            onEnter = () => { agent.SetDestination(target.position); },
            onUpdate = () => { agent.SetDestination(target.position); },
            onExit = () => {}
        };
        stateMachine = new StateMachine();
        stateMachine
            .SetStateMap(EnemyState.Patrol, patrolState)
            .SetStateMap(EnemyState.Catching, catchingState);
        stateMachine.Boost(EnemyState.Patrol);

    }

    public void Reset()
    {
        transform.position = resetPosition;
        spot.transform.rotation = resetSpotRotation;
        turnBack = false;
        startPointIndex = 1;
        stateMachine.Reset();
        isStop = false;
    }

    public void Stop()
    {
        isStop = true;
        agent.Stop();
        stateMachine.Stop();
    }

    public void Resume()
    {
        isStop = false;
        stateMachine.Resume();
    }

    private void StartPatrol()
    {
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
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.I))
        {
            stateMachine.FireEvent(EnemyEvent.PlayerEnterView);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            stateMachine.FireEvent(EnemyEvent.PlayerExitView);
        }
#endif
        stateMachine.ProcessSwitch();
        stateMachine.OnUpdate();
        if (agent.movingDirection != Vector2.zero){
			float rot = -Mathf.Atan2(agent.movingDirection.x, agent.movingDirection.y) * 180 / Mathf.PI;
			float newZ = Mathf.MoveTowardsAngle(spot.localEulerAngles.z, rot, agent.rotateSpeed * Time.deltaTime);
			spot.localEulerAngles = new Vector3(spot.localEulerAngles.x, spot.localEulerAngles.y, newZ);
		}
        var vel = agent.movingDirection;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Game.instance.Fail();
        }
    }

    public void OnLightEnter(GameObject go)
    {
        if (isStop) return;
        if (go.CompareTag("Player"))
        {
            target = go.transform;
            stateMachine.FireEvent(EnemyEvent.PlayerEnterView);
        }
    }

    public void OnLightExit(GameObject go)
    {
        if (isStop) return;
        if (go.CompareTag("Player"))
        {
            stateMachine.FireEvent(EnemyEvent.PlayerExitView);
        }
    }

    private class StateMachine
    {
        private State currentState;

        private Dictionary<EnemyState, Dictionary<EnemyEvent, EnemyState>> map;
        private Dictionary<EnemyState, State> stateMap;
        private State nextState;
        private bool needSwitch = false;
        private State initState;
        private bool isStop = false;

        public StateMachine()
        {
            map = new Dictionary<EnemyState, Dictionary<EnemyEvent, EnemyState>>();
            map[EnemyState.Patrol] = new Dictionary<EnemyEvent, EnemyState>();
            map[EnemyState.Patrol][EnemyEvent.PlayerEnterView] = EnemyState.Catching;
            map[EnemyState.Catching] = new Dictionary<EnemyEvent, EnemyState>();
            map[EnemyState.Catching][EnemyEvent.PlayerExitView] = EnemyState.Patrol;
            stateMap = new Dictionary<EnemyState, State>();
        }

        public StateMachine SetStateMap(EnemyState enemyState, State state)
        {
            stateMap[enemyState] = state;
            return this;
        }

        public void Boost(EnemyState initState)
        {
            this.initState = currentState = stateMap[initState];
            OnEnter();
        }

        public void Reset()
        {   
            OnExit();
            currentState = this.initState;
            isStop = false;
            OnEnter();
        }
        
        public void Stop()
        {
            isStop = true;
            OnExit();
        }

        public void Resume()
        {
            isStop = false;
            OnEnter();
        }

        private void OnEnter()
        {
            if (isStop) return;
            if (currentState.onEnter != null)
                currentState.onEnter();
        }

        private void OnExit()
        {
            if (isStop) return;
            if (currentState.onExit != null)
                currentState.onExit();
        }

        public void ProcessSwitch()
        {
            if (isStop) return;
            if (!needSwitch)
                return;
            needSwitch = false;
            OnExit();
            currentState = nextState;
            OnEnter();
        }

        public void OnUpdate()
        {
            if (isStop) return;
            if (currentState.onUpdate != null)
                currentState.onUpdate();
        }

        public void FireEvent(EnemyEvent enemyEvent)
        {
            if (isStop) return;
            if (map[currentState.enemyState].ContainsKey(enemyEvent))
            {
                var nextEnemyState = map[currentState.enemyState][enemyEvent];
                nextState = stateMap[nextEnemyState];
                needSwitch = true;
            }
        }
    }

    private class State
    {
        public EnemyState enemyState;
        public Action onEnter;
        public Action onExit;
        public Action onUpdate;
    }
}
