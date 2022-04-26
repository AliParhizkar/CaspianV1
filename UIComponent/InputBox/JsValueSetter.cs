using System;
using Microsoft.JSInterop;
using Caspian.Common.Extension;
using Caspian.Common;
using System.Threading.Tasks;
using System.Linq;

namespace Caspian.UI
{
    public class JsValueSetter
    {
        private readonly IInputValueInitializer stringTextBox;
        private readonly IListValueInitializer listValueInitializer;
        public JsValueSetter(IInputValueInitializer textBox, IListValueInitializer list = null)
        {
            stringTextBox = textBox;
            listValueInitializer = list;
        }

        public string Value { get; set; }

        //[JSInvokable]
        //public async void SetStringValue(string value)
        //{
        //    await stringTextBox.SetValue(value);
        //}

        //[JSInvokable]
        //public async void SetValue(decimal? value)
        //{
        //    var info = stringTextBox.GetType().GetProperty("ValueChanged");
        //    var valueChanged = info.GetValue(stringTextBox);
        //    var method = valueChanged.GetType().GetMethods().First(t => t.Name == "InvokeAsync");
        //    var type = method.GetParameters()[0].ParameterType;
        //    if (value.HasValue || type.IsNullableType())
        //    {
        //        if (value.HasValue && type.IsNullableType())
        //            type = Nullable.GetUnderlyingType(type);
        //        object convertedValue = value;
        //        if (convertedValue != null)
        //        {
        //            if (type.IsEnum)
        //                convertedValue = Enum.Parse(type, value.ToString());
        //            else
        //                convertedValue = Convert.ChangeType(convertedValue, type);
        //        }
        //        await stringTextBox.SetValue(convertedValue);
        //    }
        //    else
        //    {
        //        await stringTextBox.SetValue(Convert.ChangeType(0, type));
        //    }
        //}

        //[JSInvokable]
        //public async void SetBoolValue(bool? value)
        //{
        //    await stringTextBox.SetValue(value);
        //}

        [JSInvokable]
        public void SetEnumValue(int? value)
        {

        }

        //[JSInvokable]
        //public async void SetTextBoxValue(string value)
        //{
        //    await stringTextBox.SetValue(value);
        //}

        [JSInvokable]
        public void SetLookupValue(long value)
        {

        }

        [JSInvokable]
        public async Task SetDateValue(DateTime? date)
        {
            await stringTextBox.SetValue(date);
        }

        [JSInvokable]
        public async Task IncPageNumber()
        {
            await listValueInitializer.IncPageNumber();
        }

        [JSInvokable]
        public void Close()
        {
            listValueInitializer.Close();
        }
    }
}
