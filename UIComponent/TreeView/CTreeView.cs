using System;
using System.Linq;
using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class CTreeView<TEntity>: ComponentBase where TEntity: class
    {
        ElementReference tree;
        IList<TreeViewItem> Items;

        [CascadingParameter(Name = "TreeViewCascadeData")]
        public TreeViewCascadeData CascadeData { get; set; }

        [Parameter]
        public RenderFragment<TreeViewItem> Template { get; set; }

        [Parameter]
        public RenderFragment<TreeViewItem> ChildContent { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attrs { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, object>> OrderByExpression { get; set; }

        [Parameter]
        public bool Selectable { get; set; }

        [Parameter]
        public IEnumerable<TreeViewItem> Source { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnExpanded { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnCollapsed { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnSelected { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnChange { get; set; }

        [Parameter]
        public bool SingleSelectOnTree { get; set; }

        void UnselectTree(IEnumerable<TreeViewItem> nodes)
        {
            foreach (var node in nodes)
            {
                node.Selected = false;
                if (node.Items != null)
                    UnselectTree(node.Items);
            }
        }

        async Task DataBinding()
        {
            if (Source == null && typeof(TEntity) == typeof(TreeViewItem))
                Source = new List<TreeViewItem>();
            if (Source == null)
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var service = new SimpleService<TEntity>(scope);
                var contextType = new AssemblyInfo().GetDbContextType(typeof(TEntity));
                var query = service.GetAll(default(TEntity));
                Expression lambdExpr = null;
                if (CascadeData != null)
                {
                    foreach (var info in typeof(TEntity).GetProperties())
                    {
                        if (info.PropertyType == CascadeData.Type)
                        {
                            var idName = info.GetCustomAttribute<ForeignKeyAttribute>().Name;
                            ParameterExpression param = null;
                            if (ConditionExpression == null)
                                param = Expression.Parameter(typeof(TEntity), "t");
                            else
                                param = ConditionExpression.Parameters[0];
                            Expression expr = Expression.Property(param, idName);
                            if (expr.Type.IsNullableType())
                                expr = Expression.Property(expr, "Value");
                            var value = Convert.ChangeType(CascadeData.Value, expr.Type);
                            expr = Expression.Equal(expr, Expression.Constant(value));

                            if (ConditionExpression != null)
                                expr = Expression.And(expr, ConditionExpression.Body);
                            lambdExpr = Expression.Lambda(expr, param);
                            break;
                        }
                    }
                }
                if (lambdExpr != null)
                    query = query.Where(lambdExpr).OfType<TEntity>();
                if (OrderByExpression != null)
                    query = query.OrderBy(OrderByExpression);
                var list = new ExpressionSurvey().Survey(TextExpression);
                var type = typeof(TEntity);
                var parameter = Expression.Parameter(type, "t");
                list = list.Select(t => parameter.ReplaceParameter(t)).ToList();
                var pkey = type.GetPrimaryKey();
                var pKeyExpr = Expression.Property(parameter, pkey);
                var pkeyAdded = false;
                foreach (var expr in list)
                {
                    if (expr.Member == pkey)
                        pkeyAdded = true;
                }
                if (!pkeyAdded)
                    list.Add(pKeyExpr);
                var lambda = parameter.CreateLambdaExpresion(list);
                var dataList = await query.GetValuesAsync(list);
                var displayFunc = TextExpression.Compile();
                var valueFunc = Expression.Lambda(pKeyExpr, parameter).Compile();
                Items = new List<TreeViewItem>();
                foreach (var item in dataList)
                {
                    var text = Convert.ToString(displayFunc.DynamicInvoke(item));
                    var value = Convert.ToString(valueFunc.DynamicInvoke(item));
                    Items.Add(new TreeViewItem()
                    {
                        Text = text,
                        Value = value
                    });
                }
            }
        }

        void GetSeletcedItems(TreeViewItem item, IList<TreeViewItem> list)
        {
            if (item.Selected)
                list.Add(item);
            if (item.Items != null)
            {
                foreach (var item1 in item.Items)
                    GetSeletcedItems(item1, list);

            }
        }

        public IList<TreeViewItem> GetSeletcedItems()
        {
            var list = new List<TreeViewItem>();
            foreach (var item in Source)
                GetSeletcedItems(item, list);
            return list;
        }

        protected override async Task OnInitializedAsync()
        {
            await DataBinding();
            await base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            if (CascadeData == null)
                CascadeData = new TreeViewCascadeData();
            CascadeData.Template = Template;
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JSRuntime.InvokeVoidAsync("$.telerik.bindTree", tree);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
