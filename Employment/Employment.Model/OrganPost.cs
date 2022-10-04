using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("OrganPosts", Schema = "emp")]
    public class OrganPost
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("رشته شغلی")]
        public int CareerFieldId { get; set; }

        [ForeignKey(nameof(CareerFieldId))]
        public virtual CareerField CareerField { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("واحد سازمانی")]
        public int OrganUnitId { get; set; }

        [ForeignKey(nameof(OrganUnitId))]
        public virtual OrganUnit OrganUnit { get; set; }

        public virtual ICollection<ChildrenProperties> ChildrenProperties{ get; set; }
    }
}
