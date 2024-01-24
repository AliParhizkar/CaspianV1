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
        WindowStatus status;

        DropdownIcon borderStyle, borderWidth;

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
        public int ReportId { get; set; }

        [Parameter]
        public Alignment Alignment { get; set; }

        [Parameter]
        public Font Font { get; set; }

        [Parameter]
        public Border Border { get; set; }

        [Parameter]
        public Page Page { get; set; }

        [Parameter]
        public Table SelectedTable { get; set; }

        [Parameter]
        public EventCallback<string> OnChange { get; set; }

        async Task ChangeStyle()
        {
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync(Alignment.Style);
        }

        async Task ChangeFont(bool? underline, bool? bold, bool? italic)
        {
            if (underline.HasValue)
                Font.UnderLine = underline.Value;
            if (bold.HasValue)
                Font.Bold = bold.Value;
            if (italic.HasValue)
                Font.Italic = italic.Value;
            await ChangeStyle();
        }

        async Task ChangeFont(int? size, string family, string color)
        {
            if (size.HasValue)
                Font.Size = size.Value;
            if (family != null)
                Font.Family = family;
            if (color != null)
                Font.Color = new Color(color);
            await ChangeStyle();
        }

        async Task ChangeVerticalAlignment(VerticalAlign align)
        {
            Alignment.VerticalAlign = align;
            await ChangeStyle();
        }

        async Task ChangeBorder(BorderStyle style)
        {
            Border.BorderStyle = style;
            await ChangeStyle();
        }

        async Task ChangeBorder(int width)
        {
            Border.Width = width;
            await ChangeStyle();
        }

        async Task ChangeBorder(string color)
        {
            Border.Color = new Color(color);
            await ChangeStyle();
        }

        async Task ChangeBorder(BorderKind type)
        {
            if (type == 0 || type.ConvertToInt() == 15)
                Border.BorderKind = type;
            else if ((Border.BorderKind & type) == type)
                Border.BorderKind = (BorderKind)(Border.BorderKind - type);
            else
                Border.BorderKind |= type;
            await ChangeStyle();
        }

        async Task ChangeHorizontalAlignment(HorizontalAlign align)
        {
            Alignment.HorizontalAlign = align;
            await ChangeStyle();
        }

        public async Task CloseDropdown()
        {
            await borderStyle.Close();
            await borderWidth.Close();
        }
    }
}
