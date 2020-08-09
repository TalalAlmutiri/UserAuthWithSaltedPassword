using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UserAuthWithSaltedPassword
{
    public partial class Registration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Password must be at least 8 characters, password must contain an uppercase letter, password must contain a number
            string pattern = @"^(?=.{8,20}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$";
            if (!Regex.IsMatch(txtPassword.Value, pattern))
            {
                lbMsg.Text = "Invalid password format";
            }
            else
            {
                // Generate a random salt
                string salt = CryptographyHelper.GenerateRandomSalt(32);

                // Hashing password and salt
                string pwd = CryptographyHelper.CreateSHAHashWithSalt(txtPassword.Value, salt);

                // Encrypt the salt before storing to the database for more security 
                string encryptedSalt = CryptographyHelper.Encrypt(salt);
               
                if (Users.Insert(txtUsername.Value,pwd, encryptedSalt))
                    lbMsg.Text = "Inserted successfully";
                else
                    lbMsg.Text = "Error";
            }
        }
    }
}