<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Response.aspx.cs" Inherits="EPAYMENT.ReceiverPage" %>

<!DOCTYPE html>
<script runat="server">
    public void CallDirectTransactionResponse()
    {
        Direct_ePayment.src.main.DirectTransactionResponse directTransactionResponse = new Direct_ePayment.src.main.DirectTransactionResponse();

        directTransactionResponse.doPost();
        //        <%CallDirectTransactionResponse();%>

    }

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<script type="text/javascript" 	src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript">

    window.onload = function(){

        loadHTMLtags();

        <%CallDirectTransactionResponse();%>

        loadHTMLReponse();
    }

    function loadHTMLtags() {
        var html = '<%=Session["html-sdk-epayment"]%>';
        document.getElementById("Form").innerHTML = html.valueOf();
    }

    function loadHTMLReponse() {
        var html = '<%=Session["html-sdk-epayment-reponse"]%>';
        document.getElementById("reponse").innerHTML += html.valueOf();
    }

    </script>
</head>
<body>

    <form name="directForm" id="Form" runat="server">

    </form>
    <output id="reponse" style="display:block; width:100%; border:solid 1px black;">&nbsp;</output>
</body>
</html>
