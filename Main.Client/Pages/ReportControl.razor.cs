using Caspian.Client.Data;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace Caspian.Client
{
    public partial class ReportControl
    {
        double xStart, yStart, widthStart, heightStart, leftStart, topStart;
        ChangeType? changeType;
        bool statePushed;

        [Parameter]
        public ControlData Data { get; set; }

        public double TopStart { get; set; }

        [Parameter]
        public Page Page { get; set; }

        [Parameter]
        public BoundItem BoundItem { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        void SelectControl(MouseEventArgs args)
        {
            Page.SelectControl(this, args.AltKey);
            Page.Bound.DisableSelection();
        }

        protected override void OnInitialized()
        {
            if (Data.BondType.HasValue)
            {
                Page.SelectControl(this);
                changeType = null;
                /////If Control added on Undo we need get Id from stack
                //var id = Page.Stack.GetIdByControlData(Data);
                //if (id != null)
                //{
                //    Page.Stack.Pop();
                //    Id = id;
                //}
                //else
                //{
                //    Id = Page.GetId();
                //    Page.Stack.Push(Id);
                //}
            }
            base.OnInitialized();
        }

        void OpenWindow()
        {
            Page.OpenTextWindow();
        }

        public string GetCursor(double x, double y)
        {
            x = x - Bound.Left;
            y = y - BoundItem.Top;
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
            x = x - Bound.Left;
            y = y - BoundItem.Top;
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
            statePushed = false;
        }

        public void InitializeBeforAddedToPage()
        {
            changeType = ChangeType.Move;
            Data.BondType = null;
        }

        public void Move(int x, int y)
        {
            double left = Data.Left + x, top = Data.Top + y, right = left + Data.Width, bottom = top + Data.Height;
            if (left >= Bound.Left && right <= BoundItem.Bound.Right)
                Data.Left += x;
            if (top >= Bound.Top && bottom <= BoundItem.Bottom)
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
            if (!statePushed)
            {
                statePushed = true;
                Page.PushControl();
            }
            if (BoundItem != null)
            {
                x = x - Bound.Left;
                y = y - BoundItem.Top;
            }
            double difX = xStart - x, difY = yStart - y;
            Console.WriteLine("{0},{1}", heightStart - difY, widthStart - difX);
            switch (changeType)
            {
                case ChangeType.Move:
                    Data.Left = leftStart - difX;
                    Data.Top = topStart - difY;
                    if (Data.BondType.HasValue)
                    {
                        if (Data.Left < 0)
                            Data.Left = 0;
                        if (Data.Top < 0)
                            Data.Top = 0;
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
            if (Data.Width < 10)
                Data.Width = 10;
            if (Data.Height < 10)
                Data.Height = 10;
            double width = Data.Width, height = Data.Height;
            Page.Bound.ShowRuler(this, ref width, ref height, changeType.Value);
            Data.Width = width;
            Data.Height = height;
        }
    }
}
