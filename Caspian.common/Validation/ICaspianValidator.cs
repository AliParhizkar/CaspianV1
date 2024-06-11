using System;

namespace Caspian.Common
{
    internal interface ICaspianValidator
    {
        BatchServiceData BatchServiceData { get; set; }

        int UserId { get; }
    }
}
