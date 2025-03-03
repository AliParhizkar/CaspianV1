using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Reports", Schema = "cmn")]
    public class PersianDateTable
    {
        [Key]
        public DateOnly Date { get; set; }

        public string PersianDate { get; set; }

        public short Year { get; set; }

        public PersianMonth Month { get; set; }

        public byte Day { get; set; }

        public DayOfPersianWeek DayOfPersianWeek { get; set; }
    }
}