using Caspian.Common;
using FluentValidation;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;

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
            scope.GetService<CaspianDataService>().UserId = userId;
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

                var service = scope.GetService<IBaseService<TEntity>>();
                if (service == null)
                    throw new CaspianException($"Error: Service od type ISimpleService<{typeof(TEntity).Name}> not implimented");
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
                    info.SetValue(UpsertForm.EditContext.Model, Convert.ChangeType(0, info.PropertyType));
                    if (UpsertWindow != null)
                        await UpsertWindow.Close();
                });
            }
        }

        async Task DeleteAsync(TEntity data)
        {
            using var scope = CreateScope();
            var service = scope.GetService<IBaseService<TEntity>>();
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
                if (CrudGrid.Search != null && CrudGrid.Search == UpsertData)
                    throw new CaspianException("Error: Search object and UpsertData object is same");
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
                            if (UpsertData == null)
                                throw new CaspianException("Spicify UpsertData Parameter in CRUD Component.");
                            UpsertData.CopySimpleProperty(tempData);
                            if (UpsertForm != null && !UpsertForm.OnInternalSubmit.HasDelegate)
                                FormInitial();
                            
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
