using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gamificacion_API.Data;
using Gamificacion_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.CodeAnalysis.Scripting;
using BCrypt.Net;


namespace Gamificacion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly BdgamificacionContext _context;
        private readonly string key;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public UsuariosController(BdgamificacionContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            key = configuration.GetSection("JWTsetting").GetSection("securitykey").ToString();
        }
        private string GenerateToken(string email)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, email)
        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), 
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Definimos constantes para los roles
        private const byte ADMIN_ROLE = 1;
        private const byte GESTOR_ROLE = 2;
        private const byte STUDENT_ROLE = 3;

        [Route("api/users/authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] Auth user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Datos inválidos.");

            var userLogin = _context.Usuarios.FirstOrDefault(usr => usr.Email == user.Email);

            bool isPasswordValid = user.Password == userLogin?.Password; 

            if (userLogin == null || !isPasswordValid)
                return Unauthorized("Credenciales inválidas.");

            if (userLogin.Rol == STUDENT_ROLE)
                return Unauthorized("No tienes permisos de administrador.");

            var token = GenerateToken(user.Email);

            return Ok(new { token, user = userLogin });
        }

        [HttpPost]
        [Route("api/users/login-student")]
        public IActionResult LoginStudent([FromBody] Auth user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                return BadRequest("Datos inválidos.");

            var userLogin = _context.Usuarios.FirstOrDefault(usr => usr.Email == user.Email);

            bool isPasswordValid = user.Password == userLogin?.Password; 

            if (userLogin == null || !isPasswordValid)
                return Unauthorized("Credenciales inválidas.");

            if (userLogin.Rol != STUDENT_ROLE)
                return Unauthorized("No tienes permisos de estudiante.");

            var token = GenerateToken(user.Email);

            return Ok(new { token, user = userLogin });
        }

        private string GeneratePasswordResetToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }),
                Expires = DateTime.UtcNow.AddHours(1), // El token expira en 1 hora
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public class ForgotPasswordModel
        {
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email is required");
            }

            var email = model.Email;
            var user = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (user == null) return BadRequest("No account associated with email");

            var token = GeneratePasswordResetToken(email);

            // Aquí debes enviar el token al usuario por correo electrónico
            var callbackUrl = Url.Action(nameof(ResetPassword), "Usuarios", new { token }, protocol: HttpContext.Request.Scheme);
            await _emailService.SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking here: {callbackUrl}");

            return Ok("Password reset link has been sent to your email.");
        }


        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch
            {
                return null;
            }
        }
        public class ResetPasswordModel
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }

     
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token) || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest("Token and new password are required");
            }

            if (!IsPasswordStrong(model.NewPassword))
            {
                return BadRequest("The new password does not meet the strength requirements.");
            }

            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(model.Token);
            if (principal == null)
            {
                return BadRequest("Invalid token or token has expired");
            }

            string email = principal.Identity.Name;

            var user = _context.Usuarios.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("No user found");
            }

            user.Password = HashPassword(model.NewPassword);

            _context.SaveChanges();

            return Ok("Password has been reset successfully.");
        }

        private bool IsPasswordStrong(string password)
        {
           
            return password.Length >= 8; 
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        [HttpPost]
        [Route("api/users/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model == null)
            {
                return BadRequest("El modelo no puede ser nulo.");
            }

            if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("La contraseña antigua y la nueva son requeridas.");
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest("La nueva contraseña y la confirmación no coinciden.");
            }

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            if (string.IsNullOrWhiteSpace(user.Password) || !BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
            {
                return BadRequest("La contraseña antigua no es correcta o el hash de la contraseña almacenada no es válido.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada con éxito.");
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
          if (_context.Usuarios == null)
          {
              return NotFound();
          }
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
          if (_context.Usuarios == null)
          {
              return NotFound();
          }
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
          if (_context.Usuarios == null)
          {
              return Problem("Entity set 'BdgamificacionContext.Usuarios'  is null.");
          }
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.IdUsuario }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }
    }
}
