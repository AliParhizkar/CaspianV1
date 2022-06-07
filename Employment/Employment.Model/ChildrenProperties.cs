using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [WorkflowEntity("مشخصات فرزندان")]
    public class ChildrenProperties
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string FName { get; set; }

        [DisplayName("نام خانوادگی")]
        public string LName { get; set; }

        [DisplayName("تاریخ تولد")]
        public DateTime BirthDate { get; set; }

        [DisplayName("جنسیت")]
        public Gender Gender { get; set; }
        
        [DisplayName("تاریخ ازدواج")]
        public DateTime? MarriageDate { get; set; }

        public byte? BirthOrder { get; set; }

        [DisplayName("شهر محل تولد")]
        public int BirthCityId { get; set; }

        [ForeignKey(nameof(BirthCityId))]
        public virtual City BirthCity { get; set; }
    }
}
