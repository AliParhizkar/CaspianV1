using Caspian.Common;
using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class ProcurementItemGroupService: BaseService<ProcurementItemGroup>
    {
        public static int[] CodingLevelsLength = { 1, 2, 2, 3 };
        public ProcurementItemGroupService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).Required().UniqueAsync("گروه اقلام خریدنی با این کد در سیستم تعریف شده است")
                .CustomAsync(async t =>
                {
                    var length = CodingLevelsLength.TakeWhile((value, index) => index <= t.CodingLevels.ConvertToInt().Value).Sum();
                    if (t.Code.Length != length)
                        return $"طول کد باید {length} باشد";
                    if (t.ParentGroupId.HasValue)
                    {
                        var parent = await SingleAsync(t.ParentGroupId.Value);
                        if (!t.Code.StartsWith(parent.Code))
                            return $"کد باید با {parent.Code} شروع شود";
                    }
                    return null;
                });
            RuleFor(t => t.Name).Required().UniqueAsync("اقلام خریدنی با این نام در سیستم وجود دارد");
        }

        public override async Task<ProcurementItemGroup> AddAsync(ProcurementItemGroup group)
        {
            if (group.ParentGroupId.HasValue)
            {
                var parent = await SingleAsync(group.ParentGroupId.Value);
                group.CodingLevels = parent.CodingLevels + 1;
            }
            else
                group.CodingLevels = CodingLevels.Level1;
            return await base.AddAsync(group);
        }

        public override async Task UpdateAsync(ProcurementItemGroup group)
        {
            var old = await SingleAsync(group.Id);
            group.CodingLevels = old.CodingLevels;
            group.ParentGroupId = old.ParentGroupId;
            await base.UpdateAsync(group);
        }
    }
}
