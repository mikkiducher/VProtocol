using UnityEngine;

namespace VProtocol.MiniGame.Config.Enemies
{
    public enum EnemyVariant
    {
        SpamBot = 0,
        BruteWorm = 1,
        Phisher = 2
    }

    [CreateAssetMenu(
        fileName = "EnemyArchetypeConfig",
        menuName = "VProtocol/MiniGame/Config/Enemy Archetype",
        order = 0)]
    public sealed class EnemyArchetypeConfig : ScriptableObject
    {
        [SerializeField] private string id = "SpamBot";
        [SerializeField] private EnemyVariant variant = EnemyVariant.SpamBot;
        [SerializeField] private int maxHp = 5;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private GameObject viewPrefab;
        [SerializeField] private Sprite viewSprite;
        [SerializeField] private RuntimeAnimatorController viewAnimatorController;
        [SerializeField] private Vector3 visualScale = Vector3.one * 0.8f;
        [SerializeField] private Vector3 visualOffset = Vector3.zero;

        public string Id => id;
        public EnemyVariant Variant => variant;
        public int MaxHp => maxHp;
        public float MoveSpeed => moveSpeed;
        public GameObject ViewPrefab => viewPrefab;
        public Sprite ViewSprite => viewSprite;
        public RuntimeAnimatorController ViewAnimatorController => viewAnimatorController;
        public Vector3 VisualScale => visualScale;
        public Vector3 VisualOffset => visualOffset;

        public void Configure(string archetypeId, EnemyVariant enemyVariant, int hp, float speed)
        {
            id = string.IsNullOrWhiteSpace(archetypeId) ? "SpamBot" : archetypeId;
            variant = enemyVariant;
            maxHp = hp < 1 ? 1 : hp;
            moveSpeed = speed < 0.1f ? 0.1f : speed;
            visualScale = Vector3.one * ResolveDefaultScale(enemyVariant);
            visualOffset = Vector3.zero;
        }

        private static float ResolveDefaultScale(EnemyVariant enemyVariant)
        {
            return enemyVariant switch
            {
                EnemyVariant.BruteWorm => 1.2f,
                EnemyVariant.Phisher => 0.9f,
                _ => 0.8f
            };
        }
    }
}
