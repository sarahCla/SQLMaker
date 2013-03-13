using System;
using System.Collections.Generic;
using System.Text;

namespace SQLMaker.Helper
{
    [Serializable]
    public class ExecuteSQL
    {
        private string sql = "";
        private string paramDeclares = "";
        private string paramValues = "";

        public string Sql
        {
            get { return sql; }
            set { sql = value; }
        }

        public void initSQL(string sqlCommand)
        {
            sql = sqlCommand;
            paramDeclares = "";
            paramValues = "";
        }

        public string ParamDeclares
        {
            get { return paramDeclares; }
            set { paramDeclares = value; }
        }

        public string ParamValues
        {
            get { return paramValues; }
            set { paramValues = value; }
        }

        public string getExecuteSQL()
        {
            sql = "\r\n" + CommonFuncs.DelEmptyOrCommandLines(sql);
            return "EXECUTE SP_EXECUTESQL N'" + sql.Replace("'", "''") + "'" + ((paramDeclares.Trim() == "") ? "" : ",")
                                              + paramDeclares + ((paramDeclares.Trim() == "") ? "" : ",")
                                              + paramValues;
        }
    }

}
