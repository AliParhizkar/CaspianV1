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

        [DisplayName("حوزه")]
        public int ScopeId { get; set; }

        [ForeignKey(nameof(ScopeId))]
        public Scope Scope { get; set; }

        [DisplayName("عنوان سال ارزیابی")]
        public string Title { get; set; }

        [DisplayName("عنوان لاتین سال ارزیابی")]
        public string EnTitle { get; set; }

        [DisplayName("سال ارزیابی")]
        public int Year { get; set; }

        [DisplayName("نوبت ارزیابی")]
        public EvaluationTurn? EvaluationTurn { get; set; }

        [DisplayName("تاریخ شروع سال ارزیابی")]
        public DateOnly? StartDate { get; set; }

        [DisplayName("تاریخ پایان سال ارزیابی")]
        public DateOnly? EndDate { get; set; }

        [DisplayName("تاریخ گردش ارزیابی از")]
        public DateOnly? EvalDateFrom { get; set; }

        [DisplayName("تاریخ گردش ارزیابی تا")]
        public DateOnly? EvalDateTo { get; set; }

        [DisplayName("تاریخ اعتراض از")]
        public DateOnly? ReviewDateFrom { get; set; }

        [DisplayName("تاریخ اعتراض تا")]
        public DateOnly? ReviewDateTo { get;set; }

        [DisplayName("تاریخ ابلاغ از")]
        public DateOnly? NotificationDateFrom { get; set; }

        [DisplayName("تاریخ ابلاغ تا")]
        public DateOnly? NotificationDateTo { get; set; }
    }

    public enum EvaluationTurn: byte
    {
        [Display(Name = "دوره اول")]
        Turn1 = 1, 

        [Display(Name = "دوره دوم")]
        Turn2 = 2,

        [Display(Name = "دوره سوم")]
        Turn3 = 3,

        [Display(Name = "دوره چهارم")]
        Turn4 = 4,
    }
}
