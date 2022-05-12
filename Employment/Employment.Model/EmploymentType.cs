
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    public class EmploymentOrderType
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual ICollection<EmploymentOrder> Employments { get; set; }
    }
}
