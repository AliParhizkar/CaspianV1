using Caspian.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Model
{
    public class DemoUser: User
    {
        public IList<Meeting> Meetings { get; set; }
    }
}
