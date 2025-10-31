using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class TypeSet<T>
    {
        private Dictionary<System.Type, T> _set = new();

        public void Add<IFeatureType>(T feature)
            where IFeatureType : T
        {
            _set.Add(typeof(IFeatureType), feature);
        }

        public void Replace<IFeatureType>(T feature)
            where IFeatureType : T
        {
            _set[typeof(IFeatureType)] = feature;
        }

        public IFeatureType Get<IFeatureType>()
            where IFeatureType : T
        {
            var feature = _set[typeof(IFeatureType)];
            return (IFeatureType)feature;
        }
        
        public T Get(Type featureType)
        {
            var feature = _set[featureType];
            return feature;
        }

        public IEnumerable<T> GetAll()
        {
            return _set.Values;
        }

        public IEnumerable<KeyValuePair<System.Type, T>> GetKVP()
        {
            return _set.ToList();
        }
    }
}