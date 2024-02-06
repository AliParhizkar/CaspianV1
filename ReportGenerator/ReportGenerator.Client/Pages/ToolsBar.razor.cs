using Caspian.UI;
using Caspian.Common;
using Caspian.Report.Data;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class ToolsBar: ComponentBase
    {
        bool isSettingWindow;
        string title;
        
        Font font;
        WindowStatus status;
        Alignment alignment;
        Border border;
        Color BackGroundColor;

        DropdownIcon borderStyle, borderWidth;

        protected override void OnParametersSet()
        {
            alignment = Page.SelectedControl?.Data.Alignment ?? Page.SelectedTable?.Alignment;
            font = Page.SelectedControl?.Data.Font ?? Page.SelectedTable?.Font;
            border = Page.SelectedControl?.Data.Border ?? Page.SelectedTable?.Border;
            BackGroundColor = Page.SelectedControl?.Data.BackgroundColor ?? Page.SelectedTable?.BackgroundColor;
            base.OnParametersSet();
        }

        void CloseWindow(Caspian.UI.WindowStatus status)
        {
            Page.WindowIsOpened = status == UI.WindowStatus.Open;
            this.status = status;
        }

        void OpenSettingWindow()
        {
            Page.WindowIsOpened = true;
            isSettingWindow = true;
            status = Caspian.UI.WindowStatus.Open;
            title = "Page Setting";
        }

        void OpenFormatingWindow()
        {
            Page.WindowIsOpened = true;
            isSettingWindow = false;
            status = Caspian.UI.WindowStatus.Open;
            title = "Number Formating";
        }

        [Parameter]
        public Page Page { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        async Task ChangeStyle()
        {
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync();
        }

        async Task ChangeFont(bool? underline, bool? bold, bool? italic)
        {
            Page.PushControl();
            if (underline.HasValue)
                font.UnderLine = underline.Value;
            if (bold.HasValue)
                font.Bold = bold.Value;
            if (italic.HasValue)
                font.Italic = italic.Value;
            if (Page.SelectedTable != null)
                Page.SelectedTable.Font = font;
            await ChangeStyle();
        }

        async Task ChangeFont(int? size, string family, string color)
        {
            if (size.HasValue)
                font.Size = size.Value;
            if (family != null)
                font.Family = family;
            if (color != null)
                font.Color = new Color(color);
            if (Page.SelectedTable != null)
                Page.SelectedTable.Font = font;
            await ChangeStyle();
        }

        async Task ChangeVerticalAlignment(VerticalAlign align)
        {
            Page.PushControl();
            alignment.VerticalAlign = align;
            if (Page.SelectedTable != null)
                Page.SelectedTable.Alignment = alignment;
            await ChangeStyle();
        }

        async Task ChangeBorder(BorderStyle style)
        {
            Page.PushControl();
            border.BorderStyle = style;
            if (Page.SelectedTable != null)
                Page.SelectedTable.Border = border;
            await ChangeStyle();
        }

        async Task ChangeBorder(int width)
        {
            Page.PushControl();
            border.Width = width;
            if (Page.SelectedTable != null)
                Page.SelectedTable.Border = border;
            await ChangeStyle();
        }

        async Task ChangeBorder(string color)
        {
            border.Color = new Color(color);
            if (Page.SelectedTable != null)
                Page.SelectedTable.Border = border;
            await ChangeStyle();
        }

        async Task ChangeBackgroundColor(string color)
        {
            BackGroundColor.ColorString = color;
            await ChangeStyle();
        }

        async Task ChangeBorder(BorderKind type)
        {
            Page.PushControl();
            if (type == 0 || type.ConvertToInt() == 15)
                border.BorderKind = type;
            else if ((border.BorderKind & type) == type)
                border.BorderKind = (BorderKind)(border.BorderKind - type);
            else
                border.BorderKind |= type;
            if (Page.SelectedTable != null)
                Page.SelectedTable.Border = border;
            await ChangeStyle();
        }

        async Task ChangeHorizontalAlignment(HorizontalAlign align)
        {
            Page.PushControl();
            alignment.HorizontalAlign = align;
            if (Page.SelectedTable != null)
                Page.SelectedTable.Alignment = alignment;
            await ChangeStyle();
        }

        public async Task CloseDropdown()
        {
            await borderStyle.Close();
            await borderWidth.Close();
        }
    }
}
