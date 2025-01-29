using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Caspian.UI;

[EventHandler("onEkeydown", typeof(EKeyboardEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("onEkeypress", typeof(EKeyboardEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("onEkeyup", typeof(EKeyboardEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
    // This static class doesn't need to contain any members. It's just a place where we can put
    // [EventHandler] attributes to configure event types on the Razor compiler. This affects the
    // compiler output as well as code completions in the editor.
}


public class EKeyboardEventArgs : KeyboardEventArgs
{
    public int SelectionStart {  get; set; }

    public int SelectionEnd { get; set; }
}

