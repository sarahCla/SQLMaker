using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using BSQLMaker;
using BSQLMaker.SQLServer.Codes;
using BSQLMaker.Const;
using SQLMaker.Helper;
using System.IO;
using System.Collections;
using System.Web;

namespace BSQLMaker.SQLServer.Codes
{
    [Serializable]
    public class BCommonSql : BSqlMaker
    {
        public BCommonSql()
        {
        }

        public BCommonSql(DbHelper dbHelper, IHashObject qryParams, string conString, string sqlPath)
            : base(dbHelper, qryParams, conString, sqlPath)
        {
        }

        protected override string getBaseSQL()
        {
            return getBaseSQL(this.sqlFilePath);
        }


    }
}
