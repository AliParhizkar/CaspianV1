using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Customers", Schema = "demo")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }



        [DisplayName("Customer type"), ReportField]
        public CustomerType CustomerType { get; set; }

        [DisplayName("First name"), ReportField]
        public string FName { get; set; }

        [DisplayName("Last name"), ReportField]
        public string LName { get; set; }

        [DisplayName("Gender"), ReportField]
        public Gender? Gender { get; set; }

        [DisplayName("Company name"), ReportField]
        public string CompanyName { get; set; }

        [DisplayName("Customer number")]
        public int CustomerNumber { get; set; }

        [DisplayName("Mobile number"), ReportField]
        public string MobileNumber { get; set; }

        [DisplayName("Tel"), ReportField]
        public string Tel { get; set; }

        [CheckOnDelete("The customer has Orders and can not be removed")]
        public virtual IList<Order> Orders { get; set; }

        [CheckOnDelete("Customer is member of group and can not be removed")]
        public virtual IList<CustomerGroupMembership> CustomerGroupMemberships { get; set; }

        [CheckOnDelete("Customer has Address and can not be removed")]
        public virtual IList<CustomerAddress> CustomerAddresses { get; set; }
    }
}
