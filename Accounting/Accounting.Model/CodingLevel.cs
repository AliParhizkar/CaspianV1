using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Model
{
    [Table("CodingLevels", Schema = "acc")]
    public class CodingLevel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام سطح")]
        public string Name { get; set; }

        [DisplayName("طول سطح")]
        public int Length { get; set; }

        [DisplayName("نوع سطح")]
        public CodingLevelType CodingLevelType { get; set; }

        [MaxLength(200), DisplayName("توضیحات")]
        public string Description { get; set; }
    }
}
