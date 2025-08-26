using Caspian.Common;
using System.Reflection;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Caspian.UI;

namespace Caspian.Engine.SystemManagement
{
    public partial class MenuManagementComponent: BasePage
    {
        bool? showInMenu;
        bool? isDropped;
        bool onlyUntitledMenus;
        IList<SelectListItem> source = new List<SelectListItem>();

        [Parameter]
        public SubSystemKind Subsystem { get; set; }

        void FillUrls()
        {
            if (Service.UpsertData.ShowonMenu)
            {
                var page = new AssemblyInfo().GetWebTypes(Subsystem).SingleOrDefault(t => t.FullName == Service.UpsertData.Source);
                if (page != null)
                {
                    var urls = page.GetCustomAttributes<RouteAttribute>().Select(t => t.Template).Where(t => !t.Contains("{"));
                    source = urls.Select(t => new SelectListItem(t, t)).ToList();
                    if (urls.Count() == 1)
                        Service.UpsertData.URL = urls.Single();
                }
            }
        }

        async Task LoadMenus()
        {
            using var scope = CreateScope();
            service.Categories = await scope.GetService<MenuCategoryService>().GetAll().ToListAsync();
            service.Menus = await new MenuService(scope.ServiceProvider).GetAll().ToListAsync();
        }

        async Task UpdateMenus()
        {
            var components = new AssemblyInfo().GetWebTypes(Subsystem);
            ///Check All Urls that start with /SubsystemName
            var list = new List<string>();
            var pages = new Dictionary<int, Type>();
            foreach (var component in components)
            {
                var routes = component.GetCustomAttributes<RouteAttribute>();
                foreach (var url in routes)
                {
                    if (!url.Template.StartsWith("/" + Subsystem, StringComparison.OrdinalIgnoreCase))
                        throw new CaspianException($"On type {component.Name} path should start with /{Subsystem} ", null);
                }
                var pageAttr = component.GetCustomAttribute<PageAttribute>();
                if (routes.Any())
                {
                    if (pageAttr == null)
                        throw new CaspianException($"Type {component.Name} is page and should has PageAttribute");
                    list.Add(component.FullName);
                }
                if (pageAttr != null)
                {
                    if (pages.ContainsKey(pageAttr.Id))
                        throw new CaspianException($"Types {component.Name}, {pages.GetValueOrDefault(pageAttr.Id).Name} Has same page id");
                    pages.Add(pageAttr.Id, component);
                }
            }
            using var service = CreateScope().GetService<MenuService>();
            var menus = await service.GetAll().Where(t => Subsystem == Subsystem).ToListAsync();
            foreach (var menu in menus)
            {

                if (!components.Any(t => t.FullName == menu.Source))
                {
                    var old = await service.SingleAsync(menu.Id);
                    old.IsDropped = true;
                    await service.SaveChangesAsync();
                }
            }
            foreach (var item in list)
            {
                var menu = menus.SingleOrDefault(t => t.Source.Equals(item, StringComparison.OrdinalIgnoreCase));

                if (menu == null)
                {
                    ///Insert menu
                    await service.AddAsync(new Menu()
                    {
                        Source = item,
                        SubSystemKind = Subsystem
                    });
                    await service.SaveChangesAsync();
                }
                else if (menu.Source != item)
                {
                    ///Update source
                    var old = await service.SingleAsync(menu.Id);
                    if (old.Title.HasValue())
                    {
                        old.Source = item;
                        old.SubSystemKind = Subsystem;
                        await service.UpdateAsync(old);
                        await service.SaveChangesAsync();
                    }
                }
            }
            await Service.DataView.ResetGrid();
        }
    }
}
