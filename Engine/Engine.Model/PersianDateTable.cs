using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Engine.Model
{
    [Table("PersianDatesTable", Schema = "cmn")]
    public class PersianDateTable
    {
        [Key]
        public DateTime DateTime { get; set; }

        public string PersianDate { get; set; }
    }
}
