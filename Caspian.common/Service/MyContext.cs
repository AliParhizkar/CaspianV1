using Caspian.Common.RowNumber;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Caspian.Common
{
    public class MyContext : DbContext
    {
        //public MyContext(DbContextOptions<MyContext> options = null)
        //{
        //
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            optionsBuilder.UseSqlServer(CS.Con, t => 
            {
                t.AddRowNumberSupport();
                
            }).EnableSensitiveDataLogging();
            optionsBuilder.UseLazyLoadingProxies(false);
            base.OnConfiguring(optionsBuilder);
        }
    }
}