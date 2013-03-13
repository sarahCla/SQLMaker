using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using Carpa.Common;

namespace Carpa.Web.Script
{
    public class DbHelper
    {
        private SqlConnection connection;
        private SqlCommand command;

        public DbHelper(string connString, bool fieldNameToLower)
        {
            connection = new SqlConnection();
            connection.ConnectionString = connString;
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command = new SqlCommand("", connection);
        }

        public DataTable ExecuteSQL(string sql)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;

                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public int ExecuteIntSQL(string sql)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;

                object answer = command.ExecuteScalar();
                if (answer != null)
                {
                    Type type = answer.GetType();
                    if (type == typeof(int) || type == typeof(long) ||
                        type == typeof(uint) || type == typeof(Int16))
                    {
                        return answer != DBNull.Value ? Convert.ToInt32(answer) : 0;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                connection.Close();
            }
        }

        public int execsql(string sqlstr)
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("error");
                throw;
            }
        }

        public DataSet getDataSet(string sqlstr)
        {
            DataSet sl = new DataSet();
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(sqlstr, connection);
                sda.Fill(sl);
                return sl;

            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("error");
                throw;
            }
        }


        public string execsqlpro(string u, string p) //执行存储过程。
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userid", SqlDbType.Char, 50);
            command.Parameters.Add("@passwd", SqlDbType.Char, 50);
            command.Parameters["@userid"].Value = u;
            command.Parameters["@passwd"].Value = p;
            command.Parameters.Add("@out", SqlDbType.Char, 1);
            command.Parameters["@out"].Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();
            return command.Parameters["@out"].Value.ToString();

        }


    }
}
