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
    [SerializeField] private float timeToReload = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)] private float speedModifierOnReload = 0.5f;
    [SerializeField] private int maxShotsLeft = 6;

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
    private GameObject hitEffect = default;
    [SerializeField]
    private ScreenShakeManager screenShake = default;
    [SerializeField]
    private float screenShakeTraumaPerShot = 0.2f;
    [SerializeField]
    private float screenShakeTraumaPerPiercingShot = 0.6f;
    [SerializeField] private float hitStopAmount = 0.02f;

    [Space]
    [SerializeField]
    private int bounceLimit = 3;
    [SerializeField]
    private float timeToCharge = 0.5f;
    [SerializeField]
    private float sphereCastRadius = 0.3f;

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
    private bool wallBounce;

    private RaycastHitAscendingDistanceComparer comparer;

    Ray nextRay;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        bodyMovement = GetComponent<BodyMovement>();
        dash = GetComponent<PlayerDash>();

        rayMask = enemyLayerMask | shieldLayerMask | wallLayerMask;
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

    void BounceShot(RaycastHit hit, ref List<Vector3> linePositions)
    {
        Vector3 newDir = Vector3.Reflect(nextRay.direction, hit.normal);
        nextRay.origin = hit.point;
        nextRay.direction = newDir;
        hasBounced = true;

        linePositions.Add(hit.point);
        var hitEffectObj = Instantiate(hitEffect, hit.point, Quaternion.identity);
        hitEffectObj.transform.LookAt(hit.point + hit.normal);
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
            //Only bounce off wall when shot is charged
            wallBounce = isPiercing;

            while(bounces < bounceLimit)
            {
                if(isPiercing)
                {
                    rangedHitCount = Physics.SphereCastNonAlloc(nextRay, sphereCastRadius, rangedHits, Mathf.Infinity, rayMask);
                    if (rangedHitCount == 0) { break; }

                    Array.Sort(rangedHits, 0, rangedHitCount, comparer);
                }
                else
                {
                    Array.Clear(rangedHits, 0, rangedHits.Length);
                    rangedHitCount = Physics.SphereCast(nextRay, sphereCastRadius, out rangedHits[0], Mathf.Infinity, rayMask) ? 1 : 0;
                    if (rangedHitCount == 0) { break; }
                }
                

                for (int i = 0; i < rangedHitCount; ++i)
                {
                    if (shieldLayerMask == (shieldLayerMask | (1 << rangedHits[i].collider.gameObject.layer)))
                    {
                        bounces++;
                        BounceShot(rangedHits[i], ref linePositions);
                        break;
                    }
                    else if( (wallLayerMask == (wallLayerMask | (1 << rangedHits[i].collider.gameObject.layer))) )
                    {
                        if(wallBounce)
                        {
                            bounces++;
                            wallBounce = false;
                            BounceShot(rangedHits[i], ref linePositions);
                        }
                        else
                        {
                            linePositions.Add(rangedHits[i].point);
                            var hitEffectObj = Instantiate(hitEffect, rangedHits[i].point, Quaternion.identity);
                            hitEffectObj.transform.LookAt(rangedHits[i].point + rangedHits[i].normal);
                            hasBounced = false;
                        }
                        break;
                    }
                    else
                    {
                        hasBounced = false;
                    }

                    var health = rangedHits[i].collider.GetComponent<HealthSystem>();
                    if (health)
                    {
                        health.Kill();
                        var hitEffectObj = Instantiate(hitEffect, rangedHits[i].point, Quaternion.identity);
                        hitEffectObj.transform.LookAt(rangedHits[i].point + rangedHits[i].normal);
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

            //Do hitstop if an enemy was hit
            if(rangedHitCount > 0)
            {
                HitStopManager.Instance.HitStop(hitStopAmount);
            }

            bulletTrail.SetPositions(linePositions.ToArray());
            shotsLeft--;

            isPiercing = false;
            chargeTimer = 0.0f;
            chargeEffectStarted = false;
            chargeEffect.SetActive(false);
            aimingEffect.SetActive(false);
            wallBounce = true;
        }
    }

    void ReloadUpdate()
    {
        if(canReload)
        {
            if (shotsLeft == maxShotsLeft) return;

            if (reloadTimer >= timeToReload)
            {
                shotsLeft = maxShotsLeft;
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
