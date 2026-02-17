using VProtocol.MiniGame.Runtime.Contracts;

namespace VProtocol.MiniGame.Runtime.BarrierSystem
{
    public sealed class BarrierSystemService : IBarrierSystem
    {
        public int CurrentLayers { get; private set; }
        public int MaxLayers { get; private set; }

        public void Initialize(int layers)
        {
            MaxLayers = layers < 1 ? 1 : layers;
            CurrentLayers = MaxLayers;
        }

        public bool ConsumeLayer()
        {
            if (CurrentLayers <= 0)
            {
                return false;
            }

            CurrentLayers--;
            return CurrentLayers > 0;
        }

        public void Reset()
        {
            CurrentLayers = MaxLayers;
        }
    }
}
