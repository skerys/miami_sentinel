using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowerMovement : MonoBehaviour, IMovementInput
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    [SerializeField]
    private float searchRange = 10f;
    [SerializeField]
    private LayerMask playerLayerMask = default;
    [Header("Local Avoidance")]
    [SerializeField]
    private bool enableLocalAvoidance = true;
    [SerializeField]
    private float localAvoidanceSearchRange = 2f;
    [SerializeField]
    private LayerMask enemyLayerMask = default;
    [SerializeField]
    private float localAvoidanceFactor = 0.3f;

    private GameObject targetTransform;

    void Awake()
    {
        Horizontal = 0.0f;
        Vertical = 0.0f;
    }

    void Update()
    {
        if (targetTransform)
        {
            FollowPlayer();
            if(enableLocalAvoidance) LocalAvoidance();
        }
        else
        {
            SearchForPlayer();
        }
    }

    void LocalAvoidance()
    {
        Collider[] foundEnemies = Physics.OverlapSphere(transform.position, localAvoidanceSearchRange, enemyLayerMask);
        for(int i = 0; i < foundEnemies.Length; ++i)
        {
            Vector3 awayFromEnemy = transform.position - foundEnemies[i].transform.position;
            awayFromEnemy = awayFromEnemy.normalized * (localAvoidanceFactor / foundEnemies.Length);

            Horizontal += awayFromEnemy.x;
            Vertical += awayFromEnemy.z;
        }
    }

    void SearchForPlayer()
    {
        Collider[] foundPlayers = Physics.OverlapSphere(transform.position, searchRange, playerLayerMask);
        if (foundPlayers.Length > 0)
        {
            if (foundPlayers.Length > 1)
            {
                Debug.LogError("More than one player found by enemy", this);
                return;
            }

            targetTransform = foundPlayers[0].gameObject;
        }
    }

    void FollowPlayer()
    {
        Vector3 directionToPlayer = targetTransform.transform.position - transform.position;
        directionToPlayer.Normalize();

        Debug.DrawLine(transform.position, transform.position + directionToPlayer * 3f);

        Horizontal = directionToPlayer.x;
        Vertical = directionToPlayer.z;
    }
    


}
