using System.Collections.Generic;

namespace Caspian.UI
{
    public class BasePageService
    {
        Stack<Window> windows = new Stack<Window>();

        public void Push(Window window)
        { 
            windows.Push(window); 
        }

        public void Pop() 
        {
            windows.Pop();
        }

        public Window Peek()
        {
            if (windows.Count == 0) 
                return null;
            return windows.Peek();
        }
    }
}
