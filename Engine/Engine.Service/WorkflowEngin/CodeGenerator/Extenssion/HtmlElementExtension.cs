using System.Text;

namespace Caspian.Engine.Service
{
    public static class HtmlElementExtension
    {
        public static string GetCode(this HtmlRow row, WorkflowForm form, string userCode)
        {
            var str = new StringBuilder();
            str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"row\");\n");
            foreach (var col in row.Columns)
                str.Append(col.GetCode(form, userCode));
            str.Append("\t\t\tbuilder.CloseElement();\n");
            return str.ToString();
        }

        public static string GetCode(this HtmlColumn col, WorkflowForm form, string userCode)
        {
            var str = new StringBuilder();
            str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
            str.Append($"\t\t\tbuilder.AddAttribute(3, \"class\", \"col-md-{col.Span}\");\n");
            if (col.Component != null)
                str.Append(col.Component.GetCode(form.Name, form.WorkflowGroup.SubSystemKind, userCode));
            else if (col.InnerRows != null)
            {
                foreach (var row in col.InnerRows)
                    str.Append(row.GetCode(form, userCode));
            }
            str.Append("\t\t\tbuilder.CloseElement();\n");
            return str.ToString();
        }

        public static string GetCode(this InnerRow row, WorkflowForm form, string userCode)
        {
            var str = new StringBuilder();
            str.Append("\t\t\tbuilder.OpenElement(1, \"div\");\n");
            str.Append("\t\t\tbuilder.AddAttribute(3, \"class\", \"row\");\n");
            foreach (var col in row.HtmlColumns)
                str.Append(col.GetCode(form, userCode));
            str.Append("\t\t\tbuilder.CloseElement();\n");
            return str.ToString();
        }
    }
}
