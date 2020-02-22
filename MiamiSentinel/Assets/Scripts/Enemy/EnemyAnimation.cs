using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 5f;

    private IMovementInput input;

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

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Mathf.Rad2Deg * Time.deltaTime);
        }

    }
}
