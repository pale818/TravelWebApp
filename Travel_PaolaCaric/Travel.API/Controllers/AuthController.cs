using Microsoft.AspNetCore.Mvc;
using Travel.API.Data;
using Travel.API.Helpers;
using Travel.API.Models;
using Travel.API.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Scripting;

namespace Travel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;


        public AuthController(ApplicationDbContext context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // if Email is found it means user which wants to register with that Email already exists, mail is used
            if (_context.ApplicationUsers.Any(u => u.Email == request.Email))
                return BadRequest("Email already exists");




            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsAdmin = request.IsAdmin
            };




            _context.ApplicationUsers.Add(user);
            // calls function SaveChangesAsync to save the _context and waits to complete
            await _context.SaveChangesAsync();

            // return 200 OK to the frontend (Swagger)
            return Ok();
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginRequest request)
        {

            var user = await _context.ApplicationUsers.FirstOrDefaultAsync( u => u.UserName == request.UserName);



            if (user == null)
            {
                Console.WriteLine("User not found");
                return Unauthorized();
            }


            // id user exists thsi will be written, it is just for debugging
            Console.WriteLine($"User found: {user.UserName}");
            Console.WriteLine($"Incoming password: {request.Password}");
            Console.WriteLine($"Stored hash: {user.PasswordHash}");
            Console.WriteLine($"Password match: {BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)}");


            // here is checked pasword written in the web form with the saved password in the SQL db
            // it uses class BCrypt to dercypt saved password, because password are not saved in plain text but encrypted
            // and this is decrypting, and if does not match it returns error
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized();


            var token = _jwtTokenGenerator.Generate(user);
            return Ok(new { token });


        }


    }
}
