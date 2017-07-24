<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ok.aspx.cs" Inherits="EPAYMENT.System_ePayment.ok" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
   	<h1>Payment Information</h1>
	<hr>
	
	<% 
		/*out.println("The Payment for <b>" + request.getParameter("ref").split("PBX_2MONT")[0] + "</b>");
		out.println(" of <b>" + Double.valueOf(request.getParameter("tarif"))/100 + " EUR</b> is");*/
	%>
	<b><font color="LimeGreen">Accepted!!!!!!</font></b>
	<br><br><a href="http://localhost:49257/index_system.html">Home</a>
</body>
</html>
