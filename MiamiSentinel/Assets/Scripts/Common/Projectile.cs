using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    LayerMask damageLayermask;
    [SerializeField]
    LayerMask shieldLayermask;

    private Rigidbody body;
    private Collider creatorCollider;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void SetProjectileDirection(Vector3 direction)
    {
        body.velocity = direction.normalized * speed;
        transform.LookAt(direction);
    }

    public void SetCreatorCollider(Collider creator)
    {
        creatorCollider = creator;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == creatorCollider) return;

        //shieldLayerMask == (shieldLayerMask | (1 << rangedHits[i].collider.gameObject.layer))
        if (damageLayermask == (damageLayermask | 1 << other.gameObject.layer))
        {
            //Do damage
            var health = GetComponent<HealthSystem>();
            if (health)
            {
                health.Damage(1);
            }
        }
        Destroy(gameObject);
    }
}
