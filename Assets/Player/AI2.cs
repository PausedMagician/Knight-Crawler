using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody2D), typeof(Animator))]
// [RequireComponent(typeof(Rigidbody2D))]
public class AI2 : Humanoid
{
    [Header("AI Settings")]
    public bool agressive = false;
    public AIState defaultState = AIState.Idle;
    public float viewDistance = 10f;
    [Header("AI Components")]
    public NavMeshAgent agent;
    // public NavMeshObstacle obstacle;
    public Animator animator;
    public AIState state = AIState.Idle;

    [Header("AI Variables")]
    public Humanoid target;
    public float timer;
    public float timerMax = 30f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }


    new private void FixedUpdate()
    {
        // Check what state it should be in.
        CheckState();

        // Act on the state.
        switch (state)
        {
            case AIState.Idle:
                break;
            case AIState.Patrol:
                break;
            case AIState.Chase:
                break;
            case AIState.Attack:
                break;
            case AIState.Flee:
                RunAway();
                break;
            case AIState.Dead:
                break;
            default:
                break;
        }
    }


    void CheckState()
    {
        if (state == AIState.Dead)
        {
            return;
        }
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
        }
        else
        {
            state = defaultState;
            timer = timerMax;
        }
        if (health < maxHealth * (25 / 100))
        {
            state = AIState.Flee;
            return;
        }
    }



    List<Vector2> possiblePoints = new List<Vector2>();
    void RunAway()
    {
        if (target == null)
        {
            state = AIState.Idle;
            return;
        }
        if (Vector2.Distance(transform.position, target.transform.position) > viewDistance)
        {
            state = AIState.Idle;
            return;
        }
        else
        {
            // Points around player.
            possiblePoints = FindPointsAround(12, viewDistance*1.5f).ToList();
            if (possiblePoints.Count == 0)
            {
                return;
            }
            Vector2 closestPoint = possiblePoints[0];
            for (int i = 0; i < possiblePoints.Count; i++)
            {
                Debug.Log(Vector2.Distance(transform.position, possiblePoints[i]));
                Debug.Log(Vector2.Distance(transform.position, closestPoint));
                if (Vector2.Distance(transform.position, possiblePoints[i]) < Vector2.Distance(transform.position, closestPoint))
                {
                    closestPoint = possiblePoints[i];
                }
            }
            agent.SetDestination(closestPoint);
            Debug.DrawLine(transform.position, closestPoint, Color.red);
        }
    }


    Vector2[] FindPointsAround(int amountOfPoints, float distance)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < amountOfPoints; i++)
        {
            Vector2 dir = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / amountOfPoints), Mathf.Sin(Mathf.PI * 2 * i / amountOfPoints)).MNormalize();

            Vector2 point = (Vector2)target.transform.position + dir * distance;
            Debug.DrawLine(target.transform.position, point, Color.red);

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas))
            {
                if (Vector2.Distance(target.transform.position, hit.position) >= distance - 1f)
                {
                    points.Add(hit.position);
                }
            }

        }
        return points.ToArray();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Gizmos.color = Color.green;
        if (possiblePoints != null)
        {
            for (int i = 0; i < possiblePoints.Count; i++)
            {
                Gizmos.DrawSphere(possiblePoints[i], 0.5f);
            }
        }
    }

    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee,
        Dead
    }

}
