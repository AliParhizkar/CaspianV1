using Caspian.Engine;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model.History
{
    [WorkflowEntity("سوابق تحصیلی")]
    public class EducationRecords
    {
        [Key]
        public int Id { get; set; }

    }
}
