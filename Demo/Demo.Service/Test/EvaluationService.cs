using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using FluentValidation;

namespace Demo.Service
{
    public class EvaluationService : BaseService<Evaluation>, IBaseService<Evaluation>
    {
        public EvaluationService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("سال ارزیابی با این عنوان تعریف شده است");
            RuleFor(t => t.EnTitle).UniqAsync("عنوان لاتین سال ارزیابی تکراری است");
            RuleFor(t => t.Year).Custom(t => t.Year >= DateTime.Now.ToPersianDate().Year, "سال ارزیابی باید بزرگتر مساوی سال جاری باشد");
            RuleFor(t => t.EnddDate).LessThanOrEqualTo(t => t.StartDate);
            RuleFor(t => t.EvalDateTo).LessThanOrEqualTo(t => t.EvalDateFrom);
            RuleFor(t => t.NotificationDateTo).LessThanOrEqualTo(t => t.NotificationDateFrom);
            RuleFor(t => t.ReviewDateTo).LessThanOrEqualTo(t => t.ReviewDateFrom);
        }
    }
}
