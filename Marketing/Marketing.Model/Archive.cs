using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Archives", Schema = "Hr")]
    public class Archive: BaseEntity
    {
        [DisplayName("عنوان")]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int SecretariatId { get; set; }

        [ForeignKey(nameof(SecretariatId))]
        public Secretariat Secretariat { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public Archive Parent { get; set; }

        [CheckOnDelete("بایگانی دارای زیرشاخه می باشد و امکان حذف آن وجود ندارد.")]
        public ICollection<Archive> Archives { get; set; }
    }
}
