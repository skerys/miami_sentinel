using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Object Pooling/Enemy Factory")]
public class EnemyFactory : GameObjectFactory<BaseEnemy>
{
    [SerializeField] BaseEnemy walkerPrefab = default;
    [SerializeField] BaseEnemy sentinelPrefab = default;
    [SerializeField] BaseEnemy rangerPrefab = default;

    public void OnEnable()
    {
        prefabs.Clear();
        prefabs.Add(walkerPrefab);
        prefabs.Add(sentinelPrefab);
        prefabs.Add(rangerPrefab);
    }

    protected void CreatePools()
    {
        scene = SceneManager.GetSceneByName(name);
        if (scene.isLoaded)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();
            for(int i = 0; i < rootObjects.Length; ++i)
            {
                BaseEnemy pooledItem = rootObjects[i].GetComponent<BaseEnemy>();
                if (!pooledItem.gameObject.activeSelf)
                {
                    pools[(int)pooledItem.enemyType].Add(pooledItem);
                }
            }
            return;
        }

        pools = new List<BaseEnemy>[prefabs.Count];
        for(int i = 0; i < prefabs.Count; ++i)
        {
            pools[i] = new List<BaseEnemy>();
        }
    }

    public BaseEnemy Get(EnemyType type)
    {
        if (pools == null)
        {
            CreatePools();
        }

        if ((int)type > prefabs.Count)
        {
            Debug.LogError($"Enemy type ({type}) not found in enemy factory.");
            return null;
        }

        var instance = CreateGameObjectInstance((int)type);
        if (!instance.OriginFactory)
        {
            instance.OriginFactory = this;
        }
        return instance;
    }

    public void Reclaim(BaseEnemy enemy)
    {
        if(pools == null)
        {
            CreatePools();
        }
        pools[(int)enemy.enemyType].Add(enemy);
        enemy.gameObject.SetActive(false);
    }

}
