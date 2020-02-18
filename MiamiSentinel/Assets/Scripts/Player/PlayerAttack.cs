using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private float attackRadius = 1f;
    [SerializeField]
    private float attackAngle = 90f;
    [SerializeField]
    private LayerMask enemyLayerMask;

    private PlayerInput input;
    private float minDotProduct;
    private Collider[] hitColliders = new Collider[10];

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        minDotProduct = Mathf.Cos(attackAngle * Mathf.Deg2Rad / 2);
    }
   
    void OnValidate()
    {
        minDotProduct = Mathf.Cos(attackAngle * Mathf.Deg2Rad / 2);
    }

    void OnEnable()
    {
        input.OnAttack += DoWeakAttack;
    }
      
    void OnDisable()
    {
        input.OnAttack -= DoWeakAttack;
    }

    void DoWeakAttack()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, hitColliders, enemyLayerMask);
        for(int i = 0; i < hitCount; ++i)
        {
            Vector3 vectorToCollider = (hitColliders[i].transform.position - transform.position).normalized;

            if(Vector3.Dot(vectorToCollider, transform.forward) > minDotProduct)
            {
                Debug.Log($"Hit collider {hitColliders[i].gameObject.name} with a weak attack");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float corners = 30; // How many corners the circle should have
        float size = attackRadius; // How wide the circle should be
        Vector3 origin = transform.position; // Where the circle will be drawn around
        Vector3 startRotation = transform.right * size; // Where the first point of the circle starts
        Vector3 lastPosition = origin + startRotation;
        float angle = 0;
        while (angle <= 360)
        {
            angle += 360 / corners;
            Vector3 nextPosition = origin + (Quaternion.Euler(0, angle, 0) * startRotation);
            Gizmos.DrawLine(lastPosition, nextPosition);
            //Gizmos.DrawSphere(nextPosition, 1);

            lastPosition = nextPosition;
        }
        Gizmos.color = Color.cyan;
        Vector3 anglePoint1 = transform.rotation * new Vector3(attackRadius * Mathf.Sin(attackAngle / 2 * Mathf.Deg2Rad), 0f, attackRadius * Mathf.Cos(attackAngle / 2 * Mathf.Deg2Rad));
        Vector3 anglePoint2 = transform.rotation * new Vector3(attackRadius * Mathf.Sin(-attackAngle / 2 * Mathf.Deg2Rad), 0f, attackRadius * Mathf.Cos(-attackAngle / 2 * Mathf.Deg2Rad));
        Gizmos.DrawLine(transform.position, anglePoint1 + transform.position);
        Gizmos.DrawLine(transform.position, anglePoint2 + transform.position);
    }

    
}
