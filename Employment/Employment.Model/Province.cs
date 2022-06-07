using Caspian.Common;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    public class Province
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [CheckOnDelete]
        public virtual IList<City> Cities { get; set; }
    }
}
