using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class EducationRecordsService : BaseService<EducationRecords>, IBaseService<EducationRecords>
    {
        public EducationRecordsService(ServiceProvider provider)
            :base(provider)
        {

        }
    }
}
