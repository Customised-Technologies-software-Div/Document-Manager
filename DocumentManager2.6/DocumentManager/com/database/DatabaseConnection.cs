using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager
{
    /// <summary>
    /// This class will give OledbConnection object for connecting to db. This is required to be different class, as whenever we want to change db location, we have to just change in this one place.
    /// </summary>
    class DatabaseConnection
    {
        public static OleDbConnection GetConnection()
        {
            return new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\main.mdb");
        }
    }
}
