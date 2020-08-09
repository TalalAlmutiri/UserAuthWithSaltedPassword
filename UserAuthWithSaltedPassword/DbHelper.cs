using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace UserAuthWithSaltedPassword
{
    public class DbHelper
    {
        // You need to change connection string from Web.config
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DbConnecion"].ConnectionString;
        /// <summary>
        /// Execute Select query and return results as a DataTable
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteQuery(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {
            DataTable table = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    {
                        con.Open();
                        cmd.CommandType = cmdType;

                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());

                return null;
            }
            return table;
        }
        /// <summary>
        ///  Executes a SQL statement and returns the number of rows affected. NonQuery (Insert, update, and delete)
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>bool</returns>
        public static bool ExecuteNonQuery(string cmdText, CommandType cmdType, SqlParameter[] parameters)
        {
            var value = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    {
                        con.Open();
                        cmd.CommandType = cmdType;
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        value = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return false;
            }
            if (value < 0)
                return false;
            else
                return true;
        }
    }
}