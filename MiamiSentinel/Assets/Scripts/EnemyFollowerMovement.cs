using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowerMovement : MonoBehaviour, IMovementInput
{
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    [SerializeField]
    private float searchRange = 10f;
    [SerializeField]
    private LayerMask playerLayerMask = default;

    private GameObject targetTransform;

    void Awake()
    {
        Horizontal = 0.0f;
        Vertical = 0.0f;
    }

    void Update()
    {
        if (targetTransform)
        {
            FollowPlayer();
        }
        else
        {
            SearchForPlayer();
        }
    }

    void SearchForPlayer()
    {
        Collider[] foundPlayers = Physics.OverlapSphere(transform.position, searchRange, playerLayerMask);
        if (foundPlayers.Length > 0)
        {
            if (foundPlayers.Length > 1)
            {
                Debug.LogError("More than one player found by enemy", this);
                return;
            }

            targetTransform = foundPlayers[0].gameObject;
        }
    }

    void FollowPlayer()
    {
        Vector3 directionToPlayer = targetTransform.transform.position - transform.position;
        directionToPlayer.Normalize();

        Debug.DrawLine(transform.position, transform.position + directionToPlayer * 3f);

        Horizontal = directionToPlayer.x;
        Vertical = directionToPlayer.z;
    }
    


}
