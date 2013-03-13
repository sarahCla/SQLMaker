using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using System.Reflection;

namespace SQLMaker.Common
{
    public static class BaseMakerHelper
    {
        public static String getSQL(string sql)
        {
            IHashObject queryParams = new HashObject();
            CommonSQLMaker maker = new CommonSQLMaker(queryParams, sql);
            maker.setOriginalSQL(sql);
            maker.setTestParams();
            return maker.getSQL(false);
        }
    }
}
