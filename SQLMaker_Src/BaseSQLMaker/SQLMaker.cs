using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using SQLMaker.Helper;
using System.Windows.Forms;
using System.IO;

namespace SQLMaker
{
    [Serializable]
    public abstract class BaseSQLMaker:IDisposable
    {
        protected IHashObject queryParams;
        protected List<string> userParams;
        protected List<string> sysParams;

        protected DbHelper db;
        private ExecuteSQL executeSql;
        protected List<FunctionStrMaker> funcMakers;
        protected List<FunctionStrMaker> wrappers;
        public SQLHelper helper;
        protected string conStr;
        protected string sqlFilePath = "";

        private String originSQL = "";

        protected virtual string getSQLFilePath()
        {
            return sqlFilePath;
        }

        public string SQLFilePath
        {
            get { return getSQLFilePath(); }
            set { sqlFilePath = value; }
        }


        public BaseSQLMaker() { }

        protected void setAllParamsAuto()
        {
            foreach (KeyValuePair<string, object> pair in queryParams)
            {

                if (pair.Value != null)
                {
                    helper.SetParam(pair.Key, pair.Value.ToString());
                }
            }
        }

        public void getBaseSqlInfo(out string baseSQl, out List<string> userParams, out List<string> sysParams, out IHashObject defaultValue)
        {
            baseSQl = getBaseSQL();
            if (helper == null) helper = new SQLHelper();
            helper.getAllParams(baseSQl, out userParams, out sysParams, out defaultValue);
        }

        #region 需要子类实现的部分
        protected abstract void registFuncMakers();
        protected abstract string getBaseSQL();
        #endregion

        #region 允许子类重构的部分
        protected virtual void setParams()
        {
            setAllParamsAuto();
        }

        public virtual void setTestParams()
        {
            if (helper == null) helper = new SQLHelper();
            if (queryParams == null)
            {
                queryParams = new HashObject();
                string sql = getBaseSQL();
                helper.setSQL(getBaseSQL(), queryParams);
            }
            else
                helper.setSQL(getBaseSQL(), queryParams);
        }
        #endregion

        public List<string> getUserParams()
        {
            return userParams;
        }

        public IHashObject getParams()
        {
            if (queryParams == null)
                setTestParams();
            return queryParams;
        }

        public bool checkParams()
        {
            //ToDo: 检查基础sql中的参数，和实际的测试参数，或传入的参数是否匹配
            return false;
        }

        protected string getParam(string key)
        {
            try
            {
                return queryParams.GetValue<string>(key);
            }
            catch (Exception e)
            {
                throw new Exception(this.ToString() + "需要的参数<" + key + ">在queryParam中找到不到");
                //return "";
            }
        }

        protected void addParam(IHashObject qryParams, string key, object value)
        {
            if (qryParams.ContainsKey(key))
                qryParams[key] = value;
            else
                qryParams.Add(key, value);

        }

        protected void setParam(string key)
        {
            helper.SetParam(key, CommonFuncs.getValueIgnoreCase(queryParams, key, ""));
        }

        public BaseSQLMaker(IHashObject qryParams, string sql)
        {
            conStr = "";
            originSQL = sql;
            this.sqlFilePath = "";
            ini(null, qryParams);
        }
        
        public BaseSQLMaker(DbHelper dbHelper, IHashObject qryParams, string conString, string filePath)
        {
            conStr = conString;
            this.sqlFilePath = filePath;
            ini(dbHelper, qryParams);
        }

        protected abstract void innerIni(DbHelper dbHelper, IHashObject qryParams);


        public void ini(DbHelper dbHelper, IHashObject qryParams)
        {
            try
            {
                this.funcMakers = new List<FunctionStrMaker>();
                this.wrappers = new List<FunctionStrMaker>();
                this.db = dbHelper;
                this.queryParams = qryParams;
                executeSql = new ExecuteSQL();
                helper = new SQLHelper();

                innerIni(dbHelper, qryParams);
                
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        private string SetShortCircuit(string[] lines, string name, string endStr = "")
        {
            bool value = false;
            bool empt = false;
            endStr = (endStr != "") ? endStr : "#endif";
            endStr = endStr.ToUpper();
            string result = "";
            for (int i = 0; i < lines.GetLength(0); i++)
            {
                if (lines[i].Trim().ToUpper() == endStr && empt)
                {
                    lines[i] = "";
                    empt = false;
                    value = false;
                }
                else if (value) // && !value)//过滤条件
                {
                    lines[i] = "";
                }
                else if (lines[i].Trim().ToUpper().StartsWith("#IFDEF ") && lines[i].Trim().ToUpper().EndsWith(" " + name.ToUpper()))
                {
                    value = false;
                    empt = true;//去掉字符串
                    lines[i] = "";
                }
                else if (lines[i].Trim().ToUpper().StartsWith("#IFNDEF ") && lines[i].Trim().ToUpper().EndsWith(" " + name.ToUpper()))
                {
                    value = true;
                    empt = true;//去掉字符串
                    lines[i] = "";
                }
                result = result + ((lines[i] == "") ? "" : lines[i] + "\r\n");
            }
            return result;
        }

        private string SetShortCircuitAfter(string[] lines)
        {
            bool value = false;
            bool empt = false;
            string endStr = "#endif".ToUpper();
            string result = "";
            for (int i = 0; i < lines.GetLength(0); i++)
            {
                if (lines[i].Trim().ToUpper() == endStr && empt)
                {
                    lines[i] = "";
                    empt = false;
                    value = false;
                }
                else if (value) 
                {
                    lines[i] = "";
                }
                else if (lines[i].Trim().ToUpper().StartsWith("#IFDEF "))
                {
                    value = true;
                    empt = true;//去掉字符串
                    lines[i] = "";
                }
                else if (lines[i].Trim().ToUpper().StartsWith("#IFNDEF "))
                {
                    value = false;
                    empt = true;//去掉字符串
                    lines[i] = "";
                }
                result = result + ((lines[i] == "") ? "" : lines[i] + "\r\n");
            }
            return result;
        }


        protected string shortCircuit(string sql)
        {
            string sSql = sql;
            string[] lines = sSql.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (KeyValuePair<string, object> pair in queryParams)
            {
                sSql = SetShortCircuit(lines, pair.Key);
                if ((sSql.ToUpper().IndexOf("#IFDEF ") < 0) && (sSql.ToUpper().IndexOf("#IFNDEF ") < 0)) break;
                lines = sSql.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            lines = sSql.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            sSql = SetShortCircuitAfter(lines);
            return sSql;
        }

        protected void spliceAndWrap()
        {
            helper.setSQL(getBaseSQL(), queryParams);
            setParams();
            executeSql.initSQL(helper.Sql);
            string makerName = "";
            try
            {
                foreach (FunctionStrMaker maker in wrappers)
                {
                    makerName = maker.GetType().Name;
                    maker.refineSQL();
                    executeSql.Sql = helper.Sql;
                    executeSql.ParamDeclares +=
                        ((maker.ParamDeclares() == "" || executeSql.ParamDeclares == "") ? "" : ",") + maker.ParamDeclares();
                    executeSql.ParamValues +=
                        ((maker.ParamValues() == "" || executeSql.ParamValues == "") ? "" : ",") + maker.ParamValues();
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("SQLMaker->spliceAndWrap->wrappers(" + makerName + "): " + e.Message);
            }
            try
            {
                foreach (FunctionStrMaker maker in funcMakers)
                {
                    makerName = maker.GetType().Name;
                    maker.refineSQL();
                    executeSql.Sql = helper.Sql;
                    executeSql.ParamDeclares +=
                        ((maker.ParamDeclares() == "" || executeSql.ParamDeclares == "") ? "" : ",") + maker.ParamDeclares();
                    executeSql.ParamValues +=
                        ((maker.ParamValues() == "" || executeSql.ParamValues == "") ? "" : ",") + maker.ParamValues();
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("SQLMaker->spliceAndWrap->funcs(" + makerName + "): " + e.Message);
            }

        }

        public string getSQL(Boolean useExecuteSQL = true)
        {
            try
            {
                spliceAndWrap();
                if (useExecuteSQL)
                    return shortCircuit(executeSql.getExecuteSQL());
                else
                {
                    string s = "\r\n" + CommonFuncs.DelEmptyOrCommandLines(executeSql.Sql);
                    return shortCircuit(s);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                throw new Exception(e.Message);
            }
        }

        protected void checkSQL(string sql)
        {
            string errInfo = "";
            SQLHelper h = new SQLHelper();
            List<string> sysParams, userParams;
            IHashObject defaultValue = null;
            h.getAllParams(sql, out userParams, out sysParams, out defaultValue);
            bool haveSys = false;
            foreach (string param in userParams)
            {
               errInfo += "用户参数： [" + param + "]  没有设置对应的值，请检查输入参数\n\r";
            }
            foreach (string param in sysParams)
            {
                errInfo += "公用变量： [" + param + "] 没有设置对应的值，请检查注册的处理器\n\r";
                haveSys = true;
            }
            if (errInfo != "")
            {
                if (haveSys)
                {
                    errInfo += "\n\r\n\r";
                    errInfo += "[当前注册的公用变量]:" + "\n\r";
                    foreach (FunctionStrMaker maker in funcMakers)
                    {
                        errInfo += "类名：" + maker.GetType().Name + "\n\r";
                        foreach (KeyValuePair<string, object> pair in maker.tags)
                        {
                            errInfo += "  " + pair.Key + "\n\r";
                        }
                    }
                }
                Clipboard.Clear();
                Clipboard.SetText(errInfo);
                System.Windows.Forms.MessageBox.Show(errInfo);
            }
        }

        public string getBaseSQL(string sqlFileName)
        {
            if (sqlFileName.Equals("") && !originSQL.Equals("")) return originSQL;
            StreamReader objReader = new StreamReader(sqlFileName); 
            string sLine = "", sql = "";
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    if (sLine.Trim().StartsWith("--")) continue;
                    sql += sLine + "\r\n";
                }
            }
            objReader.Close();
            return sql;
        }

        public void Dispose()
        {
           
        }
    }
}
