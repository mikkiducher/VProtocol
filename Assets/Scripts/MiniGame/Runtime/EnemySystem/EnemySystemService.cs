using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VProtocol.MiniGame.Config.Enemies;
using VProtocol.MiniGame.Runtime.Contracts;

namespace VProtocol.MiniGame.Runtime.EnemySystem
{
    public sealed class EnemySystemService : IEnemySystem
    {
        private sealed class EnemyRuntime
        {
            public EnemyRuntime(
                string id,
                EnemyVariant variant,
                int hp,
                float speed,
                float spawnTime,
                float baseY,
                Vector3 visualOffset,
                EnemyViewBindings viewBindings,
                GameObject gameObject)
            {
                Id = id;
                Variant = variant;
                Hp = hp;
                Speed = speed;
                SpawnTime = spawnTime;
                BaseY = baseY;
                VisualOffset = visualOffset;
                ViewBindings = viewBindings;
                GameObject = gameObject;
            }

            public string Id { get; }
            public EnemyVariant Variant { get; }
            public int Hp { get; set; }
            public float Speed { get; }
            public float SpawnTime { get; }
            public float BaseY { get; }
            public Vector3 VisualOffset { get; }
            public EnemyViewBindings ViewBindings { get; }
            public GameObject GameObject { get; }
        }

        private readonly Dictionary<string, EnemyArchetypeConfig> _archetypesById = new();
        private readonly List<EnemyRuntime> _activeEnemies = new();
        private Transform _parent;
        private Vector3 _spawnPosition;
        private float _robotXPosition;
        private float _spawnYJitter;

        public int AliveCount => _activeEnemies.Count;

        public event System.Action EnemyReachedRobot;

        public void Initialize(IEnumerable<EnemyArchetypeConfig> enemyArchetypes, Transform parent, Vector3 spawnPosition, float robotXPosition, float spawnYJitter)
        {
            _archetypesById.Clear();
            _activeEnemies.Clear();
            _parent = parent;
            _spawnPosition = spawnPosition;
            _robotXPosition = robotXPosition;
            _spawnYJitter = spawnYJitter < 0f ? 0f : spawnYJitter;

            if (enemyArchetypes == null)
            {
                var fallback = ScriptableObject.CreateInstance<EnemyArchetypeConfig>();
                fallback.Configure("SpamBot", EnemyVariant.SpamBot, 8, 2.1f);
                _archetypesById[fallback.Id] = fallback;
                var bruteFallback = ScriptableObject.CreateInstance<EnemyArchetypeConfig>();
                bruteFallback.Configure("BruteWorm", EnemyVariant.BruteWorm, 20, 1.2f);
                _archetypesById[bruteFallback.Id] = bruteFallback;
                return;
            }

            foreach (var archetype in enemyArchetypes)
            {
                if (archetype == null || string.IsNullOrWhiteSpace(archetype.Id))
                {
                    continue;
                }

                _archetypesById[archetype.Id] = archetype;
            }

            if (_archetypesById.Count == 0)
            {
                var fallback = ScriptableObject.CreateInstance<EnemyArchetypeConfig>();
                fallback.Configure("SpamBot", EnemyVariant.SpamBot, 8, 2.1f);
                _archetypesById[fallback.Id] = fallback;
                var bruteFallback = ScriptableObject.CreateInstance<EnemyArchetypeConfig>();
                bruteFallback.Configure("BruteWorm", EnemyVariant.BruteWorm, 20, 1.2f);
                _archetypesById[bruteFallback.Id] = bruteFallback;
            }
        }

        public void Spawn(string enemyId)
        {
            if (!_archetypesById.TryGetValue(enemyId, out var archetype))
            {
                if (!_archetypesById.TryGetValue("SpamBot", out archetype))
                {
                    archetype = _archetypesById.Values.First();
                }
            }

            var enemyObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            enemyObject.name = $"Enemy_{archetype.Id}";
            var spawnPoint = new Vector3(
                _spawnPosition.x,
                _spawnPosition.y + Random.Range(-_spawnYJitter, _spawnYJitter),
                _spawnPosition.z);
            enemyObject = SetupVisual(enemyObject, archetype, spawnPoint, out var viewBindings);

            _activeEnemies.Add(
                new EnemyRuntime(
                    archetype.Id,
                    archetype.Variant,
                    archetype.MaxHp,
                    archetype.MoveSpeed,
                    Time.time,
                    spawnPoint.y,
                    archetype.VisualOffset,
                    viewBindings,
                    enemyObject));
        }

        public void Tick(float deltaTime)
        {
            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _activeEnemies[i];
                if (enemy.GameObject == null)
                {
                    _activeEnemies.RemoveAt(i);
                    continue;
                }

                var transform = enemy.GameObject.transform;
                var nextPosition = transform.position + Vector3.left * (enemy.Speed * deltaTime);
                if (enemy.Variant == EnemyVariant.Phisher)
                {
                    var offset = Mathf.Sin((Time.time - enemy.SpawnTime) * 3.2f) * 0.35f;
                    nextPosition.y = enemy.BaseY + offset;
                }

                transform.position = nextPosition;

                if (transform.position.x <= _robotXPosition)
                {
                    Object.Destroy(enemy.GameObject);
                    _activeEnemies.RemoveAt(i);
                    EnemyReachedRobot?.Invoke();
                }
            }
        }

        public bool ApplyDamageToFrontEnemy(int damage)
        {
            if (_activeEnemies.Count == 0 || damage <= 0)
            {
                return false;
            }

            var frontEnemy = _activeEnemies.OrderBy(enemy => enemy.GameObject.transform.position.x).First();
            frontEnemy.Hp -= damage;
            if (frontEnemy.Hp <= 0)
            {
                Object.Destroy(frontEnemy.GameObject);
                _activeEnemies.Remove(frontEnemy);
            }

            return true;
        }

        public bool TryGetFrontEnemyPosition(out Vector3 position)
        {
            if (_activeEnemies.Count == 0)
            {
                position = Vector3.zero;
                return false;
            }

            var frontEnemy = _activeEnemies.OrderBy(enemy => enemy.GameObject.transform.position.x).First();
            if (frontEnemy.GameObject == null)
            {
                position = Vector3.zero;
                return false;
            }

            if (frontEnemy.ViewBindings != null)
            {
                position = frontEnemy.ViewBindings.GetHitPointPosition();
                return true;
            }

            position = frontEnemy.GameObject.transform.position;
            return true;
        }

        public void Reset()
        {
            for (var i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _activeEnemies[i];
                if (enemy.GameObject != null)
                {
                    Object.Destroy(enemy.GameObject);
                }
            }

            _activeEnemies.Clear();
        }

        private GameObject SetupVisual(
            GameObject enemyObject,
            EnemyArchetypeConfig archetype,
            Vector3 spawnPoint,
            out EnemyViewBindings viewBindings)
        {
            GameObject visualRoot = enemyObject;
            if (archetype.ViewPrefab != null)
            {
                Object.Destroy(enemyObject);
                visualRoot = Object.Instantiate(archetype.ViewPrefab, _parent);
                visualRoot.name = $"Enemy_{archetype.Id}";
            }

            visualRoot.transform.SetParent(_parent, false);
            visualRoot.transform.position = spawnPoint + archetype.VisualOffset;
            visualRoot.transform.localScale = archetype.VisualScale;

            if (archetype.ViewSprite != null)
            {
                var spriteRenderer = visualRoot.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = visualRoot.AddComponent<SpriteRenderer>();
                }

                spriteRenderer.sprite = archetype.ViewSprite;
                spriteRenderer.color = Color.white;

                var meshRenderer = visualRoot.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
            }
            else
            {
                var meshRenderer = visualRoot.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = true;
                    meshRenderer.material.color = ResolveColor(archetype.Variant);
                }
            }

            if (archetype.ViewAnimatorController != null)
            {
                var animator = visualRoot.GetComponent<Animator>();
                if (animator == null)
                {
                    animator = visualRoot.AddComponent<Animator>();
                }

                animator.runtimeAnimatorController = archetype.ViewAnimatorController;
            }

            viewBindings = visualRoot.GetComponentInChildren<EnemyViewBindings>();
            if (viewBindings == null)
            {
                viewBindings = visualRoot.AddComponent<EnemyViewBindings>();
            }

            if (visualRoot != enemyObject)
            {
                return visualRoot;
            }

            return enemyObject;
        }

        private static Color ResolveColor(EnemyVariant variant)
        {
            return variant switch
            {
                EnemyVariant.BruteWorm => new Color(1f, 0.45f, 0.2f, 1f),
                EnemyVariant.Phisher => new Color(0.95f, 0.2f, 0.95f, 1f),
                _ => new Color(0.2f, 0.95f, 0.95f, 1f)
            };
        }
    }

    public sealed class EnemyViewBindings : MonoBehaviour
    {
        [SerializeField] private Transform hitPoint;

        public Vector3 GetHitPointPosition()
        {
            return hitPoint != null ? hitPoint.position : transform.position;
        }
    }
}
