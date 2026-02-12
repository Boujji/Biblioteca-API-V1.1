namespace BibliotecaApi2.Dtos
{
    public class LibroResponseDto
    {
        public Guid Id { get ; set; }
        public string Titulo { get ; set; } = null!;
        public string Categoria { get ; set; } = null!;
        public int CantidadLibro { get ; set; }
        public bool Disponible { get ; set; } = true;
    }
}