using System;
using System.Collections.Generic;

namespace Core
{
    public interface IFeature : IInjectableInterface
    {
        void Bootstrap(IBootstrap bootstrap);
    }
}