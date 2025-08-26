namespace Caspian.UI
{
    public class PageAttribute : Attribute
    {
        public PageAttribute(int id, string title = null)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; private set; }

        public string Title { get; private set; }
    }
}
