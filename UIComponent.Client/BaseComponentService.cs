using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public class BaseComponentService
    {
        public MessageBox MessageBox { get; set; }

        public ComponentBase Target { get; set; }
    }
}
