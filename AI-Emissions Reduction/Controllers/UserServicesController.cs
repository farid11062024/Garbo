using AI_Emissions_Reduction.Data;
using AI_Emissions_Reduction.Data.Entity.One_to_Many;
using AI_Emissions_Reduction.DTO.ReuqestDTO;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AI_Emissions_Reduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserServicesController : ControllerBase
    {
        private readonly MyDBcontext _context;
        public UserServicesController(MyDBcontext context)
        {
            _context = context;
        }

        [HttpPost("RegisterNewUser")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            // İstifadəçi artıq varsa
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser != null)
                throw new Exception("Bu e-poçtla istifadəçi artıq mövcuddur.");

            User user = new User
            {
                Name = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password) // Şifrəni hashləyirik
            };

           await  _context.Users.AddAsync(user);
           await _context.SaveChangesAsync();

            return NoContent();
        }
        private string HashPassword(string password)
        {
            throw new NotImplementedException();
        }

        [HttpGet]

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("E-poçt və ya şifrə düzgün göndərilməyib.");
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    return BadRequest("İstifadəçi tapılmadı.");
                }

                bool isPasswordValid = VerifyPassword(loginDto.Password, user.PasswordHash);

                if (!isPasswordValid)
                {
                    return BadRequest("Şifrə yanlışdır.");
                }

                return Ok(new
                {
                    message = "Daxil olma uğurludur.",
                    user = new
                    {
                        user.Name,
                        user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Daxil olarkən xəta baş verdi: {ex.Message}");
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public class PasswordHelper
        {
            public static string HashPassword(string password)
            {
                var salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                return $"{Convert.ToBase64String(salt)}:{hashed}";
            }

            public static bool VerifyPassword(string password, string storedPasswordHash)
            {
                var parts = storedPasswordHash.Split(':');
                if (parts.Length != 2)
                {
                    return false;
                }

                var salt = Convert.FromBase64String(parts[0]);
                var storedHash = parts[1];

                var computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                return computedHash == storedHash;
            }
        }
    }
}

