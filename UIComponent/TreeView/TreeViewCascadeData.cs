using System;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public class TreeViewCascadeData
    {
        public Type Type { get; set; }

        public object Value { get; set; }

        public RenderFragment<TreeViewItem> Template { get; set; }
    }
}
