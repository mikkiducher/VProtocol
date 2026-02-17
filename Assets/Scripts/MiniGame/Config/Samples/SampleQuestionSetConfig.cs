using UnityEngine;

namespace VProtocol.MiniGame.Config.Samples
{
    [CreateAssetMenu(
        fileName = "SampleQuestionSetConfig",
        menuName = "VProtocol/MiniGame/Config/Sample Question Set",
        order = 0)]
    public sealed class SampleQuestionSetConfig : ScriptableObject
    {
        [SerializeField] private TextAsset source;

        public TextAsset Source => source;
    }
}
