namespace BibliotecaApi2.Models
{
    public class Libro
    {
        public Guid Id { get ; set; }
        public string Titulo { get ; set; } = null!;
        public string Categoria { get ; set; } = null!;
        public int CantidadLibro { get ; set; }
        public ICollection<Prestamo> Prestamos { get ; set; } = new List<Prestamo>();
        public bool Disponible { get ; set; } = true;
    }
}