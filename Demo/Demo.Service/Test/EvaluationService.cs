using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using FluentValidation;
using System.Linq;

namespace Demo.Service
{
    public class EvaluationService : BaseService<Evaluation>, IBaseService<Evaluation>
    {
        void BindRules()
        {
            RuleFor(t => t.Title).Required().UniqAsync(t => t.ScopeId, t => t.Year, "سال ارزیابی با این عنوان تعریف شده است");
            RuleFor(t => t.Title).Required().Custom(t => Source != null && Source.Any(u => u != t && t.ScopeId == u.ScopeId && t.Title == u.Title && t.Year == u.Year), "سال ارزیابی با این عنوان تعریف شده است");
            RuleFor(t => t.EnTitle).UniqAsync("عنوان لاتین سال ارزیابی تکراری است");
            RuleFor(t => t.Year).Custom(t => t.Year < DateTime.Now.ToPersianDate().Year, "سال ارزیابی باید بزرگتر مساوی سال جاری باشد");
            RuleFor(t => t.EndDate).GreaterThanOrEqualTo(t => t.StartDate);
            RuleFor(t => t.EvalDateTo).GreaterThanOrEqualTo(t => t.EvalDateFrom);
            RuleFor(t => t.NotificationDateTo).GreaterThanOrEqualTo(t => t.NotificationDateFrom);
            RuleFor(t => t.ReviewDateTo).GreaterThanOrEqualTo(t => t.ReviewDateFrom);
        }

        public EvaluationService(IServiceProvider provider)
            : base(provider)
        {
            BindRules();
        }

        //public EvaluationService(IServiceProvider provider, Scope scope)
        //    :base(provider)
        //{
        //    BindRules();
        //    RuleFor(t => t.Title).Custom(t => scope.Evaluations.Any(u => t != u && t.Title == u.Title && t.ScopeId == u.ScopeId), "ارزیابی با این عنوان در سیستم تعریف شده است");
        //}
    }
}
