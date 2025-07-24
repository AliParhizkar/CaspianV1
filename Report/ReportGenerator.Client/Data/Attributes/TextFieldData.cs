namespace Caspian.Report.Data
{
    public class TextFieldData
    {
        //public string Text { get; set; }

        //public Color BackGroundColor { get; set; }

        /// <summary>
        /// Object Path for example Product.ProductCategory.Title
        /// </summary>
        public string Path { get; set; }

        public TotalFuncType? TotalFuncType { get; set; }

        public SystemVariable? SystemVariable { get; set; }

        public SystemFiledType? SystemFiledType { get; set; }
    }
}