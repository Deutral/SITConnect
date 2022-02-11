using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(tb_email.Text, "^([A-Za-z0-9_\\-\\.])+\\@([A-Za-z0-9_\\-\\.])+\\.([A-Za-z]{2,4})$") || checkPassword(tb_pwd.Text) < 4)
            {
                lbl_check.Text = "Invalid Email or Password!";
                return;
            }
            else
            {
                if (ValidateCaptcha())
                {
                    string email = tb_email.Text.ToString().Trim();
                    string pwd = tb_pwd.Text.ToString().Trim();

                    SHA512Managed hashing = new SHA512Managed();
                    string dbHash = getDBHash(email);
                    string dbSalt = getDBSalt(email);

                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdSalt = pwd + dbSalt;
                            byte[] hashSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt));
                            string emailHash = Convert.ToBase64String(hashSalt);

                            if (emailHash.Equals(dbHash))
                            {
                                Session["LoggedIn"] = email;
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                Response.Redirect("Profile.aspx", false);
                            }
                            else
                            {
                                lbl_check.Text = "Invalid Email or Password!";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { }
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx", false);
        }

        protected string getDBHash(string email)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Password FROM Accounts WHERE Email=@EMAIL";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@EMAIL", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Password"] != null)
                        {
                            if (reader["Password"] != DBNull.Value)
                            {
                                h = reader["Password"].ToString();
                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string email)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordSalt FROM Accounts WHERE Email=@EMAIL";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@EMAIL", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
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
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LfF8WweAAAAAJcee_nhr591TLtOa0geTjBzJuCq &response=" + captchaResponse);

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
            }
            catch (WebException ex)
            {
                throw ex;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}