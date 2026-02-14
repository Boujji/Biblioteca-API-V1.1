using BibliotecaApi2.Models;

namespace BibliotecaApi2.Dtos
{
    public class PutLibroDto
    {
        public string Titulo { get ; set; } = null!;
        public string Categoria { get ; set; } = null!;
        public int CantidadLibro { get ; set; }
        public bool Disponible { get ; set; } = true;
    }
}