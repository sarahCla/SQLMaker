using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DBPaging
{
    public static class HighLight
    {
        //给关键字上色

        public static void setHighLight(object sender)
        {
            RichTextBox rtb = sender as RichTextBox;
            int index = rtb.SelectionStart;    //记录修改的位置
            rtb.SelectAll();
            rtb.SelectionColor = Color.Black;
            Font OldFont = rtb.SelectionFont;
            Font fn = new Font("宋体", 9, FontStyle.Regular);
            string[] keystr ={ "select ", "from ", "where ", "and ", " or ", "order ", " by ", " desc ", "when ", "case ",
                               " then ", " end ", " on ", " in ", " is ", " else ", " left ", " join ", " not ", " null ",
                               "SELECT ", "FROM ", "WHERE ",  "WHERE "," AND ", " IS "," NULL "," LIKE "," as "," AS ","LWF", "WITH "};
            for (int i = 0; i < keystr.Length; i++)
                getBunch(keystr[i], rtb.Text, rtb);
            GetComments(rtb);
            rtb.Select(index, 0);     //返回修改的位置
            rtb.SelectionColor = Color.Black;
            rtb.SelectionFont = fn;
        }

        /*
        private static int getVarBunch(RichTextBox rtb)
        {
            int cnt = 0;
            string sText = rtb.Text;
            int sTextLength = sText.Length;
            for (int i = 0; i < sTextLength - keystrLen + 1; i++)
            {
                int j;
                for (j = 0; j < keystrLen; j++)
                {
                    if (ss[i + j] != pp[j]) break;
                }
                if (j == keystr.Length)
                {
                    rtb.Select(i, keystr.Length);
                    rtb.SelectionColor = Color.Blue;
                    cnt++;
                }
            }
            return cnt;
        }
         */

        private static int getBunch ( string keystr , string sText , RichTextBox rtb ) 
        {
            int cnt = 0, keystrLen = keystr.Length, sTextLength = sText.Length;
            char[] ss = sText.ToCharArray();
            char[] pp = keystr.ToCharArray();
            if ( keystrLen > sTextLength ) return 0;
            for ( int i = 0 ; i < sTextLength - keystrLen + 1 ; i++ )
            {
                int j;
                for ( j = 0 ; j < keystrLen ; j++ )
                {
                    if (!ss[i+j].ToString().ToUpper().Equals(pp[j].ToString().ToUpper())) break;
                }
                if (j == keystr.Length)
                {
                    rtb.Select (i , keystr.Length);
                    rtb.SelectionColor = Color.Blue;
                    cnt++;
                }
            }
            return cnt;
        }

        private static void GetComments(RichTextBox rtb)
        {
            int iNumber = 0, iShowSeat = 0;
            Font newFont = new Font("宋体", 9, FontStyle.Italic);
            string[] rtbLines = rtb.Lines;

            foreach (string sTmp in rtbLines)
            {
                iShowSeat = rtb.Text.IndexOf("--");
                iNumber = sTmp.IndexOf("--");
                if (iNumber < 0)
                {
                    Font fn = new Font("宋体", 9, FontStyle.Regular);
                    int iTmp = rtb.Text.IndexOf(sTmp);
                    rtb.Select(iTmp, sTmp.Length);
                    rtb.SelectionFont = fn;
                    continue;
                }
                else
                {
                    int iLine = rtb.Text.IndexOf(sTmp);
                    rtb.Select(iLine + iNumber, sTmp.Length - iNumber);
                    rtb.SelectionColor = Color.Red;
                    rtb.SelectionFont = newFont;
                }
            }
        }
    }
}
