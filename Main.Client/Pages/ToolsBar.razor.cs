using Caspian.Client.Data;
using Caspian.Client.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.Client
{
    public partial class ToolsBar
    {
        bool isSettingWindow;
        string title;
        Font font;
        Alignment alignment;
        Border border;
        Color BackGroundColor;

        DropdownIcon borderStyle, borderWidth;
        ComboboxComponent cmbFontSize, cmbFontFamily;
        string[] fontsSize;

        //ReportGenerator.CollorPicker cpkColor, cpkBorderColor, cpkBackGroundColor;

        protected override async Task OnInitializedAsync()
        {
            //fontsName = await Host.GetFromJsonAsync<string[]>("/ReportGenerator/GetFonts");
            fontsSize = new string[] { "8", "9", "10", "11", "12", "13", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" };
            await base.OnInitializedAsync();
        }

        public void ChangeColor(Color color, Color borderColor, Color backgroundColor)
        {
            //cpkColor.ChangeColorByControlSelecting(color.ColorString);
            //cpkBorderColor.ChangeColorByControlSelecting(borderColor.ColorString);
            //cpkBackGroundColor.ChangeColorByControlSelecting(backgroundColor.ColorString);
        }

        [Parameter]
        public string[] FontsName { get; set; }

        public string GetDefaultFont()
        {
            return FontsName.FirstOrDefault();
        }

        protected override void OnParametersSet()
        {
            alignment = Page.SelectedControl?.Data.Alignment ?? Page.SelectedTable?.Alignment;
            font = Page.SelectedControl?.Data.Font ?? Page.SelectedTable?.Font;
            border = Page.SelectedControl?.Data.Border ?? Page.SelectedTable?.Border ?? Page.SelectedBound?.Data.Border;
            BackGroundColor = Page.SelectedControl?.Data.BackgroundColor ?? Page.SelectedTable?.BackgroundColor ?? Page.SelectedBound?.Data.BackgroundColor;
            base.OnParametersSet();
        }

        //void CloseWindow(Caspian.UI.WindowStatus status)
        //{
        //    Page.WindowIsOpened = status == UI.WindowStatus.Open;
        //    this.status = status;
        //}

        async Task OpenSettingWindow()
        {
            Page.WindowIsOpened = true;
            isSettingWindow = true;
            //status = Caspian.UI.WindowStatus.Open;
            title = "Page Setting";
            await Page.OnReportSetting.InvokeAsync(null);
        }

        void OpenFormatingWindow()
        {
            Page.WindowIsOpened = true;
            isSettingWindow = false;
            //status = Caspian.UI.WindowStatus.Open;
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

        async Task ChangeFont(string size, string family, string color)
        {
            Page.PushControl();
            if (size.HasValue())
                font.Size = size;
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
            Page.PushControl();
            border.Color = new Color(color);
            if (Page.SelectedTable != null)
                Page.SelectedTable.Border = border;
            await ChangeStyle();
        }

        async Task ChangeBackgroundColor(string color)
        {
            Page.PushControl();
            if (Page.SelectedTable != null)
                Page.SelectedTable.BackgroundColor = new Color() { ColorString = color };
            else
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

        public void CloseDropdown()
        {
            borderStyle.Close();
            borderWidth.Close();
            cmbFontSize.Close();
            cmbFontFamily.Close();
        }
    }
}
