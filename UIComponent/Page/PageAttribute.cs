namespace Caspian.UI
{
    public class SourceAttribute : Attribute
    {
        public SourceAttribute(int id, string title = null)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; private set; }

        public string Title { get; private set; }
    }
}
