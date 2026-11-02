using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class NextBot : MonoBehaviour
{
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 25f;
    public float investigationDuration = 5f;

    [Header("Wandering")]
    public float wanderRadius = 20f;
    public float wanderSpeed = 5f;

    [Header("Movement")]
    public float chaseSpeed = 12f;
    public float acceleration = 18f;
    public float turnSpeed = 5f;

    [Header("Obstacle Avoidance")]
    public float obstacleCheckDistance = 2.5f;
    public float obstacleAvoidanceStrength = 4f;
    public float obstacleRayHeight = 0.8f;

    [Header("Pathfinding")]
    public float pathUpdateRate = 0.4f;
    public float waypointDistance = 0.4f;

    [Header("Stuck Detection")]
    public float stuckCheckInterval = 0.5f;
    public float minimumProgress = 0.1f;
    public float recoveryCooldown = 0.5f;

    private Rigidbody rb;
    private NavMeshPath navPath;

    private enum BotState
    {
        Wander,
        Chase,
        Investigate
    }

    private BotState currentState;

    private Vector3 targetPosition;
    private Vector3 lastCheckedPosition;

    private int currentCorner;
    private bool hasDestination;

    private float pathUpdateTimer;
    private float stuckTimer;
    private float recoveryTimer;

    private Vector3 lastPathTarget;

    private Vector3 lastKnownPlayerPosition;
    private float investigationTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        navPath = new NavMeshPath();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = false;
        }

        currentState = BotState.Wander;

        lastCheckedPosition = rb.position;

        PickRandomDestination();
    }

    private void Update()
    {
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            currentState = BotState.Chase;

            // Remember where we last saw the player.
            lastKnownPlayerPosition = player.position;
        }
        else if (currentState == BotState.Chase)
        {
            // We lost sight of the player.
            currentState = BotState.Investigate;

            investigationTimer = investigationDuration;

            SetPath(lastKnownPlayerPosition);
        }

        pathUpdateTimer -= Time.deltaTime;
        recoveryTimer -= Time.deltaTime;
        stuckTimer += Time.deltaTime;

        if (currentState == BotState.Investigate)
        {
            investigationTimer -= Time.deltaTime;

            if (investigationTimer <= 0f ||
                (hasDestination &&
                Vector3.Distance(rb.position, targetPosition) < 1.5f))
            {
                currentState = BotState.Wander;
                PickRandomDestination();
            }
        }

        // Update the chase path periodically.
        if (currentState == BotState.Chase && player != null)
        {
            if (pathUpdateTimer <= 0f)
            {
                float playerMovement = Vector3.Distance(
                    player.position,
                    lastPathTarget
                );

                if (!hasDestination || playerMovement > 1f)
                {
                    SetPath(player.position);
                }

                pathUpdateTimer = pathUpdateRate;
            }
        }

        // Pick another wandering destination when we arrive.
        if (currentState == BotState.Wander)
        {
            if (!hasDestination ||
                Vector3.Distance(rb.position, targetPosition) < 1.5f)
            {
                PickRandomDestination();
            }
        }

        CheckIfStuck();
    }

    private void FixedUpdate()
    {
        if (!hasDestination || navPath == null)
            return;

        Vector3[] corners = navPath.corners;

        if (corners.Length == 0)
            return;

        // Advance to the next waypoint when close enough.
        while (currentCorner < corners.Length - 1)
        {
            Vector3 distanceToCorner =
                corners[currentCorner] - rb.position;

            distanceToCorner.y = 0f;

            if (distanceToCorner.magnitude > waypointDistance)
                break;

            currentCorner++;
        }

        Vector3 waypoint = corners[currentCorner];

        Vector3 direction = waypoint - rb.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();
        //direction = GetAvoidanceDirection(direction);

        float speed = currentState == BotState.Chase
            ? chaseSpeed
            : wanderSpeed;

        // Slow down slightly when approaching a corner.
        if (currentCorner < corners.Length - 1)
        {
            float distanceToCorner =
                Vector3.Distance(rb.position, waypoint);

            if (distanceToCorner < 2f)
            {
                speed *= Mathf.Lerp(
                    0.5f,
                    1f,
                    distanceToCorner / 2f
                );
            }
        }

        Vector3 targetVelocity = direction * speed;

        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(
            currentVelocity.x,
            0f,
            currentVelocity.z
        );

        Vector3 velocityChange =
            targetVelocity - horizontalVelocity;

        Vector3 accelerationChange = Vector3.ClampMagnitude(
            velocityChange,
            acceleration * Time.fixedDeltaTime
        );

        rb.AddForce(
            accelerationChange,
            ForceMode.VelocityChange
        );

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            )
        );
    }


    private void CheckIfStuck()
    {
        if (stuckTimer < stuckCheckInterval)
            return;

        float distanceMoved = Vector3.Distance(
            rb.position,
            lastCheckedPosition
        );

        lastCheckedPosition = rb.position;
        stuckTimer = 0f;

        if (!hasDestination || recoveryTimer > 0f)
            return;

        float distanceToTarget = Vector3.Distance(
            rb.position,
            targetPosition
        );

        if (distanceMoved < minimumProgress &&
            distanceToTarget > 2f)
        {
            if (currentState == BotState.Wander)
            {
                // Abandon the blocked wandering destination.
                hasDestination = false;
                PickRandomDestination();
            }
            else if (currentState == BotState.Investigate)
            {
                // Try the last known player position again.
                SetPath(lastKnownPlayerPosition);
            }
            else if (currentState == BotState.Chase && player != null)
            {
                // Recalculate toward the player's current position.
                SetPath(player.position);
            }

            recoveryTimer = recoveryCooldown;
        }
    }


    private void SetPath(Vector3 destination)
    {
        NavMeshHit startHit;

        if (!NavMesh.SamplePosition(
            rb.position,
            out startHit,
            5f,
            NavMesh.AllAreas))
        {
            //Debug.LogWarning("PATH FAILED: Cannot find NextBot on NavMesh.");
            return;
        }

        NavMeshHit destinationHit;

        if (!NavMesh.SamplePosition(
            destination,
            out destinationHit,
            5f,
            NavMesh.AllAreas))
        {
            //Debug.LogWarning("PATH FAILED: Cannot find destination on NavMesh.");
            return;
        }

        NavMeshPath newPath = new NavMeshPath();

        bool foundPath = NavMesh.CalculatePath(
            startHit.position,
            destinationHit.position,
            NavMesh.AllAreas,
            newPath
        );

        if (!foundPath)
        {
            //Debug.LogWarning("PATH FAILED: CalculatePath returned false.");
            return;
        }

        if (newPath.status != NavMeshPathStatus.PathComplete)
        {
            //Debug.LogWarning(
                //"PATH INCOMPLETE: " + newPath.status
            //);
            return;
        }

        navPath = newPath;

        currentCorner = navPath.corners.Length > 1 ? 1 : 0;

        targetPosition = destinationHit.position;
        hasDestination = true;

        //Debug.Log(
            //"NEW PATH CREATED! Waypoints: " + navPath.corners.Length
        //);

        lastPathTarget = destinationHit.position;
    }

    private void PickRandomDestination()
    {
        Vector3 randomDirection =
            Random.insideUnitSphere * wanderRadius;

        randomDirection += rb.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            randomDirection,
            out hit,
            wanderRadius,
            NavMesh.AllAreas))
        {
            SetPath(hit.position);
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 target = player.position + Vector3.up;

        Vector3 direction = target - origin;
        float distance = direction.magnitude;

        if (distance > detectionRange)
            return false;

        direction.Normalize();

        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            direction,
            distance
        );

        System.Array.Sort(
            hits,
            (a, b) => a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == transform ||
                hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            bool isPlayer =
                hit.collider.transform == player ||
                hit.collider.transform.IsChildOf(player) ||
                player.IsChildOf(hit.collider.transform);

            if (isPlayer)
                return true;

            return false;
        }

        return false;
    }

    private Vector3 GetAvoidanceDirection(Vector3 moveDirection)
    {
        Vector3 rayOrigin = transform.position +
            Vector3.up * obstacleRayHeight;

        Vector3 leftDirection =
            Quaternion.Euler(0f, -35f, 0f) * moveDirection;

        Vector3 rightDirection =
            Quaternion.Euler(0f, 35f, 0f) * moveDirection;

        Vector3 avoidance = Vector3.zero;

        RaycastHit hit;

        // Check directly ahead.
        if (Physics.Raycast(
            rayOrigin,
            moveDirection,
            out hit,
            obstacleCheckDistance))
        {
            if (hit.transform != transform &&
                !hit.transform.IsChildOf(transform))
            {
                avoidance += hit.normal;
            }
        }

        // Check slightly to the left.
        if (Physics.Raycast(
            rayOrigin,
            leftDirection,
            out hit,
            obstacleCheckDistance))
        {
            if (hit.transform != transform &&
                !hit.transform.IsChildOf(transform))
            {
                avoidance += hit.normal;
            }
        }

        // Check slightly to the right.
        if (Physics.Raycast(
            rayOrigin,
            rightDirection,
            out hit,
            obstacleCheckDistance))
        {
            if (hit.transform != transform &&
                !hit.transform.IsChildOf(transform))
            {
                avoidance += hit.normal;
            }
        }

        avoidance.y = 0f;

        if (avoidance.sqrMagnitude > 0.01f)
        {
            return (
                moveDirection +
                avoidance.normalized * obstacleAvoidanceStrength
            ).normalized;
        }

        return moveDirection;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CharacterController playerController =
            collision.collider.GetComponentInParent<CharacterController>();

        if (playerController != null)
        {
            PlayerDeathManager deathManager =
                FindObjectOfType<PlayerDeathManager>();

            if (deathManager != null)
            {
                deathManager.PlayerDied();
            }
        }
    }
}



// OLD NEXTBOT LOGIC.

/*using UnityEngine;
using UnityEngine.AI;

public class NextBot : MonoBehaviour
{
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 25f;

    [Header("Wandering")]
    public float wanderRadius = 20f;

    private NavMeshAgent agent;

    private enum BotState
    {
        Wander,
        Chase
    }

    private BotState currentState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log(
        "On NavMesh: " +
        agent.isOnNavMesh
    );

        currentState = BotState.Wander;

        PickRandomDestination();
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            currentState = BotState.Chase;
        }

        switch (currentState)
        {
            case BotState.Wander:

                if (
                    !agent.pathPending &&
                    agent.remainingDistance < 1f
                )
                {
                    PickRandomDestination();
                }

                break;

            case BotState.Chase:

                if (canSeePlayer)
                {
                    agent.SetDestination(
                        player.position
                    );
                }
                else
                {
                    currentState = BotState.Wander;

                    PickRandomDestination();
                }

                break;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 origin =
            transform.position + Vector3.up * 1f;

        Vector3 target =
            player.position + Vector3.up * 1f;

        Vector3 direction =
            target - origin;

        float distance = direction.magnitude;

        if (distance > detectionRange)
            return false;

        direction.Normalize();

        // Find everything the ray hits.
        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            direction,
            distance
        );

        // Sort hits from closest to farthest.
        System.Array.Sort(
            hits,
            (a, b) => a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            // Ignore the Nextbot's own colliders.
            if (
                hit.collider.transform == transform ||
                hit.collider.transform.IsChildOf(transform)
            )
            {
                continue;
            }

            // Check whether the hit belongs to the player.
            bool isPlayer =
                hit.collider.transform == player ||
                hit.collider.transform.IsChildOf(player) ||
                player.IsChildOf(hit.collider.transform);

            if (isPlayer)
                return true;

            // Something else is blocking the player's visibility.
            return false;
        }

        return false;
    }

    private void PickRandomDestination()
    {
        Vector3 randomDirection =
            Random.insideUnitSphere *
            wanderRadius;

        randomDirection +=
            transform.position;

        NavMeshHit hit;

        if (
            NavMesh.SamplePosition(
                randomDirection,
                out hit,
                wanderRadius,
                NavMesh.AllAreas
            )
        )
        {
            agent.SetDestination(
                hit.position
            );
        }
    }
}*/
