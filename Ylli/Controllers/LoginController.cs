using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ylli.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static Benutzer benutzer = new Benutzer();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<Benutzer> Register(BenutzerDto request)
        {
            string passwordHash =
                BCrypt.Net.BCrypt.HashPassword(request.Passwort);
            benutzer.Benutzername = request.Benutzername;
            benutzer.PasswortHash = passwordHash;

            return Ok(benutzer);
        }

        [HttpPost("login")]
        public ActionResult<Benutzer> Login(BenutzerDto request)
        {
            if (benutzer.Benutzername != request.Benutzername)
            {
                return BadRequest("Benutzer nicht gefunden");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Passwort, benutzer.PasswortHash))
            {
                return BadRequest("Falsches Passwort");
            }
            string token = CreateToken(benutzer);

            return Ok(token);
        }

        private string CreateToken(Benutzer benutzer)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, benutzer.Benutzername)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
