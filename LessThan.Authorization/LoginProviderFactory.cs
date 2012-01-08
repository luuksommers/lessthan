using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessThan.Authorization.LoginProviders;

namespace LessThan.Authorization
{
    public class LoginProviderFactory
    {
        private LoginProviderFactory()
        {
            
        }

        public static ILoginProvider Create(string type)
        {
            switch (type)
            {
                case "google":
                    return new GoogleOpenIdLoginProvider();
                case "facebook":
                    return new FacebookLoginProvider();
                case "windowslive":
                    return new WindowsLiveLoginProvider();
                case "twitter":
                    return new TwitterLoginProvider();
            }
            throw new Exception(string.Format("Unknown LoginProvider: {0}", type));
        }
    }
}
