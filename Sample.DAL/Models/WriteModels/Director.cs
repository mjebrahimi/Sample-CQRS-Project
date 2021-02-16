using System.Collections.Generic;

namespace Sample.DAL.Models.WriteModels
{
    public class Director
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public ICollection<Movie> Movies { get; set; }
    }
}