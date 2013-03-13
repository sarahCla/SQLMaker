using System;
using System.Collections.Generic;
using System.Text;
using Carpa.Web.Script;
using BSQLMaker.Const;
using SQLMaker.Helper;
using SQLMaker;
using SQLMaker.Pager;

namespace BSQLMaker.SQLServer.FuncSQLMaker
{
    [Serializable]
    class SortSQLMaker : FunctionStrMaker
    {
        private string sortField;
        private string sortJoin;

        public SortSQLMaker(IHashObject qryParams, SQLHelper helper, string conString) : base(qryParams, helper, conString)
        {
        }
  
        protected override void registTags()
        {
            tags = new HashObject(new string[] { FuncTags.SORT_INFO, FuncTags.SORT_JOIN }, new string[] { sortField, sortJoin });
        }

        private string getKeyField(string baseInfoTable)
        {
            return "typeid";
        }

        protected override void getTagSQL()
        {
            sortJoin = "";
            sortField = CommonFuncs.getValueIgnoreCase(queryParams, SParam.SORT_FIELD);
            string defaultSet = CommonFuncs.getValueIgnoreCase(queryParams, SParam.SORT_DEFAULT);
            string baseInfoTable = CommonFuncs.getValueIgnoreCase(queryParams, SParam.SORT_BASE_TABLE).Trim();
            string sortType = CommonFuncs.getValueIgnoreCase(queryParams, SParam.SORT_TYPE);
            string sortKeyField = CommonFuncs.getValueIgnoreCase(queryParams, SParam.SORT_KEY_FIELD);
            if ((sortField == "") && (defaultSet != ""))
            {
                string[] sets = defaultSet.Split(',');
                baseInfoTable = sets[0];
                sortField = sets[1];
                if (sets.Length>2) sortType = sets[2];
                if (sets.Length>3) sortKeyField = sets[3];
            }
            if (sortField == "") return;

            string baseKeyField = "";
            if (baseInfoTable!="") baseKeyField = getKeyField(baseInfoTable);

            if (sortType.Trim() != "") sortType = " " + sortType;

            if (baseInfoTable!="")
            {
                
                sortField = baseInfoTable + "." + sortField;
                
                sortJoin = " LEFT JOIN " + baseInfoTable + " ON Data." + sortKeyField + " = " + baseInfoTable + "." + baseKeyField;
            }
            else
            {
                sortField += sortType;
            }
        }




    }
}
