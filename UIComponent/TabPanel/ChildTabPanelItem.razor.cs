using Caspian.Common;
using System.Reflection;
using System.ComponentModel;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class ChildTabPanelItem<TEntity, TDetail> : ComponentBase where TEntity : class where TDetail : class
    {
        string title;
        bool disabled;
        int tabIndex;

        bool CheckDetailIsNotEmpty()
        {
            var entity = TabPanel.Service.UpsertData;
            if (typeof(TEntity) == typeof(TDetail))
                return true;
            var detailInfo = typeof(TEntity).GetProperties().Single(t => t.PropertyType == typeof(TDetail)); 
            return detailInfo.GetValue(entity)  != null;
        }

        Type GetServiceType()
        {
            using var service = Factory.CreateScope().ServiceProvider.GetService<IBaseService<TDetail>>();
            return service?.GetType();
        }

        [Parameter]
        public int ColumnsCount { get; set; } = 1;

        protected override void OnInitialized()
        {
            tabIndex = TabPanel.GetTabIndex();
            if (typeof(TEntity) != typeof(TDetail))
            {
                var info = (Child.Body as MemberExpression).Member as PropertyInfo;
                title = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
            }
            else
                title = "مشخصات اصلی";
            base.OnInitialized();
        }


        [CascadingParameter]
        public EntityTabPanel<TEntity> TabPanel { get; set; }

        [Parameter]
        public Expression<Func<TEntity, TDetail>> Child { get; set; }

        [Parameter]
        public bool? Disabled { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; }

        protected override void OnParametersSet()
        {
            if (Title != null)
                title = Title;
            var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(TabPanel.Service.UpsertData));
            if (typeof(TEntity) == typeof(TDetail))
            {
                TabPanel.Service.MasterId = Convert.ToInt32(id);
                if (TabPanel.Service.MasterId > 0)
                {
                    var pKey = typeof(TDetail).GetPrimaryKey();
                    pKey.SetValue(TabPanel.Service.UpsertData, Convert.ChangeType(TabPanel.Service.MasterId, pKey.PropertyType));
                }
            }
            //disabled = TabPanel.Service.MasterId == 0 && typeof(TEntity) != typeof(TDetail);
            //if (Disabled.HasValue)
            //    disabled = Disabled.Value;
            base.OnParametersSet();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (tabIndex == TabPanel.GetSelectedTabPanelIndex() && typeof(TEntity) != typeof(TDetail))
                await TabPanel.Service.UpdateChildOfModelAsync(typeof(TDetail));
            await base.OnParametersSetAsync();
        }
    }
}
