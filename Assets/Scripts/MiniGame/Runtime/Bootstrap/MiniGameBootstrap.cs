using System;
using System.Collections;
using UnityEngine;
using VProtocol.MiniGame.Config.Levels;
using VProtocol.MiniGame.Runtime.BarrierSystem;
using VProtocol.MiniGame.Runtime.CombatSystem;
using VProtocol.MiniGame.Runtime.Contracts;
using VProtocol.MiniGame.Runtime.Core;
using VProtocol.MiniGame.Runtime.EnemySystem;
using VProtocol.MiniGame.Runtime.GameFlow;
using VProtocol.MiniGame.Runtime.MathSystem;
using VProtocol.MiniGame.Runtime.UISystem;
using VProtocol.MiniGame.Runtime.WaveSystem;

namespace VProtocol.MiniGame.Runtime.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class MiniGameBootstrap : MonoBehaviour
    {
        private sealed class SessionTelemetry
        {
            public int FastStreak { get; private set; }
            public int ComboStreak { get; private set; }
            public int MaxComboStreak { get; private set; }
            public int DifficultyTier { get; private set; }
            public int AnswersTotal { get; private set; }
            public int AnswersCorrect { get; private set; }
            public float ResponseTotalSeconds { get; private set; }
            public float LastMultiplier { get; private set; } = 1f;

            public void Reset()
            {
                FastStreak = 0;
                ComboStreak = 0;
                MaxComboStreak = 0;
                DifficultyTier = 0;
                AnswersTotal = 0;
                AnswersCorrect = 0;
                ResponseTotalSeconds = 0f;
                LastMultiplier = 1f;
            }

            public CombatPreview BuildCorrectPreview(float responseSeconds, float fastThresholdSeconds)
            {
                var isFastAnswer = responseSeconds <= fastThresholdSeconds;
                var nextFastStreak = isFastAnswer ? FastStreak + 1 : 0;
                var nextComboStreak = nextFastStreak >= 3 ? nextFastStreak : 0;
                var nextDifficultyTier = ResolveDifficultyTier(nextComboStreak);
                return new CombatPreview(nextFastStreak, nextComboStreak, nextDifficultyTier);
            }

            public void ApplyCorrect(float responseSeconds, float multiplier, CombatPreview preview)
            {
                AnswersCorrect++;
                AnswersTotal++;
                ResponseTotalSeconds += responseSeconds;
                FastStreak = preview.FastStreak;
                ComboStreak = preview.ComboStreak;
                MaxComboStreak = Math.Max(MaxComboStreak, ComboStreak);
                DifficultyTier = preview.DifficultyTier;
                LastMultiplier = multiplier;
            }

            public void ApplyWrong()
            {
                AnswersTotal++;
                FastStreak = 0;
                ComboStreak = 0;
                DifficultyTier = 0;
                LastMultiplier = 1f;
            }

            public float GetAverageResponseSeconds()
            {
                return AnswersCorrect > 0 ? ResponseTotalSeconds / AnswersCorrect : 0f;
            }

            public float GetAccuracy()
            {
                return AnswersTotal > 0 ? (float)AnswersCorrect / AnswersTotal : 0f;
            }

            private static int ResolveDifficultyTier(int comboStreak)
            {
                if (comboStreak < 3)
                {
                    return 0;
                }

                return 1 + ((comboStreak - 3) / 2);
            }
        }

        private readonly struct CombatPreview
        {
            public CombatPreview(int fastStreak, int comboStreak, int difficultyTier)
            {
                FastStreak = fastStreak;
                ComboStreak = comboStreak;
                DifficultyTier = difficultyTier;
            }

            public int FastStreak { get; }
            public int ComboStreak { get; }
            public int DifficultyTier { get; }
        }

        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private bool autoStartOnAwake = true;
        [SerializeField] private Vector3 spawnPosition = new(8f, 0f, 0f);
        [SerializeField] private float robotXPosition = -3f;
        [SerializeField] private float spawnYJitter = 1.25f;
        [Header("Laser Visuals")]
        [SerializeField] private GameObject laserViewPrefab;
        [SerializeField] private Sprite laserBodySprite;
        [SerializeField] private Sprite laserEmitterSprite;
        [SerializeField] private Sprite laserImpactSprite;
        [SerializeField] private float laserDurationSeconds = 0.1f;
        [SerializeField] private float laserThickness = 0.2f;

        private IGameFlow _gameFlow;
        private IWaveSystem _waveSystem;
        private IEnemySystem _enemySystem;
        private IMathSystem _mathSystem;
        private ICombatSystem _combatSystem;
        private IBarrierSystem _barrierSystem;
        private IUISystem _uiSystem;

        private MathRound _currentRound;
        private float _roundStartedAt;
        private float _gameStartedAt;
        private Transform _enemyRoot;
        private Transform _robotMarker;
        private SpriteRenderer _robotSpriteRenderer;
        private Animator _robotAnimator;
        private MeshRenderer _robotFallbackRenderer;
        private RobotViewBindings _robotViewBindings;
        private LevelConfig.RobotVisualProfile _robotVisualProfile;
        private bool _waveFinished;
        private bool _sessionStarted;
        private readonly SessionTelemetry _telemetry = new();

        private const float FastAnswerThresholdSeconds = 2f;

        public event Action<MiniGameResult> GameCompleted;
        public event Action<MiniGameRuntimeStats> RuntimeStatsUpdated;

        private void Awake()
        {
            BuildSystems();
            BindSystems();

            if (autoStartOnAwake)
            {
                StartLevel(levelConfig);
            }
        }

        private void Update()
        {
            if (_gameFlow.State != GameState.Playing)
            {
                return;
            }

            _waveSystem.Tick(Time.deltaTime);
            _enemySystem.Tick(Time.deltaTime);

            if (_waveFinished && _enemySystem.AliveCount == 0)
            {
                EndGame(true);
            }
        }

        [ContextMenu("Start Assigned Level")]
        public void StartAssignedLevel()
        {
            StartLevel(levelConfig);
        }

        public void StartLevel(LevelConfig config)
        {
            levelConfig = config;
            if (_sessionStarted)
            {
                CleanupSession();
            }

            InitializeSystems(config);
            _gameFlow.Reset();
            _gameFlow.StartGame();
            _waveSystem.StartWaves();
            NextRound();
            _gameStartedAt = Time.time;
            _sessionStarted = true;
            SyncUiState();
        }

        [ContextMenu("Stop Session")]
        public void StopSession()
        {
            CleanupSession();
            _gameFlow.Reset();
            SyncUiState();
        }

        [ContextMenu("Toggle Debug Overlay")]
        public void ToggleDebugOverlay()
        {
            if (_uiSystem == null)
            {
                return;
            }

            _uiSystem.ToggleOverlay();
        }

        public MiniGameRuntimeStats GetRuntimeStats()
        {
            return BuildRuntimeStats();
        }

        private void BuildSystems()
        {
            _enemyRoot = new GameObject("EnemyRoot").transform;
            _enemyRoot.SetParent(transform, false);

            _robotMarker = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            _robotMarker.name = "RobotMarker";
            _robotMarker.SetParent(transform, false);
            _robotMarker.position = new Vector3(robotXPosition, 0f, 0f);
            _robotMarker.localScale = new Vector3(1.2f, 1.2f, 1f);
            _robotFallbackRenderer = _robotMarker.GetComponent<MeshRenderer>();
            _robotViewBindings = _robotMarker.gameObject.AddComponent<RobotViewBindings>();
            if (_robotFallbackRenderer != null)
            {
                _robotFallbackRenderer.material.color = Color.green;
            }

            _gameFlow = new GameFlowService();
            _waveSystem = new WaveSystemService();
            _enemySystem = new EnemySystemService();
            _mathSystem = new MathSystemService();
            _combatSystem = new CombatSystemService();
            _barrierSystem = new BarrierSystemService();
            _uiSystem = gameObject.AddComponent<UISystemService>();
        }

        private void InitializeSystems(LevelConfig config)
        {
            if (config == null)
            {
                Debug.LogWarning("MiniGameBootstrap has no LevelConfig assigned. Using fallback runtime defaults.");
            }

            var barrierLayers = config != null ? config.BarrierLayers : 3;
            var mathConfig = config != null ? config.MathConfig : null;
            var enemyArchetypes = config != null ? config.EnemyArchetypes : null;

            _barrierSystem.Initialize(barrierLayers);
            _mathSystem.Initialize(mathConfig);
            _enemySystem.Initialize(enemyArchetypes, _enemyRoot, spawnPosition, robotXPosition, spawnYJitter);
            _waveSystem.Initialize(config);
            _uiSystem.Initialize();
            _robotVisualProfile = config != null ? config.RobotVisual : null;
            ApplyRobotVisualProfile(_robotVisualProfile);
            SetRobotVisualState(RobotVisualState.Idle);

            _waveFinished = false;
            _telemetry.Reset();
        }

        private void BindSystems()
        {
            _waveSystem.SpawnRequested += OnSpawnRequested;
            _waveSystem.WaveCompleted += OnWaveCompleted;
            _enemySystem.EnemyReachedRobot += OnEnemyReachedRobot;
            _uiSystem.AnswerSelected += OnAnswerSelected;
            _gameFlow.StateChanged += OnGameStateChanged;
        }

        private void CleanupSession()
        {
            _waveSystem.Reset();
            _enemySystem.Reset();
            _mathSystem.Reset();
            _combatSystem.Reset();
            _barrierSystem.Reset();
            _uiSystem.Reset();
            _waveFinished = false;
            _sessionStarted = false;
        }

        private void OnDestroy()
        {
            _waveSystem.SpawnRequested -= OnSpawnRequested;
            _waveSystem.WaveCompleted -= OnWaveCompleted;
            _enemySystem.EnemyReachedRobot -= OnEnemyReachedRobot;
            _uiSystem.AnswerSelected -= OnAnswerSelected;
            _gameFlow.StateChanged -= OnGameStateChanged;
        }

        private void OnSpawnRequested(VProtocol.MiniGame.Config.Waves.WaveSpawnDescriptor descriptor)
        {
            _enemySystem.Spawn(descriptor.EnemyId);
        }

        private void OnWaveCompleted()
        {
            _waveFinished = true;
        }

        private void OnEnemyReachedRobot()
        {
            if (_gameFlow.State != GameState.Playing)
            {
                return;
            }

            var hasLayersLeft = _barrierSystem.ConsumeLayer();
            if (!hasLayersLeft)
            {
                SetRobotVisualState(RobotVisualState.Break);
                EndGame(false);
                return;
            }

            SetRobotVisualState(RobotVisualState.Hit);
            SyncUiState();
        }

        private void OnAnswerSelected(int answer)
        {
            if (_gameFlow.State != GameState.Playing)
            {
                return;
            }

            if (_mathSystem.IsCorrect(_currentRound, answer))
            {
                var responseSeconds = Time.time - _roundStartedAt;
                var preview = _telemetry.BuildCorrectPreview(responseSeconds, FastAnswerThresholdSeconds);
                var damageResult = _combatSystem.ComputeDamage(
                    _currentRound.Question,
                    answer,
                    responseSeconds,
                    preview.ComboStreak,
                    preview.DifficultyTier);

                if (_enemySystem.TryGetFrontEnemyPosition(out var targetPosition))
                {
                    SetRobotVisualState(RobotVisualState.Shoot);
                    StartCoroutine(PlayLaserShot(GetLaserOrigin(), targetPosition));
                }

                _enemySystem.ApplyDamageToFrontEnemy(damageResult.Damage);
                _uiSystem.ShowCorrectFeedback(_currentRound.Question.CorrectAnswer);

                _telemetry.ApplyCorrect(responseSeconds, damageResult.Multiplier, preview);
                Debug.Log(
                    $"Combat: correct, response={responseSeconds:0.00}s, combo={preview.ComboStreak}, tier={preview.DifficultyTier}, x{damageResult.Multiplier:0.00}, dmg={damageResult.Damage}, floor={damageResult.ComplexityFloorDamage}");
            }
            else
            {
                _uiSystem.ShowWrongFeedback(_currentRound.Question.CorrectAnswer);
                var hasLayersLeft = _barrierSystem.ConsumeLayer();
                _telemetry.ApplyWrong();
                Debug.Log("Combat: wrong answer, combo reset.");
                if (!hasLayersLeft)
                {
                    SetRobotVisualState(RobotVisualState.Break);
                    EndGame(false);
                    return;
                }

                SetRobotVisualState(RobotVisualState.Hit);
            }

            SyncUiState();
            NextRound();
        }

        private void OnGameStateChanged(GameState state)
        {
            SyncUiState();
            if (!_sessionStarted)
            {
                return;
            }

            if (state == GameState.Win || state == GameState.Lose)
            {
                EmitResult(state == GameState.Win);
                _sessionStarted = false;
            }
        }

        private void NextRound()
        {
            _currentRound = _mathSystem.GenerateRound(_telemetry.DifficultyTier);
            _roundStartedAt = Time.time;
            _uiSystem.ShowQuestion(_currentRound);
        }

        private void SyncUiState()
        {
            var stats = BuildRuntimeStats();
            _uiSystem.SetGameState(_gameFlow.State, _barrierSystem.CurrentLayers);
            _uiSystem.SetCombatTelemetry(
                stats.ComboStreak,
                stats.FastStreak,
                stats.DifficultyTier,
                stats.Multiplier,
                stats.AverageResponseSeconds,
                stats.Accuracy);
            RuntimeStatsUpdated?.Invoke(stats);
        }

        private void EmitResult(bool isWin)
        {
            var duration = Mathf.Max(0f, Time.time - _gameStartedAt);
            var result = new MiniGameResult(
                isWin,
                duration,
                _telemetry.AnswersCorrect,
                _telemetry.AnswersTotal,
                _telemetry.MaxComboStreak,
                _telemetry.GetAverageResponseSeconds());

            Debug.Log(
                $"MiniGameResult => win={result.IsWin}, duration={result.DurationSeconds:0.00}s, accuracy={result.Accuracy * 100f:0}%, maxCombo={result.MaxComboStreak}");
            GameCompleted?.Invoke(result);
        }

        private void EndGame(bool isWin)
        {
            if (isWin)
            {
                SetRobotVisualState(RobotVisualState.Idle);
            }

            _gameFlow.CompleteGame(isWin);
        }

        private MiniGameRuntimeStats BuildRuntimeStats()
        {
            return new MiniGameRuntimeStats(
                _gameFlow.State,
                _barrierSystem.CurrentLayers,
                _telemetry.ComboStreak,
                _telemetry.FastStreak,
                _telemetry.DifficultyTier,
                _telemetry.LastMultiplier,
                _telemetry.AnswersCorrect,
                _telemetry.AnswersTotal,
                _telemetry.GetAverageResponseSeconds());
        }

        private void ApplyRobotVisualProfile(LevelConfig.RobotVisualProfile profile)
        {
            if (profile == null)
            {
                UseRobotFallbackVisual();
                return;
            }

            if (profile.RobotPrefab != null)
            {
                var instantiated = Instantiate(profile.RobotPrefab, transform);
                instantiated.name = "RobotMarker";
                instantiated.transform.position = _robotMarker.position;
                instantiated.transform.localScale = profile.Scale;
                Destroy(_robotMarker.gameObject);
                _robotMarker = instantiated.transform;
                _robotSpriteRenderer = _robotMarker.GetComponentInChildren<SpriteRenderer>();
                _robotAnimator = _robotMarker.GetComponentInChildren<Animator>();
                _robotFallbackRenderer = _robotMarker.GetComponent<MeshRenderer>();
                _robotViewBindings = _robotMarker.GetComponentInChildren<RobotViewBindings>();
                if (_robotViewBindings == null)
                {
                    _robotViewBindings = _robotMarker.gameObject.AddComponent<RobotViewBindings>();
                }
            }
            else
            {
                _robotMarker.localScale = profile.Scale;
                if (profile.IdleSprite != null)
                {
                    _robotSpriteRenderer = _robotMarker.GetComponent<SpriteRenderer>();
                    if (_robotSpriteRenderer == null)
                    {
                        _robotSpriteRenderer = _robotMarker.gameObject.AddComponent<SpriteRenderer>();
                    }

                    _robotSpriteRenderer.sprite = profile.IdleSprite;
                    if (_robotFallbackRenderer != null)
                    {
                        _robotFallbackRenderer.enabled = false;
                    }
                }
                else
                {
                    UseRobotFallbackVisual();
                }

                if (profile.AnimatorController != null)
                {
                    _robotAnimator = _robotMarker.GetComponent<Animator>();
                    if (_robotAnimator == null)
                    {
                        _robotAnimator = _robotMarker.gameObject.AddComponent<Animator>();
                    }

                    _robotAnimator.runtimeAnimatorController = profile.AnimatorController;
                }

                _robotViewBindings = _robotMarker.GetComponentInChildren<RobotViewBindings>();
                if (_robotViewBindings == null)
                {
                    _robotViewBindings = _robotMarker.gameObject.AddComponent<RobotViewBindings>();
                }
            }
        }

        private void UseRobotFallbackVisual()
        {
            if (_robotFallbackRenderer != null)
            {
                _robotFallbackRenderer.enabled = true;
                _robotFallbackRenderer.material.color = Color.green;
            }
        }

        private void SetRobotVisualState(RobotVisualState state)
        {
            if (_robotAnimator != null)
            {
                var stateName = state.ToString();
                _robotAnimator.Play(stateName, 0, 0f);
            }

            if (_robotSpriteRenderer == null || _robotVisualProfile == null)
            {
                if (_robotFallbackRenderer != null)
                {
                    _robotFallbackRenderer.material.color = state switch
                    {
                        RobotVisualState.Shoot => Color.cyan,
                        RobotVisualState.Hit => new Color(1f, 0.7f, 0.2f, 1f),
                        RobotVisualState.Break => Color.red,
                        _ => Color.green
                    };
                }

                return;
            }

            var sprite = state switch
            {
                RobotVisualState.Shoot => _robotVisualProfile.ShootSprite,
                RobotVisualState.Hit => _robotVisualProfile.HitSprite,
                RobotVisualState.Break => _robotVisualProfile.BreakSprite,
                _ => _robotVisualProfile.IdleSprite
            };

            if (sprite != null)
            {
                _robotSpriteRenderer.sprite = sprite;
            }
        }

        private IEnumerator PlayLaserShot(Vector3 from, Vector3 to)
        {
            if (laserViewPrefab != null)
            {
                var viewInstance = Instantiate(laserViewPrefab, transform);
                var presenter = viewInstance.GetComponent<LaserViewPresenter>();
                if (presenter == null)
                {
                    presenter = viewInstance.AddComponent<LaserViewPresenter>();
                }

                presenter.Configure(from, to, laserThickness, laserBodySprite, laserEmitterSprite, laserImpactSprite);
                yield return new WaitForSeconds(laserDurationSeconds <= 0f ? 0.1f : laserDurationSeconds);
                Destroy(viewInstance);
                yield break;
            }

            if (laserBodySprite != null || laserEmitterSprite != null || laserImpactSprite != null)
            {
                yield return PlaySpriteLaserShot(from, to);
                yield break;
            }

            var laserObject = new GameObject("LaserShot");
            var lineRenderer = laserObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
            lineRenderer.startWidth = 0.08f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = new Color(0.3f, 1f, 1f, 1f);
            lineRenderer.endColor = new Color(0.1f, 0.8f, 1f, 0.7f);

            yield return new WaitForSeconds(laserDurationSeconds <= 0f ? 0.1f : laserDurationSeconds);
            Destroy(laserObject);
        }

        private IEnumerator PlaySpriteLaserShot(Vector3 from, Vector3 to)
        {
            var laserRoot = new GameObject("LaserShotSprite").transform;
            laserRoot.SetParent(transform, false);

            var direction = to - from;
            var distance = direction.magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            var body = new GameObject("Body").transform;
            body.SetParent(laserRoot, false);
            body.position = (from + to) * 0.5f;
            body.rotation = Quaternion.Euler(0f, 0f, angle);

            var bodyRenderer = body.gameObject.AddComponent<SpriteRenderer>();
            if (laserBodySprite != null)
            {
                bodyRenderer.sprite = laserBodySprite;
                bodyRenderer.drawMode = SpriteDrawMode.Sliced;
                bodyRenderer.size = new Vector2(distance, Mathf.Max(0.05f, laserThickness));
            }
            else
            {
                bodyRenderer.sprite = laserEmitterSprite;
                body.localScale = new Vector3(distance, Mathf.Max(0.05f, laserThickness), 1f);
            }

            var emitter = new GameObject("Emitter").transform;
            emitter.SetParent(laserRoot, false);
            emitter.position = from;
            var emitterRenderer = emitter.gameObject.AddComponent<SpriteRenderer>();
            emitterRenderer.sprite = laserEmitterSprite != null ? laserEmitterSprite : laserBodySprite;

            var impact = new GameObject("Impact").transform;
            impact.SetParent(laserRoot, false);
            impact.position = to;
            var impactRenderer = impact.gameObject.AddComponent<SpriteRenderer>();
            impactRenderer.sprite = laserImpactSprite != null ? laserImpactSprite : laserBodySprite;

            yield return new WaitForSeconds(laserDurationSeconds <= 0f ? 0.1f : laserDurationSeconds);
            Destroy(laserRoot.gameObject);
        }

        private Vector3 GetLaserOrigin()
        {
            if (_robotViewBindings != null)
            {
                return _robotViewBindings.GetLaserMuzzlePosition();
            }

            return _robotMarker != null ? _robotMarker.position : transform.position;
        }
    }

    public sealed class RobotViewBindings : MonoBehaviour
    {
        [SerializeField] private Transform laserMuzzle;

        public Vector3 GetLaserMuzzlePosition()
        {
            return laserMuzzle != null ? laserMuzzle.position : transform.position;
        }
    }

    public sealed class LaserViewPresenter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer emitterRenderer;
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private SpriteRenderer impactRenderer;

        public void Configure(Vector3 from, Vector3 to, float thickness, Sprite bodySprite, Sprite emitterSprite, Sprite impactSprite)
        {
            EnsureRenderers();

            var direction = to - from;
            var distance = direction.magnitude;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            var bodyTransform = bodyRenderer.transform;
            bodyTransform.position = (from + to) * 0.5f;
            bodyTransform.rotation = Quaternion.Euler(0f, 0f, angle);
            bodyRenderer.sprite = bodySprite != null ? bodySprite : emitterSprite;
            bodyRenderer.drawMode = SpriteDrawMode.Sliced;
            bodyRenderer.size = new Vector2(distance, Mathf.Max(0.05f, thickness));

            emitterRenderer.transform.position = from;
            emitterRenderer.sprite = emitterSprite != null ? emitterSprite : bodySprite;

            impactRenderer.transform.position = to;
            impactRenderer.sprite = impactSprite != null ? impactSprite : bodySprite;
        }

        private void EnsureRenderers()
        {
            if (bodyRenderer == null)
            {
                var body = new GameObject("Body");
                body.transform.SetParent(transform, false);
                bodyRenderer = body.AddComponent<SpriteRenderer>();
            }

            if (emitterRenderer == null)
            {
                var emitter = new GameObject("Emitter");
                emitter.transform.SetParent(transform, false);
                emitterRenderer = emitter.AddComponent<SpriteRenderer>();
            }

            if (impactRenderer == null)
            {
                var impact = new GameObject("Impact");
                impact.transform.SetParent(transform, false);
                impactRenderer = impact.AddComponent<SpriteRenderer>();
            }
        }
    }
}
