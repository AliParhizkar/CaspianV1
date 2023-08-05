using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Countries ", Schema = "demo")]
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Foعلی"), Display(Name = "Gsadkj")]
        public string Title { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("The country has Cities and can not removed")]
        public virtual IList<City> Cities { get; set; }
    }
}
