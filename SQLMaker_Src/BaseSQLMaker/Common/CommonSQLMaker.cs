using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;

namespace SQLMaker.Common
{
    [Serializable]
    public class CommonSQLMaker : BaseSQLMaker
    {
        string textSql = "";

        public CommonSQLMaker()
        {
        }

        public CommonSQLMaker(IHashObject qryParams, string sql) : base(qryParams, sql)
        {
            this.textSql = sql;
        }

        public void setOriginalSQL(string sql)
        {
            this.textSql = sql;
        }

        protected override string getBaseSQL()
        {
            return textSql;
        }

        protected override void innerIni(DbHelper dbHelper, IHashObject qryParams)
        {
            try
            {
                if (qryParams == null)
                {
                    setTestParams();
                }
                registFuncMakers();
            }
            catch (Exception e)
            {
                throw new Exception("CommonSQLMaker->innerIni: " + e.Message);
            }
        }

        protected override void registFuncMakers()
        {
            //注册需要使用的SQL函数
        }


    }

}
