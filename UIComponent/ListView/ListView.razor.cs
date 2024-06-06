using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class ListView<TEntity>: DataView<TEntity>, IListViewer<TEntity> where TEntity : class
    {
        IList<Expression> fieldsExpression;
        WindowStatus status;

        protected override void OnInitialized()
        {
            PageSize = 4;
            base.OnInitialized();
        }

        public void OpenPopupWindow()
        {
            status = WindowStatus.Open;
            CreateInsert();
        }

        public override async Task DataBind()
        {
            if (fieldsExpression != null && shouldFetchData)
            {
                shouldFetchData = false;
                using var service = ServiceScopeFactory.CreateScope().GetService<BaseService<TEntity>>();
                var query = service.GetAll();
                if (ConditionExpr != null)
                    query = query.Where(ConditionExpr);
                Total = await query.CountAsync();
                var exprList = new List<MemberExpression>();
                foreach(var expr in fieldsExpression)
                {
                    var tempList = new ExpressionSurvey().Survey(expr);
                    foreach (var expr2 in tempList)
                        if (!exprList.Any(t => t.ToString() == expr2.ToString()))
                            exprList.Add(expr2);
                }
                var parameterExpr = Expression.Parameter(typeof(TEntity), "t");
                var pKey = typeof(TEntity).GetPrimaryKey();
                var primaryKeyExpr = Expression.Property(parameterExpr, pKey);
                if (!exprList.Any(t => t.ToString() == primaryKeyExpr.ToString()))
                    exprList.Add(primaryKeyExpr);
                foreach (var info in typeof(TEntity).GetProperties())
                {
                    if (info.PropertyType.IsValueType || info.PropertyType.IsNullableType())
                    {
                        var str = parameterExpr.Name + "." + info.Name;
                        if (!exprList.Any(t => t.ToString() == str))
                            exprList.Add(Expression.Property(parameterExpr, info));
                    }
                }
                if (Batch)
                {
                    source = (await query.GetValuesAsync<TEntity>(exprList)).ToList();
                    if (pageNumber == 1)
                        items = source.Take(PageSize).ToList();
                    else
                    {
                        var skip = (pageNumber - 1) * PageSize;
                        items = source.Skip(skip).Take(PageSize).ToList();
                    }
                    ManageExpressionForUpsert(exprList);
                }
                else
                    items = await query.Skip((pageNumber - 1) * PageSize).Take(PageSize).GetValuesAsync(exprList);
            }
        }

        public void AddDataField(Expression expression)
        {
            fieldsExpression.Add(expression);
        }

        [Parameter]
        public RenderFragment<RowData<TEntity>> Fields { get; set; }

        [Parameter]
        public UpsertType UpsertType { get; set; }

        [Parameter]
        public RenderFragment HeaderTempalte { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await DataBind();
            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await DataBind();
                StateHasChanged();
            }
            if (insertContinerHouldhasFocus)
            {
                insertContinerHouldhasFocus = false;
                await insertContiner.FocusAsync();
            }
            if (shouldSetFocuc)
            {
                shouldSetFocuc = false;
                await updateContiner.FocusAsync();
            }
            if (errorMessage.HasValue())
            {
                await jsRuntime.InvokeVoidAsync("$.caspian.showMessage", errorMessage);
                errorMessage = null;
            }
            else if (FormAppState.Control != null)
            {
                await FormAppState.Control.FocusAsync();
                FormAppState.Control = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }


    }
}
