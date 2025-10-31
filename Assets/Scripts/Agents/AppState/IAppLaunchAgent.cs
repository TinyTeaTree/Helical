using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public interface IAppLaunchAgent : IAgent
    {
        UniTask AppLaunch();
    }
}