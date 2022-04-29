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
}
