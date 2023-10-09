using System;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public class TreeViewCascadeData
    {
        public RenderFragment<NodeView> AfterNodeTemplate { get; set; }
    }
}
