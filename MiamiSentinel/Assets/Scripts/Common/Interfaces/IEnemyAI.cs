using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEnemyAI
{
    bool IsEnabled();

    event Action OnAttack;
}
