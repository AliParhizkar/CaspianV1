using Caspian.Common;
using FluentValidation;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using FluentValidation.Results;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;

namespace Caspian.UI
{
    public class CaspianValidationValidator<TModel>: ComponentBase, IDisposable, IControlFocuseValidation where TModel : class
    {
        [Inject]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public FormAppState FormAppState { get; set; }

        [Inject]
        public BatchServiceData BatchServiceData { get; set; }

        [Inject]
        public CaspianDataService CaspianDataService { get; set; }

        [Parameter]
        public IList<TModel> Source { get; set; }

        [CascadingParameter]
        private EditContext EditContext { get; set; }

        [CascadingParameter(Name = "ParentForm")]
        private ICaspianForm<TModel> CaspianForm { get; set; }

        [Parameter]
        public Type ValidatorType { get; set; }

        [Parameter]
        public bool OnlyValidateOnSubmit { get; set; } = true;

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        public string MasterIdName { get; set; }

        IValidator<TModel> Validator;
        ValidationMessageStore ValidationMessageStore;

        public bool IsFirstInvalidControl { get; set; }

        private void HookUpEditContextEvents()
        {
            EditContext.OnValidationRequested -= ValidationRequested;
            EditContext.OnValidationRequested += ValidationRequested;
            EditContext.OnFieldChanged -= FieldChanged;
            EditContext.OnFieldChanged += FieldChanged;
        }

        async void FieldChanged(object sender, FieldChangedEventArgs args)
        {
            //ValidationMessageStore.Clear();
            using var scope = ServiceScopeFactory.CreateScope();
            if (CaspianDataService != null)
            {
                var dataService = scope.GetService<CaspianDataService>();
                dataService.UserId = CaspianDataService.UserId;
                dataService.Language = CaspianDataService.Language;
            }
            var validator = (CaspianValidator<TModel>)Activator.CreateInstance(ValidatorType, scope.ServiceProvider);
            
            var validationResult = validator.ValidateAsync(EditContext.Model as TModel, option => 
            {
                option.IncludeProperties(args.FieldIdentifier.FieldName);
            });
            EditContext.Properties["FieldChangeAsyncTask"] = validationResult;
            var result = await validationResult;
            if (result.IsValid)
            {
                EditContext.RemoveFieldState(args.FieldIdentifier.FieldName);
                EditContext.Properties["PropertyName"] = args.FieldIdentifier.FieldName;
            }
            else
            {
                var error = result.Errors.First();
                var fieldIdentifier = EditContext.Field(error.PropertyName);
                EditContext.Properties["PropertyName"] = error.PropertyName;
                ValidationMessageStore.Clear();
                ValidationMessageStore.Add(fieldIdentifier, error.ErrorMessage);
                var qqq = EditContext.GetValidationMessages(fieldIdentifier).First();
            }
            EditContext.Properties["ValidationType"] = "FieldChanged";
            EditContext.NotifyValidationStateChanged();
        }

        async void ValidationRequested(object sender, ValidationRequestedEventArgs args)
        {
            ValidationMessageStore.Clear();
            var context = new ValidationContext<object>(EditContext.Model);
            using var scope = ServiceScopeFactory.CreateScope();
            if (CaspianDataService != null)
            {
                var dataService = scope.GetService<CaspianDataService>();
                dataService.UserId = CaspianDataService.UserId;
                dataService.Language = CaspianDataService.Language;
            }
            Validator = (IValidator<TModel>)Activator.CreateInstance(ValidatorType, scope.ServiceProvider);
            (Validator as ICaspianValidator).BatchServiceData = BatchServiceData;
            if (Source != null && Source.Count() > 0)
                (Validator as IBaseService<TModel>).SetSource(Source.AsReadOnly());
            Task<ValidationResult> asyncValidationTask;
            if (EditContext.Properties.TryGetValue("DetailType", out var objeDetail) && objeDetail != null)
                asyncValidationTask = Validator.ValidateAsync((TModel)EditContext.Model, (Type)objeDetail);
            else
                asyncValidationTask = Validator.ValidateAsync(context);
            EditContext.Properties["AsyncValidationTask"] = asyncValidationTask;
            var result = await asyncValidationTask;
            EditContext.Properties["ValidationType"] = "FormSubmitted";
            AddValidationResult(EditContext.Model, result);
        }

        void AddValidationResult(object model, ValidationResult validationResult)
        {
            foreach (ValidationFailure error in validationResult.Errors)
            {
                var fieldIdentifier = new FieldIdentifier(model, error.PropertyName);
                ValidationMessageStore.Add(fieldIdentifier, error.ErrorMessage);
            }
            EditContext.NotifyValidationStateChanged();
        }

        private void EditContextChanged()
        {
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            HookUpEditContextEvents();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            //Keep a reference to the original values so we can check if they have changed
            var previousEditContext = EditContext;
            //MasterIdName = CaspianForm.MasterIdName;
            var previousValidatorType = ValidatorType;
            await base.SetParametersAsync(parameters);
            if (EditContext == null)
                throw new NullReferenceException($"{nameof(CaspianValidationValidator<TModel>)} must be placed within an CaspianForm");
            if (ValidatorType == null)
                throw new NullReferenceException($"{nameof(ValidatorType)} must be specified.");
            if (!typeof(IValidator).IsAssignableFrom(ValidatorType))
                throw new ArgumentException($"{ValidatorType.Name} must implement {typeof(IValidator).FullName}");
            //if (ValidatorType != previousValidatorType)
            //    ValidatorTypeChanged();
            // If the EditForm.Model changes then we get a new EditContext
            // and need to hook it up
            if (EditContext != previousEditContext)
            {
                EditContextChanged();
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (CaspianForm != null)
            {
                CaspianForm.ValidationValidator = this;
                MasterIdName = CaspianForm.MasterIdName;
            }
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ///For Master Details form this code is not working
            ///We Use Caspian Form instate of this
            //if (FormAppState.ValidationChecking)
            //{
            //    var control = CaspianForm?.GetFirstInvalidControl();
            //    if (control != null)
            //    {
            //        FormAppState.ValidationChecking = false;
            //        await control.FocusAsync();
            //    }
            //    else if (FormAppState.ErrorMessage !=  null)
            //    {
            //        await JSRuntime.InvokeVoidAsync("caspian.common.showMessage", FormAppState.ErrorMessage);
            //        FormAppState.ErrorMessage = null;
            //    }
            //}
            if (CaspianForm == null && FormAppState.AllControlsIsValid)
            {
                if (FormAppState.Control?.InputElement == null)
                {
                    if (FormAppState.ErrorMessage.HasValue())
                    {
                        await JSRuntime.InvokeVoidAsync("caspian.common.showMessage", FormAppState.ErrorMessage);
                        FormAppState.ErrorMessage = null;
                    }
                }
                else
                    await FormAppState.Control.FocusAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        void IDisposable.Dispose()
        {

        }
    }
}
