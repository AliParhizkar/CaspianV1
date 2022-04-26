using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Navigation;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DataMenu: ComponentBase
    {
        IList<MenuCategory> Categories;
        IList<Menu> Menus;
        SubSystemKind? OldSubSystem;

        protected async override Task OnInitializedAsync()
        {
            var url = navigationManager.Uri.Substring(navigationManager.BaseUri.Length);
            var segments = url.Split('/');
            if (segments.Length > 0)
            {
                var subSystemKind = (SubSystemKind)typeof(SubSystemKind).GetField(segments[0]).GetValue(null);
                if (OldSubSystem != subSystemKind)
                {
                    using var scope = ServiceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetService(typeof(Context)) as Context;

                    Categories = await context.MenuCategories.Where(t => t.SubSystemKind == subSystemKind)
                        .OrderBy(t => t.Ordering).ToListAsync();
                    Menus = await context.Menus.Where(t => t.MenuCategory.SubSystemKind == subSystemKind && t.ShowonMenu)
                        .OrderBy(t => t.Ordering).ToListAsync();
                    OldSubSystem = subSystemKind;
                }
            }
            await base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await jSRuntime.InvokeVoidAsync("$.telerik.bindMenu");
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
