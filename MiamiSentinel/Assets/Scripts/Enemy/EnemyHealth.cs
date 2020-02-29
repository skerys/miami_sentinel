using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : HealthSystem
{
    private BaseEnemy baseEnemy;

    protected override void Awake()
    {
        base.Awake();
        baseEnemy = GetComponent<BaseEnemy>();
    }

    public override void Kill()
    {
        baseEnemy.OriginFactory.Reclaim(baseEnemy);
    }
}
