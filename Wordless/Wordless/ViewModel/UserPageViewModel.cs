﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wordless.Models;

namespace Wordless.ViewModel
{
    public class UserPageViewModel
    {
        public string Keyword { get; set; }
            
        public string Title { get; set; }
        
        public int TimesPurchased { get; set; }

        public Genres Genre { get; set; }
        
        public decimal Price { get; set; }

        public List<Book> BooksResult { get; set; }

        public UserPageViewModel()
        {
            Keyword = "";

            Title = "Here the title goes";
            TimesPurchased = 0;
            Price = 0;

            BooksResult = new List<Book>();
        }
    }
}