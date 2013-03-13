using System;
using System.Collections.Generic;
using System.Text;
using SQLMaker.Helper;
using Carpa.Web.Script;
using BSQLMaker.SQLServer.FuncSQLMaker;
using BSQLMaker.Const;
using SQLMaker.Pager;
using SQLMaker;


namespace BSQLMaker.SQLServer.Codes
{
    [Serializable]
    public abstract class BSqlMaker : BaseSQLMaker
    {

        protected override string getSQLFilePath()
        {
            return sqlFilePath + @"..\BDLayer\SQLServer\Procs\";
        }

        public BSqlMaker() { }

        public BSqlMaker(DbHelper dbHelper, IHashObject qryParams, string conString, string filePath) : base(dbHelper, qryParams, conString, filePath)
        {
        }


        protected override void innerIni(DbHelper dbHelper, IHashObject qryParams)
        {
            try
            {
                if (qryParams == null)
                {
                    setTestParams();
                }
                setInnerParams();
                registFuncMakers();
            } catch (Exception e)
            {
                throw new Exception("BSqlMaker->innerIni: " + e.Message);
            }
        }

        protected override void registFuncMakers()
        {
            //注册需要使用的SQL函数
            funcMakers.Add(new RightsSQLMaker(queryParams, helper, conStr));
            funcMakers.Add(new SortSQLMaker(queryParams, helper, conStr));
        }

        protected virtual void setInnerParams()
        {
        }


    }
}
