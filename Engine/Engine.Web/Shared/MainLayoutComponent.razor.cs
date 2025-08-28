using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;

namespace Caspian.Engine.Shared
{
    public partial class MainLayoutComponent
    {
        CaspianExceptionComponent exceptionComponent;
        bool hideMenu;
        string date;
        string userName;
        IList<int> menusId;
        int? userId;
        string time;
        string title;
        int menuId;
        bool userHasAccess;
        bool userIsAuthenticated;
        SubSystemKind systemKind;
        string currentUrl;
        PageData pageData;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool RightToLeft { get; set; }

        [Parameter]
        public Language Language { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        async Task ToggleMenu()
        {
            hideMenu = !hideMenu;
            await storage.SetAsync("menu-status", hideMenu);
        }

        protected override void OnInitialized()
        {
            currentUrl = navigationManager.ToBaseRelativePath(navigationManager.Uri);
            navigationManager.LocationChanged += OnLocationChanged;
            pageData = new PageData();
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            currentUrl = navigationManager.ToBaseRelativePath(e.Location);
            base.InvokeAsync(async () =>
            {
                if (userId.HasValue)
                {
                    using var scope = ServiceScopeFactory.CreateScope();
                    var service = scope.GetService<UserLoginService>();
                    service.CheckValidation = false;
                    await service.AddAsync(new UserLogin()
                    {
                        LoginDate = DateTime.Now,
                        UserId = userId.Value,
                        PageUrl = currentUrl
                    });

                    await service.SaveChangesAsync();
                }
            });
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            date = DateTime.Now.Date.ToShortDateString();
            time = DateTime.Now.TimeOfDay.ShortString();
            pageData.RightToLeft = RightToLeft;
            pageData.Language = RightToLeft ? Language.Fa : Language.En;

            var uri = new Uri(navigationManager.Uri);
            var url = uri.AbsolutePath;
            if (url.StartsWith("/"))
                url = url.Substring(1);
            var strSubsystem = url.Split('/')[0];
            var field = typeof(SubSystemKind).GetFields().Single(t => t.Name.Equals(strSubsystem,
                StringComparison.OrdinalIgnoreCase));
            systemKind = (SubSystemKind)field.GetValue(null);
            if (PageId.HasValue)
            {
                var menu = MenuService.GetMenu(PageId.Value, systemKind);
                title = "";
                if (menu != null)
                {
                    menuId = menu.Id;
                    title = menu.Title;
                }
            }
            exceptionComponent?.Recover();
            base.OnParametersSet();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (authenticationStateTask != null)
            {
                var result = await authenticationStateTask;
                userIsAuthenticated = result.User.Identity?.IsAuthenticated == true;
                if (userIsAuthenticated)
                {
                    var claim = result.User.Claims.FirstOrDefault()?.Value;
                    if (claim.HasValue())
                    {
                        userId = Convert.ToInt32(claim);
                        pageData.UserId = userId.Value;
                        if (!userName.HasValue())
                        {
                            using var scope = ServiceScopeFactory.CreateScope();
                            dataService.UserId = userId.Value;
                            dataService.Language = RightToLeft ? Language.Fa : Language.En;
                            var user = await scope.GetService<UserService>().SingleAsync(userId.Value);
                            userName = user.FName + " " + user.LName;
                            menusId = await scope.GetService<MenuAccessibilityService>().GetUserMenus(userId.Value);
                        }
                    }
                }
            }
            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var menuStatus = await storage.GetAsync<bool>("menu-status");
            var status = menuStatus.Success && menuStatus.Value;
            if (status && !hideMenu)
            {
                hideMenu = true;
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        void MainMenu()
        {

        }

        [Parameter]
        public int? PageId { get; set; }

        public void Dispose()
        {
            navigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
