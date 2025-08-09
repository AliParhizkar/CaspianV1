using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("Supplier", Schema = "wh")]
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کاربری مرتبط")]
        public int RelatedUserId { get; set; }

        public User RelatedUser { get; set; }

        [DisplayName("کد ملی")]
        public string IdCard { get; set; }

        [DisplayName("نام پدر")]
        public string ParentName { get; set; }
    }
}
