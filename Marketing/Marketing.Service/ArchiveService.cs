using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ArchiveService : BaseService<Archive>, IBaseService<Archive>
    {
        public ArchiveService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.SecretariatId, "بایگانی با این عنوان برای دبیرخانه تعریف شده است");
        }
    }
}
