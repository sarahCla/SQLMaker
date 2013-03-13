using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using SQLMaker.Helper;
using SQLMaker.Pager;

namespace SQLMaker.Pager.SQLServer
{
    [Serializable]
    public class PageSumMaker : FunctionStrMaker
    {
        public PageSumMaker(IHashObject qryParams, SQLHelper helper, string conString) : base(qryParams, helper, conString)
        {

        }

        protected override void registTags()
        {
        }

        protected override void getTagSQL()
        {
        }

        protected override string wrapSQL(string sql)
        {
            string sumField = queryParams.GetValue<string>(PageTag.SUM_FIELD);
            //string tableAlias = queryParams.GetValue<string>(PageTag.TABLE_ALIAS);
            if (sumField == "")
                throw new Exception("请在getSumData中返回需要计算合计的列信息，例如：\n return 'sum(qty) as qty, sum(total) as total'");
            string basesql = sql;
            if (basesql == "")
                throw new Exception("请在getSumSQL中返回基础SQL");
            sql = basesql + @"
    SELECT " + sumField + @" 
    FROM Data";
            return sql;
        }
    }
}
