<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="SITConnect.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table class="auto-style1">
            <tr>
                <td><asp:Image ID="imagesrc" runat="server" Visible="false" /></td>
                <td></td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_namec" runat="server" Text="Name" Visible="false" /></td>
                <td><asp:Label ID="lbl_name" runat="server" Text="x" Visible="false" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_dobc" runat="server" Text="Date of Birth" Visible="false" /></td>
                <td><asp:Label ID="lbl_dob" runat="server" Text="x" Visible="false" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_opwdc" runat="server" Text="Old Password" Visible="false" /></td>
                <td><asp:TextBox ID="tb_opwd" runat="server" Visible="false" TextMode="Password" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lbl_npwdc" runat="server" Text="New Password" Visible="false" /></td>
                <td><asp:TextBox ID="tb_npwd" runat="server" Visible="false" TextMode="Password" /></td>
            </tr>
            <tr>
                <td><asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="btnLogout_Click" Visible="false" /></td>
                <td><asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" Visible="false" /></td>
            </tr>
            <tr>
                <td></td>
                <td><asp:Label ID="lbl_pwdchk" runat="server" Text="x" Visible="false" /></td>
            </tr>
        </table>
        </div>
    </form>
</body>
</html>
