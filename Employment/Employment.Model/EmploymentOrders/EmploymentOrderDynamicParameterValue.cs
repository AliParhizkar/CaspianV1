﻿using Caspian.Engine;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("EmploymentOrderDynamicParameterValues", Schema = "emp")]
    public class EmploymentOrderDynamicParameterValue : DynamicParameterValue
    {
        public int OrderID { get; set; }

        [ForeignKey(nameof(OrderID))]
        public virtual ParametricEmploymentOrder EmploymentOrder { get; set; }
    }
}
