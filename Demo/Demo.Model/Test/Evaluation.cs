using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Evaluations", Schema = "HR")]
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("حوزه")]
        public int ScopeId { get; set; }

        [ForeignKey(nameof(ScopeId))]
        public virtual Scope Scope { get; set; }

        [DisplayName("عنوان سال ارزیابی")]
        public string Title { get; set; }

        [DisplayName("عنوان لاتین سال ارزیابی")]
        public string EnTitle { get; set; }

        [DisplayName("سال ارزیابی")]
        public int Year { get; set; }

        [DisplayName("نوبت ارزیابی")]
        public EvaluationTurn? EvaluationTurn { get; set; }

        [DisplayName("تاریخ شروع سال ارزیابی")]
        public DateTime? StartDate { get; set; }

        [DisplayName("تاریخ پایان سال ارزیابی")]
        public DateTime? EnddDate { get; set; }

        [DisplayName("تاریخ گردش ارزیابی از")]
        public DateTime? EvalDateFrom { get; set; }

        [DisplayName("تاریخ گردش ارزیابی تا")]
        public DateTime? EvalDateTo { get; set; }

        [DisplayName("تاریخ اعتراض از")]
        public DateTime? ReviewDateFrom { get; set; }

        [DisplayName("تاریخ اعتراض تا")]
        public DateTime? ReviewDateTo { get;set; }

        [DisplayName("تاریخ ابلاغ از")]
        public DateTime? NotificationDateFrom { get; set; }

        [DisplayName("تاریخ ابلاغ تا")]
        public DateTime? NotificationDateTo { get; set; }
    }

    public enum EvaluationTurn
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
