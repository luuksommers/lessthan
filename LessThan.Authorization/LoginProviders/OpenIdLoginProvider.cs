using System;
using System.Collections.Specialized;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace LessThan.Authorization.LoginProviders
{
    public abstract class OpenIdLoginProvider : ILoginProvider
    {
        private readonly OpenIdRelyingParty _openid = new OpenIdRelyingParty();

        protected abstract string OpenIdUrl { get; }

        public string Login(string callBackUrl)
        {
            var response = _openid.GetResponse();
            if (response == null)
            {
                // Stage 2: user submitting Identifier
                Identifier id;
                if (Identifier.TryParse(OpenIdUrl, out id))
                {
                    try
                    {
                        var callBackUri = new Uri(callBackUrl);
                        var request = _openid.CreateRequest(OpenIdUrl, new Realm(callBackUri), callBackUri);

                        var fetch = new FetchRequest();
                        fetch.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                        fetch.Attributes.AddRequired(WellKnownAttributes.Name.First);
                        fetch.Attributes.AddRequired(WellKnownAttributes.Name.Middle);
                        fetch.Attributes.AddRequired(WellKnownAttributes.Name.Last);
                        fetch.Attributes.AddRequired(WellKnownAttributes.Name.FullName);
                        fetch.Attributes.AddRequired(WellKnownAttributes.Name.Alias);
                        request.AddExtension(fetch);

                        request.RedirectToProvider();
                        return null;
                    }
                    catch (ProtocolException ex)
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public User CallBack(string callBackUrl, NameValueCollection queryParameters)
        {
            var response = _openid.GetResponse();
            if (response != null)
            {
                // Stage 3: OpenID Provider sending assertion response
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        var fetch = response.GetExtension<FetchResponse>();

                        var email = fetch.GetAttributeValue(WellKnownAttributes.Contact.Email);
                        var fullname = fetch.GetAttributeValue(WellKnownAttributes.Name.FullName);
                        var first = fetch.GetAttributeValue(WellKnownAttributes.Name.First);
                        var last = fetch.GetAttributeValue(WellKnownAttributes.Name.Last);
                        return new User { ClaimedIdentifier = response.ClaimedIdentifier.ToString(), EmailAddress = email, Name = (fullname ?? (first + " " + last)) };

                    case AuthenticationStatus.Canceled:
                        throw new Exception("Geannuleerd bij provider");
                    case AuthenticationStatus.Failed:
                        throw new Exception(response.Exception.Message);
                }
                throw new Exception("Wattafck");
            }
            return null;
        }
    }
}
