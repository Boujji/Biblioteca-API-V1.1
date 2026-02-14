using BibliotecaApi2.Models;

namespace BibliotecaApi2.Dtos
{
    public class UsuarioResponseDto
    {
        public Guid Id { get ; set ;}
        public string Name { get ; set ;} = null!;
        public string eMail { get ; set ;} = null!;
        public DateTime FechaRegistro { get ; set ;}
        public decimal PenalizacionPendiente { get ; set; }
        public decimal DescNextPrest { get ; set ;}
    }
}