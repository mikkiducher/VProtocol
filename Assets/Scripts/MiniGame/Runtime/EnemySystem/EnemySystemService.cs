using System.Collections.Generic;
using System.Linq;
using TMPro;
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
                TextMeshPro hpLabel,
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
                HpLabel = hpLabel;
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
            public TextMeshPro HpLabel { get; }
            public GameObject GameObject { get; }
        }

        private readonly Dictionary<string, EnemyArchetypeConfig> _archetypesById = new(System.StringComparer.OrdinalIgnoreCase);
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
                EnsureBuiltinArchetypes();
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
                EnsureBuiltinArchetypes();
                return;
            }

            EnsureBuiltinArchetypes();
        }

        public void Spawn(string enemyId)
        {
            if (!_archetypesById.TryGetValue(enemyId, out var archetype))
            {
                Debug.LogWarning($"EnemySystem: unknown enemyId '{enemyId}'. Fallback archetype will be used.");
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
            var hpLabel = ResolveHpLabel(viewBindings, enemyObject.transform, archetype.MaxHp);

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
                    hpLabel,
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
            UpdateHpLabel(frontEnemy);
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

        private static TextMeshPro ResolveHpLabel(EnemyViewBindings viewBindings, Transform parent, int hp)
        {
            if (viewBindings != null && viewBindings.HpLabel != null)
            {
                viewBindings.HpLabel.text = hp.ToString();
                return viewBindings.HpLabel;
            }

            var labelParent = viewBindings != null ? viewBindings.HpLabelAnchor : parent;
            return CreateHpLabel(labelParent, hp);
        }

        private static TextMeshPro CreateHpLabel(Transform parent, int hp)
        {
            var labelObject = new GameObject("HpLabel");
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = Vector3.zero;

            var label = labelObject.AddComponent<TextMeshPro>();
            label.text = hp.ToString();
            label.fontSize = 4f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.outlineColor = Color.black;
            label.outlineWidth = 0.2f;

            return label;
        }

        private static void UpdateHpLabel(EnemyRuntime enemy)
        {
            if (enemy.HpLabel == null)
            {
                return;
            }

            enemy.HpLabel.text = enemy.Hp > 0 ? enemy.Hp.ToString() : "0";
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

        private void EnsureBuiltinArchetypes()
        {
            if (!_archetypesById.ContainsKey("SpamBot"))
            {
                var spam = ScriptableObject.CreateInstance<EnemyArchetypeConfig>();
                spam.Configure("SpamBot", EnemyVariant.SpamBot, 8, 2.1f);
                _archetypesById[spam.Id] = spam;
            }

            if (!_archetypesById.ContainsKey("BruteWorm"))
            {
                var brute = ScriptableObject.CreateInstance<EnemyArchetypeConfig>();
                brute.Configure("BruteWorm", EnemyVariant.BruteWorm, 20, 1.2f);
                _archetypesById[brute.Id] = brute;
            }
        }
    }

}
