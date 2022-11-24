
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    public class EmploymentOrderType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان"), ReportField("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد"), ReportField("کد")]
        public string Code { get; set; }

        [DisplayName("شرح")]
        public string Description { get; set; }

        public virtual ICollection<ParametricEmploymentOrder> Employments { get; set; }
    }
}
