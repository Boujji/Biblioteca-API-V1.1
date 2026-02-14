namespace BibliotecaApi2.Dtos
{
    public class PostUsuarioDto
    {
        public string Name { get ; set; } = null!;
        public string eMail { get ; set ; } = null!;
        public DateTime FechaRegistro { get ; set ;}
    }
}