using Caspian.Common;
using System.Reflection;
using System.Collections;
using Caspian.Engine.Model;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    public class ReportLevelData
    {
        public int? Key { get; set; }

        public object Master { get; set; }

        public IList Values { get; set; }

        public IList<ReportLevelData> Details { get; set; }
    }
}
