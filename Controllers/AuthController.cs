using dotnetapp.Models;
using dotnetapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using dotnetapp.Data;

namespace dotnetapp.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserService _userService;

        public AuthController(UserService userService, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _userService = userService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Invalid user data");

            if (user.Role == "admin" || user.Role == "customer")
            {
                Console.WriteLine("asd  " + user.Role);

                var isRegistered = await _userService.RegisterAsync(user);
                Console.WriteLine("status" + isRegistered);

                if (isRegistered)
                {
                    var customUser = new User
                    {
                        Username = user.Username,
                        Password = user.Password,
                        Email = user.Email,
                        MobileNumber = user.MobileNumber,
                        Role = user.Role,
                    };

                    // Add the customUser to the DbSet and save it
                    _context.Users.Add(customUser);
                    await _context.SaveChangesAsync();

                    return Ok(user);
                }
            }

            return BadRequest("Registration failed. Username may already exist.");
        }

       [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Invalid login request");
            Console.WriteLine("controller" + request.Email);
            var token = await _userService.LoginAsync(request.Email, request.Password);
 
            if (token == null)
                return Unauthorized("Invalid email or password");
 
            // Retrieve the user from UserManager to get their roles
            var user = await _userManager.FindByEmailAsync(request.Email);
            Console.WriteLine("role"+user);
            var roles = await _userManager.GetRolesAsync(user);
 
            return Ok(new { Token = token, Roles = roles });
    
        }
    }
}