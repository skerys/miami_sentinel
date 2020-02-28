using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : MonoBehaviour, IEnemyAttack
{

    [SerializeField]
    float attackCooldown = 3f;
    [SerializeField]
    float attackRange = 10f;
    [SerializeField]
    float attackTelegraphTime = 1f;
    [SerializeField]
    Projectile projectilePrefab = default;

    private float cooldownTimer = 0.0f;
    private bool canAttack = true;

    private float prepareAttackTimer = 0.0f;

    private IEnemyAI enemyAI;
    private IMovementInput movementInput;

    private Collider myCollider;

    void Awake()
    {
        enemyAI = GetComponent<IEnemyAI>();
        movementInput = GetComponent<IMovementInput>();

        myCollider = GetComponent<Collider>();
    }

    void StartAttack()
    {
        if(canAttack)
        {
            prepareAttackTimer = attackTelegraphTime;
            canAttack = false;
            movementInput.DisableInput();
            Debug.Log($"{gameObject.name} is preparing to attack.");
        }
    }

    void ExecuteAttack()
    {
        Vector3 projectileDirection = enemyAI.TargetTransform.position - transform.position;

        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.SetProjectileDirection(projectileDirection.normalized);
        projectile.SetCreatorCollider(myCollider);
    }

    void Update()
    {
        if (!canAttack)
        {
            if (prepareAttackTimer > 0.0f)
            {
                prepareAttackTimer -= Time.deltaTime;
                if(prepareAttackTimer <= 0.0f)
                {
                    ExecuteAttack();
                    cooldownTimer = attackCooldown;
                    movementInput.EnableInput();
                }
            }

            if(cooldownTimer > 0.0f)
            {
                cooldownTimer -= Time.deltaTime;
                if(cooldownTimer <= 0.0f)
                {
                    canAttack = true;
                }
            }
        }
    }

    void OnEnable()
    {
        enemyAI.OnAttack += StartAttack;
    }

    void OnDisable()
    {
        enemyAI.OnAttack -= StartAttack;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmoExtensions.DrawCircle(transform.position, attackRange, 20);
    }
}
