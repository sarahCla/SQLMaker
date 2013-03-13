using System;
using System.Collections.Generic;
using System.Text;
using BSQLMaker.SQLServer.Codes;
using Carpa.Web.Script;

namespace BSQLMaker.SQLServer.Codes
{
    class BCommonPagerSql : BPagerSqlMaker
    {
         public BCommonPagerSql() { }

         public BCommonPagerSql(DbHelper dbHelper, IHashObject qryParams, string conString, string sqlPath)
            : base(dbHelper, qryParams, conString, sqlPath)
        {
        }

         protected override string getBaseSQL()
         {
             return getBaseSQL(this.sqlFilePath);
         }
    }
}
