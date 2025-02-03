
namespace Caspian.Common
{
    public class ComputedColumnSqlAttribute : Attribute
    {
        string sql;
        public ComputedColumnSqlAttribute(string sql) 
        {
            this.sql = sql;
        }

        internal string GetSql()
        {  
            return sql; 
        }
    }
}
