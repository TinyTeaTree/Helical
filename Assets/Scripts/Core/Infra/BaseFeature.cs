using System;

namespace Core
{
    /// <summary>
    /// Feature Script
    /// </summary>
    public abstract class BaseFeature : IFeature
    {
        protected IBootstrap _bootstrap;
        
        private static readonly Type _injectType = typeof(InjectAttribute);
        private static readonly Type _injectableType = typeof(IInjectableInterface);
        private static readonly Type _recordType = typeof(BaseRecord);
        
        public virtual void Bootstrap(IBootstrap bootstrap)
        {
            _bootstrap = bootstrap;
            Type type = this.GetType();

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.HasAttribute(_injectType))
                {
                    var propertyType = property.PropertyType;
                    if (_injectableType.IsAssignableFrom(propertyType))
                    {
                        // Try to get from features first
                        if (bootstrap.Features.ContainsKey(propertyType))
                        {
                            var feature = bootstrap.Features.Get(propertyType);
                            property.SetValue(this, feature);
                        }
                        // Then try resolvers
                        else if (bootstrap.Resolvers.ContainsKey(propertyType))
                        {
                            var resolver = bootstrap.Resolvers.Get(propertyType);
                            property.SetValue(this, resolver);
                        }
                        // Then try services
                        else if (bootstrap.Services.ContainsKey(propertyType))
                        {
                            var service = bootstrap.Services.Get(propertyType);
                            property.SetValue(this, service);
                        }
                        // Then try agents
                        else if (bootstrap.Agents.ContainsKey(propertyType))
                        {
                            var agent = bootstrap.Agents.Get(propertyType);
                            property.SetValue(this, agent);
                        }
                        else
                        {
                            Notebook.NoteError($"[Inject] can't work. Property {property.Name} type {property.PropertyType} is not registered");
                        }
                    }
                    else if (_recordType.IsAssignableFrom(propertyType))
                    {
                        var record = bootstrap.Records[propertyType];
                        property.SetValue(this, record);
                    }
                    else
                    {
                        Notebook.NoteError($"[Inject] can't work. Property {property.Name} type {property.PropertyType} is not an {nameof(IInjectableInterface)} or {nameof(BaseRecord)}");
                    }
                }
            }
        }
    }
}