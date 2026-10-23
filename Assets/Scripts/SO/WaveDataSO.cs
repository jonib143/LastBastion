using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpawnGroup
{
    public EnemyDataSO enemyData;
    public int count;
    public float spawnInterval;
    public float delayAfterGroup;
}

[CreateAssetMenu(menuName = "td/wave data")]
public class WaveDataSO : ScriptableObject
{
    public List<SpawnGroup> groups;
}