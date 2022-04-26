using System;
using System.Linq;
using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity>
    {
        int EditngEntityId;
        int? OldEditngEntityId;
        IList<TEntity> EditList;
        EditContext EditingContext;
        int? singleInserIndex;
        int? oldSingleInsertIndex;
        EditContext singleInsertEditContext;
        bool sholdSingleInsertEditContextValidated;
        CaspianContainer updateContainer;
        CaspianContainer singleInsertContainer;
        bool sholdFocucedOnUpdating;
        bool sholdFocucedOnSingleUpsert;
        CaspianValidationValidator InsertValidator;

        internal ICaspianForm MasterForm { get; set; }

        [Parameter, JsonIgnore]
        public Type ServiceType { get; set; }

        [Parameter]
        public bool MultiInsert { get; set; }

        [Parameter]
        public bool AutoInsert { get; set; }

        internal async Task SetEditngEntityId(int id)
        {
            if (EditngEntityId > 0)
                await SetEditedItemBySelectExpression();
            EditngEntityId = id;
            StateHasChanged();
        }

        async Task SetEditedItemBySelectExpression()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            var pKey = typeof(TEntity).GetPrimaryKey();
            Expression expr = Expression.Property(parameter, pKey);
            expr = Expression.Equal(expr, Expression.Constant(EditngEntityId));
            var lambda = Expression.Lambda(expr, parameter);
            var entity = (await new SimpleService<TEntity>(scope).GetAll().Where(lambda).OfType<TEntity>()
                .GetValuesAsync(SelectExpressions)).FirstOrDefault();
            var index = 0;
            foreach (var item in Items)
            {
                if (Convert.ToInt32(pKey.GetValue(item)) == EditngEntityId)
                {
                    Items[index] = entity;
                    break;
                }
                index++;
            }
        }

        internal async Task ValidateAndUpdate()
        {
            FormAppState.AllControlsIsValid = true;
            var result = EditingContext.Validate();
            if (result)
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var service = new SimpleService<TEntity>(scope);
                await service.UpdateAsync(EditingContext.Model as TEntity);
                await service.SaveChangesAsync();
                await SetEditedItemBySelectExpression();
                EditngEntityId = 0;
                OldEditngEntityId = null;
            }
            StateHasChanged();
        }

        async Task ResetForAutoInsert()
        {
            EditList = new List<TEntity>();
            oldSingleInsertIndex = null;
            singleInserIndex = null;
            var entity = Activator.CreateInstance<TEntity>();
            if (OnInternalUpsert.HasDelegate)
                await OnInternalUpsert.InvokeAsync(entity);
            if (OnUpsert.HasDelegate)
                await OnUpsert.InvokeAsync(entity);
            AddToInsertList(entity);
        }

        internal async Task SetSingleInsertIndex(int index)
        {
            if (singleInserIndex == index)
                await ValidateAndInsert();
            else
            {
                singleInserIndex = index;
                StateHasChanged();
            }
        }

        async Task ValidateAndInsert()
        {
            FormAppState.AllControlsIsValid = true;
            var masterIsNullOrValid = true;
            string masterFKName = null; ;
            object masterModel = null;
            Type masterType = null;
            object masterId = null;
            if (MasterForm != null)
            {
                masterModel = MasterForm.EditContext.Model;
                masterType = masterModel.GetType();
                masterId = masterType.GetPrimaryKey().GetValue(masterModel);
                masterFKName = typeof(TEntity).GetProperties().Single(t => t.PropertyType == masterType)
                    .GetCustomAttribute<ForeignKeyAttribute>().Name;
                if (masterId.Equals(0))
                {
                    MasterForm.ValidationValidator.IgnoreDetailsProperty = true;
                    MasterForm.IgnoreOnValidSubmit = true;
                    masterIsNullOrValid = MasterForm.EditContext.Validate();
                }
                else
                    typeof(TEntity).GetProperty(masterFKName).SetValue(singleInsertEditContext.Model, masterId);
            }
            var detailResult = singleInsertEditContext.Validate();
            if (masterIsNullOrValid && detailResult)
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var service = new SimpleService<TEntity>(scope);
                var entity = (singleInsertEditContext.Model as TEntity).Copy();
                if (masterFKName.HasValue())
                {
                    var masterId1 = typeof(TEntity).GetProperty(masterFKName).GetValue(entity);
                    if (masterId1 == null || masterId1.Equals(0))
                    {
                        var info = typeof(TEntity).GetProperties().Single(t => t.PropertyType == masterType);
                        var masterData = masterModel.Copy();
                        info.SetValue(entity, masterData);
                    }
                }
                await service.AddAsync(entity);
                await service.SaveChangesAsync();
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                if (masterModel != null && masterId.Equals(0))
                {
                    var fKName = typeof(TEntity).GetProperties().Single(t => t.PropertyType == masterType)
                        .GetCustomAttribute<ForeignKeyAttribute>().Name;
                    var fKValue = typeof(TEntity).GetProperty(fKName).GetValue(entity);
                    masterType.GetPrimaryKey().SetValue(masterModel, fKValue);
                }
                await this.SelectRowById(id);
                if (AutoInsert)
                    await ResetForAutoInsert();
                else
                {
                    EditList.Remove(EditList[singleInserIndex.Value]);
                    oldSingleInsertIndex = null;
                    singleInserIndex = null;
                }
            }
            StateHasChanged();
        }

        public async Task InsertAsync(int? index = null)
        {
            if (MultiInsert && index == null)
                throw new CaspianException("خطا: In multi insert index can not be null");
            index = index.GetValueOrDefault();
            if (singleInserIndex == index)
                await ValidateAndInsert();
            else
            {
                singleInserIndex = index;
                StateHasChanged();
            }
        }

        private Expression UpdateExpressionForMasterForm(ParameterExpression parameter, Expression expr)
        {
            if (MasterForm != null)
            {
                var masetrEntity = MasterForm.EditContext.Model;
                var masterType = masetrEntity.GetType();
                var id = masterType.GetPrimaryKey().GetValue(masetrEntity);
                var fKName = typeof(TEntity).GetProperties().Single(t => t.PropertyType == masterType)
                    .GetCustomAttribute<ForeignKeyAttribute>().Name;
                Expression formExpr = Expression.Property(parameter, fKName);
                formExpr = Expression.Equal(formExpr, Expression.Constant(id));
                if (expr == null)
                    expr = formExpr;
                else
                    expr = Expression.And(expr, formExpr);
            }
            return expr;
        }

        internal void AddToInsertList(TEntity entity)
        {
            if (EditList == null)
            {
                EditList = new List<TEntity>();
                if (MasterForm != null)
                {
                    var obj = MasterForm.EditContext.Model;
                    foreach (var info in obj.GetType().GetProperties().Where(t => t.PropertyType.IsGenericType))
                    {
                        if (info.PropertyType.IsEnumerableType() && info.PropertyType.GetGenericArguments()[0] == typeof(TEntity))
                            info.SetValue(obj, EditList);
                    }
                }
            }
            if (EditList.Count == 0 || MultiInsert)
                EditList.Add(entity);
            sholdFocucedOnSingleUpsert = true;
            StateHasChanged();
        }

        internal async Task RemoveFromList(int? index = null)
        {
            if (index.HasValue)
            {
                if (AutoInsert)
                    await ResetForAutoInsert();
                else
                {
                    EditList.RemoveAt(index.Value);
                    oldSingleInsertIndex = null;
                    singleInserIndex = null;
                }
            }
            else
            {
                await SetEditedItemBySelectExpression();
                EditngEntityId = 0;
                OldEditngEntityId = null;
            }
            StateHasChanged();
        }

        private async Task ParametersSetInitialAsync()
        {
            if (AutoInsert)
                HideInsertIcon = true;
        }

        private async Task AfterRenderInitialAsync()
        {
            if (MasterForm != null)
            {
                if (InsertValidator != null && InsertValidator.MasterIdName == null)
                {
                    var mesterType = MasterForm.EditContext.Model.GetType();
                    var info = typeof(TEntity).GetProperties().Single(t => t.PropertyType == mesterType);
                    InsertValidator.MasterIdName = info.GetCustomAttribute<ForeignKeyAttribute>().Name;
                }
                if (!MasterForm.OnInternalInvalidSubmit.HasDelegate)
                {
                    MasterForm.OnInternalInvalidSubmit = EventCallback.Factory.Create<EditContext>(this, (context) =>
                    {

                    });
                    if (AutoInsert)
                    {
                        if (EditList == null || EditList.Count == 0)
                        {
                            var entity = Activator.CreateInstance<TEntity>();
                            if (OnInternalUpsert.HasDelegate)
                                await OnInternalUpsert.InvokeAsync(entity);
                            if (OnUpsert.HasDelegate)
                                await OnUpsert.InvokeAsync(entity);
                            AddToInsertList(entity);
                        }
                    }
                }
            }

            if (sholdSingleInsertEditContextValidated)
            {
                sholdSingleInsertEditContextValidated = false;
                await ValidateAndInsert();
            }
            if (sholdFocucedOnUpdating)
            {
                sholdFocucedOnUpdating = false;
                await updateContainer.FocusAsync();
            }
            if (sholdFocucedOnSingleUpsert)
            {
                sholdFocucedOnSingleUpsert = false;
                if (singleInsertContainer != null)
                    await singleInsertContainer.FocusAsync();
            }
        }
    }
}
