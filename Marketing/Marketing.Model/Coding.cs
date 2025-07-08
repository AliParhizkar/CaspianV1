using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Coding", Schema = "mrk")]
    public class Coding
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد کل")]
        public string TotalCode { get; set; }

        [DisplayName("عنوان کل")]
        public string TotalTitle { get; set; }

        [DisplayName("کد معین")]
        public string AdjuvantCode { get; set; }

        [DisplayName("عنوان معین")]
        public string AdjuvantTitle { get; set; }


    }
}