using Caspian.Engine;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model.History
{
    [WorkflowEntity("سوابق سازمانی")]
    public class WorkHistory
    {
        [Key]
        public int Id { get; set; }
    }
}
