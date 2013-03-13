using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using System.Reflection;
using BSQLMaker.SQLServer.Codes;
using SQLMaker.Helper;

namespace BSQLMaker.SQLServer.Common
{
    public static class BMakerHelper
    {
        public static Boolean isPagerMaker(string classPath)
        {
            Assembly assembly = Assembly.Load("BSQLMaker");
            object obj = assembly.CreateInstance(classPath);
            return typeof(BPagerSqlMaker).IsInstanceOfType(obj);
        }

        public static BSqlMaker getMaker(string conString, string filePath, IHashObject queryParams)
        {
            DbHelper db = new DbHelper(conString, true);
            Assembly assembly = Assembly.Load("BSQLMaker");
            object[] param = { db, queryParams, conString, filePath };
            string className = SQLFileHelper.getMakerClass(filePath);
            Type itype = assembly.GetType(className);
            object obj = Activator.CreateInstance(itype, param);
            BSqlMaker maker = (BSqlMaker)obj;
            maker.SQLFilePath = filePath;
            return maker;
        }

        public static BSqlMaker getMaker(string conString, string filePath, string classPath, IHashObject queryParams)
        {
            DbHelper db = new DbHelper(conString, true);
            Assembly assembly = Assembly.Load("BSQLMaker");
            object[] param = { db, queryParams, conString, filePath };
            Type itype = assembly.GetType(classPath);
            object obj = Activator.CreateInstance(itype, param);
            BSqlMaker maker = (BSqlMaker)obj;
            maker.SQLFilePath = filePath;
            return maker;
        }

    }
}
