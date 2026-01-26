using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    public class LocationService: BaseService<Location>
    {
        public LocationService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Code).UniqueAsync("محل جغرافیایی با این کد درسیستم تعریف شده است");
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.ParentId, "محل جغرافیایی با این نام درسیستم تعریف شده است");
            RuleFor(t => t.LocationType).CustomAsync(async t =>
            {
                if (t.LocationType == LocationType.Country && t.ParentId != null)
                    return "محل جغرافیایی از نوع کشور نمی تواند عضو هیچ محل جغرافیایی باشد";
                if (t.LocationType != LocationType.Country && t.ParentId == null)
                    return "محل جغرافیایی از نوع شهر یا استان باید عضو یک محل جغرافیایی باشند";
                if (t.ParentId.HasValue)
                {
                    var parent = await SingleAsync(t.ParentId.Value);
                    switch(parent.LocationType)
                    {
                        case LocationType.City:
                            return "برای محل جغرافیایی شهر هیچ زیرمجموعه ای تعریف نشده است";
                        case LocationType.Province:
                            if (t.LocationType != LocationType.City)
                                return "فقط محل جغرافیایی از نوع شهر می تواند زیرمجموعه استان باشد";
                            break;
                    }
                }
                return null;
            });
        }
    }
}
