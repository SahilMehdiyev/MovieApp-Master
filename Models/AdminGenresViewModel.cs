using MovieApp.Web.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Models
{
    public class AdminGenresViewModel
    {
        [Required(ErrorMessage = "tür bilgisi girmelisiniz.")]
        public string Name { get; set; }
        public List<AdminGenreViewModel> Genres { get; set; }
    }

    public class AdminGenreViewModel
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class AdminGenreEditViewModel
    {
        public int GenreId { get; set; }
        [Required(ErrorMessage = "tür bilgisi girmelisiniz.")]
        public string Name { get; set; }
        public List<AdminMovieViewModel> Movies { get; set; }

        // Movies[0].MovieId,Movies[0].Name, 
        // Movies[1].MovieId,Movies[1].Name, 
    }


}
