using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MessengerAPI.Business;
using MessengerAPI.Models.DbModels;
using AutoMapper;
using MessengerAPI.Models.DTO;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMServerDbContext _context;

        public UserController(IMapper mapper, IMServerDbContext context)
        {
            this.mapper = mapper;
            _context = context;
        }


        // GET: api/User/5
        [HttpGet("{name}")]
        public async Task<ActionResult<UserDetailsDTO>> GetUser(string name)
        {
            var user = await _context.Users.FirstAsync(x => x.Name == name);

            if (user == null)
            {
                return NotFound();
            }

            return mapper.Map<UserDetailsDTO>(user);
        }



        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(nameof(RegisterUser))]
        public async Task<ActionResult<long>> RegisterUser(UserRegisterDTO registerDTO)
        {
            var user = mapper.Map<User>(registerDTO);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
