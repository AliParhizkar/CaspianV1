using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Caspian.Engine.Navigation
{
    public partial class DataMenu: ComponentBase
    {
        string currentUrl;
        IList<Menu> Menus;
        ElementReference elm;
        bool shouldRender = true;
        SubsystemKind? OldSubSystem;
        IList<MenuCategory> Categories;

        protected override bool ShouldRender()
        {
            return shouldRender;
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
            currentUrl = '/' + url; 
            var segments = url.Split('/');
            if (url.HasValue() && segments.Length > 0 && !segments[0].Equals("login", StringComparison.OrdinalIgnoreCase))
            {
                var str = segments[0].Split("?")[0];
                var field = typeof(SubsystemKind).GetFields().Single(t => t.Name.Equals(str, StringComparison.OrdinalIgnoreCase));
                var subSystemKind = (SubsystemKind)field.GetValue(null);
                if (OldSubSystem != subSystemKind)
                {
                    Menus = SingletonMenuService.Menus.Where(t => t.ShowOnMenu && t.SubsystemKind == subSystemKind && MenusId.Contains(t.Id)).OrderBy(t => t.Ordering).ToList();
                    Categories = SingletonMenuService.Categories.Where(t => t.SubsystemKind == subSystemKind)
                        .OrderBy(t => t.Ordering).ToList();
                    OldSubSystem = subSystemKind;
                }
            }
            base.OnInitialized();
        }
        protected override async Task OnInitializedAsync()
        {
            

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
            if (menu?.ShowOnMenu == true)
                Storege.SetAsync("CurrentShowUrl", url);
        }

        [Parameter]
        public IList<int> MenusId { get; set; }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var item = await Storege.GetAsync<string>("CurrentShowUrl");
                if (item.Success)
                {
                    shouldRender = true;
                    if (currentUrl != item.Value)
                    {
                        currentUrl = item.Value;
                        StateHasChanged();
                    }
                }
            }
            await jSRuntime.InvokeVoidAsync("caspian.common.bindMenu", elm);

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
