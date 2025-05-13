using System;

namespace Caspian.Common
{
    internal interface ICaspianValidator
    {
        BatchServiceData BatchServiceData { get; }

        int UserId { get; }
    }
}
