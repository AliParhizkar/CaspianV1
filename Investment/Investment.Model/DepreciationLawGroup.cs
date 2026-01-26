using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("DepreciationLawGroups", Schema = "ivm")]
    public class DepreciationLawGroup 
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام"), MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200), DisplayName("شرح")]
        public string Description { get; set; }

        [CheckOnDelete("گروه دارای قانون استهلاک می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<DepreciationLaw> DepreciationLaws { get; set; }
    }
}
