using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SQLMaker;
using SQLMaker.Helper;
using Carpa.Web.Script;
using BSQLMaker.Const;


namespace BSQLMaker.SQLServer.FuncSQLMaker
{
    [Serializable]
    public class RightsSQLMaker : FunctionStrMaker
    {
        private int oper;
        private Boolean isNeed;
        private string tempCreate;
        private string rightsJoin;

        public RightsSQLMaker(IHashObject qryParams, SQLHelper helper, string conString)
            : base(qryParams, helper, conString)
        {
            
        }

        //注册本SQLMaker函数要替换的SQL中的Tag，及用什么变量来替换
        protected override void registTags()
        {
            tags = new HashObject(new string[] { FuncTags.RIGHTS_TEMP_CREATE, FuncTags.RIGHTS_JOIN }, new string[] { tempCreate, rightsJoin });
        }

        private void addSQLInner()
        {
             tempCreate += @"SELECT pid as id INTO #PRights FROM rights Where eid=" + oper + "\n";
             rightsJoin += @"INNER JOIN #PRights p ON pid=p.id " + "\n";
        }

        protected override void getTagSQL()
        {
            tempCreate = "";
            rightsJoin = "";
            this.oper = queryParams.GetValue<int>(SParam.OPERATOR);

            DbHelper db = new DbHelper(conStr, true);
            addSQLInner();
        }

    }
}
