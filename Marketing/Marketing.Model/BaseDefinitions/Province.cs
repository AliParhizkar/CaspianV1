using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Provinces", Schema = "mrk")]
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کشور")]
        public int CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }

        [CheckOnDelete("استان دارای شهر می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<City> Cities { get; set; }
    }
}
