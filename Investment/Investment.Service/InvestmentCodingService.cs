using Investment.Model;
using Caspian.Common.Service;
using Caspian.Common;

namespace Investment.Service
{
    public class InvestmentCodingService:BaseService<InvestmentCoding>
    {
        public static readonly int[] LevelsLength = { 1, 2, 3, 3, 3 };
        public InvestmentCodingService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("اموالی با این نام در سیستم ثبت شده است");
            RuleFor(t => t.Code).Required().UniqueAsync("آموالی با این کد در سیستم ثبت شده است")
                .CustomAsync(async t =>
                {
                    if (t.ParentId == null)
                    {
                        if (t.Code.Length != LevelsLength[0])
                            return $"طول کد باید {LevelsLength[0]} باشد";
                    }
                    else
                    {
                        var parent = await SingleAsync(t.ParentId.Value);
                        if (parent.Level == LevelsLength.Length)
                            return $"امکان تعریف بیش از{LevelsLength.Length} سطح کد وجود ندارد";
                        var sum = LevelsLength.Where((t, index) => index <= parent.Level).Sum(t => t);
                        if (t.Code.Length != sum)
                            return $"طول کد باید {sum} رقم باشد";
                        if (!t.Code.StartsWith(parent.Code))
                            return $"کد باید با {parent.Code} شروع شود";
                    }
                    return null;
                });
        }

        public override async Task<InvestmentCoding> AddAsync(InvestmentCoding coding)
        {
            if (coding.ParentId == null)
                coding.Level = 1;
            else
            {
                var old = await SingleAsync(coding.ParentId.Value);
                coding.Level = old.Level + 1;
            }
            return await base.AddAsync(coding);
        }

        public override async Task UpdateAsync(InvestmentCoding coding)
        {
            var old = await SingleAsync(coding.ParentId.Value);
            coding.Level = old.Level;
            await base.UpdateAsync(coding);
        }
    }
}
