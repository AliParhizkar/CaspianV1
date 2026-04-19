using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplierEvaluationService: MasterDetailsService<SupplierEvaluation, SupplierEvaluationDetail>
    {
        public SupplierEvaluationService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.No).Required().UniqueAsync("ارزیابی با این شماره در سیستم ثبت شده است");
            RuleFor(t => t.EvaluationBaseType).Custom(t => t.EvaluationBaseType.HasValue && t.EvaluationBaseType != EvaluationBaseType.Rasid,
                "در حال حاضر این حالت پشتیبانی نمی شود.");
            RuleFor(t => t.ReceiptId).Required(t => t.EvaluationBaseType == EvaluationBaseType.Rasid);
            RuleFor(t => t.StartDate).Required();
            RuleFor(t => t.EndDate).Required().Custom(t => t.EndDate <= t.StartDate, "تاریخ پایان ارزیابی باید بزرگتر از تاریخ شروع ارزیابی باشد");
            RuleForEach(t => t.Details).SetValidator(new SupplierEvaluationDetailService(provider));
        }
    }
}
