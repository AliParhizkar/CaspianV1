using Caspian.UI;
using Caspian.Common;
using System.Reflection;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using System.Linq.Dynamic.Core;

namespace Caspian.Engine.SystemManagement
{
    public partial class MenuManagementComponent: BasePage
    {
        bool onlyUntitledMenus;
        IList<SelectListItem> source = new List<SelectListItem>();
        WindowStatus categoryWindowStatus;

        [Parameter]
        public SubsystemKind Subsystem { get; set; }

        void FillUrls()
        {
            if (Service.UpsertData.ShowOnMenu)
            {
                var page = new AssemblyInfo().GetWebTypes(Subsystem)
                    .SingleOrDefault(t => t.GetCustomAttribute<SourceAttribute>()?.Id == Service.UpsertData.SourceId);
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
            ///Check All Urls that start with /SubsystemName
            var components = new List<Menu>();
            var dic = new Dictionary<int, Type>();
            using var service = CreateScope().GetService<MenuService>();
            foreach (var component in new AssemblyInfo().GetWebTypes(Subsystem))
            {
                var menu = new Menu();
                menu.SubsystemKind = Subsystem;
                var routes = component.GetCustomAttributes<RouteAttribute>();
                foreach (var url in routes)
                {
                    if (!url.Template.StartsWith("/" + Subsystem, StringComparison.OrdinalIgnoreCase))
                        throw new CaspianException($"On type {component.Name} path should start with /{Subsystem} ", null);
                }
                var sourceAttr = component.GetCustomAttribute<SourceAttribute>();
                if (routes.Any())
                {
                    if (sourceAttr == null)
                        throw new CaspianException($"Type {component.Name} is page and should has SourceAttribute");
                }
                if (sourceAttr != null)
                {
                    menu.SourceId = sourceAttr.Id;
                    menu.Title = sourceAttr.Title;
                    var old = components.SingleOrDefault(t => t.SourceId == sourceAttr.Id);
                    if (old != null)
                    {
                        var errorMessage = $"Types {component.Name} & {dic[sourceAttr.Id].Name} Has same page id. ";
                        if (service.GetAll().Any(t => t.SourceId == old.Id && !t.Accessibilities.Any()))
                            errorMessage += $"You can change id of {component.Name}";
                        else if (service.GetAll().Any(t => t.SourceId == old.Id && !t.Accessibilities.Any()))
                            errorMessage += $"This code should be changed";
                        throw new CaspianException(errorMessage);
                    }
                    components.Add(menu);
                    dic.Add(sourceAttr.Id, component);
                }
            }
            var menus = await service.GetAll().Where(t => t.SubsystemKind == Subsystem).ToListAsync();
            foreach (var menu in menus)
            {
                var old = await service.SingleAsync(menu.Id);
                var component = components.SingleOrDefault(t => t.SourceId == old.SourceId);
                if (component == null)
                    old.IsDropped = true;
                else
                {
                    old.SourceId = component.SourceId;
                    old.Title = component.Title;
                    old.IsDropped = false;
                }
                await service.SaveChangesAsync();
            }
            foreach(var component in components.Where(t => !menus.Any(u => u.SourceId == t.SourceId)))
            {
                await service.AddAsync(component);
                await service.SaveChangesAsync();
            }
            await Service.DataView.ResetGrid();
        }
    }
}
