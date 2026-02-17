using System.Collections.Generic;
using UnityEngine;
using VProtocol.MiniGame.Config.Enemies;

namespace VProtocol.MiniGame.Runtime.Contracts
{
    public interface IEnemySystem
    {
        int AliveCount { get; }
        event System.Action EnemyReachedRobot;
        void Initialize(IEnumerable<EnemyArchetypeConfig> enemyArchetypes, Transform parent, Vector3 spawnPosition, float robotXPosition, float spawnYJitter);
        void Spawn(string enemyId);
        void Tick(float deltaTime);
        bool TryGetFrontEnemyPosition(out Vector3 position);
        bool ApplyDamageToFrontEnemy(int damage);
        void Reset();
    }
}
