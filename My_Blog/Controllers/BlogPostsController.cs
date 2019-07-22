using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using My_Blog.Models;
using My_Blog.Utilities;

namespace My_Blog.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index()
        {
            var allBlogPosts = db.Posts.Where(b =>b.Published).OrderByDescending(b => b.Created).ToList();
            return View(db.Posts.ToList());
            
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult AdminIndex()
        {
            return View("Index" , db.Posts.ToList());
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
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Body,Published")] BlogPost blogPost)
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
        //[Authorize(Roles = "Admin")]
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
        public ActionResult Edit([Bind(Include = "Id,Title,Slug,Body,MediaUrl,Published,Created")] BlogPost blogPost)
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
                db.Entry(blogPost).State = EntityState.Modified;
                blogPost.Updated = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
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
