using Caspian.Common;
using FluentValidation;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplierEvaluationDetailService: BaseService<SupplierEvaluationDetail>
    {
        public SupplierEvaluationDetailService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Privilege).CustomValue(t => t < 0, "امتیاز باید بزرگتر از صفر باشد")
                .CustomAsync(async t =>
                {
                    if (t.EvaluationIndicatorId == 0)
                        return null;
                    var evaluation = await provider.GetCaspianService<EvaluationIndicatorService>().SingleAsync(t.EvaluationIndicatorId);
                    if (t.Privilege > evaluation.Maximum || t.Privilege < evaluation.Minimum)
                        return $"امتیاز باید بین مقادیر {evaluation.Minimum} تا {evaluation.Maximum} باشد";
                    return null;
                });
            RuleFor(t => t.EvaluationIndicatorId).Custom(t => Source.Any(u => t.Id != u.Id &&
                t.SupplierEvaluationId == u.SupplierEvaluationId && t.EvaluationIndicatorId == t.EvaluationIndicatorId
            ), "این شاخص برای ارزیابی امتیازدهی شده است");
        }
    }
}
