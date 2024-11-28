using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Recruiting.Model
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        public virtual 

        public string Title { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }
    }
}
