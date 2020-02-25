using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 20f)]
    private float maxSpeed = 5f;

    [SerializeField, Range(0f, 100f)]
    private float maxAcceleration = 5f;

    private IMovementInput input;
    private Rigidbody body;

    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

    private Vector3 velocity;
    private Vector3 targetVelocity;

    private bool velocityChangeActive = true;

    void Awake()
    {
        input = GetComponent<IMovementInput>();
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        targetVelocity = new Vector3(input.Horizontal, 0.0f, input.Vertical);
        targetVelocity = Vector3.ClampMagnitude(targetVelocity, 1.0f);

        targetVelocity *= maxSpeed;
    }

    void FixedUpdate()
    {
        if(velocityChangeActive)
        {
            velocity = body.velocity;

            float maxSpeedChange = maxAcceleration * Time.deltaTime;

            velocity.x = Mathf.MoveTowards(body.velocity.x, targetVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(body.velocity.z, targetVelocity.z, maxSpeedChange);
        }
        body.velocity = velocity;
    }

    public void SetVelocityChangeActive(bool b)
    {
        velocityChangeActive = b;
    }

    public void ModifySpeed(float multiplier)
    {
        maxSpeed *= multiplier;
    }

}
