
using Caspian.Engine.Model;

namespace Caspian.Engine.Service
{
    /// <summary>
    /// سرویس کد و آدرس منوها که بصورت سینگلتون در سیستم نگهداری می شود
    /// </summary>
    public class SingletonMenuService
    {
        public IList<MenuCategory> Categories { get; set; }

        public IList<Menu> Menus { get; set; }

        public Menu GetMenu(string path)
        {
            return Menus.SingleOrDefault(t => t.Source == path);
        }
    }
}
