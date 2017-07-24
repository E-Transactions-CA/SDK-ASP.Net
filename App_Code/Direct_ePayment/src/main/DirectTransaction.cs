using System;
using System.Linq;
using log4net;
using Core.core;
using System.Web;
using System.Globalization;

namespace Direct_ePayment.src.main
{
    /*
     * 
     * Class to perform all the payment operations for the DirectTransaction
     * 
     */
    public class DirectTransaction : SimpleServlet
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
                string env = context.Request.Params["dropdownEnvlist"];

                ReadApplicationPropertyValues applicationPropertyValues = new ReadApplicationPropertyValues(env);

                string type = context.Request.Params["TYPE"];
                string site = (context.Request.Params.AllKeys.Contains("SITE")) ? (context.Request.Params["SITE"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.site"));
                string rang = (context.Request.Params.AllKeys.Contains("RANG")) ? (context.Request.Params["RANG"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.rang"));
                string key = (context.Request.Params.AllKeys.Contains("CLE")) ? (context.Request.Params["CLE"]) : applicationPropertyValues.getPropertyValue(env + ".direct.key");
                string version = (context.Request.Params.AllKeys.Contains("VERSION")) ? (context.Request.Params["VERSION"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.version"));
                string numquestion = context.Request.Params["NUMQUESTION"];
                string total = context.Request.Params["MONTANT"];
                string devise = (context.Request.Params.AllKeys.Contains("DEVISE")) ? (context.Request.Params["DEVISE"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.devise"));
                string reference = (context.Request.Params.AllKeys.Contains("REFERENCE")) ? (context.Request.Params["REFERENCE"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.reference"));
                string porteur = (context.Request.Params.AllKeys.Contains("PORTEUR")) ? (context.Request.Params["PORTEUR"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.porteur"));
                string dateval = (context.Request.Params.AllKeys.Contains("DATEVAL")) ? (context.Request.Params["DATEVAL"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.dateval"));
                string cvv = (context.Request.Params.AllKeys.Contains("CVV")) ? (context.Request.Params["CVV"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.cvv"));
                string activite = (context.Request.Params.AllKeys.Contains("ACTIVITE")) ? (context.Request.Params["ACTIVITE"]) : (applicationPropertyValues.getPropertyValue(env + ".direct.internet.activite"));
                string dateQ = context.Request.Params["DATEQ"];
                string useHmac = context.Request.Params["chkHmac"];
                string hmacKey = applicationPropertyValues.getPropertyValue(env + ".direct.hmac.key");
                string numAppel = context.Request.Params["NUMAPPEL"];
                string numTrans = context.Request.Params["NUMTRANS"];
                string auth = context.Request.Params["AUTORISATION"];
                string hash = context.Request.Params["HASH"];
                string useDeferredPayment = context.Request.Params["chkDefPay"];
                string differe = context.Request.Params["DIFFERE"];

                //New payment parameters for TYPE: 00001 and TYPE: 00003
                string useParams = context.Request.Params["chkParam"];
                string typeCard = context.Request.Params["TYPECARTE"];
                string selection = context.Request.Params["SELECTION"];
                string emailPorteur = context.Request.Params["EMAILPORTEUR"];

                if (string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.auth.type"), StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.auth.capture.type"), StringComparison.CurrentCultureIgnoreCase))
                {
                    generateInputsAndHash(dateQ, type, numquestion, total, site, rang, reference, version, key, devise,
                            porteur, dateval, cvv, activite, useHmac, hmacKey, null, null, null, useDeferredPayment, differe,
                            hash, useParams, typeCard, selection, emailPorteur, env, context);
                }
                // TYPE: 00004  Call for credit of the transaction
                if (string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.credit.type"), StringComparison.CurrentCultureIgnoreCase))
                {
                    generateInputsAndHash(dateQ, type, numquestion, total, site, rang, reference, version, key, devise,
                            porteur, dateval, cvv, activite, useHmac, hmacKey, null, null, null, useDeferredPayment, differe, hash,
                            null, null, null, null, env, context);
                }

                // TYPE: 00002 Call the capture or debit only of transaction 
                // TYPE: 00005 Call for the cancellation of transaction
                // TYPE: 00014 refund with numtran et numapp of transaction
                if (string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.capture.type"), StringComparison.CurrentCultureIgnoreCase)
                        || string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.cancel.type"), StringComparison.CurrentCultureIgnoreCase)
                        || string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.refund.type"), StringComparison.CurrentCultureIgnoreCase))
                {
                    generateInputsAndHash(dateQ, type, numquestion, total, site, rang, reference, version, key, devise,
                            null, null, null, null, useHmac, hmacKey, numAppel, numTrans, null, useDeferredPayment, differe, hash,
                            null, null, null, null, env, context);
                }

                // TYPE: 00013 Call to change the amount of transaction
                if (string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.modification.amount.type"), StringComparison.CurrentCultureIgnoreCase))
                {
                    generateInputsAndHash(dateQ, type, numquestion, total, site, rang, reference, version, key, devise,
                            null, null, null, null, useHmac, hmacKey, numAppel, numTrans, auth, useDeferredPayment, differe, hash,
                            null, null, null, null, env, context);
                }
                // TYPE: 00011 Verification of transaction
                if (string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.verification.type"), StringComparison.CurrentCultureIgnoreCase))
                {
                    generateInputsAndHash(dateQ, type, numquestion, total, site, rang, reference, version, key, devise, null,
                            null, null, null, useHmac, hmacKey, null, null, null, useDeferredPayment, differe, hash,
                            null, null, null, null, env, context);
                }
                // TYPE: 00017 Consultation of transaction
                if (string.Equals(type, applicationPropertyValues.getPropertyValue(env + ".direct.consultation.type"), StringComparison.CurrentCultureIgnoreCase))
                {
                    generateInputsAndHash(dateQ, type, numquestion, total, site, rang, reference, version, key, devise,
                            null, null, null, null, useHmac, hmacKey, numAppel, numTrans, null, useDeferredPayment, differe,
                            hash, null, null, null, null, env, context);
                }
                /*
                RequestDispatcher reqDispatcher = request.getRequestDispatcher("./views/ePayment.jsp");
                reqDispatcher.forward(request, response);
                 * */
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
        }

        private void generateInputsAndHash(string dateQ, string type, string numquestion, string total, string site, string rang, string reference, string version, string key,
                                            string devise, string porteur, string dateval, string cvv, string activite, string useHmac, string hmacKey, string numAppel, string numTrans,
                                            string auth, string useDeferredPayment, string differe, string hash, string useParams, string typeCard,
                                            string selection, string emailPorteur, string env, HttpContext context)
        {
            string msg = null;
            string url = this.geturl(env) + ("/PPPS.php");

            string html = "<pre>";
            html += "<form method=\"POST\" name=\"directForm\" id=\"Form\">";

            if (!string.IsNullOrEmpty(url))
            {
                Session.Add("url-sdk-epayment", url);
            }
            if (!string.IsNullOrEmpty(type))
            {
                msg = "TYPE=" + type + "";
                html += "<input type=\"hidden\" name=\"TYPE\" value=\"" + type + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "TYPE");
            }
            if (!string.IsNullOrEmpty(site))
            {
                msg += "&SITE=" + site + "";
                html += "<input type=\"hidden\" name=\"SITE\" value=\"" + site + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "SITE");
            }

            if (!string.IsNullOrEmpty(rang))
            {
                msg += "&RANG=" + rang + "";
                html += "<input type=\"hidden\" name=\"RANG\" value=\"" + rang + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "RANG");
            }

            if (!string.IsNullOrEmpty(version))
            {
                msg += "&VERSION=" + version + "";
                html += "<input type=\"hidden\" name=\"VERSION\" value=\"" + version + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "VERSION");
            }
            var ts = DateTime.UtcNow - new DateTime(2013,1,1);
            numquestion = Math.Round(ts.TotalMilliseconds / 100.0f).ToString();

            if (!string.IsNullOrEmpty(numquestion))
            {
                msg += "&NUMQUESTION=" + numquestion + "";
                html += "<input type=\"hidden\" name=\"NUMQUESTION\" value=\"" + numquestion + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "NUMQUESTION");
            }

            Decimal amount = 0;
            if (!string.IsNullOrEmpty(total))
            {
                float priceParsed = 0;

                float.TryParse(total, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out priceParsed);

                amount = (Decimal)(priceParsed * 100.0f);
                msg += "&MONTANT=" + amount + "";
                html += "<input type=\"hidden\" name=\"MONTANT\" value=\"" + amount + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "MONTANT");
            }
            if (!string.IsNullOrEmpty(devise))
            {
                msg += "&DEVISE=" + devise + "";
                html += "<input type=\"hidden\" name=\"DEVISE\" value=\"" + devise + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "DEVISE");
            }

            if (!string.IsNullOrEmpty(reference))
            {
                msg += "&REFERENCE=" + reference + "";
                html += "<input type=\"hidden\" name=\"REFERENCE\" value=\"" + reference + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "REFERENCE");
            }

            if (!string.IsNullOrEmpty(porteur))
            {
                msg += "&PORTEUR=" + porteur + "";
                html += "<input type=\"hidden\" name=\"PORTEUR\" value=\"" + porteur + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "PORTEUR");
            }

            if (!string.IsNullOrEmpty(dateval))
            {
                msg += "&DATEVAL=" + dateval + "";
                html += "<input type=\"hidden\" name=\"DATEVAL\" value=\"" + dateval + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "DATEVAL");
            }
            if (!string.IsNullOrEmpty(cvv))
            {
                msg += "&CVV=" + cvv + "";
                html += "<input type=\"hidden\" name=\"CVV\" value=\"" + cvv + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "CVV");
            }

            if (!string.IsNullOrEmpty(activite))
            {
                msg += "&ACTIVITE=" + activite + "";
                html += "<input type=\"hidden\" name=\"ACTIVITE\" value=\"" + activite + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "ACTIVITE");
            }

            if (!string.IsNullOrEmpty(dateQ))
            {
                msg += "&DATEQ=" + dateQ + "";
                html += "<input type=\"hidden\" name=\"DATEQ\" value=\"" + dateQ + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "DATEQ");
            }

            msg += "&PAYS=";
            html += "<input type=\"hidden\" name=\"PAYS\" value=\"\" />";

            if (!string.IsNullOrEmpty(numAppel))
            {
                msg += "&NUMAPPEL=" + numAppel + "";
                html += "<input type=\"hidden\" name=\"NUMAPPEL\" value=\"" + numAppel + "\" />";
            }
            else
            {
                msg += "&NUMAPPEL=";
                html += "<input type=\"hidden\" name=\"NUMAPPEL\" value=\"\" />";
                loggerCall.runError(logger, "NUMAPPEL");
            }

            if (!string.IsNullOrEmpty(numTrans))
            {
                msg += "&NUMTRANS=" + numTrans + "";
                html += "<input type=\"hidden\" name=\"NUMTRANS\" value=\"" + numTrans + "\" />";
            }
            else
            {
                msg += "&NUMTRANS=";
                html += "<input type=\"hidden\" name=\"NUMTRANS\" value=\"\" />";
                loggerCall.runError(logger, "NUMTRANS");
            }

            //		authorisation
            if (!string.IsNullOrEmpty(auth))
            {
                msg += "&AUTORISATION=" + auth + "";
                html += "<input type=\"hidden\" name=\"AUTORISATION\" value=\"" + auth + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "AUTORISATION");
            }

            //		Call Deferred Payment
            if ("YES".Equals(useDeferredPayment, StringComparison.CurrentCultureIgnoreCase) && differe != null && !differe.Length.Equals(0))
            {
                msg += "&DIFFERE=" + differe + "";
                html += "<input type=\"hidden\" name=\"DIFFERE\" value=\"" + differe + "\" />";
            }
            else
            {
                loggerCall.runError(logger, "DIFFERE");
            }

            //New payment parameters for TYPE: 00001 and TYPE: 00003
            if ("YES".Equals(useParams, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(typeCard))
                {
                    msg += "&TYPECARTE=" + typeCard + "";
                    html += "<input type=\"hidden\" name=\"TYPECARTE\" value=\"" + typeCard + "\" />";
                }
                else
                {
                    loggerCall.runError(logger, "TYPECARTE");
                }

                if (!string.IsNullOrEmpty(selection))
                {
                    msg += "&SELECTION=" + selection + "";
                    html += "<input type=\"hidden\" name=\"SELECTION\" value=\"" + selection + "\" />";
                }
                else
                {
                    loggerCall.runError(logger, "SELECTION");
                }

                if (!string.IsNullOrEmpty(emailPorteur))
                {
                    msg += "&EMAILPORTEUR=" + emailPorteur + "";
                    html += "<input type=\"hidden\" name=\"EMAILPORTEUR\" value=\"" + emailPorteur + "\" />";
                }
                else
                {
                    loggerCall.runError(logger, "EMAILPORTEUR");
                }
            }

            //		HMAC generation call
            if ("YES".Equals(useHmac, StringComparison.CurrentCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(hash))
                {
                    msg += "&HASH=" + hash + "";
                    html += "<input type=\"hidden\" name=\"HASH\" value=\"" + hash + "\" />";
                }
                else
                {
                    loggerCall.runError(logger, "HASH");
                }
                if (!string.IsNullOrEmpty(hmacKey))
                {
                    CreateHmacSignature createHmacSignature = new CreateHmacSignature();
                    string hmac = (createHmacSignature.buildHmacSignature(msg, hmacKey)).ToLower();
                    if (!string.IsNullOrEmpty(hmac))
                    {
                        msg += "&HMAC=" + hmac + "";
                        html += "<input type=\"hidden\" name=\"HMAC\" value=\"" + hmac + "\" />";
                    }
                    else
                    {
                        loggerCall.runError(logger, " HMAC generated value");
                    }
                }
                else
                {
                    loggerCall.runError(logger, " HMAC key");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(key))
                {
                    msg += "&CLE=" + key + "";
                    html += "<input type=\"hidden\" name=\"CLE\" value=\"" + key + "\" />";
                }
                else
                {
                    loggerCall.runError(logger, "CLE");
                }
            }
            html += "</form>";
            html += "</pre>";

            Session.Add("html-sdk-epayment", html);

            Core.utils.CachedObject msgCo = new Core.utils.CachedObject(msg, numquestion, 0);
            /* Place the object into the cache! */
            CacheManager.putCache(msgCo);

            String urlKey = type + site + rang;
            Core.utils.CachedObject urlCo = new Core.utils.CachedObject(url, urlKey, 0);
            /* Place the object into the cache! */
            CacheManager.putCache(urlCo);

            //context.Request.Params.Set("NUMQUESTION", numquestion);
            Session.Add("NUMQUESTION", numquestion);
            Session.Add("TYPE", type);
            Session.Add("SITE", site);
            Session.Add("RANG", rang);

            loggerCall.runInformation(logger, "HTML passed to the server", html);
            loggerCall.runInformation(logger, "MESSAGE passed to the server", msg);
        }

        /* 
        * Get the server url to access 
        * the DirectTransaction options
        */
        private String geturl(String env)
        {
            ConnectionUtils connectionUtils = new ConnectionUtils();
            connectionUtils.checkConnection(env + ".direct.server.url.", env);
            return connectionUtils.getHttpsUrl();
        }
    }
}