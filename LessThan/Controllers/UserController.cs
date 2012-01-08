using System.Linq;
using System.Web.Mvc;
using LessThan.Models;

namespace LessThan.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            using (var dc = new LessThanDatabase())
            {
                var userProfileLogin = dc.UserProfileLogins.First(a => a.UniqueIdentifier == User.Identity.Name);
                return View(userProfileLogin.UserProfile);
            }
        }

        [HttpPost]
        public ActionResult Update(string username)
        {
            using (var dc = new LessThanDatabase())
            {
                var userProfileLogin = dc.UserProfileLogins.First(a => a.UniqueIdentifier == User.Identity.Name);
                TempData["username"] = username;
                userProfileLogin.UserProfile.UserName = username;
                dc.SaveChanges();

                return Json(new { msg = "Username is bijgewerkt. <a href='" +Url.Action("Undo") + "' class='undo'>Undo</a>"  });
            }
        }

        [HttpPost]
        public ActionResult Undo()
        {
            using (var dc = new LessThanDatabase())
            {
                var userProfileLogin = dc.UserProfileLogins.First(a => a.UniqueIdentifier == User.Identity.Name);
                var originalUsername = TempData["username"] as string;
                if(!string.IsNullOrEmpty(originalUsername))
                {
                    userProfileLogin.UserProfile.UserName = originalUsername;
                    dc.SaveChanges();
                    return Json(new { msg = "Username ongedaan gemaakt" });
                }

                return Json(new { msg = "Username kon niet ongedaan worden gemaakt" });
            }
        }
    }
}
