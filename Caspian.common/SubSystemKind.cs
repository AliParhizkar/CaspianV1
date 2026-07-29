using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace Caspian.Common
{
    public enum SubsystemKind: byte
    {
        [SubsystemMetaData("Main", Schema = "dbo")]
        Main = 0,

        [SubsystemMetaData("Engine", Schema = "cmn")]
        Engine = 1,

        /// <summary>
        /// دمو
        /// </summary>
        [SubsystemMetaData("دمو", Schema = "demo")]
        Demo,

        [SubsystemMetaData("فروش", Schema = "mrk")]
        Marketing,

        [SubsystemMetaData("انبار", Schema = "wh")]
        Warehouse,

        [SubsystemMetaData("حسابداری", Schema = "acc")]
        Accounting,

        [SubsystemMetaData("اموال و دارایی ثابت", Schema = "ivm")]
        Investment,

        [SubsystemMetaData("تدارکات", Schema = "pcm")]
        Procurement,

        [SubsystemMetaData("دریافت و پرداخت", Schema = "par")]
        PaymentAndReceipt
    }

    public static class SubsystemExtension
    {
        private static Assembly GetAssembly(SubsystemKind subsystemKind, bool isModel)
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var index = path.LastIndexOf("\\");
            path = path.Substring(0, index) + "\\";
            path += subsystemKind.ToString() + (isModel ? ".Model" : ".Service") + ".dll";
            return Assembly.LoadFile(path);
        }

        public static Assembly GetEntityAssembly(this SubsystemKind systemKind)
        {
            return GetAssembly(systemKind, true);
        }

        public static (IEnumerable<string> migrationsName, IEnumerable<string> migrationsVersion, IEnumerable<string> appendedMigrations) GetMigrationData(this SubsystemKind subsystem)
        {
            var contextType = subsystem.GetEntityAssembly().GetTypes().Single(t => t.BaseType == typeof(CaspianContext));
            using var context = Activator.CreateInstance(contextType) as CaspianContext;
            var migrationsAssembly  = context.GetService<IMigrationsAssembly>();
            var migrationsName = new List<string>();
            var migrationsVersion = new List<string>();
            foreach(var migration in migrationsAssembly.Migrations)
            {
                migrationsName.Add(migration.Value.Name);
                migrationsVersion.Add(migration.Key);
            }
            var list = context.Database.GetAppliedMigrations();
            return (migrationsName, migrationsVersion, list);
        }

        public static bool HasEntityType(this SubsystemKind systemKind, string namespace_, string name)
        {
            return GetEntityAssembly(systemKind).GetTypes().Any(t => t.Namespace == namespace_ && t.Name == name); 
        }

        public static Assembly GetServiceAssembly(this SubsystemKind systemKind)
        {
            return GetAssembly(systemKind, false);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SubsystemMetaDataAttribute : Attribute
    {

        public SubsystemMetaDataAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public string Schema { get; set; }
    }
}
