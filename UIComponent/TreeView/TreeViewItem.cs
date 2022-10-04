using System.Collections.Generic;

namespace Caspian.UI
{
    public class TreeViewItem
    {
        public string Text { get; set; }

        public string Value { get; set; }

        public bool? Selected { get; set; } = false;

        public bool Expanded { get; set; }

        public bool Selectable { get; set; }

        public bool Disabled { get; set; }

        public bool Collabsable { get; set; } = true;

        public bool SingleSelect { get; set; }

        public bool ShowTemplate { get; set; }

        public byte? Depth { get; set; }

        public TreeViewItem Parent { get; set; }

        public IList<TreeViewItem> Items { get; set; }
    }
}
