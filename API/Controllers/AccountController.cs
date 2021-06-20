using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        public DataContext _Context { get; }
        public AccountController(DataContext context)
        {
            _Context = context;

        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username is Taken");
            }

            using var hmac=new HMACSHA512();

            var user = new AppUser
            {
                UserName=registerDto.Username.ToLower(),
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };

            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            return user;
        }

        private async Task<bool> UserExists(string userName)
        {
            return await _Context.Users.AnyAsync(x=> x.UserName==userName.ToLower());
        }
    }
}