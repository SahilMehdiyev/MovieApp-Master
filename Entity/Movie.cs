using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Entity
{
    public class Movie
    {
        public Movie()
        {
            Genres = new List<Genre>();
        }
        public int MovieId { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public string ImageUrl { get; set; } 
        public List<Genre> Genres { get; set; }
    }
}
