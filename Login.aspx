<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <script src="https://www.google.com/recaptcha/api.js?render=KEY"></script>
    <script type="text/javascript">
        function validate() {
            var email = document.getElementById('<%=tb_email.ClientID%>').value;
            var expr = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            if (!expr.test(email)) {
                document.getElementById("lbl_emailchk").innerHTML = "Invalid Email Address";
                document.getElementById("lbl_emailchk").style.color = "Red";
            }
        }
    </script>
</head>
<body>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('KEY', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            })
        })

    </script>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
            <tr>
                <td>Email</td>
                <td><asp:TextBox ID="tb_email" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Password</td>
                <td><asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox></td>
            </tr>
            <tr>
                <td><input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" /></td>
                <td><asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />&nbsp<asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" /></td>
            </tr>
            <tr>
                <td></td>
                <td><asp:Label ID="lbl_check" runat="server" Text="x"></asp:Label></td>
            </tr>
        </table>
                        
        </div>
    </form>
</body>
</html>
