using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service
{
    public class MeetingService : BaseService<Meeting>, IBaseService<Meeting>
    {
        public MeetingService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("دوره ای با این عنوان ثبت شده است");
        }

        public override async Task UpdateAsync(Meeting meeting)
        {
            var old = await SingleAsync(meeting.Id);
            meeting.OrderNo = old.OrderNo;
            await base.UpdateAsync(meeting);
        }

        public override async Task<Meeting> AddAsync(Meeting meeting)
        {
            var max = await GetAll().MaxAsync(t => (int?)t.OrderNo);
            meeting.OrderNo = max.GetValueOrDefault() + 1;
            return await base.AddAsync(meeting);
        }
    }
}
