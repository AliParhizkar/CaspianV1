using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Scopes", Schema = "HR")]
    public class Scope
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        [CheckOnDelete("حوزه دارای اعتبارسنجی می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Evaluation> Evaluations { get; set; }  
    }
}
