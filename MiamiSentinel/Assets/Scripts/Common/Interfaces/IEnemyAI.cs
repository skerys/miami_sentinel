using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEnemyAI
{
    void EnableAI();
    void DisableAI();

    bool IsEnabled();

    event Action OnAttack;
    void SetAttackRange(float range);
}
