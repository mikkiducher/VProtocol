using System;
using UnityEngine;

namespace VProtocol.MiniGame.Config.Waves
{
    [CreateAssetMenu(
        fileName = "WaveConfig",
        menuName = "VProtocol/MiniGame/Config/Wave",
        order = 0)]
    public sealed class WaveConfig : ScriptableObject
    {
        [SerializeField] private WaveSpawnDescriptor[] spawns = Array.Empty<WaveSpawnDescriptor>();

        public WaveSpawnDescriptor[] Spawns => spawns;
    }

    [Serializable]
    public sealed class WaveSpawnDescriptor
    {
        [SerializeField] private string enemyId = "SpamBot";
        [SerializeField] private int count = 1;
        [SerializeField] private float startAfterSeconds = 0f;
        [SerializeField] private float intervalSeconds = 1f;

        public string EnemyId => enemyId;
        public int Count => count;
        public float StartAfterSeconds => startAfterSeconds;
        public float IntervalSeconds => intervalSeconds;

        public void Configure(string enemyTypeId, int spawnCount, float spawnIntervalSeconds, float spawnStartAfterSeconds = 0f)
        {
            enemyId = string.IsNullOrWhiteSpace(enemyTypeId) ? "SpamBot" : enemyTypeId;
            count = spawnCount < 0 ? 0 : spawnCount;
            intervalSeconds = spawnIntervalSeconds < 0f ? 0f : spawnIntervalSeconds;
            startAfterSeconds = spawnStartAfterSeconds < 0f ? 0f : spawnStartAfterSeconds;
        }
    }
}
