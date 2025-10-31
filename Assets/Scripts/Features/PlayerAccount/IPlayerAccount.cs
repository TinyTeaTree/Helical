using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public interface IPlayerAccount : IFeature
    {
        UniTask Login();
        UniTask Logout();

        UniTask LinkCredentials();

        void CreateNewPlayer();
        UniTask SyncPlayerData();
    }
}