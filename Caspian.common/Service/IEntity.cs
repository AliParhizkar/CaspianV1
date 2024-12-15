using System;

namespace Caspian.Common.Service
{
    public interface IEntity
    {
        CaspianContext Context { get; }
    }
}
