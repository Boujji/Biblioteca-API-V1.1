using BibliotecaApi2.Dtos;
using BibliotecaApi2.DTOs.Auth;
using BibliotecaApi2.Models;
using BibliotecaApi2.Responses;
using BibliotecaApi2.Utils;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaApi2.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly BibliotecaApi2DbContext _context;
        private readonly IConfiguration _config;

        public AuthorizationController(BibliotecaApi2DbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.eMail == dto.eMail);

            if (user == null || !PasswordHelper.Verify(dto.Password, user.PasswordHash))
            {
                return Ok(BibliotecaApi2Response<object>.Fail(
                    "Credenciales incorrectas",
                    "AUTH_INVALID_CREDENTIALS"
                ));
            }

            var accessToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();

            var refresh = new RefreshTokens
            {
                Id = Guid.NewGuid(),
                UsuarioId = user.Id,
                Token = refreshToken,
                Expira = DateTime.UtcNow.AddDays(7),
                Revocado = false
            };

            _context.RefreshTokens.Add(refresh);
            await _context.SaveChangesAsync();

            var data = new
            {
                accessToken,
                refreshToken,
                user.Id,
                user.Name,
                user.eMail,
                user.Rol
            };

            return Ok(BibliotecaApi2Response<object>.Ok(data, "Login exitoso"));
        }

        // =========================
        // CHANGE PASSWORD
        // =========================
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                return Ok(BibliotecaApi2Response<string>.Fail("Usuario no autorizado", "AUTH_UNAUTHORIZED"));

            if (!PasswordHelper.Verify(dto.PasswordActual, user.PasswordHash))
                return Ok(BibliotecaApi2Response<string>.Fail("La contraseña actual es incorrecta", "AUTH_INVALID_PASSWORD"));

            user.PasswordHash = PasswordHelper.Hash(dto.PasswordNueva);
            await _context.SaveChangesAsync();

            return Ok(BibliotecaApi2Response<string>.Ok("OK", "Contraseña actualizada correctamente"));
        }

        // =========================
        // RECOVER PASSWORD
        // =========================
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDto dto)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.eMail == dto.eMail);

            if (user == null)
            {
                return Ok(BibliotecaApi2Response<string>.Ok(
                    "OK",
                    "Si el correo existe, se enviará recuperación"
                ));
            }

            user.PasswordHash = PasswordHelper.Hash("123456"); // simulación
            await _context.SaveChangesAsync();

            return Ok(BibliotecaApi2Response<string>.Ok(
                "OK",
                "Contraseña reiniciada. Revisa tu correo (simulado: 123456)"
            ));
        }

        // =========================
        // REFRESH TOKEN
        // =========================
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            var refresh = await _context.RefreshTokens
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

            if (refresh == null || refresh.Revocado || refresh.Expira < DateTime.UtcNow)
            {
                return Ok(BibliotecaApi2Response<string>.Fail(
                    "Refresh token inválido o expirado",
                    "AUTH_REFRESH_INVALID"
                ));
            }

            var user = refresh.Usuario;

            var newAccessToken = GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            refresh.Revocado = true;

            var newRefresh = new RefreshTokens
            {
                Id = Guid.NewGuid(),
                UsuarioId = user.Id,
                Token = newRefreshToken,
                Expira = DateTime.UtcNow.AddDays(7),
                Revocado = false
            };

            _context.RefreshTokens.Add(newRefresh);
            await _context.SaveChangesAsync();

            return Ok(BibliotecaApi2Response<object>.Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            }, "Token renovado"));
        }

        // =========================
        // LOGOUT
        // =========================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
        {
            var refresh = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

            if (refresh != null)
            {
                refresh.Revocado = true;
                await _context.SaveChangesAsync();
            }

            return Ok(BibliotecaApi2Response<string>.Ok("OK", "Sesión cerrada correctamente"));
        }

        // =========================
        // TOKEN HELPERS
        // =========================
        private string GenerateToken(Usuario user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.eMail),
                new Claim(ClaimTypes.Role, user.Rol.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }    
    }
}