using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class InsuranceCompanyService: BaseService<InsuranceCompany>
    {
        public InsuranceCompanyService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("شرکت بیمه ای با این کد در سیستم ثبت شده است");
            RuleFor(t => t.Name).Required().UniqueAsync("شرکت بیمه ای با این نام در سیستم ثبت شده است");
            RuleFor(t => t.AccountingCodeId).UniqueAsync("شرکت بیمه ای با این کد حسابداری در سیستم ثبت شده است");
            RuleFor(t => t.NationalCode).UniqueAsync("شرکت بیمه ای با این شناسه ملی در سیستم ثبت شده است");
        }
    }
}
