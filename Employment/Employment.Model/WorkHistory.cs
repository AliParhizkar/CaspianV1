using Caspian.Engine;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    [WorkflowEntity("سوابق سازمانی")]
    public class WorkHistory
    {
        [Key]
        public int Id { get; set; }
    }
}
