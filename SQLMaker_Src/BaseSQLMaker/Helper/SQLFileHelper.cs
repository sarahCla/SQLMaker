using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SQLMaker.Helper
{
    public static class SQLFileHelper
    {
        public static string getMakerClass(String sqlPath)
        {
            StreamReader objReader = new StreamReader(sqlPath);
            string sLine = "";
            string className = "";
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    if (sLine.Trim().StartsWith("--")) continue;
                    if (sLine.Trim().IndexOf("{classname=") >= 0)
                    {
                        className = sLine.Substring(sLine.Trim().IndexOf("{classname=")+11);
                        className = className.Replace("}", "");
                        break;
                    }
                }
            }
            objReader.Close();
            return className;
        }

        public static void getProcsInner(FileSystemInfo info, List<String> sqls, List<String> classes)
        {
            DirectoryInfo dir = info as DirectoryInfo;
            FileSystemInfo[] files;

            try
            {
                files = dir.GetFileSystemInfos();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            String sfile = "";
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                if (file != null)
                {
                    if ((file.FullName.Substring(file.FullName.LastIndexOf(".")).ToLower().Equals(".sql")) &&
                        (!getMakerClass(file.FullName).Equals("")))
                    {
                        sqls.Add(file.FullName);
                        classes.Add(SQLFileHelper.getMakerClass(file.FullName));
                    }
                }
                else
                    getProcsInner(files[i], sqls, classes);
            }

        }

        public static void getAllProc(String path, List<String> sqls, List<String> classes)
        {
            sqls.Clear();
            classes.Clear();

            DirectoryInfo info = new DirectoryInfo(path);
            getProcsInner(info, sqls, classes);
        }

    }
}
