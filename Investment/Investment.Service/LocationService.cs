using Caspian.Common;
using Investment.Model;
using Caspian.Common.Service;

namespace Investment.Service
{
    public class LocationService : BaseService<Location>
    {
        public LocationService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("موقعیت جغرافیایی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Code).UniqueAsync("موقعیت جغرافیایی با این کد در سیستم ثبت شده است").CustomAsync(async t => 
                {
                    if (t.Code.HasValue() && t.ParentId.HasValue)
                    {
                        var parent = await SingleAsync(t.ParentId.Value);
                        if (!t.Code.StartsWith(parent.Code))
                            return $"کد {t.LocationType.EnumText()} باید با {parent.Code} شروع شود";
                    }
                    return null;
                });
            RuleFor(t => t.ParentId).Custom(t => 
            {
                if (t.ParentId == null && t.LocationType != LocationType.Continent)
                    return $"لطفا {(t.LocationType - 1).EnumText()} این موقعیت جفرافیایی را مشخص کنید" ;
                if (t.ParentId != null && t.LocationType == LocationType.Continent)
                    return "برای موقعیت جغرافیایی قاره این موقعیت باید خالی باشد";
                return null;
            });
            RuleFor(t => t.LocationType).CustomAsync(async t =>
            {
                if (t.ParentId == null)
                    return null;
                var parent = await SingleAsync(t.ParentId.Value);
                var message = "نوع موقعیت جغرفیایی نامعتبر است";
                if (parent.LocationType == LocationType.Continent && t.LocationType != LocationType.Country)
                    return message;
                if (parent.LocationType == LocationType.Country && t.LocationType != LocationType.Province && t.LocationType != LocationType.City)
                    return message;
                if (parent.LocationType == LocationType.Province && t.LocationType != LocationType.City)
                    return message;
                if (parent.LocationType == LocationType.City && t.LocationType != LocationType.Region)
                    return message;
                return null;
            });
        }
    }
}
