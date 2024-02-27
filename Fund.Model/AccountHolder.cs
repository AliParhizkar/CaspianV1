using Caspian.Engine;
using Caspian.Engine.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    [Display(Name = "صندوق دار")]
    public abstract class AccountHolder
    {
        [Key]
        public virtual int Id { get; set; }

        public long Code { get; set; }

        [Display(Name = "وضعیت")]
        public Status Status {  get; set; }

        [Display(Name = "تاریخ شروع")]
        public DateTime? BeginDate { get; set; }

        [Display(Name = "تاریخ پایان")]
        public DateTime? EndDate {  get; set; }

        //public AccountHolderTypeEnum Type { get; set; }

        [Display(Name = "توضیح")]
        public string Description { get; set; }

        [Display(Name = "کاربر")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }

    public enum AccountHolderTypeEnum : byte
    {
        None = 0,
        Cashier = 1,
        Treasurer = 2
    }
}
