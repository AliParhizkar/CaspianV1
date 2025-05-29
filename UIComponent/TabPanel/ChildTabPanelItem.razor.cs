using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class ChildTabPanelItem<TEntity, TDetail> : ComponentBase where TEntity : class where TDetail : class
    {
        string title;
        bool disabled;

        bool CheckDetailIsNotEmpty()
        {
            var entity = TabPanel.Service.UpsertData;
            if (typeof(TEntity) == typeof(TDetail))
                return true;
            var detailInfo = typeof(TEntity).GetProperties().Single(t => t.PropertyType == typeof(TDetail)); 
            return detailInfo.GetValue(entity)  != null;
        }

        [Parameter]
        public int ColumnsCount { get; set; } = 1;

        protected override void OnInitialized()
        {
            TabPanel.GetTabIndex(typeof(TDetail));
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
            base.OnParametersSet();
        }
    }
}
