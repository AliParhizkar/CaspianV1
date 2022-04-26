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

        protected override void OnAfterRender(bool firstRender)
        {
            if (MasterForm != null)
            {
                if ((MasterForm as ICaspianForm).MasterIdName == null)
                {
                    var info = typeof(TDetail).GetProperties().Single(t => t.PropertyType == typeof(TMaster));
                    (MasterForm as ICaspianForm).MasterIdName = info.GetCustomAttribute<ForeignKeyAttribute>().Name;
                }
                if (Grid != null)
                    Grid.MasterForm = MasterForm;
            }
            base.OnAfterRender(firstRender);
        }
    }
}
