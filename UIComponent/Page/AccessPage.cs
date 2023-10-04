using System;
using System.Linq;
using Caspian.Common;
using System.Reflection;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class AccessPage<TMaster, TAccess, TMember> : SimplePage<TAccess> where TAccess : class where TMember : class
    {
        protected TMember MemberSearch { get; set; } = Activator.CreateInstance<TMember>();

        protected DataGrid<TMember> MemberGrid { get; set; }

        protected override void OnInitialized()
        {
            var info = typeof(TAccess).GetProperties().Single(t => t.PropertyType == typeof(TMember));
            var member = Activator.CreateInstance<TMember>();
            info.SetValue(SearchData, member);
            base.OnInitialized();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && UpsertForm != null)
            {
                if (CrudGrid != null)
                {
                    CrudGrid.SetDeleteMessage(null);
                    var masterInfos = typeof(TAccess).GetProperties().Where(t => t.PropertyType == typeof(TMaster));
                    if (masterInfos.Count() > 1)
                        throw new CaspianException("خطا: More than a property of type " + typeof(TMaster).Name + " in type " + typeof(TAccess).Name + " is Exist");
                    if (masterInfos.Count() == 0)
                        throw new CaspianException("خطا: No property of type " + typeof(TMaster).Name + " in type " + typeof(TAccess).Name + " is Exist");
                    var masterInfo = masterInfos.Single();
                    var foreignKeyAttr = masterInfo.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKeyAttr == null)
                        throw new CaspianException("Property " + masterInfo.Name + "in type " + typeof(TAccess).Name + "has not ForeignKey Attribute");
                    Expression expr = Expression.Property(Expression.Parameter(typeof(TAccess)), foreignKeyAttr.Name);
                    if (expr.Type.IsNullableType())
                        expr = Expression.Property(expr, "Value");
                    expr = Expression.Equal(expr, Expression.Constant(MasterId));
                    CrudGrid.InternalConditionExpr = expr;
                }
                if (MemberGrid != null)
                {
                    var masterIdName = typeof(TAccess).GetProperties().Single(t => t.PropertyType == typeof(TMaster)).GetCustomAttribute<ForeignKeyAttribute>().Name;
                    var u = Expression.Parameter(typeof(TAccess));
                    Expression innerExpr = Expression.Property(u, masterIdName);
                    if (innerExpr.Type.IsNullableType())
                        innerExpr = Expression.Property(innerExpr, "Value");
                    innerExpr = Expression.Equal(innerExpr, Expression.Constant(MasterId));
                    innerExpr = Expression.Lambda(innerExpr, u);
                    var accessListInf = typeof(TMember).GetProperties().Where(t => typeof(IEnumerable<TAccess>).IsAssignableFrom(t.PropertyType));
                    if (accessListInf.Count() != 1)
                        throw new CaspianException("خطا: Type " + typeof(TMember).Name + " Must has a Property of Type IEnumerable<" + typeof(TAccess).Name + ">");
                    Expression expression = Expression.Property(Expression.Parameter(typeof(TMember)), accessListInf.Single());
                    var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Any").LastOrDefault().MakeGenericMethod(typeof(TAccess));
                    expression = Expression.Call(method, expression, innerExpr);
                    expression = Expression.Not(expression);
                    MemberGrid.InternalConditionExpr = expression;
                }
                if (UpsertForm != null)
                {
                    if (!UpsertForm.OnInternalSubmit.HasDelegate)
                    {
                        UpsertForm.OnInternalSubmit = EventCallback.Factory.Create<EditContext>(this, (context) =>
                        {
                            SetData(context);
                        });
                    }
                    if (!UpsertForm.OnInternalValidSubmit.HasDelegate)
                    {
                        UpsertForm.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (context) =>
                        {
                            await UpsertAsync((TAccess)context.Model);
                        });
                    }
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [Parameter]
        public int MasterId { get; set; }

        protected async Task AddAll()
        {
            using var scope = CreateScope();
            var query = MemberGrid.GetAllEntities(scope);
            var count = query.Count();
            if (await Confirm($"Do you add {count} records"))
            {
                
            }
        }

        void SetData(EditContext context)
        {
            var masterInfos = typeof(TAccess).GetProperties().Where(t => t.PropertyType == typeof(TMaster));
            if (masterInfos.Count() > 1)
                throw new CaspianException("More than a property of type " + typeof(TMaster).Name + " in type " + typeof(TAccess).Name + " is Exist");
            if (masterInfos.Count() == 0)
                throw new CaspianException("No property of type " + typeof(TMaster).Name + " in type " + typeof(TAccess).Name + " is Exist");
            var masterInfo = masterInfos.Single();
            var foreignKeyAttr = masterInfo.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKeyAttr == null)
                throw new CaspianException("Property " + masterInfo.Name + "in type " + typeof(TAccess).Name + "has not ForeignKey Attribute");
            typeof(TAccess).GetProperty(foreignKeyAttr.Name).SetValue(UpsertData, MasterId);
            var memberInfos = typeof(TAccess).GetProperties().Where(t => t.PropertyType == typeof(TMember));
            if (memberInfos.Count() > 1)
                throw new CaspianException("More than a property of type " + typeof(TMember).Name + " in type " + typeof(TAccess).Name + " is Exist");
            if (memberInfos.Count() == 0)
                throw new CaspianException("No property of type " + typeof(TMember).Name + " in type " + typeof(TAccess).Name + " is Exist");
            var memberInfo = memberInfos.Single();
            foreignKeyAttr = memberInfo.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKeyAttr == null)
                throw new CaspianException("Property " + masterInfo.Name + "in type " + typeof(TAccess).Name + "has not ForeignKey Attribute");
            var memberIdInfo = typeof(TAccess).GetProperty(foreignKeyAttr.Name);
            typeof(TAccess).GetPrimaryKey().SetValue(UpsertData, 0);
            memberIdInfo.SetValue(UpsertData, MemberGrid.SelectedRowId);
        }

        protected override async Task UpsertAsync(TAccess data)
        {
            var Pkey = typeof(TAccess).GetPrimaryKey();
            var id = Convert.ToInt32(Pkey.GetValue(data));
            if (id > 0)
                Pkey.SetValue(data, 0);
            await base.UpsertAsync(data);
            await MemberGrid.ReloadAsync();
        }

        protected async override Task DeleteAsync(TAccess data = null)
        {
            if (data == null)
            {
                var tempData = CrudGrid.GetSelectedData();
                if (tempData == null)
                {
                    if (dataService.Language == Language.Fa)
                        ShowMessage("لطفا یک ردیف را برای حذف انتخاب نمایید.");
                    else
                        ShowMessage("Please select a row to delete");
                    return;
                }
                data = tempData;
            }
            var id = Convert.ToInt32(typeof(TAccess).GetPrimaryKey().GetValue(data));
            using var scope = CreateScope();
            var service = new BaseService<TAccess>(scope.ServiceProvider);
            var old = await service.SingleOrDefaultAsync(id);
            if (old == null)
            {
                ShowMessage("آیتم از سیستم حذف شده است");
                return;
            }
            var memberInfos = typeof(TAccess).GetProperties().Where(t => t.PropertyType == typeof(TMember));
            if (memberInfos.Count() > 1)
                throw new CaspianException("خطا: More than a property of type " + typeof(TMember).Name + " in type " + typeof(TAccess).Name + " is Exist");
            if (memberInfos.Count() == 0)
                throw new CaspianException("خطا: No property of type " + typeof(TMember).Name + " in type " + typeof(TAccess).Name + " is Exist");
            var memberInfo = memberInfos.Single();
            var foreignKeyAttrMember = memberInfo.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKeyAttrMember == null)
                throw new CaspianException("Property " + memberInfo.Name + "in type " + typeof(TAccess).Name + "has not ForeignKey Attribute");
            var memberId = Convert.ToInt32(typeof(TAccess).GetProperty(foreignKeyAttrMember.Name).GetValue(old));
            await base.DeleteAsync(old);
            await MemberGrid.SelectRowById(memberId);
        }

    }
}
