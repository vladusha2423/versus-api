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
    public class ExerciseController : ControllerBase
    {
        private readonly VersusContext _context;
        private readonly UserManager<User> _userManager;

        public ExerciseController(VersusContext context, UserManager<User> um)
        {
            _context = context;
            _userManager = um;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercise()
        {
            return await _context.Exercise.ToListAsync();
        }

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
        
        [HttpGet("PullUps/{id}")]
        public async Task<ActionResult<Settings>> GetPullUpsByUserId(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Не найден пользователь с таким Id");

            var user = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PullUps)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user.Exercises == null)
            {
                return NotFound("У пользователя отсутствует связанная сущность \"Exercises\"");
            }

            if (user.Exercises.PullUps == null)
            {
                return NotFound("У User.Exercises отсутствует связанная сущность PullUps");
            }

            return Ok(user.Exercises.PullUps);
        }
        
        [HttpGet("PushUps/{id}")]
        public async Task<ActionResult<Settings>> GetPushUpsByUserId(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Не найден пользователь с таким Id");

            var user = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(e => e.PushUps)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user.Exercises == null)
            {
                return NotFound("У пользователя отсутствует связанная сущность \"Exercises\"");
            }

            if (user.Exercises.PushUps == null)
            {
                return NotFound("У User.Exercises отсутствует связанная сущность PushUps");
            }

            return Ok(user.Exercises.PushUps);
        }
        
        [HttpGet("Abs/{id}")]
        public async Task<ActionResult<Settings>> GetAbsByUserId(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Не найден пользователь с таким Id");

            var user = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Abs)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user.Exercises == null)
            {
                return NotFound("У пользователя отсутствует связанная сущность \"Exercises\"");
            }

            if (user.Exercises.Abs == null)
            {
                return NotFound("У User.Exercises отсутствует связанная сущность Abs");
            }

            return Ok(user.Exercises.Abs);
        }
        
        [HttpGet("Squats/{id}")]
        public async Task<ActionResult<Settings>> GetSquatsByUserId(Guid id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id))
                return NotFound("Не найден пользователь с таким Id");

            var user = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(e => e.Squats)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user.Exercises == null)
            {
                return NotFound("У пользователя отсутствует связанная сущность \"Exercises\"");
            }

            if (user.Exercises.Squats == null)
            {
                return NotFound("У User.Exercises отсутствует связанная сущность Squats");
            }

            return Ok(user.Exercises.Squats);
        }
        
        [HttpPut("pullups/{id}")]
        public async Task<IActionResult> PutPullupsByUserId(Guid id, Exercise ex)
        {
            if (!await _userManager.Users.AnyAsync(s => s.Id == id))
                return NotFound("Такого UserID не существует");

            var reqUser = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(s => s.PullUps)
                .FirstOrDefaultAsync(s => s.Id == id);

            var reqExercise = reqUser.Exercises.PullUps;

            reqExercise.Wins = ex.Wins;
            reqExercise.Losses = ex.Losses;
            reqExercise.HighScore = ex.HighScore;

            _context.Entry(reqExercise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reqExercise);
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
        }
        
        [HttpPut("pushups/{id}")]
        public async Task<IActionResult> PutPushupsByUserId(Guid id, Exercise ex)
        {
            if (!await _userManager.Users.AnyAsync(s => s.Id == id))
                return NotFound("Такого UserID не существует");

            var reqUser = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(s => s.PushUps)
                .FirstOrDefaultAsync(s => s.Id == id);

            var reqExercise = reqUser.Exercises.PushUps;

            reqExercise.Wins = ex.Wins;
            reqExercise.Losses = ex.Losses;
            reqExercise.HighScore = ex.HighScore;

            _context.Entry(reqExercise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reqExercise);
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
        }
        
        [HttpPut("abs/{id}")]
        public async Task<IActionResult> PutAbsByUserId(Guid id, Exercise ex)
        {
            if (!await _userManager.Users.AnyAsync(s => s.Id == id))
                return NotFound("Такого UserID не существует");

            var reqUser = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(s => s.Abs)
                .FirstOrDefaultAsync(s => s.Id == id);

            var reqExercise = reqUser.Exercises.Abs;

            reqExercise.Wins = ex.Wins;
            reqExercise.Losses = ex.Losses;
            reqExercise.HighScore = ex.HighScore;

            _context.Entry(reqExercise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reqExercise);
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
        }
        
        [HttpPut("squats/{id}")]
        public async Task<IActionResult> PutSquatsByUserId(Guid id, Exercise ex)
        {
            if (!await _userManager.Users.AnyAsync(s => s.Id == id))
                return NotFound("Такого UserID не существует");

            var reqUser = await _userManager.Users
                .Include(u => u.Exercises)
                .ThenInclude(s => s.Squats)
                .FirstOrDefaultAsync(s => s.Id == id);

            var reqExercise = reqUser.Exercises.Squats;

            reqExercise.Wins = ex.Wins;
            reqExercise.Losses = ex.Losses;
            reqExercise.HighScore = ex.HighScore;

            _context.Entry(reqExercise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(reqExercise);
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
        }

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
        
        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise(Exercise exercise)
        {
            _context.Exercise.Add(exercise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExercise", new { id = exercise.Id }, exercise);
        }

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
