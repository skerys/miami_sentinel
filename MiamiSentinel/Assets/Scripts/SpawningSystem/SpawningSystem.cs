﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//currently used in a switch statement but later should be passed down for the factory/object pooling system
public enum EnemyType
{
    Walker
};

public class SpawningSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject walkerEnemy;
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

    //Based on handmade spawning "blocks":
    void SpawnSingle(EnemyType enemyType)
    {
        Vector3 spawnLocation = GetRandomPointInsideArea(baseSpawnBorder);
        switch (enemyType)
        {
            case EnemyType.Walker:
            {
                    Instantiate(walkerEnemy, new Vector3(spawnLocation.x, walkerEnemy.transform.position.y, spawnLocation.z), Quaternion.identity);
                    enemyCountInWorld++;
                    break;
            }
        }
    }

    //later should implement an alternate version SpawnPackWithWeights that can spawn
    //multiple different enemies and has probability weights for each one
    void SpawnPack(EnemyType enemyType, int enemyCount)
    {
        SpawnPack(enemyType, enemyCount, defaultPackRadius);
    }

    
    void SpawnPack(EnemyType enemyType, int enemyCount, float packRadius) 
    {
        Vector3 packCenter = GetRandomPointInsideArea(baseSpawnBorder + packRadius);
        GameObject toSpawn;
        switch (enemyType)
        {
            case EnemyType.Walker:
                {
                    toSpawn = walkerEnemy;
                    break;
                }
        }

        for(int i = 0; i < enemyCount; ++i)
        {
            Vector2 randomInPackCircle = Random.insideUnitCircle * packRadius;
            Vector3 spawnLocation = packCenter + new Vector3(randomInPackCircle.x, 0f, randomInPackCircle.y);
            Instantiate(walkerEnemy, new Vector3(spawnLocation.x, walkerEnemy.transform.position.y, spawnLocation.z), Quaternion.identity);
            enemyCountInWorld++;
        }
    }

    Vector3 GetRandomPointInsideArea(float border)
    {
        for(int i = 0; i < 10; ++i)
        {
            float randomX = Random.Range(areaCenter.x - areaWidthHeight.x / 2 + border, areaCenter.x + areaWidthHeight.x / 2 - border);
            float randomY = Random.Range(areaCenter.z - areaWidthHeight.y / 2 + border, areaCenter.z + areaWidthHeight.y / 2 - border);

            Vector3 randomPoint = new Vector3(randomX, 0.0f, randomY);
            if(Vector3.Distance(player.transform.position, randomPoint) > playerSafeDistance)
            {
                return randomPoint;
            }
        }
        Debug.LogError($"Couldn't find an available point with border: {border}.");
        return Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, new Vector3(areaWidthHeight.x, 1.0f, areaWidthHeight.y));
    }
}