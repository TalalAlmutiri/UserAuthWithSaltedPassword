using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace UserAuthWithSaltedPassword
{
    public class Users
    {
        private string username;
        private string userPwd;
        private string userSalt;

        public string Username { get => username; set => username = value; }
        public string UserPwd { get => userPwd; set => userPwd = value; }
        public string UserSalt { get => userSalt; set => userSalt = value; }

        public static Users GetUserInfo(string InputUsername)
        {
            try
            {
                string sql = @"SELECT * FROM UsersInfo WHERE Username=@Username";
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter ("@Username",InputUsername),
                 };

                DataTable table = DbHelper.ExecuteQuery(sql, CommandType.Text, parameters);
                // Check if the user exists
                if (table.Rows.Count <= 0)
                    return null;
                else
                {
                    Users users = new Users();
                    users.Username = table.Rows[0]["Username"].ToString();
                    users.UserPwd = table.Rows[0]["HashedPwd"].ToString();
                    users.UserSalt = table.Rows[0]["Salt"].ToString();

                    return users;
                }
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return null;
            }
        }

        public static bool Insert(string Username, string Pwd, string Salt)
        {
            try
            {
                // Inserting a new user to a database
                string sql = @"INSERT INTO UsersInfo VALUES(@Username,@Pwd,@Salt)";
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter ("@Username",Username),
                    new SqlParameter ("@Pwd",Pwd),
                    new SqlParameter ("@Salt",Salt),
                 };

                if (DbHelper.ExecuteNonQuery(sql, CommandType.Text, parameters))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return false;
            }
        }

    }
}