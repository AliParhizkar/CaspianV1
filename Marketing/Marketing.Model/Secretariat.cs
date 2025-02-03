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

        [Required]
        public string Name { get; set; }

        [DisplayName("پیش کد نامه های دریافتی")]
        public string PreCodeReceivedLetter { get; set; }

        
        public string PostCodeReceivedLetter { get; set; }

        public string PreCodeSendedLetter { get; set; }

        public string PostCodeSendedLetter { get; set; }
    }
}
