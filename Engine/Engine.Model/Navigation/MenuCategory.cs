using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("MenuCategories", Schema = "cmn")]
    public class MenuCategory
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        public SubsystemKind SubsystemKind { get; set; }

        [DisplayName("ICon")]
        public string IconFont { get; set; }

        public int Ordering { get; set; }

        [CheckOnDelete("منوی اصلی دارای منوی فرعی می باشد و امکان حذف آن وجود ندارد")]
        public IList<Menu> Menus { get; set; }
    }
}
