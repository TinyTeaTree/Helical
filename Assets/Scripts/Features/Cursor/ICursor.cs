using Core;

namespace Game
{
    public interface ICursor : IFeature
    {
        void SetCursorMode(AbilityMode mode);
    }
}