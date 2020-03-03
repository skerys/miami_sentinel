﻿using System.Collections;
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
        if (baseEnemy.OriginFactory)
        {
            baseEnemy.OriginFactory.Reclaim(baseEnemy);
        }
        else 
        {
            Destroy(gameObject);
        } 
    }
}
