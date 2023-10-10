using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using ToDoAPI.Models;

namespace ToDoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TokensController : ControllerBase
{
    private readonly ILogger<TokensController> _logger;

    public TokensController(ILogger<TokensController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Post([FromBody] DTOs.Login data)
    {
        //Initialize db
        ToDoDbContext db = new ToDoDbContext();

        //find user
        var user = db.Users.Find(data.IdUser);
        if (user == null) return Unauthorized(new { detail = "User not found" });

        //compare password
        byte[] hshPass = KeyDerivation.Pbkdf2(
            password: data.Password,
            salt: Convert.FromBase64String(user.Salt),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        );
        string b64Pass = Convert.ToBase64String(hshPass);
        if (user.Password != b64Pass) return Unauthorized(new { detail = "password is wrong", userName = user, password = user.Password , b64pass = b64Pass, hash = hshPass});

        //information correct--generating token
        SecurityTokenDescriptor desc = new SecurityTokenDescriptor();
        desc.Subject = new ClaimsIdentity(
            new Claim[]
            {
                new Claim(ClaimTypes.Name, user.IdUser),
                new Claim(ClaimTypes.Role, "user")
            }
        );
        desc.NotBefore = DateTime.UtcNow;
        desc.Expires = DateTime.UtcNow.AddHours(3);
        desc.IssuedAt = DateTime.UtcNow;
        desc.Issuer = Program.Issuer;
        desc.Audience = "public";
        desc.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Program.SecurityKey)
            ),
            SecurityAlgorithms.HmacSha256Signature
        );
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        SecurityToken token = handler.CreateToken(desc);
        string tokenScript = handler.WriteToken(token);

        return Ok(new { token = tokenScript });
    }
}