using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicEfCoreDemo.Data;
using BasicEfCoreDemo.Models;

namespace BasicEfCoreDemo.Controllers
{
    /// <summary>
    /// API Controller for managing actors and their associated movies.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly SampleDbContext _context;

        public ActorsController(SampleDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all actors including their associated movies.
        /// </summary>
        /// <returns>List of actors with associated movies.</returns>
        // GET: api/Actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActors()
        {
            // Check if Actors DbSet is null
            if (_context.Actors == null)
            {
                return NotFound();
            }

            // Return all actors with their associated movies
            return await _context.Actors.Include(x => x.Movies).ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific actor by ID including their associated movies.
        /// </summary>
        /// <param name="id">The ID of the actor to retrieve.</param>
        /// <returns>The actor with the specified ID.</returns>
        // GET: api/Actors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Actor>> GetActor(Guid id)
        {
            // Check if Actors DbSet is null
            if (_context.Actors == null)
            {
                return NotFound();
            }

            // Find actor by ID
            var actor = await _context.Actors.FindAsync(id);

            // If actor is not found, return NotFound
            if (actor == null)
            {
                return NotFound();
            }

            // Return the actor
            return actor;
        }

        /// <summary>
        /// Updates an existing actor.
        /// </summary>
        /// <param name="id">The ID of the actor to update.</param>
        /// <param name="actor">The updated actor object.</param>
        /// <returns>No content if successful, bad request if IDs do not match, or not found if actor does not exist.</returns>
        // PUT: api/Actors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActor(Guid id, Actor actor)
        {
            // Check if IDs match
            if (id != actor.Id)
            {
                return BadRequest();
            }

            // Mark actor entity as modified
            _context.Entry(actor).State = EntityState.Modified;

            try
            {
                // Save changes asynchronously
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check if actor exists
                if (!ActorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Return NoContent if successful
            return NoContent();
        }

        /// <summary>
        /// Creates a new actor.
        /// </summary>
        /// <param name="actor">The actor object to create.</param>
        /// <returns>The newly created actor.</returns>
        // POST: api/Actors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Actor>> PostActor(Actor actor)
        {
            // Check if Actors DbSet is null
            if (_context.Actors == null)
            {
                return Problem("Entity set 'SampleDbContext.Actors' is null.");
            }

            // Add actor to DbSet
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();

            // Return newly created actor
            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        /// <summary>
        /// Deletes an actor by ID.
        /// </summary>
        /// <param name="id">The ID of the actor to delete.</param>
        /// <returns>No content if successful, not found if actor does not exist.</returns>
        // DELETE: api/Actors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(Guid id)
        {
            // Check if Actors DbSet is null
            if (_context.Actors == null)
            {
                return NotFound();
            }

            // Find actor by ID
            var actor = await _context.Actors.FindAsync(id);
            // If actor is not found, return NotFound
            if (actor == null)
            {
                return NotFound();
            }

            // Remove actor from DbSet
            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            // Return NoContent if successful
            return NoContent();
        }

        /// <summary>
        /// Adds a movie to an actor's list of associated movies.
        /// </summary>
        /// <param name="id">The ID of the actor.</param>
        /// <param name="movieId">The ID of the movie to add.</param>
        /// <returns>The updated actor with the added movie.</returns>
        [HttpPost("{id}/movies/{movieId}")]
        public async Task<IActionResult> AddMovie(Guid id, Guid movieId)
        {
            // Check if Actors DbSet is null
            if (_context.Actors == null)
            {
                return NotFound("Actors is null.");
            }

            // Find actor by ID and include movies
            var actor = await _context.Actors.Include(x => x.Movies).SingleOrDefaultAsync(x => x.Id == id);
            // If actor is not found, return NotFound
            if (actor == null)
            {
                return NotFound($"Actor with id {id} not found.");
            }

            // Find movie by ID
            var movie = await _context.Movies.FindAsync(movieId);
            // If movie is not found, return NotFound
            if (movie == null)
            {
                return NotFound($"Movie with id {movieId} not found.");
            }

            // Check if movie already exists in actor's Movies collection
            if (actor.Movies.Any(x => x.Id == movie.Id))
            {
                return Problem($"Movie with id {movieId} already exists for Actor {id}.");
            }

            // Add movie to actor's Movies collection
            actor.Movies.Add(movie);
            await _context.SaveChangesAsync();

            // Return updated actor
            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        /// <summary>
        /// Retrieves all movies associated with a specific actor.
        /// </summary>
        /// <param name="id">The ID of the actor.</param>
        /// <returns>The list of movies associated with the actor.</returns>
        [HttpGet("{id}/movies")]
        public async Task<IActionResult> GetMovies(Guid id)
        {
            // Check if Actors DbSet is null
            if (_context.Actors == null)
            {
                return NotFound("Actors is null.");
            }

            // Find actor by ID and include movies
            var actor = await _context.Actors.Include(x => x.Movies).SingleOrDefaultAsync(x => x.Id == id);
            // If actor is not found, return NotFound
            if (actor == null)
            {
                return NotFound($"Actor with id {id} not found.");
            }

            // Return actor's Movies collection
            return Ok(actor.Movies);
        }

        /// <summary>
        /// Removes a movie from an actor's list of associated movies.
        /// </summary>
        /// <param name="id">The ID of the actor.</param>
        /// <param name="movieId">The ID of the movie to remove.</param>
        /// <returns>No content if successful, not found if actor or movie does not exist.</returns>
        [HttpDelete("{id}/movies/{movieId}")]
        public async Task<IActionResult> DeleteMovie(Guid id, Guid movieId)
        {
            // Check if Actors DbSet is null
            if (_context.Actors == null)
            {
                return NotFound("Actors is null.");
            }

            // Find actor by ID and include movies
            var actor = await _context.Actors.Include(x => x.Movies).SingleOrDefaultAsync(x => x.Id == id);
            // If actor is not found, return NotFound
            if (actor == null)
            {
                return NotFound($"Actor with id {id} not found.");
            }

            // Find movie by ID
            var movie = await _context.Movies.FindAsync(movieId);
            // If movie is not found, return NotFound
            if (movie == null)
            {
                return NotFound($"Movie with id {movieId} not found.");
            }

            // Remove movie from actor's Movies collection
            actor.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            // Return NoContent if successful
            return NoContent();
        }

        /// <summary>
        /// Checks if an actor with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the actor to check.</param>
        /// <returns>True if actor exists, false otherwise.</returns>
        private bool ActorExists(Guid id)
        {
            // Check if any actor with given ID exists in Actors DbSet
            return (_context.Actors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
