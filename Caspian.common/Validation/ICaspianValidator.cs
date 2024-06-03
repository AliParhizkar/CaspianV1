using System;

namespace Caspian.Common
{
    public interface ICaspianValidator
    {
        BatchServiceData BatchServiceData { get; set; }

        int UserId { get; }
    }
}
