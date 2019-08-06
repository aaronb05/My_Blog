using My_Blog.Models;
using My_Blog.ViewModels;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;


namespace My_Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? page)
        {                        
            var posts = db.Posts.Where(b => b.Published).OrderByDescending(b => b.Created).Take(6).ToList();
            
            var myLandingPageVM = new LandingPageVM
            {
                PostOne = posts.FirstOrDefault(),
                PostTwo = posts.Skip(1).FirstOrDefault(),
                PostThree = posts.Skip(2).FirstOrDefault(),
                PostFour = posts.Skip(3).FirstOrDefault(),
                PostFive = posts.Skip(4).FirstOrDefault(),
                PostSix = posts.LastOrDefault()
            };

            return View(myLandingPageVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminIndex()
        {
            return View("AdminIndex", db.Posts.ToList());
        }

       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View("About");
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var from = model.FromName + ":  " + $"{ model.FromEmail}<{ConfigurationManager.AppSettings["emailto"]}>";
                    //var body = model.Body;
                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject,
                        Body = model.Body,
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

    }
}

      