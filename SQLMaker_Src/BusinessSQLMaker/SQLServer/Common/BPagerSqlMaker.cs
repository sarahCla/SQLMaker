using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Carpa.Web.Script;
using BSQLMaker.SQLServer.FuncSQLMaker;
using SQLMaker.Pager;
using BSQLMaker.Const;
using SQLMaker;
using SQLMaker.Pager.SQLServer;
using SQLMaker.Helper;

namespace BSQLMaker.SQLServer.Codes
{
    [Serializable]
    public abstract class BPagerSqlMaker : BSqlMaker, IPager
    {
        protected Boolean isDenseRank;        

        public BPagerSqlMaker()
        {
        }

        public BPagerSqlMaker(DbHelper dbHelper, IHashObject qryParams, string conString, string sqlPath) : base(dbHelper, qryParams, conString, sqlPath)
        {
        }

        protected override void setInnerParams()
        {
            base.setInnerParams();
            addParam(queryParams, PageTag.DENSE_RANK, (isDenseRank) ? "1" : "0");
        }

        public DataTable getPageData()
        {
            int pageIndex = Int32.Parse(CommonFuncs.getHashObject(queryParams, PageTag.PAGE_INDEX, "0"));
            int pageCount = Int32.Parse(CommonFuncs.getHashObject(queryParams, PageTag.PAGE_RECORD_COUNT, "20"));
            return db.ExecuteSQL(getPageDataSQL(pageIndex, pageCount));
        }

        public DataTable getSumData()
        {
            return db.ExecuteSQL(getSumDataSQL());
        }

        public int getRecordCount()
        {
            DataTable dt = db.ExecuteSQL(getRecordCountSQL());
            if (dt.Rows.Count <= 0)
                return 0;
            return Int32.Parse(dt.Rows[0][PageTag.ROWCOUNT].ToString());
        }

        protected virtual string getDefaultSortInfo()
        {
            return "";
        }

        public string getPageDataSQL(int pageIndex, int pageCount, bool showErrorInfo = false)
        {
            try
            {
                wrappers.Clear();
                wrappers.Add(new PageDataStrMaker(queryParams, helper, conStr, pageIndex, pageCount, getDefaultSortInfo()));
                string sql = getSQL();
                if (showErrorInfo)
                {
                    checkSQL(sql);
                }
                return sql;
            }
            catch (Exception error)
            {
                throw new Exception("BPagerSqlMaker->getPageDataSQL:" + error.Message);
            }
        }

        public string getSumDataSQL(bool showErrorInfo = false)
        {
            try
            {
                wrappers.Clear();
                wrappers.Add(new PageSumMaker(queryParams, helper, conStr));
                string sql = getSQL();
                if (showErrorInfo)
                {
                    checkSQL(sql);
                }
                return sql;
            }
            catch (Exception error)
            {
                throw new Exception("BPagerSqlMaker->getSumDataSQL:" + error.Message);
            }
        }

        public string getRecordCountSQL(bool showErrorInfo = false)
        {
            try
            {
                wrappers.Clear();
                wrappers.Add(new RecordCountMaker(queryParams, helper, conStr));
                string sql = getSQL();
                if (showErrorInfo)
                {
                    checkSQL(sql);
                }
                return sql;
            }
            catch (Exception error)
            {
                throw new Exception("BPagerSqlMaker->getRecordCountSQL:" + error.Message);
            }
        }

    }
}
