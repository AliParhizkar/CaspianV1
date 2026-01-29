using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class MaterialLocationService : BaseService<MaterialLocation>
    {
        public MaterialLocationService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.StockroomId, "آدرس کالایی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Code).Required().UniqueAsync(t => t.StockroomId, "آدرس کالایی با این کد درسیستم تعریف شده است")
                .CustomAsync(async t =>
                {
                    if (t.ParentLocationId.HasValue)
                    {
                        var parent = await SingleAsync(t.ParentLocationId.Value);
                        if (t.Code.Length <= parent.Code.Length || !t.Code.StartsWith(parent.Code))
                            return $"کد باید با {parent.Code} شروع شود";
                    }
                    return null;
                });
        }
    }
}
