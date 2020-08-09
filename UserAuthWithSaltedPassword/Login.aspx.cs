using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UserAuthWithSaltedPassword
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Password must be at least 8 characters, password must contain an uppercase letter, password must contain a number
            string pattern = @"^(?=.{8,20}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$";
            if (!Regex.IsMatch(txtPassword.Value, pattern))
            {
                lbMsg.Text = "Invalid password format";
            }
            else
            {
                Users users = Users.GetUserInfo(txtUsername.Value);
                if (users == null)
                    lbMsg.Text = "Username or password incorrect";
                else
                {
                    // Decrypt the salt
                    string decryptedSalt = CryptographyHelper.Decrypt(users.UserSalt);

                    string loginSaltedPwd = CryptographyHelper.CreateSHAHashWithSalt(txtPassword.Value, decryptedSalt);

                    if (String.Compare(loginSaltedPwd.Trim(), users.UserPwd.Trim(), false) == 0)
                        lbMsg.Text = "Login successful";
                    else
                        lbMsg.Text = "Username or password incorrect";
                }
            }
        }
    }
}