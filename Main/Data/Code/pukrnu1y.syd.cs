using Demo.Model;
using Demo.Service;
using Caspian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
	public partial class Employment: BasePage 
	{
		public void Test()
		{
			this.Gender = Gender.Male;
			using var service = CreateScope().GetService<CustomerService>();
		}

		public void Initialize()
		{
			
		}
	}
}