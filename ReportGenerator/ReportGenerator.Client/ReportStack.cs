using Caspian.Report;
using System.Text.Json;
using Caspian.Report.Data;
using ReportGenerator.Client.Data;

namespace ReportGenerator.Client
{
    public class ReportStack
    {
        Stack<StackData> stack;
        Stack<StackData> tempStack;
        public ReportStack()
        {
            stack = new Stack<StackData>();
            tempStack = new Stack<StackData>();
        }

        public void Push(BoundItem bound)
        {
            if (bound != null)
            {
                tempStack.Clear();
                if (stack.Count > 50)
                    stack = new Stack<StackData>(stack.Where((t, index) => index < 50));
                var json = JsonSerializer.Serialize(bound.Data);
                var data = JsonSerializer.Deserialize<BoundItemData>(json);
                data.Table = null;
                data.Controls = null;
                stack.Push(new StackData()
                {
                    BoundItem = data
                });
            }
        }

        public void Push(ReportControl control)
        {
            if (control != null)
            {
                tempStack.Clear();
                if (stack.Count > 50)
                    stack = new Stack<StackData>(stack.Where((t, index) => index <= 50));
                var json = JsonSerializer.Serialize(control.Data);
                stack.Push(new StackData()
                {
                    Id = control.Data.Id,
                    Control = JsonSerializer.Deserialize<ControlData>(json),
                });
            }
        }

        public void Push(string addedControlId)
        {
            stack.Push(new StackData()
            {
                Id = addedControlId
            });
        }


        //public void Push(StackData data)
        //{
        //    stack.Push(data);
        //}

        //public void Pop()
        //{
        //    stack.Pop();
        //}

        public string GetIdByControlData(ControlData controlData)
        {
            return stack.FirstOrDefault(t => t.Control == controlData)?.Id;
        }

        public StackData Undo()
        {
            var data = stack.Pop();
            tempStack.Push(data);
            return data;
        }

        public StackData Redo()
        {
            var data = tempStack.Pop();
            stack.Push(data);
            return data;
        }

        public bool CanUndo => stack.Count > 0;

        public bool CanRedo => tempStack.Count > 0;
    }
}
