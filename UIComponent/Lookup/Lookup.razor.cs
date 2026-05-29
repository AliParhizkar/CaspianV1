using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Caspian.UI
{
    public partial class Lookup<TEntity, TValue> : CBaseInput<TValue>, ILookup<TEntity> where TEntity: class
    {
        TValue oldValue;
        bool mustClear;
        string SearchStr;
        bool shouldRender;
        DataGrid<TEntity> grid;
        Dictionary<string, object> inputAttrs = new Dictionary<string, object>();
        WindowStatus status;
        bool valueUpdated, advanceSearch;

        internal Action<TEntity> OnSelect { get; set; }

        [Parameter]
        public bool HideHeader { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string WindowTitle { get; set; } = "حستجو ...";

        [Parameter]
        public bool HideIcon { get; set; }

        [Parameter]
        public bool AutoHide { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter]
        public bool OpenOnFocus { get; set; }

        [Parameter]
        public bool CloseOnBlur { get; set; }

        [CascadingParameter]
        public CaspianContainer Container { get; set; }

        [Parameter]
        public string PlaceHolder { get; set; }

        [Parameter]
        public EventCallback<SelectStatus> OnSelecting { get; set; }

        [Parameter]
        public bool AlwaysAdvanceSearch { get; set; }

        IDictionary<string, object> GetMainAttribute()
        {
            var className = "t-widget c-lookup";
            if (Disabled)
                className += " t-state-disabled";
            if (ErrorMessage.HasValue())
                className += " t-state-error";
            var dic = new Dictionary<string, object>()
            {
                {"style", Style }, {"closeOnBlur", CloseOnBlur}, {"error-message", ErrorMessage}, {"class", className}, 
                {"autoHide", AutoHide}, {"advanceSearch", advanceSearch}
            };
            return dic;
        }

        void OpenWindow(bool advanceSearch = false)
        {
            if (!Disabled)
            {
                if (status == WindowStatus.Close)
                    status = WindowStatus.Open;
                this.advanceSearch = advanceSearch;
            }
        }

        [JSInvokable]
        public async Task SetSearchValue(string value)
        {
            OpenWindow();
            if (mustClear)
         {
                await SetTextOnClientAsync("");
                SearchStr = "";
                mustClear = false;
            }
            else
                SearchStr = value;
            StateHasChanged();
        }

        public async Task SetTextOnClientAsync(string text)
        {
            if (InputElement != null)
                await jsRuntime.InvokeVoidAsync("caspian.common.setValueOnClient", InputElement, text);
        }

        protected override void OnInitialized()
        {
            shouldRender = true;
            status = WindowStatus.Close;
            base.OnInitialized();
        }

        protected override bool ShouldRender()
        {
            if (shouldRender)
                return true;
            shouldRender = true;
            return false;
        }

        internal async Task<bool> CloseHelpForm(TEntity selectedEntity, bool shouldRender = false)
        {
            var flag = selectedEntity == null || !OnSelecting.HasDelegate;
            if (!flag)
            {
                var selectStatus = new SelectStatus();
                selectStatus.Id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(selectedEntity));
                await OnSelecting.InvokeAsync(selectStatus);
                flag = !selectStatus.Cancel;
            }
            if (flag)
            {
                status = WindowStatus.Close;
                if (grid != null)
                {
                    shouldRender = true;
                    grid.EnableLoading();
                }
                if (shouldRender)
                    StateHasChanged();
                return true;
            }
            return false;
        }

        [JSInvokable]
        public async Task Close()
        {
            await CloseHelpForm(null, true);
        }

        async Task OnKeyUp(KeyboardEventArgs e)
        {
            if (valueUpdated && (e.Code == "Enter" || e.Code == "NumpadEnter"))
            {
                valueUpdated = false;
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync();
            }
            else
                shouldRender = false;
        }

        internal async Task<string> GetText(int value)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService(typeof(IBaseService<TEntity>)) as BaseService<TEntity>;
            var t = Expression.Parameter(typeof(TEntity), "t");
            Expression expr = Expression.Property(t, typeof(TEntity).GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(value));
            if (TextExpression == null)
                throw new CaspianException("خطا: Please specify TextExpression parameter in lookup component");
            var result = await service.GetAll().Where(Expression.Lambda(expr, t)).Select(TextExpression).FirstAsync();
            return result;
        }

        async Task ILookup<TEntity>.SelectOnLookup(bool changeState)
        {
            if (grid?.SelectedRowId != null)
            {
                if (OnSelect != null)
                    OnSelect(grid.GetSelectedData());
                var result = await CloseHelpForm(grid.GetSelectedData());
                if (result)
                {
                    var value = grid.SelectedRowId.Value;
                    await SetValue(value, false);
                    valueUpdated = true;
                    oldValue = Value;
                    var text = await GetText(value);
                    await SetTextOnClientAsync(text);
                    if (changeState)
                        StateHasChanged();
                }
            }
        }

        async Task OnKeyDownHandler(KeyboardEventArgs e)
        {
            switch (e.Code)
            {
                case "ArrowUp":
                    grid?.SelectPrevRow();
                    break;
                case "ArrowDown":
                    grid?.SelectNextRow();
                    break;
                case "Enter":
                case "NumpadEnter":
                    await (this as ILookup<TEntity>).SelectOnLookup(false);
                    break;
                case "Escape":
                    status = WindowStatus.Close;
                    break;
                case "Backspace":
                    if (!Value.Equals(default(TValue)))
                    {
                        Value = default(TValue);
                        oldValue = Value;
                        await ValueChanged.InvokeAsync(default(TValue));
                        if (OnChange.HasDelegate)
                            await OnChange.InvokeAsync();
                        mustClear = true;
                    }
                    grid?.SelectFirstPage();
                    grid?.SelectFirstRow();
                    break;
                default:
                        shouldRender = false;
                    break;
            }

        }

        void ILookup<TEntity>.SetAndInitializeGrid(DataGrid<TEntity> grid)
        {
            this.grid = grid;
            grid.SelectFirstRow();
            grid.OnInternalRowSelect = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                if (OnSelect != null)
                    OnSelect(entity);
                if(await CloseHelpForm(entity))
                {
                    var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                    await SetValue(id);
                    await SetTextAsync();
                }
            });
        }

        bool ILookup<TEntity>.AdvanceSearch { get { return advanceSearch; } }

        protected override void OnParametersSet()
        {
            if (AlwaysAdvanceSearch)
                advanceSearch = true;
            inputAttrs = new Dictionary<string, object>();
            inputAttrs["class"] = AutoHide ? "t-input auto-hide" : "t-input";
            if (PlaceHolder.HasValue())
                inputAttrs.Add("placeholder", PlaceHolder);
            Container?.SetControl(this);
            if (OpenOnFocus)
            {
                inputAttrs.Add("onfocus", new Action(() =>
                {
                    OpenWindow();
                    SearchStr = "";
                }));
            }
            if (Disabled)
                inputAttrs.Add("disabled", true);
            if (HideHeader)
                AutoHide = true;
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                CaspianForm?.SetFirstControl(this);
                await jsRuntime.InvokeVoidAsync("caspian.common.bindLookup", InputElement, dotnet);
            }

            if (!Value.IsEqual(oldValue) && (Value == null || Value.Equals(0)))
            {
                oldValue = Value;
                await jsRuntime.InvokeVoidAsync("caspian.common.setValueOnClient", InputElement, "");
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        async Task SetTextAsync()
        {
            string text = null;
            if (Value == null || Value.Equals(0))
                text = "";
            else if (!Value.Equals(oldValue))
            {
                oldValue = Value;
                text = await GetText(Convert.ToInt32(Value));
            }
            if (text != null)
                await SetTextOnClientAsync(text);
        }

        public async Task SetValue(long id, bool fireEvent = true)
        {
            if (!Disabled)
            {
                var type = typeof(TValue);
                if (type.IsNullableType())
                    type = Nullable.GetUnderlyingType(type);
                var tempValue = Convert.ChangeType(id, type);
                Value = (TValue)tempValue;
                Value = (TValue)tempValue;
                if (fireEvent)
                {
                    if (ValueChanged.HasDelegate)
                        await ValueChanged.InvokeAsync(Value);
                    if (OnChange.HasDelegate)
                        await OnChange.InvokeAsync();
                }
                BindEditContext();
                EntitySearch?.EnableLoadData();
            }
        }

        public void SetSearchStringValue(string searchStr)
        {
            SearchStr = searchStr;
            StateHasChanged();
        }
    }

    internal interface ILookup<TEntity> where TEntity : class
    {
        void SetAndInitializeGrid(DataGrid<TEntity> grid);

        Task SelectOnLookup(bool changeState);

        bool AdvanceSearch { get; }
    }

    public class SelectStatus
    {
        public int Id { get; internal set; }

        public bool Cancel { get; set; }
    }
}
