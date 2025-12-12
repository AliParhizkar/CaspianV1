using Accounting.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Service
{
    public class AccountingCodeService : BaseService<AccountingCode>
    {
        public AccountingCodeService(IServiceProvider provider) : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync(t => t.ParentCodeId, " جسابی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.CodingLevelType)
                .Custom(t => t.CodingLevelType != CodingLevelType.Level1 && t.ParentCodeId == null, "فقط کدهای سطح اول می توانند پدر نداشته باشند");
            RuleFor(t => t.Code).Required().UniqueAsync("کد حسابی با این عنوان در سیستم ثبت شده است")
                .CustomAsync(async t => {
                    if (!long.TryParse(t.Code, out _))
                        return "کد فقط باید عدد باشد.";
                    var length = await base.GetService<CodingLevelService>().GetAll().Where(u => u.CodingLevelType <= t.CodingLevelType).SumAsync(u => u.Length);
                    if (t.Code.Length != length)
                        return $"طول کد باید {length} باشد.";
                    if (t.ParentCodeId.HasValue)
                    {
                        var parent = await SingleAsync(t.ParentCodeId.Value);
                        if (!t.Code.StartsWith(parent.Code))
                            return $"کد باید با {parent.Code} شروع شود";
                    }
                    return null;
                });
        }
    }
}
