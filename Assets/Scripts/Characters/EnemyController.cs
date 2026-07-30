using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BMD.CharacterController
{
    #region Configuration
    [SerializeField] float repathInterval = 0.1f;
    #endregion

    #region Cached references
    private NavMeshAgent agent;
    Enemy enemy;
    #endregion

    #region Runtime Variables
    Coroutine repathCoroutine;
    #endregion

    #region Preallocation
    Vector2 inputDirection = Vector2.zero;
    
    #endregion

    #region Properties
    #endregion
    protected override void Awake()
    {
        base.Awake();

        SetupAgent();
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
    /// <param name="target"></param>
    public void RepathImmediate(Player target = null)
    {
        if (target == null)
        {
            if (TryFindNearestPlayer(out Player closestPlayer)) 
                MoveTo(closestPlayer.transform.position);
        }
        else
        {
            MoveTo(target.transform.position);
        }
        
    }

    void SetMoveInput()
    {
        inputDirection = GetMoveDirection();
        moveDirection.x = inputDirection.x;
        moveDirection.y = 0;
        moveDirection.z = inputDirection.y;
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
                distanceToPlayer = distance;
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
