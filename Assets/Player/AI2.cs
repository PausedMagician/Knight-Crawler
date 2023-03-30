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
    [Header("Equipment")]
    public Vector2 levelRange = new Vector2(1, 5);
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
        if (sprinting)
        {
            agent.speed = movementSpeed * sprintspeed;
        }
        else
        {
            agent.speed = movementSpeed;
        }
        // Check what state it should be in.
        CheckState();

        // Act on the state.
        switch (state)
        {
            case AIState.Idle:
                break;
            case AIState.Wander:
                Wander();
                break;
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack:
                Attack();
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


    public bool targetInSight = false;
    public bool attacking = false;
    void CheckState()
    {
        if (state == AIState.Dead)
        {
            if(target) {
                target.targetedBy.Remove(this as Humanoid);
                target = null;
            }
            return;
        }
        if (timer > 0)
        {
            if (Mathf.Round(timer) % 5 == 0 && target != null)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, target.transform.position - transform.position, viewDistance).OrderBy(h => h.distance).ToArray();
                if (hits.Length > 0)
                {
                    if (hits[0].collider.gameObject.GetComponent<Humanoid>() == target)
                    {
                        targetInSight = true;
                    }
                }
            }
            if (targetInSight)
            {
                timer += Time.fixedDeltaTime / 2;
            }
            timer -= Time.fixedDeltaTime;
        }
        else
        {
            state = defaultState;
            target.targetedBy.Remove(this as Humanoid);
            target = null;
            timer = timerMax;
        }
        if (health < maxHealth * (25 / 100))
        {
            state = AIState.Flee;
            return;
        }
        if (state == AIState.Chase)
        {
            Chase();
            if(!target.targetedBy.Contains(this as Humanoid)) {
                target.targetedBy.Add(this);
            }
            switch (equippedWeapon)
            {
                case Melee melee:
                    TurnWeapon(transform.position, target.transform.position, Time.fixedDeltaTime);
                    break;
                case Ranged ranged:
                case Magic magic:
                    TurnWeapon(transform.position, target.transform.position, Time.fixedDeltaTime, 45);
                    break;
                default:
                    break;
            }
        }
    }

    [Header("Patrol")]
    public int patrolIndex = 0;
    public List<Vector2> patrolPoints;
    public Vector2 patrolTarget;
    public float patrolTimer;
    public Vector2 patrolTimerValues = new Vector2(3, 5);
    void Patrol()
    {
        Vector2 point;
        if (patrolIndex == 0)
        {
            point = patrolPoints[patrolPoints.Count - 1];
        }
        else
        {
            point = patrolPoints[patrolIndex - 1];
        }
        if (patrolTimer <= 0)
        {
            patrolTarget = patrolPoints[patrolIndex] + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * 0.5f;
            patrolIndex++;
            agent.SetDestination(patrolTarget);
            if (patrolIndex > patrolPoints.Count - 1)
            {
                patrolIndex = 0;
            }
            patrolTimer = Random.Range(patrolTimerValues.x, patrolTimerValues.y);
        }
        else if (Vector2.Distance(patrolTarget, transform.position) < 1.5f || Vector2.Distance(point, transform.position) < 1.5f)
        {
            // Debug.Log("Close " + Vector2.Distance(patrolTarget, transform.position));
            if (Mathf.Round(patrolTimer) % 3 == 0 && Mathf.Round(patrolTimer) != 0 && Vector2.Distance(agent.destination, transform.position) < 0.05f)
            {
                float x = Random.Range(0.5f, 1f);
                if(Random.Range(0, 2) == 0) {
                    x *= -1;
                }
                float y = Random.Range(0.5f, 1f);
                if(Random.Range(0, 2) == 0) {
                    y *= -1;
                }
                agent.SetDestination(point + new Vector2(y, x) * 1f);
            }
            patrolTimer -= Time.fixedDeltaTime;
        }
        else
        {
            // ????? what the fuck happened
        }
        Vector2 dir;
        if(Vector2.Distance(agent.destination, transform.position) < 0.2f) {
            dir = Vector2.zero;
        } else {
            dir = transform.position + agent.velocity;
        }
        TurnWeapon(transform.position, dir, Time.fixedDeltaTime);
    }

    [Header("Wander")]
    Vector2 wanderTarget;
    void Wander()
    {

    }

    [Header("Chase")]
    Vector2 chaseTarget;
    void Chase()
    {
        switch (equippedWeapon)
        {
            case Melee:
                chaseTarget = target.transform.position + ((transform.position - target.transform.position).normalized * (agent.radius * 2.5f));
                if (Vector2.Distance(chaseTarget, transform.position) < 5f && Vector2.Distance(chaseTarget, transform.position) > 2f)
                {
                    sprinting = true;
                }
                else
                {
                    sprinting = false;
                }
                if(Vector2.Distance(chaseTarget, transform.position) < agent.radius * 2) {
                    if(!attacking) {
                        attacking = true;
                        agent.isStopped = true;
                        Invoke("Attack", 0.2f);
                    }
                }
                agent.SetDestination(chaseTarget);
                break;
            case Magic:
            case Ranged:
                if (Vector2.Distance(transform.position, target.transform.position) < viewDistance * 0.33f)
                {
                    RunAway();
                }
                else if (Vector2.Distance(transform.position, target.transform.position) > viewDistance * 0.5f)
                {
                    chaseTarget = (transform.position - target.transform.position).normalized * viewDistance * 0.4f;
                    agent.SetDestination(chaseTarget);
                }
                else
                {
                    if (!attacking)
                    {
                        attacking = true;
                        agent.isStopped = true;
                        Invoke("Attack", 0.25f);
                    }
                }
                break;
            default:
                break;
        }
    }
    [Header("Flee")]
    public List<Vector2> possiblePoints = new List<Vector2>();
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
            possiblePoints = FindPointsAround(12, viewDistance * 1.5f).ToList();
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
        TurnWeapon(transform.position, transform.position + agent.desiredVelocity, Time.fixedDeltaTime);
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

    public override void Die()
    {
        base.Die();
        this.state = AIState.Dead;
        agent.isStopped = true;
    }
    protected override void Attack()
    {
        base.Attack();
        attacking = false;
        agent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        switch (state)
        {
            case AIState.Idle:
                break;
            case AIState.Wander:
                break;
            case AIState.Patrol:
                Gizmos.color = Color.green;
                if (patrolPoints != null)
                {
                    for (int i = 0; i < patrolPoints.Count; i++)
                    {
                        Gizmos.DrawSphere(patrolPoints[i], 0.5f);
                    }
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(patrolTarget, 0.5f);
                }
                break;
            case AIState.Chase:
                break;
            case AIState.Attack:
                break;
            case AIState.Flee:
                Gizmos.color = Color.green;
                if (possiblePoints != null)
                {
                    for (int i = 0; i < possiblePoints.Count; i++)
                    {
                        Gizmos.DrawSphere(possiblePoints[i], 0.5f);
                    }
                }
                break;
            case AIState.Dead:
                break;
            default:
                break;
        }
        Gizmos.color = Color.magenta;
        if(agent) {
            Gizmos.DrawWireSphere(agent.destination, 0.2f);
        }
    }

    public enum AIState
    {
        Idle,
        Wander,
        Patrol,
        Chase,
        Attack,
        Flee,
        Dead
    }

    public void reset()
    {
        state = defaultState;
        target = null;
        targetInSight = false;
        chaseTarget = Vector2.zero;
        patrolTarget = Vector2.zero;
        wanderTarget = Vector2.zero;
        timer = timerMax;
        patrolTimer = 0;
        possiblePoints = new List<Vector2>();
    }

}
