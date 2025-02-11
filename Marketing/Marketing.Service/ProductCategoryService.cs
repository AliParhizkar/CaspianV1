using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class SecretariatService : BaseService<Secretariat>, IBaseService<Secretariat>
    {
        public SecretariatService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("The title of the product category must be unique.");
            RuleFor(t => t.Periority).Range((byte)0, (byte)20, "فقط مقادیر 0 تا 20 معتبر است");
        }
    }
}
