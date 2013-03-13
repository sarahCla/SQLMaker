using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Carpa.Web.Script;
using SQLMaker.Helper;
using BSQLMaker.SQLServer.FuncSQLMaker;
using BSQLMaker.SQLServer.Procs;
using BSQLMaker.SQLServer.Codes;
using BSQLMaker.Const;
using System.Reflection;
using BSQLMaker.SQLServer;
using System.Text.RegularExpressions;
using System.IO;
using SQLMaker.Helper;
using BSQLMaker.SQLServer.Common;

namespace DBPaging
{
    public partial class frmSqlMakerTester : Form
    {
        private List<String> sqls = new List<string>();
        private List<String> classes = new List<string>();
        private String filePath = "";
        private String className = "";
        private String assemblyName = "BSQLMaker";

        private List<string> _userParams, _sysParams;
        private IHashObject _defaultValue;

        string conString = "";
        string procPath = "";
        //=============================================


        private void ini()
        {
            rPagerSql.Text = "";
            rSumSql.Text = "";
            rRecordCountSql.Text = "";
            rCommonSql.Text = "";
            _userParams = null;
            _sysParams = null;
            _defaultValue = null;
        }

        public frmSqlMakerTester()
        {
            InitializeComponent();
            //getAllProc();
        }

        //获取所有的存储过程
        private void getAllProc()
        {
            conString = txtConstr.Text;
            procPath = txtPath.Text;
            if (procPath.Equals("")) 
            {
                MessageBox.Show("请先设置SQL文件所在路径");
                txtPath.Focus();
                return;
            }
            if (!Directory.Exists(procPath))
            {
                MessageBox.Show("SQL文件路径不存在，请检查");
                txtPath.Focus();
                return;
            }
            ini();
            SPList.DataSource = null;
            SQLFileHelper.getAllProc(procPath, sqls, classes);
            SPList.DataSource = sqls;
            if (SPList.Items.Count > 0) SPList.SelectedItem = 1;
        }

        private BSqlMaker getSQL(Boolean bPagerMaker, IHashObject queryParams)
        {
            if (queryParams == null) queryParams = new HashObject();
            BSqlMaker maker = BMakerHelper.getMaker(conString, filePath, className, queryParams);
            try
            {
                if (bPagerMaker)
                {
                    rSumSql.Text = ((BPagerSqlMaker)maker).getSumDataSQL();
                    rRecordCountSql.Text = ((BPagerSqlMaker)maker).getRecordCountSQL();
                    rPagerSql.Text = ((BPagerSqlMaker)maker).getPageDataSQL(0, 20, true);

                }
                else
                {
                    rCommonSql.Text = maker.getSQL();
                }
            }
            catch (Exception error)
            {
                addErrList("getSQL出错：" + error.Message);
            }
            return maker;
        }


        /**
         * 获取sql文件中的“公用变量”，“默认配置参数”，“用户参数”等信息，分析出没有对应解析器的变量，参数
         **/
        private void showAllParams(string classPath, BSqlMaker rpt)
        {
            try
            {
                string baseSQL;
                dgvParamsAnalyze.RowCount = 1;
                BSqlMaker report = rpt;
                if (rpt == null) report = getMaker(classPath);
                report.getBaseSqlInfo(out baseSQL, out _userParams, out _sysParams, out _defaultValue);
                rchBaseSQL.Text = baseSQL;
                dgvParamsAnalyze.RowCount = _sysParams.Count + 1;
                int i = 0;
                foreach (string param in _sysParams)
                {
                    if (param.Trim() == "") continue;
                    dgvParamsAnalyze[1, i].Value = param;
                    dgvParamsAnalyze[0, i].Value = "公用变量";
                    dgvParamsAnalyze[2, i].Value = "";
                    dgvParamsAnalyze[3, i].Value = "";
                    dgvParamsAnalyze.Rows[i].DefaultCellStyle.BackColor = Color.Beige;
                    i++;
                }

                dgvParamsAnalyze.RowCount += _userParams.Count;
                dgvParamsAnalyze.RowCount += _defaultValue.Count;

                foreach (KeyValuePair<string, object> pair in _defaultValue)
                {
                    string param = pair.Key.Trim();
                    if (param.Equals("")) continue;
                    if (_userParams.IndexOf(param)>=0) continue;
                    dgvParamsAnalyze[1, i].Value = param;
                    dgvParamsAnalyze[2, i].Value = pair.Value;
                    dgvParamsAnalyze[0, i].Value = "默认配置参数";
                    dgvParamsAnalyze.Rows[i].DefaultCellStyle.BackColor = Color.Aqua;
                    i++;
                }
                foreach (string param in _userParams)
                {
                    if (param.Trim() == "") continue;
                    dgvParamsAnalyze[1, i].Value = param;
                    if (param.IndexOf("=") > 0) continue;
                    dgvParamsAnalyze[0, i].Value = "用户参数";
                    if (_defaultValue.ContainsKey(param))
                    {
                        dgvParamsAnalyze[2, i].Value = _defaultValue.GetValue<string>(param);
                    }
                    dgvParamsAnalyze.Rows[i].DefaultCellStyle.BackColor = Color.AntiqueWhite;
                    i++;
                }

                IHashObject qryParams = report.helper.getParamsSetted();
                bool bSet = false;
                foreach (KeyValuePair<string, object> pair in qryParams)
                {
                    bSet = false;
                    int rowcount = dgvParamsAnalyze.RowCount - 1;
                    for (int row = 0; row < rowcount; row++)
                    {
                        if (dgvParamsAnalyze[1, row].Value == null) continue;
                        if (String.Compare(dgvParamsAnalyze[1, row].Value.ToString(), pair.Key, true) != 0) continue;
                        dgvParamsAnalyze[4, row].Value = pair.Value.ToString();
                        dgvParamsAnalyze[3, row].Value = pair.Key;
                        bSet = true;
                        if (dgvParamsAnalyze[1, row].Value.ToString() != pair.Key)
                            dgvParamsAnalyze.Rows[row].DefaultCellStyle.BackColor = Color.Red;
                        break;
                    }
                    if (!bSet)
                    {
                        dgvParamsAnalyze.RowCount += 1;
                        dgvParamsAnalyze[4, i].Value = pair.Value.ToString();
                        dgvParamsAnalyze[3, i].Value = pair.Key;
                        i++;
                    }
                }

            }
            catch (Exception error)
            {
                addErrList("showBaseSQLParams出错：" + error.Message);
            }
        }

        private void ExecuteSQL(Boolean isPagerSQL)
        {
            DbHelper db = new DbHelper(conString, true);
            if (isPagerSQL)
            {
                try
                {
                    dataGridView1.DataSource = db.ExecuteSQL(rPagerSql.Text);
                }
                catch (Exception error)
                {
                    addErrList("获取分页数据SQL执行出错:" + "\n\r" + error.Message);
                }

                try
                {
                    dataGridView2.DataSource = db.ExecuteSQL(rSumSql.Text);
                }
                catch (Exception error)
                {
                    addErrList("获取合计数据SQL执行出错:" + "\n\r" + error.Message);
                }

                label4.Text = "";
                try
                {
                    label4.Text = "总行数为：" + db.ExecuteIntSQL(rRecordCountSql.Text).ToString();
                }
                catch (Exception error)
                {
                    addErrList("获取总行数SQL执行出错:" + "\n\r" + error.Message);
                }
            }
            else
            {
                dgvNoPageSql.DataSource = db.ExecuteSQL(rCommonSql.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            splitContainer3.SplitterDistance = (this.Width - 100) / 3;
            splitContainer4.SplitterDistance = (this.Width - 100) / 3;
        }

        private BSqlMaker getMaker(string classPath)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            object obj = assembly.CreateInstance(classPath);
            BSqlMaker reports = (BSqlMaker)obj;
            return reports;
        }

        
        private void showIniParams()
        {
            DataGridView grid = dgvParamsSet;
            try
            {
                addErrList("显示测试参数开始....");
                grid.RowCount = 1;
                grid.RowCount = _userParams.Count + 1;
                int i = 0;
                foreach (string param in _userParams)
                {
                    if (param.Trim() == "") continue;
                    dgvParamsAnalyze[1, i].Value = param;
                    if (param.IndexOf("=") > 0) continue;
                    grid[0, i].Value = param;
                    if (_defaultValue.ContainsKey(param))
                    {
                        grid[1, i].Value = _defaultValue.GetValue<string>(param);
                    }
                    i++;
                }
                addErrList("显示测试参数结束....");
            }
            catch (Exception error)
            {
                addErrList("showTestParams出错：" + error.Message);
            }
        }

        private void addErrList(string errMsg)
        {
            ErrList.Items.Add(errMsg);
        }

        /**
         * 界面处理，只有分页的sql才显示分页tabpage
         **/
        private void iniTabPage(Boolean bPagerMaker)
        {
            ini();
            if (bPagerMaker)
            {
                tpPageSql.Parent = tabControl1;
                tpNoPageSql.Parent = null;
                tabControl1.SelectedTab = tpPageSql;
            }
            else
            {
                tpPageSql.Parent = null;
                tpNoPageSql.Parent = tabControl1;
                tabControl1.SelectedTab = tpNoPageSql;
            }
        }

        private Boolean testProc()
        {
            try
            {
                ErrList.Items.Clear();
                addErrList("正在分析....");

                //获取sql文件中的默认参数设置，显示到表格中
                BSqlMaker maker = getMaker(className);
                maker.SQLFilePath = filePath;
                maker.setTestParams();

                Boolean bPagerMaker = BMakerHelper.isPagerMaker(className);
                iniTabPage(bPagerMaker);
                maker = getSQL(bPagerMaker, null);

                showAllParams(className, maker);
                showIniParams();
                ExecuteSQL(bPagerMaker);
                addErrList("测试完毕");

                ErrList.ForeColor = Color.Blue;
                return true;
            }
            catch (Exception error)
            {
                ErrList.ForeColor = Color.Red;
                addErrList("显示默认参数出错:" + "\n\r" + error.Message);
                return false;
            }
        }


        private void SPList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SPList.DataSource == null) return;
            filePath = sqls[SPList.SelectedIndex];
            className = classes[SPList.SelectedIndex];
            lbClassName.Text = lbClassName.Tag + className;
            testProc();
        }

        private void btnShowTestParams_Click(object sender, EventArgs e)
        {
            showIniParams();
        }

        /**
         * 根据表格中设置的参数的对应值，组装出参数哈希对象
         **/
        private IHashObject getParams()
        {
            IHashObject p = new HashObject();
            for(int i=0; i<dgvParamsSet.RowCount-1; i++)
            {
                if (dgvParamsSet[0, i].Value==null || dgvParamsSet[0, i].Value.ToString() == "") continue;
                p.Add(dgvParamsSet[0, i].Value.ToString(), (dgvParamsSet[1, i].Value==null) ? "": dgvParamsSet[1, i].Value.ToString());
            }
            return p;
        }

        private void ClipboardCopy(RichTextBox edit)
        {
            Clipboard.Clear();
            Clipboard.SetText(edit.Text);
        }

        private void btnCopy1_Click(object sender, EventArgs e)
        {
            ClipboardCopy(rPagerSql);
        }

        private void btnCopy2_Click(object sender, EventArgs e)
        {
            ClipboardCopy(rSumSql);
        }

        private void btnCopy3_Click(object sender, EventArgs e)
        {
            ClipboardCopy(rRecordCountSql);
        }

        private void ErrList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void frmSqlMakerTester_ResizeEnd(object sender, EventArgs e)
        {
            splitContainer3.SplitterDistance = (this.Width - 100) / 3;
            splitContainer4.SplitterDistance = (this.Width - 100) / 3;
        }

        /**
         * 按照表格中填入的参数值，重新执行语句
         **/
        private void btnTestNow_Click(object sender, EventArgs e)
        {
            try
            {
                ErrList.Items.Clear();
                addErrList("正在分析....");
                if (SPList.Text == "")
                {
                    addErrList("请先选择需要测试的类");
                }

                BSqlMaker maker = BMakerHelper.getMaker(conString, filePath, className, getParams());
                
                //maker.setTestParams();

                Boolean bPagerMaker = BMakerHelper.isPagerMaker(className);
                iniTabPage(bPagerMaker);
                maker = getSQL(bPagerMaker, getParams());
                maker.SQLFilePath = filePath;

                showAllParams(className, maker);
                
                //showIniParams();

                ExecuteSQL(bPagerMaker);
                addErrList("测试完毕");
            }
            catch (Exception error)
            {
                addErrList("显示默认参数出错:" + "\n\r" + error.Message);
            }
        }

        private void rchBaseSQL_TextChanged(object sender, EventArgs e)
        {
            HighLight.setHighLight(sender);
        }

        private void rchBaseSQL_BindingContextChanged(object sender, EventArgs e)
        {
            HighLight.setHighLight(sender);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            getAllProc();
        }

    }
}
