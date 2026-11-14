using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using FluentValidation;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class EvaluationService : DetailService<Evaluation>
    {
        public EvaluationService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.ScopeId, t => t.Year, "سال ارزیابی با این عنوان تعریف شده است");
            RuleFor(t => t.Title).Required().Custom(t => Source.Any(u => u != t && t.ScopeId == u.ScopeId && t.Title == u.Title && t.Year == u.Year), "سال ارزیابی با این عنوان تعریف شده است");
            RuleFor(t => t.EnTitle).UniqueAsync("عنوان لاتین سال ارزیابی تکراری است");
            RuleFor(t => t.Year).Custom(t => t.Year < DateTime.Now.ToPersianDate().Year, "سال ارزیابی باید بزرگتر مساوی سال جاری باشد");
            RuleFor(t => t.EndDate).GreaterThan(t => t.StartDate).WithMessage("تاریخ پایان نمی تواند قبل از تاریخ شروع باشد");
            RuleFor(t => t.EvalDateTo).GreaterThan(t => t.EvalDateFrom).WithMessage("تاریخ پایان نمی تواند قبل از تاریخ شروع باشد");
            RuleFor(t => t.NotificationDateTo).GreaterThan(t => t.NotificationDateFrom).WithMessage("تاریخ پایان نمی تواند قبل از تاریخ شروع باشد");
            RuleFor(t => t.ReviewDateTo).GreaterThan(t => t.ReviewDateFrom).WithMessage("تاریخ پایان نمی تواند قبل از تاریخ شروع باشد");
        }
    }
}
