using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Entity
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }
    }

   
}
