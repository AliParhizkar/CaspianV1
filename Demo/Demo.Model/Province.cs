using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Provinces", Schema = "demo")]
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// لیست شهرهای این استان
        /// </summary>
        [CheckOnDelete("استان دااری شهر می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<City> Cities { get; set; }
    }
}
