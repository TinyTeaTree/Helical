using Core;

namespace Game
{
    public interface ILoadingScreen : IFeature
    {
        bool IsShowing { get; }

        void Show(LoadingScreenType type);
        void Hide();

        void SetProgress(float normalizedProgress);
    }
}