using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpawningSystem : MonoBehaviour
{
    [SerializeField]
    private EnemyFactory enemyFactory;
    [SerializeField]
    private Vector3 areaCenter;
    [SerializeField]
    private Vector2 areaWidthHeight;
    [SerializeField]
    private float defaultPackRadius;
    [SerializeField]
    private float baseSpawnBorder; //to make sure no spawns happen inside the wall ever
    [SerializeField]
    private float playerSafeDistance; //how far should the spawn be from player to happen (so enemies dont spawn on you)
                                      //however currently packs can still spawn on player since packRadius is usually bigger than player safe distance
                                      //(maybe use playerSafeDistance + packRadius) when spawningPacks
    [SerializeField]
    private PlayerInput player;

    private int enemyCountInWorld;

    [ContextMenu("Spawn a Single Walker")]
    void SpawnSingleWalker()
    {
        SpawnSingle(EnemyType.Walker);
    }

    [ContextMenu("Spawn a Pack of Walkers")]
    void SpawnPackWalker()
    {
        SpawnPack(EnemyType.Walker, 7);
    }

    [ContextMenu("Spawn a Single Sentinel")]
    void SpawnSingleShield()
    {
        SpawnSingle(EnemyType.Sentinel);
    }

    [ContextMenu("Spawn a Pack of Sentinels")]
    void SpawnPackShield()
    {
        SpawnPack(EnemyType.Sentinel, 3, 5);
    }

    //Based on handmade spawning "blocks":
    void SpawnSingle(EnemyType enemyType)
    {
        Vector2 spawnLocation = GetRandomPointInsideArea(baseSpawnBorder);
        var newEnemy = enemyFactory.Get(enemyType);
        newEnemy.transform.position = new Vector3(spawnLocation.x, newEnemy.transform.position.y, spawnLocation.y);
        enemyCountInWorld++;
    }

    //later should implement an alternate version SpawnPackWithWeights that can spawn
    //multiple different enemies and has probability weights for each one
    void SpawnPack(EnemyType enemyType, int enemyCount)
    {
        SpawnPack(enemyType, enemyCount, defaultPackRadius);
    }

    
    void SpawnPack(EnemyType enemyType, int enemyCount, float packRadius) 
    {
        Vector2 packCenter = GetRandomPointInsideArea(baseSpawnBorder + packRadius);

        for(int i = 0; i < enemyCount; ++i)
        {
            Vector2 randomInPackCircle = Random.insideUnitCircle * packRadius;
            Vector2 spawnLocation = packCenter + new Vector2(randomInPackCircle.x, randomInPackCircle.y);
            var newEnemy = enemyFactory.Get(enemyType);
            newEnemy.transform.position = new Vector3(spawnLocation.x, newEnemy.transform.position.y, spawnLocation.y);
            enemyCountInWorld++;
        }
    }

    Vector2 GetRandomPointInsideArea(float border)
    {
        for(int i = 0; i < 10; ++i)
        {
            float randomX = Random.Range(areaCenter.x - areaWidthHeight.x / 2 + border, areaCenter.x + areaWidthHeight.x / 2 - border);
            float randomY = Random.Range(areaCenter.z - areaWidthHeight.y / 2 + border, areaCenter.z + areaWidthHeight.y / 2 - border);

            Vector2 randomPoint = new Vector2(randomX, randomY);
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
            if(Vector2.Distance(playerPos, randomPoint) > playerSafeDistance)
            {
                return randomPoint;
            }
        }
        Debug.LogError($"Couldn't find an available point with border: {border}.");
        return Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, new Vector3(areaWidthHeight.x, 1.0f, areaWidthHeight.y));
    }
}
