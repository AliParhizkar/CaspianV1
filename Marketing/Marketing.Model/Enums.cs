using System.ComponentModel.DataAnnotations;

namespace Marketing.Model
{
    public enum ActiveType :byte
    {
        [Display(Name = "فعال")]
        Active = 1,

        [Display(Name = "غیرفعال")]
        DeActive
    }
}
