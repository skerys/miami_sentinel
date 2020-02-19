using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 20f)]
    private float maxSpeed = 5f;

    [SerializeField, Range(0f, 100f)]
    private float maxAcceleration = 5f;

    private IMovementInput input;
    private Rigidbody body;

    private Vector3 velocity;
    private Vector3 targetVelocity;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
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
        velocity = body.velocity;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(body.velocity.x, targetVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(body.velocity.z, targetVelocity.z, maxSpeedChange);
        body.velocity = velocity;
    }
}
