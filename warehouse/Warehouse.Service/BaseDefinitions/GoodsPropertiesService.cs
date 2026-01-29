using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class GoodsPropertiesService: MasterDetailsService<GoodsProperties, PropertyList>, IBaseService<GoodsProperties>
    {
        public GoodsPropertiesService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("ویژگی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.MinimumValue).Custom(t => t.PropertyType != PropertyType.Number && t.MinimumValue.HasValue, 
                "حداقل مقدار فقط برای نوع عددی می تواند پر شود");
            RuleFor(t => t.MaximumValue).Custom(t => t.PropertyType != PropertyType.Number && t.MaximumValue.HasValue,
                "حداکثر مقدار فقط برای نوع عددی می تواند پر شود");
            RuleFor(t => t.NumberDigit).Custom(t => {
                if (t.PropertyType != PropertyType.Number && t.NumberDigit.HasValue)
                    return "تعداد ارقام اعشار فقط در حالت نوع داده عددی می تواند پر باشد";
                if (t.NumberDigit < 1 || t.NumberDigit > 4)
                    return "تعداد ارقام اعشار باید بین یک تا پنج باشد";
                return null;
            });
            RuleFor(t => t.MaxLength).Custom(t =>
            {
                if (t.MaxLength.HasValue)
                {
                    if (t.PropertyType != PropertyType.String)
                        return "جداکثر تعداد حروف فقط برای نوع متنی می تواند پر شود.";
                    if (t.FixLength == true)
                        return "در حالت طول ثابت، جداکثر تعداد حروف نمی تواند پر باشد";
                }
                return null;
            });
            RuleFor(t => t.Length).Custom(t =>
            {
                if (t.Length.HasValue)
                {
                    if (t.PropertyType != PropertyType.String)
                        return "طول فقط برای نوع متنی می تواند پر باشد";
                    if (t.FixLength != true)
                        return "طول فقط در حالتی می تواند پر باشد که طول ثابت باشد";
                }
                return null;
            });
            RuleFor(t => t.FixLength).Custom(t => t.FixLength == true && t.PropertyType != PropertyType.String, 
                "طول ثابت فقط باید در حالت متنی پر باشد");
            RuleFor(t => t.Properties).Custom(t => t.PropertyType == PropertyType.List && (t.Properties == null || t.Properties.Count < 2),
                "لیست حداقل باید دو آیتم داشته باشد");
            RuleForEach(t => t.Properties).SetValidator(t => new PropertyListService(provider));
        }
    }
}
