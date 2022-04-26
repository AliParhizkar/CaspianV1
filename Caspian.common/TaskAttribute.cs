using System;

namespace Caspian.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TaskAttribute : Attribute
    {
        public string Title { get; set; }

        public TaskAttribute(string title)
        {
            Title = title; 
        }
    }
}
