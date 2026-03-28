
using Caspian.Common;
using Caspian.Engine.Model;
using Elfie.Serialization;

namespace Caspian.Engine.Service
{
    /// <summary>
    /// سرویس کد و آدرس منوها که بصورت سینگلتون در سیستم نگهداری می شود
    /// </summary>
    public class SingletonMenuService
    {
        public IList<MenuCategory> Categories { get; set; }

        public IList<Menu> Menus { get; set; }

        public Menu GetMenu(int sourceId, SubsystemKind systemKind)
        {
            return Menus.SingleOrDefault(t => t.SourceId == sourceId && t.SubsystemKind == systemKind);
        }

        public Menu GetMenu(string url)
        {
            return Menus.SingleOrDefault(t => t.URL == url);
        }
    }
}
