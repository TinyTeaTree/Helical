using System;
using System.Collections.Generic;

namespace Core
{
    public interface IBootstrap
    {
        TypeSet<IFeature> Features { get; }
        TypeSet<IService> Services { get; }
        TypeSet<IAgent> Agents { get; }
        Dictionary<Type, BaseFactory> Factories { get; }
        Dictionary<Type, BaseRecord> Records { get; }
    }
}