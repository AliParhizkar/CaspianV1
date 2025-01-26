using Microsoft.AspNetCore.Components.Web;

namespace Caspian.UI
{
    public class NodeView
    {
        public NodeView()
        {

        }

        public NodeView(string value, string text, bool collabsable = true, bool selectable = false)
        {
            Text = text;
            Value = value;
            Collabsable = collabsable;
            Selectable = selectable;
        }

        public string Text { get; set; }

        public string Value { get; set; }

        public bool? Selected { get; set; } = false;

        public bool Expanded { get; set; }

        public bool Selectable { get; set; }

        public bool Disabled { get; set; }

        public bool Collabsable { get; set; } = true;

        public bool SingleSelect { get; set; }

        public byte? Depth { get; set; }

        public NodeView Parent { get; set; }

        public IList<NodeView> Children { get; set; }
    }

    public class NodeMouseEventArg
    {
        public NodeMouseEventArg(MouseEventArgs mouseEventArgs, NodeView nodeView) 
        {
            NodeView = nodeView;
            MouseEventArgs = mouseEventArgs;
        }

        public MouseEventArgs MouseEventArgs { get; set; }
        public NodeView NodeView{ get; set; }
    }
}
