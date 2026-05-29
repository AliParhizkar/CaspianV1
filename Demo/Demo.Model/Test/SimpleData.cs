using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("SimpleData", Schema = "demo")]
    public class SimpleData
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("نوع")]
        public SimpleDataType DataType { get; set; }

        [InverseProperty(nameof(Employee.Scope))]
        [CheckOnDelete("حوزه دارای استخدام می باشد و امکان حذف آن وجود ندارد.")]
        public ICollection<Employee> EmployeesScope { get; set; }

        [InverseProperty(nameof(Employee.CostCenter))]
        [CheckOnDelete("مرکز هزینه دارای استخدام می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Employee> EmployeesCostCenter { get; set; }

        [InverseProperty(nameof(Employee.EmployeeType))]
        [CheckOnDelete("نوع کارمند دارای استخدام می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Employee> EmployeesEmployeeType { get; set; }

        [InverseProperty(nameof(Employee.EmploymentType))]
        [CheckOnDelete("نوع استخدام دارای استخدام می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Employee> EmployeesEmploymentType { get; set; }

        [CheckOnDelete("عنوان شغل همسر دارای استخدام می باشد و امکان حذف آن وجود ندارد.")]
        public ICollection<Family> Families { get; set; }

        [CheckOnDelete("کارمندی با این دین ثبت شده و امکان حذف آن وجود ندارد")]
        public ICollection<ReligionAndSubReligion> ReligionAndSubReligions { get; set; }

        [CheckOnDelete("دین دارای مذهب می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SubReligion> SubReligions { get; set; }
    }
}
