using Core;
using UnityEngine;

namespace Agents
{
    public class AppExitAgent : BaseAgent<IAppExitAgent>, IAppExitAgent
    {
        public static void SelfRegister(IAppExitAgent agent)
        {
            System.Action quitAction = null;
            quitAction = () =>
            {
                agent.AppExit();
                Application.quitting -= quitAction;
            };
            Application.quitting += quitAction;
        }
        
        public void AppExit()
        {
            foreach (var receiver in _features)
            {
                receiver.AppExit();
            }

            foreach (var receiver in _services)
            {
                receiver.AppExit();
            }
        }
    }
}