using System.Reflection;
using Caspian.Common.Extension;

namespace Caspian.Common
{
    public class BatchServiceData
    {
        public BatchServiceData() 
        {
            DetailPropertiesInfo = new List<PropertyInfo>();
        }

        public Type MasterType { get; set; }

        public PropertyInfo ThirdLevelProperty { get; set; }

        public int MasterId { get; set; }

        public IList<PropertyInfo> DetailPropertiesInfo { get; set; }
    }
}
