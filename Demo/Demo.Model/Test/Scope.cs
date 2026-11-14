using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Scopes", Schema = "demo")]
    public class Scope
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Scope Title")]
        public string Title { get; set; }

        [DisplayName("Scope Title En")]
        public string EnTitle { get; set; }

        [CheckOnDelete("The scope is referenced in validation rules and cannot be deleted.")]
        public IList<Evaluation> Evaluations { get; set; }  
    }
}
