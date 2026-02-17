namespace VProtocol.MiniGame.Runtime.Contracts
{
    public interface IBarrierSystem
    {
        int CurrentLayers { get; }
        int MaxLayers { get; }
        void Initialize(int layers);
        bool ConsumeLayer();
        void Reset();
    }
}
