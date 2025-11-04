using Core;

namespace Game
{
    public interface IGridSelection : IFeature
    {
        void Start();
        void Halt();
        void SetAbilityMode(AbilityMode mode);
    }
}