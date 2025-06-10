using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Countries", Schema = "mrk")]
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [CheckOnDelete("کشور دارای استان می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Province> Provinces { get; set; }

        [CheckOnDelete("کشور دارای شهر می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<City> Cities { get; set; }
    }
}
