using System;
using System.Collections.Generic;
using System.Text;

namespace SQLMaker.Pager
{
    [Serializable]
    public static class PageTag
    {
        public const string REPORT_DATA = "__ReportData__";
        public const string ROWNO = "__RowNo__";
        public const string ROWCOUNT = "__RowCount__";

        public const string PAGE_INDEX = "__PageIndex__";
        public const string PAGE_RECORD_COUNT = "__PageRecordCount__";
        public const string SUM_FIELD = "SumFields";
        public const string TABLE_ALIAS = "__TableAlias__";
        public const string DENSE_RANK = "__DenseRank__";
    }
}
