
namespace Caspian.Common
{
    public class ComputedSqlColumnAttribute : Attribute
    {
        string sql;
        public ComputedSqlColumnAttribute(string sql) 
        {
            this.sql = sql;
        }

        internal string GetSql()
        {  
            return sql; 
        }
    }
}
