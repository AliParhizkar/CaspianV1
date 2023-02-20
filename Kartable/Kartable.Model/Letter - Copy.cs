﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartable.Model
{
    [Table("LetterRefrences", Schema = "kar")]
    public class LetterRefrence
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// کد نامه
        /// </summary>
        [DisplayName("نامه")]
        public int LetterId { get; set; }

        /// <summary>
        /// مشخصات نامه
        /// </summary>
        [ForeignKey(nameof(LetterId))]
        public virtual Letter Letter { get; set; }
    }
}