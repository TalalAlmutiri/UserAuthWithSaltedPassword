<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UserAuthWithSaltedPassword.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <h4 class="text-center pt-2 font-weight-bold">Login Form</h4>
        <div class="card text-left mr-auto ml-auto p-2 " style="width: 70%">
            <div>
                <label class="form-labe font-weight-bold pt-2" for="txtName">Username:</label>
                <input id="txtUsername" runat="server" type="text" required="required" class="form-control bg-white" maxlength="10" />
            </div>
            <div>
                <label class="form-label font-weight-bold pt-2" for="txtLastName">Password:</label>
                <input id="txtPassword" runat="server" type="password" required="required" class="form-control bg-white" maxlength="20" />
            </div>
            <div class="text-center pt-2">
                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-dark btn-md btn-rounded font-weight-bold text-capitalize " OnClick="btnLogin_Click" />
            </div>
        </div>
        <div class="text-center pt-2">
            <asp:Label ID="lbMsg" runat="server" CssClass="text-danger"></asp:Label>
        </div>
    </form>
</body>
</html>
