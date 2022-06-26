using Caspian.Engine;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    public class EmploymentOrderDynamicParameterValue : DynamicParameterValue
    {
        public int OrderID { get; set; }

        [ForeignKey(nameof(OrderID))]
        public virtual EmploymentOrder EmploymentOrder { get; set; }
    }
}
