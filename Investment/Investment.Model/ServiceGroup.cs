using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("ServiceGroups", Schema = "ivm")]
    public class ServiceGroup
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد گروه خدمات")]
        public string Code { get; set; }

        [DisplayName("نام گروه خدمات")]
        public string Name { get; set; }

        [DisplayName("سطح کدینگ")]
        public ServiceGroupLevelType ServiceGroupLevelType { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public ServiceGroup ParentServiceGroup { get; set; }

        [DisplayName("ملاحظات"), MaxLength(200)]
        public string Description { get; set; }

        [CheckOnDelete("گروه سرویس دارای زیرگروه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ServiceGroup> ServiceGroups { get; set; } 
    }
}
