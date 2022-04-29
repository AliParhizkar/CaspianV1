using System;

namespace Caspian.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActorAttribute : Attribute
    {
        public string Title { get; set; }

        public ActorAttribute(string title)
        {
            Title = title;
        }
    }
}
