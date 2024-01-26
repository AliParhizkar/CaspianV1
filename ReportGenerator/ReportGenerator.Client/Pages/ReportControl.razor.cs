using Caspian.Report.Data;
using Microsoft.AspNetCore.Components;

namespace Caspian.Report
{
    public partial class ReportControl: ComponentBase
    {
        double xStart, yStart, widthStart, heightStart, leftStart, topStart;
        ChangeType? changeType;

        [Parameter]
        public ControlData Data { get; set; }

        public double TopStart { get; set; }

        [Parameter]
        public Page Page { get; set; }

        [Parameter]
        public BoundItem BoundItem { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        void SelectControl()
        {
            Page.SelectControl(this);
            Page.Bound.DisableSelection();
        }

        protected override void OnInitialized()
        {
            if (Data.BondType.HasValue)
            {
                Page.SelectControl(this);
                changeType = null;
            }
            base.OnInitialized();
        }

        void OpenWindow()
        {
            Page.OpenTextWindow();
        }

        public string GetCursor(double x, double y)
        {
            double right = Data.Left + Data.Width, bottom = Data.Top + Data.Height;
            if (Math.Abs(y - Data.Top) < 5 && x > Data.Left && x < right)
                return "n-resize";
            if (Math.Abs(y - bottom) < 5 && x > Data.Left && x < right)
                return "s-resize";
            if (Math.Abs(x - Data.Left) < 5 && y > Data.Top && y < bottom)
                return "e-resize";
            if (Math.Abs(x - right) < 5 && y > Data.Top && y < bottom)
                return "e-resize";
            if (x > Data.Left + 5 && x < right - 5 && y > Data.Top + 5 && y < bottom - 5)
                return "move";
            return "default";
        }

        public void DragStart(double x, double y)
        {
            double right = Data.Left + Data.Width, bottom = Data.Top + Data.Height;
            if (Math.Abs(y - Data.Top) < 5 && x > Data.Left && x < right)
                changeType = ChangeType.TopResize;
            else if (Math.Abs(y - bottom) < 5 && x > Data.Left && x < right)
                changeType = ChangeType.BottomResize;
            else if (Math.Abs(x - Data.Left) < 5 && y > Data.Top && y < bottom)
                changeType = ChangeType.LeftResize;
            else if (Math.Abs(x - right) < 5 && y > Data.Top && y < bottom)
                changeType = ChangeType.RightResize;
            else if (x > Data.Left + 5 && x < right - 5 && y > Data.Top + 5 && y < bottom - 5)
                changeType = ChangeType.Move;
            else
                changeType = null;
            if (changeType.HasValue)
            {
                Page.Bound.DisableSelection();
                xStart = x;
                yStart = y;
                leftStart = Data.Left;
                topStart = Data.Top;
                widthStart = Data.Width;
                heightStart = Data.Height;
            }
        }

        public void InitializeBeforAddedToPage()
        {
            changeType = ChangeType.Move;
            Data.BondType = null;
        }

        public void Move(int x, int y)
        {
            Data.Left += x;
            Data.Top += y;

        }

        public void Resize(int width, int height)
        {
            Data.Width += width;
            Data.Height += height;
        }

        public void Drag(double x, double y)
        {
            if (changeType == null)
                return;
            double difX = xStart - x, difY = yStart - y;
            switch (changeType)
            {
                case ChangeType.Move:
                    Data.Left = leftStart - difX;
                    Data.Top = topStart - difY;
                    if (Data.BondType.HasValue)
                    {
                        if (Data.Left < Bound.Left)
                            Data.Left = Bound.Left;
                        if (Data.Top < BoundItem.Top)
                            Data.Top = BoundItem.Top;
                        if (Data.Left + Data.Width > Page.Bound.Right)
                            Data.Left = Page.Bound.Right - Data.Width;
                    }
                    break;
                case ChangeType.BottomResize:
                    Data.Height = heightStart - difY;
                    break;
                case ChangeType.RightResize:
                    Data.Width = widthStart - difX;
                    break;
                case ChangeType.LeftResize:
                    Data.Left = leftStart - difX;
                    Data.Width = widthStart + difX;
                    break;
                case ChangeType.TopResize:
                    Data.Top = topStart - difY;
                    Data.Height = heightStart + difY;
                    break;
            }
            double width = Data.Width, height = Data.Height;
            Page.Bound.ShowRuler(Data, ref width, ref height, changeType.Value);
            Data.Width = width;
            Data.Height = height;

        }
    }
}
