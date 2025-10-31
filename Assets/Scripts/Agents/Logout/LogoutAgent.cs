using Core;

namespace Agents
{
    public class LogoutAgent : BaseAgent<ILogoutAgent>, ILogoutAgent
    {
        public void Logout()
        {
            foreach (var feature in _features)
            {
                feature.Logout();
            }

            foreach (var service in _services)
            {
                service.Logout();
            }
        }
    }
}