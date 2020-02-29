using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    LayerMask damageLayerMask;
    [SerializeField]
    LayerMask shieldLayerMask;

    private Rigidbody body;
    private Collider creatorCollider;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void SetProjectileDirection(Vector3 direction)
    {
        body.velocity = direction.normalized * speed;
        transform.LookAt(transform.position + direction);
    }

    public void SetCreatorCollider(Collider creator)
    {
        creatorCollider = creator;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == creatorCollider) return;

        if(shieldLayerMask == (shieldLayerMask | 1 << other.gameObject.layer)){
            //Get the normal of collision with shield
            Debug.Log("hit shield");
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, shieldLayerMask))
            {
                body.velocity = Vector3.Reflect(body.velocity, hit.normal);
            }
            //To avoid destruction of this object
            return;
        }
        if (damageLayerMask == (damageLayerMask | 1 << other.gameObject.layer))
        {
            //Do damage
            var health = other.GetComponent<HealthSystem>();
            if (health)
            {
                health.Damage(1);
            }
        }
        Destroy(gameObject);
    }
}
