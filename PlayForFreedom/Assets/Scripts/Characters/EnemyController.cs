using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

/// <summary>
/// Enemy controller contains the enemy movement and attack logic. 
/// This emulates the player inputs allowing the charafcter controller to interface with the ai the same way as a player.
/// </summary>
public class EnemyController : BMD.CharacterController
{
    const float SPLINE_PROGRESS_INCREMENT = 0.1f;
    #region Configuration
    [SerializeField] float repathInterval = 0.1f;

    [Header("Attack settings")]
    [SerializeField] AttackTargetType attackTargetType = AttackTargetType.Nothing;

    [Header("Pathing Settings")]
    [SerializeField] EnemyPathType enemyPathType = EnemyPathType.ToPlayer;
    #endregion

    #region Cached references
    private NavMeshAgent agent;
    Enemy enemy;
    EnemyPathsManger enemyPathsManager;
    SplineContainer splineContainer;
    #endregion

    #region Runtime Variables
    Coroutine repathCoroutine;
    Player currentTarget;

    SplinePath path;

    #endregion

    #region Preallocation
    Vector2 inputDirection = Vector2.zero;
    float splineProgress;
    #endregion

    #region Properties
    #endregion
    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponent<Enemy>();
        
    }
    void SetupSplines()
    {
        switch (enemyPathType)
        {
            case EnemyPathType.ToPlayer:
            case EnemyPathType.NoPath:
            case EnemyPathType.Random:  // TODO setup random movement
                return;
            case EnemyPathType.LeaveRoom:
                splineContainer = enemyPathsManager.LeaveRoomSplines;
                break;
            case EnemyPathType.RoomOrbit:
                splineContainer = enemyPathsManager.RoomOrbitSplines;
                break;
            
        }

        if (splineContainer == null) return;
        if (splineContainer.Splines.Count <= 0) return;

        Matrix4x4 containerTransform = splineContainer.transform.localToWorldMatrix;

        Spline selectedSpline = splineContainer.Splines[Random.Range(0, splineContainer.Splines.Count)];

        path = new SplinePath(new[]
        {
            new SplineSlice<Spline>(selectedSpline, new SplineRange(0, selectedSpline.Count), containerTransform)
        });

        SplineUtility.GetNearestPoint(path, transform.position, out float3 nearest, out float progress);
        splineProgress = progress;
    }

    void SetupAgent()
    {

        agent = GetComponent<NavMeshAgent>();

        // Manual movement mode: agent does NOT move or rotate the transform
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = true; // keep this true for normal humanoids

        StartCoroutine(MoveToNavMesh());

        agent.nextPosition = transform.position;
    }
    IEnumerator MoveToNavMesh()
    {
        if(!IsServer) yield break;

        yield return null;
        // Auto-align agent to navmesh height
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            agent.nextPosition = transform.position;
        }
        else
        {
            Debug.LogError($"{name}: Could not find navmesh under enemy {name}!", this);
        }
    }
    protected override void Start()
    {
        base.Start();

        enemyPathsManager = enemy.ParentSpawner.GetComponentInChildren<EnemyPathsManger>();

        SetupSplines();

        SetupAgent();

    }
    protected override void Update()
    {
        if (!IsServer) return;
        base.Update();

        if (enemy == null) return;
        if (enemy.IsDead) return;

        Attack();
        SetLookInput();
    }
    void Attack()
    {
        

        if (!enemy.ReadyToFire) return;
        switch (attackTargetType)
        {
            case AttackTargetType.Nothing:
                return;
            case AttackTargetType.Forward:
                RequestFireWeapon();
                break;
            case AttackTargetType.Player:
                if (currentTarget == null) return;
                RequestFireWeapon();
                break;

        }


    }
    void SetLookInput()
    {
        switch (attackTargetType)
        {
            case AttackTargetType.Nothing:
            case AttackTargetType.Forward:
                return;
            case AttackTargetType.Player:
                lookInput = Vector2.zero;

                if (currentTarget == null) return;
                Vector3 lookInput3D = currentTarget.transform.position - transform.position;

                lookInput.x = lookInput3D.x;
                lookInput.y = lookInput3D.z;

                break;

        }
    }
    protected override void FixedUpdate()
    {
        if (!IsServer) return;
        base.FixedUpdate();
        SetMoveInput();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        repathCoroutine = StartCoroutine(RepathLoop());
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if(repathCoroutine != null) StopCoroutine(repathCoroutine);
    }
    IEnumerator RepathLoop()
    {
        if (!IsServer) yield break;

        while (true)
        {
            yield return new WaitForSeconds(repathInterval);
            RepathImmediate();
        }
    }
    /// <summary>
    /// Call externally to trigger an enemy to immediately change target, providing a player as a target, or leaving blank to find nearest player.
    /// </summary>
    /// <param name="aggroTarget"></param>
    public void RepathImmediate(Player aggroTarget = null)
    {
        Player closestPlayer = null;

        if (aggroTarget == null)
        {
            TryFindNearestPlayer(out closestPlayer);
        } 
        
        currentTarget = aggroTarget == null ? closestPlayer : aggroTarget;
        
        switch (enemyPathType)
        {
            case EnemyPathType.ToPlayer:
                if (currentTarget)  MoveTo(currentTarget.transform.position);
                else                MoveTo(transform.position);
                break;
            case EnemyPathType.NoPath:
                MoveTo(transform.position);
                break;
            case EnemyPathType.Random:  // TODO setup random movement
                Debug.LogError($"{name}: Random move direction not implemented yet, please select another type", this);
                return;
            case EnemyPathType.LeaveRoom:
            case EnemyPathType.RoomOrbit:
                MoveTo(GetPositionOnSplinePath());
                break;

        }
           
        
    }
    void SetMoveInput()
    {
        

        inputDirection = GetMoveDirection();

        moveDirection.x = inputDirection.x;
        moveDirection.y = 0;
        moveDirection.z = inputDirection.y;
    }
    Vector3 GetPositionOnSplinePath()
    {
        SplineUtility.GetNearestPoint(path, transform.position, out float3 nearest, out float progressAtCurrentPoint);
        progressAtCurrentPoint += SPLINE_PROGRESS_INCREMENT;
        while (progressAtCurrentPoint > 1)
        {
            progressAtCurrentPoint--;
        }

        Vector3 newTarget = path.EvaluatePosition(progressAtCurrentPoint);

        Debug.Log($"{name} moving from {transform.position} to {newTarget}. CurrentProgress {progressAtCurrentPoint}");

        return newTarget;

    }
    Vector2 GetMoveDirection()
    {
        if (IsDead) return Vector2.zero;
        // If agent has no path yet (or path is done), no movement
        if (!agent.hasPath) return Vector2.zero;

        // If we are at our destination, no move direction.
        if(HasReachedDestination()) return Vector2.zero;

        // Primary option: use desiredVelocity (already a good steering vector)
        Vector3 dir = agent.desiredVelocity;
        dir.y = 0f;

        dir.Normalize();
        return new Vector2(dir.x, dir.z);
    }
    bool MoveTo(Vector3 targetPos)
    {
        if (IsDead) return false;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name}: NavMeshAgent is not on a NavMesh. Cannot MoveTo().", this);
            return false;
        }
       
        bool success = agent.SetDestination(targetPos);

        if (success) agent.isStopped = false;

        return success;
    }
    bool TryFindNearestPlayer(out Player closestPlayer)
    {
        closestPlayer = null;

        if (Player.AllPlayers.Count == 0) return false;

        float distance = float.MaxValue;

        foreach (Player p in Player.AllPlayers)
        {
            float distanceToPlayer = Vector3.Distance(p.transform.position, transform.position);
            if (distanceToPlayer < distance)
            {
                distance = distanceToPlayer;
                closestPlayer = p;
            }
        }

        return true;
    }
    void LateUpdate()
    {
        if (!IsServer) return;

        if (!agent.isOnNavMesh) return;

        agent.nextPosition = transform.position;
    }
    /// <summary>
    /// True when we've effectively arrived at the agent destination.
    /// This is used by EnemyController for patrol / walk logic.
    /// </summary>
    public bool HasReachedDestination()
    {
        if (IsDead) return true;

        if (agent.pathPending) return false;

        // remainingDistance is valid even when updatePosition=false (it uses internal nextPosition)
        return agent.hasPath && agent.remainingDistance <= agent.stoppingDistance;
    }

}
