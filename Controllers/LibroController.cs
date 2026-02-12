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
    }
}