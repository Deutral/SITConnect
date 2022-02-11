using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{

    public class myObject
    {
        public string success { get; set; }
        public List<string> ErrorMsg { get; set; }
    }

    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string filename = Path.GetFileName(ImgUpload.PostedFile.FileName);

            if (string.IsNullOrEmpty(tb_fname.Text) || string.IsNullOrEmpty(tb_lname.Text) || !Regex.IsMatch(tb_email.Text, "^([A-Za-z0-9_\\-\\.])+\\@([A-Za-z0-9_\\-\\.])+\\.([A-Za-z]{2,4})$") || string.IsNullOrEmpty(tb_dob.Text) || !Regex.IsMatch(filename, "^([A-Za-z0-9_\\.\\-])+(.jpeg|.jpg|.png)$") || checkPassword(tb_pwd.Text) < 4)
            {
                if (string.IsNullOrEmpty(tb_fname.Text))
                {
                    lbl_fnamechk.Text = "Invalid First Name";
                    lbl_fnamechk.ForeColor = Color.Red;
                }
                else
                {
                    lbl_fnamechk.Text = "Valid";
                    lbl_fnamechk.ForeColor = Color.Green;
                }

                if (string.IsNullOrEmpty(tb_lname.Text))
                {
                    lbl_lnamechk.Text = "Invalid Last Name";
                    lbl_lnamechk.ForeColor = Color.Red;
                }
                else
                {
                    lbl_lnamechk.Text = "Valid";
                    lbl_lnamechk.ForeColor = Color.Green;
                }

                if (tb_ccinfo.Text.Length != 16)
                {
                    lbl_ccchk.Text = "Invalid Credit Card Number";
                    lbl_ccchk.ForeColor = Color.Red;
                }
                else
                {
                    lbl_ccchk.Text = "Valid";
                    lbl_ccchk.ForeColor = Color.Green;
                }

                if (!Regex.IsMatch(tb_email.Text, "^([A-Za-z0-9_\\-\\.])+\\@([A-Za-z0-9_\\-\\.])+\\.([A-Za-z]{2,4})$"))
                {
                    lbl_emailchk.Text = "Invalid Email Address";
                    lbl_emailchk.ForeColor = Color.Red;
                }
                else
                {
                    lbl_emailchk.Text = "Valid";
                    lbl_emailchk.ForeColor = Color.Green;
                }

                if (string.IsNullOrEmpty(tb_dob.Text))
                {
                    lbl_dobchk.Text = "Invalid Date of Birth";
                    lbl_dobchk.ForeColor = Color.Red;
                }
                else
                {
                    lbl_dobchk.Text = "Valid";
                    lbl_dobchk.ForeColor = Color.Green;
                }

                if (!Regex.IsMatch(filename, "^([A-Za-z0-9_\\.\\-])+(.jpeg|.jpg|.png)$"))
                {
                    lbl_imgchk.Text = "Invalid File Format";
                    lbl_imgchk.ForeColor = Color.Red;
                }
                else
                {
                    lbl_imgchk.Text = "Valid";
                    lbl_imgchk.ForeColor = Color.Green;
                }

                int scores = checkPassword(tb_pwd.Text);
                string status = "";
                switch (scores)
                {
                    case 1:
                        status = "Very Weak";
                        break;
                    case 2:
                        status = "Weak";
                        break;
                    case 3:
                        status = "Medium";
                        break;
                    case 4:
                        status = "Strong";
                        break;
                    case 5:
                        status = "Excellent";
                        break;
                }
                lbl_pwdchk.Text = "Status: " + status;
                if (scores < 4)
                {
                    lbl_pwdchk.ForeColor = Color.Red;
                }
                else { lbl_pwdchk.ForeColor = Color.Green; }
            }
            else
            {
                if (ValidateCaptcha())
                {
                    string pwd = tb_pwd.Text.ToString().Trim();

                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    SHA512Managed hashing = new SHA512Managed();

                    string pwdWithSalt = pwd + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;

                    createAccount();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Registration Successful!'); window.location='Login.aspx';", true);
                }
            }
        }

        protected void createAccount()
        {            
            try
            {
                using (Stream fs = ImgUpload.PostedFile.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bytes = br.ReadBytes((int)fs.Length);
                        using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO Accounts VALUES(@FirstName, @LastName, @CreditCard, @Email, @Password, @PasswordSalt, @DateOfBirth, @Image)"))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                {
                                    cmd.CommandType = System.Data.CommandType.Text;
                                    cmd.Parameters.AddWithValue("@FirstName", tb_fname.Text.Trim());
                                    cmd.Parameters.AddWithValue("@LastName", tb_lname.Text.Trim());
                                    cmd.Parameters.AddWithValue("@CreditCard", encryptData(tb_ccinfo.Text.Trim()));
                                    cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Password", finalHash);
                                    cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                    cmd.Parameters.AddWithValue("@DateOfBirth", tb_dob.Text.Trim());
                                    cmd.Parameters.AddWithValue("@Image", bytes);
                                    cmd.Connection = con;
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected byte [] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }
            return score;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=KEY &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        myObject jsonObject = js.Deserialize<myObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }catch (WebException ex)
            {
                throw ex;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}