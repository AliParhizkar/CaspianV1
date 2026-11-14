using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Evaluations", Schema = "demo")]
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Scope")]
        public int ScopeId { get; set; }

        [ForeignKey(nameof(ScopeId))]
        public Scope Scope { get; set; }

        [DisplayName("Evaluation Title")]
        public string Title { get; set; }

        [DisplayName("Evaluation Title En")]
        public string EnTitle { get; set; }

        [DisplayName("Evaluation Year")]
        public int Year { get; set; }

        [DisplayName("Evaluation Period")]
        public EvaluationTurn? EvaluationTurn { get; set; }

        [DisplayName("Evaluation Start Date")]
        public DateOnly? StartDate { get; set; }

        [DisplayName("Evaluation End Date")]
        public DateOnly? EndDate { get; set; }

        [DisplayName("Evaluation Process From")]
        public DateOnly? EvalDateFrom { get; set; }

        [DisplayName("Evaluation Process To")]
        public DateOnly? EvalDateTo { get; set; }

        [DisplayName("Appeal Start Date")]
        public DateOnly? ReviewDateFrom { get; set; }

        [DisplayName("Appeal End Date")]
        public DateOnly? ReviewDateTo { get;set; }

        [DisplayName("Notification Start Date")]
        public DateOnly? NotificationDateFrom { get; set; }

        [DisplayName("Notification End Date")]
        public DateOnly? NotificationDateTo { get; set; }
    }

    public enum EvaluationTurn: byte
    {
        [Display(Name = "First Period")]
        Turn1 = 1, 

        [Display(Name = "Second Period")]
        Turn2 = 2,

        [Display(Name = "Third Period")]
        Turn3 = 3,

        [Display(Name = "Fourth Period")]
        Turn4 = 4,
    }
}
