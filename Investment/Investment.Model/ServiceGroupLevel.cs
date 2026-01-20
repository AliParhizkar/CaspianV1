using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("ServiceGroupLevel", Schema = "ivm")]
    public class ServiceGroupLevel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام گروه")]
        public string Name { get; set; }

        [DisplayName("طول سطح گروه")]
        public int Length { get; set; }

        [DisplayName("نوع سطح گروه")]
        public ServiceGroupLevelType LevelType { get; set; }

        [DisplayName("توضیحات"), MaxLength(200)]
        public string Description { get; set; }
    }
}
