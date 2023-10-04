using Demo.Model;
using Demo.Service;
using Caspian.Dynamicform.Component;
using System.Linq;

namespace Caspian.Dynamic.WorkflowForm
{
    public partial class Employment : BasePage
    {
        Context context = new Demo.Model.Context();
        public void Test()
        {
            this.Gender = Gender.Male;
            context.Cities.Any();
            using var service = CreateScope().GetService<CustomerService>();
        }

        public void Initialize()
        {

        }
    }
}