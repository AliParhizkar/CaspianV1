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
        Type GetServiceType()
        {
            using var service = Factory.CreateScope().ServiceProvider.GetService<IBaseService<TDetail>>();
            return service?.GetType();
        }

        [Parameter]
        public int ColumnsCount { get; set; }

        [Parameter]
        public IUIService<TDetail> Service { get; set; }

        protected override void OnInitialized()
        {
            if (Child.Body.NodeType == ExpressionType.MemberAccess)
            {
                var info = (Child.Body as MemberExpression).Member as PropertyInfo;
                title = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
            }
            var entity = TabPanel.Service.UpsertData;
            Service.TabPanelInitialize();
            base.OnInitialized();
        }

        async void OnValidSubmit()
        {
            if (typeof(TEntity) == typeof(TDetail))
            {
                var entity = TabPanel.Service.UpsertData;
                using var service = Factory.CreateScope().GetService<IBaseService<TEntity>>();
                var id = typeof(TEntity).GetPrimaryKey().GetValue(entity);
                if (id.Equals(0))
                    await service.AddAsync(entity);
                else
                    await service.UpdateAsync(entity);
                await service.SaveChangesAsync();
                
                TabPanel.ChangeState();
            }
            
        }

        [CascadingParameter]
        public TabPanelEntity<TEntity> TabPanel { get; set; }

        [Parameter]
        public Expression<Func<TEntity, TDetail>> Child { get; set; }

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
            if (TabPanel.Service.MasterId > 0)
            {
                Service.MasterId = TabPanel.Service.MasterId;
                var info = typeof(TDetail).GetPrimaryKey();
                info.SetValue(Service.UpsertData, Convert.ChangeType(TabPanel.Service.MasterId, info.PropertyType));
            }
            if (typeof(TEntity) == typeof(TDetail))
            {
                var id = typeof(TEntity).GetPrimaryKey().GetValue(TabPanel.Service.UpsertData);
                Service.MasterId = Convert.ToInt32(id);
                if (Service.MasterId > 0)
                {
                    var pKey = typeof(TDetail).GetPrimaryKey();
                    pKey.SetValue(Service.UpsertData, Convert.ChangeType(Service.MasterId, pKey.PropertyType));
                }
                //disabled = id.Equals(0);
            }
            base.OnParametersSet();
        }
    }
}
