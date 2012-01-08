using System;
using System.Collections.Specialized;
using System.Configuration;

namespace LessThan.Authorization.LoginProviders
{
    public class TwitterLoginProvider : ILoginProvider
    {
        private string TwitterAppId
        {
            get
            {
                return ConfigurationManager.AppSettings["twitterAppId"];
            }
        }

        private string TwitterAppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["twitterAppSecret"];
            }
        }

        public string Login(string callbackUrl)
        {
            string redirectTo = string.Format(
    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}",
    Uri.EscapeDataString(TwitterAppId),
    Uri.EscapeDataString(callbackUrl));

            return redirectTo;
        }

        public User CallBack(string callBackUrl, NameValueCollection queryParameters)
        {
            throw new NotImplementedException();
        }
    }
}
