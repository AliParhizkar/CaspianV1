using Caspian.Engine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{

    public class ForTestEmployment
    {
        [Key]
        public int Id { get; set; }

        [ReportField("مشخصات حکم"), ForeignKey("dsadas")]
        public virtual ParametricEmploymentOrder Order { get; set; }
    }
}
