using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEnemyAI
{
    bool IsEnabled();
    Transform TargetTransform { get; }

    event Action OnAttack;
}
