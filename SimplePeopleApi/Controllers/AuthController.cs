using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using SimplePeopleApi.Data;
using SimplePeopleApi.Models;
using SimplePeopleApi.Services;

namespace SimplePeopleApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJwtService _jwt;

        public AuthController(AppDbContext db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Username and password are required" });
            var exists = await _db.Usuarios.AnyAsync(u => u.Nome == req.Username);
            if (exists) return Conflict(new { message = "Username already exists" });

            var user = new Models.Usuario
            {
                Nome = req.Username,
                Senha = BCrypt.Net.BCrypt.HashPassword(req.Password),
                DataCriacao = DateTime.Now
            };

            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "User created" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Nome == req.Username);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });

            bool verified = BCrypt.Net.BCrypt.Verify(req.Password, user.Senha);
            if (!verified) return Unauthorized(new { message = "Invalid credentials" });

            var token = _jwt.GenerateToken(user.Nome);
            return Ok(new { token });
        }
    }

    public record RegisterRequest(string Username, string Password);
    public record LoginRequest(string Username, string Password);
}
