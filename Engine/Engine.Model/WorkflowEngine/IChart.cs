
namespace Caspian.Engine.Model
{
    public interface IChart<TOrgan, TPost>
    {
        public TOrgan Superior();

        public TOrgan AboveSuperior();

        public IList<TPost> GetPosts();

        public IList<TOrgan> Posts();


    }
}
