using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ScheduleUsers.Models;

namespace ScheduleUsers.Controllers
{
    public class WorkTimeEventController : Controller
    {
    
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: WorkTimeEvent
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var workTimeEvents = db.WorkTimeEvents.Include(w => w.User);
            return View(workTimeEvents.ToList());
        }

        public ActionResult findAll()
        {
            return Json(db.WorkTimeEvents.Where(g => g.End != null).AsEnumerable().Select(e => new {
                id = e.Id,
                title = e.Title,
                description = e.Note,
                start = e.Start.ToString("yyyy-MM-dd" + "T" + "hh:mm"),
                end = e.End.Value.ToString("yyyy-MM-dd" + "T" + "hh:mm")
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: WorkTimeEvent/inValid
        public ActionResult inValid()
        {
            return View();
        }

        

        // GET: WorkTimeEvent/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkTimeEvent workTimeEvent = db.WorkTimeEvents.Find(id);
            if (workTimeEvent == null)
            {
                return HttpNotFound();
            }
            return View(workTimeEvent);
        }

        // GET: WorkTimeEvent/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }



        // POST: WorkTimeEvent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoginViewModel workTimeEvent)    // workTimeEvent grabs the login email, password, and remember me
        {
            PasswordHasher ph = new PasswordHasher();
            


            // Checks Db users for email that matches the email user typed in
            ApplicationUser dbUser = db.Users.FirstOrDefault(x => x.Email == workTimeEvent.Email);

            // If email is not in Db
            if (dbUser == null)
            {
                ModelState.AddModelError("", "Login failed!");
                return View("inValid");
            }

            // Grabs user hashed PW from Db and PW user typed in and check is they match
            var result = ph.VerifyHashedPassword(dbUser.PasswordHash, workTimeEvent.Password);
            // If PW doesn't match
            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Login failed!");
                return View("inValid");
            }
               
            // Grabs Db event that doesn't have an end value and matches user ID
            var notFinishedEvent = db.WorkTimeEvents.FirstOrDefault(x => x.Id == dbUser.Id && !x.End.HasValue);
            // If event has already been created that doesn't have an end value, update end value
            if (notFinishedEvent != null)
            {
                notFinishedEvent.End = DateTime.Now;
                db.SaveChanges();
                return View();
            }
            else
            {
                WorkTimeEvent clockIn = new WorkTimeEvent()
                {
                    User = db.Users.Where(e => e.Email == workTimeEvent.Email).First(), // Uses Db to return list of emails, then grabs the first (and only) user with that email.
                    Start = DateTime.Now,
                    EventID = Guid.NewGuid()
                };

                db.WorkTimeEvents.Add(clockIn);
                db.SaveChanges();
                return View();
            }
        }



        // GET: WorkTimeEvent/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkTimeEvent workTimeEvent = db.WorkTimeEvents.Find(id);
            if (workTimeEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Users, "Id", "FirstName", workTimeEvent.Id);
            return View(workTimeEvent);
        }

        // POST: WorkTimeEvent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,Start,End,Note,Timestamp,Title,ActiveSchedule,Id")] WorkTimeEvent workTimeEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(workTimeEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Users, "Id", "FirstName", workTimeEvent.Id);
            return View(workTimeEvent);
        }

        // GET: WorkTimeEvent/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkTimeEvent workTimeEvent = db.WorkTimeEvents.Find(id);
            if (workTimeEvent == null)
            {
                return HttpNotFound();
            }
            return View(workTimeEvent);
        }

        // POST: WorkTimeEvent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            WorkTimeEvent workTimeEvent = db.WorkTimeEvents.Find(id);
            db.WorkTimeEvents.Remove(workTimeEvent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
