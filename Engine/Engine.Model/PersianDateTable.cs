using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("PersianDatesTable", Schema = "cmn")]
    public class PersianDateTable
    {
        [Key]
        public DateTime DateTime { get; set; }

        public string PersianDate { get; set; }

        public int Year { get; set; }

        public PersianMonth Month { get; set; }

        public byte Day { get; set; }
    }
}
