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
            IConfigurationRoot configuration = new ConfigurationBuilder().Build();
            string connectionString = configuration.GetConnectionString("ConnectionStrings");
            optionsBuilder.UseSqlServer(CS.Con, t => 
            {
                t.AddRowNumberSupport();
                
            }).EnableSensitiveDataLogging();
            optionsBuilder.UseLazyLoadingProxies(false);
            base.OnConfiguring(optionsBuilder);
        }

        //[DbFunction("ConvertToInteger", Schema = "dbo")]
        public static int? ConvertToInteger(string value)
        {
            throw new NotImplementedException("خطا:this method mapped to sql and no need to called");
        }

        //[DbFunction("ConvertToDecimal", Schema = "dbo")]
        public static decimal? ConvertToDecimal(string value)
        {
            throw new NotImplementedException("خطا:this method mapped to sql and no need to called");
        }

        //[DbFunction("ConvertToString", Schema = "dbo")]
        public static string ConvertToString(int value)
        {
            throw new NotImplementedException("خطا:this method mapped to sql and no need to called");
        }

        //[DbFunction("EnumFaName", Schema = "dbo")]
        public string EnumFaName(string value)
        {
            throw new NotImplementedException("خطا:this method mapped to sql and no need to called");
        }
    }
}