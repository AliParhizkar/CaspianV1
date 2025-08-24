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
        public ValueTypeContainer(string path)
        {
            PropertyPath = path;
        }

        public string PropertyPath { get; set; }

        public object From { get; set; }

        public object To { get; set; }

        public bool? Value { get; set; }
    }
}
