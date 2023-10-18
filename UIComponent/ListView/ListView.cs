using System;
using System.Linq;
using Caspian.Common;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Caspian.UI
{
    public partial class ListView<TEntity>: DataView<TEntity> where TEntity : class
    {
        IList<TEntity> items;
        IList<Expression> fieldsExpression;
        int total;

        protected override void OnInitialized()
        {
            ContentHeight = 400;
            PageSize = 4;
            base.OnInitialized();
        }

        public override async Task DataBind()
        {
            if (fieldsExpression != null && shouldFetchData)
            {
                shouldFetchData = false;
                using var service = ServiceScopeFactory.CreateScope().GetService<BaseService<TEntity>>();
                var query = service.GetAll();
                if (ExpressionCondition !=  null)
                    query = query.Where(ExpressionCondition);
                total = await query.CountAsync();
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
                items = await query.Skip((pageNumber - 1) * PageSize).Take(PageSize).GetValuesAsync(exprList);
            }
        }

        //async Task ChangePageNumber(int pageNum)
        //{
        //    if (pageNumber != pageNum)
        //    {
        //        pageNumber = pageNum;
        //        shouldFetchData = true;
        //        await DataBind();
        //    }
        //}

        internal void AddDataField(Expression expression)
        {
            fieldsExpression.Add(expression);
        }

        //[Parameter]
        //public RenderFragment<TEntity> EditTemplate { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ExpressionCondition { get; set; }

        [Parameter]
        public RenderFragment<TEntity> Fields { get; set; }

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
            else if (FormAppState.Element.HasValue)
            {
                await jsRuntime.InvokeVoidAsync("$.caspian.focusAndShowErrorMessage", FormAppState.Element);
                FormAppState.Element = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
