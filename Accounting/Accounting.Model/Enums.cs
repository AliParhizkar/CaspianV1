using System.ComponentModel.DataAnnotations;

namespace Accounting.Model
{
    public enum SimpleDataType: byte
    {
        [Display(Name = "واحد مالی")]
        FinancialUnit
    }
}
