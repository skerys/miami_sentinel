using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EndlessSpawner : MonoBehaviour
{
    [SerializeField] private float spawnCooldown = 2.0f;
    [SerializeField] private int spawnsPerLevel = 5;

    private float spawnTimer = 0.0f;

    private SpawningSystem spawningSystem;

    private int currentLevel = 0;
    private int actionsDone;

    private List<List<Action>> levelSpawnLists;
    private List<Action> activeSpawns;

    private void Awake()
    {
        spawningSystem = GetComponent<SpawningSystem>();

        //TODO: Create a custom editor to set this up:
        List<Action> levelOneSpawns = new List<Action>();
        levelOneSpawns.Add(() => spawningSystem.SpawnSingle(EnemyType.Walker));
        levelOneSpawns.Add(() => spawningSystem.SpawnSingle(EnemyType.Ranger));

        List<Action> levelTwoSpawns = new List<Action>();
        levelTwoSpawns.Add(() => spawningSystem.SpawnSingle(EnemyType.Sentinel));
        levelTwoSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Walker, 3));
        levelTwoSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Ranger, 2));

        List<Action> levelThreeSpawns = new List<Action>();
        levelThreeSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Sentinel, 2));
        levelThreeSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Walker, 4));
        levelTwoSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Ranger, 3));

        List<Action> levelFourSpawns = new List<Action>();
        levelFourSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Sentinel, 2));
        levelFourSpawns.Add(() => spawningSystem.SpawnSingle(EnemyType.Summoner));

        levelSpawnLists = new List<List<Action>>();
        activeSpawns = new List<Action>();

        levelSpawnLists.Add(levelOneSpawns);
        levelSpawnLists.Add(levelTwoSpawns);
        levelSpawnLists.Add(levelThreeSpawns);
        levelSpawnLists.Add(levelFourSpawns);

        IncreaseLevel();

    }

    private void Update()
    {
        if(spawnTimer < spawnCooldown)
        {
            spawnTimer += Time.deltaTime;
            if(spawnTimer >= spawnCooldown)
            {
                DoSpawn();
                spawnTimer = 0.0f;
                actionsDone++;

                if(actionsDone >= spawnsPerLevel)
                {
                    IncreaseLevel();
                    actionsDone = 0;
                }
            }
        }
    }

    void IncreaseLevel()
    {
        currentLevel++;
        foreach(var spawn in levelSpawnLists[currentLevel - 1])
        {
            activeSpawns.Add(spawn);
        }
    }

    void DoSpawn()
    {
        int randomIndex = UnityEngine.Random.Range(0, activeSpawns.Count);
        activeSpawns[randomIndex]();
    }


}
