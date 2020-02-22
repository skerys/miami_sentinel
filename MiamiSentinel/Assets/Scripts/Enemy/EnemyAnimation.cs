using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private IMovementInput input;

    void Awake()
    {
        input = GetComponent<IMovementInput>();
    }

    void Update()
    {
        Vector3 moveDirection = new Vector3(input.Horizontal, 0.0f, input.Vertical);
        transform.LookAt(transform.position + moveDirection, Vector3.up);
    }
}
