<%@ Page Language="C#"  %>

<!DOCTYPE html>
<script runat="server">
    public void Payment_Click(object sender, EventArgs e)
    {
        Direct_ePayment.src.main.DirectTransaction directTransaction = new Direct_ePayment.src.main.DirectTransaction();
        //Direct_ePayment.src.main.DirectTransactionResponse directTransactionResponse = new Direct_ePayment.src.main.DirectTransactionResponse();

        //FIRST THIS ONE
        directTransaction.doPost();

        //THEN THIS ONE
        //directTransactionResponse.doPost();

        System.Diagnostics.Debug.WriteLine("Information parsed, now redirecting to ReceiverPage.aspx.");
        Response.Redirect("Response.aspx");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        DATEQ.DataBind();
    }

    private string CleanDate()
    {
        string dayAdd = "";
        string monthAdd = "";
        string yearAdd = "";
        if (System.DateTime.UtcNow.Day < 10)
            dayAdd = "0";
        if (System.DateTime.UtcNow.Month < 10)
            monthAdd = "0";
        if (System.DateTime.UtcNow.Year < 10)
            yearAdd = "0";
        return (dayAdd + System.DateTime.UtcNow.Day.ToString() + monthAdd + System.DateTime.UtcNow.Month.ToString() + yearAdd + System.DateTime.UtcNow.Year.ToString());
    }
</script>


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<link href="Content/Style/ePayment-sdk-direct.css" rel="stylesheet" type="text/css" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin-left: 40px">
    <div id="topContent">
        Direct ePayment Test Example
    </div>
    <br />
    <br />
    <div id="EnvironnementDiv">
        <span id="EnvironnementText">Environnement</span>
        <asp:DropDownList ID="dropdownEnvlist" runat="server" Width="200px">
            <asp:ListItem Selected="True" Text="Preproduction" Value="preprod"></asp:ListItem>
            <asp:ListItem Text="Production" Value="prod"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <section id="sectionForDiv">
        <div id="InputBoxes">
            Site<span class="ParamNames"><asp:TextBox ID="SITE" runat="server" type="number" Width="200px" size="7" maxlength="7">1999887</asp:TextBox></span>
            <br /><br />
            Rang<span class="ParamNames"><asp:TextBox ID="RANG" runat="server" type="number" Width="200px" size="2" maxlength="2">43</asp:TextBox></span>
            <br /><br />
            Clé<span class="ParamNames"><asp:TextBox ID="CLE" runat="server" Width="200px" size="10" maxlength="10">1999887I</asp:TextBox></span>
            <br /><br />
            Version<span class="ParamNames"><asp:TextBox ID="VERSION" runat="server" type="number" Width="200px" size="5" maxlength="5">00104</asp:TextBox></span>
            <br /><br />
            Montant<span class="ParamNames"><asp:TextBox ID="MONTANT" runat="server" type="number" Width="200px" size="10" maxlength="10">10.00</asp:TextBox></span>
            <br /><br />
            Devise<span class="ParamNames"><asp:TextBox ID="DEVISE" runat="server" type="number" Width="200px">978</asp:TextBox></span>
            <br /><br />
            Référence<span class="ParamNames"><asp:TextBox ID="REFERENCE" runat="server" type="text" Width="200px" size="40" maxlength="100" >TestTransaction</asp:TextBox></span>
            <br /><br />
            Porteur<span class="ParamNames"><asp:TextBox ID="PORTEUR" runat="server" type="number" Width="200px">4974014889415605</asp:TextBox></span>
            <br /><br />
            DateVal<span class="ParamNames"><asp:TextBox ID="DATEVAL" runat="server" type="number" Width="200px">1217</asp:TextBox></span>
            <br /><br />
            CVV<span class="ParamNames"><asp:TextBox ID="CVV" runat="server" type="number" Width="200px">123</asp:TextBox></span>
            <br /><br />
            Activité<span class="ParamNames">
                        <asp:DropDownList ID="ACTIVITE" runat="server" Height="16px" Width="200px">
                            <asp:ListItem Text="Internet" Value="024" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Reccuring" Value="027"></asp:ListItem>
                            <asp:ListItem Text="MO/TO" Value="021"></asp:ListItem>
                            <asp:ListItem Text="Vide" Value="020"></asp:ListItem>
                        </asp:DropDownList>
                    </span>
            <br /><br />
            DATEQ (JJMMAAAA)<span class="ParamNames"><asp:TextBox ID="DATEQ" runat="server" Width="200px"  size="14" maxlength="14" Text="<%# CleanDate() %>" Height="20px"></asp:TextBox></span>
            <br /><br />
            <asp:HiddenField ID="PAYS" runat="server" Value=""/>
            <asp:HiddenField ID="HASH" runat="server" Value="SHA512"/>
            To include HMAC<asp:RadioButtonList  RepeatDirection="Horizontal" ID="chkHmac" runat="server">
                                <asp:ListItem Text="Yes" Selected="False"></asp:ListItem>
                                <asp:ListItem Text="No" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
            <br /><br />
            Numéro d'appel<span class="ParamNames"><asp:TextBox ID="NUMAPPEL" runat="server" type="number" Width="200px" Text="0018587948"></asp:TextBox></span>
            <br /><br />
            Numéro de transaction<span class="ParamNames"><asp:TextBox ID="NUMTRANS" runat="server" type="number" Width="200px" Text="0008503351"></asp:TextBox></span>
            <br /><br />
            Authorisation<span class="ParamNames"><asp:TextBox ID="AUTORISATION" runat="server" type="text" Width="200px"></asp:TextBox></span>
            <br /><br />
            Deferred Payment<asp:RadioButtonList  RepeatDirection="Horizontal" ID="chkDefPay" runat="server">
                                <asp:ListItem Text="Yes" ></asp:ListItem>
                                <asp:ListItem Text="No" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
            <br />
            Différé<span class="ParamNames"><asp:TextBox ID="DIFFERE" runat="server" type="number" Width="200px" Text="001"></asp:TextBox></span>
            <br /><br />
            To include new parameters
            <asp:RadioButtonList  RepeatDirection="Horizontal" ID="chkParam" runat="server" >
                <asp:ListItem Text="Yes"></asp:ListItem>
                <asp:ListItem Text="No" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>
            <br />
            Type de Carte<span class="ParamNames"><asp:DropDownList ID="TYPECARTE" runat="server" Height="16px" Width="200px">
                                <asp:ListItem Selected="True" Text="CB"></asp:ListItem>
                                <asp:ListItem Text="VISA"></asp:ListItem>
                                <asp:ListItem Text="EUROCARD_MASTERCARD"></asp:ListItem>
                                <asp:ListItem Text="ELECTRON"></asp:ListItem>
                                <asp:ListItem Text="MAESTRO"></asp:ListItem>
                                <asp:ListItem Text="VPAY"></asp:ListItem>
                            </asp:DropDownList>
                        </span>
            <br /><br />
            Selection<span class="ParamNames">
                        <asp:DropDownList ID="SELECTION" runat="server" Height="17px" Width="200px">
                            <asp:ListItem Selected="True" Text="Par défaut" value="00"></asp:ListItem>
                            <asp:ListItem Text="Par le porteur" value="01"></asp:ListItem>
                        </asp:DropDownList>
                    </span>
            <br /><br />
            Email porteur<span class="ParamNames"><asp:TextBox ID="EMAILPORTEUR" runat="server" type="email" Width="200px">test@exemple.com</asp:TextBox></span>
            <br /><br />
            <div style="border:1px solid black">
                <br /><br />
                Type de question<asp:DropDownList ID="TYPE" runat="server" Height="16px" Width="200px">
                                    <asp:ListItem Selected="True" Text="Autorisation" Value="00001"></asp:ListItem>
                                    <asp:ListItem Text="Débit" Value="00002"></asp:ListItem>
                                    <asp:ListItem Text="Autorisation + Débit" Value="00003"></asp:ListItem>
                                    <asp:ListItem Text="Crédit" Value="00004"></asp:ListItem>
                                    <asp:ListItem Text="Annulation" Value="00005"></asp:ListItem>
                                    <asp:ListItem Text="Modification d'un montant" Value="00013"></asp:ListItem>
                                    <asp:ListItem Text="Remboursement" Value="00014"></asp:ListItem>
                                    <asp:ListItem Text="Vérification" Value="00011"></asp:ListItem>
                                    <asp:ListItem Text="Consultation" Value="00017"></asp:ListItem>
                                </asp:DropDownList>
                <asp:Button ID="Button1" runat="server" Text="Payment" Width="200px" OnClick="Payment_Click"/>
                <br /><br />
            </div>
        </div>
    </section>
    </form>
</body>
</html>
