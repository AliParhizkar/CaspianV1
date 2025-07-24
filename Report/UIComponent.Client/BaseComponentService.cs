using Microsoft.AspNetCore.Components;

namespace Caspian.UI.Client
{
    public class BaseComponentService
    {
        public MessageBox MessageBox { get; set; }

        public ComponentBase Target { get; set; }
    }
}
