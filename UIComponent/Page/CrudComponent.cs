using System;
using System.Linq;
using Caspian.Common;
using FluentValidation;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Routing;

namespace Caspian.UI
{
    public partial class CrudComponent<TEntity>: ICrudComponent where TEntity : class
    {
        string errorMessage;
        MessageBox messageBox;
        int userId;

        public DataGrid<TEntity> CrudGrid { get; set; }

        public Window UpsertWindow { get; set; }

        public CaspianForm<TEntity> UpsertForm { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public TEntity UpsertData { get; set; }

        [CascadingParameter]
        Task<AuthenticationState> authenticationStateTask { get; set; }

        [Parameter]
        public EventCallback<FormData<TEntity>> OnUpsert { get; set; }

        IServiceScope CreateScope()
        {
            var scope = ServiceScopeFactory.CreateScope();
            scope.ServiceProvider.GetService<CaspianDataService>().UserId = userId;
            return scope;
        }

        protected override async Task OnInitializedAsync()
        {
            if (authenticationStateTask != null)
            {
                var result = await authenticationStateTask;
                userId = Convert.ToInt32(result.User.Claims.FirstOrDefault()?.Value);
            }
            await base.OnInitializedAsync();
        }

        async Task UpsertAsync(TEntity data)
        {
            var formData = new FormData<TEntity>()
            {
                Model = data
            };
            if (OnUpsert.HasDelegate)
                await OnUpsert.InvokeAsync(formData);
            if (!formData.Cancel)
            {
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(data));
                using var scope = CreateScope();

                var service = scope.ServiceProvider.GetService<IBaseService<TEntity>>();
                if (service == null)
                    throw new CaspianException("خطا: Service od type ISimpleService<" + typeof(TEntity).Name + "> not implimented", null);
                if (id == 0)
                    await service.AddAsync(data);
                else
                    await service.UpdateAsync(data);
                await service.SaveChangesAsync();
                if (id == 0)
                {
                    id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(data));
                    await CrudGrid.SelectRowById(id);
                }
                else
                    await CrudGrid.ReloadAsync();
                if (UpsertWindow != null)
                    await UpsertWindow.Close();
                await UpsertForm.ResetAsync();
            }
        }

        void ICrudComponent.SetWindow(Window window)
        {
            UpsertWindow = window;
        }
        void FormInitial()
        {
            if (UpsertForm != null)
            {
                UpsertForm.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (EditContext context) =>
                {
                    await UpsertAsync((TEntity)context.Model);
                });
                UpsertForm.OnInternalReset = EventCallback.Factory.Create(this, async () =>
                {
                    var info = typeof(TEntity).GetPrimaryKey();
                    var field = new FieldIdentifier(UpsertForm.EditContext.Model, info.Name);
                    info.SetValue(UpsertForm.EditContext.Model, 0);
                    if (UpsertWindow != null)
                        await UpsertWindow.Close();
                });
            }
        }

        async Task DeleteAsync(TEntity data)
        {
            using var scope = CreateScope();
            var service = scope.ServiceProvider.GetService<IBaseService<TEntity>>();
            var result = await service.ValidateAsync(data, t => t.IncludeRuleSets("Remove"));
            if (result.IsValid)
            {
                errorMessage = null;
                if (!CrudGrid.DeleteMessage.HasValue() || await messageBox.Confirm(CrudGrid.DeleteMessage))
                {
                    var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(data));
                    var old = await service.SingleAsync(id);
                    await service.RemoveAsync(old);
                    try
                    {
                        await service.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException exp)
                    {
                        errorMessage = "این آیتم قبلا حذف شده است";
                    }
                    await CrudGrid.ReloadAsync();
                }
            }
            else
                errorMessage = result.Errors.First().ErrorMessage;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            FormInitial();
            if (CrudGrid != null)
            {
                CrudGrid.OnInternalDelete = EventCallback.Factory.Create<TEntity>(this, async (data) =>
                {
                    await DeleteAsync(data);
                });
                CrudGrid.OnInternalUpsert = EventCallback.Factory.Create<TEntity>(this, async (data) =>
                {
                    if (UpsertWindow != null)
                    {
                        await UpsertWindow.Open();
                        UpsertWindow.OnInternalOpen = EventCallback.Factory.Create(this, async () =>
                        {
                            using var scope = CreateScope();
                            var service = new BaseService<TEntity>(scope.ServiceProvider);
                            var value = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(data));
                            TEntity tempData = null;
                            if (value == 0)
                                tempData = Activator.CreateInstance<TEntity>();
                            else
                                tempData = await service.SingleAsync(value);
                            UpsertData.CopySimpleProperty(tempData);
                            if (UpsertForm != null && !UpsertForm.OnInternalSubmit.HasDelegate)
                                FormInitial();
                            UpsertForm?.FocusToFirstControl();
                        });
                    }
                    else
                    {
                        var value = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(data));
                        var tempData = default(TEntity);
                        if (value == 0)
                            tempData = Activator.CreateInstance<TEntity>();
                        else
                        {
                            using var scope = CreateScope();
                            var service = new BaseService<TEntity>(scope.ServiceProvider);
                            tempData = await service.SingleAsync(value);
                        }
                        UpsertData.CopySimpleProperty(tempData);
                        if (UpsertForm != null && !UpsertForm.OnInternalSubmit.HasDelegate)
                            FormInitial();
                        UpsertForm?.FocusToFirstControl();
                    }
                });
                if (errorMessage != null)
                {
                    await jsRuntime.InvokeVoidAsync("$.caspian.showMessage", errorMessage);
                    errorMessage = null;
                }
            }
            if (UpsertWindow != null)
            {
                UpsertWindow.OnInternalClose = EventCallback.Factory.Create(this, async () =>
                {
                    await UpsertWindow.Close();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
