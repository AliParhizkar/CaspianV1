using Demo.Model; 
using Demo.Service;
using System.Collections.Generic;
using Capian.Dynamicform.Component;


namespace Caspian.Dynamic.WorkflowForm
{
	public partial class Employment
	{
		public bool ForTest(IList<Area> areas, int id)
		{
			using var scope = CreateScope().GetService<CustomerService>();
            return false;
        }
		
		public void Initialize()
		{

		}
	}
}