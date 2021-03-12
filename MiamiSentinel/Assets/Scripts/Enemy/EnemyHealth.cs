using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : HealthSystem
{
    private BaseEnemy baseEnemy;
    [SerializeField] private bool killable = true;

    protected override void Awake()
    {
        base.Awake();
        baseEnemy = GetComponent<BaseEnemy>();
    }

    public override void Kill()
    {
        if(!killable) return;
        if (baseEnemy.OriginFactory != null)
        {
            baseEnemy.OriginFactory.Reclaim(baseEnemy);
        }
        else 
        {
            Destroy(gameObject);
        } 
    }
}
