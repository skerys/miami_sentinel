using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowerAI : MonoBehaviour, IMovementInput, IEnemyAI
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
    [SerializeField]
    private bool divideFactorAmongsAllAvoidables = true;

    private GameObject targetTransform;

    private bool isActive = true;

    public event Action OnAttack = delegate { };

    private float attackRange;

    void Awake()
    {
        Horizontal = 0.0f;
        Vertical = 0.0f;
    }

    void Update()
    {
        if (isActive)
        {
            if (targetTransform)
            {
                FollowPlayer();
                if(enableLocalAvoidance) LocalAvoidance();
                CheckForAttack();
            }
            else
            {
                SearchForPlayer();
            }
        }
        else
        {
            Vertical = 0.0f;
            Horizontal = 0.0f;
        }
    }

    void LocalAvoidance()
    {
        Collider[] foundEnemies = Physics.OverlapSphere(transform.position, localAvoidanceSearchRange, enemyLayerMask);
        for(int i = 0; i < foundEnemies.Length; ++i)
        {
            Vector3 awayFromEnemy = transform.position - foundEnemies[i].transform.position;
            float multiplier = divideFactorAmongsAllAvoidables ? (localAvoidanceFactor / foundEnemies.Length) : localAvoidanceFactor;
            awayFromEnemy = awayFromEnemy.normalized * multiplier;

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

    void CheckForAttack()
    {
        Vector3 vectorToPlayer = targetTransform.transform.position - transform.position;
        if(vectorToPlayer.magnitude <= attackRange)
        {
            OnAttack();
        }
    }

    public void EnableAI()
    {
        isActive = true;
    }

    public void DisableAI()
    {
        isActive = false;
    }

    public bool IsEnabled()
    {
        return isActive;
    }

    public void SetAttackRange(float range)
    {
        attackRange = range;
    }
}
