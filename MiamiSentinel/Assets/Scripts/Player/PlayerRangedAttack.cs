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
    [SerializeField] private int maxShotsLeft = 9999;

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
    private int bounceLimit = 5;
    [SerializeField]
    private float timeToCharge = 0.5f;
    [SerializeField]
    private float initialSpherecastRadius = 0.01f;
    [SerializeField]
    private float afterBounceHelpAngle = 10f;

    private float chargeTimer= 0.0f;
    private bool chargeEffectStarted = false;
    private bool isPiercing = false;



    private PlayerInput input;
    private BodyMovement bodyMovement;
    private PlayerDash dash;

    private RaycastHit[] rangedHits = new RaycastHit[20];
    private int shotsLeft = 9999;


    private LayerMask rayMask;
    private bool hasBounced = false;
    private RaycastHitAscendingDistanceComparer comparer;

    Ray nextRay;

    public bool helperOnDebug = false;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        bodyMovement = GetComponent<BodyMovement>();
        dash = GetComponent<PlayerDash>();

        rayMask = shieldLayerMask | wallLayerMask;
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
        nextRay.direction = newDir.normalized;
        hasBounced = true;

        linePositions.Add(hit.point);
        var hitEffectObj = Instantiate(hitEffect, hit.point, Quaternion.identity);
        hitEffectObj.transform.LookAt(hit.point + hit.normal);
    }

    void DoRangedAttack()
    {
        if(shotsLeft <= 0) return;

        List<Vector3> linePositionsInitial = new List<Vector3>();
        List<Vector3> linePositionsBounced = new List<Vector3>();

        linePositionsInitial.Add(transform.position);

        nextRay.origin = transform.position;
        nextRay.direction = (input.LookAtPos - transform.position).normalized;

        int rangedHitCount;
        bool wallBounce = isPiercing;

        float coneDot = Mathf.Cos(afterBounceHelpAngle * Mathf.Deg2Rad);
        float helpAngle = afterBounceHelpAngle * Mathf.Deg2Rad;

        for(int i = 0; i < bounceLimit; i++)
        {
            bool firstRay = i == 0;
            
            var linePositions = firstRay ? linePositionsInitial : linePositionsBounced;
            rangedHitCount = 0;

            RaycastHit finalHit;
            //Shoot simple ray and check for walls/shields
            rangedHitCount = Physics.Raycast(nextRay, out finalHit, Mathf.Infinity, rayMask) ? 1 : 0;
            Debug.Log(i + " : " + rangedHitCount);
            Debug.Log(finalHit.point);
            
            //If it is a bounced shot, do the cone check and realign based on the least different angle enemy found
            if(!firstRay && helperOnDebug)
            {
                //Calculate optimal spehere Cast radius
                float hitToSurfaceAngle = (90f + Vector3.Angle(finalHit.normal, -nextRay.direction)) * Mathf.Deg2Rad;
                float maxDistanceToSurface = finalHit.distance * Mathf.Sin(hitToSurfaceAngle) / Mathf.Sin(Mathf.PI - hitToSurfaceAngle - helpAngle);
                float sphereCastRadius = maxDistanceToSurface * Mathf.Sin(helpAngle) / Mathf.Sin(Mathf.PI / 2f - helpAngle);

                //Backtrack ray origin by radius to hit avoid enemies being inside the initial sphere
                Debug.Log(nextRay.origin.ToString() + nextRay.direction.ToString() + sphereCastRadius.ToString());
                rangedHitCount = Physics.SphereCastNonAlloc(nextRay.origin - nextRay.direction * sphereCastRadius * 2f, sphereCastRadius, nextRay.direction, rangedHits, Mathf.Infinity, enemyLayerMask);

                //For each enemy, check if theyre actually in the cone by comparing dot product
                float bestCaseAngle = -1f;
                for(int j = 0; j < rangedHitCount; j++)
                {
                    Debug.Log("enemy hit on: " + rangedHits[j].collider.gameObject.name);
                    var dirToEnemy = (rangedHits[j].point - nextRay.origin).normalized;
                    var enemyDot = Vector3.Dot(nextRay.direction, dirToEnemy);
                    if(!firstRay && enemyDot < coneDot)
                    {
                        //Enemy is not within the cone
                        continue;
                    }
                    if(enemyDot > bestCaseAngle)
                    {
                        bestCaseAngle = enemyDot;
                        nextRay.direction = dirToEnemy;
                    }
                }
                //New final hit
                if(bestCaseAngle > -1f) rangedHitCount = Physics.Raycast(nextRay, out finalHit, Mathf.Infinity, rayMask) ? 1 : 0;
            }

            //Sphere cast for enemiessa
            if(isPiercing)
            {
                rangedHitCount = Physics.SphereCastNonAlloc(nextRay, initialSpherecastRadius, rangedHits, finalHit.distance, enemyLayerMask);
            }
            else
            {           
                rangedHitCount = Physics.SphereCast(nextRay, initialSpherecastRadius, out rangedHits[0], finalHit.distance, enemyLayerMask) ? 1 : 0; 
            }

            //For each enemy, reduce health
            for(int j = 0; j < rangedHitCount; j++)
            {
                var health = rangedHits[j].collider.GetComponent<HealthSystem>();
                if (!firstRay && health)
                {
                    health.Kill();
                    if(!isPiercing) finalHit = rangedHits[j];

                    var hitEffectObj = Instantiate(hitEffect, rangedHits[j].point, Quaternion.identity);
                    hitEffectObj.transform.LookAt(rangedHits[j].point + rangedHits[j].normal);
                }
            }
            //Do bounce if needed
            Debug.Log(finalHit.point);
            if (shieldLayerMask == (shieldLayerMask | (1 << finalHit.collider.gameObject.layer)))
            {
                BounceShot(finalHit, ref linePositions);
            }
            else if(wallBounce && (wallLayerMask == (wallLayerMask | (1 << finalHit.collider.gameObject.layer))) )
            { 
                wallBounce = false;
                BounceShot(finalHit, ref linePositions);   
            }
            else
            {
                linePositions.Add(finalHit.point);
                var hitEffectObj = Instantiate(hitEffect, finalHit.point, Quaternion.identity);
                hitEffectObj.transform.LookAt(finalHit.point + finalHit.normal);
                hasBounced = false;
            }

            if(!hasBounced) break;
        }

        //Populate effect data
        var bulletTrail = isPiercing ? Instantiate(bulletTrailPrefabPiercing) : Instantiate(bulletTrailPrefab);
        bulletTrail.SetTransform(transform);
        linePositionsBounced.Insert(0, linePositionsInitial[linePositionsInitial.Count - 1]);
        bulletTrail.SetPositionsInitial(linePositionsInitial.ToArray());
        bulletTrail.SetPositionsAfterBounce(linePositionsBounced.ToArray());

        //Screenshake
        float trauma = isPiercing ? screenShakeTraumaPerPiercingShot : screenShakeTraumaPerShot;
        screenShake.AddTrauma(trauma);

        //Hitstop

        //Decrease ammo and reset effects/charging
        shotsLeft--;
        isPiercing = false;
        chargeTimer = 0.0f;
        chargeEffectStarted = false;
        chargeEffect.SetActive(false);
        aimingEffect.SetActive(false);
    }
    
    void OnEnable()
    {
        input.OnRangedAttack += ChargeRangedAttack;
        input.OnRangedAttackRelease += DoRangedAttack;
    }

    void OnDisable()
    {
        input.OnRangedAttack -= ChargeRangedAttack;
        input.OnRangedAttackRelease -= DoRangedAttack;
    }

    public int GetBulletCount() { return shotsLeft; }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)) helperOnDebug = !helperOnDebug;
    }

}

public class RaycastHitAscendingDistanceComparer : IComparer<RaycastHit>
{
    public int Compare(RaycastHit hit1, RaycastHit hit2)
    {
        return hit1.distance.CompareTo(hit2.distance);
    }
}
