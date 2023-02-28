using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Engine.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Caspian.Engine.Navigation
{
    public partial class DataMenu: ComponentBase
    {
        string curentUrl;
        IList<Menu> Menus;
        ElementReference elm;
        bool sholdRender = true;
        SubSystemKind? OldSubSystem;
        IList<MenuCategory> Categories;

        protected override bool ShouldRender()
        {
            return sholdRender;
        }

        async Task ChangeValueAsync()
        {
            await OnChange.InvokeAsync();
        }

        [Parameter]
        public EventCallback OnChange { get; set; }


        protected override void OnInitialized()
        {
            var url = navigationManager.Uri.Substring(navigationManager.BaseUri.Length);
            curentUrl = '/' + url;
            var segments = url.Split('/');
            if (url.HasValue() && segments.Length > 0 && !segments[0].Equals("login", StringComparison.OrdinalIgnoreCase))
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
            await jSRuntime.InvokeVoidAsync("$.caspian.bindMenu", elm);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
