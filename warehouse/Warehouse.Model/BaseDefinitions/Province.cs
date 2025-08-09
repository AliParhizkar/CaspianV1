using Caspian.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Warehouse.Model
{
    [Table("Provinces", Schema = "wh")]
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [CheckOnDelete("استان دارای شهر می باشد و امکان حذف آن وجود ندارد")]
        public IList<City> Cities { get; set; }

        [CheckOnDelete("استان دارای فروشنده می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Seller> Sellers { get; set; }
    }
}
