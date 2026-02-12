using BibliotecaApi2.Models;
using BibliotecaApi2.Responses;
using BibliotecaApi2.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controllers]")]
    public class PrestamoController : ControllerBase
    {
        private readonly BibliotecaApi2DbContext _context;

        public PrestamoController(BibliotecaApi2DbContext context)
        {
            _context = context;
        }

        // ===================
        // Get: api/prestamo
        // ===================
        [HttpGet]
        public async Task<IActionResult> GetPrestamo ()
        {
            var prestamos = await _context.Prestamos
                .OrderByDescending(p=> p.FechaInicio)
                .Select(p=> new PrestamoResponseDto
                {
                    Id = p.Id,
                    UsuarioId = p.UsuarioId,
                    LibroId = p.LibroId,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    FechaDevolucion = p.FechaDevolucion,
                    CostoDiario = p.CostoDiario
                })
                .ToListAsync();

            return Ok(BibliotecaApi2Response<List<PrestamoResponseDto>>.Ok(prestamos));
        }

        // ====================
        // GetId: api/prestamo
        // ====================
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetPrestamoById (Guid PresId)
        {
            var prestamo = await _context.Prestamos.Where(p=> p.Id == PresId).FirstOrDefaultAsync();

            if( prestamo == null)
                return NotFound(BibliotecaApi2Response<string>.Fail("No se ha encontrado nignuna coinicidencia"));
            
            var ResponseDto = new PrestamoResponseDto
            {
                Id = prestamo.Id,
                LibroId = prestamo.LibroId,
                Libro = prestamo.Libro,
                UsuarioId = prestamo.UsuarioId,
                Usuario = prestamo.Usuario,
                FechaInicio = prestamo.FechaInicio,
                FechaFin = prestamo.FechaFin,
                FechaDevolucion = prestamo.FechaDevolucion,
                CostoDiario = prestamo.CostoDiario
            };

            return Ok(BibliotecaApi2Response<PrestamoResponseDto>.Ok( ResponseDto , "Prestamo Enonctrado Correctamente"));
        }

        // ===================
        // Post: api/Prestamo
        // ===================
        [HttpPost]
        public async Task<IActionResult> PostPrestamo ([FromBody] PostPrestamoDto PostDto)
        {
            var libroDisponible = await _context.Libros
            .Where(l=> l.Id == PostDto.LibroId && l.Disponible == true)
            .FirstOrDefaultAsync();

            var usuarioExiste = await _context.Usuarios
            .Where(u=> u.Id == PostDto.UsuarioId)
            .FirstOrDefaultAsync();

            if (usuarioExiste == null)
                return BadRequest(BibliotecaApi2Response<string>.Fail("Usuario no encontrado"));

            if (libroDisponible == null)
                return BadRequest(BibliotecaApi2Response<string>.Fail("El libro no esta disponible"));

            var prestamoActivo = usuarioExiste.Prestamos.Count(p=> p.FechaDevolucion == null && p.FechaFin < DateTime.UtcNow);    

            if (prestamoActivo > 3)
                return BadRequest(BibliotecaApi2Response<string>.Fail("El usaurio tiene demasiados prestamos"));

            if ((PostDto.FechaFin - PostDto.FechaInicio).TotalDays > 15)
                return BadRequest(BibliotecaApi2Response<string>.Fail("La Duracion del prestamo no puede superar los 15 dias"));

            var costoEstimado = (int)(PostDto.FechaFin - PostDto.FechaInicio).TotalDays * PostDto.CostoDiario;     

            var prestamo = new Prestamo
            {
              Id = new Guid(),
              FechaInicio = PostDto.FechaInicio,
              FechaFin = PostDto.FechaFin,
              LibroId = PostDto.LibroId,  
              UsuarioId = PostDto.UsuarioId,
              CostoDiario = PostDto.CostoDiario  
            };

            _context.Prestamos.Add(prestamo);
            
            libroDisponible.CantidadLibro --;            
            if (libroDisponible.CantidadLibro == 0)
                libroDisponible.Disponible = false;

            await _context.SaveChangesAsync();

            return Ok(prestamo);                   
        }

        // ===================
        // Put : api/Prestamo
        // ===================
        [HttpPut("putReturn/{id}")]
        public async Task<IActionResult> PutReturnPrestamo ([FromBody] Guid PresId)
        {
            var prestamoReturn = await _context.Prestamos.Include(p=> p.Libro).Include(p=> p.Usuario).Where(p=> p.Id == PresId).FirstOrDefaultAsync();
            
            if(prestamoReturn == null)
                return NotFound(BibliotecaApi2Response<string>.Fail("Error. Prestamo no encontrado."));

            prestamoReturn.FechaDevolucion = DateTime.UtcNow;
            if(prestamoReturn.Libro.Disponible == false)
            {
                prestamoReturn.Libro.Disponible = true;
                prestamoReturn.Libro.CantidadLibro ++;
            }

            var penalizacion = (int)(prestamoReturn.FechaDevolucion.Value - prestamoReturn.FechaFin).TotalDays;

            if(penalizacion > 0)
            {
                prestamoReturn.Usuario.PenalizacionPendiente = penalizacion * prestamoReturn.CostoDiario * 1.5m;
            }

            var Discount = prestamoReturn.Usuario.Prestamos
            .OrderByDescending(p=> p.FechaDevolucion)
            .Take(3)
            .ToList();

            if(Discount.Count == 3 && Discount.All(p=> p.FechaFin >= p.FechaDevolucion))
                prestamoReturn.Usuario.DescNextPrest = 20;

            await _context.SaveChangesAsync(); 

            return Ok(new {message = "Prestamo modificado correctamente"}); 
        } 

    }
}
