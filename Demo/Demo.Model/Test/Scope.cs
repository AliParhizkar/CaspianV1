using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Scopes", Schema = "HR")]
    public class Scope
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("عنوان لاتین")]
        public string EnTitle { get; set; }

        [CheckOnDelete("حوزه دارای اعتبارسنجی می باشد و امکان حذف آن وجود ندارد")]
        public IList<Evaluation> Evaluations { get; set; }  
    }
}
