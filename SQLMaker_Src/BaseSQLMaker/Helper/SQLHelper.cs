using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;

namespace SQLMaker.Helper
{
    [Serializable]
    public class SQLHelper
    {
        private string sql;
        private string[] lines;
        private IHashObject paramsSeted = new HashObject();

        public string Sql
        {
            get
            {
                return sql;
            }
        }

        private void intlSetSql(string baseSql)
        {
            sql = baseSql;
            setParamToUpper();
            lines = sql.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void setSQL(string baseSql, IHashObject queryParams)
        {
            sql = baseSql;
            extractParamFromBaseSQL(queryParams);
            intlSetSql(sql);
        }

        private void extractParamFromBaseSQL(IHashObject queryParams)
        {
            try
            {
                string[] ss = sql.Split('}');
                string[] arry = new string[ss.Length - 1];
                string key, value;
                int index = 0;
                for (int i = 0; i < ss.Length - 1; i++)
                {
                    if (ss[i].IndexOf("{-") > 0) continue;
                    arry[i] = ss[i].Substring(ss[i].IndexOf("{"), ss[i].Length - ss[i].IndexOf("{")) + "}";
                    index = arry[i].IndexOf("=");
                    if (index <= 0) continue;
                    key = arry[i].Substring(1, index - 1).Trim();
                    value = arry[i].Substring(index + 1, arry[i].Length - index - 2);
                    if (!queryParams.ContainsKey(key))
                    {
                        queryParams.Add(key.Trim(), value);
                    }
                    sql = sql.Replace(arry[i], "");                    
                }
            }
            catch (Exception e)
            {
                throw new Exception("SQLHelper抽取参数时出错：" + e.Message);
            }
              
        }

        private void setParamToUpper()
        {
            string[] ss = sql.Split('}');
            string[] arry = new string[ss.Length - 1];
            for (int i = 0; i < ss.Length - 1; i++)
            {
                if (ss[i].IndexOf("{-") > 0)
                {
                    arry[i] = ss[i].Substring(ss[i].IndexOf("{-"), ss[i].Length - ss[i].IndexOf("{-")) + "}";
                }
                else
                {
                    arry[i] = ss[i].Substring(ss[i].IndexOf("{"), ss[i].Length - ss[i].IndexOf("{")) + "}";
                }
                sql = sql.Replace(arry[i], arry[i].ToUpper());
            }
        }

        public void SetParam(string name, string value)
        {
            if (string.IsNullOrEmpty(value)) value = "";
            sql = sql.Replace("{" + name.ToUpper() + "}", value);
            CommonFuncs.addHashObject(paramsSeted, name, value);
        }

        public IHashObject getParamsSetted()
        {
            return paramsSeted;
        }

        /**
         * 从baseSql抽取系统参数及用户参数
         * 用户参数：{ParamName}
         * 系统参数：{-ParamName-}
         **/
        public void getAllParams(string baseSql, out List<string> userParams, out List<string> sysParams, out IHashObject defaultValue)
        {
            userParams = new List<string>();
            sysParams = new List<string>();
            defaultValue = new HashObject();
            string s = baseSql;
            string[] ss = s.Split('}');
            string[] arry = new string[ss.Length - 1];
            string param = "", value = "";
            int index = -1;
            for (int i = 0; i < ss.Length - 1; i++)
            {
                if (ss[i].IndexOf("{-") > 0)
                {
                    arry[i] = ss[i].Substring(ss[i].IndexOf("{-") + 2, ss[i].Length - ss[i].IndexOf("{-")-3); 
                    param = arry[i];
                    index = arry[i].IndexOf("=");
                    if (index >= 0)
                    {
                        value = param.Substring(index+1);
                        param = param.Substring(0, index);
                        defaultValue.Add(param, value);
                    }
                    else
                    {
                        if (!sysParams.Contains(param))
                            sysParams.Add(arry[i]);
                    }
                }
                else
                {
                    arry[i] = ss[i].Substring(ss[i].IndexOf("{") + 1, ss[i].Length - ss[i].IndexOf("{")-1);
                    param = arry[i];
                    index = arry[i].IndexOf("=");
                    if (index >= 0)
                    {
                        value = param.Substring(index+1);
                        param = param.Substring(0, index);
                        defaultValue.Add(param, value);
                    }
                    else
                    {
                        if (!userParams.Contains(param))
                            userParams.Add(arry[i]);
                    }
                }
            }
        }

        public string paramsNotSet()
        {
            string paramlist = "";
            string[] ss = sql.Split('}');
            string[] arry = new string[ss.Length - 1];
            for (int i = 0; i < ss.Length - 1; i++)
            {
                if (ss[i].IndexOf("{-") > 0)
                {
                    arry[i] = ss[i].Substring(ss[i].IndexOf("{-"), ss[i].Length - ss[i].IndexOf("{-")) + "}";
                }
                else
                {
                    arry[i] = ss[i].Substring(ss[i].IndexOf("{"), ss[i].Length - ss[i].IndexOf("{")) + "}";
                }
                paramlist += "," + arry[i];
            }
            return paramlist;
        }

        public void SetShortCircuit(bool value, string name, string endStr = "")
        {
            bool empt = false;
            endStr = (endStr != "") ? endStr : "#endif";
            string result = "";
            for (int i = 0; i < lines.GetLength(0); i++)
            {
                if (lines[i].Trim().ToUpper() == endStr.ToUpper() && empt)
                {
                    lines[i] = "";
                    empt = false;
                }
                else if (empt && !value)//过滤条件
                {
                    lines[i] = "";
                }
                else if (lines[i].Trim().ToUpper().StartsWith("#IFDEF ") && lines[i].Trim().ToUpper().EndsWith(" " + name.ToUpper()) ||
                        lines[i].Trim().ToUpper().StartsWith("#IFNDEF ") && lines[i].Trim().ToUpper().EndsWith(" " + name.ToUpper()) && value)
                {

                    empt = true;//去掉字符串
                    lines[i] = "";
                }
                result = result + ((lines[i] == "") ? "" : lines[i] + "\r\n");
            }
            sql = result;
            lines = sql.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
