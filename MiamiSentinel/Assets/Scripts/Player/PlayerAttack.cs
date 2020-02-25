using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayerMask = default;

    [Header("Melee Attack")]
    [SerializeField]
    private float attackCooldown = 0.5f;
    [SerializeField]
    private float attackRadius = 1f;
    [SerializeField]
    private float attackAngle = 90f;
    [SerializeField]
    private float stunDuration = 0.3f;

    private float cooldownTimer = 0.0f;
    private bool canAttack = true;

    private PlayerInput input;

    private float minDotProduct;
    private Collider[] hitColliders = new Collider[20];
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        OnValidate();
    }
   
    void OnValidate()
    {
        minDotProduct = Mathf.Cos(attackAngle * Mathf.Deg2Rad / 2);
    }


    void Update()
    {
        if (!canAttack)
        {
            cooldownTimer += Time.deltaTime;
            if(cooldownTimer >= attackCooldown)
            {
                canAttack = true;
                cooldownTimer = 0.0f;
            }
        }
    }

    void DoWeakAttack()
    {
        if (canAttack)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, hitColliders, enemyLayerMask);
            for (int i = 0; i < hitCount; ++i)
            {
                Vector3 vectorToCollider = (hitColliders[i].transform.position - transform.position).normalized;

                if (Vector3.Dot(vectorToCollider, transform.forward) > minDotProduct)
                {
                    Debug.Log($"Hit collider {hitColliders[i].gameObject.name} with a weak attack");
                    var health = hitColliders[i].GetComponent<HealthSystem>();
                    if (health)
                    {
                        health.Damage(1);
                        health.Stun(stunDuration);
                    }
                }
            }
            canAttack = false;
        }
    }
    
    void OnEnable()
    {
        input.OnMeleeAttack += DoWeakAttack;
    }
      
    void OnDisable()
    {
        input.OnMeleeAttack -= DoWeakAttack;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        GizmoExtensions.DrawCircle(transform.position, attackRadius, 30);

        Gizmos.color = Color.cyan;
        Vector3 anglePoint1 = transform.rotation * new Vector3(attackRadius * Mathf.Sin(attackAngle / 2 * Mathf.Deg2Rad), 0f, attackRadius * Mathf.Cos(attackAngle / 2 * Mathf.Deg2Rad));
        Vector3 anglePoint2 = transform.rotation * new Vector3(attackRadius * Mathf.Sin(-attackAngle / 2 * Mathf.Deg2Rad), 0f, attackRadius * Mathf.Cos(-attackAngle / 2 * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, anglePoint1 + transform.position);
        Gizmos.DrawLine(transform.position, anglePoint2 + transform.position);
    }

    
}
