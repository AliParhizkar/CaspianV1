using System;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public class TreeViewCascadeData
    {
        public RenderFragment<TreeViewItem> AfterNodeTemplate { get; set; }
    }
}
