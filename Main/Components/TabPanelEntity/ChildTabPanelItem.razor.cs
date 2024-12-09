using Caspian.Common;
using System.Reflection;
using System.ComponentModel;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Test
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

        TDetail GetDetail()
        {
            var entity = TabPanel.Service.UpsertData;
            if (Child.Body.NodeType == ExpressionType.MemberAccess)
            {
                var info = (Child.Body as MemberExpression).Member as PropertyInfo;
                title = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                var detail = info.GetValue(entity) as TDetail;
                if (detail == null)
                    detail = Activator.CreateInstance<TDetail>();
                return detail;
            }
            if (Child.Body.NodeType == ExpressionType.Parameter)
                return entity as TDetail;
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }

        async void OnValidSubmit()
        {
            if (typeof(TEntity) == typeof(TDetail))
            {
                var entity = TabPanel.Service.UpsertData;
                using var scope = Factory.CreateScope();
                var service = scope.GetService<IBaseService<TEntity>>();
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
        public TabPaneEntity<TEntity> TabPanel { get; set; }

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
            if (Child.Body.NodeType != ExpressionType.Parameter)
            {
                var id = typeof(TEntity).GetPrimaryKey().GetValue(TabPanel.Service.UpsertData);
                disabled = id.Equals(0);
            }
            base.OnParametersSet();
        }
    }
}
