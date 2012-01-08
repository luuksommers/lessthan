using System.Linq;
using System.Web.Mvc;
using LessThan.Models;

namespace LessThan.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        public ActionResult Index()
        {
            using (var dc = new LessThanDatabase())
            {
                var userProfileLogin = dc.UserProfileLogins.First(a => a.UniqueIdentifier == User.Identity.Name);
                return View(userProfileLogin.UserProfile.UserProfileTeams.Select(a=>a.Team));
            }
        }
    }
}
