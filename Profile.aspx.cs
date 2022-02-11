using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Profile : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        string fname = "";
        string lname = "";
        DateTime dob;
        byte[] Img;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    string email = Session["LoggedIn"].ToString();
                    SqlConnection connection = new SqlConnection(MYDBConnectionString);
                    string sql = "select FirstName, LastName, Email, Password, PasswordSalt, DateOfBirth, Image FROM Accounts WHERE Email=@EMAIL";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@EMAIL", email);
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["FirstName"] != null)
                                {
                                    if (reader["FirstName"] != DBNull.Value)
                                    {
                                        fname = reader["FirstName"].ToString();
                                    }
                                }

                                if (reader["LastName"] != null)
                                {
                                    if (reader["LastName"] != DBNull.Value)
                                    {
                                        lname = reader["LastName"].ToString();
                                    }
                                }

                                if (reader["DateOfBirth"] != null)
                                {
                                    if (reader["DateOfBirth"] != DBNull.Value)
                                    {
                                        dob = (DateTime)reader["DateOfBirth"];
                                    }
                                }

                                if (reader["Image"] != null)
                                {
                                    if (reader["Image"] != DBNull.Value)
                                    {
                                        Img = (byte[])reader["Image"];
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }

                    lbl_namec.Visible = true;
                    lbl_name.Visible = true;
                    lbl_name.Text = fname + " " + lname;
                    lbl_dobc.Visible = true;
                    lbl_dob.Visible = true;
                    string dobstr = dob.Date.ToShortDateString();
                    lbl_dob.Text = dobstr;
                    imagesrc.Visible = true;
                    string imgstr = Convert.ToBase64String(Img);
                    imagesrc.ImageUrl = "data:Image/;base64," + imgstr;
                    lbl_opwdc.Visible = true;
                    tb_opwd.Visible = true;
                    lbl_npwdc.Visible = true;
                    tb_npwd.Visible = true;
                    btnLogout.Visible = true;
                    btnUpdate.Visible = true;
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Request.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Request.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Request.Cookies["AuthToken"].Value = string.Empty;
                Request.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (checkPassword(tb_npwd.Text) < 4)
            {
                lbl_pwdchk.Visible = true;
                lbl_pwdchk.Text = "Invalid Password";
            }
            else
            {
                string email = Session["LoggedIn"].ToString();
                string pwd = tb_opwd.Text.ToString().Trim();

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
                            string npwd = tb_npwd.Text.ToString().Trim();

                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];

                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);

                            string pwdWithSalt = npwd + salt;
                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(npwd));
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                            finalHash = Convert.ToBase64String(hashWithSalt);

                            RijndaelManaged cipher = new RijndaelManaged();
                            cipher.GenerateKey();
                            Key = cipher.Key;
                            IV = cipher.IV;

                            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE Accounts SET Password = @Password, PasswordSalt = @PasswordSalt WHERE Email=@EMAIL"))
                                {
                                    cmd.Parameters.AddWithValue("@EMAIL", email);
                                    using (SqlDataAdapter sda = new SqlDataAdapter())
                                    {
                                        cmd.CommandType = System.Data.CommandType.Text;
                                        cmd.Parameters.AddWithValue("@Password", finalHash);
                                        cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                        cmd.Connection = con;
                                        con.Open();
                                        cmd.ExecuteNonQuery();
                                        con.Close();
                                    }
                                }
                            }
                        }
                        else
                        {
                            lbl_pwdchk.Visible = true;
                            lbl_pwdchk.Text = "Invalid Password";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Update Successful!');", true);
            }   
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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
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
    }
}