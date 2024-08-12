using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Engine.Model
{
    [Table("PersianDateConvertor")]
    public class PersianDateConvertor
    {
        [Key]
        public DateTime Date { get; set; }

        public short Year { get; set; }

        public PersianMonth Month { get; set; }

        public byte Day {  get; set; }

        public DayOfWeek DayOfWeek { get; set; }
    }
}
