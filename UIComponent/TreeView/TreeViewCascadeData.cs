using System;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public class TreeViewCascadeData
    {
        Action<Type , object > LoadData { get; set; }

        public RenderFragment<TreeViewItem> Template { get; set; }
    }
}
