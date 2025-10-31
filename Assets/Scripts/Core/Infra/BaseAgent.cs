using System.Collections.Generic;

namespace Core
{
    public abstract class BaseAgent
    {
        public abstract void Setup(IEnumerable<IFeature> features, IEnumerable<IService> services);
    }
    
    public class BaseAgent<TypeAgent> : BaseAgent, IAgent
        where TypeAgent : IAgent
    {
        protected List<TypeAgent> _features = new();
        protected List<TypeAgent> _services = new();
        
        public override void Setup(IEnumerable<IFeature> features, IEnumerable<IService> services)
        {
            foreach (var feature in features)
            {
                if (feature is TypeAgent agentFeature)
                {
                    _features.Add(agentFeature);
                }
            }
            
            foreach (var service in services)
            {
                if (service is TypeAgent agentServices)
                {
                    _services.Add(agentServices);
                }
            }
        }

    }
}