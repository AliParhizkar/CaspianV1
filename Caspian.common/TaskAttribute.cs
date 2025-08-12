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

    internal class ValueTypeContainer
    {
        public string propertyPath { get; set; }

        public object From { get; set; }

        public object To { get; set; }
    }
}
