using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttack : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayerMask = default;

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

    private float reloadTimer = 0.0f;
    private bool alreadyReloading = false;

    private PlayerInput input;
    private BodyMovement bodyMovement;

    private RaycastHit[] rangedHits = new RaycastHit[20];
    private int shotsLeft = 6;
   
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        bodyMovement = GetComponent<BodyMovement>();
    }

    void DoRangedAttack()
    {
        if (shotsLeft > 0)
        {
            screenShake.AddTrauma(screenShakeTraumaPerShot);
            int rangedHitCount = Physics.RaycastNonAlloc(transform.position, input.LookAtPos - transform.position, rangedHits, Mathf.Infinity, enemyLayerMask);
            for (int i = 0; i < rangedHitCount; ++i)
            {
                Debug.Log($"Hit collider {rangedHits[i].collider.gameObject.name} with a ranged attach");
                var health = rangedHits[i].collider.GetComponent<HealthSystem>();
                if (health)
                {
                    health.Kill();
                }
            }

            RaycastHit trailHit;
            if (Physics.Raycast(transform.position, input.LookAtPos - transform.position, out trailHit, Mathf.Infinity, wallLayerMask))
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
