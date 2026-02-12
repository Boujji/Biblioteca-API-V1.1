using BibliotecaApi2.Models;

namespace BibliotecaApi2.Dtos
{
    public class PostPrestamoDto
    {
        public Guid UsuarioId { get ; set; }
        public Guid LibroId { get ; set; }
        public DateTime FechaInicio { get ; set; }
        public DateTime FechaFin { get ; set; }
        public DateTime? FechaDevolucion { get ; set; }
        public decimal CostoDiario {get ; set;}
    }
}