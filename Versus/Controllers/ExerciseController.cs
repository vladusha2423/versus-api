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
    public class ExerciseController : ControllerBase
    {
        private readonly VersusContext _context;

        public ExerciseController(VersusContext context)
        {
            _context = context;
        }

        // GET: api/Exercise
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercise()
        {
            return await _context.Exercise.ToListAsync();
        }

        // GET: api/Exercise/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(Guid id)
        {
            var exercise = await _context.Exercise.FindAsync(id);

            if (exercise == null)
            {
                return NotFound();
            }

            return exercise;
        }

        // PUT: api/Exercise/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercise(Guid id, Exercise exercise)
        {
            exercise.Id = id;

            _context.Entry(exercise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExerciseExists(id))
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

        // POST: api/Exercise
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise(Exercise exercise)
        {
            _context.Exercise.Add(exercise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExercise", new { id = exercise.Id }, exercise);
        }

        // DELETE: api/Exercise/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Exercise>> DeleteExercise(Guid id)
        {
            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            _context.Exercise.Remove(exercise);
            await _context.SaveChangesAsync();

            return exercise;
        }

        private bool ExerciseExists(Guid id)
        {
            return _context.Exercise.Any(e => e.Id == id);
        }
    }
}
