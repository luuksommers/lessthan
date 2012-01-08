using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace LessThan.Authorization.LoginProviders
{
    public class WindowsLiveLoginProvider : ILoginProvider
    {
        private string LiveIdAppId
        {
            get
            {
                return ConfigurationManager.AppSettings["liveIdAppId"];
            }
        }

        private string LiveIdAppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["liveIdAppSecret"];
            }
        }

        public string Login(string callbackUrl)
        {
            string redirectTo = string.Format(
                "https://oauth.live.com/authorize?client_id={0}&scope=wl.basic%20wl.emails&response_type=token&redirect_uri={1}",
                Uri.EscapeDataString(LiveIdAppId),
                Uri.EscapeDataString(callbackUrl));

            return redirectTo;
        }

        public User CallBack(string callbackUrl, NameValueCollection queryParameters)
        {
            string code = queryParameters["code"];

            if (code != null)
            {
                try
                {
                    // Construct a request for an access token.
                    WebRequest tokenRequest = WebRequest.Create(string.Format(
                        "https://oauth.live.com/token?client_id={0}&redirect_uri={1}& client_secret={2}&code={3}&grant_type=authorization_code.",
                        Uri.EscapeDataString(LiveIdAppId),
                        Uri.EscapeDataString(callbackUrl),
                        Uri.EscapeDataString(LiveIdAppSecret),
                        Uri.EscapeDataString(code)));
                    tokenRequest.Method = "GET";

                    // Send the request and get the response.
                    WebResponse tokenResponse = tokenRequest.GetResponse();

                    string tokenResponseText = new StreamReader(tokenResponse.GetResponseStream()).ReadLine();
                    NameValueCollection tokenResponseData = System.Web.HttpUtility.ParseQueryString(tokenResponseText);

                    var request = WebRequest.Create("https://apis.live.net/v5.0/me?access_token=" + Uri.EscapeDataString(tokenResponseData["access_token"]));
                    using (var response = request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var graph = OAuthGraph.Deserialize(responseStream);
                            return new User { ClaimedIdentifier = graph.Id, EmailAddress = graph.EmailAddress, Name = graph.Name };
                        }
                    }

                    return new User();
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
                    throw new Exception(string.Format("Failure occurred contacting consent service: Response={0}", responseBody));
                }
                catch
                {
                    throw new Exception("Failed to get access token. Ensure that the verifier token provided is valid.");
                }
            }

            throw new Exception("Dat ging in ieder geval niet goed.");
        }
    }
}
