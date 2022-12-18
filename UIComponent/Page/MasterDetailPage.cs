using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class MasterDetailPage<TMaster, TDetail> : BasePage where TMaster: class where TDetail: class
    {
        protected CaspianForm<TMaster> MasterForm { get; set; }

        protected DataGrid<TDetail> Grid { get; set; }

        protected TMaster UpsertData { get; set; } = Activator.CreateInstance<TMaster>();
    }
}
