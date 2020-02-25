using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DashMode
{
    TowardsMouse,
    TowardsVelocity
};

public class PlayerDash : MonoBehaviour
{
    [SerializeField]
    private DashMode dashMode;

    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashCooldown;
    

    private float dashTimeLeft;
    private float cooldownTimer;

    private bool dashInProgress = false;
    [HideInInspector]
    public bool canDash = true;

    private PlayerInput input;
    private BodyMovement bodyMovement;
    private PlayerRangedAttack rangedAttack;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        bodyMovement = GetComponent<BodyMovement>();
        rangedAttack = GetComponent<PlayerRangedAttack>();
    }

    void StartDash()
    {
        if (canDash)
        {
            canDash = false;
            dashInProgress = true;
            dashTimeLeft = dashTime;

            Vector3 dashVelocity = default;
            if (dashMode == DashMode.TowardsMouse)
            {
                Vector3 towardsLookPos = input.LookAtPos - transform.position;
                dashVelocity = towardsLookPos.normalized * dashSpeed;
            }
            else if (dashMode == DashMode.TowardsVelocity)
            {
                dashVelocity = bodyMovement.Velocity.normalized * dashSpeed;
            }

            bodyMovement.Velocity = dashVelocity;
            bodyMovement.SetVelocityChangeActive(false);

            rangedAttack.ReloadEnd();
            rangedAttack.canReload = false;
        }
    }

    void DashUpdate()
    {
        dashTimeLeft -= Time.deltaTime;
        if(dashTimeLeft <= 0.0f)
        {
            EndDash();
        }
    }

    void EndDash()
    {
        dashInProgress = false;
        bodyMovement.SetVelocityChangeActive(true);
        cooldownTimer = dashCooldown;
        rangedAttack.canReload = true;
    }

    void Update()
    {
        if (dashInProgress) {
            DashUpdate();
        }
        else if (!canDash)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0.0f)
            {
                canDash = true;
            }
        }
    }

    void OnEnable()
    {
        input.OnDash += StartDash;
    }
    
    void OnDisable()
    {
        input.OnDash -= StartDash;
    }
}
