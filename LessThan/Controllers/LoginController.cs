using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LessThan.Authorization;
using LessThan.Models;

namespace LessThan.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index(string returnUrl)
        {
            return View();
        }

        public ActionResult Login(string type, string returnUrl)
        {
            var provider = LoginProviderFactory.Create(type);

            // Keep returnUrl split for debugging purpose
            var providerUrl = provider.Login(Url.Action("CallBack", "Login", new { type }, "http"));

            // OpenId Provider sets the httpresponse manually
            if (providerUrl != null)
            {
                return Redirect(providerUrl);
            }
            return Content("");
        }

        public ActionResult CallBack(string type, string returnUrl)
        {
            var provider = LoginProviderFactory.Create(type);

            var user = provider.CallBack(Url.Action("CallBack","Login", new { type }, "http"), Request.Params);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            // Check if this is only a new mapping or a new login
            if(User.Identity.IsAuthenticated)
            {
                AddUserProfileLogin(user);
            }
            else
            {
                AddUserProfile(user);
                Response.Cookies.Add(new HttpCookie("gravatar", user.EmailAddress));
                Response.Cookies.Add(new HttpCookie("name", user.Name));
                FormsAuthentication.SetAuthCookie(user.ClaimedIdentifier, true);
            }

            return RedirectToAction("Index", "Tasks");
        }

        private void AddUserProfile(User user)
        {
            using (var dc = new LessThanDatabase())
            {
                var userProfileLogin = dc.UserProfileLogins.FirstOrDefault(a => a.UniqueIdentifier == user.ClaimedIdentifier);
                if (userProfileLogin == null)
                {
                    var userProfile = new UserProfile
                                      {
                                          EmailAddress = user.EmailAddress,
                                          FullName = user.Name,
                                          UserName = user.EmailAddress
                                      };
                    dc.UserProfiles.AddObject(userProfile);
                    userProfileLogin = new UserProfileLogin
                    {
                        UniqueIdentifier = user.ClaimedIdentifier,
                        UserProfile = userProfile
                    };
                    dc.UserProfileLogins.AddObject(userProfileLogin);
                    dc.SaveChanges();
                }
            }
        }

        private void AddUserProfileLogin(User user)
        {
            using (var dc = new LessThanDatabase())
            {
                var userProfileLogin = dc.UserProfileLogins.FirstOrDefault(a => a.UniqueIdentifier == user.ClaimedIdentifier);
                if (userProfileLogin == null)
                {
                    userProfileLogin = new UserProfileLogin
                                           {
                                               UniqueIdentifier = user.ClaimedIdentifier,
                                               UserProfile = dc.UserProfiles.First(a => a.UserProfileLogins.Any(b=>b.UniqueIdentifier == User.Identity.Name))
                                           };
                    dc.UserProfileLogins.AddObject(userProfileLogin);
                    dc.SaveChanges();
                }
            }
        }
    }
}
