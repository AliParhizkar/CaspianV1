using Caspian.Common;
using System.Collections;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class CheckboxList<TEntity, TDetails> : ComponentBase where TEntity: class where TDetails : class
    {
        bool LoadData;
        IList<SelectListItem> Items;
        IList<TDetails> details;
        IList<int> SelectedIds;
        string filterText;

        void UpdateSelectedIds(bool flag, string value)
        {
            var id = Convert.ToInt32(value);
            if (flag)
                SelectedIds.Add(id);
            else
                SelectedIds.Remove(id);
            UpdateChangedEntities();
        }

        void UpdateChangedEntities()
        {
            if (Service == null)
            {
                var type = typeof(TDetails).IsArray;
            }
            var otherInfo = typeof(TDetails).GetForeignKey(typeof(TEntity));
            var masterType = Service.GetType().GenericTypeArguments[0];
            var masterInfo = typeof(TDetails).GetForeignKey(masterType);
            var list = new List<ChangedEntity<TDetails>>();
            /// Added to list
            foreach (var id in SelectedIds)
            {
                if (!details.Any(t => Convert.ToInt32(otherInfo.GetValue(t)) == id))
                {
                    var entity = Activator.CreateInstance<TDetails>();
                    otherInfo.SetValue(entity, Convert.ChangeType(id, otherInfo.PropertyType));
                    masterInfo.SetValue(entity, Convert.ChangeType(Service.MasterId, masterInfo.PropertyType));
                    list.Add(new ChangedEntity<TDetails>() 
                    { 
                        Entity = entity, 
                        ChangeStatus = ChangeStatus.Added 
                    });
                }
            }
            /// Removed from list
            var pkeyInfo = typeof(TDetails).GetPrimaryKey();
            foreach (var detail in details)
            {
                var otherId = otherInfo.GetValue(detail);
                if (!SelectedIds.Contains(Convert.ToInt32(otherId)))
                {
                    var entity = Activator.CreateInstance<TDetails>();
                    otherInfo.SetValue(entity, otherId);
                    masterInfo.SetValue(entity, Convert.ChangeType(Service.MasterId, masterInfo.PropertyType));
                    pkeyInfo.SetValue(entity, pkeyInfo.GetValue(detail));
                    list.Add(new ChangedEntity<TDetails>()
                    {
                        Entity = entity,
                        ChangeStatus = ChangeStatus.Deleted
                    });
                }
            }
            Service.ChangedEntities = list;
        }

        async Task DataBinding()
        {
            if (LoadData)
            {
                var service = Provider.GetService(typeof(IBaseService<TEntity>)) as IBaseService<TEntity>;
                var query = service.GetAll(default(TEntity));
                if (ConditionExpression != null)
                    query = query.Where(ConditionExpression);
                if (OrderByExpression != null)
                    query = query.OrderBy(OrderByExpression);
                var list = new ExpressionSurvey().Survey(TextExpression);
                var type = typeof(TEntity);
                var parameter = Expression.Parameter(type, "t");
                list = list.Select(t => parameter.ReplaceParameter(t)).ToList();
                var pkey = type.GetPrimaryKey();
                var pKeyExpr = Expression.Property(parameter, pkey);
                var pkeyAdded = false;
                foreach (var expr1 in list)
                {
                    if (expr1.Member == pkey)
                        pkeyAdded = true;
                }
                if (!pkeyAdded)
                    list.Add(pKeyExpr);
                var dataList = await query.GetValuesAsync(list);
                var displayFunc = TextExpression.Compile();
                var info = typeof(TEntity).GetPrimaryKey();
                Items = new List<SelectListItem>();
                foreach (var item in dataList)
                {
                    var text = Convert.ToString(displayFunc.DynamicInvoke(item));
                    var value = Convert.ToString(info.GetValue(item));
                    Items.Add(new SelectListItem(value, text));
                }
            }
        }

        protected override void OnInitialized()
        {
            LoadData = true;
            SelectedIds = new List<int>();
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            if (Service?.MasterId > 0)
            {
                var masterType = Service.GetType().GetGenericArguments()[0];
                var masterIdInfo = typeof(TDetails).GetForeignKey(masterType);
                var param = Expression.Parameter(typeof(TDetails), "t");
                Expression expr = Expression.Property(param, masterIdInfo);
                expr = Expression.Equal(expr, Expression.Constant(Service.MasterId));
                var lambda = Expression.Lambda(expr, param);
                using var service = Provider.CreateScope().GetService<IBaseService<TDetails>>();
                var query = service.GetAll().Where(lambda);
                var detailIdInfo = typeof(TDetails).GetForeignKey(typeof(TEntity));
                var exprDetailId = Expression.Property(param, detailIdInfo);
                var exprPKey = Expression.Property(param, typeof(TDetails).GetPrimaryKey());
                details = await query.GetValuesAsync(exprDetailId, exprPKey);
                foreach ( var detail in details )
                {
                    var otherId = detailIdInfo.GetValue(detail);
                    var id = Convert.ToInt32(otherId);
                    SelectedIds.Add(id);
                }
            }
            else
                details = new List<TDetails>();
            await base.OnInitializedAsync();
        }

        protected async override Task OnParametersSetAsync()
        {
            await DataBinding();
            await base.OnParametersSetAsync();
        }

        [Parameter]
        public EventCallback<TDetails> AddToChanged { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public bool Filterable { get; set; } = true;

        [Parameter]
        public IDetailBatchService<TDetails> Service { get; set; }

        public TDetails Value { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> OrderByExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public string Style { get; set; }
    }
}
