using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerRangedAttack : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayerMask = default;
    [SerializeField]
    private LayerMask shieldLayerMask = default;

    [SerializeField]
    private float timeToReload = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float speedModifierOnReload = 0.5f;
    [SerializeField]
    private BulletTrail bulletTrailPrefab = default;
    [SerializeField]
    private ScreenShakeManager screenShake = default;
    [SerializeField]
    private float screenShakeTraumaPerShot = 0.2f;
    [SerializeField]
    private LayerMask wallLayerMask = default;

    [SerializeField]
    private int bounceLimit = 3;

    private float reloadTimer = 0.0f;
    private bool alreadyReloading = false;

    private PlayerInput input;
    private BodyMovement bodyMovement;
    private PlayerDash dash;

    private RaycastHit[] rangedHits = new RaycastHit[20];
    private int shotsLeft = 6;

    [HideInInspector]
    public bool canReload = true;

    private LayerMask rayMask;
    private bool hasBounced = false;
    private int bounces = 0;

    private RaycastHitAscendingDistanceComparer comparer;

    Ray nextRay;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        bodyMovement = GetComponent<BodyMovement>();
        dash = GetComponent<PlayerDash>();

        rayMask = enemyLayerMask | shieldLayerMask;
        comparer = new RaycastHitAscendingDistanceComparer();
    }

    void DoRangedAttack()
    {
        if (shotsLeft > 0)
        {
            List<Vector3> linePositions = new List<Vector3>();
            linePositions.Add(transform.position);

            nextRay.origin = transform.position;
            nextRay.direction = input.LookAtPos - transform.position;

            screenShake.AddTrauma(screenShakeTraumaPerShot);
            bounces = 0;
            while(bounces < bounceLimit)
            {
              
                int rangedHitCount = Physics.RaycastNonAlloc(nextRay, rangedHits, Mathf.Infinity, rayMask);
                if(rangedHitCount == 0) { break; }

                Array.Sort(rangedHits, 0, rangedHitCount, comparer);

                for (int i = 0; i < rangedHitCount; ++i)
                {
                    //Debug.Log($"Shield layer mask value: {shieldLayerMask.value}\nHit collider layer value: {rangedHits[i].collider.gameObject.layer}");
                    if (shieldLayerMask == (shieldLayerMask | (1 << rangedHits[i].collider.gameObject.layer)))
                    {
                        Debug.Log($"Bounce no. {bounces} happened");
                        Vector3 newDir = Vector3.Reflect(nextRay.direction, rangedHits[i].normal);
                        nextRay.origin = rangedHits[i].point;
                        nextRay.direction = newDir;
                        bounces++;
                        hasBounced = true;

                        linePositions.Add(rangedHits[i].point);
                        break;
                    }
                    else
                    {
                        hasBounced = false;
                    }

                    Debug.Log($"Hit collider {rangedHits[i].collider.gameObject.name} with a ranged attach");
                    var health = rangedHits[i].collider.GetComponent<HealthSystem>();
                    if (health)
                    {
                        health.Kill();
                    }
                }

                if (!hasBounced) break;
            
            }
            

            RaycastHit trailHit;
            var bulletTrail = Instantiate(bulletTrailPrefab);
            bulletTrail.SetTransform(transform);
            if (Physics.Raycast(nextRay, out trailHit, Mathf.Infinity, wallLayerMask))
            {
                linePositions.Add(trailHit.point);
                bulletTrail.SetPositions(linePositions.ToArray());
            }
            else
            {
                linePositions.Add(30f * (input.LookAtPos - transform.position));
                bulletTrail.SetPositions(linePositions.ToArray());
            }
            shotsLeft--;
        }
    }

    void ReloadUpdate()
    {
        if(canReload)
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
                reloadTimer += Time.deltaTime;
                if (!alreadyReloading)
                {
                    //Started reloading
                    bodyMovement.ModifySpeed(speedModifierOnReload);
                    alreadyReloading = true;

                    dash.canDash = false;
                }
            }
        }
    }

    public void ReloadEnd()
    {
        if (shotsLeft == 6) return;

        reloadTimer = 0.0f;
        alreadyReloading = false;
        bodyMovement.ModifySpeed(1.0f / speedModifierOnReload);

        dash.canDash = true;
    }

    void OnEnable()
    {
        input.OnRangedAttack += DoRangedAttack;
        input.OnReload += ReloadUpdate;
        input.OnReleaseReload += ReloadEnd;
    }

    void OnDisable()
    {
        input.OnRangedAttack -= DoRangedAttack;
        input.OnReload -= ReloadUpdate;
        input.OnReleaseReload -= ReloadEnd;
    }

    public int GetBulletCount() { return shotsLeft; }
    public float GetReloadProgress() { return reloadTimer; }
    public float GetReloadTime() { return timeToReload; }

}

public class RaycastHitAscendingDistanceComparer : IComparer<RaycastHit>
{
    public int Compare(RaycastHit hit1, RaycastHit hit2)
    {
        return hit1.distance.CompareTo(hit2.distance);
    }
}
