<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registration</title>
    <script src="https://www.google.com/recaptcha/api.js?render=KEY"></script>
    <script type="text/javascript">
        function validate() {
            var fname = document.getElementById('<%=tb_fname.ClientID%>').value;

            if (fname == "") {
                document.getElementById("lbl_fnamechk").innerHTML = "Invalid First Name";
                document.getElementById("lbl_fnamechk").style.color = "Red";
            } else {
                document.getElementById("lbl_fnamechk").innerHTML = "Valid!";
                document.getElementById("lbl_fnamechk").style.color = "Green";
            }

            var lname = document.getElementById('<%=tb_lname.ClientID%>').value;

            if (lname == "") {
                document.getElementById("lbl_lnamechk").innerHTML = "Invalid Last Name";
                document.getElementById("lbl_lnamechk").style.color = "Red";
            } else {
                document.getElementById("lbl_lnamechk").innerHTML = "Valid!";
                document.getElementById("lbl_lnamechk").style.color = "Green";
            }

            var ccinfo = document.getElementById('<%=tb_ccinfo.ClientID%>').value;

            if (ccinfo.length != 16) {
                document.getElementById("lbl_ccchk").innerHTML = "Invalid Credit Card Number";
                document.getElementById("lbl_ccchk").style.color = "Red";
            } else {
                document.getElementById("lbl_ccchk").innerHTML = "Valid!";
                document.getElementById("lbl_ccchk").style.color = "Green";
            }

            var email = document.getElementById('<%=tb_email.ClientID%>').value;
            var expr = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            if (!expr.test(email)) {
                document.getElementById("lbl_emailchk").innerHTML = "Invalid Email Address";
                document.getElementById("lbl_emailchk").style.color = "Red";
            } else {
                document.getElementById("lbl_emailchk").innerHTML = "Valid!";
                document.getElementById("lbl_emailchk").style.color = "Green";
            }

            var dob = document.getElementById('<%=tb_dob.ClientID%>').value
            if (dob == "") {
                document.getElementById("lbl_dobchk").innerHTML = "Invalid Date of Birth";
                document.getElementById("lbl_dobchk").style.color = "Red";
            } else {
                document.getElementById("lbl_dobchk").innerHTML = "Valid!";
                document.getElementById("lbl_dobchk").style.color = "Green";
            }

            var img = document.getElementById('<%=ImgUpload.ClientID%>').value
            var expr = /^(([A-Za-z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))+(.jpeg|.jpg|.png)$/;
            if (!expr.test(img)) {
                document.getElementById("lbl_imgchk").innerHTML = "Invalid File Format";
                document.getElementById("lbl_imgchk").style.color = "Red";
            } else {
                document.getElementById("lbl_imgchk").innerHTML = "Valid!";
                document.getElementById("lbl_imgchk").style.color = "Green";
            }

            var pwd = document.getElementById('<%=tb_pwd.ClientID%>').value;

            if (pwd.length < 12) {
                document.getElementById("lbl_pwdchk").innerHTML = "Minimum Password Length of 12 Characters.";
                document.getElementById("lbl_pwdchk").style.color = "Red";
                return ("too_short");
            } else if (pwd.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchk").innerHTML = "Password require at least 1 number";
                document.getElementById("lbl_pwdchk").style.color = "Red";
                return ("no_number");
            } else if (pwd.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchk").innerHTML = "Password require at least 1 lowercase letter";
                document.getElementById("lbl_pwdchk").style.color = "Red";
                return ("no_lowercase");
            } else if (pwd.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchk").innerHTML = "Password require at least 1 uppercase letter";
                document.getElementById("lbl_pwdchk").style.color = "Red";
                return ("no_uppercase");
            } else if (pwd.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("lbl_pwdchk").innerHTML = "Password require at least 1 special character";
                document.getElementById("lbl_pwdchk").style.color = "Red";
                return ("no_specialchar");
            } else {
                document.getElementById("lbl_pwdchk").innerHTML = "Valid!";
                document.getElementById("lbl_pwdchk").style.color = "Green";
            }
        }
    </script>
</head>
<body>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('KEY', { action: 'Register' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            })
        })
    </script>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
                <tr>
                    <td>First Name</td>
                    <td><asp:TextBox ID="tb_fname" runat="server" onkeyup="javascript:validate()"></asp:TextBox></td>
                    <td><asp:Label ID="lbl_fnamechk" runat="server" Text="x"></asp:Label></td>
                </tr>
                <tr>
                    <td>Last Name</td>
                    <td><asp:TextBox ID="tb_lname" runat="server" onkeyup="javascript:validate()"></asp:TextBox></td>
                    <td><asp:Label ID="lbl_lnamechk" runat="server" Text="x"></asp:Label></td>
                </tr>
                <tr>
                    <td>Credit Card Info</td>
                    <td><asp:TextBox ID="tb_ccinfo" runat="server" onkeyup="javascript:validate()"></asp:TextBox></td>
                    <td><asp:Label ID="lbl_ccchk" runat="server" Text="x"></asp:Label></td>
                </tr>
                <tr>
                    <td>Email</td>
                    <td><asp:TextBox ID="tb_email" runat="server" onkeyup="javascript:validate()"></asp:TextBox></td>
                    <td><asp:Label ID="lbl_emailchk" runat="server" Text="x"></asp:Label></td>
                </tr>
                <tr>
                    <td>Password</td>
                    <td><asp:TextBox ID="tb_pwd" runat="server" onkeyup="javascript:validate()"></asp:TextBox></td>
                    <td><asp:Label ID="lbl_pwdchk" runat="server" Text="x"></asp:Label></td>
                </tr>
                <tr>
                    <td>Date of Birth</td>
                    <td><asp:TextBox ID="tb_dob" runat="server" TextMode="Date" onkeyup="javascript:validate()"></asp:TextBox></td>
                    <td><asp:Label ID="lbl_dobchk" runat="server" Text="x"></asp:Label></td>
                </tr>
                <tr>
                    <td>Photo</td>
                    <td><asp:FileUpload ID="ImgUpload" runat="server" onkeyup="javascript:validate()"></asp:FileUpload></td>
                    <td><asp:Label ID="lbl_imgchk" runat="server" Text="x"></asp:Label></td>
                </tr>
                <tr>
                    <td><input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" /></td>
                    <td><asp:button id="btnsubmit" runat="server" text="register" Width="250px" onclick="btnSubmit_Click" /></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
