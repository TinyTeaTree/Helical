using Core;

namespace Agents
{
    public class LoggedInAgent : BaseAgent<ILoggedInAgent>, ILoggedInAgent
    {
        public void LoggedIn()
        {
            foreach (var feature in _features)
            {
                feature.LoggedIn();
            }
        }
    }
}