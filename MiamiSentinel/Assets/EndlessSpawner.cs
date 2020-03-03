using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EndlessSpawner : MonoBehaviour
{
    private SpawningSystem spawningSystem;

    private int currentLevel;
    private int actionsDone;

    private List<List<Action>> spawnsPerLevel;

    private void Awake()
    {
        spawningSystem = GetComponent<SpawningSystem>();

        //TODO: Create a custom editor to set this up:
        List<Action> levelOneSpawns = new List<Action>();
        levelOneSpawns.Add(() => spawningSystem.SpawnSingle(EnemyType.Walker));
        levelOneSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Walker, 3));

        List<Action> levelTwoSpawns = new List<Action>();
        levelTwoSpawns.Add(() => spawningSystem.SpawnSingle(EnemyType.Ranger));
        levelTwoSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Walker, 7));

        List<Action> levelThreeSpawns = new List<Action>();
        levelThreeSpawns.Add(() => spawningSystem.SpawnSingle(EnemyType.Sentinel));
        levelThreeSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Ranger, 4));

        List<Action> levelFourSpawns = new List<Action>();
        levelFourSpawns.Add(() => spawningSystem.SpawnPack(EnemyType.Sentinel, 2));

    }


}
