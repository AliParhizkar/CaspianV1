using Caspian.Common;
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

        [DisplayName("Customer type")]
        public CustomerType CustomerType { get; set; }

        [DisplayName("First name")]
        public string FName { get; set; }

        [DisplayName("Last name")]
        public string LName { get; set; }

        [DisplayName("Gender")]
        public Gender? Gender { get; set; }

        [DisplayName("Company name")]
        public string CompanyName { get; set; }

        [DisplayName("Customer name"), ComputedSqlColumn("(case when [CustomerType]=(1) then ([FName]+' ')+[LName] else [CompanyName] end)")]
        public string CustomerName { get; set; }

        [DisplayName("Customer number")]
        public int CustomerNumber { get; set; }

        [DisplayName("Mobile number")]
        public string MobileNumber { get; set; }

        [DisplayName("Tel")]
        public string Tel { get; set; }

        [CheckOnDelete("The customer has Orders and can not be removed")]
        public IList<Order> Orders { get; set; }

        [CheckOnDelete("Customer is member of group and can not be removed")]
        public IList<CustomerGroupMembership> CustomerGroupMemberships { get; set; }

        [CheckOnDelete("Customer has Address and can not be removed")]
        public IList<CustomerAddress> CustomerAddresses { get; set; }
    }
}
