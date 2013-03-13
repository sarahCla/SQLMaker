using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using SQLMaker.Helper;
using SQLMaker.Pager;

namespace SQLMaker.Pager.SQLServer
{
    [Serializable]
    public class RecordCountMaker : FunctionStrMaker
    {
        public RecordCountMaker(IHashObject qryParams, SQLHelper helper, string conString)
            : base(qryParams, helper, conString)
        {

        }

        protected override void registTags()
        {
            tags = new HashObject(
                                new string[] { "DenseRank", "RowCount" },
                                new string[] { (queryParams.GetValue<string>(PageTag.DENSE_RANK) == "1") ? "Dense_Rank()" : "Row_Number()",
                                    PageTag.ROWNO });            
        }

        protected override void getTagSQL()
        {
            
        }

        protected override string wrapSQL(string sql)
        {
            string wrapedsql = sql + @"
    , __ReportData__ AS (
        select {-DenseRank-} over (order by  {-sortInfo-}) AS RowNo, Data.*
		from Data
             {-sortJoinInfo-}
    )
    SELECT MAX(RowNo) AS {-RowCount-} FROM  __ReportData__";
            return wrapedsql;
        }

    }
}
