using My_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Blog.ViewModels
{
    public class LandingPageVM
    {
        public BlogPost PostOne { get; set; }
        public BlogPost PostTwo { get; set; }
        public BlogPost PostThree { get; set; }
        public BlogPost PostFour { get; set; }
        public BlogPost PostFive { get; set; }
        public BlogPost PostSix { get; set; }
    }

}