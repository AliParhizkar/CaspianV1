using System;

namespace Caspian.Common.Service
{
    public interface IEntity
    {
        MyContext Context { get; }
    }
}
