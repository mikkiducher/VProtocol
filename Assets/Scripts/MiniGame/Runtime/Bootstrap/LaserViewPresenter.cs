using UnityEngine;

namespace VProtocol.MiniGame.Runtime.Bootstrap
{
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
