using System;
using Microsoft.JSInterop;
using System.Threading.Tasks;

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

        [JSInvokable]
        public async Task SetDateValue(DateTime? date)
        {
            await stringTextBox.SetValue(date);
        }


    }
}
