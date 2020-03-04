using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Walker,
    Sentinel,
    Ranger,
    Summoner,
    Minion
};

public class BaseEnemy : MonoBehaviour
{
    public EnemyType enemyType;

    private EnemyFactory originFactory;
    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory");
            originFactory = value;
        }
    }
}
