<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ePayment.aspx.cs" Inherits="WebApplicationEpayment.System_ePayment2.ePayment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript">
        window.onload = function ()
        {
            document.systemForm.submit();
        }
    </script>
</head>
<body>
    <asp:Label ID="lb_html" runat="server"></asp:Label>
<%--    <button onclick="document.systemForm.submit()">go</button>--%>
</body>
</html>
