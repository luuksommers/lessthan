namespace LessThan.Authorization.LoginProviders
{
    public class GoogleOpenIdLoginProvider : OpenIdLoginProvider
    {
        protected override string OpenIdUrl
        {
            get { return "https://www.google.com/accounts/o8/id"; }
        }
    }
}
