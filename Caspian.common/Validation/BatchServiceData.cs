using System.Reflection;
using Caspian.Common.Extension;

namespace Caspian.Common
{
    public class BatchServiceData
    {
        public Type MasterType { get; set; }

        public int MasterId { get; set; }

        public PropertyInfo GetMasterInfo(Type type)
        {
            return type.GetForeignKey(MasterType);
        }

        public IList<PropertyInfo> DetailPropertiesInfo { get; set; }
    }
}
