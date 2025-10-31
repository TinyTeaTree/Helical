using System;

namespace Core
{
    public abstract class BaseFeature : IFeature
    {
        protected IBootstrap _bootstrap;
        
        private static readonly Type _injectType = typeof(InjectAttribute);
        private static readonly Type _featureType = typeof(IFeature);
        private static readonly Type _serviceType = typeof(IService);
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
                    if (_featureType.IsAssignableFrom(propertyType))
                    {
                        var feature = bootstrap.Features.Get(propertyType);
                        property.SetValue(this, feature);
                    }
                    else if (_serviceType.IsAssignableFrom(propertyType))
                    {
                        var service = bootstrap.Services.Get(propertyType);
                        property.SetValue(this, service);
                    }
                    else if (_recordType.IsAssignableFrom(propertyType))
                    {
                        var record = bootstrap.Records[propertyType];
                        property.SetValue(this, record);
                    }
                    else
                    {
                        Notebook.NoteError($"[Inject] can't work. Property {property.Name} type {property.PropertyType} is not an {nameof(IFeature)} or {nameof(IService)}");
                    }
                }
            }
        }
    }
}