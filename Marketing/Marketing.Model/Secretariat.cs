using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Secretariat", Schema = "Hr")]
    public class Secretariat
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [DisplayName("پیش کد نامه های دریافتی")]
        public string PreCodeReceivedLetter { get; set; }

        [DisplayName("پیش کد نامه های ارسالی")]
        public string PreCodeSendedLetter { get; set; }

        [DisplayName("پیش کد نامه های موقت")]
        public string PreCodeTempLetter { get; set; }

        [DisplayName("اولویت")]
        public byte Periority { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }

        [DisplayName("محرمانه")]
        public bool IsIConfidential { get; set; }

        [DisplayName("شرح"), MaxLength(250)]
        public string Descript { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public Secretariat Parent { get; set; }

        [CheckOnDelete("دبیرخانه دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Secretariat> Children { get;set; }

        [CheckOnDelete("دبیرخانه دارای بایگانی می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Archive> Archives { get; set; }
    }
}
