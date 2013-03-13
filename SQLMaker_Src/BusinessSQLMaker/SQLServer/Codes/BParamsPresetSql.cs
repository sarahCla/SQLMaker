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

namespace BSQLMaker.SQLServer.Procs
{
    [Serializable]
    public class BParamsPresetSql : BSqlMaker
    {
        public BParamsPresetSql()
        {
        }

        public BParamsPresetSql(DbHelper dbHelper, IHashObject qryParams, string conString, string sqlPath)
            : base(dbHelper, qryParams, conString, sqlPath)
        {
        }

        protected override void setParams()
        {
            setAllParamsAuto();
            string parid = queryParams.GetValue<string>("parid");
            helper.SetParam("TempLen", (parid == "") || (parid == "00000") ? "5" : (parid.Length + 5).ToString());
        }

        protected override string getBaseSQL()
        {
            return getBaseSQL("test.sql");
        }

    }
}
