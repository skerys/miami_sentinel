using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private KeyCode attackKey;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public Vector3 MouseScreenPosition { get; private set; }

    public event Action OnMeleeAttack = delegate { };
    public event Action OnRangedAttack = delegate { };

    void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");

        MouseScreenPosition = Input.mousePosition;

        if (Input.GetKeyDown(attackKey))
        {
            OnMeleeAttack();
        }
    }
}
