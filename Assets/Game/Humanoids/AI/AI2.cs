using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshPlus;
using System.Linq;
using UnityEngine.Tilemaps;

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

    CircularMovement circularMovement;
    public float minaf = 3f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circularMovement = GameObject.Find("DetectionRange").GetComponent<CircularMovement>();


    }


    new private void FixedUpdate()
    {
        if (CanMove())
        {
            if (sprinting)
            {
                agent.speed = movementSpeed * sprintspeed;
            }
            else
            {
                agent.speed = movementSpeed;
            }
        }
        else
        {
            agent.speed = 0;
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
            case AIState.Guard:
                Guard();
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
            if (target)
            {
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
            if (target)
            {
                target.targetedBy.Remove(this as Humanoid);
                target = null;
            }
            timer = timerMax;
        }
        if (state == AIState.Chase)
        {
            Chase();
            if (!target.targetedBy.Contains(this as Humanoid))
            {
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
        else if(state == AIState.Patrol || state == AIState.Guard) {
            if(target) {
                state = AIState.Chase;
            }
        }
        if ((float)health < (float)maxHealth * (25f / 100f))
        {
            state = AIState.Flee;
            return;
        }
    }

    public void CheckForTargets()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, viewDistance, Vector2.zero).Where(h => h.collider.gameObject.GetComponent<Humanoid>() != null).OrderBy(h => h.distance).ToArray();
        foreach (RaycastHit2D hit in hits)
        {
            Humanoid humanoid = hit.collider.gameObject.GetComponent<Humanoid>();
            if(humanoid != this) {
                if(humanoid.team != this.team) {
                    Debug.Log("Not the same team");
                    RaycastHit2D[] checkHits = Physics2D.RaycastAll(transform.position, humanoid.transform.position - transform.position, viewDistance).OrderBy(h => h.distance).ToArray();
                    Debug.DrawRay(transform.position, humanoid.transform.position - transform.position, Color.red, 1f);
                    foreach (RaycastHit2D checkHit in checkHits)
                    {
                        Humanoid checkHumanoid = checkHit.collider.gameObject.GetComponent<Humanoid>();
                        if(checkHumanoid == this) {
                            continue;
                        }
                        if(checkHumanoid == humanoid) {
                            Debug.Log("Found target");
                            target = humanoid;
                            state = AIState.Chase;
                            break;
                        } else if (checkHit.collider.transform.parent.gameObject.name == "Floors") {
                            Debug.Log("Hit tilemap");
                            continue;
                        } else if (checkHit.collider.isTrigger) {
                            Debug.Log("Hit trigger");
                            continue;
                        } else {
                            Debug.Log("Didn't hit target");
                            break;
                        }
                    }
                    if(target) {
                        break;
                    }
                }
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
        if(!target) {
            CheckForTargets();
        }
        Vector2 point;
        if (patrolIndex == 0)
        {
            point = patrolPoints[patrolPoints.Count - 1];
        }
        else
        {
            point = patrolPoints[patrolIndex - 1];
        }
        if (patrolTimer <= 0 && CanMove())
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
                if (Random.Range(0, 2) == 0)
                {
                    x *= -1;
                }
                float y = Random.Range(0.5f, 1f);
                if (Random.Range(0, 2) == 0)
                {
                    y *= -1;
                }
                agent.SetDestination(point + new Vector2(y, x) * 1f);
            }
            patrolTimer -= Time.fixedDeltaTime;
        }
        else
        {
            agent.SetDestination(patrolTarget);
        }
        Vector2 dir;
        if (Vector2.Distance(agent.destination, transform.position) < 0.2f)
        {
            dir = Vector2.zero;
        }
        else
        {
            dir = transform.position + agent.velocity;
        }
        TurnWeapon(transform.position, dir, Time.fixedDeltaTime);
    }

    [Header("Guard")]
    public Vector2 guardPoint;
    void Guard() {
        CheckForTargets();
        if(Vector2.Distance(transform.position, guardPoint) > 0.25f) {
            agent.SetDestination(guardPoint);
        }
    }

    [Header("Wander")]
    public Vector2 wanderPoint; //Where it wanders around
    public Vector2 wanderTarget; //It's current wander target
    public float wanderRadius; //The radius around wanderPoint it will go
    void Wander()
    {
        if(wanderTarget == Vector2.positiveInfinity || wanderTarget == Vector2.negativeInfinity) {
            wanderTarget = transform.position;
        }
        if (Vector2.Distance(transform.position, wanderTarget) < 1f || wanderTarget == Vector2.zero)
        {
            if(wanderRadius == -1) {
                wanderTarget = (Vector2)transform.position + new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            } else {
                wanderTarget = new Vector2(Random.Range(wanderPoint.x - wanderRadius, wanderPoint.x + wanderRadius), Random.Range(wanderPoint.y - wanderRadius, wanderPoint.y + wanderRadius));
            }
            NavMesh.SamplePosition(wanderTarget, out NavMeshHit hit, 15f, NavMesh.AllAreas);
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
        }
        if((Vector2)agent.destination != wanderTarget) {
            agent.SetDestination(wanderTarget);
        }
    }

    [Header("Chase")]
    Vector2 chaseTarget;
    void Chase()
    {
        if(!target) {
            CheckForTargets();
        }
        if(!target) {
            state = defaultState;
            return;
        }
        if (target.health <= 0)
        {
            target = null;
            state = defaultState;
            return;
        }
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
                if (Vector2.Distance(chaseTarget, transform.position) < agent.radius * 2)
                {
                    if (!attacking)
                    {
                        attacking = true;
                        agent.isStopped = true;
                        Invoke("Attack", 0.2f);
                    }
                }
                agent.SetDestination(chaseTarget);
                break;
            case Magic:
            case Ranged:
                //     if (Vector2.Distance(transform.position, target.transform.position) < viewDistance * 0.1f)
                //     {
                //         RunAway();
                //     }
                //     else if (Vector2.Distance(transform.position, target.transform.position) > viewDistance * 0.5f)
                //     {
                //         chaseTarget = (transform.position - target.transform.position).normalized * viewDistance * 0.4f;
                //         agent.SetDestination(chaseTarget);
                //     }
                //     else
                //     {
                //         if (!attacking)
                //         {
                //             attacking = true;
                //             agent.isStopped = true;
                //             Invoke("Attack", 0.25f);
                //         }
                //     }
                //     break;
                // default:
                //     break;

                chaseTarget = target.transform.position + ((transform.position - target.transform.position).normalized * (viewDistance * .5f));
                // Checkfor(transform.position, target.transform.position);
                if (Vector2.Distance(chaseTarget, transform.position) < 5f && Vector2.Distance(chaseTarget, transform.position) > 2f)
                {
                    sprinting = true;
                }
                if (circularMovement.parent != this)
                {
                    circularMovement.parent = this;
                }
                if (circularMovement.Checkfor(target))
                {
                    if (Vector2.Distance(chaseTarget, transform.position) < agent.radius * 1.75f)
                    {
                        if (!attacking)
                        {
                            attacking = true;
                            agent.isStopped = true;
                            Invoke("Attack", 1f);
                        }
                    }
                }
                else
                {
                    chaseTarget = target.transform.position + ((transform.position - target.transform.position).normalized * (viewDistance * .3f));
                    // Debug.Log(chaseTarget);
                }
                agent.SetDestination(chaseTarget);
                break;
        }
    }
    [Header("Flee")]
    public List<Vector2> possiblePoints = new List<Vector2>();

    void RunAway()
    {
        if (target == null)
        {
            state = defaultState;
            return;
        }
        if (Vector2.Distance(transform.position, target.transform.position) > viewDistance)
        {
            state = defaultState;
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
                // Debug.Log(Vector2.Distance(transform.position, possiblePoints[i]));
                // Debug.Log(Vector2.Distance(transform.position, closestPoint));
                if (Vector2.Distance(transform.position, possiblePoints[i]) < Vector2.Distance(transform.position, closestPoint))
                {
                    closestPoint = possiblePoints[i];
                }
            }
            agent.SetDestination(closestPoint);
            // Debug.DrawLine(transform.position, closestPoint, Color.red);
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
            // Debug.DrawLine(target.transform.position, point, Color.red);

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
        Debug.Log("Dead");
        if (equippedWeapon)
        {
            Vector2 pos = transform.position;
            pos += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.2f, -1f)).normalized * Random.Range(1.2f, 1.5f);
            GameObject go = Instantiate(GameController.GetInstance().itemPrefab, pos, Quaternion.identity);
            go.transform.position = pos;
            go.GetComponent<Item>().SetItem(equippedWeapon);
        }
        if (equippedArmor)
        {
            Vector2 pos = transform.position;
            pos += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.2f, -1f)).normalized * Random.Range(1.2f, 1.5f);
            GameObject go = Instantiate(GameController.GetInstance().itemPrefab, pos, Quaternion.identity);
            go.transform.position = pos;
            go.GetComponent<Item>().SetItem(equippedArmor);
        }
        Destroy(gameObject, .1f);
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
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(wanderPoint, wanderRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(wanderPoint, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(wanderTarget, 0.5f);
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
        if (agent)
        {
            Gizmos.DrawWireSphere(agent.destination, 0.2f);
        }
    }

    public enum AIState
    {
        Idle,
        Wander,
        Patrol,
        Guard,
        Chase,
        Attack,
        Flee,
        Dead
    }


    // bool Checkfor(Vector2 froms, Vector2 tos)
    // {
    //     Vector2 trueDirection = (tos - froms);
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, trueDirection);
    //     Debug.DrawRay(transform.position, trueDirection , Color.green);
    //     if (hit.collider.gameObject.name == "Player")
    //     {
    //         Debug.Log("Player");
    //         Debug.Log($"{hit.collider.gameObject.name} was hit");
    //         return true;
    //     }
    //     else if (hit.collider.gameObject.name != "Player")
    //     {
    //         Debug.Log("Not Player");
    //         Debug.Log($"{hit.collider.gameObject.name} was hit");
    //         return false;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

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

    // public bool Checkfor()
    // {
    //     if (target)
    //     {
    //         Vector2 froms = transform.position;
    //         Vector2 tos = target.transform.position;
    //         Vector2 trueDirection = (tos - froms);
    //         RaycastHit2D hit = Physics2D.Raycast(transform.position, trueDirection);
    //         // if (hit.collider.gameObject.name == "Player" || hit.collider.isTrigger)
    //         // {
    //         //     // Debug.Log("Player");
    //         //     Debug.Log($"{hit.collider.gameObject.name} was hit");
    //         //     return true;
    //         // }
    //         // else if (hit.collider.gameObject.name != "Player")
    //         // {
    //         //     // Debug.Log("Not Player");
    //         //     Debug.Log($"{hit.collider.gameObject.name} was hit");
    //         //     return false;
    //         // }
    //         // else
    //         // {
    //         //     return false;
    //         // }
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    public void StartDodging(Projectile projectile)
    {
        if (state == AIState.Dead)
        {
            return;

        }
        Attacked(projectile.shooter);

        if (dodgeTimer <= 0 && Random.Range(1, 101) < 25)
        {
            Vector2 shooterDirection = projectile.shooter.transform.position - transform.position;
            Vector2 dodgeDirection;
            if (Random.Range(0, 2) == 1)
            {
                dodgeDirection = new Vector2(-shooterDirection.y, shooterDirection.x);
            }
            else
            {
                dodgeDirection = new Vector2(shooterDirection.y, -shooterDirection.x);
            }

            RaycastHit2D[] hits;
            //Order by distance closest hit first
            hits = Physics2D.RaycastAll(rb.position, dodgeDirection, dodgeMultiplier).OrderBy(h => h.distance).ToArray();
            Debug.DrawRay(transform.position, dodgeDirection.normalized * 1f, Color.red, 6f);
            // Debug.DrawRay(transform.position, dodgeDirection.normalized * 3f, Color.red, 6f);
            bool dodge = true;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<AI2>() == this)
                {
                    continue;
                }
                else if (hit.collider.isTrigger)
                {
                    continue;
                }
                else
                {
                    dodge = false;
                    break;
                }
            }
            if (dodge && CanMove())
            {
                dodging = true;
                agent.SetDestination(rb.position + dodgeDirection);
                rb.MovePosition(Vector2.Lerp(rb.position, (rb.position + dodgeDirection.normalized * dodgeMultiplier), 0.5f));
            }
            else
            {
                dodgeDirection = dodgeDirection * -1;
                RaycastHit2D[] hits2;
                hits2 = Physics2D.RaycastAll(rb.position, dodgeDirection, dodgeMultiplier).OrderBy(h => h.distance).ToArray();
                Debug.DrawRay(transform.position, dodgeDirection.normalized * 1f, Color.red, 6f);
                // Debug.DrawRay(transform.position, dodgeDirection.normalized * 3f, Color.red, 6f);
                bool dodge2 = true;
                foreach (RaycastHit2D hit in hits2)
                {
                    if (hit.collider.gameObject.GetComponent<AI2>() == this)
                    {
                        continue;
                    }
                    else if (hit.collider.isTrigger)
                    {
                        continue;
                    }
                    else
                    {
                        dodge2 = false;
                        break;
                    }
                }
                if (dodge2 && CanMove())
                {
                    dodging = true;
                    agent.SetDestination(rb.position + dodgeDirection);
                    rb.MovePosition(Vector2.Lerp(rb.position, (rb.position + dodgeDirection.normalized * dodgeMultiplier), 0.5f));
                }
            }
        }
    }
}
