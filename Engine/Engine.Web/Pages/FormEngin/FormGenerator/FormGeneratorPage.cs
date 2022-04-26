using System;
using System.IO;
using Caspian.UI;
using System.Linq;
using Caspian.Common;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.JSInterop;
using Page.FormEngine.Models;
using Caspian.Engine.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.Engine.FormGenerator
{
    public partial class FormGeneratorPage
    {
        bool isUpdated;
        string message1;
        FormModel model;
        WindowStatus status;
        FormControl formControl;
        FormWindowType? windowType;
        bool firstRender;

        protected async override Task OnInitializedAsync()
        {
            firstRender = true;
            using var scope = ServiceScopeFactory.CreateScope();
            var form = await new FormService(scope).SingleAsync(FormId);
            model = new FormModel();
            var path = Assembly.GetExecutingAssembly().GetMapPath() + "/Data/Form/" + form.FileName + ".xml";
            if (form.FileName.HasValue())
            {
                var element = XElement.Load(path);
                model = new FormModel(element);
            }
            else
            {
                model.Bond = new FormBond()
                {
                    Width = 20,
                    Height = 12,
                    BackGroundColor = new ReportUiModels.Color()
                };
            }
            model.Id = FormId;
            await base.OnInitializedAsync();
        }

        async Task<bool> ParametrHasData(int parameterId)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var form = await new FormService(scope).SingleAsync(FormId);
            var formGroup = form.FormGroup;
            var type = new AssemblyInfo().GetModelType(formGroup.Subsystem, formGroup.ClassName);
            if (type == null)
                throw new MyException("خطا: In namespace " + formGroup.Namespace + " type with name " + formGroup.ClassName + " not exists");
            var attr = type.GetCustomAttribute<DynamicFieldAttribute>();
            if (attr == null)
                throw new MyException("خطا: Type" + type.Name + " must have DynamicFieldAttribute ");
            var dynamicValueType = attr.ParameterValueType;
            if (dynamicValueType.BaseType != typeof(BaseDynamicParameterData))
                throw new MyException("خطا: Type " + dynamicValueType.Name + " must impliment type BaseDynamicParameterData");
            var type1 = typeof(DbSet<>).MakeGenericType(dynamicValueType);
            PropertyInfo paramsValueInfo = null;
            var contextType = new AssemblyInfo().GetDbContextType(dynamicValueType);
            var context = scope.ServiceProvider.GetService(contextType);
            foreach (var info in contextType.GetProperties())
            {
                if (info.PropertyType == type1)
                    paramsValueInfo = info;
            }
            if (paramsValueInfo == null)
                throw new Exception("خطا: Please add a property for type " + dynamicValueType.Name + " to dbcontext");
            return await (paramsValueInfo.GetValue(context) as IQueryable<BaseDynamicParameterData>).AnyAsync(t => t.DynamicParameterId == parameterId);
        }

        async Task RemoveDeletedParameters(IEnumerable<FormControl> controls)
        {
            var ids = controls.Where(t => t.Id.GetValueOrDefault() > 0).Select(t => t.Id.Value);
            using var context = new Context();
            var deletedControls = await context.DynamicParameters.Where(t => t.FormId == FormId && !ids.Contains(t.Id)).ToListAsync();
            foreach(var ctr in deletedControls)
            {
                var result = await ParametrHasData(ctr.Id);
                if (result)
                    throw new MyException("کنترل " + ctr.Title + " مورد استفاده قرار گرفته و امکان حذف آن وجود ندارد");
            }
            context.DynamicParameters.RemoveRange(deletedControls);
            ids = deletedControls.Select(t => t.Id).ToList();
            var parametersValues = await context.DynamicParametersValues.Where(t => ids.Contains(t.DynamicParameterId)).ToListAsync();
            context.DynamicParametersValues.RemoveRange(parametersValues);
        }

        async Task SaveData(FormModel model)
        {
            if (model.Bond.Controls == null)
                throw new MyException("فرم فاقد کنترل می باشد");
            foreach (var control in model.Bond.Controls)
            {
                var ctrType = control.FormControlType;
                if (ctrType == FormControlType.CheckListBox || ctrType == FormControlType.DropdownList)
                    if (control.Items == null || control.Items.Count == 0)
                        throw new MyException("لطفا عناصر چک لیست باکس یا لیست فرو افتادنی را مشخص نمایئد");
                if (control.Text.HasValue())
                    control.Text = System.Uri.UnescapeDataString(control.Text);
                else if (control.FormControlType != FormControlType.Lable && control.FormControlType != FormControlType.Panel)
                    throw new MyException("لطفا غنوان کنترل را مشخص نمایید");
                if (control.FormControlType == FormControlType.TextBox && control.InputControlType == null)
                    throw new MyException("لطفا نوع کنترل " + control.Text + " را مشخص نمایید");
            }
            ///ثبت یا بروزرسانی پارامترها
            var parameterControls = model.Bond.Controls.Where(t => t.FormControlType != FormControlType.Panel &&
                t.FormControlType != FormControlType.Lable);
            using var scope = CreateScope();
            var maxId = (await new DynamicParameterService(scope).GetAll().MaxAsync(t => (int?)t.Id)).GetValueOrDefault() + 1;
            using var context = new Context();
            foreach (var ctr in parameterControls)
            {
                if (parameterControls.Count(t => t.Text == ctr.Text) > 1)
                    throw new MyException("عنوان " + ctr.Text + " به بیش از یک کنترل اختصاص داده شده است");
                var old = await context.DynamicParameters.SingleOrDefaultAsync(t => t.FormId == FormId && t.Title == ctr.Text);
                if (old == null && ctr.Id.GetValueOrDefault() > 0)
                    old = await context.DynamicParameters.SingleOrDefaultAsync(t => t.Id == ctr.Id.Value);
                if (old == null)
                {
                    ctr.Id = maxId;
                    context.DynamicParameters.Add(new DynamicParameter()
                    {
                        FormId = FormId,
                        Id = maxId,
                        Title = ctr.Text,
                        FormControlType = ctr.FormControlType.Value,
                        InputControlType = ctr.InputControlType
                    });

                    if (ctr.Items != null && ctr.Items.Count > 1)
                    {
                        var list = new List<DynamicParameterValue>();
                        var maxId1 = (await context.DynamicParametersValues.MaxAsync(t => (int?)t.Id)).GetValueOrDefault() + 1;
                        foreach(var item in ctr.Items)
                        {
                            list.Add(new DynamicParameterValue()
                            {
                                Id = maxId1,
                                DynamicParameterId = maxId,
                                Value = item
                            }) ;
                            maxId1++;
                        }
                        await context.DynamicParametersValues.AddRangeAsync(list);
                        await context.SaveChangesAsync();
                    }
                    maxId++;
                }
                else
                {
                    ctr.Id = old.Id;
                    if (old.FormControlType != ctr.FormControlType || old.InputControlType != ctr.InputControlType)
                    {
                        var flag = old.FormControlType == ctr.FormControlType && ctr.FormControlType == FormControlType.TextBox && old.InputControlType == InputControlType.Integer && ctr.InputControlType == InputControlType.Numeric;
                        if (!flag && await ParametrHasData(old.Id))
                            throw new MyException("کنترل " + old.Title + " مورد استفاده قرار گرفته و امکان تغییر ماهیت کنترل وجود ندارد" );
                        old.FormControlType = ctr.FormControlType.Value;
                        old.InputControlType = ctr.InputControlType;
                        var oldItems = await context.DynamicParametersValues.Where(t => t.DynamicParameterId == old.Id).ToListAsync();
                        context.DynamicParametersValues.RemoveRange(oldItems);

                    }
                }
                await context.SaveChangesAsync();
            }
            await RemoveDeletedParameters(parameterControls);
            var doc = model.GetXmlElement();
            var formService = new FormService(scope);
            var form = await formService.SingleAsync(FormId);
            var fileName = form.FileName;
            if (!fileName.HasValue())
                fileName = Path.GetRandomFileName().Replace(".", "");
            var path = Assembly.GetExecutingAssembly().GetMapPath();
            path += "/Data/Form/";
            doc.Save(path + fileName + ".xml");
            form.FileName = fileName;
            isUpdated = true;
            await formService.SaveChangesAsync();
        }

        [JSInvokable]
        public async Task Save(FormModel model)
        {
            using var context = new Context();
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await SaveData(model);
                await transaction.CommitAsync();
            }
            catch(MyException ex)
            {
                message1 = ex.Message;
                await transaction.RollbackAsync();
            }
            StateHasChanged();
        }

        protected async override Task OnAfterRenderAsync(bool f)
        {
            if (firstRender && model != null)
            {
                firstRender = false;
                var json = model.GetJson();
                await jsRuntime.InvokeVoidAsync("$.form.formBind", DotNetObjectReference.Create(this), json);
            }
            if (message1.HasValue())
            {
                await jsRuntime.InvokeVoidAsync("$.telerik.showMessage", message1);
                message1 = null;
            }
            if (isUpdated)
            {
                await jsRuntime.InvokeVoidAsync("$.telerik.showMessage", "ثبت با موفقیت انجام شد");
                isUpdated = false;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void ShowLableWindow(FormControl model)
        {
            if (model.SpecialField.HasValue)
                model.Text = "{" + model.SpecialField.FaText() + "}";
            if (model.Text.HasValue())
                model.Text = System.Uri.UnescapeDataString(model.Text);
            formControl = model;
            status = WindowStatus.Open;
            windowType = FormWindowType.Lable;
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowTextboxWindow(FormControl control)
        {
            if (control.Text.HasValue())
                control.Text = System.Uri.UnescapeDataString(control.Text);
            formControl = control;
            status = WindowStatus.Open;
            windowType = FormWindowType.Textbox;
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowListWindow(FormControl control)
        {
            if (control.Text.HasValue())
                control.Text = System.Uri.UnescapeDataString(control.Text);
            formControl = control;
            status = WindowStatus.Open;
            windowType = FormWindowType.List;
            StateHasChanged();
        }

        [Parameter]
        public int FormId { get; set; }
    }

    public enum FormWindowType
    {
        Lable = 1,

        Textbox,

        List
    }
}
