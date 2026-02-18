using TMPro;
using UnityEngine;

namespace VProtocol.MiniGame.Runtime.EnemySystem
{
    public sealed class EnemyViewBindings : MonoBehaviour
    {
        [SerializeField] private Transform hitPoint;
        [SerializeField] private Transform hpLabelAnchor;
        [SerializeField] private TextMeshPro hpLabel;

        public Vector3 GetHitPointPosition()
        {
            return hitPoint != null ? hitPoint.position : transform.position;
        }

        public Transform HpLabelAnchor => hpLabelAnchor != null ? hpLabelAnchor : transform;
        public TextMeshPro HpLabel => hpLabel;
    }
}
