using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        public int ProvinceId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public virtual Province Province { get; set; }

        public string Title { get; set; }

        public virtual ICollection<ChildrenProperties> ChildrenProperties{ get; set; }
    }
}
