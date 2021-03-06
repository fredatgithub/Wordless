﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wordless.Models;
using System.Data.Entity;
using Wordless.ViewModel;

namespace Wordless.Controllers
{
    public class AuthorPageController : Controller
    {
        WordlessContext db = new WordlessContext();
        
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            SetListsForViews();
            Book book = new Book();
            AuthorPageViewModel author = new AuthorPageViewModel();

            //ViewBag.List = author.BooksResult.ToList();

            author.BooksResult = book.GetAll();
            
            return View(author);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(AuthorPageViewModel author)
        {
            SetListsForViews();
            if (ModelState.IsValid)
            {
                Book book = new Book();

                var books = book.GetAll();
               
                if (!String.IsNullOrEmpty(author.Keyword))
                {
                    books = books.Where(b => b.Title.ToLower().Contains(author.Keyword.ToLower())).ToList();
                }
                author.BooksResult = books;
            }
            return View(author);
        }

        public ActionResult Details(int id)
        {
            Book book = db.Books.Single(b => b.BookId == id);
            
            return View(book);
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetListsForViews();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book, HttpPostedFileBase upload)
        {
            SetListsForViews();
            if (ModelState.IsValid)
            {
                var userId = (Int64)Session["currentUserId"];
                var user = db.Users.Find(userId);
                var newFile = new File
                {
                    FileName = System.IO.Path.GetFileName(upload.FileName),
                    ContentType = upload.ContentType,
                    User = user,
                    UploadedOn = DateTime.Now

                };
                if (newFile.ContentType == "application/pdf")
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        newFile.Content = reader.ReadBytes(upload.ContentLength);
                    }
                    db.Files.Add(newFile);
                }
                
                book.File = newFile;
                book.Author = user;
                db.Books.Add(book);   
                db.SaveChanges();
                return Redirect("Create");               
            }
            return View(book);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Book book = db.Books.Single(b => b.BookId == id);

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                Book bookFromDB = db.Books.Single(b => b.BookId == book.BookId);

                bookFromDB.Title = book.Title;
                bookFromDB.TimesPurchased = book.TimesPurchased;
                bookFromDB.Genre = book.Genre;
                bookFromDB.Price = book.Price;
                bookFromDB.Comments = book.Comments;

                db.SaveChanges();

                return RedirectToAction("Details", new { id = book.BookId });
            }
            return View(book);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Book book = db.Books.Single(b => b.BookId == id);

            if (book == null)
            {
                return HttpNotFound();
            }
            
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Book book = db.Books.Single(b => b.BookId == id);
            db.Books.Remove(book);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public void SetListsForViews()
        {
            WordlessContext db = new WordlessContext();
            //sparar mest nedladdade i en lista
            ViewBag.MostDownloaded = (from d in db.Books
                                      orderby d.TimesPurchased descending
                                      select d).Take(4).ToList();
            var listFromDb = db.PurchasedBooks.Include(u => u.Buyer).ToList();
            List<PurchasedBook> purchasedList = new List<PurchasedBook>();
            //räknar ut snittet på rating
            foreach (var item in listFromDb)
            {
                //hur många gånger en bok har blvit köpt
                var times = db.PurchasedBooks.Where(t => t.BookId == item.BookId).Count();
                if (times > 1 && purchasedList.Any(f => f.BookId == item.BookId) == false)
                {
                    //totala summan av alla ratings
                    var sum = db.PurchasedBooks.Where(t => t.BookId == item.BookId).Sum(t => t.Rating);
                    //summan delat på antalet nedladdingar(förutsatt att alla har gett en rating :) ) 
                    var avgRating = sum / times;
                    item.Rating = avgRating;
                }
                if (purchasedList.Any(f => f.BookId == item.BookId) == false)
                {
                    purchasedList.Add(item);
                }
                //sorterar listan
                var sortedList = (from x in purchasedList
                                  orderby x.Rating descending
                                  select x).Take(4).ToList();
                ViewBag.BestRating = sortedList;
                //skapar en lista av antalet kommentarer
                ViewBag.MostCommented = (from b in db.Books
                                         orderby b.Comments.Count() descending
                                         select b).Take(4).ToList();
            }
        }
    }
}