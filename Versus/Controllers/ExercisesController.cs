using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly VersusContext _context;

        public ExercisesController(VersusContext context)
        {
            _context = context;
        }

        // GET: api/Exercises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercises>>> GetExercises()
        {
            return await _context.Exercises
                .Include(e => e.PushUps)
                .Include(e => e.PullUps)
                .Include(e => e.Abs)
                .Include(e => e.Squats)
                .ToListAsync();
        }

        // GET: api/Exercises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exercises>> GetExercises(Guid id)
        {
            var exercises = await _context.Exercises
                .Include(e => e.PushUps)
                .Include(e => e.PullUps)
                .Include(e => e.Abs)
                .Include(e => e.Squats)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exercises == null)
            {
                return NotFound();
            }

            return exercises;
        }

        // GET: api/Exercises/5
        [HttpPost("reset/{id}")]
        public async Task<ActionResult<Exercises>> ResetExercises(Guid id)
        {
            var exercises = await _context.Exercises
                .Include(e => e.PushUps)
                .Include(e => e.PullUps)
                .Include(e => e.Abs)
                .Include(e => e.Squats)
                .FirstOrDefaultAsync(e => e.Id == id);

            exercises.PushUps.Wins = 0;
            exercises.PushUps.Losses = 0;
            exercises.PushUps.HighScore = 0;
            exercises.PullUps.Wins = 0;
            exercises.PullUps.Losses = 0;
            exercises.PullUps.HighScore = 0;
            exercises.Abs.Wins = 0;
            exercises.Abs.Losses = 0;
            exercises.Abs.HighScore = 0;
            exercises.Squats.Wins = 0;
            exercises.Squats.Losses = 0;
            exercises.Squats.HighScore = 0;

            _context.Entry(exercises).State = EntityState.Modified;

            if (exercises == null)
            {
                return NotFound();
            }

            try
            {
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExercisesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // PUT: api/Exercises/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercises(Guid id, Exercises exercises)
        {
            exercises.Id = id;

            exercises.PushUpsId = exercises.PushUps.Id;
            exercises.PullUpsId = exercises.PullUps.Id;
            exercises.AbsId = exercises.Abs.Id;
            exercises.SquatsId = exercises.Squats.Id;

            _context.Entry(exercises).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExercisesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Exercises
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Exercises>> PostExercises(Exercises exercises)
        {
            if (exercises.PushUps == null)
                exercises.PushUps = new Exercise();
            if (exercises.PullUps == null)
                exercises.PullUps = new Exercise();
            if (exercises.Abs == null)
                exercises.Abs = new Exercise();
            if (exercises.Squats == null)
                exercises.Squats = new Exercise();
            _context.Exercises.Add(exercises);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExercises", new { id = exercises.Id }, exercises);
        }

        // DELETE: api/Exercises/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Exercises>> DeleteExercises(Guid id)
        {
            var exercises = await _context.Exercises
                .Include(e => e.PushUps)
                .Include(e => e.PullUps)
                .Include(e => e.Abs)
                .Include(e => e.Squats)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exercises == null)
            {
                return NotFound();
            }

            _context.Exercise.Remove(exercises.PushUps);
            _context.Exercise.Remove(exercises.PullUps);
            _context.Exercise.Remove(exercises.Abs);
            _context.Exercise.Remove(exercises.Squats);
            _context.Exercises.Remove(exercises);
            await _context.SaveChangesAsync();

            return exercises;
        }

        private bool ExercisesExists(Guid id)
        {
            return _context.Exercises.Any(e => e.Id == id);
        }
    }
}
