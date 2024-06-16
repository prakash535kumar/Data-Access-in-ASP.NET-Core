using Newtonsoft.Json;

namespace BasicEfCoreDemo.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ReleaseYear { get; set; }
        public List<Actor> Actors { get; set; } = new();
        [JsonIgnore]
        public List<MovieActor> MovieActors { get; set; } = new();
    }

    public class MovieActor
    {
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public Guid ActorId { get; set; }
        public Actor Actor { get; set; } = null!;
        public DateTime UpdateTime { get; set; }
    }

    public class Actor
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Movie> Movies { get; set; } = new();
        [JsonIgnore]
        public List<MovieActor> MovieActors { get; set; } = new();
    }
}
