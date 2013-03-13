using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using SQLMaker.Helper;

namespace SQLMaker
{
    [Serializable]
    public abstract class FunctionStrMaker
    {
        protected SQLHelper helper;
        protected IHashObject queryParams;
        public IHashObject tags;
        protected string conStr;

        protected string paramDeclares = "";
        protected string paramValues = "";

        public string ParamValues()
        {
            return paramValues.Trim(); 
        }

        public string ParamDeclares()
        {
            return paramDeclares.Trim(); 
        }

        public IHashObject QueryParams
        {
            get { return queryParams; }
            set
            {
                queryParams = value;
                getTagSQL();
            }
        }

        public FunctionStrMaker(IHashObject qryParams, SQLHelper sqlHelper, string conString)
        {
            this.conStr = conString;
            this.helper = sqlHelper;
            QueryParams = qryParams;
        }

        private void makeSQL()
        {
            registTags();
            string sql = helper.Sql;
            string sqlWraped = wrapSQL(helper.Sql);
            if (sql != sqlWraped)
                helper.setSQL(sqlWraped, queryParams);
                //helper.Sql = sqlWraped;

            if (tags != null)
            {
                foreach (KeyValuePair<string, object> a in tags)
                {
                    if (a.Value == null)
                        throw new Exception(this.ToString() + "的Tag<" + a.Key + ">的值为null");
                    helper.SetParam("-" + a.Key + "-", a.Value.ToString());
                }
            }
        }

        protected string getParam(string key)
        {
            return queryParams.GetValue<string>(key);
        }

        public void refineSQL()
        {
            try
            {
                getTagSQL();
                makeSQL();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        protected virtual string wrapSQL(string sql)
        {
            return sql;
        }

        #region 具体的SQL功能拼接器要实现的部分

        protected abstract void getTagSQL();
        protected abstract void registTags();

        #endregion




    }
}
