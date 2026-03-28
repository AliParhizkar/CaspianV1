using Caspian.Common;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class EntityTypeService : BaseService<EntityType>
    {
        public EntityTypeService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("موجودیتی با این عنوان در سیستم تعریف شده است");
            RuleFor(t => t.Namespace).Required();
            RuleFor(t => t.Name).Required();
            RuleFor(t => t.Subsystem).Custom(t => t.Subsystem.HasEntityType(t.Namespace, t.Namespace), "نوع نامعتبر است");
        }
    }
}
