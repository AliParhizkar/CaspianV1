using Caspian.Common;
using Caspian.Common.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Caspian.Engine.Service
{
    public class BlazorControlService : BaseService<BlazorControl>
    {
        public BlazorControlService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Caption).Required().UniqAsync("کنترلی با این عنوان در سیستم ثبت شده است");
        }
    }
}
