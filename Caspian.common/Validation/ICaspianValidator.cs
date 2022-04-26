using System;

namespace Caspian.Common
{
    public interface ICaspianValidator
    {
        static IServiceProvider Provider { get; set; }

        bool IgnoreDetailsProperty { get; set; }
    }
}
