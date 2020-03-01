using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerRangedAttack : MonoBehaviour
{
    [Header("Layer Masks")]
    [SerializeField]
    private LayerMask enemyLayerMask = default;
    [SerializeField]
    private LayerMask shieldLayerMask = default;
    [SerializeField]
    private LayerMask wallLayerMask = default;

    [Header("Reloading")]
    [SerializeField]
    private float timeToReload = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float speedModifierOnReload = 0.5f;
    [Header("Effects")]
    [SerializeField]
    private BulletTrail bulletTrailPrefab = default;
    [SerializeField]
    private BulletTrail bulletTrailPrefabPiercing = default;
    [SerializeField]
    private float timeToChargeEffectStart = 0.1f;
    [SerializeField]
    private GameObject chargeEffect;
    [SerializeField]
    private GameObject aimingEffect;
    [SerializeField]
    private ScreenShakeManager screenShake = default;
    [SerializeField]
    private float screenShakeTraumaPerShot = 0.2f;
    [SerializeField]
    private float screenShakeTraumaPerPiercingShot = 0.6f;

    [Space]
    [SerializeField]
    private int bounceLimit = 3;
    [SerializeField]
    private float timeToCharge = 0.5f;

    private float chargeTimer= 0.0f;
    private bool chargeEffectStarted = false;
    private bool isPiercing = false;

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

    void ChargeRangedAttack()
    {
        if (shotsLeft <= 0) return;

        if(chargeTimer < timeToCharge)
        {
            chargeTimer += Time.deltaTime;

            if(!chargeEffectStarted && chargeTimer >= timeToChargeEffectStart)
            {
                chargeEffect.SetActive(true);
                var particleSystems = chargeEffect.GetComponentsInChildren<ParticleSystem>();
                foreach(var system in particleSystems)
                {
                    system.Play();
                }
                chargeEffectStarted = true;
            }

            if(chargeTimer >= timeToCharge)
            {
                isPiercing = true;
                chargeEffect.SetActive(false);
                aimingEffect.SetActive(true);
                var particleSystems = aimingEffect.GetComponentsInChildren<ParticleSystem>();
                foreach (var system in particleSystems)
                {
                    system.Play();
                }
            }
        }
    }

    void DoRangedAttack()
    {
        if (shotsLeft > 0)
        {
            List<Vector3> linePositions = new List<Vector3>();
            linePositions.Add(transform.position);

            nextRay.origin = transform.position;
            nextRay.direction = input.LookAtPos - transform.position;

            float trauma = isPiercing ? screenShakeTraumaPerPiercingShot : screenShakeTraumaPerShot;
            screenShake.AddTrauma(trauma);

            int bounces = 0;
            int rangedHitCount = 0;

            while(bounces < bounceLimit)
            {
                if(isPiercing)
                {
                    rangedHitCount = Physics.RaycastNonAlloc(nextRay, rangedHits, Mathf.Infinity, rayMask);
                    if (rangedHitCount == 0) { break; }

                    Array.Sort(rangedHits, 0, rangedHitCount, comparer);
                }
                else
                {
                    Array.Clear(rangedHits, 0, rangedHits.Length);
                    rangedHitCount = Physics.Raycast(nextRay, out rangedHits[0], Mathf.Infinity, rayMask) ? 1 : 0;
                    if (rangedHitCount == 0) { break; }
                }
                

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
                        if (!isPiercing)
                        {
                            linePositions.Add(rangedHits[i].point);
                        }
                    }
                }

                if (!hasBounced) break;
            
            }
            var bulletTrail = isPiercing ? Instantiate(bulletTrailPrefabPiercing) : Instantiate(bulletTrailPrefab);
            bulletTrail.SetTransform(transform);

            Debug.Log($"ranged hit count {rangedHitCount}");

            if(isPiercing || rangedHitCount == 0)
            {
                RaycastHit trailHit;
                if (Physics.Raycast(nextRay, out trailHit, Mathf.Infinity, wallLayerMask))
                {
                    linePositions.Add(trailHit.point);
                }
                else
                {
                    linePositions.Add(30f * (input.LookAtPos - transform.position));
                }
            }
         
            bulletTrail.SetPositions(linePositions.ToArray());
            shotsLeft--;

            isPiercing = false;
            chargeTimer = 0.0f;
            chargeEffectStarted = false;
            chargeEffect.SetActive(false);
            aimingEffect.SetActive(false);

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
        if (alreadyReloading)
        {
            bodyMovement.ModifySpeed(1.0f / speedModifierOnReload);
        }
        alreadyReloading = false;

        dash.canDash = true;
    }

    void OnEnable()
    {
        input.OnRangedAttack += ChargeRangedAttack;
        input.OnRangedAttackRelease += DoRangedAttack;
        input.OnReload += ReloadUpdate;
        input.OnReleaseReload += ReloadEnd;
    }

    void OnDisable()
    {
        input.OnRangedAttack -= ChargeRangedAttack;
        input.OnRangedAttackRelease -= DoRangedAttack;
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
