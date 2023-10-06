using System.Reflection;

namespace Caspian.UI
{
    public class BatchService
    {
        public PropertyInfo IgnorePropertyInfo { get; set; }

        public int MasterId { get; set; }

        /// <summary>
        /// For example in customer membership Customer type is Master type Membership is Detail type and Group type is other type
        /// </summary>
        public PropertyInfo OtherTypeInfo { get; set; }
    }
}
