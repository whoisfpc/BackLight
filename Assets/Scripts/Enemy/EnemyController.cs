using System.Collections;
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
    
    public Transform points;
    public RouteMap route;

    public Transform target;
    public Transform spot;

    private bool turnBack;
    private int startPointIndex;
    private PolyNavAgent agent;
    private StateMachine stateMachine;

    // Use this for initialization
    private void Start()
    {
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player be catched!!!!!!");
        }
    }

    private class StateMachine
    {
        private State currentState;

        private Dictionary<EnemyState, Dictionary<EnemyEvent, EnemyState>> map;
        private Dictionary<EnemyState, State> stateMap;
        private State nextState;
        private bool needSwitch = false;

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
            currentState = stateMap[initState];
            OnEnter();
        }

        private void OnEnter()
        {
            if (currentState.onEnter != null)
                currentState.onEnter();
        }

        private void OnExit()
        {
            if (currentState.onExit != null)
                currentState.onExit();
        }

        public void ProcessSwitch()
        {
            if (!needSwitch)
                return;
            needSwitch = false;
            OnExit();
            currentState = nextState;
            OnEnter();
        }

        public void OnUpdate()
        {
            if (currentState.onUpdate != null)
                currentState.onUpdate();
        }

        public void FireEvent(EnemyEvent enemyEvent)
        {
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
