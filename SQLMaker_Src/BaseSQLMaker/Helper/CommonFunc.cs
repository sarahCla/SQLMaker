using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;

namespace SQLMaker.Helper
{
    [Serializable]
    public static class CommonFuncs
    {
        public static string DelEmptyOrCommandLines(string s)
        {
            string ret = "";
            string[] lines = s.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.Trim() == "") continue;
                if (line.Trim().StartsWith("--")) continue;
                ret += (line + "\r\n");
            }
            return ret;
        }

        public static string getValueIgnoreCase(IHashObject obj, string key, string sDefault = "")
        {
            if (obj.ContainsKey(key))
                return obj[key].ToString();
            
            foreach (KeyValuePair<string, object> pair in obj)
            {
                if (pair.Key.ToUpper() == key.ToUpper())
                    return pair.Value.ToString();
            }
            return sDefault;
        }

        public static string getHashObject(IHashObject obj, string key, string sDefault="")
        {
            if (obj.ContainsKey(key))
                return obj[key].ToString();
            else
                return sDefault;
        }

        public static void addHashObject(IHashObject obj, string key, object value)
        {
            if (obj.Keys.Contains(key))
                obj[key] = value;
            else
                obj.Add(key, value);
        }
        public static int GetMonthDiff(DateTime dtBegin, DateTime dtEnd)
        {
            int Month = 0;

            if ((dtEnd.Year - dtBegin.Year) == 0)
            {
                Month = dtEnd.Month - dtBegin.Month;
            }
            if ((dtEnd.Year - dtBegin.Year) >= 1)
            {
                if (dtEnd.Month - dtBegin.Month < 0)
                {
                    Month = (dtEnd.Year - dtBegin.Year - 1) * 12 + (12 - dtBegin.Month) + dtEnd.Month + 1;
                }
                else
                {
                    Month = (dtEnd.Year - dtBegin.Year) * 12 + dtEnd.Month - dtBegin.Month + 1;
                }
            }
            else
            {
                Month++;
            }

            return Month;
        }

    }
}
