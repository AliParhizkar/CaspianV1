using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
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
                    CrudGrid.SetDeleteMessage("");
                    var masterIdInfo = typeof(TAccess).GetForeignKey(typeof(TMaster));
                    Expression expr = Expression.Property(Expression.Parameter(typeof(TAccess)), masterIdInfo);
                    if (expr.Type.IsNullableType())
                        expr = Expression.Property(expr, "Value");
                    expr = Expression.Equal(expr, Expression.Constant(Convert.ChangeType(MasterId, masterIdInfo.PropertyType.GetUnderlyingType())));
                    CrudGrid.InternalConditionExpr = expr;
                }
                if (MemberGrid != null)
                {
                    var masterIdInfo = typeof(TAccess).GetForeignKey(typeof(TMaster));
                    var u = Expression.Parameter(typeof(TAccess), "u");
                    Expression innerExpr = Expression.Property(u, masterIdInfo);
                    if (innerExpr.Type.IsNullableType())
                        innerExpr = Expression.Property(innerExpr, "Value");
                    innerExpr = Expression.Equal(innerExpr, Expression.Constant(Convert.ChangeType(MasterId, masterIdInfo.PropertyType.GetUnderlyingType())));
                    innerExpr = Expression.Lambda(innerExpr, u);
                    var accessListInf = typeof(TMember).GetProperties().Where(t => typeof(IEnumerable<TAccess>).IsAssignableFrom(t.PropertyType));
                    if (accessListInf.Count() != 1)
                        throw new CaspianException("Error: Type " + typeof(TMember).Name + " Must has a Property of Type IEnumerable<" + typeof(TAccess).Name + ">");
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
            var masterIdInfo = typeof(TAccess).GetForeignKey(typeof(TMaster));
            var masterType = masterIdInfo.PropertyType.GetUnderlyingType();
            masterIdInfo.SetValue(UpsertData, Convert.ChangeType(MasterId, masterType));
            var memberIdInfo = typeof(TAccess).GetForeignKey(typeof(TMember));
            var pKey = typeof(TAccess).GetPrimaryKey();
            pKey.SetValue(UpsertData, Convert.ChangeType(0, pKey.PropertyType));
            memberIdInfo.SetValue(UpsertData, Convert.ChangeType(MemberGrid.SelectedRowId ?? 0, masterType));
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
                ShowMessage("The item has been removed from the system");
                return;
            }
            var memberInfos = typeof(TAccess).GetProperties().Where(t => t.PropertyType == typeof(TMember));
            if (memberInfos.Count() > 1)
                throw new CaspianException("Error: More than a property of type " + typeof(TMember).Name + " in type " + typeof(TAccess).Name + " is Exist");
            if (memberInfos.Count() == 0)
                throw new CaspianException("Error: No property of type " + typeof(TMember).Name + " in type " + typeof(TAccess).Name + " is Exist");
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
