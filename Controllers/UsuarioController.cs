using BibliotecaApi2.Models;
using BibliotecaApi2.Responses;
using BibliotecaApi2.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi2.Utils;

namespace BibliotecaApi2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly BibliotecaApi2DbContext _context;

        public UsuarioController(BibliotecaApi2DbContext context)
        {
            _context = context;
        }

        //
        // Get : api/usuario
        //
        [HttpGet]
        public async Task<IActionResult> GetUsuarios ()
        {
            var Usuarios = await _context.Usuarios.Select(u=> new UsuarioResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                eMail = u.eMail,
                FechaRegistro = u.FechaRegistro,
                DescNextPrest = u.DescNextPrest,
                PenalizacionPendiente = u.PenalizacionPendiente
            })
            .ToListAsync();

            return Ok(BibliotecaApi2Response<IEnumerable<UsuarioResponseDto>>.Ok( Usuarios));
        }

        // ====================
        // Get : api/usuarioId
        //=====================
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetUserById (Guid Id)
        {
            var User = await _context.Usuarios.Where(u=> u.Id == Id).FirstOrDefaultAsync();
            
            if(User == null)
                return NotFound(BibliotecaApi2Response<string>.Fail("Usuario no encontrado"));

            var UserResponse = new UsuarioResponseDto
            {
                Id = User.Id,
                Name = User.Name,
                eMail = User.eMail,
                FechaRegistro = User.FechaRegistro,
                DescNextPrest = User.DescNextPrest,
                PenalizacionPendiente = User.DescNextPrest
            };

            return Ok(BibliotecaApi2Response<UsuarioResponseDto>.Ok(UserResponse,"Usuario encontrado correctamente."));     
        }

        //
        // Get : api/usuariosMorosos
        //
        [HttpGet("morosidad/")]
        public async Task<IActionResult> GetUsuariosMorosos ()
        {
            var usuariosConPrestamo = await _context.Usuarios
            .Include(u=> u.Prestamos)
            .Where(u=> u.Prestamos.Any(p=> p.FechaFin < DateTime.UtcNow && p.FechaDevolucion == null))
            .ToListAsync();
            
            var usuariosMorosos = usuariosConPrestamo.Select(u=> new 
            {
                User = u,
                PrestamosVencidos = u.Prestamos.Count(p=> p.FechaFin < DateTime.UtcNow && p.FechaDevolucion == null),
                DiasPromedioRetraso = u.Prestamos
                .Where(p=> p.FechaFin < DateTime.UtcNow && p.FechaDevolucion == null)
                .Select(p=> (double)(DateTime.UtcNow - p.FechaFin).TotalDays)
                .DefaultIfEmpty(0)
                .Average()
            })
            .Where(u=> u.PrestamosVencidos >= 2)
            .Select(u=> new UsuarioMorosDto
            {
                name = u.User.Name,
                ScoreMorosidad = (u.PrestamosVencidos * 0.5m) + ((decimal)u.DiasPromedioRetraso * 0.3m) + (u.User.PenalizacionPendiente * 0.2m),
                DiasRetraso = u.DiasPromedioRetraso
            })
            .OrderByDescending(u=> u.ScoreMorosidad)
            .Take(5)
            .ToList();

            return Ok(BibliotecaApi2Response<List<UsuarioMorosDto>>.Ok(usuariosMorosos));
        }

        // ==================
        // Post : api/usuario
        // ==================
        [HttpPost]
        public async Task<IActionResult> PostUsuario ([FromBody] PostUsuarioDto PostUser)
        {
            if(!ModelState.IsValid)
                return BadRequest(BibliotecaApi2Response<string>.Fail("ModelState Error."));

            var newUser = new Usuario
            {
                Name = PostUser.Name,
                eMail = PostUser.eMail,
                PasswordHash = PasswordHelper.Hash(PostUser.Password),
                FechaRegistro = DateTime.UtcNow,
            };

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Usuario Creado Coreectamente."});        
        }

        // ==================
        // Put : api/usuario
        // ==================
        [HttpPut("{Id}")]
        public async Task<IActionResult> PutUsuario (Guid Id, [FromBody] PutUsuarioDto UsuarioEd)
        {
            if(!ModelState.IsValid)
                return BadRequest(BibliotecaApi2Response<string>.Fail("ModelState Error."));

            var user = await _context.Usuarios.Where(u=> u.Id == Id).FirstOrDefaultAsync();

            if(user == null)
                return NotFound(BibliotecaApi2Response<string>.Fail("Usuario no encontrado."));

            user.Name = UsuarioEd.Name;
            user.eMail = UsuarioEd.Name;
            user.PenalizacionPendiente = UsuarioEd.PenalizacionPendiente;
            user.DescNextPrest = UsuarioEd.DescNextPrest;

            await _context.SaveChangesAsync();

            return Ok(BibliotecaApi2Response<Usuario>.Ok(user , "Usuario modificado exitosamente."));         
        }

        // =====================
        // Delete : api/usuario
        // =====================
        [HttpDelete("usuario/{Id}")]
        public async Task<IActionResult> DeleteUserById (Guid Id)
        {
            var usuario = await _context.Usuarios.Where(u=> u.Id == Id).FirstOrDefaultAsync();

            if(usuario == null)
                return NotFound(BibliotecaApi2Response<string>.Fail("Usuario no encontrado."));

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Usuario Eliminado."});    
        }
    }  
}      