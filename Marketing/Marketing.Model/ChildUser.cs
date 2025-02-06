using Caspian.Engine.Model;

namespace Marketing.Model
{
    public class ChildUser: User
    {
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
