using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    public class Account
    {
        public virtual int Id { get; set; }

        [Display(Name = "نوع صندوق")]
        public virtual CashBoxType CashBoxType { get; set; }
        
        [Display(Name = "خزانه دار")]
        public virtual AccountHolder AccountHolder { get; set; }
        
        [Display(Name = "عنوان")]
        public virtual string Title { get; set; }
        
        public virtual string AccountTitle { get; set; }

        [Display(Name = "سقف انتقال")]
        public virtual double FloorLimit { get; set; }
        
        [Display(Name = "وضعیت")]
        public virtual Status Status { get; set; }
        
        [Display(Name = "توضیح")]
        public virtual string Description { get; set; }
    }
}