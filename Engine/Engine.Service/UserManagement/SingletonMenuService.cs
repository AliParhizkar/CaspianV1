
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

        public int GetMenuId(string path)
        {
            return Menus.Single(t => t.Source == path).Id;
        }
    }
}
