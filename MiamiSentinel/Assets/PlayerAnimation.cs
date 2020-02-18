using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;

    private PlayerInput input;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        Ray mouseRay = mainCam.ScreenPointToRay(input.MouseScreenPosition);

        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity))
        {
            Vector3 lookPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookPos, Vector3.up);
        }
        
    }
}
