using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace LessThan.Authorization.LoginProviders
{
    public class FacebookLoginProvider : ILoginProvider
    {
        private string FacebookAppId
        {
            get
            {
                return ConfigurationManager.AppSettings["facebookAppId"];
            }
        }

        private string FacebookAppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["facebookAppSecret"];
            }
        }

        public string Login(string callbackUrl)
        {
            string redirectTo = string.Format(
                "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope=email",
                Uri.EscapeDataString(FacebookAppId),
                Uri.EscapeDataString(callbackUrl));

            return redirectTo;
        }

        public User CallBack(string callbackUrl, NameValueCollection queryParameters)
        {
            string code = queryParameters["code"];
            string errorReason = queryParameters["error_reason"];

            if (code != null)
            {
                try
                {
                    // Construct a request for an access token.
                    WebRequest tokenRequest = WebRequest.Create(string.Format(
                        "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                        Uri.EscapeDataString(FacebookAppId),
                        Uri.EscapeDataString(callbackUrl),
                        Uri.EscapeDataString(FacebookAppSecret),
                        Uri.EscapeDataString(code)));
                    tokenRequest.Method = "GET";


                    // Send the request and get the response.
                    WebResponse tokenResponse = tokenRequest.GetResponse();
                    string tokenResponseText = new StreamReader(tokenResponse.GetResponseStream()).ReadLine();
                    NameValueCollection tokenResponseData = System.Web.HttpUtility.ParseQueryString(tokenResponseText);

                    var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + Uri.EscapeDataString(tokenResponseData["access_token"]));
                    using (var response = request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var graph = OAuthGraph.Deserialize(responseStream);
                            return new User{ ClaimedIdentifier = graph.Id, EmailAddress = graph.EmailAddress, Name = graph.Name};
                        }
                    }
                }
                catch (WebException webException)
                {
                    string responseBody = null;
                    if (webException.Response != null)
                    {
                        using (var sr = new StreamReader(webException.Response.GetResponseStream(), Encoding.UTF8))
                        {
                            responseBody = sr.ReadToEnd();
                        }
                    }
                    throw new Exception( string.Format("Failure occurred contacting graph service: Response={0}", responseBody) );
                }
                catch
                {
                    throw new Exception(string.Format("Failed to get access token. Ensure that the verifier token provided is valid."));
                }
            }

            throw new Exception(string.Format("Dat ging in ieder geval niet goed: {0}", errorReason));
        }
    }
}
