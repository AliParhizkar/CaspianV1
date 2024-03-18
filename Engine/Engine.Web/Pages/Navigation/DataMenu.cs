using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Engine.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Caspian.Engine.Service;

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
            navigationManager.LocationChanged += NavigationManager_LocationChanged;
            curentUrl = '/' + url;
            var segments = url.Split('/');
            if (url.HasValue() && segments.Length > 0 && !segments[0].Equals("login", StringComparison.OrdinalIgnoreCase))
            {
                var str = segments[0].Split("?")[0];
                var field = typeof(SubSystemKind).GetFields().Single(t => t.Name.Equals(str, StringComparison.OrdinalIgnoreCase));
                var subSystemKind = (SubSystemKind)field.GetValue(null);
                if (OldSubSystem != subSystemKind)
                {
                    Menus = SingletonMenuService.Menus.Where(t => t.ShowonMenu && t.SubSystemKind == subSystemKind && MenusId.Contains(t.Id)).OrderBy(t => t.Ordering).ToList();
                    Categories = SingletonMenuService.Categories.Where(t => t.SubSystemKind == subSystemKind)
                        .OrderBy(t => t.Ordering).ToList();
                    OldSubSystem = subSystemKind;
                }
            }
            base.OnInitialized();
        }
        protected override async Task OnInitializedAsync()
        {
            var item = await Storege.GetAsync<string>("CurentShowUrl");
            if (item.Success)
            {
                sholdRender = true;
                curentUrl = item.Value;
            }
            await base.OnInitializedAsync();
        }

        private void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            var uri = new Uri(navigationManager.Uri);
            var url = "";
            foreach (var segment in uri.Segments)
            {
                if (!Int64.TryParse(segment, out _))
                    url += segment;
            }
            if (url.EndsWith('/'))
                url = url.Substring(0, url.Length - 1);
            var menu = SingletonMenuService.GetMenu(url);
            if (menu?.ShowonMenu == true)
                Storege.SetAsync("CurentShowUrl", url);
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
