using System;
using System.Linq;
using log4net;
using Core.core;
using System.Web;
using System.IO;
using System.Globalization;

namespace Direct_ePayment.src.main
{
    /*
     * 
     * Class to perform all the payment operations for the DirectTransaction
     * 
     */
    public class DirectTransactionResponse
    {

        private const long serialVersionUID = 1L;
        private static readonly ILog logger = LogManager.GetLogger(typeof(DirectTransaction));
        internal LoggerCall loggerCall = new LoggerCall();

        /*
	     * Get all variables for DIRECT_ePayment 
	     * TYPE: Type of action to perform
	     * SITE: Site number provided by the bank
	     * RANG: Rank number provided by the bank
	     * CLE: an additional security key on the PPPS exchanges
	     * VERSION: Protocol Version
	     * NUMQUESTION: Unique identifier per day
	     * MONTANT: Amount in cents
	     * DEVISE: Currency type in number format
	     * REFERENCE: Command reference
	     * PORTEUR: card or token number
	     * DATEVAL: Validity date of the card
	     * CVV: CVV of bank card
	     * ACTIVITE: Sent from the flow
	     * DATEQ: Used in SQL queries to the question (DDMMYYYY)
	     * useHmac: To use HMAC or not
	     * hmacKey: Hash-based Message Authentication Code to verify the authenticity of the site vendor
	     * NUMAPPEL: Call number
	     * NUMTRANS: Transaction number
	     * AUTORISATION: authorization number
	     * HASH: Algorithm used for signing the message
	     * PAYS: Indication of the country of the card
	     * DIFFERE: Number of days for a deferred payment
	     * useParams : To use new parameters or not
	     * TYPECARTE : CB, VISA, EUROCARD_MASTERCARD, ELECTRON, MAESTRO ou VPAY
	     * SELECTION : 00 (default setting)  / 01 (In the case of a selection made by the holder/porteur).
	     * EMAILPORTEUR : E-mail, so that a payment ticket is sent to the bearer
	     */

        public void doPost()
        {
            HttpContext context = HttpContext.Current;

            try
            {
                string numQuestion = context.Session["NUMQUESTION"].ToString();

                Core.utils.CachedObject msgCo = (Core.utils.CachedObject)CacheManager.getCache(numQuestion);
                String msg = ((String)msgCo._object).ToString();

                if (msg != null)
                {
                    loggerCall.runInformation(logger, "MESSAGE passed to the server", msg);
                }
                else
                {
                    loggerCall.runError(logger, msg);
                }



                String type = context.Session["TYPE"].ToString();
                String site = context.Session["SITE"].ToString();
                String rang = context.Session["RANG"].ToString();
                String urlKey = type + site + rang;

                Core.utils.CachedObject urlCo = (Core.utils.CachedObject)CacheManager.getCache(urlKey);
                String url = ((String)urlCo._object).ToString();

                if (url != null)
                {
                    loggerCall.runInformation(logger, "The server to access", url);

                    context.Response.ContentType = "text/html;charset=UTF-8";

                    context.Response.Headers["Cache-control"] = "no-cache, no-store";
                    context.Response.Headers["Pragma"] = "no-cache";
                    context.Response.Headers["Expires"] = "-1";

                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                }
                else
                {
                    loggerCall.runError(logger, url);
                }

                Uri newurl = new Uri(url.ToString());

                System.Net.HttpWebRequest con = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                con.Method = "POST";
                //con.Timeout = 5000;

                String postParams = msg;

                Stream requestStream = con.GetRequestStream();
                StreamWriter sw = new StreamWriter(requestStream);
                sw.Write(postParams);
                sw.Flush();
                sw.Close();

                /*
                System.IO.BinaryWriter paramsWriter = new System.IO.BinaryWriter(con.GetRequestStream());
                paramsWriter.Write(postParams);
                paramsWriter.Flush();
                paramsWriter.Close();
                */

                string res = "";
                try
                {
                    Stream response = con.GetResponse().GetResponseStream();
                    StreamReader buf = new StreamReader(response);
                    res = buf.ReadToEnd();
                    buf.Close();
                }
                catch (Exception ex)
                {
                    loggerCall.runError(logger, "ERROR HERE ON THE NEW CODE");
                }

                // build HTML code
                String htmlRespone = "";
                /*
                htmlRespone += "<html>";
                htmlRespone += "<head>";
                htmlRespone += "<link href='/Direct_ePayment/css/ePayment-sdk.css' rel='stylesheet' type='text/css'>";
                htmlRespone += "<title>Test PPPS HMAC</title>";
                htmlRespone += "</head>";
                htmlRespone += "<body>";
                */
                htmlRespone += "<h1>Direct Payment Information Test example</h1>";
                htmlRespone += "<hr>";
                htmlRespone += "<h2>Response :</h2>";

                // process response
                int status = context.Response.StatusCode;

                if (status == 200)
                {
                    loggerCall.runInformation(logger, "The server response status is (valid)", status.ToString());
                    if (res.ToString() != null)
                    {
                        loggerCall.runInformation(logger, "The returned response", res);
                        htmlRespone += "<p><font face=\"verdana\" color=\"green\">";

                        String[] responseOutput = res.ToString().Split('&');
                        for (int i = 0; i < responseOutput.Length; i++)
                        {
                            htmlRespone += responseOutput[i] + "</br>";
                        }
                        htmlRespone += "</font></p>";
                    }
                }
                else
                {
                    htmlRespone += "<p><font face=\"verdana\" color=\"red\">Server Error!</font></p>";
                    loggerCall.runError(logger, res);
                }

                /*
                htmlRespone += "<br/><a href='/Direct_ePayment/index.html'>Home</a>";
                htmlRespone += "</body>";
                htmlRespone += "</html>";
                */

                // return response
                context.Session.Add("html-sdk-epayment-reponse", htmlRespone.ToString());

                loggerCall.runInformation(logger, "---------------Ending Transaction----------", "Direct");
            }
            finally
            {

            }
        }
    }
}