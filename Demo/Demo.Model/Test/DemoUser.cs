using Caspian.Engine.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    public class ChildUser: User
    {
        public IList<Meeting> Meetings { get; set; }
    }
}
