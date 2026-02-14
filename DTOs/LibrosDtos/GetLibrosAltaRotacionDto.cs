namespace BibliotecaApi2.Dtos
{
    public class GetLibrosAltaRotacionDto
    {
        public string Titulo { get ; set; } = null!;
        public double PromedioDuracion { get ; set; }
        public int CantidadPrestamos { get ; set; }
        public int UsuariosDistintos { get ; set; }
    }
}