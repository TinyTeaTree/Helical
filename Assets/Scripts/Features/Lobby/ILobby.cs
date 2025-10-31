using Core;

namespace Game
{
    public interface ILobby : IFeature
    {
        void Show();

        void DisplayOptions();

        void Hide();
    }
}