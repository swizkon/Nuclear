using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuclear.EventSourcing.MySql
{
    public sealed class MySqlConnectionString
    {
        private string connectionString;

        public MySqlConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static explicit operator MySqlConnectionString(string s)
        {
            return new MySqlConnectionString(s);
        }

        public static explicit operator string(MySqlConnectionString obj)
        {
            return obj.connectionString;
        }
    }
}
