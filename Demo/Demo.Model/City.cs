using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Cities", Schema = "demo")]
    public class City
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }
    }
}
