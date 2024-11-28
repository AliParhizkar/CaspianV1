using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruiting.Model
{
    [Table("Countries", Schema = "Rec")]
    public class Country
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }
    }
}
