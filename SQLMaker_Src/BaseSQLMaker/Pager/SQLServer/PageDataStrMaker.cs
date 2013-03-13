using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using SQLMaker.Helper;
using SQLMaker.Pager;

namespace SQLMaker.Pager.SQLServer
{
    [Serializable]
    public class PageDataStrMaker : FunctionStrMaker
    {
        private int index;
        private int count;
        private string sortInfo;

        public PageDataStrMaker(IHashObject qryParams, SQLHelper helper, string conString, int pageIndex, int pageCount, string sort) : base(qryParams, helper, conString)
        {
            this.index = pageIndex;
            this.count = pageCount;
            this.sortInfo = sort;
        }

        protected override void registTags()
        {
            tags = new HashObject(
                    new string[] { "DenseRank"},
                    new string[] { (queryParams.GetValue<string>(PageTag.DENSE_RANK) == "1") ? "Dense_Rank()" : "Row_Number()"});
        }

        protected override void getTagSQL()
        {
        }

        protected override string wrapSQL(string sql)
        {
            paramDeclares = "N'@recIndexBegin int, @recIndexEnd int, @pageCount int'";
            paramValues = (index * count) + "," + ((index + 1) * count) + "," + count;
            string wrapSQL = sql + @"
    , __ReportData__ AS (
        select Count(*) over () AS Rowc,{-DenseRank-} over (order by  {-sortInfo-}) AS RowNo, Data.*
		from Data
             {-sortJoinInfo-}
    )
    select * from __ReportData__ 
    where (@pageCount=0) or (RowNo > @recIndexBegin) and (RowNo<= @recIndexEnd)";
            return wrapSQL;
        }
    }
}
