using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BMD.CharacterController
{
    #region Configuration
    #endregion

    #region Cached references
    private NavMeshAgent agent;
    #endregion

    #region Runtime Variables
    #endregion

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();
        moveDirection = Vector3.right;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Debug.Log(moveDirection);
    }
}
