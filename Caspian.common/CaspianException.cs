using System;

namespace Caspian.Common
{
    public class CaspianException : Exception
    {
        internal int? _HelpId;
        public CaspianException(string message, int? helpId = null)
            : base(message)
        {
            _HelpId = helpId;
        }
    }

    public class MyException : Exception
    {
        public MyException(string message)
            : base(message)
        {

        }
    }
}
