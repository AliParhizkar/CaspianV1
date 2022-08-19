using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("MenuCategories", Schema = "cmn")]
    public class MenuCategory
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public SubSystemKind SubSystemKind { get; set; }

        public string IconFont { get; set; }

        public int Ordering { get; set; }

        [CheckOnDelete("منوی اصلی دارای منوی فرعی می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Menu> Menus { get; set; }
    }
}
