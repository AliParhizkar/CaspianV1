using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Fund.Model
{
    public abstract class Account
    {
        [Key]
        public virtual int Id { get; set; }

        [Display(Name = "عنوان")]
        public virtual string Title { get; set; }

        [Display(Name = "سقف انتقال")]
        public virtual decimal FloorLimit { get; set; }

        [Display(Name = "وضعیت")]
        public virtual Status Status { get; set; }

        [Display(Name = "توضیح")]
        public virtual string Description { get; set; }

        [Display(Name = "نوع صندوق")]
        public int CashBoxTypeId { get; set; }

        [Display(Name = "صندوق مرجع جایگاه")]
        public bool? IsReferenceCashBox { get; set; }

        [Display(Name = "خزانه دار")]
        public int AccountHolderId { get; set; }

        [ForeignKey(nameof(CashBoxTypeId))]
        public virtual CashBoxType CashBoxType { get; set; }

        [ForeignKey(nameof(AccountHolderId))]
        public virtual AccountHolder AccountHolder { get; set; }
    }
    
    public enum AccountTypeEnum : byte
    {
        None = 0,
        Cashbox=1,
        Treasury=2,
        Leakage=3,
        Over=4
    }
}