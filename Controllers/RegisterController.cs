using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using ToDoAPI.Models;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ILogger<RegisterController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] DTOs.User data)
        {
            ToDoDbContext db = new();

            // generate a 128-bit salt using a secure PRNG
            byte[] genSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(genSalt);
            }

            byte[] hshPass = KeyDerivation.Pbkdf2(
                password: data.Password,
                salt: genSalt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );
            string b64Pass = Convert.ToBase64String(hshPass);

            var user = new Models.Users();
            user.IdUser = data.IdUser;
            user.Password = b64Pass;
            user.Salt = Convert.ToBase64String(genSalt); // Convert to base64 string

            db.Users.Add(user);
            db.SaveChanges();

            return Ok(new { detail = "Register successfully."});
        }

    }
}