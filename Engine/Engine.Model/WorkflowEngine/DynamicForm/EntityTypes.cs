using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("EntityTypes", Schema = "cmn")]
    public class EntityType
    {
        [Key]
        public int Id { get; set; }

        public SubSystemKind SubSystem { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public ValidationType ValidationType { get; set; }

        public string Title { get; set; }

        [CheckOnDelete("نوع موجودیت دارای فیلد می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<DataModelField> DataModelFields { get; set; }
    }
}
