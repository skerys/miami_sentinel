using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private float attackCooldown = 2f;
    [SerializeField]
    private float attackRadius = 0.5f;
    [SerializeField]
    private float attackAngle = 90f;
    [SerializeField]
    private LayerMask playerLayerMask = default;
    [SerializeField]
    private float attackTelegraphTime = 1f;

    private float cooldownTimer = 0.0f;
    private bool canAttack = true;

    private float prepareAttackTimer = 0.0f;

    private float minDotProduct;
    private IEnemyAI enemyAI;

    void Awake()
    {
        enemyAI = GetComponent<IEnemyAI>();
        enemyAI.SetAttackRange(attackRadius);
        OnValidate();
    }

    void OnValidate()
    {
        minDotProduct = Mathf.Cos(attackAngle * Mathf.Deg2Rad / 2);
    }

    void StartAttack()
    {
        if(canAttack)
        {
            prepareAttackTimer = attackTelegraphTime;
            canAttack = false;
            enemyAI.DisableAI();
            Debug.Log($"{gameObject.name} is preparing to attack.");
        }
    }

    void ExecuteAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerLayerMask);
        foreach(var collider in hits)
        {
            Vector3 vectorToCollider = (collider.transform.position - transform.position).normalized;

            if(Vector3.Dot(vectorToCollider, transform.forward) > minDotProduct)
            {
                Debug.Log($"{gameObject.name} hits player ({collider.gameObject.name})");
            }
        }
    }

    void Update()
    {
        if (!canAttack)
        {
            if(prepareAttackTimer > 0.0f)
            {
                prepareAttackTimer -= Time.deltaTime;
                if(prepareAttackTimer <= 0.0f)
                {
                    ExecuteAttack();
                    cooldownTimer = attackCooldown;
                    enemyAI.EnableAI();
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmoExtensions.DrawCircle(transform.position, attackRadius, 20);

        Gizmos.color = Color.yellow;
        Vector3 anglePoint1 = transform.rotation * new Vector3(attackRadius * Mathf.Sin(attackAngle / 2 * Mathf.Deg2Rad), 0f, attackRadius * Mathf.Cos(attackAngle / 2 * Mathf.Deg2Rad));
        Vector3 anglePoint2 = transform.rotation * new Vector3(attackRadius * Mathf.Sin(-attackAngle / 2 * Mathf.Deg2Rad), 0f, attackRadius * Mathf.Cos(-attackAngle / 2 * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, anglePoint1 + transform.position);
        Gizmos.DrawLine(transform.position, anglePoint2 + transform.position);
    }


}
