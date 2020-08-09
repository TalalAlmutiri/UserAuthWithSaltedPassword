# UserAuthWithSaltedPassword
A web application using ASP.net and Csharp to implement  salt hashing for protecting user passwords on user registration and login. 
This app work to add a salt to a user password then hashing the salt with the password as one input. Finally, the hashcode encrypted using AES Advanced Encryption Standard before storing to the database

A cryptographic salt is made up of random bits added to each password instance before its hashing. It works to force the passwords uniqueness, increase their complexity without increasing user requirements, and to mitigate password attacks like rainbow tables

ref: https://auth0.com/blog/adding-salt-to-hashing-a-better-way-to-store-passwords/

![Salted](https://user-images.githubusercontent.com/62042702/89741807-15e0dc00-da9d-11ea-94cb-e75daae08b5f.png)

لمزيد م المعلومات
https://3alam.pro/talal-almutairi/articles/user-protection-with-salted-password-hashing


Database script

    CREATE TABLE [dbo].[UsersInfo](
      [Username] [varchar](20) NOT NULL,
      [HashedPwd] [varchar](200) NULL,
      [Salt] [varchar](100) NULL,
     CONSTRAINT [PK_UsersInfo] PRIMARY KEY CLUSTERED 
    (
      [Username] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    GO


CryptographyHelper.cs

     public class CryptographyHelper
    {
        private static string ByteToHexString(byte[] Data)
        {
            StringBuilder sBuilder = new StringBuilder();
            try
            {
                int i;
                for (i = 0; i <= Data.Length - 1; i++)
                    sBuilder.Append(Data[i].ToString("x2"));
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return "";
            }
            return sBuilder.ToString();
        }

        public static string CreateSHAHashWithSalt(string Password, string Salt)
        {
            try
            {
                SHA512 sha512 = new SHA512CryptoServiceProvider();
                byte[] hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(Password + Salt));
                return ByteToHexString(hash);
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return "";
            }
        }

        public static string GenerateRandomSalt(int KeyLength)
        {
            try
            {
                byte[] data = new byte[KeyLength];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(data);
                }

                // The length of 32 chars will be 44 according to base64 string.
                // Base64 formula to calculate output = CEILING.MATH(4*n/3)
                // for 32 chars: CEILING.MATH(4*32/3) = 43
                // if Len(output) % 4 != 0 then the base64 will add (padding) '=' until equals Len(output) % 4 = 0
                return Convert.ToBase64String(data);

            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return "";
            }
        }
        //128
        private static readonly string keyStr = ConfigurationManager.AppSettings["Key"];
        private static readonly string ivStr = ConfigurationManager.AppSettings["IV"];

        // AES Advanced Encryption Standard
        public static string Encrypt(string plainText)
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes(keyStr);
            byte[] iV = ASCIIEncoding.ASCII.GetBytes(ivStr);
            byte[] cipher;
            // Create a new RijndaelManaged.    
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Key = key;
                aes.IV = iV;
  
                ICryptoTransform encryptorTran = aes.CreateEncryptor(aes.Key, aes.IV);
                // Create MemoryStream    
                using (MemoryStream mStream = new MemoryStream())
                {
                    // Creating a stream that links data streams to cryptographic transformations.
                    using (CryptoStream crystm = new CryptoStream(mStream, encryptorTran, CryptoStreamMode.Write))
                    {
                        // writing data to a stream    
                        using (StreamWriter sw = new StreamWriter(crystm))
                            sw.Write(plainText);
                        cipher = mStream.ToArray();
                    }
                }
            }
  
            return (Convert.ToBase64String(cipher));
        }

        internal static string Decrypt(string cipherText)
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes(keyStr);
            byte[] iV = ASCIIEncoding.ASCII.GetBytes(ivStr);
            string plaintext = null;
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Key = key;
                aes.IV = iV;
                ICryptoTransform encryptorTran = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] cipher = Convert.FromBase64String(cipherText);
                using (MemoryStream mStream = new MemoryStream(cipher))
                {
                    using (CryptoStream crystm = new CryptoStream(mStream, encryptorTran, CryptoStreamMode.Read))
                    {
                        // Read  a stream    
                        using (StreamReader reader = new StreamReader(crystm))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
    
 
 Registration Form
 
 ![Reg](https://user-images.githubusercontent.com/62042702/89742040-082c5600-da9f-11ea-999a-dab995d2a854.png)
 
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
          
  Login Form
  
  ![Log](https://user-images.githubusercontent.com/62042702/89742054-27c37e80-da9f-11ea-890e-6a9211b14b09.png)

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
