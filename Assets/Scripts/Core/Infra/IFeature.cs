using System;
using System.Collections.Generic;

namespace Core
{
    public interface IFeature
    {
        void Bootstrap(IBootstrap bootstrap);
    }
}