using BibliotecaApi2.Models;

namespace BibliotecaApi2.Dtos
{
    public class PrestamoResponseDto
    {
        public Guid Id { get ; set; }
        public Guid UsuarioId { get ; set; }
        public Usuario Usuario { get ; set; } = null!;
        public Guid LibroId { get ; set; }
        public Libro Libro { get ; set; } = null!;
        public DateTime FechaInicio { get ; set; }
        public DateTime FechaFin { get ; set; }
        public DateTime? FechaDevolucion { get ; set; }
        public decimal CostoDiario {get ; set;}
    }
}