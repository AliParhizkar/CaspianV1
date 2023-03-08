using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Menus", Schema = "cmn")]
    public class Menu
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Source")]
        public string Source { get; set; }

        [DisplayName("Menu category")]
        public int? MenuCategoryId { get; set; }

        [ForeignKey(nameof(MenuCategoryId))]
        public virtual  MenuCategory MenuCategory { get; set; }

        public SubSystemKind? SubSystemKind { get; set; }

        public int Ordering { get; set; }

        [DisplayName("Show in menu")]
        public bool ShowonMenu { get; set; }

        /// <summary>
        /// Addresses that exist in the database but are not in the system
        /// </summary>
        [DisplayName("Invalid address")]
        public bool InvalidAddress { get; set; }

        [CheckOnDelete("The menu has access and cannot be deleted")]
        public IList<MenuAccessibility> Accessibilities { get; set; }
    }
}
