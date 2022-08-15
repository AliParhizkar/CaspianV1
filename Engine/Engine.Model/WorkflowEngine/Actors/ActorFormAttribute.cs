using System;

namespace Caspian.Engine
{
    public class ActorFormAttribute: Attribute 
    {
        public string Url { get; set; }
        public ActorFormAttribute(string url)
        {
            Url = url;
        }
    }

    public class WorkflowEntityAttribute: Attribute
    {
        public string Title { get; private set; }

        public WorkflowEntityAttribute(string title)
        {
            Title = title;
        }
    }
}
