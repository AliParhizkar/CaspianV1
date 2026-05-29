using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;
using System.Threading.Tasks;

namespace Warehouse.Service
{
    public class PropertyOfGoodsService: BaseService<PropertyOfGoods>
    {
        GoodsProperties goodsProperties;

        async Task<GoodsProperties> GetGoodsProperties(int id)
        {
            if (goodsProperties?.Id != id)
                goodsProperties = await ServiceProvider.GetCaspianService<GoodsPropertiesService>().SingleAsync(id);
            return goodsProperties;
        }

        public PropertyOfGoodsService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.GoodsPropertiesId).UniqueAsync(t => t.GoodsId, "این ویژگی قبلا به کالا اضافه شده است");
            RuleFor(t => t.BooleanField).CustomAsync(async t =>
            {
                var old = await GetGoodsProperties(t.GoodsPropertiesId);
                if (old.PropertyType == PropertyType.Boolean)
                {
                    if (t.BooleanField == null)
                        return "مقدار این فیلد باید مشخص باشد";
                }
                else if (t.BooleanField.HasValue)
                    return "مقدار این فیلد باید خالی باشد";
                return null;
            });
            RuleFor(t => t.StringField).CustomAsync(async t =>
            {
                var old = await GetGoodsProperties(t.GoodsPropertiesId);
                if (old.PropertyType == PropertyType.String)
                {
                    if (!t.StringField.HasValue())
                        return "مقدار این فیلد باید مشخص باشد";
                    if (old.MaxLength.HasValue && t.StringField.Length > old.MaxLength)
                        return $"ماکزیمم طول باید {old.MaxLength} باشد";
                    if (old.FixLength.HasValue && t.StringField.Length != old.Length)
                        return $"طول رشته باید ثابت و برابر {old.Length} باشد";
                }
                else if (t.StringField != null)
                    return "مقدار این فیلد باید خالی باشد";
                return null;
            });
            RuleFor(t => t.NumericField).CustomAsync(async t =>
            {
                var old = await GetGoodsProperties(t.GoodsPropertiesId);
                if (old.PropertyType == PropertyType.Number)
                {
                    if (t.NumericField == null)
                        return "مقدار این فیلد باید مشخص باشد";
                    if (old.MaximumValue.HasValue && t.NumericField > old.MaximumValue)
                        return $"مقدار این فیلد نمی تواند بیشتر از {old.MaximumValue} باشد";
                    if (old.MinimumValue.HasValue && t.NumericField < old.MinimumValue)
                        return $" مقدار این فیلد نمی تواند کمتر از {old.MinimumValue} باشد";
                }
                else if (t.NumericField.HasValue)
                    return "مقدار این فیلد باید خالی باشد";
                return null;
            });
            RuleFor(t => t.DateField).CustomAsync(async t =>
            {
                var old = await GetGoodsProperties(t.GoodsPropertiesId);
                if (old.PropertyType == PropertyType.Date)
                {
                    if (t.DateField == null)
                        return "مقدار این فیلد باید مشخص باشد";
                }
                else if (t.DateField.HasValue)
                    return "مقدار این فیلد باید خالی باشد";
                return null;
            });
        }
    }
}
