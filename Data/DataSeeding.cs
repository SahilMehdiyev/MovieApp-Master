using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Data
{
    public static class DataSeeding
    {
        public static void Seed(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<MovieContext>();

            context.Database.Migrate(); // dotnet ef database update

            var genres = new List<Genre>()
                        {
                            new Genre {Name="Macera", Movies=
                                new List<Movie>(){
                                    new Movie {
                                        Title="yeni macera filmi 1",
                                        Description="Every six years, an ancient order of jiu-jitsu fighters joins forces to battle a vicious race of alien invaders. But when a celebrated war hero goes down in defeat, the fate of the planet and mankind hangs in the balance.",
                                        ImageUrl="1.jpg"
                                    },
                                    new Movie {
                                        Title="yeni macera filmi 2",
                                        Description="A rowdy, unorthodox Santa Claus is fighting to save his declining business. Meanwhile, Billy, a neglected and precocious 12 year old, hires a hit m...",
                                        ImageUrl="2.jpg"
                                    },

                                } 
                            },
                            new Genre {Name="Komedi"},
                            new Genre {Name="Romantik"},
                            new Genre {Name="Savaş"},
                            new Genre {Name="Bilim Kurgu"}
                        };
            var movies = new List<Movie>()
                        {
                            new Movie {
                                Title="Jiu Jitsu",
                                Description="Every six years, an ancient order of jiu-jitsu fighters joins forces to battle a vicious race of alien invaders. But when a celebrated war hero goes down in defeat, the fate of the planet and mankind hangs in the balance.",
                                ImageUrl="1.jpg",
                                Genres = new List<Genre>() {genres[0], new Genre(){Name="Yeni Tür"}, genres[1] }
                            },
                            new Movie {
                                Title="Fatman",
                                Description="A rowdy, unorthodox Santa Claus is fighting to save his declining business. Meanwhile, Billy, a neglected and precocious 12 year old, hires a hit m...",
                                ImageUrl="2.jpg",
                                Genres = new List<Genre>() {genres[0],genres[2] }
                            },
                            new Movie {
                                Title="The Dalton Gang",
                                Description="When their brother Frank is killed by an outlaw, brothers Bob Dalton, Emmett Dalton and Gray Dalton join their local sheriff's department. When the...",
                                ImageUrl="3.jpg",
                                Genres = new List<Genre>() {genres[1], genres[3] }
                            },
                                new Movie {
                                Title="Tenet",
                                Description="Armed with only one word - Tenet - and fighting for the survival of the entire world, the Protagonist journeys through a twilight world of internat...",
                                ImageUrl="4.jpg",
                                Genres = new List<Genre>() {genres[0], genres[1] }
                            },
                            new Movie {
                                Title="The Craft: Legacy",
                                Description="An eclectic foursome of aspiring teenage witches get more than they bargained for as they lean into their newfound powers.",
                                ImageUrl="5.jpg",
                                Genres = new List<Genre>() {genres[2], genres[4] }
                            },
                            new Movie {
                                Title="Hard Kill",
                                Description="The work of billionaire tech CEO Donovan Chalmers is so valuable that he hires mercenaries to protect it, and a terrorist group kidnaps his daughte...",
                                ImageUrl="6.jpg",
                                Genres = new List<Genre>() {genres[1], genres[2] }
                            }
                        };
            //var users = new List<User>() {
            //    new User() { Username="usera",Email="usera@gmail.com",Password="1234",},
            //    new User() { Username="userb",Email="userb@gmail.com",Password="1234",},
            //    new User() { Username="userc",Email="userc@gmail.com",Password="1234",},
            //    new User() { Username="userb",Email="userb@gmail.com",Password="1234",}
            //};
            //var people = new List<Person>()
            //{
            //    new Person()
            //    {
            //        Name = "Personel 1",
            //        Biography = "tabıtım 1",
            //        User = users[0]
            //    },
            //    new Person()
            //    {
            //        Name="Personel 2",
            //        Biography="tabıtım 2",
            //        User = users[1]
            //    }
            //};
            //var crews = new List<Crew>()
            //{ 
            //    new Crew() { Movie=movies[0], Person=people[0],Job="Yönetmen" },
            //    new Crew() { Movie=movies[0], Person=people[1],Job="Yönetmen Yard." }
            //};
            //var casts = new List<Cast>()
            //{ 
            //    new Cast() {Movie=movies[0], Person=people[0],Name="Oyuncu Adı 1",Character="Karakter 1"},
            //    new Cast() {Movie=movies[0], Person=people[1],Name="Oyuncu Adı 2",Character="Karakter 2"},
            //};

            if (context.Database.GetPendingMigrations().Count()==0)
            {
                if (context.Genres.Count() == 0)
                {
                    context.Genres.AddRange(genres);
                }

                if (context.Movies.Count()==0)
                {
                    context.Movies.AddRange(movies);
                }

                //if (context.Users.Count() == 0)
                //{
                //    context.Users.AddRange(users);
                //}

                //if (context.People.Count() == 0)
                //{
                //    context.People.AddRange(people);
                //}

                //if (context.Casts.Count() == 0)
                //{
                //    context.Casts.AddRange(casts);
                //}

                //if (context.Crews.Count() == 0)
                //{
                //    context.Crews.AddRange(crews);
                //}

                context.SaveChanges(); 
            }
        }
    }
}
