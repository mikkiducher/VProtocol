using UnityEngine;

namespace VProtocol.MiniGame.Runtime.Bootstrap
{
    public sealed class RobotViewBindings : MonoBehaviour
    {
        [SerializeField] private Transform laserMuzzle;

        public Vector3 GetLaserMuzzlePosition()
        {
            return laserMuzzle != null ? laserMuzzle.position : transform.position;
        }
    }
}
