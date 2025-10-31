using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public interface IPlayerAccount : IFeature
    {
        bool IsLoggedIn { get; }
        string PlayerId { get; }
        
        UniTask Login();
        UniTask Logout();
        
        //Task LinkCredentials(); 

        UniTask CreateNewPlayer();
    }
}