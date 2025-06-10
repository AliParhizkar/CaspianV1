using System.Reflection;

namespace Caspian.Common
{
    public class BatchServiceData
    {
        public Type MasterType { get; set; }

        public PropertyInfo ThirdLevelProperty { get; set; }

        public int MasterId { get; set; }

        public IList<PropertyInfo> DetailPropertiesInfo { get; set; } = new List<PropertyInfo>();
    }
}
