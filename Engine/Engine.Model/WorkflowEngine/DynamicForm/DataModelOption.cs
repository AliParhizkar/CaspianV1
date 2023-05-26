using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("DataModelOptions", Schema = "cmn")]
    public class DataModelOption
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("فیلد")]
        public int FieldId { get; set; }

        [ForeignKey(nameof(FieldId))]
        public virtual DataModelField Field { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }
    }
}
