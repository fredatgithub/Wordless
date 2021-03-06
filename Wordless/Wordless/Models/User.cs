﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Wordless.Models
{
    
    public class User
    {
        public Int64 UserId { get; set; }         //användarens ID 
        
        [MinLength(2), MaxLength(15)]               
        public string Username { get; set; }    //användarens användarnamn (2-15)

        [MinLength(2), MaxLength(15)]
        public string Password { get; set; }    //användarens lösenord      (2-15)  
        public string Name { get; set; }        //användarens namn (max 20)
        public string Email { get; set; }       //användarens e-mail
        public bool Author { get; set; }        //om användaren är en författaren
        public bool Admin { get; set; }         //om användaren är admin
        public decimal Funds { get; set; }      //en användarens pengar (precision 4,2- 1234.56)

        //Navigation Properties
        public virtual IList<PurchasedBook> PurchasedBooks { get; set; }    //lista av böcker användaren har köpt
        public virtual IList<Book> WrittenBooks { get; set; }               //lista av böcken användaren har skrivit        
        public virtual IList<Comment> Comments { get; set; }                //list av kommentarer användaren har skrivit
        public virtual IList<File> Files { get; set; }


        public List<User> GetAll()
        {
            WordlessContext db = new WordlessContext();

            return db.Users.ToList();
        }
        
    }
}