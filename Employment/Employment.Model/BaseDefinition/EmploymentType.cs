
using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("EmploymentOrderTypes", Schema = "emp")]
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

        /// <summary>
        /// مشخصات حکم هایی که از این نوع
        /// </summary>
        [CheckOnDelete("نوع حکم دارای حکم می باشد و امکان حذف آن وجود ندارد")]
        public virtual ICollection<ParametricEmploymentOrder> Employments { get; set; }
    }
}
