using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee Attack")]
    [SerializeField]
    private float attackCooldown = 3f;
    [SerializeField]
    private float attackRadius = 1f;
    [SerializeField]
    private float attackAngle = 90f;
    [SerializeField]
    private LayerMask enemyLayerMask = default;

    [Header("Ranged Attack")]
    [SerializeField]
    private float timeToReload = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float speedModifierOnReload = 0.5f;
    [SerializeField]
    private BulletTrail bulletTrailPrefab = default;
    [SerializeField]
    private ScreenShakeManager screenShake = default;

    private float cooldownTimer = 0.0f;
    private bool canAttack = true;

    private float reloadTimer = 0.0f;
    private bool alreadyReloading = false;

    private PlayerInput input;
    private BodyMovement bodyMovement;

    private float minDotProduct;
    private Collider[] hitColliders = new Collider[20];
    private RaycastHit[] rangedHits = new RaycastHit[20];

    private int shotsLeft = 6;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        bodyMovement = GetComponent<BodyMovement>();
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
        if(canAttack)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, hitColliders, enemyLayerMask);
            for(int i = 0; i < hitCount; ++i)
            {
                Vector3 vectorToCollider = (hitColliders[i].transform.position - transform.position).normalized;

                if(Vector3.Dot(vectorToCollider, transform.forward) > minDotProduct)
                {
                    Debug.Log($"Hit collider {hitColliders[i].gameObject.name} with a weak attack");
                    var health = hitColliders[i].GetComponent<HealthSystem>();
                    if (health)
                    {
                        health.Damage(1);
                    }
                }
            }
            canAttack = false;
        }
    }

    void DoRangedAttack()
    {
        if (shotsLeft > 0)
        {
            screenShake.AddTrauma(0.2f);
            int rangedHitCount = Physics.RaycastNonAlloc(transform.position, input.LookAtPos - transform.position, rangedHits, Mathf.Infinity, enemyLayerMask);
            for(int i = 0; i < rangedHitCount; ++i)
            {
                Debug.Log($"Hit collider {rangedHits[i].collider.gameObject.name} with a ranged attach");
                var health = rangedHits[i].collider.GetComponent<HealthSystem>();
                if (health)
                {
                    health.Kill();
                }
            }

            RaycastHit trailHit;
            if (Physics.Raycast(transform.position, input.LookAtPos - transform.position, out trailHit, Mathf.Infinity))
            {
                var bulletTrail = Instantiate(bulletTrailPrefab);
                bulletTrail.SetPositions(transform.position, trailHit.point);
            }
            else
            {
                var bulletTrail = Instantiate(bulletTrailPrefab);
                bulletTrail.SetPositions(transform.position, 30f * (input.LookAtPos - transform.position));
            }
            shotsLeft--;
        }
    }

    void ReloadUpdate()
    {
        if (shotsLeft == 6) return;

        if (reloadTimer >= timeToReload)
        {
            shotsLeft++;
            reloadTimer = 0.0f;
        }
        else
        {
            //Currently reloading
            if (!alreadyReloading)
            {
                bodyMovement.ModifySpeed(speedModifierOnReload);
                alreadyReloading = true;
            }
            reloadTimer += Time.deltaTime;
        }
    }

    void ReloadEnd()
    {
        reloadTimer = 0.0f;
        alreadyReloading = false;
        bodyMovement.ModifySpeed(1.0f / speedModifierOnReload);
    }

    void OnEnable()
    {
        input.OnMeleeAttack += DoWeakAttack;
        input.OnRangedAttack += DoRangedAttack;
        input.OnReload += ReloadUpdate;
        input.OnReleaseReload += ReloadEnd;
    }
      
    void OnDisable()
    {
        input.OnMeleeAttack -= DoWeakAttack;
        input.OnRangedAttack -= DoRangedAttack;
        input.OnReload -= ReloadUpdate;
        input.OnReleaseReload -= ReloadEnd;
    }

    public int GetBulletCount() { return shotsLeft; }
    public float GetReloadProgress() { return reloadTimer; }
    public float GetReloadTime() { return timeToReload; }

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
