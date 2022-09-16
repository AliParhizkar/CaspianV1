using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Engine.Model;
using Microsoft.AspNetCore.Components;

namespace Caspian.Engine.Navigation
{
    public partial class DataMenu: ComponentBase
    {
        IList<MenuCategory> Categories;
        IList<Menu> Menus;
        SubSystemKind? OldSubSystem;

        protected override void OnInitialized()
        {
            var url = navigationManager.Uri.Substring(navigationManager.BaseUri.Length);
            var segments = url.Split('/');
            if (segments.Length > 0)
            {
                var subSystemKind = (SubSystemKind)typeof(SubSystemKind).GetField(segments[0]).GetValue(null);
                if (OldSubSystem != subSystemKind)
                {
                    Menus = SingletonMenuService.Menus.Where(t => t.ShowonMenu && t.MenuCategory.SubSystemKind == subSystemKind && MenusId.Contains(t.Id)).OrderBy(t => t.Ordering).ToList();
                    Categories = SingletonMenuService.Categories.Where(t => t.SubSystemKind == subSystemKind)
                        .OrderBy(t => t.Ordering).ToList();
                    OldSubSystem = subSystemKind;
                }
            }
            base.OnInitialized();
        }

        [Parameter]
        public IList<int> MenusId { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await jSRuntime.InvokeVoidAsync("$.telerik.bindMenu");
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
