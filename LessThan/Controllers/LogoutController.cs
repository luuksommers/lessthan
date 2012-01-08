using System.Web.Mvc;
using System.Web.Security;

namespace LessThan.Controllers
{
    [Authorize]
    public class LogoutController : Controller
    {
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
