using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWanderAI : MonoBehaviour, IMovementInput, IEnemyAI
{
    [SerializeField]
    private float wanderTime = 2.0f;
    [SerializeField]
    private float waitTime = 1.0f;
    [SerializeField]
    private LayerMask wallMask = default;
    [SerializeField]
    private float minWanderDistance = 3.0f;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public Transform TargetTransform { get; private set; }

    public event Action OnAttack = delegate { };
    private bool isActive = true;

    private float moveTimer = 0.0f;
    private float waitingTimer = 0.0f;
    private bool isWaiting = false;

    private Vector2 currentDirection;

    void Awake()
    {
        Horizontal = 0.0f;
        Vertical = 0.0f;

        currentDirection = FindNewDirection();
    }

    void Update()
    {
        if (!isWaiting)
        {
            //Currently moving in currentDirection
            moveTimer += Time.deltaTime;
            Horizontal = currentDirection.x;
            Vertical = currentDirection.y;

            if(moveTimer >= wanderTime)
            {
                isWaiting = true;
                moveTimer = 0.0f;
            }
        }
        else
        {
            //Currently waiting
            waitingTimer += Time.deltaTime;
            Horizontal = 0.0f;
            Vertical = 0.0f;

            if(waitingTimer >= waitTime)
            {
                isWaiting = false;
                waitingTimer = 0.0f;
                currentDirection = FindNewDirection();
            }
        }
    }

    Vector2 FindNewDirection()
    {
        Vector2 tempDir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 rayDir = new Vector3(tempDir.x, 0.0f, tempDir.y);
        int iterationCount = 0;


        while(Physics.Raycast(transform.position, rayDir, minWanderDistance, wallMask))
        {
            tempDir = UnityEngine.Random.insideUnitCircle.normalized;
            rayDir = new Vector3(tempDir.x, 0.0f, tempDir.y);
            iterationCount++;
            if (iterationCount > 5) break; //To avoid infinite loop situations
        }

        return tempDir;
    }

    public void DisableInput()
    {
        isActive = true;
    }

    public void EnableInput()
    {
        isActive = false;
    }

    public bool IsEnabled()
    {
        return isActive;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        GizmoExtensions.DrawCircle(transform.position, minWanderDistance, 20);
    }
}
