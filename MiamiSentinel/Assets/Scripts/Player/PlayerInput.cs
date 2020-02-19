using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private KeyCode attackKey;
    [SerializeField]
    private Camera mainCam;
    [SerializeField]
    private LayerMask mouseCollisionLayerMask;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public Vector3 LookAtPos { get; private set; }

    public event Action OnMeleeAttack = delegate { };
    public event Action OnRangedAttack = delegate { };

    void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");

        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, mouseCollisionLayerMask))
        {
            LookAtPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Debug.DrawRay(transform.position, LookAtPos - transform.position);
        }

        if (Input.GetKeyDown(attackKey))
        {
            OnMeleeAttack();
        }
    }
}
