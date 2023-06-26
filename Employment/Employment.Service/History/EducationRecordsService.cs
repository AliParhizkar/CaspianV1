using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class EducationRecordsService : BaseService<EducationRecords>, IBaseService<EducationRecords>
    {
        public EducationRecordsService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
