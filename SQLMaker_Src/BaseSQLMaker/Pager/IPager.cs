using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLMaker.Pager
{
    public interface IPager
    {
        string getPageDataSQL(int pageIndex, int pageCount, bool showErrorInfo = false);

        string getSumDataSQL(bool showErrorInfo = false);

        string getRecordCountSQL(bool showErrorInfo = false);
    }
}
