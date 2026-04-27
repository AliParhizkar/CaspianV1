using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplyingUnitService: BaseService<SupplyingUnit>
    {
        public static int[] CodingLevelsLength = { 1, 2, 2, 3 };

        public SupplyingUnitService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("واحد تامینی با این کد در سیستم تعریف شده است")
                .CustomAsync(async t =>
                {
                    var length = CodingLevelsLength.TakeWhile((value, index) => index <= t.CodingLevels.ConvertToInt().Value).Sum();
                    if (t.Code.Length != length)
                        return $"طول کد باید {length} باشد";
                    if (t.ParentUnitId.HasValue)
                    {
                        var parent = await SingleAsync(t.ParentUnitId.Value);
                        if (!t.Code.StartsWith(parent.Code))
                            return $"کد باید با {parent.Code} شروع شود";
                    }
                    return null;
                });
            RuleFor(t => t.Title).Required().UniqueAsync("واحد تامینی با این عنوان در سیستم تعریف شده است");
        }
    }
}
