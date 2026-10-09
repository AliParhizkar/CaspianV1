using System.Reflection;

namespace Caspian.Common
{
    public class BatchServiceData
    {
        public Type MasterType { get; set; }


        public IList<PropertyInfo> DetailPropertiesInfo { get; set; } = new List<PropertyInfo>();
    }
}
