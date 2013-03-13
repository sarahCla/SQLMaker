using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Carpa.Web.Script;
using SQLMaker.Common;

namespace MyProject2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            rMakedSQL.Text = "";
            if (rOriginSQL.Text.Trim().Equals("")) return;
            rMakedSQL.Text = BaseMakerHelper.getSQL(rOriginSQL.Text);
        }
    }
}
