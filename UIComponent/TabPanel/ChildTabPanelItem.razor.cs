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

        TDetail GetDetail()
        {
            var entity = TabPanel.Service.UpsertData;
            if (typeof(TEntity) == typeof(TDetail))
                return entity as TDetail;
            var detailInfo = typeof(TEntity).GetProperties().Single(t => t.PropertyType == typeof(TDetail)); 
            var detail = detailInfo.GetValue(entity) as TDetail ?? Activator.CreateInstance<TDetail>();
            return detail;
        }

        Type GetServiceType()
        {
            using var service = Factory.CreateScope().ServiceProvider.GetService<IBaseService<TDetail>>();
            return service?.GetType();
        }

        [Parameter]
        public int ColumnsCount { get; set; }

        protected override void OnInitialized()
        {
            if (typeof(TEntity) != typeof(TDetail))
            {
                var info = (Child.Body as MemberExpression).Member as PropertyInfo;
                title = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
            }
            TabPanel.Service.TabPanelItemInitialize(typeof(TDetail));
            base.OnInitialized();
        }

        [CascadingParameter]
        public EntityTabPanel<TEntity> TabPanel { get; set; }

        [Parameter]
        public Expression<Func<TEntity, TDetail>> Child { get; set; }

        [Parameter]
        public bool? Disabled { get; set; }

        [Parameter]
        public RenderFragment<TDetail> Content { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }

        protected override void OnParametersSet()
        {
            if (Title != null)
                title = Title;
            if (typeof(TEntity) == typeof(TDetail))
            {
                var id = typeof(TEntity).GetPrimaryKey().GetValue(TabPanel.Service.UpsertData);
                TabPanel.Service.MasterId = Convert.ToInt32(id);
                if (TabPanel.Service.MasterId > 0)
                {
                    var pKey = typeof(TDetail).GetPrimaryKey();
                    pKey.SetValue(TabPanel.Service.UpsertData, Convert.ChangeType(TabPanel.Service.MasterId, pKey.PropertyType));
                }
            }
            disabled = TabPanel.Service.MasterId == 0 && typeof(TEntity) != typeof(TDetail);
            if (Disabled.HasValue)
                disabled = Disabled.Value;
            base.OnParametersSet();
        }
    }
}
