using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;

namespace Caspian.Common.RowNumber
{
    public class SqlServerMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
    {
        public IEnumerable<IMethodCallTranslator> Translators { get; }

        public SqlServerMethodCallTranslatorPlugin()
        {
            Translators = new List<IMethodCallTranslator>
                    {
                        new SqlServerRowNumberTranslator()
                    };
        }
    }
}
