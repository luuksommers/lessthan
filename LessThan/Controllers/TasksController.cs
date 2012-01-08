using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LessThan.Engine;
using LessThan.Models;
using SignalR;

namespace LessThan.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        public ActionResult Index()
        {
            using(var db = new LessThanDatabase())
            {
                return View(db.Tasks.ToArray());
            }
        }

        public JsonResult GetAll()
        {
            using (var db = new LessThanDatabase())
            {
                return Json(db.Tasks.ToArray().Select(a => a.ToJson()).ToArray(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ToggleTimer(int id, bool start)
        {
            using (var db = new LessThanDatabase())
            {
                var task = db.Tasks.FirstOrDefault(x => x.Id == id);
                if (task != null)
                {
                    //task.IsStarted = start;
                    if (start)
                    {
                        //task.DateStarted = DateTime.Now;
                    }
                    else
                    {
                        //if (task.DateStarted.HasValue)
                        {
                            //var diff = DateTime.Now - task.DateStarted.Value;
                            //var timespent = (int) Math.Round(diff.TotalMinutes);

                            //task.TimeSpent = timespent;
                            //task.DateStarted = null;
                        }
                    }
                    return Json(task.TimeSpent);
                }
                return Json(false);
            }
        }

        [HttpPost]
        public EmptyResult ToggleComplete(int id, bool done)
        {
            using (var db = new LessThanDatabase())
            {
                var task = db.Tasks.FirstOrDefault(x => x.Id == id);
                if (task != null)
                {
                    task.IsDone = done;
                    db.SaveChanges();
                }
            }

            return new EmptyResult();
        }

        [HttpPost]
        public EmptyResult SaveTime(int id, int minutes)
        {
            using (var db = new LessThanDatabase())
            {
                var task = db.Tasks.FirstOrDefault(x => x.Id == id);
                if (task != null)
                {
                    task.TimeSpent = new TimeSpan(0, minutes, 0);
                    db.SaveChanges();
                }
            }

            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult Add(string rawtask)
        {
            var task = TaskBuilder.CreateTask(rawtask);

            using (var db = new LessThanDatabase())
            {
                task.DateCreated = DateTime.Now;
                task.CreatorId = db.UserProfileLogins.First(a => a.UniqueIdentifier == User.Identity.Name).UserProfileId ?? 0;
                db.Tasks.AddObject(task);
                db.SaveChanges();
            }

            IConnection connection = Connection.GetConnection<SignalREndPoint>();
            connection.Broadcast(new { func = "Add", task });

            return Json(task.ToJson());
        }

        public JsonResult GetAlreadyUsedProjects()
        {
            return Json(new {projects = new string[] {"LessThan", "Izr", "Wonen"}}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssignedTo()
        {
            return Json(new { projects = new string[] { "Thijs", "Luuk" } }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(int id, string user)
        {
            IConnection connection = Connection.GetConnection<SignalREndPoint>();
            connection.Broadcast(new { func = "Update", task = id, user });
            return Content(string.Empty);
        }

        public ActionResult TimerStart(int id, string user)
        {
            IConnection connection = Connection.GetConnection<SignalREndPoint>();
            connection.Broadcast(new { func = "TimerStart", task = id, user });
            return Content(string.Empty);
        }
    }
}
