using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using My_Blog.Models;
using My_Blog.Utilities;
using PagedList;
using PagedList.Mvc;


namespace My_Blog.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr).Where(b => b.Published).OrderByDescending(b => b.Created);

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var listPosts = db.Posts.AsQueryable();
            var allBlogPosts = db.Posts.Where(b => b.Published).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize);
            
            return View(blogList.ToPagedList(pageNumber, pageSize));
            
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminIndex()
        {
           var allBlogPosts = db.Posts.OrderByDescending(b => b.Created).ToList();

           return View(allBlogPosts);
        }

        public IQueryable<BlogPost> IndexSearch(string searchstr)
        {
            IQueryable<BlogPost> result = null;
            if (searchstr != null)
            {
                result = db.Posts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchstr) || p.Body.Contains(searchstr) || p.Comments.Any(c => c.Body.Contains(searchstr) || c.Author.FirstName.Contains(searchstr) || c.Author.LastName.Contains(searchstr) || c.Author.DisplayName.Contains(searchstr) || c.Author.Email.Contains(searchstr)));
            }
            else
            {
                result = db.Posts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);

        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug ==slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Title,Body,Published, MediaUrl")] BlogPost blogPost, HttpPostedFileBase image)
        {
            var slug = StringUtilities.MakeSlug(blogPost.Title);

            if (slug != blogPost.Slug)
            {
                if (string.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid Title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The Title Must Be Unique");
                    return View(blogPost);
                }
            }

            if (ModelState.IsValid)
            {
                if (ImgUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads"), fileName));
                    blogPost.MediaUrl = "/Uploads" + fileName;
                }
            }

            if (ModelState.IsValid)
            {
                //var slugMaker = new StringUtilities();
                //var slug = slugMaker.MakeSlug(blogPost.Title);

                //class name, property name, give data = slug
                blogPost.Slug = slug;
                blogPost.Created = DateTimeOffset.Now;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Slug,Body,MediaUrl,Published,Created")] BlogPost blogPost, HttpPostedFileBase image)
        {
            var newSlug = StringUtilities.MakeSlug(blogPost.Title);

            if (newSlug != blogPost.Slug)
            {
                if (string.IsNullOrWhiteSpace(newSlug))
                {
                    ModelState.AddModelError("Title", "Invalid Title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.Slug == newSlug))
                {
                    ModelState.AddModelError("Title", "The Title Must Be Unique");
                    return View(blogPost);
                }
            }

            if (ModelState.IsValid)
            {
                if (ImgUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads"), fileName));
                    blogPost.MediaUrl = "/Uploads" + fileName;
                }
            }

            if (ModelState.IsValid)
            {
                db.Entry(blogPost).State = EntityState.Modified;
                blogPost.Updated = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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
