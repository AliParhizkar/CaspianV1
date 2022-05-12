using Caspian.Engine;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    [WorkflowEntity("وضعیت تاهل")]
    public class MarriageProperties
    {
        [Key]
        public int Id { get; set; }

        public string FName { get; set; }

        public string LName { get; set; }

        public DateTime MarriageDate { get; set; } = DateTime.Now;

        public DateTime? BirthDate { get;set; } = DateTime.Now;

        public BaseStudy? BaseStudy { get; set; }
    }
}
