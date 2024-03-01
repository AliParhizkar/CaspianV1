using System.Linq;
using Caspian.Common;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using FluentValidation.Results;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public partial class CaspianForm<TEntity>: ICaspianForm where TEntity : class
    {
        string ErrorMessage;
        bool checkValidation;
        IList<IControl> controls;
        string ICaspianForm.MasterIdName { get; set; }
        bool ICaspianForm.IgnoreOnValidSubmit { get; set; }
        TEntity oldModel;

        [CascadingParameter]
        public CrudComponent<TEntity> CrudComponent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TEntity Model { get; set; }

        public EditContext EditContext { get; private set; }

        [Parameter]
        public EventCallback<EditContext> OnInvalidSubmit { get; set; }

        [Parameter]
        public EventCallback<EditContext> OnSubmit { get; set; }

        [Parameter]
        public EventCallback OnReset { get; set; }

        [Parameter]
        public EventCallback<EditContext> OnValidSubmit { get; set; }

        internal EventCallback<EditContext> OnInternalSubmit { get; set; }

        internal EventCallback<EditContext> OnInternalValidSubmit { get; set; }

        public EventCallback<EditContext> OnInternalInvalidSubmit { get; set; }

        internal EventCallback OnInternalReset { get; set; }

        public void AddControl(IControl control)
        {
            if (!controls.Contains(control))
                controls.Add(control);
        }

        public CaspianValidationValidator ValidationValidator { get; set; }

        public async Task FocusToFirstControlAsync()
        {
            if (controls.Count > 0)
                await controls[0].FocusAsync();
        }

        protected override void OnInitialized()
        {
            controls = new List<IControl>();
            if (FormAppState == null)
                FormAppState = new FormAppState();
            if (CrudComponent != null)
                CrudComponent.UpsertForm = this;
            FormAppState.AllControlsIsValid = true;
            base.OnInitialized();
        }

        async Task OnFormSubmitHandler(EditContext context)
        {
            if (OnSubmit.HasDelegate)
                await OnSubmit.InvokeAsync(EditContext);
            if (OnInternalSubmit.HasDelegate)
                await OnInternalSubmit.InvokeAsync(EditContext);
            FormAppState.AllControlsIsValid = true;
            ErrorMessage = null;
            EditContext.Validate();
            EditContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
            var result = await (Task<ValidationResult>)asyncValidationTask;
            if (result.IsValid)
            {
                if (OnValidSubmit.HasDelegate)
                    await OnValidSubmit.InvokeAsync(context);
                if (OnInternalValidSubmit.HasDelegate)
                    await OnInternalValidSubmit.InvokeAsync(context);

            }
            else
            {
                if (FormAppState.AllControlsIsValid)
                    ErrorMessage = EditContext.GetValidationMessages().First();
                if (OnInvalidSubmit.HasDelegate)
                    await OnInvalidSubmit.InvokeAsync(EditContext);
                if (OnInternalInvalidSubmit.HasDelegate)
                    await OnInternalInvalidSubmit.InvokeAsync(EditContext);
            }
        }

        async Task ResetFormAsync()
        {
            foreach (var control in controls)
            {
                await control.ResetAsync();
            }
            if (OnReset.HasDelegate)
                await OnReset.InvokeAsync();
        }


        void ValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            //FormAppState.AllControlsIsValid = true;
            //FormAppState.Element = null;
            //ErrorMessage = null;
            //var result = EditContext.GetValidationMessages().FirstOrDefault();
        }

        void ValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            //var result = EditContext.GetValidationMessages();
            //checkValidation = true;
            //if (!result.Any() && FormAppState.AllControlsIsValid)
            //{
            //    ErrorMessage = null;
            //    bool isValidSubmit = true;
            //    var form = this as ICaspianForm;
            //    if (!form.IgnoreOnValidSubmit)
            //    {
            //        if (OnValidSubmit != null)
            //            isValidSubmit = await OnValidSubmit.Invoke(EditContext);
            //        if (isValidSubmit && OnInternalValidSubmit.HasDelegate)
            //            await OnInternalValidSubmit.InvokeAsync(EditContext);
            //    }
            //    form.IgnoreOnValidSubmit = false;
            //}
            //else
            //{
            //    if (FormAppState.AllControlsIsValid)
            //    {
            //        var result1 = EditContext.GetValidationMessages();
            //        ErrorMessage = result1.FirstOrDefault();
            //    }
            //    if (OnInvalidSubmit.HasDelegate)
            //        await OnInvalidSubmit.InvokeAsync(EditContext);
            //    if (OnInternalInvalidSubmit.HasDelegate)
            //        await OnInternalInvalidSubmit.InvokeAsync(EditContext);
            //}
        }

        protected override void OnParametersSet()
        {
            if (EditContext == null || !Model.Equals(oldModel))
            {
                EditContext = new EditContext(Model);
                oldModel = Model;
            }
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (ErrorMessage.HasValue())
            {
                var message = ErrorMessage;
                ErrorMessage = null;
                await jsRuntime.InvokeVoidAsync("$.caspian.showMessage", message);
            }
            else if (FormAppState.Element.HasValue)
                await jsRuntime.InvokeVoidAsync("$.caspian.focusAndShowErrorMessage", FormAppState.Element);
            FormAppState.Element = null;
            FormAppState.ErrorMessage = null;
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task ResetAsync()
        {
            await ResetFormAsync();
        }

        async void OnFormSubmit()
        {
            ErrorMessage = null;
            await OnSubmit.InvokeAsync(EditContext);
            EditContext.Validate();
        }

    }
}
