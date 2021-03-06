﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 5f;

    private IMovementInput input;
    private BodyMovement body;

    void Awake()
    {
        input = GetComponent<IMovementInput>();
    }

    void Update()
    {
        Vector3 moveDirection = new Vector3(input.Horizontal, 0.0f, input.Vertical);

        if(moveDirection.sqrMagnitude > 0.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }
}
