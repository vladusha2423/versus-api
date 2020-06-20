using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExercisesController : ControllerBase
    {
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;

        public ExercisesController(VersusContext context, UserManager<User> um)
        {
            _context = context;
            _userManager = um;
        }

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
        
        [HttpGet("user/{id}")]
        public async Task<ActionResult<Settings>> GetExercisesByUserId(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Не найден пользователь с таким Id");

            var user = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PullUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PushUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Abs)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Squats)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user.Exercises == null)
            {
                return NotFound("У пользователя отсутствует связанная сущность \"Exercises\"");
            }

            user.Exercises.User = null;

            return Ok(user.Exercises);
        }

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

        [HttpPost("reset/{id}")]
        public async Task<ActionResult<Exercises>> ResetExercises(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Такого Юзера не существует");

            var user = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PullUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PushUps)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Abs)
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Squats)
                .FirstOrDefaultAsync(u => u.Id == id);

            user.Exercises.PushUps.Wins = 0;
            user.Exercises.PushUps.Losses = 0;
            user.Exercises.PushUps.HighScore = 0;
            user.Exercises.PullUps.Wins = 0;
            user.Exercises.PullUps.Losses = 0;
            user.Exercises.PullUps.HighScore = 0;
            user.Exercises.Abs.Wins = 0;
            user.Exercises.Abs.Losses = 0;
            user.Exercises.Abs.HighScore = 0;
            user.Exercises.Squats.Wins = 0;
            user.Exercises.Squats.Losses = 0;
            user.Exercises.Squats.HighScore = 0;

            _context.Entry(user.Exercises).State = EntityState.Modified;

            if (user.Exercises == null)
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
                if (!ExercisesExists(user.Exercises.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        
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
