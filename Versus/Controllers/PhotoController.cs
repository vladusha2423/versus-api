using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Versus.Core.EF;
using Versus.Data.Entities;

namespace Versus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhotoController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly VersusContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public PhotoController(UserManager<User> userManager, IWebHostEnvironment appEnvironment, VersusContext context)
        {
            _userManager = userManager;
            _appEnvironment = appEnvironment;
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddFile([FromForm]IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                try
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    
                    string path = "/img/" + user.UserName + "_" + uploadedFile.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    user.Photo = "http://188.225.73.49" + path;
                    await _userManager.UpdateAsync(user);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex);
                }
            }
            
            return BadRequest("File is null");
        }
    }
}