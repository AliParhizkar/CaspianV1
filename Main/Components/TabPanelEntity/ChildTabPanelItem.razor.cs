using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Test
{
    public partial class ChildTabPanelItem<TEntity, TDetail> : ComponentBase where TEntity : class where TDetail : class
    {
        Type GetServiceType()
        {
            using var service = Factory.CreateScope().ServiceProvider.GetService<IBaseService<TDetail>>();
            return service?.GetType();
        }

        TDetail GetDetail()
        {
            if (Child.Body.NodeType == ExpressionType.MemberAccess)
            {
                var info = (Child.Body as MemberExpression).Member as PropertyInfo;
                var detail = info.GetValue(Entity) as TDetail;
                if (detail == null)
                    detail = Activator.CreateInstance<TDetail>();
                return detail;
            }
            if (Child.Body.NodeType == ExpressionType.Parameter)
                return Entity as TDetail;
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }
        void OnValidSubmit()
        {
        }

        [CascadingParameter]
        public TEntity Entity { get; set; }

        [Parameter]
        public Expression<Func<TEntity, TDetail>> Child { get; set; }

        [Parameter]
        public RenderFragment<TDetail> Content { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }
    }
}
