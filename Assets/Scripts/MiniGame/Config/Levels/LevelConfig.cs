using System.Collections.Generic;
using UnityEngine;
using VProtocol.MiniGame.Config.Enemies;
using VProtocol.MiniGame.Config.Math;
using VProtocol.MiniGame.Config.Waves;

namespace VProtocol.MiniGame.Config.Levels
{
    [CreateAssetMenu(
        fileName = "LevelConfig",
        menuName = "VProtocol/MiniGame/Config/Level",
        order = 0)]
    public sealed class LevelConfig : ScriptableObject
    {
        [System.Serializable]
        public sealed class CombatSettings
        {
            [SerializeField] private float fastAnswerThresholdSeconds = 2f;
            [SerializeField] private float comboWindowSeconds = 2.5f;

            public float FastAnswerThresholdSeconds => fastAnswerThresholdSeconds;
            public float ComboWindowSeconds => comboWindowSeconds;
        }

        [System.Serializable]
        public sealed class RobotVisualProfile
        {
            [SerializeField] private GameObject robotPrefab;
            [SerializeField] private Sprite idleSprite;
            [SerializeField] private Sprite shootSprite;
            [SerializeField] private Sprite hitSprite;
            [SerializeField] private Sprite breakSprite;
            [SerializeField] private RuntimeAnimatorController animatorController;
            [SerializeField] private Vector3 scale = Vector3.one * 1.2f;

            public GameObject RobotPrefab => robotPrefab;
            public Sprite IdleSprite => idleSprite;
            public Sprite ShootSprite => shootSprite;
            public Sprite HitSprite => hitSprite;
            public Sprite BreakSprite => breakSprite;
            public RuntimeAnimatorController AnimatorController => animatorController;
            public Vector3 Scale => scale;
        }

        [SerializeField] private int barrierLayers = 3;
        [SerializeField] private MathConfig mathConfig;
        [SerializeField] private WaveConfig waveConfig;
        [SerializeField] private List<EnemyArchetypeConfig> enemyArchetypes = new();
        [SerializeField] private CombatSettings combat = new();
        [SerializeField] private RobotVisualProfile robotVisual = new();

        public int BarrierLayers => barrierLayers;
        public MathConfig MathConfig => mathConfig;
        public WaveConfig WaveConfig => waveConfig;
        public IReadOnlyList<EnemyArchetypeConfig> EnemyArchetypes => enemyArchetypes;
        public CombatSettings Combat => combat;
        public RobotVisualProfile RobotVisual => robotVisual;

        public void Configure(
            int layers,
            MathConfig levelMathConfig,
            WaveConfig levelWaveConfig,
            List<EnemyArchetypeConfig> levelEnemyArchetypes)
        {
            barrierLayers = layers < 1 ? 1 : layers;
            mathConfig = levelMathConfig;
            waveConfig = levelWaveConfig;
            enemyArchetypes = levelEnemyArchetypes ?? new List<EnemyArchetypeConfig>();
        }
    }
}
