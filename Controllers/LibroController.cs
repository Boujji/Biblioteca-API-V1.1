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
    public class LibroController : ControllerBase
    {
        private readonly BibliotecaApi2DbContext _context;

        public LibroController(BibliotecaApi2DbContext context)
        {
            _context = context;
        }

        // =================
        // Get : api/libros
        // =================
        [HttpGet]
        public async Task<IActionResult> GetLibros ()
        {
            var libros = await _context.Libros
            .OrderByDescending(l=> l.Titulo)
            .Select(l=> new LibroResponseDto
            {
                Titulo = l.Titulo,
                Id = l.Id,
                Categoria = l.Categoria,
                CantidadLibro = l.CantidadLibro,
                Disponible = l.Disponible
            })
            .ToListAsync();

            return Ok(BibliotecaApi2Response<ICollection<LibroResponseDto>>.Ok(libros));
        }

        // =============
        // Get : api/Id 
        // =============
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetLibroById (Guid Id)
        {
            var Libro = await _context.Libros.Where(l=> l.Id == Id).FirstOrDefaultAsync();

            if(Libro == null)
                return NotFound(BibliotecaApi2Response<string>.Fail("No se ha encontrado ninguna coincidencia."));

            var ResponseDto = new LibroResponseDto
            {
                Id = Libro.Id,
                Titulo = Libro.Titulo,
                Categoria = Libro.Categoria,
                CantidadLibro = Libro.CantidadLibro,
                Disponible = Libro.Disponible
            };

            return Ok (BibliotecaApi2Response<LibroResponseDto>.Ok(ResponseDto,"Libro encontrado correctamente"));        
        }

        // =============================
        // Get : api/librosMayorVolumen
        // =============================
        [HttpGet("librosAltaRotacion/")]
        public async Task<IActionResult> GetLibrosAltaRotacion ()
        {
            var librosMuyPrestados = await _context.Libros
            .Include(l=> l.Prestamos)
            .Where(l=> l.Prestamos.Count >= 10)
            .Select(l=> new GetLibrosAltaRotacionDto
            {
                Titulo = l.Titulo,
                PromedioDuracion = l.Prestamos.Average(p=> (p.FechaFin - p.FechaInicio).TotalDays),
                CantidadPrestamos = l.Prestamos.Count(),
                UsuariosDistintos = l.Prestamos.Select(p=> p.UsuarioId).Distinct().Count()
            })
            .Where(n=> n.PromedioDuracion >= 5.0 && n.UsuariosDistintos > 3)
            .OrderByDescending(l=> l.CantidadPrestamos)
            .ToListAsync();

            return Ok(BibliotecaApi2Response<ICollection<GetLibrosAltaRotacionDto>>.Ok(librosMuyPrestados));
        }

        // ==================
        // Post : api/libros
        // ==================
        [HttpPost("libro/")]
        public async Task<IActionResult> PostLibro ([FromBody] PostLibroDto Newlibro)
        {
            if(!ModelState.IsValid)
                return BadRequest(BibliotecaApi2Response<string>.Fail("ModelState Error"));

            var Libro = new Libro
            {
                Id = new Guid(),
                Titulo = Newlibro.Titulo,
                Categoria = Newlibro.Categoria,
                CantidadLibro = Newlibro.CantidadLibro,
                Disponible = Newlibro.Disponible
            };

            _context.Libros.Add(Libro);
            await _context.SaveChangesAsync();

            return Ok(Libro);
        }

        // ===================
        // Put : api/PutLibro
        // ===================
        [HttpPut("libro/{Id}")]
        public async Task<IActionResult> PutLibro ([FromBody] PutLibroDto ModLibro, Guid Id)
        {
            if(!ModelState.IsValid)
                return BadRequest(BibliotecaApi2Response<string>.Fail("ModelState Error"));

            var libroEd = await _context.Libros.Where(l=> l.Id == Id).FirstOrDefaultAsync();

            if(libroEd == null)
                return NotFound(BibliotecaApi2Response<string>.Fail("No se ha encontrado el libro."));

            libroEd.Titulo = ModLibro.Titulo;
            libroEd.Categoria = ModLibro.Categoria;
            libroEd.CantidadLibro = ModLibro.CantidadLibro;
            libroEd.Disponible = ModLibro.Disponible;

            await _context.SaveChangesAsync();

            return Ok(BibliotecaApi2Response<Libro>.Ok(libroEd , "Libro modificado correctamente."));        
        }
    }
}