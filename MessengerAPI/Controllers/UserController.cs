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
using MessengerCommon;

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
        [HttpGet(nameof(GetUserByName) + "/{name}")]
        public async Task<ActionResult<UserDetailsDTO>> GetUserByName(string name)
        {
            var user = await _context.Users.FirstAsync(x => x.Name == name);

            if (user == null)
            {
                return NotFound();
            }

            return mapper.Map<UserDetailsDTO>(user);
        }

        [HttpGet(nameof(GetUserById) + "/{id}")]
        public async Task<ActionResult<UserDetailsDTO>> GetUserById(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return mapper.Map<UserDetailsDTO>(user);
        }


        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(nameof(RegisterUser))]
        public async Task<ActionResult<TokenDTO>> RegisterUser(UserRegisterDTO registerDTO)
        {
            var user = mapper.Map<User>(registerDTO);
            var number = RandomNumber.GenerateRandomNumber(256);
            user.UserToken = Convert.ToBase64String(number);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return mapper.Map<TokenDTO>(user);
        }

        [HttpPost(nameof(LoginUser))]
        public async Task<ActionResult<TokenDTO>> LoginUser(UserLoginDTO loginDTO)
        {
            var user = await _context.Users.FirstAsync(x => x.Name == loginDTO.Name);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (user.Password == loginDTO.Password)
            {
                return BadRequest("Wrong Password");
            }

            return mapper.Map<TokenDTO>(user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id, string userToken)
        {
            var user = await _context.Users.FindAsync(id);
            if (user.UserToken != userToken)
            {
                return BadRequest("Not Authorized");
            }
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
