using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager
{
    class DatabaseConnection
    {
        public static OleDbConnection GetConnection()
        {
            return new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=E:\Vinay\Training\Document Manager\DocMgr Files\main.mdb");
        }
    }
}
